using System;
using System.Linq;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Threading;
using Models;
using NetCoreServer;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            // SSL server address
            string address = "127.0.0.1";
            if (args.Length > 0)
            {
                address = args[0];
            }

            // SSL server port
            int port = 2222;
            if (args.Length > 1)
            {
                port = int.Parse(args[1]);
            }

            Console.WriteLine($"SSL server address: {address}");
            Console.WriteLine($"SSL server port: {port}");

            Console.WriteLine();

            // Create and prepare a new SSL client context
            var context = new SslContext(SslProtocols.Tls12, new X509Certificate2(@"C:\Users\user\Downloads\client.pfx", "qwerty"), (sender, certificate, chain, sslPolicyErrors) => true);

            // Create a new SSL client
            var client = new SocketClient(context, address, port);

            // Connect the client
            Console.Write("Client connecting...");
            client.ConnectAsync();
            Console.WriteLine("Done!");

            Console.WriteLine("Press Enter to stop the client or '!' to reconnect the client...");

            // Perform text input
            for (;;)
            {
                string line = GenerateRandomData();

                if (string.IsNullOrEmpty(line))
                {
                    break;
                }

                // Disconnect the client
                if (line == "!")
                {
                    Console.Write("Client disconnecting...");
                    client.DisconnectAsync();
                    Console.WriteLine("Done!");
                    continue;
                }

                // Map client data to a model
                var separatedData = line.Split(';');
                var data = new DataModel(
                    separatedData[0],
                    separatedData[1],
                    separatedData[2],
                    separatedData[3],
                    separatedData[4],
                    separatedData[5],
                    separatedData[6],
                    separatedData[7],
                    int.Parse(separatedData[8]),
                    int.Parse(separatedData[9]));

                // JSON serialize the client data
                string serializedData = JsonSerializer.Serialize(data);

                //// Gzip the client data
                //var compressedData = GzipHelper.Compress(serializedData);

                // Send the client data
                client.SendAsync(serializedData);
            }

            // Disconnect the client
            Console.Write("Client disconnecting...");
            client.DisconnectAndStop();
            Console.WriteLine("Done!");
        }

        public enum DataType
        {
            Number,
            Text
        }

        public static string GenerateRandomString(int length, DataType dataType)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string numbers = "0123456789";
            var random = new Random();

            Thread.Sleep(100);

            return random.Next(0,51) == 42 && dataType == DataType.Text
                ? "virus"
                : new string(
                    Enumerable
                        .Repeat(dataType == DataType.Text ? chars : numbers, length)
                        .Select(s=>s[random.Next(s.Length)]).ToArray());
        }

        public static string GenerateRandomData()
        {
            var random = new Random();

            var firstName = GenerateRandomString(random.Next(1, 11), DataType.Text);
            var lastName = GenerateRandomString(random.Next(1, 11), DataType.Text);
            var city = GenerateRandomString(random.Next(1, 11), DataType.Text);
            var postCode = GenerateRandomString(random.Next(1, 5), DataType.Number);
            var appVersion = GenerateRandomString(random.Next(1, 3), DataType.Number);
            var email = GenerateRandomString(random.Next(1, 21), DataType.Text);
            var music = GenerateRandomString(random.Next(1, 31), DataType.Text);
            var performer = GenerateRandomString(random.Next(1, 21), DataType.Text);
            var year = GenerateRandomString(4, DataType.Number);
            var hour = GenerateRandomString(2, DataType.Number);

            return string.Join(';', firstName, lastName, city, postCode, appVersion, email, music, performer, year, hour);
        }
    }
}
