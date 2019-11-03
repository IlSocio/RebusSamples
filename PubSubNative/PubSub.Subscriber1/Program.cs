using System;
using System.Configuration;
using System.Threading.Tasks;
using PubSub.Publisher.Contracts;
using Rebus.Activation;
using Rebus.Config;
using Rebus.Handlers;
using Rebus.Logging;
using Rebus.Persistence.FileSystem;
using Rebus.Persistence.InMem;
using Rebus.Retry.Simple;
using Rebus.Routing.TypeBased;
using Rebus.Transport.FileSystem;

namespace PubSub.Subscriber1
{
    class Program
    {
        static void Main()
        {
            const string QUEUE = "rebus-Subscriber1";
            const string QUEUE_ERRORS = "rebus-errors";

            using (var activator = new BuiltinHandlerActivator())
            {
                activator.Register(() => new NotificationsHandler());

                /*
                // for local Debug...
                const string QUEUE_PUBLISHER = "rebus-Publisher";
                Configure.With(activator)
                    .Logging(l => l.ColoredConsole(minLevel: LogLevel.Warn))
                    .Options(b => b.SimpleRetryStrategy(errorQueueAddress: QUEUE_ERRORS))
                    .Transport(t => t.UseFileSystem(@"c:\servicebus\", QUEUE))
                    .Routing(r => r.TypeBased().MapAssemblyOf<StringNotification>(QUEUE_PUBLISHER))
                    .Start(); /**/

                // Azure Service Bus...
                const string CONNECTION_STRING = "";
                Configure.With(activator)
                    .Logging(l => l.ColoredConsole(minLevel: LogLevel.Warn))
                    .Options(b => b.SimpleRetryStrategy(errorQueueAddress: QUEUE_ERRORS))
                    .Transport(t => t.UseAzureServiceBus(CONNECTION_STRING, QUEUE))
                    .Start(); /**/

                var bus = activator.Bus;

                bus.Subscribe<StringNotification>().Wait();
                bus.Subscribe<DateTimeNotification>().Wait();
                bus.Subscribe<TimeSpanNotification>().Wait();
                // 
                // Creates a "rebus-Subscriber1" subscription for the following topics:
                // pubsubshared_messages_stringmessage__pubsub_messages
                // pubsubshared_messages_datetimemessage__pubsub_messages
                // pubsubshared_messages_timespanmessage__pubsub_messages
                // 
                // The subscription forwards messages to the "rebus-Subscriber1" queue
                //
                Console.WriteLine("This is Subscriber 1");
                Console.WriteLine("Press ENTER to quit");
                Console.ReadLine();
                Console.WriteLine("Quitting...");
            }
        }
    }

    class NotificationsHandler : IHandleMessages<StringNotification>, IHandleMessages<DateTimeNotification>, IHandleMessages<TimeSpanNotification>
    {
        public async Task Handle(StringNotification message)
        {
            Console.WriteLine("Got string: {0}", message.Text);
        }

        public async Task Handle(DateTimeNotification message)
        {
            Console.WriteLine("Got DateTime: {0}", message.DateTime);
        }

        public async Task Handle(TimeSpanNotification message)
        {
            Console.WriteLine("Got TimeSpan: {0}", message.TimeSpan);
        }
    }
}
