using System;
using System.Linq;
using System.Text;
using RabbitMQ.Client;

namespace RabbitSend
{
    public class Send
    {
        public static void SendMessage(string[] args)
        {
            var factory = new ConnectionFactory {HostName = "localhost"};

            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare("direct_logs", ExchangeType.Direct);
                    var message = GetMessage(args);
                    var severity = args.Length > 0 ? args[ 0 ] : "info";
                    var body = Encoding.UTF8.GetBytes(message);
                    channel.BasicPublish("direct_logs", severity, null, body);
                    Console.WriteLine(" [x] Sent '{0}': '{1}'", message, severity);
                }
            }

            Console.WriteLine(" Press [enter] to exit");
            Console.ReadKey();
        }

        private static string GetMessage(string[] args)
        {
            return args.Length > 1 ? string.Join(" ", args.Skip(1).ToArray()) : "Hello World!";
        }
    }
}