using System;
using System.Security.Cryptography;

namespace SharpChatClient{
    class CryptographyService{
        RSA? InboundRSA;
        RSA OutboundRSA;
        Aes? aes;
        byte[]? sessionToken;

        public CryptographyService(){
            this.OutboundRSA = RSA.Create();
            this.OutboundRSA.KeySize = 4096;
        }

        public void SetInboundRSA(RSA rsa){
            this.InboundRSA = rsa;
        }

        public void SetAES(Aes aes){
            this.aes = aes;
        }

        public void SetSessionToken(byte[] sessionToken){
            this.sessionToken = sessionToken;
        }

        public byte[] GetSessionToken(){
            return this.sessionToken ;//?? throw new NullReferenceException("Session token is null");
        }

        public Aes GetAes(){
            return this.aes ?? throw new Exception("AES not set");
        }

        public string Hash(string toHash){
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(toHash);
            byte[] hash = SHA256.Create().ComputeHash(bytes);
            return BitConverter.ToString(hash);
        }

        public RSA GetOutboundRSA(){
            return OutboundRSA;
        }
    }
}