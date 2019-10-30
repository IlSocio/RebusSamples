﻿using System;
using System.Configuration;
using PubSub.Messages;
using Rebus.Activation;
using Rebus.Config;
using Rebus.Logging;
using Rebus.Persistence.FileSystem;
// ReSharper disable BadControlBracesIndent

namespace PubSub.Publisher
{
    class Program
    {
        static readonly string JsonFilePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "rebus_subscriptions.json");

        static void Main()
        {
            using (var activator = new BuiltinHandlerActivator())
            {

                Configure.With(activator)
                    .Logging(l => l.ColoredConsole(minLevel: LogLevel.Warn))
                    .Transport(t => t.UseAzureServiceBus(GetConnectionString(), "publisher"))
                    //                    .Subscriptions(s => s.UseJsonFile(JsonFilePath))
                    .Start();

                var startupTime = DateTime.Now;

                while (true)
                {
                    Console.WriteLine(@"a) Publish string
b) Publish DateTime
c) Publish TimeSpan
q) Quit");

                    var keyChar = char.ToLower(Console.ReadKey(true).KeyChar);
                    var bus = activator.Bus.Advanced.SyncBus;

                    switch (keyChar)
                    {
                        case 'a':
                            bus.Publish(new StringMessage("Hello there, this is a string message from a publisher!"));
                            break;

                        case 'b':
                            bus.Publish(new DateTimeMessage(DateTime.Now));
                            break;

                        case 'c':
                            bus.Publish(new TimeSpanMessage(DateTime.Now - startupTime));
                            break;

                        case 'q':
                            goto consideredHarmful;

                        default:
                            Console.WriteLine($"There's no option '{keyChar}'");
                            break;
                    }
                }

                consideredHarmful: Console.WriteLine("Quitting!");
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
