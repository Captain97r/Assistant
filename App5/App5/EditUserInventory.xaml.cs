﻿using Acr.UserDialogs;
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
                    Weapons.Add(new Item {name = item.caliber + "x" + item.case_length + " " + item.features, caliber = item.caliber, features=item.features, case_length = item.case_length, penetration_class = item.penetration_class });
                else
                {
                    Weapons.Add(new Item { name = item.name, caliber = item.caliber, cost = item.cost, country = item.country, accuracy = item.accuracy, damage = item.damage, type = item.type });
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

        private async void DropWeapon(Item item)
        {
            if (Player.weapon_ids.Length == 1 || Player.weapon_ids.Length == 2) Player.weapon_ids = "";
            else
            {
                Player.weapon_ids = Player.weapon_ids.Substring(0, Player.weapon_ids.IndexOf(item.id)) + Player.weapon_ids.Substring(Player.weapon_ids.IndexOf(item.id) + (item.id).Length);
                if (Player.weapon_ids.IndexOf(";;") != -1)
                {
                    Player.weapon_ids = Player.weapon_ids.Substring(0, Player.weapon_ids.IndexOf(";;")) + Player.weapon_ids.Substring(Player.weapon_ids.IndexOf(";;") + 1);
                }
                if ((String.Equals(Player.weapon_ids.Substring(0, 1), ";")))
                {
                    Player.weapon_ids = Player.weapon_ids.Substring(1);
                }
                if ((String.Equals(Player.weapon_ids.Substring(Player.weapon_ids.Length - 1, 1), ";")))
                {
                    Player.weapon_ids = Player.weapon_ids.Substring(0, Player.weapon_ids.Length - 1);
                }
            }
            UserDialogs.Instance.ShowLoading("Забираем...");
            await RefreshPlayer();
            UserDialogs.Instance.HideLoading();
        }


        private async void DropAmmo(Item item)
        {
            if (Player.ammo_ids.Length == 3) Player.ammo_ids = "";
            else
            {
                Player.ammo_ids = Player.ammo_ids.Substring(0, Player.ammo_ids.IndexOf(item.id)) + Player.ammo_ids.Substring(Player.ammo_ids.IndexOf(item.id) + (item.id).Length);
                if (Player.ammo_ids.IndexOf(";;") != -1)
                {
                    Player.ammo_ids = Player.ammo_ids.Substring(0, Player.ammo_ids.IndexOf(";;")) + Player.ammo_ids.Substring(Player.ammo_ids.IndexOf(";;") + 1);
                }
                if ((String.Equals(Player.ammo_ids.Substring(0, 1), ";")))
                {
                    Player.ammo_ids = Player.ammo_ids.Substring(1);
                }
                if ((String.Equals(Player.ammo_ids.Substring(Player.ammo_ids.Length - 1, 1), ";")))
                {
                    Player.ammo_ids = Player.ammo_ids.Substring(0, Player.ammo_ids.Length - 1);
                }
            }
            UserDialogs.Instance.ShowLoading("Забираем...");
            await RefreshPlayer();
            UserDialogs.Instance.HideLoading();
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