using System;
using System.Collections;
using System.Linq;
using log4net;
using NServiceBus;
using NServiceBus.Unicast;

namespace NServiceBusDemo
{
    class Program
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(AnotherMessageHandler));

        static string GetName(Type type)
        {
            return type.AssemblyQualifiedName;
        }

        static void Main()
        {
            SetLoggingLibrary.Log4Net(log4net.Config.XmlConfigurator.Configure);

            Logger.Debug("Program start");


            var mapping = new Hashtable
            {
               { GetName(typeof(SomeMessage)), "SomeMessage.Input" },
               { GetName(typeof(AnotherMessage)), "AnotherMessage.Input" }
            };

            AddReceiverBus(
                "SomeMessage.Input",
                typeof(SomeMessageHandler));

            AddReceiverBus(
                "AnotherMessage.Input",
                typeof(AnotherMessageHandler));

            StructureMap.ObjectFactory.Inject(
                CreateSenderBus(mapping));

            SendMessageOnTheBus();

            // wait for exit
            Console.ReadLine();
        }

        private static IBus CreateSenderBus(Hashtable mapping)
        {
            return Configure.With()
                .StructureMapBuilder()
                .XmlSerializer()
                .MsmqTransport()
                    .IsTransactional(true)
                .UnicastBus()
                    .ImpersonateSender(false)
                    .AddMapping(mapping)
                .CreateBus()
                .Start();
        }

        private static void AddReceiverBus(string inputQueue, Type handler)
        {
            var types = typeof(UnicastBus).Assembly.GetTypes().Union(
                new[] { handler });

            Configure.With(types)
                .CustomConfigurationSource(new MyConfigurationSource(inputQueue))
                .StructureMapBuilder()
                .XmlSerializer()
                .MsmqTransport()
                    .IsTransactional(true)
                    .MsmqSubscriptionStorage()
                .UnicastBus()
                    .ImpersonateSender(false)
                    .LoadMessageHandlers()
                .CreateBus()
                .Start();
        }

        private static void SendMessageOnTheBus()
        {
            var bus = StructureMap.ObjectFactory.GetInstance<IBus>();

            bus.Send(new SomeMessage { Text = "Hello World!" });
        }
    }
}
