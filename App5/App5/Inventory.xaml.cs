using Acr.UserDialogs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App5
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Inventory : ContentPage
    {
        static string room;
        public InventoryItems items;
        public Player Player;
        public GridUnitType HEIGHT = (Xamarin.Forms.GridUnitType)(Application.Current.MainPage.Height / 300);
        public GridUnitType WIDTH = (Xamarin.Forms.GridUnitType)(Application.Current.MainPage.Width / 200);

        public Inventory(Player player, string _room)
        {
            Player = new Player();                                                                                                              //!
            room = _room;
            Player = player;

            Player.PropertyChanged += new PropertyChangedEventHandler(Rebuild);

            InitializeComponent();
            
            DownloadInventoryItems(player, false);

        }

        private async void Rebuild(object sender, PropertyChangedEventArgs e)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                UserDialogs.Instance.ShowLoading("Обновление...");
                await DownloadInventoryItems(Player, true);
                UserDialogs.Instance.HideLoading();
            });
        }

        private async Task DownloadInventoryItems(Player player, bool isReBuild)                                                                //main drawing item grid method
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                UserDialogs.Instance.ShowLoading("Загрузка...");
                InventoryItems itemList = await GetInventory(player.weapon_ids, player.ammo_ids, player.armor_ids);
            
                List<Uri> uriList = await GetImages(itemList);
            
                int num = uriList.Count;
                int column_num = 3;

                CreateGrid(num, column_num, itemList, uriList, isReBuild);
                UserDialogs.Instance.HideLoading();
            });
        }

        private Task<InventoryItems> GetInventory(string weapons, string ammo, string armor)                                                                  //getting item info for every single one for user
        {
            AmmoReq req;

            return Task.Run(() =>
            {
                req = new AmmoReq() { weapon_ids = weapons, ammo_ids = ammo, armor_ids = armor };
                string post = JsonConvert.SerializeObject(req);
                items = JsonConvert.DeserializeObject<InventoryItems>(Methods.POST_request(post, "get-inventory"));
                return items;
            });
        }

        private static Task<List<Uri>> GetImages(InventoryItems items)                                                                          //getting just image URLs for every item
        {
            List<Uri> uris = new List<Uri>();

            return Task.Run(() =>
            {
                foreach (Item item in items.item)
                {
                    uris.Add(new Uri(item.image));
                }
                return uris;
            });
        }


        private void CreateGrid(int num, int column_num, InventoryItems itemList, List<Uri> uriList, bool isReBuild)                            //building a grid
        {
            if (isReBuild)                                                                                                                      //if grid is now rebuilding, clear all grid cells
            {
                armor_grid.ColumnDefinitions.Clear();
                armor_grid.RowDefinitions.Clear();
                armor_grid.Children.Clear();

                weapon_grid.ColumnDefinitions.Clear();
                weapon_grid.RowDefinitions.Clear();
                weapon_grid.Children.Clear();

                ammo_grid.ColumnDefinitions.Clear();
                ammo_grid.RowDefinitions.Clear();
                ammo_grid.Children.Clear();

                trunc_grid.ColumnDefinitions.Clear();
                trunc_grid.RowDefinitions.Clear();
                trunc_grid.Children.Clear();
            }
            for (int i = 0; i < column_num; i++)                                                                                                //adding columns
            {
                armor_grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                weapon_grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                ammo_grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                trunc_grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            }

            //armor_grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            //weapon_grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            //ammo_grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            //trunc_grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });



            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            ////////////////////////////////////////////////////////////////////////////     VARIABLE DEFINITIONS     /////////////////////////////////////////////////////////////////////////////////////////////////////////
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


            List<int> free_column = new List<int>();                                                                                            //arrays for free cells
            List<int> free_row = new List<int>() ;

            List<int> busy_column = new List<int>();                                                                                            // arrays for busy cells
            List<int> busy_row = new List<int>();

            int item_id = 0;                                                                                                                    //current pos x, y and current item id

            int armor_c = 0, armor_r = 0;
            int weapon_c = 0, weapon_r = 0;
            int ammo_c = 0, ammo_r = 0;
            int trunc_c = 0, trunc_r = 0;


///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////      CREATING FRAMES     /////////////////////////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


            foreach (Uri uri in uriList)                                                                                                        //building an image
            {
                Image image = new Image() { Scale = 1.5};
                image.Source = new UriImageSource
                {
                    CachingEnabled = true,
                    Uri = uri,
                    AutomationId = itemList.item[item_id].id
                };

                Frame frame = new Frame()                                                                                                       //building frames
                {
                    Content = image,
                    OutlineColor = Color.White,
                    HeightRequest = 20
                };

                var gestureRecognizer = new TapGestureRecognizer();                                                                             //Adding gesture recognizer for every single frame
                gestureRecognizer.Tapped += (o, e) => OnItemTapped(o, e);

                frame.GestureRecognizers.Add(gestureRecognizer);


///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////      ITEMS DISTRIBUTION     /////////////////////////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



                if (Convert.ToInt32(itemList.item[item_id].id) > 200 && Convert.ToInt32(itemList.item[item_id].id) < 300)                        //filling grid with "large" (2x3) items
                {
                    armor_grid.Children.Add(frame, armor_c, armor_r);
                    //Grid.SetColumnSpan(frame, 2);
                    frame.HeightRequest *= 4;

                    armor_c++;
                    if (armor_c == column_num)
                    {
                        armor_r ++;
                        armor_c = 0;
                        armor_grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, HEIGHT) });
                    }
                }
                else if (Convert.ToInt32(itemList.item[item_id].id) > 10 && Convert.ToInt32(itemList.item[item_id].id) < 100)                        //filling grid with "long" (2x1) items
                {
                    if (weapon_c == column_num - 1) { free_column.Add(weapon_c); free_row.Add(weapon_r); weapon_c = 0; weapon_r++; }

                    weapon_grid.Children.Add(frame, weapon_c, weapon_r);
                    Grid.SetColumnSpan(frame, 2);

                    weapon_c++;
                    weapon_c++;
                    if (weapon_c == column_num)
                    {
                        weapon_r++;
                        weapon_c = 0;
                        weapon_grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, HEIGHT) });
                    }
                }
                else if ((free_column.Count == 0) && (Convert.ToInt32(itemList.item[item_id].id) < 11) && (Convert.ToInt32(itemList.item[item_id].id) > 0))
                {
                    weapon_grid.Children.Add(frame, weapon_c, weapon_r);
                    weapon_c++;
                    if (weapon_c == column_num)
                    {
                        weapon_r++;
                        weapon_c = 0;
                        weapon_grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, HEIGHT) });
                    }
                }
                else if (Convert.ToInt32(itemList.item[item_id].id) > 100 && Convert.ToInt32(itemList.item[item_id].id) < 200)
                {
                    ammo_grid.Children.Add(frame, ammo_c, ammo_r);                                                                        
                    ammo_c++;
                    if (ammo_c == column_num)
                    {
                        ammo_r++;
                        ammo_c = 0;
                        ammo_grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, HEIGHT) });
                    }
                }
                else if (free_column.Count != 0)                                                                                                //filling free cells with "short" items
                {
                    weapon_grid.Children.Add(frame, free_column[0], free_row[0]);
                    free_column.RemoveAt(0);
                    free_row.RemoveAt(0);
                    free_column.Add(weapon_c);
                    free_row.Add(weapon_r);
                }

                item_id++;
            }
            if (armor_r == 0 && armor_c == 0) armor.HeightRequest = 0;
            else armor.HeightRequest = 30;
            if (weapon_r == 0 && weapon_c == 0) weapon.HeightRequest = 0;
            else weapon.HeightRequest = 30;
            if (ammo_r == 0 && ammo_c == 0) ammo.HeightRequest = 0;
            else ammo.HeightRequest = 30;
            if (trunc_r == 0 && trunc_c == 0) trunc.HeightRequest = 0;
            else trunc.HeightRequest = 30;
        }


        private async void OnItemTapped(object ob, EventArgs e)                                                                                 //tap handler
        {
            Frame frame = (Frame)ob;
            Image content = (Image)frame.Content;
            UriImageSource source = (UriImageSource)content.Source;
            string id = source.AutomationId;

            foreach (Item item in items.item)
            {
                if (String.Equals(id, item.id) && (GetItemType(item) == itemType.ammo))
                {
                    string action = await DisplayActionSheet(item.caliber + "x" + item.case_length + " (" + item.features + ")\r\n", "Отмена", null, "\r\nВыбросить", "Информация");
                    ItemTypeDefinitor(action, item, item.id);
                    break;
                }
                else if (String.Equals(id, item.id))
                {
                    string action = await DisplayActionSheet(item.name + "\r\n", "Отмена", null, "Надеть", "Выбросить", "Информация");
                    ItemTypeDefinitor(action, item, item.id);
                    break;
                }
            }
        }
        

        private async void ItemTypeDefinitor(string action, Item item, string id)
        {
            switch(action)
            {
                case "Надеть":
                    UserDialogs.Instance.ShowLoading("Надеваем...");
                    switch (GetItemType(item))
                    {
                        case itemType.weapon:

                            if (String.Equals(Player.active_weapon1, ""))
                            {
                                Player.active_weapon1 = id;
                            }
                            else if (String.Equals(Player.active_weapon2, ""))
                            {
                                Player.active_weapon2 = id;
                            }
                            else
                            {
                                await DisplayAlert("Ошибка", "Некуда ебать ложить!", "Иди нахуй");
                                UserDialogs.Instance.HideLoading();
                                break;
                            }
                            Player.weapon_ids = NewItemAction(action, Player.weapon_ids, id);
                            await RefreshPlayer();
                            await DownloadInventoryItems(Player, true);
                            UserDialogs.Instance.HideLoading();
                            break;

                        case itemType.armor:
                            UserDialogs.Instance.ShowLoading("Надеваем...");

                            if (String.Equals(Player.active_armor, ""))
                            {
                                Player.active_armor = id;
                            }
                            else
                            {
                                await DisplayAlert("Ошибка", "Некуда ебать ложить!", "Иди нахуй");
                                UserDialogs.Instance.HideLoading();
                                break;
                            }

                            Player.armor_ids = NewItemAction(action, Player.armor_ids, id);
                            await RefreshPlayer();
                            await DownloadInventoryItems(Player, true);
                            UserDialogs.Instance.HideLoading();
                            break;
                            /*
                        case itemType.helmet:
                            UserDialogs.Instance.ShowLoading("Надеваем...");
                            await RefreshPlayer();
                            await DownloadInventoryItems(Player, true);
                            Player.armor_ids = NewItemAction(action, Player.armor_ids, id);
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
                case "Выбросить":
                    UserDialogs.Instance.ShowLoading("Кидаем подальше...");
                    switch (GetItemType(item))
                    {
                        case itemType.weapon:
                            Player.weapon_ids = NewItemAction(action, Player.weapon_ids, id);
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
                case "Информация":
                    if (Convert.ToInt32(item.id) < 100)
                        await DisplayAlert("Информация", "Название: " + item.name + "\r\nТип: " + item.type + "\r\nСтрана: " + item.country + "\r\nКалибр: " + item.caliber + "x" + item.case_length + "\r\nТочность: " + Convert.ToDouble(item.accuracy) * 100 + "%\r\nУрон: " + item.damage + "\r\nСтоимость: " + item.cost, "ОК");
                    if (Convert.ToInt32(item.id) > 100 && Convert.ToInt32(item.id) < 200)
                    {
                        if (!String.Equals(item.features, "")) await DisplayAlert("Информация", "Калибр: " + item.caliber + "x" + item.case_length + " (" + item.features + ")\r\nБронепробитие (класс брони): " + item.penetration_class, "ОК");
                        else await DisplayAlert("Информация", "Калибр: " + item.caliber + "x" + item.case_length + "\r\nБронепробитие (класс брони): " + item.penetration_class, "ОК");
                    }
                    if (Convert.ToInt32(item.id) > 200 && Convert.ToInt32(item.id) < 300)
                    {
                        await DisplayAlert("Информация", "Принадлежность: " + item.groupment + "\r\nКласс брони: " + item.penetration_class + "\r\nРадиозащита: " + item.radio_protection + "\r\nТермозащита: " + item.temp_protection + "\r\nЭлектрозащита: " + item.electric_protection + "\r\nХимзащита: " + item.chemic_protection + "\r\nХз какая-то тоже защита: " + item.psy_protection + "\r\nКласс брони: " + item.penetration_class + "\r\nКонтейнеры: " + item.containers + "\r\nПНВ: " + item.PNV + "\r\nВес: " + item.weight + "\r\nСтоимость: " + item.cost, "ОК");
                    }
                    break;
            }
            
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

        /*
        private async void ItemAction(string action, Item item)                                                                                 //menu for each item
        {
            switch (action)
            {
                case "Надеть":
                    if (Convert.ToInt32(item.id) < 100)
                    {
                        if (String.Equals(Player.active_weapon1, ""))
                        {
                            Player.active_weapon1 = item.id;
                            UserDialogs.Instance.ShowLoading("Надеваем...");
                            await RefreshPlayer();
                            await DownloadInventoryItems(Player, true);
                            DropWeapon(item);
                            UserDialogs.Instance.HideLoading();
                        }
                        else if (String.Equals(Player.active_weapon2, ""))
                        {
                            Player.active_weapon2 = item.id;
                            UserDialogs.Instance.ShowLoading("Надеваем...");
                            await RefreshPlayer();
                            await DownloadInventoryItems(Player, true);
                            DropWeapon(item);
                            UserDialogs.Instance.HideLoading();
                        }
                        else await DisplayAlert("Ошибка", "Некуда ебать ложить!", "Иди нахуй");
                    }
                    else
                    {
                        string identificator = item.id.Substring(0, 1);
                        switch (identificator)
                        {
                            case "2":
                                break;
                            case "3":
                                break;
                        }
                    }
                    break;

                case "Выбросить":
                    if (Convert.ToInt32(item.id) < 100)
                        DropWeapon(item);
                    if (Convert.ToInt32(item.id) > 100)
                        DropAmmo(item);
                        break;

                case "Информация":
                    if (Convert.ToInt32(item.id) < 100)
                        await DisplayAlert("Информация", "Название: " + item.name + "\r\nТип: " + item.type + "\r\nСтрана: " + item.country + "\r\nКалибр: " + item.caliber + "x" + item.case_length + "\r\nТочность: " + Convert.ToDouble(item.accuracy) * 100 + "%\r\nУрон: " + item.damage + "\r\nСтоимость: " + item.cost, "ОК");
                    if (Convert.ToInt32(item.id) > 100)
                    {
                         if (!String.Equals(item.features, "")) await DisplayAlert("Информация", "Калибр: " + item.caliber + "x" + item.case_length + " (" + item.features + ")\r\nБронепробитие (класс брони): " + item.penetration_class, "ОК");
                         else await DisplayAlert("Информация", "Калибр: " + item.caliber + "x" + item.case_length + "\r\nБронепробитие (класс брони): " + item.penetration_class, "ОК");
                    }
                    break;

            }
        }
        */
        /*
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
            UserDialogs.Instance.ShowLoading("Кидаем подальше...");
            await DownloadInventoryItems(Player, true);
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
            UserDialogs.Instance.ShowLoading("Кидаем подальше...");
            await DownloadInventoryItems(Player, true);
            await RefreshPlayer();
            UserDialogs.Instance.HideLoading();
        }
        */


        private Task RefreshPlayer()
        {
            return Task.Run(() =>
            {
                string serialized = JsonConvert.SerializeObject(Player);
                Methods.POST_request(serialized, "update-user-info", room);
            });
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
    }
}