using System;

namespace SharpChatServer
{
    [Serializable]
    public class DisconnectMessage : MessageContent
    {
        public bool Disconnect = true;
        public DisconnectMessage(){}
    }
}