using System;
using System.Collections.Concurrent;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitSend
{
    public class RpcClient
    {
        public RpcClient()
        {
            var factory = new ConnectionFactory {HostName = "localhost"};
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            var replyQueueName = channel.QueueDeclare().QueueName;
            var consumer = new EventingBasicConsumer(channel);
            properties = channel.CreateBasicProperties();
            var correlationId = Guid.NewGuid().ToString();
            properties.CorrelationId = correlationId;
            properties.ReplyTo = replyQueueName;
            consumer.Received += (model, ea) =>
                                 {
                                     var body = ea.Body.ToArray();
                                     var response = Encoding.UTF8.GetString(body);
                                     if (ea.BasicProperties.CorrelationId == correlationId)
                                         responseQueue.Add(response);
                                 };

            channel.BasicConsume(consumer, replyQueueName, true);
        }

        private readonly IModel channel;
        private readonly IConnection connection;
        private readonly IBasicProperties properties;
        private readonly BlockingCollection<string> responseQueue = new BlockingCollection<string>();

        public string Call(string message)
        {
            var messageBytes = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish("", "rpc_queue", properties, messageBytes);

            return responseQueue.Take();
        }

        public void Close()
        {
            connection.Close();
        }
    }
}