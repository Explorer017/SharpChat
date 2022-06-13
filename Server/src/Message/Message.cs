using System;
using System.Xml.Serialization;

namespace SharpChatServer
{
    [Serializable]
    [XmlInclude(typeof(PingMessage)), XmlInclude(typeof(PingResponseMessage)), XmlInclude(typeof(UserMessage)), XmlInclude(typeof(DisconnectMessage)), XmlInclude(typeof(ConnectMessage)), XmlInclude(typeof(EncryptMessage)), XmlInclude(typeof(EncryptionAcknowledgedMessage)), XmlInclude(typeof(AuthenticateMessage)), XmlInclude(typeof(AuthConfirmMessage)), XmlInclude(typeof(AuthDenyMessage)), XmlInclude(typeof(AesConfirmMessage))]
    public class Message{
        public MessageType type;
        public MessageContent? content;
        public Message(MessageType type, MessageContent? content){
            this.type = type;
            this.content = content;
        }

        public Message(){
            // Empty constructor for XML serialization
            // Do not use
            // Why does it even need this
            // what is the point?
        }
    }

    [Serializable]
    public abstract class MessageContent {
        // this class exists solely to be used as a placeholder for XML serialization
        // it does not contain anything and will be removed when i can figure out how to make it not exist
    }

    public enum MessageType{
        Ping,
        PingResponse,
        Message,
        Disconnect,
        Connect,
        Encrypt,
        EncryptionAcknowledged,
        Authenticate,
        AuthConfirm,
        AuthDeny,
        ConfirmConnection
    }
}
