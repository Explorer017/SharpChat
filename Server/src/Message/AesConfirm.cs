using System;

namespace SharpChatServer
{
    [Serializable]
    public class AesConfirmMessage : MessageContent
    {
        public bool AesConfirm = true;
        public byte[]? sessionToken;
        public AesConfirmMessage()
        {
        }
    }
}
