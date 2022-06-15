using System;

namespace SharpChatClient
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
