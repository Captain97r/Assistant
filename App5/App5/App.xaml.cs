using Plugin.AutoLogin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

using Xamarin.Forms;

namespace App5
{
	public partial class App : Application
	{

        public App()
        {

            InitializeComponent();

            if (CrossAutoLogin.Current.UserIsSaved)
            {
                JSON json = new JSON() { type = "authentification", login = CrossAutoLogin.Current.UserEmail, pass = CrossAutoLogin.Current.UserPassword };
                string serialized = Newtonsoft.Json.JsonConvert.SerializeObject(json);
                string responce = Methods.Auth(serialized);

                if (responce == "0")
                {
                    MainPage = new NavigationPage(new RoomList(rights.user));
                }
                else if (responce == "1")
                {
                    MainPage = new NavigationPage(new AdminMenu());
                }
            }
            else MainPage = new NavigationPage(new AuthorizationMenu());
        }
        

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
    }
}
