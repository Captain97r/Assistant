using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App5
{
    class ListenerThread
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
                Out = Methods.ListenSocket(post);
                cur_player = JsonConvert.DeserializeObject<Player>(Out);

                //string serialized = JsonConvert.SerializeObject(player);

                if (!String.Equals(Out, prev))
                {
                    messageChanged.OnMessageChanged(cur_player);
                    prev = Out;
                }
                Thread.Sleep(15000);
            }
        }

    }
}
