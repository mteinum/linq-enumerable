using NServiceBus;
using NServiceBus.ObjectBuilder;

namespace NServiceBusDemo
{
    public static class ConfigureSoapSerializer
    {
        public static Configure SoapSerializer(this Configure config)
        {
            config.Configurer.ConfigureComponent(
                typeof(SoapMessageSerializer),
                ComponentCallModelEnum.Singleton);

            return config;
        }
    }
}