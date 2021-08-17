using System;

namespace RabbitSend
{
    public class Send
    {
        public static void SendMessage()
        {
            var rpcClient = new RpcClient();
            Console.WriteLine(" Press [enter] to exit");
            var response = rpcClient.Call("13");
            Console.WriteLine(" [.] Got '{0}'", response);
            rpcClient.Close();
            Console.ReadKey();
        }
    }
}