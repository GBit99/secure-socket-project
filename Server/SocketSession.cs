using System;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Models;
using NetCoreServer;

namespace Server
{
    public class SocketSession : SslSession
    {
        private DataModel savedData;

        public SocketSession(SslServer server) : base(server) { }

        protected override void OnConnected()
        {
            Console.WriteLine($"SSL session with Id {Id} connected!");
        }

        protected override void OnHandshaked()
        {
            Console.WriteLine($"SSL session with Id {Id} handshaked!");

            // Send initial message
            string message = "Hello from SSL app! Please send the following parameters separated by a semicolon or a ! symbol to disconnect. Parameters: FirstName; LastName; City; PostCode; AppVersion; Email; Music; Performer; Year; Hour";

            Send(message);
        }

        protected override void OnDisconnected()
        {
            Console.WriteLine($"SSL session with Id {Id} disconnected!");
        }

        protected override void OnReceived(byte[] buffer, long offset, long size)
        {
            string jsonData = Encoding.UTF8.GetString(buffer, (int)offset, (int)size);

            // Base 64 encoded compressed data
            //string compressedData = Convert.ToBase64String(buffer, (int)offset, (int)size);

            // json encoded data
            //string jsonData = GzipHelper.Decompress(compressedData);

            if (jsonData == "!")
            {
                Disconnect();
            }
            else
            {
                if (jsonData.Contains("virus"))
                {
                    string message = "Data contains a virus!";

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(message);
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.White;

                    Send(message);
                    return;
                }

                // Deserialize the incoming JSON object into the data model
                var dataObject = JsonSerializer.Deserialize<DataModel>(jsonData);

                savedData = dataObject;

                Console.WriteLine("Incoming: \n" + dataObject);
            }
        }

        protected override void OnError(SocketError error)
        {
            Console.WriteLine($"SSL session caught an error with code {error}");
        }
    }
}
