using Grpc.Core;
using System;
using Reverse;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReverseClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Channel channel = new Channel("127.0.0.1:50051", ChannelCredentials.Insecure);
            var client = new ReverseService.ReverseServiceClient(channel);

            //UnaryCall(client);
            ServerStreamingCall(client).Wait();
            
            channel.ShutdownAsync().Wait();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        static void UnaryCall(ReverseService.ReverseServiceClient client)
        {
            string textToReverse = "Bangladesh";
            var requestData = new ReverseRequest() { Data = textToReverse };

            var reply = client.ReverseString(requestData);
            Console.WriteLine("Reverse of " + textToReverse + " --->>> " + reply.Reversed);
        }

        static async Task ServerStreamingCall(ReverseService.ReverseServiceClient client)
        {
            string textToReverse = "I Like Programming";
            var requestData = new ReverseRequest() { Data = textToReverse };

            Console.WriteLine("Main text: " + textToReverse);
            using (var call = client.ReverseStringWithCaseSupport(requestData))
            {
                var responseStream = call.ResponseStream;
                while (await responseStream.MoveNext())
                {
                    Console.WriteLine(responseStream.Current.Reversed);
                }
            }
        }
    }
}
