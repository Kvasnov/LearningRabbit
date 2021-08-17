using System;
using System.Text;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitReceive
{
    public class Receive
    {
        public static void ReceiveMessage()
        {
            var factory = new ConnectionFactory {HostName = "localhost"};

            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare("task_queue", true, false, false, null);
                    channel.BasicQos(0, 1, false);
                    Console.WriteLine(" [x] Waiting for messages.");
                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                                         {
                                             var body = ea.Body.ToArray();
                                             var message = Encoding.UTF8.GetString(body);
                                             Console.WriteLine(" [x] Receive {0}", message);
                                             var dots = message.Split('.').Length - 1;
                                             Thread.Sleep(dots * 1000);
                                             Console.WriteLine(" [x] Done");
                                             channel.BasicAck(ea.DeliveryTag, false);
                                         };

                    channel.BasicConsume("task_queue", false, consumer);
                    Console.WriteLine(" Press [enter] to exit.");
                    Console.ReadKey();
                }
            }
        }
    }
}