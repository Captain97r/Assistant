using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App5.AddItems
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddItem : TabbedPage
    {
        Player Player { get; set; }
        string room;
        public AddItem (Player player, string _room)
        {
            Player = player;
            room = _room;

            this.Children.Add(new Handguns(player, room));
            this.Children.Add(new Submachine_guns(player, room));
            this.Children.Add(new Shotguns(player, room));
            this.Children.Add(new Assault_rifles(player, room));
            this.Children.Add(new Rifles(player, room));
            this.Children.Add(new Ammo(player, room));
            this.Children.Add(new Armor(player, room));

            InitializeComponent();
        }



        public static string Insert(string str, string id)
        {
            string[] items = str.Split(';');
            List<string> result;
            if (!String.Equals(str, "")) result = new List<string>(items);
            else result = new List<string>();
            result.Add(id);
            str = String.Join(";", result);
            return str;
        }
    }

}