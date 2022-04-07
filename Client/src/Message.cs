using System;

namespace SharpChatClient
{
    public class Message{
        public MessageType type;
        public string message;
        public Message(MessageType type, string message){
            this.type = type;
            this.message = message;
        }
        public Message(){
            // Empty constructor for XML serialization
            // Do not use
            // Why does it even need this
            // what is the point?
        }
    }

    public enum MessageType{
        Message,
        Connect,
        Disconnect,
        Ping,
        PingResponse
    }
}