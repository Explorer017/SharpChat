using System;
using System.Security.Cryptography;
using System.Net;
using System.Net.Sockets;
using Spectre.Console;

namespace SharpChatServer{
    class User{
        int id;
        string username = "NOT SET";
        RSA rsa;
        Aes aes;
        public User(){
            // generate rsa
            rsa = RSA.Create();
            rsa.KeySize = 8192;
            aes = Aes.Create();
            aes.KeySize = 256;
            aes.GenerateKey();
            aes.GenerateIV();
            
        }

        public static User? Create(TcpClient client, UserService userService){
            // Receive Message
            Message message = Transfer.recieveMessage(client.GetStream());
            if (message.type == MessageType.Ping){
                Server.Log(Logger.Info, $"Client {client.Client.RemoteEndPoint} sent a ping!");
                Transfer.sendMessage(client.GetStream(), new Message(MessageType.PingResponse, Server.config.name, Server.config.motd));
                message = Transfer.recieveMessage(client.GetStream());
            } if (message.type == MessageType.Connect){
                User user = new User();
                user.SendRSA(client);
                message = Transfer.receiveMessageRSA(client.GetStream(), user.GetRSA());
                if (message.type != MessageType.Authenticate){
                    Server.Log(Logger.Warning, $"Client {client.Client.RemoteEndPoint} sent an invalid authorisation message! Disconnecting");
                    client.Close();
                    return null;
                }
                int confirm = 0;
                if (message.field4 == 0){
                    if(userService.Register(message.field1, message.field2)){confirm = 1; }
                    else { confirm = 0; }
                } else if (message.field4 == 1){
                    if (userService.Login(message.field1, message.field2)){confirm = 1;}
                    else { confirm = 0; }
                } else {
                    Server.Log(Logger.Warning, $"Client {client.Client.RemoteEndPoint} sent an invalid authorisation message! Disconnecting");
                    client.Close();
                    return null;
                }
                RSA clientRSA = RSA.Create();
                message = Transfer.receiveMessageRSA(client.GetStream(), user.GetRSA());
                if (message.type != MessageType.Encrypt){
                    Server.Log(Logger.Warning, $"Client {client.Client.RemoteEndPoint} sent an invalid authorisation message! Disconnecting");
                    client.Close();
                    return null;
                }
                clientRSA.ImportRSAPublicKey(message.field3, out _);
                Transfer.sendMessageRSA(client.GetStream(), new Message(MessageType.Confirm, user.GetAES().Key, user.GetAES().IV, confirm), clientRSA);
                if (confirm == 0) {client.Close(); return null;}
                Server.Log(Logger.Info, Transfer.receiveMessageAES(client.GetStream(), user.GetAES()).type.ToString());
                return user;
            }
            return null;
        }
            

        public void SendRSA(TcpClient client){
            Transfer.sendMessage(client.GetStream(), new Message(MessageType.Encrypt, rsa.ExportRSAPublicKey()));
        }

        public RSA GetRSA(){
            return rsa;
        }

        public Aes GetAES(){
            return aes;
        }

        
    }
}