using System;

namespace SharpChatClient{
    [Serializable]
    public class AuthDenyMessage : MessageContent{
        public string? Reason;
        public AuthDenyMessage(string reason){
            Reason = reason;
        }
        public AuthDenyMessage(){}
    }
}