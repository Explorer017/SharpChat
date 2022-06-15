using System;
using System.Security.Cryptography;
using System.Net;
using System.Net.Sockets;
using Spectre.Console;

namespace SharpChatServer{
    class User : IDisposable{
        int id;
        string username = "NOT SET";
        RSA rsa;
        Aes aes;
        TcpClient? client { get; set; }
        byte[] sessionToken;
        public User(){
            // generate rsa
            rsa = RSA.Create();
            rsa.KeySize = 8192;
            aes = Aes.Create();
            aes.KeySize = 256;
            aes.GenerateKey();
            aes.GenerateIV();
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create()){
                sessionToken = new byte[128];
                rng.GetBytes(sessionToken);
            }
            
        }

        public static User? Create(TcpClient client, UserService userService){
            // Receive Message
            Message message = Transfer.recieveMessage(client.GetStream());

            if (message.type == MessageType.Ping){
                Server.Log(Logger.Info, $"Client {client.Client.RemoteEndPoint} sent a ping!");
                if (Server.config == null) {throw new NullReferenceException("Server Config file Error!");}

                Transfer.sendMessage(client.GetStream(), new Message(MessageType.PingResponse, new PingResponseMessage(Server.config.name, Server.config.motd, userService.users.Count)));
                message = Transfer.recieveMessage(client.GetStream());
            } if (message.type == MessageType.Connect){
                User user = new User();
                user.SendRSA(client);
                message = Transfer.receiveMessageRSA(client.GetStream(), user.GetRSA());
                if (message == null){ throw new Exception();}
                if (message.type != MessageType.Authenticate){
                    Server.Log(Logger.Warning, $"Client {client.Client.RemoteEndPoint} sent an invalid authorisation message! Disconnecting");
                    client.Close();
                    return null;
                }
                Server.Log(Logger.Info, $"waiting for authentication from {client.Client.RemoteEndPoint}...");
                AuthenticateMessage? authMessage = (AuthenticateMessage?)message.content;
                bool confirm = false;
                if (authMessage == null){ throw new Exception();}
                if (authMessage.NewAccount == true){
                    if(userService.Register(authMessage.Username ?? throw new Exception(), authMessage.HashedPassword ?? throw new Exception())){confirm = true; }
                    else { confirm = false; }
                } else {
                    if (userService.Login(authMessage.Username ?? throw new Exception(), authMessage.HashedPassword ?? throw new Exception())){confirm = true;}
                    else { confirm = false; }
                }
                Server.Log(Logger.Info, $"{client.Client.RemoteEndPoint} has {(confirm ? "successfully" : "failed")} authenticated");
                user.username = authMessage.Username;
                RSA clientRSA = RSA.Create();
                message = Transfer.receiveMessageRSA(client.GetStream(), user.GetRSA());
                if (message.type != MessageType.Encrypt){
                    Server.Log(Logger.Warning, $"Client {client.Client.RemoteEndPoint} sent an invalid authorisation message! Disconnecting");
                    client.Close();
                    return null;
                }
                EncryptMessage? encryptMessage = (EncryptMessage?)message.content;
                if (encryptMessage == null){ throw new Exception();}
                clientRSA.ImportRSAPublicKey(encryptMessage.RSAPublicKey, out _);
                if (confirm){
                    Transfer.sendMessageRSA(client.GetStream(), new Message(MessageType.AuthConfirm, new AuthConfirmMessage(user.GetAES().Key, user.GetAES().IV)), clientRSA);
                } else {
                    Transfer.sendMessageRSA(client.GetStream(), new Message(MessageType.AuthDeny, new AuthDenyMessage("Your login details did not match any user on file, or the account you were trying to register already exists.")), clientRSA);
                }
                Message ConfirmConnection = Transfer.receiveMessageAES(client.GetStream(), user.GetAES());
                if (ConfirmConnection.type != MessageType.ConfirmConnection){
                    Server.Log(Logger.Warning, $"Client {client.Client.RemoteEndPoint} sent an invalid authorisation message! Disconnecting");
                    client.Close();
                    return null;
                }
                if (ConfirmConnection.content == null) { throw new NullReferenceException();}/*
                if (((ConfirmConnection)ConfirmConnection.content).sessionToken != user.GetSessionToken()){
                    Server.Log(Logger.Warning, $"Client {client.Client.RemoteEndPoint} sent an invalid authorisation message! Disconnecting");
                    client.Close();
                    return null;
                }*/
                user.client = client;
                return user;
            }
            return null;
        }
            

        public void SendRSA(TcpClient client){
            Transfer.sendMessage(client.GetStream(), new Message(MessageType.Encrypt, new EncryptMessage(rsa.ExportRSAPublicKey())));
        }

        public RSA GetRSA(){
            return rsa;
        }

        public Aes GetAES(){
            return aes;
        }

        public void Dispose(){
            rsa.Dispose();
            aes.Dispose();
        }

        public Message SendMessage(Message message){
            if (client == null){
                throw new Exception("Client is null");
            }
            return Transfer.sendMessageAES(client.GetStream(), message, aes);
        }

        public Message ReceiveMessage(){
            if (client == null){
                throw new Exception("Client is null");
            }
            return Transfer.receiveMessageAES(client.GetStream(), aes);
        }
        
        public void MessageReciever(){
                if (client == null){
                    throw new Exception("Client is null");
                }
                Message message = ReceiveMessage();
                if (message.type == MessageType.Disconnect){
                    Server.Log(Logger.Info, $"Client {client.Client.RemoteEndPoint} disconnected");
                    client.Close();
                    client = null;
                    return;
                }
                Server.Log(Logger.Info, $"Client {client.Client.RemoteEndPoint} sent a message");
                //Server.Log(Logger.Info, $"Message: {message.field1}");
                //Server.Log(Logger.Info, $"Message: {message.field2}");
                //Server.Log(Logger.Info, $"Message: {message.field3}");
                //Server.Log(Logger.Info, $"Message: {message.field4}");
        }

        public TcpClient GetClient(){
            return client!;
        }

        public string GetUsername(){
            return username;
        }

        public string GetLoggableUsername(){
            if (client == null){
                throw new Exception("Client is null");
            }
            return $"{this.username}({this.client.Client.RemoteEndPoint})";
        }

        public byte[] GetSessionToken(){
            return sessionToken;
        }

        public bool VerifySessionToken(UserMessage message){
            if (message.sessionToken == null){
                return false;
            }
            return message.sessionToken.SequenceEqual(sessionToken);
        }
    }
}