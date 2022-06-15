using System;
using Spectre.Console;
using System.Net.Sockets;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace SharpChatClient{
    class Program{

        static void Main(string[] args){
            AnsiConsole.Markup("[bold green]SharpChat Terminal Client[/]\n[bold yellow]Version: 2.0.1[/]\n");
            TcpClient client = new TcpClient();
            try {
            client.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 12340));
            } catch(Exception e) {
                Log(Logger.Error, "Could not connect to server!");
                Log(Logger.Error, e.Message);
                AnsiConsole.WriteException(e);
            }
            CryptographyService cryptographyService;
            Transfer.sendMessage(client.GetStream(), new Message(MessageType.Ping, new PingMessage()));
            Message ping = Transfer.recieveMessage(client.GetStream());
            if (ping.type == MessageType.PingResponse){
                PingResponseMessage? pingResponse = (PingResponseMessage?)ping.content;
                if (pingResponse == null){
                    throw new Exception("PingResponseMessage is null!");
                }
                AnsiConsole.Markup($"[bold white]{pingResponse.ServerName}[/]\n[white]{pingResponse.ServerMOTD}[/]\n[green]Users Connected: [/][blue]{pingResponse.UsersOnline}[/]\n");
            }
            else {
                Log(Logger.Error, "Server did not send a valid ping response!");
            }
            NetworkStream stream = client.GetStream();
            Transfer.sendMessage(stream, new Message(MessageType.Connect, new ConnectMessage()));
            Message? rsaInfo = null;
            cryptographyService = new CryptographyService();
            string LogOrRegister = AnsiConsole.Prompt(new SelectionPrompt<string>()
                                            .Title("[green]Login[/] or [yellow]Register[/]?")
                                            .AddChoices(new[]{"Login", "Register"}));
            string Username = AnsiConsole.Ask<string>("[white bold]Username:[/]");
            string Password = cryptographyService.Hash(AnsiConsole.Prompt(new TextPrompt<string>("[red bold]Password:[/]").PromptStyle("red").Secret()));
            AnsiConsole.Status().Start("Waiting for RSA key...", ctx =>
                {
                using (RSA rsa = RSA.Create()){
                    // Recieve RSA key
                    rsaInfo = Transfer.recieveMessage(stream);

                    // Check if the message is the correct type
                    if (rsaInfo.type == MessageType.Encrypt){
                        if (rsaInfo.content == null) {throw(new Exception("RSA key is null!"));}
                        EncryptMessage rsaMessage = (EncryptMessage)rsaInfo.content;
                        // Import RSA key
                        rsa.ImportRSAPublicKey(rsaMessage.RSAPublicKey, out _);
                        // Store the RSA key
                        cryptographyService.SetInboundRSA(rsa);
                        ctx.Status("Sending credentials...");
                        // Send credentials
                        // string to byte[]
                        Transfer.sendMessageRSA(stream, new Message(MessageType.Authenticate, new AuthenticateMessage(Username , Encoding.UTF8.GetBytes(Password), LogOrRegister == "Login" ? false : true)), rsa);
                        // Send RSA key
                        Transfer.sendMessageRSA(stream, new Message(MessageType.Encrypt, new EncryptMessage(cryptographyService.GetOutboundRSA().ExportRSAPublicKey())), rsa);
                        // Receive response
                        Message response = Transfer.receiveMessageRSA(stream, cryptographyService.GetOutboundRSA());
                        if (response.type == MessageType.AuthConfirm){
                            if (response.content == null) {throw(new Exception("AuthConfirmMessage is null!"));}
                            AuthConfirmMessage authConfirm = (AuthConfirmMessage)response.content;
                            AnsiConsole.Markup("[green]Successfully logged in![/]\n");
                            Aes aes = Aes.Create();
                            aes.Key = authConfirm.AesKey ?? throw new NullReferenceException("Session key is null!");
                            aes.IV = authConfirm.AesIV ?? throw new NullReferenceException("IV is null!");
                            //cryptographyService.SetSessionToken(authConfirm.sessionToken ?? throw new NullReferenceException("Session token cannot be null!"));
                            cryptographyService.SetAES(aes);
                            //aes.Dispose();
                            Transfer.sendMessageAES(stream, new Message(MessageType.ConfirmConnection, new ConfirmConnection()), cryptographyService.GetAes());
                        }
                        else {
                            AnsiConsole.Markup("[red]Server did not send a valid response![/]\n");
                        }
                    }
                }
            });


            while (true){
                // press escape to exit
                if (Console.ReadKey(true).Key == ConsoleKey.Escape){
                    Transfer.sendMessageAES(stream, new Message(MessageType.Disconnect, new DisconnectMessage()), cryptographyService.GetAes());
                    Log(Logger.Info, $"Disconnected from server!");
                    break;
                }
                Transfer.sendMessageAES(stream, new Message(MessageType.Message, new UserMessage(AnsiConsole.Ask<string>(">"), cryptographyService.GetSessionToken())), cryptographyService.GetAes());
            }
            

            

        }
        public static void Log(Logger log, string message){
            switch(log){
                case Logger.Info:
                    AnsiConsole.Markup($"[bold green][[Info]][/]");
                    break;
                case Logger.Warning:
                    AnsiConsole.Markup($"[bold yellow][[Warning]][/]");
                    break;
                case Logger.Error:
                    AnsiConsole.Markup($"[bold red][[Error]][/]");
                    break;
            }
            AnsiConsole.Markup($":[bold white][[{DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")}]][/] ");
            AnsiConsole.Markup($"{message}\n");
        }
    }
}