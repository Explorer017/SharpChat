using System;

namespace SharpChatClient{

    [Serializable]
    public class PingMessage : MessageContent{
        public bool Ping = true;
        public PingMessage(){}
    }
}