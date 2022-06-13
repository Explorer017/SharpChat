using System;

namespace SharpChatServer{

    [Serializable]
    public class PingMessage : MessageContent{
        public bool Ping = true;
        public PingMessage(){}
    }
}