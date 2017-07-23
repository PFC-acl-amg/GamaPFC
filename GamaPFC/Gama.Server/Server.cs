using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Gama.Server
{
    public class Server
    {
        public static Hashtable ClientList = new Hashtable();

        static void Main(string[] args)
        {
            StartListening();
        }

        public static void StartListening()
        {
            TcpListener serverSocket = new TcpListener(IPAddress.Any, 8888);
            TcpClient clientSocket = default(TcpClient);

            serverSocket.Start();

            Console.WriteLine("Gama Server Started ....");

            string dataFromClient = null;

            try
            {
                while ((true))
                {
                    clientSocket = serverSocket.AcceptTcpClient();

                    byte[] bytesFrom = new byte[10025];
                    int bufferSize = 0;
                    bufferSize = clientSocket.ReceiveBufferSize;
                    bufferSize = 8196;

                    NetworkStream networkStream = clientSocket.GetStream();
                    networkStream.Read(bytesFrom, 0, bufferSize);

                    dataFromClient = System.Text.Encoding.ASCII.GetString(bytesFrom);
                    dataFromClient = dataFromClient.Substring(0, dataFromClient.IndexOf("$"));

                    string clientName = dataFromClient.Remove(dataFromClient.IndexOf("INICIO DE CONEXION"), "INICIO DE CONEXION".Length);

                    if (!ClientList.Contains(clientName))
                        ClientList.Add(clientName, clientSocket);

                    Console.WriteLine($"<NUEVA CONEXION> <{clientName}> se ha conectado");

                    ClientHandler client = new ClientHandler();
                    client.Start(clientSocket, clientName, ClientList);
                }
            }
            catch (Exception ex)
            {
                clientSocket.Close();
                if (dataFromClient != null)
                    ClientList.Remove(dataFromClient);
                serverSocket.Stop();
                Console.WriteLine("exit");
                Console.ReadLine();
            }
        }

        public static void Broadcast(string mensaje, string senderName)
        {
            List<string> disposableClients = new List<string>();

            foreach (DictionaryEntry Item in ClientList)
            {
                string clientName = Item.Key.ToString();

                if (clientName == senderName)
                    continue;

                TcpClient broadcastSocket = (TcpClient)Item.Value;

                if (!broadcastSocket.Connected)
                {
                    disposableClients.Add(Item.Key.ToString());
                    continue;
                }

                NetworkStream broadcastStream = broadcastSocket.GetStream();

                byte[] broadcastBytes = null;
                broadcastBytes = Encoding.ASCII.GetBytes(mensaje + "$");
 
                try
                {
                    broadcastStream.Write(broadcastBytes, 0, broadcastBytes.Length);
                    broadcastStream.Flush();
                    Console.WriteLine($"...<ENVIO>       <{senderName}> ha enviado un mensaje a <{clientName}>");
                }
                catch (IOException)
                {
                    continue;
                }
            }

            foreach (var key in disposableClients)
            {
                ClientList.Remove(key);
            }
        }  
    }


   
}