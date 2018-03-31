using Acr.UserDialogs;
using Newtonsoft.Json;
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
	public partial class Armor : ContentPage
	{

        public List<Item> Items { get; set; }
        public Player Player { get; set; }
        public string room;

        public Armor (Player player, string _room)
		{
            Player = player;
            room = _room;

            InventoryItems listItem = new InventoryItems();

            ItemReq req = new ItemReq() { type = "armor"};

            string post = JsonConvert.SerializeObject(req);
            listItem = JsonConvert.DeserializeObject<InventoryItems>(Methods.POST_request(post, "get-item-list-by-type"));

            Items = new List<Item>();

            foreach (Item item in listItem.item)
            {
                Items.Add(new Item { id = item.id, name = item.name, groupment = item.groupment, cost = item.cost, penetration_class = item.penetration_class, radio_protection = item.radio_protection, temp_protection = item.temp_protection, electric_protection = item.electric_protection, chemic_protection = item.chemic_protection, psy_protection = item.psy_protection, containers = item.containers, PNV = item.PNV, weight = item.weight });
            }

            this.BindingContext = this;

            InitializeComponent();
        }


        private async void Item_Clicked(object source, ItemTappedEventArgs e)
        {
            Item selectedItem = e.Item as Item;
            if (selectedItem != null)
            {
                UserDialogs.Instance.ShowLoading("Добавляем...");
                Player.armor_ids = AddItem.Insert(Player.armor_ids, selectedItem.id);
                await RefreshPlayer();
                UserDialogs.Instance.HideLoading();
            }
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