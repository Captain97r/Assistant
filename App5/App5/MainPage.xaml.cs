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
using Acr.UserDialogs;
using Rg.Plugins.Popup.Extensions;
using System.Globalization;
using System.Reflection;
using System.ComponentModel.DataAnnotations;

namespace App5
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {
        public static Player Player { get; set; }
        FreePoints_Popup popup;

        MessageChanged messageChanged = new MessageChanged();

        SocketReq req;
        string req_serialized;
        static string room;

        public MainPage(Player _p, string _room)
        {
            room = _room;
            Player = _p;

            req = new SocketReq() { action = "get", user = Player.id, room = _room };

            req_serialized = Newtonsoft.Json.JsonConvert.SerializeObject(req);

            InitializeComponent();

            popup = new FreePoints_Popup(Player, room);
            if (Convert.ToInt32(Player.free_points) > 0)
                Navigation.PushPopupAsync(popup);

            this.BindingContext = Player;

            ListenerThread listener = new ListenerThread(req_serialized);
            //ListenSocketThread listener = new ListenSocketThread("get");
        }
        
    }


    public class MainHandler
    {
        public static void MessageEventHandler(object sender, MsgEventArgs m)
        {
            MainPage.Player.hp = Convert.ToString(Convert.ToInt32(m.current_player.hp) + Convert.ToInt32(m.current_player.stamina));
            MainPage.Player.radiation = m.current_player.radiation;
            MainPage.Player.hunger = m.current_player.hunger;
            MainPage.Player.drought = m.current_player.drought;
            MainPage.Player.money = m.current_player.money;
            MainPage.Player.isAlive = m.current_player.isAlive;
            MainPage.Player.isBleeding = m.current_player.isBleeding;

            MainPage.Player.weapon_ids = m.current_player.weapon_ids;
            MainPage.Player.ammo_ids = m.current_player.ammo_ids;
            MainPage.Player.armor_ids = m.current_player.armor_ids;

            MainPage.Player.active_helmet = m.current_player.active_helmet;
            MainPage.Player.active_armor = m.current_player.active_armor;
            MainPage.Player.active_boots = m.current_player.active_boots;
            MainPage.Player.active_weapon1 = m.current_player.active_weapon1;
            MainPage.Player.active_weapon2 = m.current_player.active_weapon2;

            MainPage.Player.stamina = m.current_player.stamina;
            MainPage.Player.agility = m.current_player.agility;
            MainPage.Player.intelligence = m.current_player.intelligence;
            MainPage.Player.charisma = m.current_player.charisma;

            MainPage.Player.free_points = m.current_player.free_points;
            
        }
    }
}

    
