using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitReceive
{
    public class Receive
    {
        public static void ReceiveMessage(string[] args)
        {
            var factory = new ConnectionFactory {HostName = "localhost"};

            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare("topic_logs", ExchangeType.Topic);
                    var queueName = channel.QueueDeclare().QueueName;

                    if (args.Length < 1)
                    {
                        Console.Error.WriteLine("Usage: {0} [binding_key...]", Environment.GetCommandLineArgs()[ 0 ]);
                        Console.WriteLine(" Press [enter] to exit.");
                        Console.ReadLine();
                        Environment.ExitCode = 1;

                        return;
                    }

                    foreach (var severity in args)
                        channel.QueueBind(queueName, "topic_logs", severity);

                    Console.WriteLine(" [x] Waiting for messages.");
                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                                         {
                                             var body = ea.Body.ToArray();
                                             var message = Encoding.UTF8.GetString(body);
                                             var routingKey = ea.RoutingKey;
                                             Console.WriteLine(" [x] Receive '{0}':'{1}'", routingKey, message);
                                         };

                    channel.BasicConsume(queueName, true, consumer);
                    Console.WriteLine(" Press [enter] to exit.");
                    Console.ReadKey();
                }
            }
        }
    }
}