using System;

namespace SharpChatServer{

    [Serializable]
    public class ConnectMessage : MessageContent{
        public bool Connect = true;
        public ConnectMessage(){}
    }

}