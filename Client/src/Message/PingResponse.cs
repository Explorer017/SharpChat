using System;

namespace SharpChatClient{

    [Serializable]
    public class PingResponseMessage : MessageContent{
        public string? ServerName;
        public string? ServerMOTD;
        public int? UsersOnline;
        public PingResponseMessage(string serverName, string serverMOTD, int usersOnline){
            ServerName = serverName;
            ServerMOTD = serverMOTD;
            UsersOnline = usersOnline;
        }

        public PingResponseMessage(){}
    }
}