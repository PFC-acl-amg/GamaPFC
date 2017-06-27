using Gama.Atenciones.Wpf.Eventos;
using Gama.Common.Eventos;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Gama.Atenciones.Wpf.Services
{
    public class ClientService
    {
        public const string INICIO_DE_CONEXION = "INICIO DE CONEXION";
        TcpClient _ClientSocket;
        NetworkStream _ServerStream;
        string _ReadData;
        string _ClientName;
        Thread _ClientThread;
        EventAggregator _EventAggregator;

        public ClientService(EventAggregator eventAggregator)
        {
            _ClientSocket = new TcpClient();
            _ServerStream = default(NetworkStream);
            _ReadData = null;

            _EventAggregator = eventAggregator;
            _ConectarAlServidor();
        }

        public bool IsConnected()
        {
            return (_ClientSocket != null && _ClientSocket.Connected);
        }

        public void TryConnect()
        {
            _ConectarAlServidor();
        }

        private void _ConectarAlServidor()
        { 
            try
            {
                if (_ClientSocket.Connected)
                {
                    _EventAggregator.GetEvent<LaConexionConElServidorHaCambiadoEvent>().Publish(MensajeDeConexion.Conectado);
                    return;
                }
                _ClientSocket.Connect("80.59.101.181", 8888);
                _ServerStream = _ClientSocket.GetStream();

                AtencionesResources.ClientId = Guid.NewGuid().ToString();
                _ClientName = AtencionesResources.ClientId.ToString();

                EnviarMensaje($"{INICIO_DE_CONEXION}{AtencionesResources.ClientId}");

                _ClientThread = new Thread(_RecibirMensaje);
                _ClientThread.Start();

                _EventAggregator.GetEvent<LaConexionConElServidorHaCambiadoEvent>().Publish(MensajeDeConexion.Conectado);
            }
            catch (Exception ex)
            {
                _EventAggregator.GetEvent<LaConexionConElServidorHaCambiadoEvent>().Publish(MensajeDeConexion.NoConectado);
                // No se ha podido conectar con la herramienta servidor
            }
        }

        private void _RecibirMensaje()
        {
            try
            {
                while (true)
                {
                    _ServerStream = _ClientSocket.GetStream();

                    int bufferSize = 0;
                    byte[] inStream = new byte[10025];
                    bufferSize = _ClientSocket.ReceiveBufferSize;
                    bufferSize = 8196;

                    _ServerStream.Read(inStream, 0, bufferSize);

                    string dataFromServer = Encoding.ASCII.GetString(inStream);
                    dataFromServer = dataFromServer.Substring(0, dataFromServer.IndexOf("$"));

                    if (!dataFromServer.Contains(INICIO_DE_CONEXION))
                        _EventAggregator.GetEvent<ServidorActualizadoDesdeFueraEvent>().Publish(dataFromServer);
                }
            } catch (Exception ex)
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
                _ServerStream.Write(message, 0, message.Length);
                _ServerStream.Flush();
            }
            catch (Exception)
            {
                // El servidor se ha desconectado
            }
        }

        public void Desconectar()
        {
            if (_ClientSocket.Connected)
            {
                EnviarMensaje("<EOC>");
                _ClientSocket.Close();
                _ClientThread.Abort();
            }
        }
    }
}
