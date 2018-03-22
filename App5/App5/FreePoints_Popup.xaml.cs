using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Extensions;
using Acr.UserDialogs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.ComponentModel;

namespace App5
{
	public partial class FreePoints_Popup : PopupPage
    {
        Player Player { get; set; }
        Temp temp { get; set; }
        string room;
        int current_free;
		public FreePoints_Popup (Player player, string _room)
		{
            Player = player;
            temp = new Temp();

            temp.stamina = Player.stamina;
            temp.agility = Player.agility;
            temp.intelligence = Player.intelligence;
            temp.charisma = Player.charisma;
            temp.free_points = Player.free_points;

            room = _room;
            current_free = Convert.ToInt32(temp.free_points);
            InitializeComponent();
            BindingContext = temp;
		}


        private void IncSt(object sender, EventArgs e)
        {
            if (Convert.ToInt32(temp.free_points) > 0)
            {
                temp.stamina = Convert.ToString(Convert.ToInt32(temp.stamina) + 1);
                temp.free_points = Convert.ToString(Convert.ToInt32(temp.free_points) - 1);
            }
        }

        private void DecSt(object sender, EventArgs e)
        {
            if (Convert.ToInt32(temp.stamina) > 1 && current_free > Convert.ToInt32(temp.free_points))
            {
                temp.stamina = Convert.ToString(Convert.ToInt32(temp.stamina) - 1);
                temp.free_points = Convert.ToString(Convert.ToInt32(temp.free_points) + 1);
            }
        }

        private void IncAg(object sender, EventArgs e)
        {
            if (Convert.ToInt32(temp.free_points) > 0)
            {
                temp.agility = Convert.ToString(Convert.ToInt32(temp.agility) + 1);
                temp.free_points = Convert.ToString(Convert.ToInt32(temp.free_points) - 1);
            }
        }

        private void DecAg(object sender, EventArgs e)
        {
            if (Convert.ToInt32(temp.agility) > 1 && current_free > Convert.ToInt32(temp.free_points))
            {
                temp.agility = Convert.ToString(Convert.ToInt32(temp.agility) - 1);
                temp.free_points = Convert.ToString(Convert.ToInt32(temp.free_points) + 1);
            }
        }

        private void IncInt(object sender, EventArgs e)
        {
            if (Convert.ToInt32(temp.free_points) > 0)
            {
                temp.intelligence = Convert.ToString(Convert.ToInt32(temp.intelligence) + 1);
                temp.free_points = Convert.ToString(Convert.ToInt32(temp.free_points) - 1);
            }
        }

        private void DecInt(object sender, EventArgs e)
        {
            if (Convert.ToInt32(temp.intelligence) > 1 && current_free > Convert.ToInt32(temp.free_points))
            {
                temp.intelligence = Convert.ToString(Convert.ToInt32(temp.intelligence) - 1);
                temp.free_points = Convert.ToString(Convert.ToInt32(temp.free_points) + 1);
            }
        }

        private void IncCh(object sender, EventArgs e)
        {
            if (Convert.ToInt32(temp.free_points) > 0)
            {
                temp.charisma = Convert.ToString(Convert.ToInt32(temp.charisma) + 1);
                temp.free_points = Convert.ToString(Convert.ToInt32(temp.free_points) - 1);
            }
        }

        private void DecCh(object sender, EventArgs e)
        {
            if (Convert.ToInt32(temp.charisma) > 1 && current_free > Convert.ToInt32(temp.free_points))
            {
                temp.charisma = Convert.ToString(Convert.ToInt32(temp.charisma) - 1);
                temp.free_points = Convert.ToString(Convert.ToInt32(temp.free_points) + 1);
            }
        }

        private async void Ok(object sender, EventArgs e)
        {
                Player.stamina = temp.stamina;
                Player.agility = temp.agility;
                Player.intelligence = temp.intelligence;
                Player.charisma = temp.charisma;
                Player.free_points = temp.free_points;
                CloseAllPopup();
                await RefreshPlayer();
        }


        private void OnCloseButtonTapped(object sender, EventArgs e)
        {
            CloseAllPopup();
        }

        protected override bool OnBackgroundClicked()
        {
            CloseAllPopup();
            return false;
        }

        private async void CloseAllPopup()
        {
            await Navigation.PopAllPopupAsync();
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



    public class Temp : INotifyPropertyChanged
    {
        private string _stamina;
        private string _agility;
        private string _intelligence;
        private string _charisma;
        private string _fifth_char;

        private string _free_points;


        public string stamina
        {
            get { return _stamina; }
            set
            {
                if (_stamina != value)
                {
                    _stamina = value;
                    OnPropertyChanged("stamina");
                }
            }
        }
        public string agility
        {
            get { return _agility; }
            set
            {
                if (_agility != value)
                {
                    _agility = value;
                    OnPropertyChanged("agility");
                }
            }
        }
        public string intelligence
        {
            get { return _intelligence; }
            set
            {
                if (_intelligence != value)
                {
                    _intelligence = value;
                    OnPropertyChanged("intelligence");
                }
            }
        }
        public string charisma
        {
            get { return _charisma; }
            set
            {
                if (_charisma != value)
                {
                    _charisma = value;
                    OnPropertyChanged("charisma");
                }
            }
        }
        public string fifth_char
        {
            get { return _fifth_char; }
            set
            {
                if (_fifth_char != value)
                {
                    _fifth_char = value;
                    OnPropertyChanged("fifth_char");
                }
            }
        }
        

        public string free_points
        {
            get { return _free_points; }
            set
            {
                if (_free_points != value)
                {
                    _free_points = value;
                    OnPropertyChanged("free_points");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}