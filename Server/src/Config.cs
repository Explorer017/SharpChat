using System;
using Spectre.Console;
using System.Net;

namespace SharpChatServer{
    class Config{
        public string name;
        public string version;
        public string ip;
        public int port;
        public string motd;
        public string database;

        public Config(string FileName){
            // check if file exists
            if(!System.IO.File.Exists(FileName)){
                Server.Log(Logger.Warning, "Config file not found, creating new one!");
                // create file
                System.IO.File.Create(FileName).Close();
                System.IO.File.WriteAllText(FileName,"Name=SharpChatServer\nVersion=2.0.1\nMOTD=This is the Default MOTD.\nIP=127.0.0.1\nPort=12340\nDatabase=Database.db3");
            }
            // Read the config file
            string[] lines = System.IO.File.ReadAllLines(FileName);
            // loop through the lines
            foreach(string line in lines){
                // Split the line into a key and value
                string[] split = line.Split('=');
                // Check if the key is valid
                if(!Enum.IsDefined(typeof(ConfigKey), split[0])){
                    // If not, throw an exception
                    throw new Exception("Invalid config key: " + split[0]);
                }

                switch((ConfigKey)Enum.Parse(typeof(ConfigKey), split[0])){
                    case ConfigKey.Name:
                        name = split[1];
                        break;
                    case ConfigKey.Version:
                        version = split[1];
                        break;
                    case ConfigKey.IP:
                        ip = split[1];
                        break;
                    case ConfigKey.Port:
                        port = int.Parse(split[1]);
                        break;
                    case ConfigKey.MOTD:
                        motd = split[1];
                        break;
                    case ConfigKey.Database:
                        database = split[1];
                        break;
                }
            }
            // check if all values are set
            if(name == null || version == null || ip == null || port == 0 || motd == null){
                throw new Exception("Config file is missing values!");
            }
            
        }

        public IPAddress GetIP(){
            return IPAddress.Parse(ip);
        }
    }

    public enum ConfigKey{
        Name,
        Version,
        IP,
        Port,
        DatabaseUsername,
        DatabasePassword,
        MOTD,
        Database
    }
}