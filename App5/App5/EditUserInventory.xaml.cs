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
            req = new AmmoReq() { weapon_ids = player.weapon_ids, ammo_ids = player.ammo_ids };
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
                                /*
                            case itemType.helmet:
                                UserDialogs.Instance.ShowLoading("Кидаем подальше...");
                                Player.armor_ids = NewItemAction(action, Player.armor_ids, id);
                                await RefreshPlayer();
                                await DownloadInventoryItems(Player, true);
                                UserDialogs.Instance.HideLoading();
                                break;
                            case itemType.armor:
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
                default:
                    return itemType.unknown;
            }
        }

        private string NewItemAction(string action, string str, string id)
        {
            if (str.IndexOf(";") == -1)
            {
                if (str.Length == 1 || str.Length == 2 || str.Length == 3) str = "";
            }
            else
            {
                str = str.Substring(0, str.IndexOf(id)) + str.Substring(str.IndexOf(id) + (id).Length);
                if (str.IndexOf(";;") != -1)
                {
                    str = str.Substring(0, str.IndexOf(";;")) + str.Substring(str.IndexOf(";;") + 1);
                }
                if ((String.Equals(str.Substring(0, 1), ";")))
                {
                    str = str.Substring(1);
                }
                if ((String.Equals(str.Substring(str.Length - 1, 1), ";")))
                {
                    str = str.Substring(0, str.Length - 1);
                }
            }
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