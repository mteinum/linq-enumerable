using System.Collections;
using NServiceBus;
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
}