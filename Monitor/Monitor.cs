using System;
using System.Collections.Generic;

using NetMQ;
using NetMQ.Sockets;

namespace Monitor
{
    class Monitor
    {
        public void Run()
        {
            NetMQFrame id = null;
            var msg = new NetMQMessage();

            using var producer = new RouterSocket("@tcp://*:5555");
            using var consumer = new RouterSocket("@tcp://*:5556");
            using var controller = new PublisherSocket("@tcp://*:5557");
            using var poller = new NetMQPoller { producer, consumer, controller };
            {
                consumer.ReceiveReady += (s, a) =>
                {
                    // 1. READY message
                    // 2. 
                    var msgFromConsumer = producer.ReceiveMultipartMessage();
                    Console.WriteLine($"Received monitor <- consumer {msgFromConsumer}\n\n");

                    var id = msgFromConsumer.Pop(); msgFromConsumer.Pop();
                    var rxid = msgFromConsumer.Pop();

                    if (!rxid.ConvertToString().Equals("READY"))
                    {
                        msgFromConsumer.Pop();
                        while (!msgFromConsumer.IsEmpty)
                        {
                            msg.Append(msgFromConsumer.Pop());
                        }
                    }


                   
                    // forward it to producer
                    var msgToProducer = new NetMQMessage();
                    msgToProducer.Append(rxid);
                    msgToProducer.AppendEmptyFrame();
                    producer.SendMultipartMessage(msgToProducer);
                    Console.WriteLine($"Sent monitor -> producer {msgToProducer}\n\n");
                };

                producer.ReceiveReady += (s, a) =>
                {
                    Console.WriteLine("received from producer");
                    //var message = receiver.;
                    var msgFromProducer = producer.ReceiveMultipartMessage();
                    Console.WriteLine($"Received monitor <- producer {msgFromProducer}\n\n");

                    // forward it to consumer
                    if (id != null)
                    {
                        var msgToConsumer = new NetMQMessage();
                        msgToConsumer.Append(id);
                        msgToConsumer.AppendEmptyFrame();

                        while (!msgFromProducer.IsEmpty)
                        {
                            msgToConsumer.Append(msgFromProducer.Pop());
                        }
                        consumer.SendMultipartMessage(msgToConsumer);
                        Console.WriteLine($"Sent monitor -> consumer {msgToConsumer}\n\n");

                    }
                };

                controller.ReceiveReady += (s, a) =>
                {

                };

                poller.Run();

            }

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
        private Queue<NetMQMessage> msgQ;
    }

    class Program
    {
        static void Main(string[] args)
        {
            Monitor monitor = new Monitor();
            monitor.Run();
        }
    }
}
