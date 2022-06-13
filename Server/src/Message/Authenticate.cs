using System;

namespace SharpChatServer
{
    [Serializable]
    public class AuthenticateMessage : MessageContent
    {
        public string? Username;
        public byte[]? HashedPassword;
        public bool? NewAccount;

        public AuthenticateMessage(string username, byte[] hashedPassword, bool newAccount)
        {
            Username = username;
            HashedPassword = hashedPassword;
            NewAccount = newAccount;
        }

        public AuthenticateMessage(){}
        
    }
}