using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Reverse;

namespace ReverseServer
{
    class ReverseImpl : ReverseService.ReverseServiceBase
    {
        // Unary call 
        public override Task<ReverseReply> ReverseString(ReverseRequest request, ServerCallContext context)
        {
            var reverseReply = new ReverseReply
            {
                Reversed = new string(request.Data.Reverse().ToArray())
            };

            return Task.FromResult (reverseReply);
        }

        // Server side streaming call
        public override async Task ReverseStringWithCaseSupport(ReverseRequest request, IServerStreamWriter<ReverseReply> responseStream,
            ServerCallContext context)
        {
            List<ReverseReply> reverseReplies = new List<ReverseReply>
            {
                new ReverseReply { Reversed = new string(request.Data.Reverse().ToArray()) },
                new ReverseReply { Reversed = new string(request.Data.ToLower().Reverse().ToArray()) },
                new ReverseReply { Reversed = new string(request.Data.ToUpper().Reverse().ToArray()) }
            };

            foreach (var reply in reverseReplies)
            {
                Thread.Sleep(10 * 1000);
                await responseStream.WriteAsync(new ReverseReply(reply));
            }
        }
    }

    class Program
    {
        const int Port = 50051;

        public static void Main(string[] args)
        {
            Server server = new Server
            {
                Services = { ReverseService.BindService(new ReverseImpl()) },
                Ports = { new ServerPort("localhost", Port, ServerCredentials.Insecure) }
            };
            server.Start();

            Console.WriteLine("Reverse server listening on port " + Port);
            Console.WriteLine("Press any key to stop the server...");
            Console.ReadKey();

            server.ShutdownAsync().Wait();
        }
    }
}
