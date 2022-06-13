using System;

namespace SharpChatClient{

    [Serializable]
    public class ConnectMessage : MessageContent{
        public bool Connect = true;
        public ConnectMessage(){}
    }

}