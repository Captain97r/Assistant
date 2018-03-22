using Acr.UserDialogs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        Dictionary<string, Item> active_items { get; set; }
        //public static ActiveItemsReq idList { get; set; }

        public static InventoryItems items;

        public HUD (Player player, string _room)
		{
            Player = player;
            room = _room;

            active_items = GetActiveItems(Player);
            ImageList = active_items;

            //idList = new ActiveItemsReq() { helmet_id = player.active_helmet, armor_id = player.active_armor, boots_id = player.active_boots, weapon1_id = player.active_weapon1, weapon2_id = player.active_weapon2 };

            this.BindingContext = ImageList;

            Player.PropertyChanged += new PropertyChangedEventHandler(Rebuild);

            InitializeComponent();
        }

        private async void Rebuild(object sender, PropertyChangedEventArgs e)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                await RefillFields();
            });
        }

        private async Task RefillFields()
        {
            UserDialogs.Instance.ShowLoading("Обновление...");
            await Refresh();

            try { if (ImageList.ContainsKey("weapon1")) { weapon1_frame.Content = new Image() { Source = ImageSource.FromUri(new Uri(ImageList["weapon1"].image)), Rotation = 270, AutomationId= "weapon1", Scale = 1.5 }; } } catch(NullReferenceException) {  } catch (KeyNotFoundException) { weapon1_frame.Content = null; }
            try { if (ImageList.ContainsKey("weapon2")) { weapon2_frame.Content = new Image() { Source = ImageSource.FromUri(new Uri(ImageList["weapon2"].image)), Rotation = 270, AutomationId = "weapon2", Scale = 1.5 }; }  } catch (NullReferenceException) {  } catch (KeyNotFoundException) { weapon2_frame.Content = null; }
            try { if (ImageList.ContainsKey("helmet")) {helmet_frame.Content = new Image() { Source = ImageSource.FromUri(new Uri(ImageList["helmet"].image)), Rotation = 270, AutomationId = "helmet", Scale = 1.5 }; }  } catch(NullReferenceException) {  } catch (KeyNotFoundException) { helmet_frame.Content = null; }
            try { if (ImageList.ContainsKey("armor")) {armor_frame.Content = new Image() { Source = ImageSource.FromUri(new Uri(ImageList["armor"].image)), Rotation = 270, AutomationId = "armor", Scale = 1.5 }; }  } catch(NullReferenceException) {  } catch (KeyNotFoundException) { armor_frame.Content = null; } 
            try { if (ImageList.ContainsKey("boots")) {boots_frame.Content = new Image() { Source = ImageSource.FromUri(new Uri(ImageList["boots"].image)), Rotation = 270, AutomationId = "boots", Scale = 1.5 }; } } catch(NullReferenceException) {  } catch (KeyNotFoundException) { boots_frame.Content = null; }
            UserDialogs.Instance.HideLoading();
        }

        private async Task Refresh()
        {
                active_items = GetActiveItems(Player);
                ImageList = active_items;
        }

        private async void OnItem_Clicked(object source, EventArgs e)
        {
            try
            {
                Frame frame = (Frame)source;
                Image content = (Image)frame.Content;

                if (content.Source != null)
                {
                    UriImageSource uri_source = (UriImageSource)content.Source;
                    Uri uri = (Uri)uri_source.Uri;

                    foreach (Item item in ImageList.Values)
                    {
                        if (item != null)
                        {
                            if (String.Equals(uri, item.image))
                            {
                                string action = await DisplayActionSheet(item.name, "Отмена", null, "Убрать в рюкзак", "Выбросить нахуй", "Информация");
                                ItemAction(action, item, content.AutomationId, frame);
                                break;
                            }
                        }
                    }
                }
            } 
            catch(System.NullReferenceException)
            {
                await DisplayAlert ("Ты заебал", "Тут вообще-то пусто", "ОК");
            }
        }

        private async void ItemAction(string action, Item item, string slot, Frame frame)
        {
            switch (action)
            {
                case "Выбросить нахуй":
                    Drop(item, slot, drop.permanently);

                    //frame.Content = null;
                    break;


                case "Убрать в рюкзак":
                    Drop(item, slot, drop.toBackpack);
                    
                    //frame.Content = null;
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
                    if (mode == drop.toBackpack)
                        Player.weapon_ids = Player.weapon_ids.Insert(Player.weapon_ids.Length, ";" + item.id);
                    weapon1_frame.Content = null;

                    break;

                case "weapon2":
                    Player.active_weapon2 = "";
                    if (mode == drop.toBackpack)
                        Player.weapon_ids = Player.weapon_ids.Insert(Player.weapon_ids.Length, ";" + item.id);
                    weapon2_frame.Content = null;

                    break;

                case "helmet":
                    Player.active_helmet = "";
                    //if (mode == drop.toBackpack)
                    //Player.weapon_ids = Player.weapon_ids.Insert(Player.weapon_ids.Length, ";" + item.id);

                    break;

                case "armor":
                    Player.active_armor = "";
                    //if (mode == drop.toBackpack)
                    //Player.weapon_ids = Player.weapon_ids.Insert(Player.weapon_ids.Length, ";" + item.id);

                    break;

                case "boots":
                    Player.active_boots = "";
                    //if (mode == drop.toBackpack)
                    //Player.weapon_ids = Player.weapon_ids.Insert(Player.weapon_ids.Length, ";" + item.id);

                    break;

                default:
                    await DisplayAlert("", slot, "OK");
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