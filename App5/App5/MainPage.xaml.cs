using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Diagnostics;
using System.Net.WebSockets;
using Newtonsoft.Json;

namespace App5
{
    public partial class MainPage : ContentPage
    {
        public static Player Player { get; set; }

        MessageChanged messageChanged = new MessageChanged();

        SocketReq req;
        string req_serialized;
        static string room;

        public MainPage(Player _p, string _room)
        {
            room = _room;
            Player = _p;

            req = new SocketReq() {action = "get", user = Player.id, room = _room };

            req_serialized = Newtonsoft.Json.JsonConvert.SerializeObject(req);

            InitializeComponent();

            this.BindingContext = Player;

            ListenerThread listener = new ListenerThread(req_serialized);
            //ListenSocketThread listener = new ListenSocketThread("get");
        }

        private async void inventory_clicked(object sender, EventArgs e)
        {
            Inventory.IsEnabled = false;
            await Navigation.PushModalAsync(new PlayerAdditionData(Player, room));
            Inventory.IsEnabled = true;
        }

    }
    

    public class MainHandler
    {
        public static void MessageEventHandler(object sender, MsgEventArgs m)
        {
            MainPage.Player.hp = m.current_player.hp;
            MainPage.Player.radiation = m.current_player.radiation;
            MainPage.Player.hunger = m.current_player.hunger;
            MainPage.Player.drought = m.current_player.drought;
            MainPage.Player.money = m.current_player.money;
            MainPage.Player.isAlive = m.current_player.isAlive;

            MainPage.Player.weapon_ids = m.current_player.weapon_ids;
            MainPage.Player.ammo_ids = m.current_player.ammo_ids;
            MainPage.Player.armor_ids = m.current_player.armor_ids;

            MainPage.Player.active_helmet = m.current_player.active_helmet;
            MainPage.Player.active_armor = m.current_player.active_armor;
            MainPage.Player.active_boots = m.current_player.active_boots;
            MainPage.Player.active_weapon1 = m.current_player.active_weapon1;
            MainPage.Player.active_weapon2 = m.current_player.active_weapon2;

            MainPage.Player.first_char = m.current_player.first_char;
            MainPage.Player.second_char = m.current_player.second_char;
            MainPage.Player.third_char = m.current_player.third_char;
            MainPage.Player.fourth_char = m.current_player.fourth_char;
            MainPage.Player.fifth_char = m.current_player.fifth_char;

            //if (String.Equals(App.Current.MainPage.Navigation.NavigationStack.First().Title, "Инвентарь") || String.Equals(App.Current.MainPage.Navigation.NavigationStack.First().Title, "AD"))
            //{
            //
            //
        }

        //public static void InventoryChanged(object sender, MsgEventArgs e)
        //{

       // }
    }




    public class Money : IMarkupExtension
    {
        public object ProvideValue(IServiceProvider serviceProvider)
        {
            return "Деньги: " + Methods.GetProperty("money");
        }
    }

    public class Status : IMarkupExtension
    {
        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Methods.GetProperty("isAlive") == "1") return "В игре  ";
            else return "Мертв  ";
        }
    }

    public class HP : IMarkupExtension
    {
        public object ProvideValue(IServiceProvider serviceProvider)
        {
            return "HP: " + Methods.GetProperty("hp");
        }

    }

    public class Rad : IMarkupExtension
    {
        public object ProvideValue(IServiceProvider serviceProvider)
        {
            return "Радиация: " + Methods.GetProperty("radiation") + " рад";
        }
    }

    public class Hunger : IMarkupExtension
    {
        public object ProvideValue(IServiceProvider serviceProvider)
        {
            return "Голод: " + Methods.GetProperty("hunger");
        }
    }

    public class NickName : IMarkupExtension
    {
        public object ProvideValue(IServiceProvider serviceProvider)
        {
            return "  Имя: " + Methods.GetProperty("name");
        }
    }
}
