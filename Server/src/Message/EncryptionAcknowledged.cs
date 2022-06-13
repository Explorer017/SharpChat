using System;

namespace SharpChatServer{
    [Serializable]
    public class EncryptionAcknowledgedMessage : MessageContent{
        public bool EncryptionAcknowledged = true;
        public EncryptionAcknowledgedMessage(){}
    }
}