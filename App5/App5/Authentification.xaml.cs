using Acr.UserDialogs;
using Plugin.AutoLogin;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App5
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Authentification : ContentPage
    {
        public Authentification()
        {
            InitializeComponent();
        }

        private async void auth_clicked(object sender, System.EventArgs e)
        {
            button_auth.IsEnabled = false;

            if (CrossAutoLogin.Current.UserIsSaved == false)
            {
                CrossAutoLogin.Current.SaveUserInfos(loginEntry.Text, passEntry.Text);
            }

            UserDialogs.Instance.ShowLoading("Авторизация...", MaskType.Black);
            string responce = await Auth();
            UserDialogs.Instance.HideLoading();

            if (responce == "0")
            {
                await Navigation.PushModalAsync(new RoomList(rights.user));
            }
            else if (responce == "1")
            {
                await Navigation.PushModalAsync(new AdminMenu());
            }
            else if (responce == "-1")
            {
                await DisplayAlert("Error", "Подключение к Интернету отсутствует!", "OK");
            }
            else
            {
                await DisplayAlert("Error", responce, "OK");
            }
            button_auth.IsEnabled = true;

        }


        private async Task<string> Auth()
        {
            return await Task.Run(() =>
             {
                 JSON json = new JSON() { type = "authentification", login = loginEntry.Text, pass = passEntry.Text };
                 string serialized = Newtonsoft.Json.JsonConvert.SerializeObject(json);
                 return Methods.Auth(serialized, actionType.auth);
             });
        }

    }

}