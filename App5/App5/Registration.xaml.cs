﻿using Plugin.AutoLogin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App5
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Registration : ContentPage
    {
        public Registration()
        {
            InitializeComponent();
        }

        private async void OK_clicked(object sender, System.EventArgs e)
        {
            Reg.IsEnabled = false;
            if (CrossAutoLogin.Current.UserIsSaved == false)
            {
                CrossAutoLogin.Current.SaveUserInfos(loginEntry.Text, passEntry.Text);
            }

            Button button = (Button)sender;

            JSON json = new JSON() { type = "registration", login = loginEntry.Text, pass = passEntry.Text };
            string serialized = Newtonsoft.Json.JsonConvert.SerializeObject(json);
            string responce = Methods.Auth(serialized, actionType.reg);

            button.Text = responce;
            button.BackgroundColor = Color.Green;
            if (responce == "-1") await DisplayAlert("Error", "Подключение к Интернету отсутствует!", "OK");
            else await Navigation.PushModalAsync(new Authentification());
            Reg.IsEnabled = true;
        }
    }
}