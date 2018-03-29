using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using System.Diagnostics;

namespace App5
{

    public enum rights { user, admin }
    public enum actionType { reg, auth}
    public enum itemType { weapon, ammo, helmet, armor, boots, loot, artifact, unknown}

    class Methods
    {

        public static string cookie;
        public static string server_address = "http://myfuckingserver.ddns.net/";
        //public const string server_address = "http://192.168.1.2/";

        public static string Auth(string s, actionType type)
        {

            System.Security.Cryptography.AesCryptoServiceProvider AES = new System.Security.Cryptography.AesCryptoServiceProvider();

            System.Net.ServicePointManager.ServerCertificateValidationCallback += (o, certificate, chain, errors) => true;
            
            string Out = String.Empty;

            WebRequest req = WebRequest.Create(server_address + "server/web/log");
            req.Method = "POST";
            req.ContentType = "application/json";
            WebResponse resp;

            try { 

                byte[] sentData = Encoding.GetEncoding(1251).GetBytes(s);
                req.ContentLength = sentData.Length;
                Stream sendStream = req.GetRequestStream();                                     //System.Net.WebException: Error: NameResolutionFailure
                sendStream.Write(sentData, 0, sentData.Length);
                sendStream.Close();

                resp = req.GetResponse();

                if (type == actionType.auth)
                    cookie = resp.Headers["Set-Cookie"];

                Stream ReceiveStream = resp.GetResponseStream();
                StreamReader sr = new StreamReader(ReceiveStream, Encoding.UTF8);

                Char[] read = new Char[256];
                int count = sr.Read(read, 0, 256);


                while (count > 0)
                {
                    String str = new String(read, 0, count);
                    Out += str;
                    count = sr.Read(read, 0, 256);
                }
            }
            catch (System.Net.WebException e)
            {
                Debug.WriteLine("ReceiveError");
                return "-1";
            }
            catch (System.Net.Sockets.SocketException e1)
            {
                Debug.WriteLine("Network is unreachable");
                return "-1";
            }

            return Out;
        }
        
        public static string POST_request(string post, string path, string room = "")
        {
            ServicePointManager.ServerCertificateValidationCallback +=
           delegate (object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate,
                                   System.Security.Cryptography.X509Certificates.X509Chain chain,
                                   System.Net.Security.SslPolicyErrors sslPolicyErrors)
           {
               return true; // **** Always accept
           };

            if (!String.Equals(room, ""))
            {
                post = post.Insert(1, "\"room\":\"" + room + "\",");
            }

            string Out = String.Empty;

            string url = server_address + "server/web/" + path;
            WebRequest req = WebRequest.Create(url);
            req.Headers.Add(HttpRequestHeader.Cookie, Methods.cookie);
            req.Method = "POST";
            req.ContentType = "application/json";
            WebResponse resp;

            try { 

            byte[] sentData = Encoding.GetEncoding(65001).GetBytes(post);
            req.ContentLength = sentData.Length;
            Stream sendStream = req.GetRequestStream();
            sendStream.Write(sentData, 0, sentData.Length);
            sendStream.Close();

            resp = req.GetResponse();

            Stream ReceiveStream = resp.GetResponseStream();
            StreamReader sr = new StreamReader(ReceiveStream, Encoding.UTF8);

            Char[] read = new Char[256];
            int count = sr.Read(read, 0, 256);


            while (count > 0)
            {
                String str = new String(read, 0, count);
                Out += str;
                count = sr.Read(read, 0, 256);
            }

            }
            catch (System.Net.WebException e)
            {
                Debug.WriteLine("ReceiveError");
                return "-1";
            }
            catch (System.Net.Sockets.SocketException e1)
            {
                Debug.WriteLine("Network is unreachable");
                return "-1";
            }

            return Out;
        }

        public static string GET_request(string path)
        {

            ServicePointManager.ServerCertificateValidationCallback +=
            delegate (object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate,
                                    System.Security.Cryptography.X509Certificates.X509Chain chain,
                                    System.Net.Security.SslPolicyErrors sslPolicyErrors)
            {
                return true; // **** Always accept
           };


            string Out = String.Empty;

            string url = server_address + "server/web/" + path;
            WebRequest req = WebRequest.Create(url);
            req.Headers.Add(HttpRequestHeader.Cookie, Methods.cookie);
            req.Method = "GET";

            try { 
            WebResponse resp = req.GetResponse();

            Stream ReceiveStream = resp.GetResponseStream();
            StreamReader sr = new StreamReader(ReceiveStream, Encoding.UTF8);

            Char[] read = new Char[256];
            int count = sr.Read(read, 0, 256);


            while (count > 0)
            {
                String str = new String(read, 0, count);
                Out += str;
                count = sr.Read(read, 0, 256);
            }

            }
            catch (System.Net.WebException e)
            {
                Debug.WriteLine("ReceiveError");
                return "-1";
            }
            catch (System.Net.Sockets.SocketException e1)
            {
                Debug.WriteLine("Network is unreachable");
                return "-1";
            }
            return Out;
        }
        
        public static string[] GetRoomList()                                                                         //another signature
        {


            ServicePointManager.ServerCertificateValidationCallback +=
           delegate (object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate,
                                   System.Security.Cryptography.X509Certificates.X509Chain chain,
                                   System.Net.Security.SslPolicyErrors sslPolicyErrors)
           {
               return true; // **** Always accept
           };



            string Out = String.Empty;

            string url = server_address + "server/web/room_list";
            WebRequest req = WebRequest.Create(url);
            req.Headers.Add(HttpRequestHeader.Cookie, Methods.cookie);
            req.Method = "GET";

            try
            {
            WebResponse resp = req.GetResponse();

            Stream ReceiveStream = resp.GetResponseStream();
            StreamReader sr = new StreamReader(ReceiveStream, Encoding.UTF8);

            Char[] read = new Char[256];
            int count = sr.Read(read, 0, 256);


            while (count > 0)
            {
                String str = new String(read, 0, count);
                Out += str;
                count = sr.Read(read, 0, 256);
            }
            
            }
            catch (System.Net.WebException e)
            {
                Debug.WriteLine("-1");
                Debug.WriteLine(e.ToString());
                return null;                                                                                            //TODO: rewrite error return
            }

    string[] list = Out.Split('.');

            return list;
        }
        
        public static string ListenSocket(string post)                                                                  //another path
        {
            ServicePointManager.ServerCertificateValidationCallback +=
            delegate (object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate,
                                    System.Security.Cryptography.X509Certificates.X509Chain chain,
                                    System.Net.Security.SslPolicyErrors sslPolicyErrors)
            {
                return true; // **** Always accept
           };

            //var httpClient = new HttpClient(new NativeMessageHandler());

                string Out = String.Empty;

                string url = server_address + "server/socket/get-user-info";
                WebRequest req = WebRequest.Create(url);
                req.Headers.Add(HttpRequestHeader.Cookie, Methods.cookie);
                req.Method = "POST";
                req.ContentType = "application/json";
                WebResponse resp = null;

                try
                {
                    byte[] sentData = Encoding.GetEncoding(65001).GetBytes(post);
                    req.ContentLength = sentData.Length;
                    Stream sendStream = req.GetRequestStream();
                    sendStream.Write(sentData, 0, sentData.Length);
                    sendStream.Close();

                    resp = req.GetResponse();
                
                
                    Stream ReceiveStream = resp.GetResponseStream();
                    StreamReader sr = new StreamReader(ReceiveStream, Encoding.UTF8);

                    Char[] read = new Char[256];
                    int count = sr.Read(read, 0, 256);
                
                    while (count > 0)
                    {
                        String str = new String(read, 0, count);
                        Out += str;
                        count = sr.Read(read, 0, 256);
                    }
                }
                catch (System.Net.WebException e)
                {
                    Debug.WriteLine("ReceiveError uu");
                    return "-1";
                }
                catch (System.Net.Sockets.SocketException e1)
                {
                    Debug.WriteLine("Network is unreachable");
                    return "-1";
                }
                return Out;
        }
        
        /*

        public static string Reg(string s)                                                                          //just because without cookies
        {

            ServicePointManager.ServerCertificateValidationCallback +=
           delegate (object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate,
                                   System.Security.Cryptography.X509Certificates.X509Chain chain,
                                   System.Net.Security.SslPolicyErrors sslPolicyErrors)
           {
               return true; // **** Always accept
           };


            WebRequest req = WebRequest.Create(server_address + "server/web/log");
            req.Method = "POST";
            req.ContentType = "application/json";
            WebResponse resp;

            byte[] sentData = Encoding.GetEncoding(1251).GetBytes(s);
            req.ContentLength = sentData.Length;
            Stream sendStream = req.GetRequestStream();
            sendStream.Write(sentData, 0, sentData.Length);
            sendStream.Close();

            resp = req.GetResponse();

            Stream ReceiveStream = resp.GetResponseStream();
            StreamReader sr = new StreamReader(ReceiveStream, Encoding.UTF8);

            Char[] read = new Char[256];
            int count = sr.Read(read, 0, 256);

            string Out = String.Empty;

            while (count > 0)
            {
                String str = new String(read, 0, count);
                Out += str;
                count = sr.Read(read, 0, 256);
            }
            return Out;
        }

            */

        /*

        public static string GetProperty(string property_name)
        {

            ServicePointManager.ServerCertificateValidationCallback +=
           delegate (object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate,
                                   System.Security.Cryptography.X509Certificates.X509Chain chain,
                                   System.Net.Security.SslPolicyErrors sslPolicyErrors)
           {
               return true; // **** Always accept
           };


            string url = server_address + "server/web/" + property_name;
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Headers.Add(HttpRequestHeader.Cookie, Methods.cookie);
            req.AllowAutoRedirect = false;
            req.Method = "GET";
            HttpWebResponse resp;
            resp = (HttpWebResponse)req.GetResponse();

            Stream ReceiveStream = resp.GetResponseStream();
            StreamReader sr = new StreamReader(ReceiveStream, Encoding.UTF8);

            Char[] read = new Char[256];
            int count = sr.Read(read, 0, 256);

            string Out = String.Empty;

            while (count > 0)
            {
                String str = new String(read, 0, count);
                Out += str;
                count = sr.Read(read, 0, 256);
            }
            return Out;
        }

        */

    }

    public class Item
    {
        public string id { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string country { get; set; }
        public string caliber { get; set; }
        public string case_length { get; set; }
        public string accuracy { get; set; }
        public string damage { get; set; }
        public string cost { get; set; }
        public string features { get; set; }
        public string penetration_class { get; set; }
        public string image { get; set; }

        public string groupment { get; set; }
        public string radio_protection { get; set; }
        public string temp_protection { get; set; }
        public string electric_protection { get; set; }
        public string chemic_protection { get; set; }
        public string psy_protection { get; set; }
        public string containers { get; set; }
        public string PNV { get; set; }
        public string weight { get; set; }
    }

    public class InventoryItems : INotifyPropertyChanged
    {
        private List<Item> _item;


        public List<Item> item
        {
            get { return _item; }
            set
            {
                if (_item != value)
                {
                    _item = value;
                    OnPropertyChanged("item");
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

    public class InventoryList
    {
        public List<InventoryItems> InventoryItems { get; set; }
    }
    
    public class AmmoReq
    {
        public string weapon_ids { get; set; }
        public string ammo_ids { get; set; }
        public string armor_ids { get; set; }
    }
    
    public class ActiveItemsReq
    {
        public string helmet_id { get; set; }
        public string armor_id { get; set; }
        public string boots_id { get; set; }
        public string weapon1_id { get; set; }
        public string weapon2_id { get; set; }
    }

    public class SocketReq
    {
        public string action { get; set; }
        public string user { get; set; }
        public string room { get; set; }
    }
    
    public class JSON
    {
        public string type { get; set; }
        public string login { get; set; }
        public string pass { get; set; }
    }

    public class ROOM
    {
        public string name { get; set; }
    }
    
    public class Player : INotifyPropertyChanged
    {

        private string _id;
        private string _user;
        private string _hp;
        private string _hunger;
        private string _drought;
        private string _radiation;
        private string _money;
        private string _isAlive;
        private string _isBleeding;

        private string _weapon_ids;
        private string _ammo_ids;
        private string _armor_ids;

        private string _active_helmet;
        private string _active_armor;
        private string _active_boots;
        private string _active_weapon1;
        private string _active_weapon2;

        private string _stamina;
        private string _agility;
        private string _intelligence;
        private string _charisma;
        private string _fifth_char;

        private string _hand_p;
        private string _sub_p;
        private string _shot_p;
        private string _rifle_p;
        private string _assault_p;
        private string _sniper_p;

        private string _free_points;

        public string real_hp { get; set; }

        public string id
        {
            get { return _id; }
            set
            {
                if (_id != value)
                {
                    _id = value;
                    OnPropertyChanged("id");
                }
            }
        }
        public string user
        {
            get { return _user; }
            set
            {
                if (_user != value)
                {
                    _user = value;
                    OnPropertyChanged("user");
                }
            }
        }
        public string hp
        {
            get { return _hp; }
            set
            {
                if (_hp != value)
                {
                    _hp = value;
                    OnPropertyChanged("hp");
                }
            }
        }
        public string hunger
        {
            get { return _hunger; }
            set
            {
                if (_hunger != value)
                {
                    _hunger = value;
                    OnPropertyChanged("hunger");
                }
            }
        }
        public string drought
        {
            get { return _drought; }
            set
            {
                if (_drought != value)
                {
                    _drought = value;
                    OnPropertyChanged("drought");
                }
            }
        }
        public string radiation
        {
            get { return _radiation; }
            set
            {
                if (_radiation != value)
                {
                    _radiation = value;
                    OnPropertyChanged("radiation");
                }
            }
        }
        public string money
        {
            get { return _money; }
            set
            {
                if (_money != value)
                {
                    _money = value;
                    OnPropertyChanged("money");
                }
            }
        }
        
        
        public string isAlive
        {
            get
            {
                return _isAlive;
            }
            set
            {
                if ((String.Equals(_isAlive, "") && (Convert.ToInt32(_hp) < 0)) || (String.Equals(_isAlive, null) && (Convert.ToInt32(_hp) < 0)))
                {
                    _isAlive = "Мертв";
                    OnPropertyChanged("isAlive");
                }
                else if ((String.Equals(_isAlive, "") && (Convert.ToInt32(_hp) > 0)) || (String.Equals(_isAlive, null) && (Convert.ToInt32(_hp) < 0)))
                {
                    _isAlive = "В игре";
                    OnPropertyChanged("isAlive");
                }
                else if ((String.Equals(_isAlive, "В игре") && (Convert.ToInt32(_hp) < 0)))
                { 
                    _isAlive = "Мертв";
                    OnPropertyChanged("isAlive");
                }
                else  if((String.Equals(_isAlive, "Мертв") && (Convert.ToInt32(_hp) > 0)))
                {
                    _isAlive = "В игре";
                    OnPropertyChanged("isAlive");
                }
            }
        }
        public string isBleeding
        {
            get { return (Convert.ToInt32(_isBleeding) > 0) ? "1" : "0"; }
            set
            {
                if (_isBleeding != value)
                {
                    _isBleeding = value;
                    OnPropertyChanged("isBleeding");
                }
            }
        }

        public string weapon_ids
        {
            get { return _weapon_ids; }
            set
            {
                if (_weapon_ids != value)
                {
                    _weapon_ids = value;
                    OnPropertyChanged("weapon_ids");
                }
            }
        }
        public string ammo_ids
        {
            get { return _ammo_ids; }
            set
            {
                if (_ammo_ids != value)
                {
                    _ammo_ids = value;
                    OnPropertyChanged("ammo_ids");
                }
            }
        }
        public string armor_ids
        {
            get { return _armor_ids; }
            set
            {
                if (_armor_ids != value)
                {
                    _armor_ids = value;
                    OnPropertyChanged("armor_ids");
                }
            }
        }

        public string active_helmet
        {
            get { return _active_helmet; }
            set
            {
                if (_active_helmet != value)
                {
                    _active_helmet = value;
                    OnPropertyChanged("active_helmet");
                }
            }
        }
        public string active_armor
        {
            get { return _active_armor; }
            set
            {
                if (_active_armor != value)
                {
                    _active_armor = value;
                    OnPropertyChanged("active_armor");
                }
            }
        }
        public string active_boots
        {
            get { return _active_boots; }
            set
            {
                if (_active_boots != value)
                {
                    _active_boots = value;
                    OnPropertyChanged("active_boots");
                }
            }
        }
        public string active_weapon1
        {
            get { return _active_weapon1; }
            set
            {
                if (_active_weapon1 != value)
                {
                    _active_weapon1 = value;
                    OnPropertyChanged("active_weapon1");
                }
            }
        }
        public string active_weapon2
        {
            get { return _active_weapon2; }
            set
            {
                if (_active_weapon2 != value)
                {
                    _active_weapon2 = value;
                    OnPropertyChanged("active_weapon2");
                }
            }
        }


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


        public string hand_p
        {
            get { return _hand_p; }
            set
            {
                if (_hand_p != value)
                {
                    _hand_p = value;
                    OnPropertyChanged("hand_p");
                }
            }
        }
        public string sub_p
        {
            get { return _sub_p; }
            set
            {
                if (_sub_p != value)
                {
                    _sub_p = value;
                    OnPropertyChanged("sub_p");
                }
            }
        }
        public string shot_p
        {
            get { return _shot_p; }
            set
            {
                if (_shot_p != value)
                {
                    _shot_p = value;
                    OnPropertyChanged("shot_p");
                }
            }
        }
        public string rifle_p
        {
            get { return _rifle_p; }
            set
            {
                if (_rifle_p != value)
                {
                    _rifle_p = value;
                    OnPropertyChanged("rifle_p");
                }
            }
        }
        public string assault_p
        {
            get { return _assault_p; }
            set
            {
                if (_assault_p != value)
                {
                    _assault_p = value;
                    OnPropertyChanged("assault_p");
                }
            }
        }
        public string sniper_p
        {
            get { return _sniper_p; }
            set
            {
                if (_sniper_p != value)
                {
                    _sniper_p = value;
                    OnPropertyChanged("sniper_p");
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

    public class PlayerList
    {
        public List<Player> players { get; set; }
    }

    public class ItemReq
    {
        public string type { get; set; }
        public string subtype { get; set; }
    }
}
