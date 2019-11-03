using System;
using System.Configuration;
using System.Threading.Tasks;
using PubSub.ReverseGeocoder.Contracts;
using Rebus.Activation;
using Rebus.Bus;
using Rebus.Config;
using Rebus.Handlers;
using Rebus.Logging;
using Rebus.Persistence.FileSystem;
using Rebus.Persistence.InMem;
using Rebus.Pipeline;
using Rebus.Retry.Simple;
using Rebus.Routing.TypeBased;
using Rebus.Transport.FileSystem;
using Rebus.Transport.InMem;

namespace PubSub.ReverseGeocoder
{
    class Program
    {
        static void Main()
        {
            const string QUEUE = "rebus-ReverseGeocoder";
            const string QUEUE_ERRORS = "rebus-errors";

            using (var activator = new BuiltinHandlerActivator())
            {
                activator.Register((b, c) => new ReveseGeoRequestHandler(b, c));

                /*
                // for local Debug...
                Configure.With(activator)
                    .Logging(l => l.ColoredConsole(minLevel: LogLevel.Warn))
                    .Options(b => b.SimpleRetryStrategy(errorQueueAddress: QUEUE_ERRORS))
                    .Transport(t => t.UseFileSystem(@"c:\servicebus\", QUEUE))
                    .Start(); /**/

                // Azure Service Bus...
                const string CONNECTION_STRING = "";
                Configure.With(activator)
                    .Logging(l => l.ColoredConsole(minLevel: LogLevel.Warn))
                    .Options(b => b.SimpleRetryStrategy(errorQueueAddress: QUEUE_ERRORS))
                    .Transport(t => t.UseAzureServiceBus(CONNECTION_STRING, QUEUE))
                    .Start(); /**/


                Console.WriteLine("This is the Worker");
                Console.WriteLine("Press ENTER to quit"); Console.ReadLine();
                Console.WriteLine("Quitting...");
            }
        }

    }

    class ReveseGeoRequestHandler : IHandleMessages<ReverseGeoRequest>
    {
        private IBus Bus;
        private IMessageContext MessageContext;

        public ReveseGeoRequestHandler(IBus bus, IMessageContext messageContext)
        {
            Bus = bus;
            MessageContext = messageContext;
        }

        public async Task Handle(ReverseGeoRequest message)
        {
            Console.WriteLine("Got Request: {0}", message);
            var resp = new ReverseGeoResponse("Turin");
            await Bus.Reply(resp);
        }
    }
}
