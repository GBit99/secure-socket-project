using System;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using Models;
using NetCoreServer;

namespace Client
{
    public class SocketClient : SslClient
    {
        private bool _stop;

        public SocketClient(SslContext context, string address, int port) : base(context, address, port) { }

        public void DisconnectAndStop()
        {
            _stop = true;
            DisconnectAsync();
            while (IsConnected)
            {
                Thread.Yield();
            }
        }

        protected override void OnConnected()
        {
            Console.WriteLine($"SSL client connected a new session with Id {Id}");
        }

        protected override void OnHandshaked()
        {
            Console.WriteLine($"SSL client handshaked a new session with Id {Id}");
        }

        protected override void OnDisconnected()
        {
            Console.WriteLine($"SSL client disconnected a session with Id {Id}");

            // Wait for a while...
            Thread.Sleep(1000);

            // Try to connect again4
            if (!_stop)
            {
                ConnectAsync();
            }
        }

        protected override void OnReceived(byte[] buffer, long offset, long size)
        {
            var serverMessage = Encoding.UTF8.GetString(buffer, (int)offset, (int)size);

            Console.WriteLine($"Server message:\n{serverMessage}");
        }

        protected override void OnError(SocketError error)
        {
            Console.WriteLine($"SSL client caught an error with code {error}");
        }
    }
}
