using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BnbServer
{
    class Program
    {
        public static Hashtable clientList = new Hashtable();
        static void Main(string[] args)
        {
            TcpListener serverSocket = new TcpListener(System.Net.IPAddress.Any, 7777);

            TcpClient clientSocket = default(TcpClient);
            int counter = 0;

            serverSocket.Start();
            Console.WriteLine("Chat Server Started....");
            while (true)
            {
                Console.WriteLine("Chat Server Waiting....");
                clientSocket = serverSocket.AcceptTcpClient();
                counter++;

                byte[] bytesFrom = new byte[clientSocket.ReceiveBufferSize];
                string dataFromClient = null;

                NetworkStream networkStream = clientSocket.GetStream();
                networkStream.Read(bytesFrom, 0, clientSocket.ReceiveBufferSize);
                dataFromClient = Encoding.UTF8.GetString(bytesFrom);
                dataFromClient = dataFromClient.Substring(0, dataFromClient.IndexOf("$"));

                clientList.Add(dataFromClient, clientSocket);
                Console.WriteLine(dataFromClient + " Joined");

                //if(counter == 1)
                //{
                //    broadcast("'" + dataFromClient + "'님이 채팅중입니다.", dataFromClient, false);
                //}
                //else if(counter == 2)
                //{
                //    //List<string[]> keys = clientList.Keys.Cast<string[]>().ToList();
                //    //string[] e = new string[2];
                //    //e = keys[0];
                //    //foreach (DictionaryEntry item in clientList)
                //    //{
                //    //    string b = item.k
                //    //    dataFromClient = b;
                //    //}
                //    broadcast("'" + "jody816" + "'님이 채팅중입니다.", dataFromClient, false);

                //    broadcast("'" + dataFromClient + "'님이 채팅중입니다.", dataFromClient, false);
                //}

                if (counter == 1)
                {
                    broadcast("'" + dataFromClient + "'님이 채팅중입니다.", dataFromClient, false);
                }
                else if (counter == 2)
                {
                    foreach (DictionaryEntry item in clientList)
                    {
                        broadcast("'" + item.Key + "'님이 채팅중입니다.", dataFromClient, false);
                        Thread.Sleep(100);
                    }
                }

                handleClient client = new handleClient();
                client.startClient(clientSocket, dataFromClient, clientList);
            }
        }

        private static void broadcast(string msg, string uName, bool flag)
        {
            TcpClient broadcastSocket;
            NetworkStream broadcastStream;
            byte[] broadcastBytes = null; // new byte[100]
            foreach (DictionaryEntry item in clientList)
            {
                broadcastSocket = (TcpClient)item.Value; //Key는 아이디 Value는 클라이언트 소켓
                broadcastStream = broadcastSocket.GetStream();
                if (flag)
                    broadcastBytes = Encoding.UTF8.GetBytes(uName + " : " + msg);
                else
                    broadcastBytes = Encoding.UTF8.GetBytes(msg);

                broadcastStream.Write(broadcastBytes, 0, broadcastBytes.Length);
                broadcastStream.Flush();
            }
        }

        //private static void showid(string uName, int counter, bool flag)
        //{
        //    TcpClient showidSocket;
        //    NetworkStream showidStream;
        //    byte[] showidBytes = null;
            
        //    if(counter == 1)
        //    {
        //        foreach(string item in clientList)
        //        {
        //            showidSocket = 
        //        }
        //    }

        //}

        private class handleClient
        {
            TcpClient clientSocket;
            string clNo;
            Hashtable clientList;

            //public handleClient()
            //{

            //}
            public void startClient(TcpClient clientSocket, string dataFromClient, Hashtable clientList)
            {
                this.clientSocket = clientSocket;
                this.clNo = dataFromClient;
                this.clientList = clientList;

                Thread clThread = new Thread(doChat);
                clThread.Start();
            }

            private void doChat()
            {
                byte[] byteFrom = new byte[clientSocket.ReceiveBufferSize];
                string dataFromClient = null;
                NetworkStream networkStream = null;
                while (true)
                {
                    try
                    {
                        networkStream = clientSocket.GetStream();
                        networkStream.Read(byteFrom, 0, clientSocket.ReceiveBufferSize);
                        dataFromClient = Encoding.UTF8.GetString(byteFrom);
                        dataFromClient = dataFromClient.Substring(0, dataFromClient.IndexOf("$"));

                        Console.WriteLine("From Client - " + clNo + " : " + dataFromClient);
                        Program.broadcast(dataFromClient, clNo, true);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                }
            }
        }
    }
}
