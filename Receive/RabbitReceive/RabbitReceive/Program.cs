namespace RabbitReceive
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Receive.ReceiveMessage(args);
        }
    }
}