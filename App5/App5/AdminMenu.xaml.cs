using Newtonsoft.Json;
using Plugin.AutoLogin;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Acr.UserDialogs;

namespace App5
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AdminMenu : ContentPage
    {
        public AdminMenu()
        {
            InitializeComponent();
        }

        private async void join_clicked(object sender, System.EventArgs e)
        {
            button_join_game.IsEnabled = false;
            await Navigation.PushModalAsync(new RoomList(rights.admin));
            button_join_game.IsEnabled = true;
        }

        private async void create_room_clicked(object sender, System.EventArgs e)
        {
            button_create_room.IsEnabled = false;
            PromptResult room_name = await UserDialogs.Instance.PromptAsync(new PromptConfig()
            {
                CancelText = "Отмена",
                OkText = "Создать",
                MaxLength = 20,
                Title = "Создание комнаты",
                Placeholder = "Введите название",
                IsCancellable = true
            });

            if (room_name.Ok)
            {
                ROOM new_room = new ROOM() { name = room_name.Value };
                string serialized = Newtonsoft.Json.JsonConvert.SerializeObject(new_room);
                string responce = Methods.POST_request(serialized, "create_room");

                await UserDialogs.Instance.AlertAsync(responce);
            }
            button_create_room.IsEnabled = true;

        }

        private async void logout_clicked(object sender, EventArgs e)
        {
            logout_button.IsEnabled = false;

            CrossAutoLogin.Current.DeleteUserInfos();
            Methods.cookie = "";
            await Navigation.PushModalAsync(new AuthorizationMenu());

            logout_button.IsEnabled = true;

        }
        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}