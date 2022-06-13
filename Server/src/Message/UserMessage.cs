using System;

namespace SharpChatServer
{
    [Serializable]
    public class UserMessage : MessageContent{
        public string? Message;
        public byte[]? sessionToken;
        public UserMessage(string message, byte[] sessionToken){
            Message = message;
            this.sessionToken = sessionToken;
        }
        public UserMessage(){}
    }
}