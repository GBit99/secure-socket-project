using System;
using System.Linq;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
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
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;

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
                    firstName: separatedData[0],
                    lastName: separatedData[1],
                    city: separatedData[2],
                    postCode: separatedData[3],
                    appVersion: separatedData[4],
                    email: separatedData[5],
                    music: separatedData[6],
                    performer: separatedData[7],
                    year: int.Parse(separatedData[8]),
                    hour: int.Parse(separatedData[9]),
                    md5Hash: separatedData[10],
                    fileName: separatedData[11],
                    fileType: separatedData[12],
                    date: DateTime.Parse(separatedData[13]));

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

            return random.Next(0,101) == 42 && dataType == DataType.Text
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
            var md5 = GenerateRandomMD5Hash();
            var fileName = GenerateRandomString(random.Next(5, 20), DataType.Text);
            var fileType = GenerateRandomString(random.Next(2, 5), DataType.Text);
            var date = new DateTime(random.Next(1900, 2022), random.Next(1, 13), random.Next(1, 29));


            return string.Join(';', firstName, lastName, city, postCode, appVersion, email, music, performer, year, hour, md5, fileName, fileType, date.ToString());
        }

        public static string GenerateRandomMD5Hash()
        {
            var random = new Random();

            var md5Hasher = MD5.Create();
            var randomString = GenerateRandomString(random.Next(20, 100), DataType.Text);
            var randomStringBytes = Encoding.ASCII.GetBytes(randomString);

            var hashedBytes = md5Hasher.ComputeHash(randomStringBytes);

            return string.Join(string.Empty, hashedBytes.Select(b => b.ToString("x2")));
        }
    }
}
