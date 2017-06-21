using Gama.Atenciones.Wpf.Eventos;
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

        private void _ConectarAlServidor()
        { 
            _ClientSocket.Connect("80.59.101.181", 8888);
            _ServerStream = _ClientSocket.GetStream();

            AtencionesResources.ClientId = System.Security.Principal.WindowsIdentity.GetCurrent().Name + Guid.NewGuid().ToString();
            _ClientName = AtencionesResources.ClientId.ToString();

            EnviarMensaje($"{AtencionesResources.ClientId}");

            _ClientThread = new Thread(_RecibirMensaje);
            _ClientThread.Start();
        }

        private void _RecibirMensaje()
        {
            while (true)
            {
                _ServerStream = _ClientSocket.GetStream();

                int bufferSize = 0;
                byte[] inStream = new byte[10025];
                bufferSize = _ClientSocket.ReceiveBufferSize;
                bufferSize = 8196;

                _ServerStream.Read(inStream, 0, bufferSize);

                _EventAggregator.GetEvent<ServidorActualizadoDesdeFueraEvent>().Publish();
            }
        }

        public void EnviarMensaje(string mensaje)
        {
            byte[] message = Encoding.ASCII.GetBytes(mensaje + "$");
            _ServerStream.Write(message, 0, message.Length);
            _ServerStream.Flush();
        }

        public void Desconectar()
        {
            EnviarMensaje("<EOC>" + _ClientName + "XXX");
            _ClientSocket.Close();
            _ClientThread.Abort();
        }
    }
}
