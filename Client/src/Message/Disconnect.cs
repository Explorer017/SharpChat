using System;

namespace SharpChatClient
{
    [Serializable]
    public class DisconnectMessage : MessageContent
    {
        public bool Disconnect = true;
        public DisconnectMessage(){}
    }
}