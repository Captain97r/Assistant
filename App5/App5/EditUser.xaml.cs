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
    public partial class EditUser : TabbedPage
    {
        public EditUser (Player player, string room)
        {
            this.Children.Add(new EditUserChars(player, room));
            this.Children.Add(new EditUserInventory(player, room));
            InitializeComponent();
        }
    }
}