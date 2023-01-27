using System;
using System.Threading.Tasks;


using NetMQ;
using NetMQ.Sockets;

namespace Monitor
{
    class Program
    {
        static void Main(string[] args)
        {
            Monitor monitor = new Monitor();
            monitor.run();
        }

        // Socket 

        class Monitor
        {
            public void run()
            {

                Random rand = new Random();
                int item = default;

                using (var sender = new RouterSocket("@tcp://*:5557"))
                using (var controller = new SubscriberSocket())
                {
                    // Connect
                    //sender.Connect("tcp://127.0.0.1:5557");
                    controller.Connect("tcp://127.0.0.1:5558");
                    controller.Subscribe("wake producer");

                    Console.WriteLine("Producer Connecting...");

                    // Wait Publish (Sleep)
                    Console.WriteLine("Waiting the signal...");

                    //string messageTopicReceived = ;
                    //string messageReceived = controller.ReceiveFrameString();
                    //Console.WriteLine(messageReceived);

                    // if signaled
                    while (controller.ReceiveFrameString() != "wake producer")
                        ;

                    // produce one
                    item = rand.Next(1, 1000);
                    Console.WriteLine($"\t{item} was produced\n");

                    // send
                    // if okay : take a break
                    // if not okay : retry
                    while (true)
                    {
                        sender.SendFrame(item.ToString());
                        if (sender.ReceiveSignal()) break;
                        Task.Delay(1000).Wait();
                    }

                    int sleep = rand.Next(1, 5) * 1000;
                    Task.Delay(sleep).Wait();
                }

            }
        }
    }
}
