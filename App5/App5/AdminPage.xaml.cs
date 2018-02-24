using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App5
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AdminPage : ContentPage
    {
        public PlayerList PlayerList { get; set; }
        string room = "";
        public AdminPage(string _room)
        {

            room = _room;

            ROOM json = new ROOM() { name = room };
            string responce = Methods.POST_request(JsonConvert.SerializeObject(json), "get-users");

            PlayerList playerList = JsonConvert.DeserializeObject<PlayerList>(responce);
            PlayerList = playerList;
            
            this.BindingContext = this.PlayerList;

            InitializeComponent();
        }

        public async void OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            Player selectedPlayer = e.Item as Player;
            if (selectedPlayer != null)
            {
                string action = await DisplayActionSheet("Действия", "Отмена", null, "Информация", "Редактировать", "Кикнуть", "Забанить");
                PlayerAction(action, selectedPlayer);
            }
        }

        private async void PlayerAction(string action, Player player)
        {
            switch(action)
            {
                case "Информация":
                    await DisplayAlert("Информация", "Имя: " + player.user + "\r\nЗдоровье: " + player.hp + "\r\nРадиация: " + player.radiation + "\r\nГолод: " + player.hunger + "\r\n" + player.isAlive, "ОК");
                    break;
                case "Редактировать":
                    await Navigation.PushModalAsync(new EditUser(player, room));
                    break;

            }
        }

    }

    public class ExtendedViewCell : ViewCell
    {
        public static BindableProperty SelectedBackgroundColorProperty =
            BindableProperty.Create("SelectedBackgroundColor",
                                    typeof(Color),
                                    typeof(ExtendedViewCell),
                                    Color.Blue);

        public Color SelectedBackgroundColor
        {
            get { return (Color)GetValue(SelectedBackgroundColorProperty); }
            set { SetValue(SelectedBackgroundColorProperty, value); }
        }
    }
}