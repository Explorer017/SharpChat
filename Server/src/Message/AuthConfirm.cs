using System;

namespace SharpChatServer{
    [Serializable]
    public class AuthConfirmMessage : MessageContent{
        public byte[]? AesKey;
        public byte[]? AesIV;
        public byte[]? sessionToken;
        public AuthConfirmMessage(byte[] aesKey, byte[] aesIV, byte[] sessionToken){
            AesKey = aesKey;
            AesIV = aesIV;
            this.sessionToken = sessionToken;
        }
        public AuthConfirmMessage(){}
    }
}