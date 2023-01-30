using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;


using NetMQ;
using NetMQ.Sockets;

namespace Producer
{
    class Program
    {
        static void Main(string[] args)
        {
            Producer producer = new Producer();
            var tList = new List<Thread>();

            for (int i=0;i<1;i++)
            {
                Thread t = new Thread(producer.run);
                t.Start();
                tList.Add(t);
            }
            //producer.run();
        }

        // Socket 

        class Producer
        {
            public void run()
            {
                Random rand = new Random();

                using (var sender = new RequestSocket(">tcp://localhost:5555"))
                using (var controller = new SubscriberSocket(">tcp://localhost:5557"))
                {
                    // Connect
                    //sender.Connect("tcp://127.0.0.1:555");
                    //controller.Connect("tcp://127.0.0.1:5558");
                    //controller.Subscribe("wake producer");

                    //Console.WriteLine("Producer Connecting...");

                    // Wait Publish (Sleep)
                    //Console.WriteLine("Waiting the signal...");

                    //string messageTopicReceived = ;
                    //string messageReceived = controller.ReceiveFrameString();
                    //Console.WriteLine(messageReceived);

                    // if signaled
                    //while (controller.ReceiveFrameString() != "wake producer")
                    //;

                    // produce one
                    while (true)
                    {
                        int item1 = rand.Next(1, 1000);
                        int item2 = rand.Next(1, 1000);
                        Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId}\t{item1} + {item2} was produced");

                        // Send
                        var msgToMonitor = new NetMQMessage();
                        msgToMonitor.Append(item1);
                        msgToMonitor.Append(item2);

                        sender.SendMultipartMessage(msgToMonitor);
                        Console.WriteLine($"Sent msg: {msgToMonitor.ToString()}");

                        // Receive
                        var msgFromMonitor = sender.ReceiveFrameString();
                        Console.WriteLine($"Received msg: {msgFromMonitor}\n\n");

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
    }
}
