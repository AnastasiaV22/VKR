using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Xml.Serialization;
using System.Xml;
using System.Runtime.Serialization.Formatters.Binary;
using lib;

namespace serverApp
{
    class Server
    {
        string dbname = "EDSTestBD"; // название базы данных

        TcpListener Listener;
        DataBase dataBace;
        string connectionStr = "Server=localhost;Port=1602;User Id=postgres;Password=1234;Database=EDSTestBD";
        List<Client> clients;

        //string connectionStr = "Server=localhost;Port=1602;User Id=postgres;Password=1234;Database=EDSTestBD"; // строка подключения к базе данных

        public Server(IPAddress ipaddress, int port) {

            Listener = new TcpListener(ipaddress, port);

            try
            {

                //подключение базы данных
                dataBace = new DataBase(connectionStr, dbname);

                //подключение прослушивателя
                Listener.Start();
                Console.WriteLine("сервер запущен");

                while (true) // прослушивание новых подключений
                {
                    //начало прослушивание данных для авторизации 
                    TcpClient tcpClient = Listener.AcceptTcpClient();

                    //новый поток для подключенного клиента
                    Thread newClientThread = new Thread(delegate() { ProcessClient(tcpClient, dataBace); });
                    newClientThread.Start();

                }
            }
            catch (Exception e)
            {

                Environment.Exit(0);
            }
    
        }

        // остановка сервера
        ~Server() {
            if (Listener != null) {

                Console.WriteLine("сервер выключен");
                Listener.Stop();
            }
        }
                // login и password из request
                //string login = "admin";
                //string password = "1234";
       static void ProcessClient(TcpClient tcpClient, DataBase _dataBace) {
            
            var stream = tcpClient.GetStream();
            var response = new List<byte>();
            int bytesRead = 10;

            Client client = new Client(tcpClient); ;

            while (true)
            {   
                //считываение запроса от клиента до его окончания 
                while ((bytesRead = stream.ReadByte()) != '\n')
                {
                    response.Add((byte)bytesRead);
                }

                //преобразование строки в кодировку UTF8 
                var res = Encoding.UTF8.GetString(response.ToArray());
                if (res == "close") break;

                //разбор команды для выполнения
                string[] clientCommand = res.Split('|');

                switch (clientCommand[0])
                {
                    case "autorization":
                        try
                        {
                            string login = clientCommand[1];
                            string password = clientCommand[2];
                            if (_dataBace.SearchAccount(login, password))
                            {
                                client = new Client(tcpClient);
                                client.userName = login;
                                _dataBace.autorization(client);
                                Console.WriteLine("Подключен: " + client.userName);
                                
                                //Отправление ответа клиенту об успешном подключении 
                                stream.Write(Encoding.UTF8.GetBytes("accepted|\n"));

                            }
                            else
                            {
                                Console.WriteLine("попытка подключения отклонена");

                                //Отправка клиенту ответа о неправильности введенных данных
                                stream.Write(Encoding.UTF8.GetBytes("denied|\n"));
                            }
                            
                        }
                        catch(Exception ex) { }
                    break;

                    case "EndingEDS":
                        try 
                        { 
                            if (client.isAuthorized)
                            {
                                List<Person> resalt = new List<Person>();
                                resalt = _dataBace.EndingEDS(client);

                                BinaryFormatter bForm;
                                bForm = new BinaryFormatter();

                                stream.Write(Encoding.UTF8.GetBytes($"rows|person|{resalt.Count}\n"));
                                foreach (Person pers in resalt){

                                    bForm.Serialize(tcpClient.GetStream(), pers);

                                  //  stream.Write(Encoding.UTF8.GetBytes("\n"));

                                }

                            }
                        }
                        catch(Exception ex) { }
                        break;

                    case "":
                        break;
                    default:
                        
                        break;
                
                
                }

                
                
            }
            stream.Close();
            tcpClient.Close();
        }

        // чтение файла
        async Task ReadFile(int len) { 
        
        }


        static void Main(string[] args) {

            string ipAddressString = "127.0.0.1"; //адрес сервера

            IPAddress address = IPAddress.Parse(ipAddressString);
            Server server;

            try
            {
                Console.WriteLine("IP-aдрес задан: " + ipAddressString);
                server = new Server(address, 1601);  // порт сервера

            }
            catch (Exception e)
            {
                Console.WriteLine("IP-адрес не задан");
            }
            
        }


    }

}