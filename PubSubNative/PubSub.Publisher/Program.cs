using System;
using System.Configuration;
using PubSub.Messages;
using Rebus.Activation;
using Rebus.Config;
using Rebus.Logging;
using Rebus.Persistence.FileSystem;
using Rebus.Persistence.InMem;
using Rebus.Transport.FileSystem;
// ReSharper disable BadControlBracesIndent

namespace PubSub.Publisher
{
    class Program
    {
        static void Main()
        {
            using (var activator = new BuiltinHandlerActivator())
            {
                var subscriberStore = new InMemorySubscriberStore();

                // TODO: Configura per usare Azure Storage Queue e SQL database per le sottoscrizioni...

                Configure.With(activator)
                    .Logging(l => l.ColoredConsole(minLevel: LogLevel.Warn))
                    .Transport(t => t.UseFileSystem(@"c:\rebus\", "SSEBridge"))
                    .Subscriptions(s => s.UseJsonFile(@"c:\rebus\subscriptions.json"))
                    .Start();

                //    .Transport(t => t.UseAzureServiceBus(GetConnectionString(), "SseBridge"))

                // for unit-tests
                //    .Transport(t => t.UseInMemoryTransport(new InMemNetwork(), "owin-test"))
                //    .Subscriptions(s => s.StoreInMemory(subscriberStore)) // useful in Unit-tests

                // for local Debug
                //    .Transport(t => t.UseFileSystem(@"c:\rebus\", "SseBridge"))
                //    .Subscriptions(s => s.UseJsonFile(@"c:\rebus\subscriptions.json"))

                var startupTime = DateTime.Now;

                while (true)
                {
                    Console.WriteLine(@"1) Publish string");
                    Console.WriteLine(@"2) Publish DateTime");
                    Console.WriteLine(@"3) Publish TimeSpan");
                    Console.WriteLine(@"4) Send ReverseGeoCode Command");
                    Console.WriteLine(@"q) Quit");

                    var keyChar = char.ToLower(Console.ReadKey(true).KeyChar);
                    var bus = activator.Bus.Advanced.SyncBus;

                    switch (keyChar)
                    {
                        case '1':
                            bus.Publish(new StringMessage("Hello there, this is a string message from a publisher!"));
                            break;

                        case '2':
                            bus.Publish(new DateTimeMessage(DateTime.Now));
                            break;

                        case '3':
                            bus.Publish(new TimeSpanMessage(DateTime.Now - startupTime));
                            break;

                        case 'q':
                            Console.WriteLine("Quitting!");
                            return;
                    }
                }
            }
        }

        static string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["servicebus"]?.ConnectionString
                ?? throw new ConfigurationErrorsException(@"Could not find 'servicebus' connection string. 

Please provide a valid Azure Service Bus connection string, most likely by going to your service bus namespace in the Azure Portal
and retrieving either 'Primary (...)' or 'Secondary Connection String' from the 'Shared Access Policies' tab.

If you create another SAS connection string, you need to give it 'Manage' rights, because Rebus (by default) wants to help
you create all of the necessary entities (queues, topics, subscriptions).

You may also provide a less priviledges SAS signature, but then you would need to create the entities yourself and disable
Rebus' ability to automatically create these things.");
        }
    }
}
