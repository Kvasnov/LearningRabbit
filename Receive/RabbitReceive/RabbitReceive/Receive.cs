using System;
using System.Text;
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
                    channel.ExchangeDeclare("logs", ExchangeType.Fanout);
                    var queueName = channel.QueueDeclare().QueueName;
                    channel.QueueBind(queueName, "logs", "");
                    Console.WriteLine(" [x] Waiting for messages.");
                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                                         {
                                             var body = ea.Body.ToArray();
                                             var message = Encoding.UTF8.GetString(body);
                                             Console.WriteLine(" [x] Receive {0}", message);
                                         };

                    channel.BasicConsume(queueName, true, consumer);
                    Console.WriteLine(" Press [enter] to exit.");
                    Console.ReadKey();
                }
            }
        }
    }
}