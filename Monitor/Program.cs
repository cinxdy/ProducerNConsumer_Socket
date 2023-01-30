using System;
using System.Threading.Tasks;

using System.Collections.Generic;

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
            private Queue<NetMQMessage> msgQ;
            public void run()
            {
                using (var rxFromProducer = new RouterSocket("@tcp://*:5555"))
                using (var rxFromConsumer = new RouterSocket("@tcp://*:5556"))
                using (var controller = new PublisherSocket("@tcp://*:5557"))
                using (var poller = new NetMQPoller { rxFromProducer, rxFromConsumer, controller })
                {
                    rxFromProducer.ReceiveReady += (s, a) =>
                    {
                        //var message = receiver.;
                        var msgFromProducer = rxFromProducer.ReceiveMultipartMessage();

                        msgQ.Enqueue(msgFromProducer);
                        
                        
                    };

                    rxFromConsumer.ReceiveReady += (s, a) =>
                    {
                        var msgFromConsumer = rxFromProducer.ReceiveMultipartMessage();
                        var id = msgFromConsumer.Pop(); msgFromConsumer.Pop();

                        if (!msgFromConsumer.Pop().ToString().Equals("READY"))
                        {
                            var msgToConsumer = new NetMQMessage();
                            msgToConsumer.Append(id);
                            msgToConsumer.AppendEmptyFrame();

                            msgToConsumer.Append(msgQ.Dequeue());

                            rxFromProducer.SendMultipartMessage(msgToConsumer);
                            Console.WriteLine("Sent Done\n\n");
                        }
                    };

                    controller.ReceiveReady += (s, a) =>
                    {

                    };

                    poller.Run();

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
                }

            }
        }
    }
}
