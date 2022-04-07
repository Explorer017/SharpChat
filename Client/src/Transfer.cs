using System;
using System.Xml;
using System.Xml.Serialization;
using System.Net;
using System.Net.Sockets;


namespace SharpChatClient{
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
                return (Message)serializer.Deserialize(stream);
            }
        }

        public static byte[] StreamToByteArray(NetworkStream stream, int length){
            byte[] buffer = new byte[length];
            stream.Read(buffer, 0, length);
            return buffer;
        }

        public static Message recieveMessage(NetworkStream stream){
            int length = stream.ReadByte();
            byte[] bytes = StreamToByteArray(stream, length);
            return ByteArrayToMessage(bytes);
        }

        public static Message sendMessage(NetworkStream stream, Message message){
            byte[] bytes = MessageToByteArray(message);
            stream.WriteByte((byte)bytes.Length);
            stream.Write(bytes, 0, bytes.Length);
            return message;
        }
    }
}