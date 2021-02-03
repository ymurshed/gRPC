using Grpc.Core;
using System;
using System.Threading;
using Calculator;

namespace SubClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Channel channel = new Channel("127.0.0.1:50051", ChannelCredentials.Insecure);

            var client = new Calculate.CalculateClient(channel);
            string opr = "-";

            int i = 0;
            while (i < 100)
            {
                i++;
                var number1 = new Random().Next(100);
                var number2 = new Random().Next(100);

                var reply = client.GetResult(new ActionRequest() { Number1 = number1, Number2 = number2, Operation = opr });
                Console.WriteLine(number1 + " " + opr + " " + number2 + " --->>> " + reply.Result);
                Thread.Sleep(1000);
            }

            channel.ShutdownAsync().Wait();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
