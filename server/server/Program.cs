using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace server
{
    class Program
    {
        static List<position> ord;

        static Byte[] switcher (string mes)
        {
            string[] comand = mes.Split(new char[] { '@' });
            string[] edit = comand[1].Split(new char[] { '#' });

            switch (comand[0])
            {
                case "add":
                    {
                        ord.Add(new position(edit[0], Convert.ToInt32(edit[1])));
                    }
                    break;
                case "view":
                    {
                        string str = "";

                        foreach (var item in ord)
                        {
                            str += $"@{item.title}#{item.cost.ToString()}";
                        }
                        return (Encoding.UTF8.GetBytes(str));
                    }
                case "delall":
                    {
                        ord = null;
                    }
                    break;
                case "del":
                    {
                        ord.Remove(ord.Find(x => x.title == edit[0]));
                    }
                    break;
                case "edit":
                    {
                        ord.Remove(ord.Find(x => x.title == edit[0]));
                        ord.Add(new position(edit[0], Convert.ToInt32(edit[1])));
                    }
                    break;
                case "price":
                    {
                        int price = 0;

                        foreach (var item in ord)
                        {
                            price += Convert.ToInt32(item.cost);
                        }
                        return (Encoding.UTF8.GetBytes(price.ToString()));
                    }
                default:
                    break;
            }
            return (Encoding.UTF8.GetBytes(""));
        }

        static void Main(string[] args)
        {
            IPAddress localAddr = IPAddress.Parse("127.0.0.1");
            int port = 8888;
            TcpListener server = new TcpListener(localAddr, port);
            ord = new List<position>();
            server.Start();
            Console.WriteLine("Сервер запущен!");

            while (true)
            {
                try
                {
                    // Подключение клиента
                    TcpClient client = server.AcceptTcpClient();
                    NetworkStream stream = client.GetStream();
                    // Обмен данными
                    try
                    {
                        if (stream.CanRead)
                        {
                            byte[] myReadBuffer = new byte[1024];
                            StringBuilder myCompleteMessage = new StringBuilder();
                            int numberOfBytesRead = 0;
                            do
                            {
                                numberOfBytesRead = stream.Read(myReadBuffer, 0, myReadBuffer.Length);
                                myCompleteMessage.AppendFormat("{0}", Encoding.UTF8.GetString(myReadBuffer, 0, numberOfBytesRead));
                            }
                            while (stream.DataAvailable);
                            Byte[] responseData = switcher(myCompleteMessage.ToString());
                            stream.Write(responseData, 0, responseData.Length);
                        }
                    }
                    finally
                    {
                        stream.Close();
                        client.Close();
                    }
                }
                catch
                {
                    server.Stop();
                    break;
                }
            }
        }
    }
}
