using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;


namespace App5
{

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AuthorizationMenu : ContentPage
    {
        public AuthorizationMenu()
        {
            InitializeComponent();
        }

        private async void Reg(object sender, System.EventArgs e)
        {
            Auth_button.IsEnabled = false;
            await Navigation.PushModalAsync(new Registration());
            Auth_button.IsEnabled = true;
        }

        private async void Auth(object sender, System.EventArgs e)
        {
            Reg_clicked.IsEnabled = false;
            await Navigation.PushModalAsync(new Authentification());
            Reg_clicked.IsEnabled = true;
        }

    }
    /*
    public class Auth : IMarkupExtension
    {

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            return CheckAuthorization(Methods.cookie);
        }

        public static string CheckAuthorization(string cookie)
        {

            return Methods.GET_request("");
            
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create("http://192.168.1.2/server/web/");
            req.Headers.Add(HttpRequestHeader.Cookie, cookie);
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

    } */

}