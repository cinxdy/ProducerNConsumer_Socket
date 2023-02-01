using System;
using System.Text;
using System.Threading.Tasks;


using NetMQ;
using NetMQ.Sockets;

namespace Consumer
{
    class Consumer
    {
        public void Run()
        {
            Random rand = new Random();

            using var sender = new RequestSocket(">tcp://localhost:5556");
            using var controller = new SubscriberSocket(">tcp://localhost:5557");

            sender.Options.Identity = Encoding.UTF8.GetBytes("Consumer1");

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
            Console.WriteLine("Sent READY consumer -> monitor");

            while (true)
            {
                var msgFromMonitor = sender.ReceiveMultipartMessage();

                var id = msgFromMonitor.Pop();
                msgFromMonitor.Pop();

                var item1 = msgFromMonitor.Pop().ConvertToInt32();
                var item2 = msgFromMonitor.Pop().ConvertToInt32();
                Console.WriteLine($"Received msg consumer <- monitor:{id.ConvertToInt32()} : {item1} + {item2} = ?");

                // Calculating
                int result = item1 + item2;

                var msgToMonitor = new NetMQMessage();
                msgToMonitor.Append(id);
                msgToMonitor.AppendEmptyFrame();
                msgToMonitor.Append($"{item1}+{item2}={result}");

                sender.SendMultipartMessage(msgToMonitor);
                Console.WriteLine(msgToMonitor);
                Console.WriteLine($"Sent msg consumer -> monitor:{msgToMonitor}\n\n");

            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Consumer consumer = new Consumer();
            consumer.Run();
        }
    }

}
