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


            img_grid.ColumnDefinitions = new ColumnDefinitionCollection();                                                                      //creating grid collections
            img_grid.RowDefinitions = new RowDefinitionCollection();

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
            InventoryItems itemList = await GetInventory(player.weapon_ids, player.ammo_ids);
            
            List<Uri> uriList = await GetImages(itemList);
            
            int num = uriList.Count;
            int column_num = 3;

            CreateGrid(num, column_num, itemList, uriList, isReBuild);
        }

        private Task<InventoryItems> GetInventory(string weapons, string ammo)                                                                  //getting item info for every single one for user
        {
            AmmoReq req;

            return Task.Run(() =>
            {
                req = new AmmoReq() { weapon_ids = weapons, ammo_ids = ammo };
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
                img_grid.ColumnDefinitions.Clear();
                img_grid.RowDefinitions.Clear();
                img_grid.Children.Clear();
            }
            for (int i = 0; i < column_num; i++)                                                                                                //adding columns
                img_grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star)  });


            int rows = (int)(((double)num / column_num) + 0.99);                                                                                //preliminary counting and adding rows
            for (int i = 0; i < rows; i++)
                img_grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, HEIGHT) });

            List<int> free_column = new List<int>();                                                                                            //array for free cells
            List<int> free_row = new List<int>() ;
            int count_col = 0, count_row = 0, item_id = 0;                                                                                      //current pos x, y and current item id
            int multislot_item = 0;

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
                    OutlineColor = Color.White
                };

                var gestureRecognizer = new TapGestureRecognizer();                                                                             //Adding gesture recognizer for every single frame
                gestureRecognizer.Tapped += (o, e) => OnItemTapped(o, e);

                frame.GestureRecognizers.Add(gestureRecognizer);

                
                if (Convert.ToInt32(itemList.item[item_id].id) > 10 && Convert.ToInt32(itemList.item[item_id].id) < 100)                        //filling grid with "long" items
                {
                    if (count_col == column_num-1) { free_column.Add(count_col); free_row.Add(count_row); count_col = 0; count_row++; }
                    img_grid.Children.Add(frame, count_col, count_row);
                    Grid.SetColumnSpan(frame, 2);
                    count_col++;
                    multislot_item++;
                }
                else if (free_column.Count != 0)                                                                                                //filling free cells with "short" items
                {
                    img_grid.Children.Add(frame, free_column[0], free_row[0]);
                    free_column.RemoveAt(0);
                    free_row.RemoveAt(0);
                    free_column.Add(count_col);
                    free_row.Add(count_row);
                }
                else img_grid.Children.Add(frame, count_col, count_row);                                                                        //regular filling
                count_col++;
                if (count_col == column_num)
                {
                    count_row++;
                    count_col = 0;
                }
                item_id++;
            }

            int rows_recount = (int)(((double)(num + multislot_item) / column_num) + 0.99);                                                     //recounting number of rows
            
            /*
            while (count_col < column_num && count_col != 0)
            {
                Label label = new Label();
                label.Text = Convert.ToString(count_col);
                Frame empty_frame = new Frame()
                {
                    OutlineColor = Color.White,
                    Content = label
                };
                img_grid.Children.Add(empty_frame, count_col++, count_row);
            }
            */
            for (int i = 0; i < rows_recount - rows; i++)
            {
                img_grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, HEIGHT) });                                          //adding necessary rows
            }
        }


        private async void OnItemTapped(object ob, EventArgs e)                                                                                 //tap handler
        {
            Frame frame = (Frame)ob;
            Image content = (Image)frame.Content;
            UriImageSource source = (UriImageSource)content.Source;
            string id = source.AutomationId;

            foreach (Item item in items.item)
            {
                if (String.Equals(id, item.id) && ((Convert.ToInt32(id) < 100) || (Convert.ToInt32(id) > 200)))
                {
                    string action = await DisplayActionSheet(item.name + "\r\n", "Отмена", null, "Надеть", "Выбросить", "Информация");
                    ItemAction(action, item);
                    break;
                }
                else if (String.Equals(id, item.id) && ((Convert.ToInt32(id) > 100 && Convert.ToInt32(id) < 200)))
                {
                    string action = await DisplayActionSheet(item.caliber + "x" + item.case_length + " (" + item.features + ")\r\n", "Отмена", null, "\r\nВыбросить", "Информация");
                    ItemAction(action, item);
                    break;
                }
            }
        }

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