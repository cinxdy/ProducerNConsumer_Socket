using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;


using NetMQ;
using NetMQ.Sockets;
using System.Diagnostics;

namespace Producer
{
    class Producer
    {
        public void Run()
        {
            Random rand = new Random();

            using (var sender = new RequestSocket(">tcp://localhost:5555"))
            using (var controller = new SubscriberSocket(">tcp://localhost:5557"))
            {
                // Connect
                controller.Subscribe("wake producer");

                Debug.WriteLine("Producer Connecting...");

                // Wait Publish (Sleep)
                //Debug.WriteLine("Waiting the signal...");

                //string messageTopicReceived = ;
                //string messageReceived = controller.ReceiveFrameString();
                //Console.WriteLine(messageReceived);

                // if signaled
                //while (controller.ReceiveFrameString() != "wake producer")
                //;
                var threadId = Thread.CurrentThread.ManagedThreadId;
                // produce one
                while (true)
                {
                    int item1 = rand.Next(1, 1000);
                    int item2 = rand.Next(1, 1000);

                    Debug.WriteLine($"{threadId}\t{item1} + {item2} was produced");

                    // Send
                    var msgToMonitor = new NetMQMessage();
                    msgToMonitor.Append(item1);
                    msgToMonitor.Append(item2);

                    sender.SendMultipartMessage(msgToMonitor);

                    Console.WriteLine($"Sent msg producer -> monitor: {msgToMonitor}");

                    // Receive
                    var msgFromMonitor = sender.ReceiveFrameString();
                    Console.WriteLine($"Received msg producer <- monitor: {msgFromMonitor}\n\n");

                    int sleep = rand.Next(1, 5) * 1000;
                    Task.Delay(sleep).Wait();
                }

                // if okay : take a break
                // if not okay : retry
                //while (true)
                //{
                //    sender.SendFrame(item.ToString());
                //    if (sender.ReceiveSignal()) break;
                //    Task.Delay(1000).Wait();
                //}
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var producer = new Producer();
            var threads = new List<Thread>();

            for (int i=0;i<1;i++)
            {
                Thread t = new Thread(producer.Run);
                t.Start();
                threads.Add(t);
            }
        }
    }
}
