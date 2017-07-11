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
    public class ClientHandler
    {
        public TcpClient ClientSocket;
        public Hashtable ClientList;
        public string LastMessage = "";
        public string ClientName { get; set; }

        public void Start(TcpClient inClientSocket, string clineNo, Hashtable clientList)
        {
            this.ClientSocket = inClientSocket;
            this.ClientName = clineNo;
            this.ClientList = clientList;
            Thread ctThread = new Thread(ListenClient);
            ctThread.Start();
        }

        public void ListenClient()
        {
            byte[] bytesFrom = new byte[10025];
            string dataFromClient = null;
            int _Length = Guid.NewGuid().ToString().Length;

            while ((true))
            {
                try
                {
                    NetworkStream networkStream = ClientSocket.GetStream();
                    int bufferSize = 0;
                    bufferSize = ClientSocket.ReceiveBufferSize;
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
                    if (message == LastMessage)
                    {
                        ClientSocket.Close();
                        ClientSocket = null;
                        ClientList.Remove(this.ClientName);
                        Console.WriteLine($"<DESCONEXION>    <{ClientName}> se ha desconectado forzadamente desde servidor");
                        break;
                    }

                    if (dataFromClient.StartsWith("<EOC>"))
                    {
                        ClientSocket.Close();
                        ClientSocket = null;
                        ClientList.Remove(this.ClientName);
                        Console.WriteLine($"<DESCONEXION>    <{ClientName}> se ha desconectado");
                        break;
                    }
                    else
                    {
                        Console.WriteLine($"<BROADCAST>      <{ClientName}> empieza a emitir para {modulo}...");
                        index = dataFromClient.IndexOf("@@") + 2;
                        LastMessage = dataFromClient.Substring(index, _Length);
                        Server.Broadcast($"{ClientName}@MODULO:@{modulo}$", ClientName);
                        Console.WriteLine($"</BROADCAST>     <{ClientName}> termina de emitir...");
                    }
                }
                catch (Exception ex)
                {
                    ClientSocket.Close();
                    ClientList.Remove(this.ClientName);
                    Console.WriteLine($"<DESCONEXION> <{ClientName}>se ha desconectado sin cerrar la conexión. Todo OK.");
                    break;
                }
            }
        }
    }
}
