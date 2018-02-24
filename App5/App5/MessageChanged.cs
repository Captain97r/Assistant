using System;
using System.Collections.Generic;
using System.Text;

namespace App5
{
    delegate void MessageEventHandler(object sender, MsgEventArgs e);

    class MessageChanged
    {
        public event MessageEventHandler MessageHasChanged;

        public void OnMessageChanged(Player player)
        {
            MsgEventArgs m = new MsgEventArgs();

            if (MessageHasChanged != null)
            {
                m.current_player = player;
                MessageHasChanged(this, m);
            }
        }
    }

    public class MsgEventArgs : EventArgs
    {
        public Player current_player;
    }
}
