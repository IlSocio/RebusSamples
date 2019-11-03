using System;
using System.Configuration;
using System.Threading.Tasks;
using PubSub.Publisher.Contracts;
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
using Rebus.Subscriptions;
using Rebus.Transport.FileSystem;
using Rebus.Transport.InMem;

namespace PubSub.Publisher
{
    class Program
    {
        static void Main()
        {
            const string QUEUE = "rebus-Publisher";
            const string QUEUE_ERRORS = "rebus-errors";
            const string QUEUE_GEOCODER = "rebus-ReverseGeocoder";

            using (var activator = new BuiltinHandlerActivator())
            {
                activator.Register((bus, messageContext) => new Handler(bus, messageContext));

                /*
                // for local Debug...
                Configure.With(activator)
                    .Logging(l => l.ColoredConsole(minLevel: LogLevel.Warn))
                    .Options(o => o.SimpleRetryStrategy(errorQueueAddress: QUEUE_ERRORS))
                    .Transport(t => t.UseFileSystem(@"c:\servicebus\", QUEUE))
                    .Subscriptions(s => s.UseJsonFile(@"c:\servicebus\subscriptions.json"))
                    .Routing(r => r.TypeBased().MapAssemblyOf<ReverseGeoRequest>(QUEUE_GEOCODER))
                    .Start(); /**/

                /*
                // for integration-tests
                Configure.With(activator)
                    .Logging(l => l.ColoredConsole(minLevel: LogLevel.Warn))
                    .Options(o => o.SimpleRetryStrategy(errorQueueAddress: QUEUE_ERRORS))
                    .Transport(t => t.UseInMemoryTransport(new InMemNetwork(), QUEUE))
                    .Subscriptions(s => s.StoreInMemory(new InMemorySubscriberStore())) // useful in Unit-tests
                    .Routing(r => r.TypeBased().MapAssemblyOf<ReverseGeoRequest>(QUEUE_GEOCODER))
                    .Start(); /**/


                // Azure Service Bus...
                const string CONNECTION_STRING = "";
                Configure.With(activator)
                    .Logging(l => l.ColoredConsole(minLevel: LogLevel.Warn))
                    .Options(o => o.SimpleRetryStrategy(errorQueueAddress: QUEUE_ERRORS))
                    .Transport(t => t.UseAzureServiceBus(CONNECTION_STRING, QUEUE))
                    .Routing(r => r.TypeBased().MapAssemblyOf<ReverseGeoRequest>(QUEUE_GEOCODER))
                    .Start(); /**/


                var startupTime = DateTime.Now;
                while (true)
                {
                    Console.WriteLine(@"1) Publish String Notification");
                    Console.WriteLine(@"2) Publish DateTime Notification");
                    Console.WriteLine(@"3) Publish TimeSpan Notification");
                    Console.WriteLine(@"4) Send ReverseGeoCode Request");
                    Console.WriteLine(@"q) Quit");

                    var keyChar = char.ToLower(Console.ReadKey(true).KeyChar);
                    var busSync = activator.Bus.Advanced.SyncBus;

                    switch (keyChar)
                    {
                        case '1':
                            busSync.Publish(new StringNotification("Hello there, this is a string notification from a Publisher!"));
                            break;

                        case '2':
                            busSync.Publish(new DateTimeNotification(DateTime.Now));
                            break;

                        case '3':
                            busSync.Publish(new TimeSpanNotification(DateTime.Now - startupTime));
                            break;

                        case '4':
                            busSync.Send(new ReverseGeoRequest(10, 20));
                            break;

                        case 'q':
                            Console.WriteLine("Quitting!");
                            return;
                    }
                }
            }
        }
    }

    class Handler : IHandleMessages<ReverseGeoResponse>
    {
        private IBus Bus;
        private IMessageContext MessageContext;

        public Handler(IBus bus, IMessageContext messageContext)
        {
            Bus = bus;
            MessageContext = messageContext;
        }

        public async Task Handle(ReverseGeoResponse message)
        {
            Console.WriteLine($"Got Response: {message}");

            Console.WriteLine("Sending Notification...");
            await Bus.Publish(new StringNotification("Received new ReverseGeocoded Position"));

            // TODO: update database...

            Console.WriteLine("Notification Sent");
        }
    }

}
