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
	public partial class PlayerAdditionData : TabbedPage
	{
        static Player Player { get; set; }
        static string room;

		public PlayerAdditionData (Player player, string _room)
		{
            Player = player;
            room = _room;

            this.Children.Add(new MainPage(player, _room));
            this.Children.Add(new Inventory(player, _room));
            this.Children.Add(new HUD(player, _room)); 

            InitializeComponent();
		}
	}
}