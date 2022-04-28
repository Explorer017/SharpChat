using System;
using System.Data.SQLite;

namespace SharpChatServer{
    class Database{

        //TODO: fix the "Non-nullable field" error (there is no way they can be null anyway so aaaaaaaaaaaa)
        private static SQLiteConnection connection;
        private static SQLiteCommand command;
        public Database(string location){

            if (!System.IO.File.Exists("database.db3"))
            {
                Server.Log(Logger.Warning,"Database not found, creating new one...");
                string createTableQuery = @"CREATE TABLE IF NOT EXISTS [LOGIN] (
                [ID] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                [USERNAME] NVARCHAR(2048)  NULL,
                [PSWDHASH] VARCHAR(2048)  NULL,
                [SALT] VARCHAR(2048)  NULL,
                [ITERATIONS] INTEGER  NULL
                )";
                connection = new SQLiteConnection("data source=" + location) ?? throw new Exception("Could not create database!");
                command = new SQLiteCommand(connection) ?? throw new Exception("Could not create command!");
                connection.Open();
                command.CommandText = createTableQuery;
                command.ExecuteNonQuery();
                connection.Close();
            }
            else{
                connection = new SQLiteConnection("data source=" + location) ?? throw new Exception("Could not connect to database!");
                command = new SQLiteCommand(connection) ?? throw new Exception("Could not create command!");
            }
        }
        /// <summary>
        /// Adds a new user to the database
        /// </summary>
        public void Store(string username, byte[] password, byte[] salt, int iterations){
            connection.Open();
            command.CommandText = "INSERT INTO LOGIN (USERNAME, PSWDHASH, SALT, ITERATIONS) VALUES (@USERNAME, @PSWDHASH, @SALT, @ITERATIONS)";
            command.Parameters.AddWithValue("@USERNAME", username);
            command.Parameters.AddWithValue("@PSWDHASH", ByteArrToString(password));
            command.Parameters.AddWithValue("@SALT", ByteArrToString(salt));
            command.Parameters.AddWithValue("@ITERATIONS", iterations);
            command.ExecuteNonQuery();
            connection.Close();
        }
        /// <summary>
        /// Checks if a username exists in the database
        /// </summary>
        public bool CheckIfNameExists(string username){
            connection.Open();
            command.CommandText = "SELECT * FROM LOGIN WHERE USERNAME = @USERNAME";
            command.Parameters.AddWithValue("@USERNAME", username);
            SQLiteDataReader reader = command.ExecuteReader();
            if(reader.HasRows){
                reader.Close();
                connection.Close();
                return true;
            }
            reader.Close();
            connection.Close();
            return false;
        }

        public byte[]? getPasswordHash(string username){
            connection.Open();
            command.CommandText = "SELECT PSWDHASH FROM LOGIN WHERE USERNAME = @USERNAME";
            command.Parameters.AddWithValue("@USERNAME", username);
            SQLiteDataReader reader = command.ExecuteReader();
            if(reader.HasRows){
                reader.Read();
                byte[] hash = StringToByteArr((string)reader["PSWDHASH"]);
                reader.Close();
                connection.Close();
                return hash;
            }
            reader.Close();
            connection.Close();
            return null;
        }

        public byte[]? getSalt(string username){
            connection.Open();
            command.CommandText = "SELECT SALT FROM LOGIN WHERE USERNAME = @USERNAME";
            command.Parameters.AddWithValue("@USERNAME", username);
            SQLiteDataReader reader = command.ExecuteReader();
            if(reader.HasRows){
                reader.Read();
                byte[] salt = StringToByteArr((string)reader["SALT"]);
                reader.Close();
                connection.Close();
                return salt;
            }
            reader.Close();
            connection.Close();
            return null;
        }

        private string ByteArrToString(byte[] bytes){
            return BitConverter.ToString(bytes);
        }
        private byte[] StringToByteArr(string str){
            string[] strArr = str.Split('-');
            byte[] bytes = new byte[strArr.Length];
            for(int i = 0; i < strArr.Length; i++){
                bytes[i] = Convert.ToByte(strArr[i], 16);
            }
            return bytes;
        }
    }
}