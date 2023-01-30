using System;
using System.Threading.Tasks;


using NetMQ;
using NetMQ.Sockets;

namespace Consumer
{
    class Program
    {
        static void Main(string[] args)
        {
            Consumer consumer = new Consumer();
            consumer.run();
        }

        // Socket 

        class Consumer
        {
            public void run()
            {

                Random rand = new Random();
                //int item = default;

                using (var sender = new RequestSocket(">tcp://localhost:5556"))
                using (var controller = new SubscriberSocket(">tcp://localhost:5557"))
                {
                    // Connect
                    //sender.Connect("tcp://127.0.0.1:5557");
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
                    //    ;

                    sender.SendFrame("READY");

                    while (true)
                    {
                        var msgFromMonitor = sender.ReceiveMultipartMessage();
                        var id = msgFromMonitor.Pop();
                        msgFromMonitor.Pop();
                        var item1 = msgFromMonitor.Pop().ConvertToInt32();
                        var item2 = msgFromMonitor.Pop().ConvertToInt32();
                        Console.WriteLine($"Received msg:{id.ConvertToInt32()} : {item1} + {item2} = ?");

                        // Calculating
                        int result = item1 + item2;

                        var msgToMonitor = new NetMQMessage();
                        msgToMonitor.Append(id);
                        msgToMonitor.AppendEmptyFrame();
                        msgToMonitor.Append($"{item1}+{item2}={result}");

                        sender.SendMultipartMessage(msgToMonitor);
                        Console.WriteLine(msgToMonitor);
                        Console.WriteLine("Sent Done\n\n");

                    }
                }

            }
        }
    }
}
