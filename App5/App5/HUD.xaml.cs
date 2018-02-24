using Acr.UserDialogs;
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
    public partial class HUD : ContentPage
    {
        enum drop { permanently, toBackpack };
        public Player Player { get; set; }
        public string room;
        public Dictionary<string, Item> ImageList { get; set; }
        //public static ActiveItemsReq idList { get; set; }

        public static InventoryItems items;

        public HUD (Player player, string _room)
		{
            Player = player;
            room = _room;

            Dictionary<string, Item> active_items = GetActiveItems(Player);
            ImageList = active_items;

            //idList = new ActiveItemsReq() { helmet_id = player.active_helmet, armor_id = player.active_armor, boots_id = player.active_boots, weapon1_id = player.active_weapon1, weapon2_id = player.active_weapon2 };

            this.BindingContext = ImageList;

            InitializeComponent();
        }

        private async void OnItem_Clicked(object source, EventArgs e)
        {
            Frame frame = (Frame)source;
            Image content = (Image)frame.Content;
            UriImageSource uri_source = (UriImageSource)content.Source;
            Uri uri = (Uri)uri_source.Uri;

            foreach (Item item in ImageList.Values)
            {
                if (String.Equals(uri, item.image))
                {
                    string action = await DisplayActionSheet("Действия", "Отмена", null, "Убрать в рюкзак", "Выбросить нахуй", "Информация");
                    ItemAction(action, item, uri_source.AutomationId);
                    break;
                }
            }
        }

        private async void ItemAction(string action, Item item, string slot)
        {
            switch (action)
            {
                case "Выбросить нахуй":
                    Drop(item, slot, drop.permanently);
                    break;


                case "Убрать в рюкзак":
                    Drop(item, slot, drop.toBackpack);
                    break;


                case "Информация":
                    await DisplayAlert("Информация", "Название: " + item.name + "\r\nТип: " + item.type + "\r\nСтрана: " + item.country + "\r\nКалибр: " + item.caliber + "x" + item.case_length + "\r\nТочность: " + Convert.ToDouble(item.accuracy) * 100 + "%\r\nУрон: " + item.damage + "\r\nСтоимость: " + item.cost, "ОК");
                    break;
            }
        }


        private async void Drop(Item item, string slot, drop mode)
        {
            UserDialogs.Instance.ShowLoading("Я же говорил что хуйня");
            switch (slot)
            {
                case "weapon1":
                    Player.active_weapon1 = "";
                    if (mode == drop.permanently)
                        Player.weapon_ids = Player.weapon_ids.Insert(Player.weapon_ids.Length, ";" + item.id);

                    UserDialogs.Instance.ShowLoading("Добавляем...");
                    await RefreshPlayer();
                    UserDialogs.Instance.HideLoading();

                    break;

                case "weapon2":
                    Player.active_weapon2 = "";
                    if (mode == drop.permanently)
                        Player.weapon_ids = Player.weapon_ids.Insert(Player.weapon_ids.Length, ";" + item.id);

                    UserDialogs.Instance.ShowLoading("Добавляем...");
                    await RefreshPlayer();
                    UserDialogs.Instance.HideLoading();

                    break;

                case "helmet":
                    Player.active_helmet = "";
                    //if (mode == drop.permanently)
                        //Player.weapon_ids = Player.weapon_ids.Insert(Player.weapon_ids.Length, ";" + item.id);

                    UserDialogs.Instance.ShowLoading("Добавляем...");
                    await RefreshPlayer();
                    UserDialogs.Instance.HideLoading();

                    break;

                case "armor":
                    Player.active_armor = "";
                    //if (mode == drop.permanently)
                        //Player.weapon_ids = Player.weapon_ids.Insert(Player.weapon_ids.Length, ";" + item.id);

                    UserDialogs.Instance.ShowLoading("Добавляем...");
                    await RefreshPlayer();
                    UserDialogs.Instance.HideLoading();

                    break;

                case "boots":
                    Player.active_boots = "";
                    //if (mode == drop.permanently)
                        //Player.weapon_ids = Player.weapon_ids.Insert(Player.weapon_ids.Length, ";" + item.id);

                    UserDialogs.Instance.ShowLoading("Добавляем...");
                    await RefreshPlayer();
                    UserDialogs.Instance.HideLoading();
                    
                    break;
            }
            await RefreshPlayer();
            UserDialogs.Instance.HideLoading();
        }


        private Dictionary<string, Item> GetActiveItems(Player player)
        {
            ActiveItemsReq req = new ActiveItemsReq() { helmet_id = player.active_helmet, armor_id = player.active_armor, boots_id = player.active_boots, weapon1_id = player.active_weapon1, weapon2_id=player.active_weapon2};
            string post = JsonConvert.SerializeObject(req);
            string resp = Methods.POST_request(post, "get-active-items", room );
            //ActiveItemsReq active_items = JsonConvert.DeserializeObject<ActiveItemsReq>(resp);
            //return active_items;

            Dictionary<string, Item> activeItemList = JsonConvert.DeserializeObject<Dictionary<string, Item>>(resp);
            return activeItemList;
            
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