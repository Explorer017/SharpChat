using System;

namespace SharpChatServer
{
    [Serializable]
    public class EncryptMessage : MessageContent
    {
        byte[]? RSAPublicKey;
        public EncryptMessage(byte[] _RSAPublicKey)
        {
            this.RSAPublicKey = _RSAPublicKey;
        }
        public EncryptMessage() {}
    }
}