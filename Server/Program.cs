using System;
using System.Net;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using NetCoreServer;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            // SSL server port
            int port = 2222;

            // If arguments are provided use the first one as a port number
            if (args.Length > 0)
            {
                port = int.Parse(args[0]);
            }

            Console.WriteLine($"SSL server port: {port}");

            Console.WriteLine();

            // Create and prepare a new SSL server context using TLS protocol
            // and pass the path for the server certificate.
            var context = new SslContext(SslProtocols.Tls12,
                new X509Certificate2(@"C:\Users\user\Downloads\server.pfx", "qwerty"));

            // Create a new SSL server instance
            var server = new SocketServer(context, IPAddress.Any, port);

            // Start the server
            Console.Write("Server starting...");
            server.Start();
            Console.WriteLine("Done!");

            Console.WriteLine("Press Enter to stop the server or '!' to restart the server...");

            // Perform text input
            for (;;)
            {
                string line = Console.ReadLine();

                if (string.IsNullOrEmpty(line))
                {
                    break;
                }

                // Restart the server if a special symbol is inputed
                if (line == "!")
                {
                    Console.Write("Server restarting...");
                    server.Restart();
                    Console.WriteLine("Done!");
                    continue;
                }

                //??? this might not be needed
                // Multicast admin message to all sessions
                line = "(admin) " + line;
                server.Multicast(line);
            }

            // Stop the server
            Console.Write("Server stopping...");
            server.Stop();
            Console.WriteLine("Done!");
        }
    }
}
