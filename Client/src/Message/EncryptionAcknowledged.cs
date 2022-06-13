using System;

namespace SharpChatClient{
    [Serializable]
    public class EncryptionAcknowledgedMessage : MessageContent{
        public bool EncryptionAcknowledged = true;
        public EncryptionAcknowledgedMessage(){}
    }
}