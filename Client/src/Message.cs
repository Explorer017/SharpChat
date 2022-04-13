using System;

namespace SharpChatClient
{
    public class Message{
        public MessageType type;
        public string? field1;
        public string? field2;
        public byte[]? field3;
        public int field4;
        public byte[]? field5;
        public byte[]? SessionToken;
        public Message(MessageType type, byte[] field3){
            this.type = type;
            this.field3 = field3;
        }
        public Message(MessageType type, string field1, string field2){
            this.type = type;
            this.field1 = field1;
            this.field2 = field2;
        }
        public Message(MessageType type, string field1, string field2, int field4){
            this.type = type;
            this.field1 = field1;
            this.field2 = field2;
            this.field4 = field4;
        }
        public Message(MessageType type, string field1, string field2, byte[] field3, int field4){
            this.type = type;
            this.field1 = field1;
            this.field2 = field2;
            this.field3 = field3;
            this.field4 = field4;
        }
        public Message(MessageType type, string field1){
            this.type = type;
            this.field1 = field1;
            this.field2 = "NO DATA";
        }
        public Message (MessageType type, string field1, byte[] sessionToken){
            this.type = type;
            this.field1 = field1;
            this.SessionToken = sessionToken;
        }
        public Message(MessageType type, byte[] field3, byte[] field5, int field4){
            this.type = type;
            this.field3 = field3;
            this.field4 = field4;
            this.field5 = field5;
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
        PingResponse,
        Encrypt,
        Authenticate,
        Confirm,
        SessionToken
    }
}
