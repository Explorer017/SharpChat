using System;

namespace SharpChatServer
{
    [Serializable]
    public class ConfirmConnection : MessageContent
    {
        public bool AesConfirm = true;
        public byte[]? sessionToken;
        public ConfirmConnection()
        {
        }
    }
}
