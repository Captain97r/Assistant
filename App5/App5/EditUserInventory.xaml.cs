using Acr.UserDialogs;
using App5.AddItems;
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
	public partial class EditUserInventory : ContentPage
	{
        static Player Player { get; set; }
        static string room;
        public List<Item> Weapons { get; set; }

        public static InventoryItems items;

        public EditUserInventory (Player player, string _room)
		{
            Player = player;
            room = _room;
            Weapons = new List<Item>();

            AmmoReq req;
            req = new AmmoReq() { weapon_ids = player.weapon_ids, ammo_ids = player.ammo_ids, armor_ids = player.armor_ids };
            string post = JsonConvert.SerializeObject(req);
            InventoryItems items = JsonConvert.DeserializeObject<InventoryItems>(Methods.POST_request(post, "get-inventory"));

            foreach (Item item in items.item)
            {
                if (String.Equals(item.name, null))
                    Weapons.Add(new Item {id = item.id, name = item.caliber + "x" + item.case_length + " " + item.features, caliber = item.caliber, features=item.features, case_length = item.case_length, penetration_class = item.penetration_class });
                else
                {
                    Weapons.Add(new Item { id = item.id, name = item.name, caliber = item.caliber, cost = item.cost, country = item.country, accuracy = item.accuracy, damage = item.damage, type = item.type });
                }
            }

            
            
            this.BindingContext = this;

			InitializeComponent ();
		}

        private async void OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            Item selectedItem = e.Item as Item;
            if (selectedItem != null)
            {
                string action = await DisplayActionSheet("Действия", "Отмена", null, "Забрать :)");
                switch(action)
                {
                    case "Забрать :)":
                        UserDialogs.Instance.ShowLoading("Обновление...");
                        switch (GetItemType(selectedItem))
                        {
                            case itemType.weapon:
                                Player.weapon_ids = NewItemAction(action, Player.weapon_ids, selectedItem.id);
                                await RefreshPlayer();
                                UserDialogs.Instance.HideLoading();
                                break;
                            case itemType.armor:
                                Player.armor_ids = NewItemAction(action, Player.armor_ids, selectedItem.id);
                                await RefreshPlayer();
                                UserDialogs.Instance.HideLoading();
                                break;
                                /*
                            case itemType.helmet:
                                UserDialogs.Instance.ShowLoading("Кидаем подальше...");
                                Player.armor_ids = NewItemAction(action, Player.armor_ids, id);
                                await RefreshPlayer();
                                await DownloadInventoryItems(Player, true);
                                UserDialogs.Instance.HideLoading();
                                break;
                            
                            case itemType.loot:
                                //Player.armor_ids = newItemAction(action);
                                break;
                            case itemType.artifact:
                                //Player.armor_ids = newItemAction(action);
                                break;
                                */
                        }
                        break;
                }
            }
        }

        private async void Add_clicked(object sender, EventArgs e)
        {
            Add.IsEnabled = false;
            await Navigation.PushModalAsync(new AddItem(Player, room));
            Add.IsEnabled = true;
        }

        private itemType GetItemType(Item item)
        {
            switch (Convert.ToInt32(item.id) / 100)
            {
                case 0:
                    return itemType.weapon;
                case 1:
                    return itemType.ammo;
                case 2:
                    return itemType.armor;
                default:
                    return itemType.unknown;
            }
        }

        private string NewItemAction(string action, string str, string id)
        {
            bool founded = false;
            string[] items = str.Split(';');
            List<string> result = new List<string>();
            for (int i = 0; i < items.Length; i++)
            {
                if (!String.Equals(items[i], id)) result.Add(items[i]);
                else if (!founded)
                    founded = true;
                else
                    result.Add(items[i]);
            }
            str = String.Join(";", result);
            return str;
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