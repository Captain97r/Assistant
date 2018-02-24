using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App5
{

    public enum rights { user, admin }

    class Methods
    {

        public static string cookie;
        public static string server_address = "http://myfuckingserver.ddns.net/";
        //public const string server_address = "http://192.168.100.106/";

        public static string Auth(string s)
        {

            System.Security.Cryptography.AesCryptoServiceProvider AES = new System.Security.Cryptography.AesCryptoServiceProvider();

            System.Net.ServicePointManager.ServerCertificateValidationCallback += (o, certificate, chain, errors) => true;

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
            cookie = resp.Headers["Set-Cookie"];

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

            string url = server_address + "server/web/" + path;
            WebRequest req = WebRequest.Create(url);
            req.Headers.Add(HttpRequestHeader.Cookie, Methods.cookie);
            req.Method = "POST";
            req.ContentType = "application/json";
            WebResponse resp;

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

            string Out = String.Empty;

            while (count > 0)
            {
                String str = new String(read, 0, count);
                Out += str;
                count = sr.Read(read, 0, 256);
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



            string url = server_address + "server/web/" + path;
            WebRequest req = WebRequest.Create(url);
            req.Headers.Add(HttpRequestHeader.Cookie, Methods.cookie);
            req.Method = "GET";
            WebResponse resp = req.GetResponse();

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
        
        public static string[] GetRoomList()                                                                         //another signature
        {


            ServicePointManager.ServerCertificateValidationCallback +=
           delegate (object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate,
                                   System.Security.Cryptography.X509Certificates.X509Chain chain,
                                   System.Net.Security.SslPolicyErrors sslPolicyErrors)
           {
               return true; // **** Always accept
           };


            string url = server_address + "server/web/room_list";
            WebRequest req = WebRequest.Create(url);
            req.Headers.Add(HttpRequestHeader.Cookie, Methods.cookie);
            req.Method = "GET";
            WebResponse resp = req.GetResponse();

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



            string url = server_address + "server/socket/get-user-info";
            WebRequest req = WebRequest.Create(url);
            req.Headers.Add(HttpRequestHeader.Cookie, Methods.cookie);
            req.Method = "POST";
            req.ContentType = "application/json";
            WebResponse resp;

            byte[] sentData = Encoding.GetEncoding(1251).GetBytes(post);
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
    }
    
    public class Item : INotifyPropertyChanged
    {
        private string _id;
        private string _name;
        private string _type;
        private string _country;
        private string _caliber;
        private string _case_length;
        private string _accuracy;
        private string _damage;
        private string _cost;
        private string _features;
        private string _penetration_class;
        private string _image;

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

        public string name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged("name");
                }
            }
        }

        public string type
        {
            get { return _type; }
            set
            {
                if (_type != value)
                {
                    _type = value;
                    OnPropertyChanged("type");
                }
            }
        }

        public string country
        {
            get { return _country; }
            set
            {
                if (_country != value)
                {
                    _country = value;
                    OnPropertyChanged("country");
                }
            }
        }

        public string caliber
        {
            get { return _caliber; }
            set
            {
                if (_caliber != value)
                {
                    _caliber = value;
                    OnPropertyChanged("caliber");
                }
            }
        }

        public string case_length
        {
            get { return _case_length; }
            set
            {
                if (_case_length != value)
                {
                    _case_length = value;
                    OnPropertyChanged("case_length");
                }
            }
        }

        public string accuracy
        {
            get { return _accuracy; }
            set
            {
                if (_accuracy != value)
                {
                    _accuracy = value;
                    OnPropertyChanged("accuracy");
                }
            }
        }

        public string damage
        {
            get { return _damage; }
            set
            {
                if (_damage != value)
                {
                    _damage = value;
                    OnPropertyChanged("damage");
                }
            }
        }

        public string cost
        {
            get { return _cost; }
            set
            {
                if (_cost != value)
                {
                    _cost = value;
                    OnPropertyChanged("cost");
                }
            }
        }

        public string features
        {
            get { return _features; }
            set
            {
                if (_features != value)
                {
                    _features = value;
                    OnPropertyChanged("features");
                }
            }
        }

        public string penetration_class
        {
            get { return _penetration_class; }
            set
            {
                if (_penetration_class != value)
                {
                    _penetration_class = value;
                    OnPropertyChanged("penetration_class");
                }
            }
        }

        public string image
        {
            get { return _image; }
            set
            {
                if (_image != value)
                {
                    _image = value;
                    OnPropertyChanged("image");
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
        private bool _isAlive;

        private string _weapon_ids;
        private string _ammo_ids;
        private string _armor_ids;

        private string _active_helmet;
        private string _active_armor;
        private string _active_boots;
        private string _active_weapon1;
        private string _active_weapon2;

        private string _first_char;
        private string _second_char;
        private string _third_char;
        private string _fourth_char;
        private string _fifth_char;

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
            get { return _isAlive ? "Жив" : "Мертв"; ; }
            set
            {
                if (_isAlive != Convert.ToBoolean(value))
                {
                    _isAlive = Convert.ToBoolean(value);
                    OnPropertyChanged("isAlive");
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


        public string first_char
        {
            get { return _first_char; }
            set
            {
                if (_first_char != value)
                {
                    _first_char = value;
                    OnPropertyChanged("first_char");
                }
            }
        }
        public string second_char
        {
            get { return _second_char; }
            set
            {
                if (_second_char != value)
                {
                    _second_char = value;
                    OnPropertyChanged("second_char");
                }
            }
        }
        public string third_char
        {
            get { return _third_char; }
            set
            {
                if (_third_char != value)
                {
                    _third_char = value;
                    OnPropertyChanged("third_char");
                }
            }
        }
        public string fourth_char
        {
            get { return _fourth_char; }
            set
            {
                if (_fourth_char != value)
                {
                    _fourth_char = value;
                    OnPropertyChanged("fourth_char");
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
