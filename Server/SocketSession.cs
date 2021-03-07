using System;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Models;
using NetCoreServer;

namespace Server
{
    public class SocketSession : SslSession
    {
        private const string GUID_REGEX = @"(\{){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\}){0,1}";

        public SocketSession(SslServer server) : base(server) { }

        protected override void OnConnected()
        {
            Console.WriteLine($"SSL session with Id {Id} connected!");
        }

        protected override void OnHandshaked()
        {
            Console.WriteLine($"SSL session with Id {Id} handshaked!");

            // Send initial message
            string message = "Hello from SSL app! Please send the following parameters separated by a semicolon or a ! symbol to disconnect. Parameters: FirstName; LastName; City; PostCode; AppVersion; Email; Music; Performer; Year; Hour; MD5 hash; FileName; FileType; Date";

            Send(message);
        }

        protected override void OnDisconnected()
        {
            Console.WriteLine($"SSL session with Id {Id} disconnected!");
        }

        protected override void OnReceived(byte[] buffer, long offset, long size)
        {
            string rawData = Encoding.UTF8.GetString(buffer, (int)offset, (int)size);

            if (rawData == "!")
            {
                Disconnect();
            }
            else
            {
                //if (rawData.Contains("virus"))
                //{
                //    string message = "Data contains a virus!";

                //    Console.ForegroundColor = ConsoleColor.Red;
                //    Console.WriteLine(message);
                //    Console.WriteLine();
                //    Console.ForegroundColor = ConsoleColor.White;

                //    Send(message);
                //    return;
                //}

                try
                {
                    var dataModel = JsonSerializer.Deserialize<DataModel>(rawData);

                    Console.WriteLine("Incoming: \n" + dataModel);

                    // Send a processing successful server result
                    Send(dataModel.Id, StatusCode.Success);
                }
                catch (Exception)
                {
                    // Extract the GUID/s using regex
                    var regexMatches = Regex.Matches(rawData, GUID_REGEX);

                    for (int i = 0; i < regexMatches.Count; i++)
                    {   
                        if (Guid.TryParse(regexMatches[i].Value, out Guid parsedGuid))
                        {
                            // Send a processing failure server result
                            Send(parsedGuid, StatusCode.Failure);
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"Error: Could not parse the GUID [{regexMatches[i].Value}]");
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                    }
                }
            }
        }

        public long Send(Guid elementId, StatusCode status)
        {
            var responseModel = new ServerResult(elementId, status);

            var serializedResponse = JsonSerializer.Serialize(responseModel);

            return base.Send(serializedResponse);
        }

        public override long Send(string text)
        {
            var responseModel = new ServerResult(text);

            var serializedResponse = JsonSerializer.Serialize(responseModel);

            return base.Send(serializedResponse);
        }

        protected override void OnError(SocketError error)
        {
            Console.WriteLine($"SSL session caught an error with code {error}");
        }
    }
}
