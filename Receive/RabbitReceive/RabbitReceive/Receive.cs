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
                    channel.QueueDeclare("rpc_queue", false, false, false, null);
                    channel.BasicQos(0, 1, false);
                    var consumer = new EventingBasicConsumer(channel);
                    channel.BasicConsume("rpc_queue", false, consumer);
                    Console.WriteLine(" [x] Awaiting RPC request.");
                    consumer.Received += (model, ea) =>
                                         {
                                             string response;
                                             var body = ea.Body.ToArray();
                                             var properties = ea.BasicProperties;
                                             var replyProperties = channel.CreateBasicProperties();
                                             replyProperties.CorrelationId = properties.CorrelationId;
                                             var message = Encoding.UTF8.GetString(body);
                                             int number;

                                             if (!int.TryParse(message, out number))
                                             {
                                                 Console.WriteLine(" [.] Incorrect number");
                                                 response = string.Empty;
                                             }
                                             else
                                             {
                                                 Console.WriteLine(" [.] Fib({0})", message);
                                                 response = Fib(number).ToString();
                                             }

                                             var responseBytes = Encoding.UTF8.GetBytes(response);
                                             channel.BasicPublish("", properties.ReplyTo, replyProperties, responseBytes);
                                             channel.BasicAck(ea.DeliveryTag, false);
                                             var routingKey = ea.RoutingKey;
                                             Console.WriteLine(" [x] Receive '{0}':'{1}'", routingKey, message);
                                         };

                    Console.WriteLine(" Press [enter] to exit.");
                    Console.ReadKey();
                }
            }
        }

        private static int Fib(int n)
        {
            if (n == 0 || n == 1)
                return n;

            return Fib(n - 1) + Fib(n - 2);
        }
    }
}