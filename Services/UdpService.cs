using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace UWB.Services
{
    internal class UdpService<T> : IDisposable where T : class
    {
        private Queue<T> updates;
        private readonly UdpClient udpClient;

        public UdpService(int port)
        {
            udpClient = new UdpClient(port);
            updates = new Queue<T>();
        }

        public async Task Start()
        {
            while (true)
            {
                try
                {
                    var result = await udpClient.ReceiveAsync();
                    var returnData = Encoding.ASCII.GetString(result.Buffer);
                    var update = JsonConvert.DeserializeObject<T>(returnData) ?? throw new();

                    lock (updates)
                    {
                        updates.Enqueue(update);
                    }
                }
                catch { }
            }
        }

        public T[] GetUpdates()
        {
            T[] pendingMessages;
            lock (updates)
            {
                pendingMessages = new T[updates.Count];
                int i = 0;
                while (updates.Count != 0)
                {
                    pendingMessages[i] = updates.Dequeue();
                    i++;
                }
            }
            return pendingMessages;
        }

        public void Dispose()
        {
            udpClient.Close(); //
        }
    }
}
