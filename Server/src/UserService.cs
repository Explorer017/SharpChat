using System;
using Spectre.Console;
using System.Security.Cryptography;
using System.Text;

namespace SharpChatServer{
    class UserService{
        Database database;
        List<User> users = new List<User>();
        public UserService(Database database){
            this.database = database;
        }

        private byte[] GenerateSalt(int length) {
            RandomNumberGenerator rng = RandomNumberGenerator.Create();
            byte[] salt = new byte[length];
            rng.GetBytes(salt);
            return salt;
        }

        private byte[] GenerateHash(byte[] password, byte[] salt, int iterations, int length){
            using (var deriveBytes = new Rfc2898DeriveBytes(password, salt, iterations)){
                return deriveBytes.GetBytes(length);
            }
        }

        public bool Register(string username, string password){
            if(database.CheckIfNameExists(username) == true){
                return false;
            }
            byte[] salt = GenerateSalt(32);
            byte[] hash = GenerateHash(Encoding.UTF8.GetBytes(password), salt, 10000, 32);
            database.Store(username, hash, salt, 10000);
            return true;
        }
        public bool Login(string username, string password){
            byte[] hash = database.getPasswordHash(username);
            if(hash == null){
                return false;
            }
            byte[] salt = database.getSalt(username);
            if (salt == null){
                return false;
            }
            byte[] hash2 = GenerateHash(Encoding.UTF8.GetBytes(password), salt, 10000, 32);
            if(hash.Length != hash2.Length){
                return false;
            }
            for(int i = 0; i < hash.Length; i++){
                if(hash[i] != hash2[i]){
                    return false;
                }
            }
            return true;
        }
    }
}