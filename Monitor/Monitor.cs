using NetMQ;
using NetMQ.Sockets;
using System;
using System.Collections.Generic;
using System.Diagnostics;

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


            consumer.ReceiveReady += (s, a) =>
            {
                // 1. READY message at the first receive
                // 2. Result message after the first receive
                var msgFromConsumer = consumer.ReceiveMultipartMessage();
                Console.WriteLine($"Received monitor <- consumer {msgFromConsumer}");

                id = msgFromConsumer.Pop(); msgFromConsumer.Pop();
                var rxid = msgFromConsumer.Pop();

                Debug.WriteLine($"monitor: {id}  {rxid.ConvertToString()}");

                if (!rxid.ConvertToString().Equals("READY"))
                {
                    msgFromConsumer.Pop();
                    // forward it to producer
                    var msgToProducer = new NetMQMessage();
                    msgToProducer.Append(rxid);
                    msgToProducer.AppendEmptyFrame();

                    while (!msgFromConsumer.IsEmpty)
                    {
                        msgToProducer.Append(msgFromConsumer.Pop());
                    }

                    producer.SendMultipartMessage(msgToProducer);
                    Console.WriteLine($"Sent monitor -> producer {msgToProducer}\n\n");
                }
                else
                {
                    Debug.WriteLine(id.ConvertToString());
                }
            };

            producer.ReceiveReady += (s, a) =>
            {
                Debug.WriteLine($"received from producer: {(id == null ? "null" : id.ConvertToInt32())}");

                //var message = receiver.;
                if (id != null)
                {
                    var msgFromProducer = producer.ReceiveMultipartMessage();
                    Console.WriteLine($"Received monitor <- producer {msgFromProducer}");

                    // forward it to consumer
                    var msgToConsumer = new NetMQMessage();
                    msgToConsumer.Append(id);
                    msgToConsumer.AppendEmptyFrame();

                    while (!msgFromProducer.IsEmpty)
                    {
                        msgToConsumer.Append(msgFromProducer.Pop());
                    }

                    consumer.SendMultipartMessage(msgToConsumer);
                    Console.WriteLine($"Sent monitor -> consumer {msgToConsumer}\n\n");
                    id = null;
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

        //private Queue<NetMQMessage> idQ;

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
