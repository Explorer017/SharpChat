using System;
using Spectre.Console;
using System.Net.Sockets;
using System.Net;

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
            NetworkStream stream = client.GetStream();
            Log(Logger.Info, $"Connected to server!");
            Transfer.sendMessage(stream, new Message(MessageType.Connect,"Hello World!"));
            Log(Logger.Info, $"Sent message!");
            while (true){
                // press escape to exit
                if (Console.ReadKey(true).Key == ConsoleKey.Escape){
                    Transfer.sendMessage(stream, new Message(MessageType.Disconnect, "Goodbye World!"));
                    Log(Logger.Info, $"Disconnected from server!");
                    break;
                }
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
            AnsiConsole.Markup($":[bold white][[{DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss")}]][/] ");
            AnsiConsole.Markup($"{message}\n");
        }
    }
}