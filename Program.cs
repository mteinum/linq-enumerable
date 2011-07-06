using System;
using System.Collections;
using System.Linq;
using log4net;
using NServiceBus;
using NServiceBus.Config;
using NServiceBus.Config.ConfigurationSource;
using System.Configuration;
using NServiceBus.Unicast;
using NServiceBus.Unicast.Config;

namespace NServiceBusDemo
{
    public static class MyMappingExtension
    {
        public static ConfigUnicastBus AddMapping(this ConfigUnicastBus config, Hashtable mapping)
        {
            config.RunCustomAction(() =>
                Configure.Instance.Configurer.ConfigureProperty<UnicastBus>(x => x.MessageOwners, mapping)
                );

            return config;
        }
    }

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
                    //.AddMapping(mapping)
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

    sealed class MyConfigurationSource : IConfigurationSource
    {
        private readonly string _inputQueue;

        public MyConfigurationSource(string inputQueue)
        {
            _inputQueue = inputQueue;
        }

        public T GetConfiguration<T>() where T : class
        {
            Console.WriteLine("==== {0} ==== ", typeof(T));

            if (typeof(T) == typeof(MsmqTransportConfig))
                return new MsmqTransportConfig
                           {
                               InputQueue = _inputQueue
                           } as T;

            return ConfigurationManager.GetSection(typeof(T).Name) as T;
        }
    }
}
