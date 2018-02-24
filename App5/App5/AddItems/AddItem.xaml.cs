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

            InitializeComponent();
        }
    }

}