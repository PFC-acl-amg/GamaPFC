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
    class Program
    {
        public static Hashtable _ClientList = new Hashtable();

        static void Main(string[] args)
        {
            TcpListener serverSocket = new TcpListener(IPAddress.Any, 8888);
            TcpClient clientSocket = default(TcpClient);

            serverSocket.Start();

            Console.WriteLine("Chat Server Started ....");

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
                    
                    if (!_ClientList.Contains(clientName))
                        _ClientList.Add(clientName, clientSocket);

                    Console.WriteLine($"<NUEVA CONEXION> <{clientName}> se ha conectado");

                    ClientHandler client = new ClientHandler();
                    client.Start(clientSocket, clientName, _ClientList);
                }
            }
            catch (Exception ex)
            {
                clientSocket.Close();
                if (dataFromClient != null)
                    _ClientList.Remove(dataFromClient);
                serverSocket.Stop();
                Console.WriteLine("exit");
                Console.ReadLine();
            }
        }

        public static void Broadcast(string mensaje, string senderName)
        {
            List<string> disposableClients = new List<string>();

            foreach (DictionaryEntry Item in _ClientList)
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
                _ClientList.Remove(key);
            }
        }  
    }


    public class ClientHandler
    {
        TcpClient _ClientSocket;
        Hashtable _ClientsList;
        string _LastMessage = "";
        public string ClientName { get; set; }

        public void Start(TcpClient inClientSocket, string clineNo, Hashtable clientList)
        {
            this._ClientSocket = inClientSocket;
            this.ClientName = clineNo;
            this._ClientsList = clientList;
            Thread ctThread = new Thread(ListenClient);
            ctThread.Start();
        }

        private void ListenClient()
        {
            byte[] bytesFrom = new byte[10025];
            string dataFromClient = null;
            int _Length = Guid.NewGuid().ToString().Length;

            while ((true))
            {
                try
                {
                    NetworkStream networkStream = _ClientSocket.GetStream();
                    int bufferSize = 0;
                    bufferSize = _ClientSocket.ReceiveBufferSize;
                    bufferSize = 8196;
                    networkStream.Read(bytesFrom, 0, bufferSize);

                    dataFromClient = Encoding.ASCII.GetString(bytesFrom);
                    dataFromClient = dataFromClient.Substring(0, dataFromClient.IndexOf("$"));

                    string modulo = "";
                    if (dataFromClient.Contains("ATENCIONES"))
                        modulo = "ATENCIONES";
                    else if (dataFromClient.Contains("SOCIOS"))
                        modulo = "SOCIOS";
                    else if (dataFromClient.Contains("COOPERACION"))
                        modulo = "COOPERACION";

                    int index = dataFromClient.IndexOf("@@") + 2;
                    string message = dataFromClient.Substring(index, _Length);
                    if (message == _LastMessage)
                    {
                        _ClientSocket.Close();
                        _ClientSocket = null;
                        _ClientsList.Remove(this.ClientName);
                        Console.WriteLine($"<DESCONEXION>    <{ClientName}> se ha desconectado forzadamente desde servidor");
                        break;
                    }

                    if (dataFromClient.StartsWith("<EOC>"))
                    {
                        _ClientSocket.Close();
                        _ClientSocket = null;
                        _ClientsList.Remove(this.ClientName);
                        Console.WriteLine($"<DESCONEXION>    <{ClientName}> se ha desconectado");
                        break;
                    }
                    else
                    {
                        Console.WriteLine($"<BROADCAST>      <{ClientName}> empieza a emitir para {modulo}...");
                        index = dataFromClient.IndexOf("@@") + 2;
                        _LastMessage = dataFromClient.Substring(index, _Length);
                        Program.Broadcast($"{ClientName}@MODULO:@{modulo}$", ClientName);
                        Console.WriteLine($"</BROADCAST>     <{ClientName}> termina de emitir...");
                    }
                }
                catch (Exception ex)
                {
                    _ClientSocket.Close();
                    _ClientsList.Remove(this.ClientName);
                    Console.WriteLine($"<DESCONEXION> <{ClientName}>se ha desconectado sin cerrar la conexión. Todo OK.");
                    break;
                }
            }
        }
    } 
}