using System;

namespace SharpChatClient
{
    public class Message{
        public MessageType type;
        public string? field1;
        public string? field2;
        public Message(MessageType type, string field1, string field2){
            this.type = type;
            this.field1 = field1;
            this.field2 = field2;
        }
        public Message(MessageType type, string field1){
            this.type = type;
            this.field1 = field1;
            this.field2 = "NO DATA";
        }
        public Message(MessageType type){
            this.type = type;
            this.field1 = "NO DATA";
            this.field2 = "NO DATA";
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