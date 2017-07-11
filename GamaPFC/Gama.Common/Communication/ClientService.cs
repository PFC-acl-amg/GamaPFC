using Gama.Common.Eventos;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Gama.Common.Communication
{
    public class ClientService
    {
        public const string INICIO_DE_CONEXION = "INICIO DE CONEXION";
        public const string FIN_DE_CONEXION = "<EOC>";
        public TcpClient ClientSocket;
        public NetworkStream ServerStream;
        public string ReadData;
        public string ClientName;
        public Thread ClientThread;
        EventAggregator _EventAggregator;

        public ClientService(EventAggregator eventAggregator, string clientName)
        {
            ClientSocket = new TcpClient();
            ServerStream = default(NetworkStream);
            ReadData = null;
            ClientName = clientName;

            _EventAggregator = eventAggregator;
            Conectar();
        }

        public bool IsConnected()
        {
            return (ClientSocket != null && ClientSocket.Client != null && ClientSocket.Connected);
        }

        public void TryConnect()
        {
            Conectar();
        }

        public void Conectar()
        {
            try
            {
                if (IsConnected())
                {
                    _EventAggregator.GetEvent<LaConexionConElServidorHaCambiadoEvent>().Publish(MensajeDeConexion.Conectado);
                    return;
                }

                ClientSocket.Close();
                ClientSocket = new TcpClient();
                ServerStream = default(NetworkStream);
                ClientSocket.Connect("localhost", 8888); //"80.59.101.181"
                ServerStream = ClientSocket.GetStream();
                
                EnviarMensaje($"{INICIO_DE_CONEXION}{ClientName}");

                ClientThread = new Thread(RecibirMensaje);
                ClientThread.Start();

                _EventAggregator.GetEvent<LaConexionConElServidorHaCambiadoEvent>().Publish(MensajeDeConexion.Conectado);
            }
            catch (Exception ex)
            {
                _EventAggregator.GetEvent<LaConexionConElServidorHaCambiadoEvent>().Publish(MensajeDeConexion.NoConectado);
                // No se ha podido conectar con la herramienta servidor
            }
        }

        public void RecibirMensaje()
        {
            try
            {
                while (true)
                {
                    ServerStream = ClientSocket.GetStream();

                    int bufferSize = 0;
                    byte[] inStream = new byte[10025];
                    bufferSize = ClientSocket.ReceiveBufferSize;
                    bufferSize = 8196;

                    ServerStream.Read(inStream, 0, bufferSize);

                    string dataFromServer = Encoding.ASCII.GetString(inStream);
                    dataFromServer = dataFromServer.Substring(0, dataFromServer.IndexOf("$"));

                    if (!dataFromServer.Contains(INICIO_DE_CONEXION))
                        _EventAggregator.GetEvent<ServidorActualizadoDesdeFueraEvent>().Publish(dataFromServer);
                }
            }
            catch (Exception ex)
            {
                _EventAggregator.GetEvent<LaConexionConElServidorHaCambiadoEvent>().Publish(MensajeDeConexion.NoConectado);
                // El servidor se ha desconectado
            }
        }

        public void EnviarMensaje(string mensaje)
        {
            try
            {
                byte[] message = Encoding.ASCII.GetBytes(mensaje + "$");
                ServerStream.Write(message, 0, message.Length);
                ServerStream.Flush();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);// El servidor se ha desconectado
            }
        }

        public void Desconectar()
        {
            if (ClientSocket.Connected)
            {
                EnviarMensaje($"<EOC>Cliente {ClientName} ha hecho un broadcast @@{Guid.NewGuid()}%%");
                ClientSocket.Close();
                ClientThread.Abort();
            }
        }
    }
}
