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

            sliderHp.Maximum = 80 + Convert.ToInt32(Player.stamina) * 2;

            this.BindingContext = Player;
        }

        async void Apply_Clicked(object sender, EventArgs e)
        {
            UserDialogs.Instance.ShowLoading("Обновление...");
            Player.stamina = st.Value;
            Player.agility = ag.Value;
            Player.intelligence = i.Value;
            Player.charisma = ch.Value;
            Player.free_points = free.Value;
            Player.money = money.Text;
            Player.hand_p = hand.Value;
            Player.sub_p = sub.Value;
            Player.shot_p = shot.Value;
            Player.rifle_p = rifle.Value;
            Player.assault_p = assault.Value;
            Player.sniper_p = sniper.Value;
            await RefreshPlayer();
            UserDialogs.Instance.HideLoading();
        }





        async void isAliveChange_Clicked(object sender, EventArgs e)
        {
            UserDialogs.Instance.ShowLoading("Обновление...");
            Player.hp = (Convert.ToInt32(Player.hp) > 0) ? "0" : "100";
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

    public class MyStepper : ContentView
    {
        Label name, value;
        Button plus, minus;
        
        public MyStepper()
        {
            name = new Label { FontSize = 18, Margin = new Thickness(19, 10, 0, 0), TextColor = Color.Black };
            value = new Label { FontSize = 18, Margin = new Thickness(19, 10, 0, 0), TextColor = Color.Black };
            plus = new Button() { Text = "+", HorizontalOptions = LayoutOptions.Center, TextColor = Color.Black };
            minus = new Button() { Text = "-", HorizontalOptions = LayoutOptions.Center, TextColor = Color.Black };
            plus.Clicked += OnPlusButtonClicked;
            minus.Clicked += OnMinusButtonClicked;

            Grid grid = new Grid
            {
                RowDefinitions =
                {
                    new RowDefinition{Height = new GridLength(1, GridUnitType.Star)}
                },
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                },
                ColumnSpacing = -3,
                RowSpacing = -3

            };
            grid.Children.Add(name, 0, 0);
            grid.Children.Add(minus, 1, 0);
            grid.Children.Add(value, 2, 0);
            grid.Children.Add(plus, 3, 0);

            Content = grid;
        }
        
        public static readonly BindableProperty NameProperty =
            BindableProperty.Create("Name", typeof(string), typeof(ContentView), "");

        public static readonly BindableProperty ValueProperty =
            BindableProperty.Create("Value", typeof(string), typeof(ContentView), "");
        
        public string Name
        {
            get { return (string)GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
        }
        public string Value
        {
            get { return (string)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            if (BindingContext != null)
            {
                name.Text = Name;
                value.Text = Value;
            }
        }

        void OnPlusButtonClicked(object sender, EventArgs e)
        {
            Value = Convert.ToString(Convert.ToInt32(Value) + 1);
            OnBindingContextChanged();
        }

        void OnMinusButtonClicked(object sender, EventArgs e)
        {
            Value = Convert.ToString(Convert.ToInt32(Value) - 1);
            OnBindingContextChanged();
        }
    }

}