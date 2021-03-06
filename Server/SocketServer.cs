using System;
using System.Net;
using System.Net.Sockets;
using NetCoreServer;

namespace Server
{
    public class SocketServer : SslServer
    {
        public SocketServer(SslContext context, IPAddress address, int port) : base(context, address, port) { }

        protected override SslSession CreateSession() { return new SocketSession(this); }

        protected override void OnError(SocketError error)
        {
            Console.WriteLine($"SSL server caught an error with code {error}");
        }
    }
}
