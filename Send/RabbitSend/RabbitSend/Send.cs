using System;
using System.Text;
using RabbitMQ.Client;

namespace RabbitSend
{
    public class Send
    {
        public static void SendMessage()
        {
            var factory = new ConnectionFactory {HostName = "localhost"};

            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare("hello", false, false, false, null);
                    var message = "Hello world!";
                    var body = Encoding.UTF8.GetBytes(message);
                    channel.BasicPublish("", "hello", null, body);
                    Console.WriteLine(" [x] Sent {0}", message);
                }
            }

            Console.WriteLine(" Press [enter] to exit");
            Console.ReadKey();
        }
    }
}