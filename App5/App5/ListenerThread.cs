using Acr.UserDialogs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App5
{
    class ListenerThread: ContentPage
    {
        public Thread listenerThrd;

        MessageChanged messageChanged = new MessageChanged();

        string post;

        public ListenerThread(string _post)
        {
            post = _post;

            messageChanged.MessageHasChanged += new MessageEventHandler(MainHandler.MessageEventHandler);
            listenerThrd = new Thread(new ThreadStart(this.run));
            listenerThrd.Name = "listener";
            listenerThrd.IsBackground = true;
            listenerThrd.Start();
        }

        void run()
        {
            string prev = null;
            string Out = null;
            Player cur_player = null;

            while (true)
            {
                try
                {
                    Out = Methods.ListenSocket(post);
                    if (String.Equals(Out, "-1")) continue;
                    else cur_player = JsonConvert.DeserializeObject<Player>(Out); //System.ArgumentNullException: <Timeout exceeded getting exception details>

                    //string serialized = JsonConvert.SerializeObject(player);

                    if (Out == null) continue;
                    if (!String.Equals(Out, prev))
                    {
                        messageChanged.OnMessageChanged(cur_player);
                        prev = Out;
                    }
                    Thread.Sleep(1000);
                }
                catch(System.Net.WebException e)
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        await DisplayAlert("Error", "Ошибка в потоке ListenerThread" + e, "OK");
                    });
                }
                catch(Exception e1)
                {
                    Debug.WriteLine(e1);
                }
                //catch (System.Net.)
            }
        }

    }



    class RefresherThread
    {
        public Thread refresherThrd;

        MessageChanged messageChanged = new MessageChanged();

        string path;
        Player player = null;
        string room;

        public RefresherThread(string _path, Player _player, string _room)
        {
            path = _path;
            player = _player;
            room = _room;

            messageChanged.MessageHasChanged += new MessageEventHandler(MainHandler.MessageEventHandler);
            refresherThrd = new Thread(new ThreadStart(this.run));
            refresherThrd.Name = "refresher";
            refresherThrd.IsBackground = true;
            refresherThrd.Start();
        }

        void run()
        {
            string Out = null;

            string serialized = JsonConvert.SerializeObject(player);
                
            Out = Methods.POST_request(serialized, path, room);
            player = JsonConvert.DeserializeObject<Player>(Out); //System.ArgumentNullException: <Timeout exceeded getting exception details>
            
            messageChanged.OnMessageChanged(player);
            refresherThrd.Abort();
        }

    }



}
