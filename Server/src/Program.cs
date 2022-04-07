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
        static void Main(string[] args){
            AnsiConsole.Markup("[bold green]SharpChat Server[/]\n[bold yellow]Version: 2.0.1[/]\n");
            Config config = new Config("server.cfg");
            Log(Logger.Info, "Config loaded!");
            IPEndPoint ipEndPoint = new IPEndPoint(config.GetIP(), config.port);
            TcpListener listener = new TcpListener(ipEndPoint);
            listener.Start();
            Log(Logger.Info, "Listening on " + ipEndPoint.ToString());

            while (true){
                if (listener.Pending()){
                    TcpClient client = listener.AcceptTcpClient();
                    Log(Logger.Info, "Client connected!");
                    
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