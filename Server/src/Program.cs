using System;
using Spectre.Console;
using System.Net.Sockets;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SharpChatServer{
    class Server{
        public static Config config;
        static void Main(string[] args){
            AnsiConsole.Markup("[bold green]SharpChat Server[/]\n[bold yellow]Version: 2.0.1[/]\n");
            config = new Config("server.cfg");
            Log(Logger.Info, "Config loaded!");
            Database database = new Database(config.database);
            Log(Logger.Info, "Database loaded!");
            UserService userService = new UserService(database);
            Log(Logger.Info, "UserService loaded!");

            // Start TCP Listeners
            IPEndPoint ipEndPoint = new IPEndPoint(config.GetIP(), config.port);
            TcpListener listener = new TcpListener(ipEndPoint);
            listener.Start();
            Log(Logger.Info, "Listening on " + ipEndPoint.ToString());
            Log(Logger.Info, "Server is up!");
            while (!!!false){
                if (listener.Pending()){
                    Task.Run(() =>
                        {
                            User? user = User.Create(listener.AcceptTcpClient(), userService);
                            Message? message = null;
                            if (user != null){
                                userService.AddUser(user);
                                Log(Logger.Info, "User " + user.GetUsername() + " connected!");
                            } else {
                                Log(Logger.Error, "User could not be created!");
                                // TODO: Handle this error
                            }
                            bool isRunning = true;
                            while (isRunning){
                                try{
                                    message = Transfer.receiveMessageAES(user.GetClient().GetStream(), user.GetAES());
                                } catch(IOException e){
                                    Log(Logger.Warning, "User " + user.GetUsername() + " disconnected without sending a disconnect message!");
                                    //TODO: userService.RemoveUser(user);
                                    isRunning = false;
                                } catch (Exception e){
                                    Log(Logger.Error, "An error occured while receiving a message!");
                                    AnsiConsole.WriteException(e);
                                    isRunning = false;
                                }
                                if (message.type == MessageType.Disconnect){
                                    Log(Logger.Info, "User " + user.GetUsername() + " disconnected!");
                                    isRunning = false;
                                }
                                else if (message.type == MessageType.Message){
                                    Log(Logger.Info, $"{user.GetClient().Client.RemoteEndPoint} sent a message: {message.field1}");
                                }
                            }
                        }
                    );
                }
            }
        }

        public static void Log(Logger log, string message){
            switch(log){
                case Logger.Info:
                    AnsiConsole.Markup($"[bold green][[Info]][/]");
                    break;
                case Logger.Warning:
                    AnsiConsole.Markup($"[bold yellow][[Warn]][/]");
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