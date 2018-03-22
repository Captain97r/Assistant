using Newtonsoft.Json;
using Plugin.AutoLogin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Acr.UserDialogs;

namespace App5
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RoomList : ContentPage
    {
        public List<Room> Rooms { get; set; }

        rights right = rights.user;

        public RoomList(rights _right)
        {
            right = _right;
            Rooms = new List<Room>();

            string[] room_list = Methods.GetRoomList();

            foreach (string room in room_list)
            {
                if (String.Equals(room, "")) continue;
                ROOM new_room = new ROOM() { name = room };
                string serialized = Newtonsoft.Json.JsonConvert.SerializeObject(new_room);
                string responce = Methods.POST_request(serialized, "room_info");
                Room deserialized = Newtonsoft.Json.JsonConvert.DeserializeObject<Room>(responce);
                Rooms.Add(new Room {  id = deserialized.id, name = deserialized.name, status = deserialized.status });
            }

            this.BindingContext = this;

            InitializeComponent();
        }


        private async void logout_clicked(object sender, EventArgs e)
        {
            logout_button.IsEnabled = false;
            CrossAutoLogin.Current.DeleteUserInfos();
            Methods.cookie = "";
            await Navigation.PushModalAsync(new AuthorizationMenu());
            logout_button.IsEnabled = true;
        }

        public async void OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            
            Room selectedRoom = e.Item as Room;
            if (selectedRoom != null)
            {
                UserDialogs.Instance.ShowLoading("Загрузка...", MaskType.Gradient);
                string responce = await OpenRoom(e);
                UserDialogs.Instance.HideLoading();

                Player player = JsonConvert.DeserializeObject<Player>(responce);

                if ((!String.Equals(responce, "nioh")) && right == rights.user)
                {
                    await Navigation.PushModalAsync(new PlayerAdditionData(player, selectedRoom.name));
                }
                else if ((!String.Equals(responce, "nioh")) && right == rights.admin)
                {
                    await Navigation.PushModalAsync(new AdminPage(selectedRoom.name));
                }
                else
                {
                    await DisplayAlert("Error", responce, "OK");
                }
            }
            
        }

        private async Task<string> OpenRoom(ItemTappedEventArgs e)
        {
            Room selectedRoom = e.Item as Room;
            return await Task.Run(() =>
            {
                string serialized = Newtonsoft.Json.JsonConvert.SerializeObject(selectedRoom);
                string responce = Methods.POST_request(serialized, "select_room");
                return responce;
            });
        }

        
        protected override bool OnBackButtonPressed()
        {
            if (right == rights.user) return true;
            else
            {
                Navigation.PopModalAsync();
                return true;
            }

        }
    }
    /*
    public class RoomId : IMarkupExtension
    {
        public object ProvideValue(IServiceProvider serviceProvider)
        {
            Room room = new Room { Name = Methods.GET_request("room_list"), Id = 1, Status = "waiting" };
            return room.Id;
        }
    }

    public class RoomName : IMarkupExtension
    {
        public object ProvideValue(IServiceProvider serviceProvider)
        {
            Room room = new Room { Name = Methods.GET_request("room_list"), Id = 1, Status = "waiting" };
            return room.Name;
        }
    }

    public class RoomStatus : IMarkupExtension
    {
        public object ProvideValue(IServiceProvider serviceProvider)
        {
            Room room = new Room { Name = Methods.GET_request("room_list"), Id = 1, Status = "waiting" };
            return room.Status;
        }
    }
*/
    public class Room
    {
        public int id { get; set; }
        public string name { get; set; }
        public string status { get; set; }
    }
    
    public class CustomCell : ExtendedViewCell
    {
        Label id, name, status;
        

        public CustomCell()
        {
            id = new Label { FontSize = 18, Margin = new Thickness(19, 10 , 0, 0) };
            name = new Label { FontSize = 18, Margin = new Thickness(19, 10, 0, 0) };
            status = new Label { FontSize = 18, Margin = new Thickness(19, 10, 0, 0) };

            Grid grid = new Grid
            {
                RowDefinitions =
                {
                    new RowDefinition{Height = new GridLength(1, GridUnitType.Star)}
                },
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                },
                ColumnSpacing = -3,
                RowSpacing = -3
                
            };

            grid.Children.Add(id, 0, 0);
            grid.Children.Add(name, 1, 0);
            grid.Children.Add(status, 2, 0);
            
            View = grid;
        }

        public static readonly BindableProperty IDProperty =
            BindableProperty.Create("ID", typeof(string), typeof(CustomCell), "");

        public static readonly BindableProperty NameProperty =
            BindableProperty.Create("Name", typeof(string), typeof(CustomCell), "");

        public static readonly BindableProperty StatusProperty =
            BindableProperty.Create("Status", typeof(string), typeof(CustomCell), "");

        public string ID
        {
            get { return (string)GetValue(IDProperty); }
            set { SetValue(IDProperty, value); }
        }

        public string Name
        {
            get { return (string)GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
        }
        public string Status
        {
            get { return (string)GetValue(StatusProperty); }
            set { SetValue(StatusProperty, value); }
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            if (BindingContext != null)
            {
                id.Text = ID;
                name.Text = Name;
                status.Text = Status;
            }
        }
    }


}