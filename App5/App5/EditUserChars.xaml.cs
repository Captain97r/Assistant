using Acr.UserDialogs;
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
    public partial class EditUserChars : ContentPage
    {
        static Player Player { get; set; }
        static string room;
        private double stepValue = 1.0;

        public EditUserChars(Player player, string _room)
        {
            room = _room;
            Player = player;

            InitializeComponent();

            this.BindingContext = Player;
        }

        async void Apply_Clicked(object sender, EventArgs e)
        {
            UserDialogs.Instance.ShowLoading("Обновление...");
            await RefreshPlayer();
            UserDialogs.Instance.HideLoading();
        }





        async void isAliveChange_Clicked(object sender, EventArgs e)
        {
            UserDialogs.Instance.ShowLoading("Обновление...");
            await RefreshPlayer();
            UserDialogs.Instance.HideLoading();
        }

        async void LightWound_Clicked(object sender, EventArgs e)
        {
            UserDialogs.Instance.ShowLoading("Обновление...");
            Player.hp = Convert.ToString(Convert.ToInt32(Player.hp) - 10);
            await RefreshPlayer();
            UserDialogs.Instance.HideLoading();
        }

        async void RegularWound_Clicked(object sender, EventArgs e)
        {
            UserDialogs.Instance.ShowLoading("Обновление...");
            Player.hp = Convert.ToString(Convert.ToInt32(Player.hp) - 25);
            await RefreshPlayer();
            UserDialogs.Instance.HideLoading();
        }

        async void HeavyWound_Clicked(object sender, EventArgs e)
        {
            UserDialogs.Instance.ShowLoading("Обновление...");
            Player.hp = Convert.ToString(Convert.ToInt32(Player.hp) - 50);
            await RefreshPlayer();
            UserDialogs.Instance.HideLoading();
        }

        async void Anomaly_Clicked(object sender, EventArgs e)
        {
            UserDialogs.Instance.ShowLoading("Обновление...");
            Player.hp = Convert.ToString(Convert.ToInt32(Player.hp) - 15);
            Player.radiation = Convert.ToString(Convert.ToInt32(Player.radiation) + 3);
            await RefreshPlayer();
            UserDialogs.Instance.HideLoading();
        }






        void OnHpChanged(object sender, ValueChangedEventArgs e)
        {
            var newStep = Math.Round(e.NewValue / stepValue);
            var val = newStep * stepValue;
            sliderHp.Value = val;
            hp.Text = String.Format("Здоровье: {0}", e.NewValue);
            Player.hp = Convert.ToString(val);
        }

        void OnRadChanged(object sender, ValueChangedEventArgs e)
        {
            var newStep = Math.Round(e.NewValue / stepValue);
            var val = newStep * stepValue;
            sliderRad.Value = val;
            rad.Text = String.Format("Радиация: {0}", e.NewValue);
            Player.radiation = Convert.ToString(val);
        }

        void OnHungerChanged(object sender, ValueChangedEventArgs e)
        {
            var newStep = Math.Round(e.NewValue / stepValue);
            var val = newStep * stepValue;
            sliderHunger.Value = val;
            hunger.Text = String.Format("Голод: {0}", e.NewValue);
            Player.hunger = Convert.ToString(val);
        }




        private Task RefreshPlayer()
        {
            return Task.Run(() =>
            {
                string serialized = JsonConvert.SerializeObject(Player);
                Methods.POST_request(serialized, "update-user-info", room);
            });
        }
    }
}