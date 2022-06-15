using System;
using System.Xml;
using System.Xml.Serialization;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Security.Cryptography;


namespace SharpChatServer{
    class Transfer{

        /// <summary>
        /// <para>Serializes an object to XML, then converts it to a byte array.</para>
        /// <para>The object must be serializable.</para>
        /// </summary>
        public static byte[] MessageToByteArray(Message message){
            XmlSerializer serializer = new XmlSerializer(typeof(Message));
            using(MemoryStream stream = new MemoryStream()){
                serializer.Serialize(stream, message);
                return stream.ToArray();
            }
        }

        /// <summary>
        /// <para>Converts a byte array to an object.</para>
        /// <para>The object must be serializable.</para>
        /// </summary>
        public static Message ByteArrayToMessage(byte[] bytes){
            XmlSerializer serializer = new XmlSerializer(typeof(Message));
            using(MemoryStream stream = new MemoryStream(bytes)){
                return (Message?)serializer.Deserialize(stream) ?? throw new Exception("Could not deserialize message.");
            }
        }

        public static byte[] StreamToByteArray(NetworkStream stream, int length){
            byte[] buffer = new byte[length];
            stream.Read(buffer, 0, length);
            return buffer;
        }

        public static Message recieveMessage(NetworkStream stream){
            byte[] length = new byte[4];
            stream.Read(length, 0, length.Length);
            int lengthInt = BitConverter.ToInt32(length, 0);
            byte[] bytes = StreamToByteArray(stream, lengthInt);
            return ByteArrayToMessage(bytes);
        }
        public static Message sendMessage(NetworkStream stream, Message message){
            byte[] bytes = MessageToByteArray(message);
            //Console.WriteLine(BitConverter.ToString(bytes));
            stream.Write(BitConverter.GetBytes(bytes.Length), 0, 4);
            stream.Write(bytes, 0, bytes.Length);
            return message;
        }

        public static Message sendMessageRSA(NetworkStream stream, Message message, RSA rsa){
            byte[] bytes = MessageToByteArray(message);
            Console.WriteLine(Encoding.UTF8.GetString(bytes));
            byte[] encrypted = rsa.Encrypt(bytes, RSAEncryptionPadding.Pkcs1);
            Console.WriteLine(Encoding.UTF8.GetString(encrypted));
            stream.Write(BitConverter.GetBytes(encrypted.Length), 0, 4);
            stream.Write(encrypted, 0, encrypted.Length);
            return message;
        }

        public static Message receiveMessageRSA(NetworkStream stream, RSA rsa){
            byte[] length = new byte[4];
            stream.Read(length, 0, length.Length);
            int lengthInt = BitConverter.ToInt32(length, 0);
            byte[] bytes = StreamToByteArray(stream, lengthInt);
            byte[] decrypted = rsa.Decrypt(bytes, RSAEncryptionPadding.Pkcs1);
            return ByteArrayToMessage(decrypted);
        }
        public static Message sendMessageAES(NetworkStream stream, Message message, Aes aes){
            byte[] bytes = MessageToByteArray(message);
            byte[] encrypted = aes.CreateEncryptor().TransformFinalBlock(bytes, 0, bytes.Length);
            stream.Write(BitConverter.GetBytes(encrypted.Length), 0, 4);
            stream.Write(encrypted, 0, encrypted.Length);
            return message;
        }

        public static Message receiveMessageAES(NetworkStream stream, Aes aes){
            byte[] length = new byte[4];
            stream.Read(length, 0, length.Length);
            int lengthInt = BitConverter.ToInt32(length, 0);
            Server.Log(Logger.Info, "Length: " + lengthInt);
            byte[] bytes = StreamToByteArray(stream, lengthInt);
            Server.Log(Logger.Info, "Bytes: " + BitConverter.ToString(bytes));
            byte[] decrypted = aes.CreateDecryptor().TransformFinalBlock(bytes, 0, bytes.Length);
            Server.Log(Logger.Info, "Decrypted message: " + Encoding.UTF8.GetString(decrypted));
            return ByteArrayToMessage(decrypted);
        }
    }
}