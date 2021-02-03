using System;
using System.Threading.Tasks;
using Grpc.Core;
using Calculator;

namespace SimpleCalculatorServer
{
    class CalculatorImpl : Calculate.CalculateBase
    {
        // Server side handler of the calculator RPC
        public override Task<ActionReply> GetResult(ActionRequest request, ServerCallContext context)
        {
            double output = request.Number1 + request.Number2;

            switch (request.Operation)
            {
                case "+":
                    output = request.Number1 + request.Number2;
                    break;

                case "-":
                    output = request.Number1 - request.Number2;
                    break;

                case "*":
                    output = request.Number1 * request.Number2;
                    break;

                case "/":
                    output = request.Number1 / request.Number2;
                    break;
            }

            return Task.FromResult(new ActionReply() {Result = output});
        }
    }

    class Program
    {
        const int Port = 50051;

        static void Main(string[] args)
        {
            Server server = new Server
            {
                Services = { Calculate.BindService(new CalculatorImpl()) },
                Ports = { new ServerPort("localhost", Port, ServerCredentials.Insecure) }
            };
            server.Start();

            Console.WriteLine("Simple calculator server listening on port " + Port);
            Console.WriteLine("Press any key to stop the server...");
            Console.ReadKey();

            server.ShutdownAsync().Wait();
        }
    }
}
