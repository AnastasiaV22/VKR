using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using lib;

namespace ClientApp
{
    internal class ServerConnection
    {
        bool isConnected;
        TcpClient client;

        public ServerConnection()
        {
            //запрос авторизации к серверу
            client = new TcpClient("127.0.0.1", 1601);
            isConnected = true;

        }
        ~ServerConnection() { }

        public string Request(string request)
        {
            // Передача запроса сервера в виде байтов
            var stream = client.GetStream();
            stream.Write(Encoding.UTF8.GetBytes(request), 0, Encoding.UTF8.GetBytes(request).Length);
            
            List<byte> byteResponse = new List<byte>();
            int bytesRead = 10;

            //считываение ответа от сервера
            while ((bytesRead = stream.ReadByte()) != '\n')
            {
                byteResponse.Add((byte)bytesRead);
            }
            stream.Close();

            //Преобразование последовательности байтов в строку
            return Encoding.UTF8.GetString(byteResponse.ToArray());

        }

        public string[,] RequestRows(string request) {
            var stream = client.GetStream();
            stream.Write(Encoding.UTF8.GetBytes(request), 0, Encoding.UTF8.GetBytes(request).Length);

            List<byte> byteResponse = new List<byte>();
            int bytesRead = 10;

            //считываение ответа от сервера
            while ((bytesRead = stream.ReadByte()) != '\n')
            {
                byteResponse.Add((byte)bytesRead);
            }
            string[] serverResponce = Encoding.UTF8.GetString(byteResponse.ToArray()).Split('|');

            List<Person> rows = new List<Person>();

            BinaryFormatter bForm;
            bForm = new BinaryFormatter();
            string[,] rowsPers = new string[Convert.ToInt32(serverResponce[2])-1,5];

            if (serverResponce[1] == "Person")
            {
                for (int i = 0; i < Convert.ToInt32(serverResponce[2])-1; i++)
                {
                    Person pers_ = ((Person) bForm.Deserialize(client.GetStream()));
                    
                    rowsPers[i, 0] = Convert.ToString(pers_.id);
                    rowsPers[i, 1] = pers_.surname + " " + pers_.name + " " + pers_.fathername;
                    rowsPers[i, 2] = pers_.GetWorkPlace().department + "/" + pers_.GetWorkPlace().division;
                    rowsPers[i, 3] = Convert.ToString(pers_.GetLastEDS().dateofend);
                    rowsPers[i, 4] = pers_.GetLastEDS().status;
                }

                return rowsPers;

            }

            // другие варианты
            return null;

        }



    }
}
