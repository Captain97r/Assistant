using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace App5
{
    class ListenSocketThread
    {
        const int port = 80;
        string address;

        TcpClient client = null;
        public Thread listenerThrd;
        MessageChanged messageChanged = new MessageChanged();
        string post;

        public ListenSocketThread(string _post)
        {
            string ip = Convert.ToString(System.Net.Dns.GetHostAddresses("myfuckingserver.ddns.net")[0]);
            address = ip;
            post = _post;
            messageChanged.MessageHasChanged += new MessageEventHandler(MainHandler.MessageEventHandler);
            listenerThrd = new Thread(new ThreadStart(this.run));
            listenerThrd.Name = "listener";
            listenerThrd.IsBackground = true;
            listenerThrd.Start();
        }

        void run()
        {
            Player cur_player = null;
            
                client = new TcpClient(address, port);
                NetworkStream stream = client.GetStream();
                byte[] data = Encoding.Unicode.GetBytes(post);
                stream.Write(data, 0, data.Length);
                
                while (true)
                {
                    data = new byte[64];
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    do
                    {
                        bytes = stream.Read(data, 0, data.Length);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (stream.DataAvailable);

                    cur_player = JsonConvert.DeserializeObject<Player>(builder.ToString());

                    messageChanged.OnMessageChanged(cur_player);
                }
        }
    }
}
