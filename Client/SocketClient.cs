using System;
using System.Collections.Generic;
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
        private const int ITEMS_TO_DEQUEUE = 100;

        private bool _stop;

        public SocketClient(SslContext context, string address, int port) : base(context, address, port) 
        {
            ToBeSentQueue = new Queue<DataModel>(1_000_000);
            AwaitingResponseItems = new List<DataModel>(1_000_000);
        }

        public Queue<DataModel> ToBeSentQueue { get; set; }
        public List<DataModel> AwaitingResponseItems { get; set; }

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

            // Try to connect again
            if (!_stop)
            {
                ConnectAsync();
            }
        }

        public void SendAsync(DataModel model)
        {
            // Enqueue the current model that's about to be sent
            ToBeSentQueue.Enqueue(model);

            // If there are items in the queue, send some of them first
            for (int i = 0; i < ITEMS_TO_DEQUEUE; i++)
            {
                if (ToBeSentQueue.TryDequeue(out DataModel dequeuedModel))
                {
                    // And send the model that has been waiting in the queue
                    AwaitingResponseItems.Add(dequeuedModel);

                    var serializedModel = JsonSerializer.Serialize(dequeuedModel);

                    base.SendAsync(serializedModel);
                }
                else
                {
                    break;
                }
            }
        }

        protected override void OnReceived(byte[] buffer, long offset, long size)
        {
            // Turn into string
            var serverMessage = Encoding.UTF8.GetString(buffer, (int)offset, (int)size);

            // JSON deserialize into server result model
            var serverResult = JsonSerializer.Deserialize<ServerResult>(serverMessage);

            switch (serverResult.Status)
            {
                case StatusCode.Failure:
                    var failedItem = AwaitingResponseItems.Find(e => e.Id == serverResult.ElementId.Value);
                    ToBeSentQueue.Enqueue(failedItem);
                    AwaitingResponseItems.Remove(failedItem);
                    Console.BackgroundColor = ConsoleColor.Red;
                    break;
                case StatusCode.Success:
                    var successfullItem = AwaitingResponseItems.Find(e => e.Id == serverResult.ElementId.Value);
                    AwaitingResponseItems.Remove(successfullItem);
                    break;
            }

            Console.WriteLine($"Server message:\n{serverResult}");
            Console.BackgroundColor = ConsoleColor.White;
        }

        protected override void OnError(SocketError error)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"SSL client caught an error with code {error}");
            Console.ForegroundColor = ConsoleColor.Black;
        }
    }
}
