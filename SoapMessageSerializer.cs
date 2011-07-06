using System.IO;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Soap;
using NServiceBus;
using NServiceBus.Serialization;

namespace NServiceBusDemo
{
    public class SoapMessageSerializer : IMessageSerializer
    {
        private readonly SoapFormatter _formatter;

        public SoapMessageSerializer()
        {
            _formatter = new SoapFormatter
            {
                AssemblyFormat = FormatterAssemblyStyle.Simple,
                TypeFormat = FormatterTypeStyle.TypesWhenNeeded
            };
        }

        public void Serialize(IMessage[] messages, Stream stream)
        {
            _formatter.Serialize(stream, messages);
        }

        public IMessage[] Deserialize(Stream stream)
        {
            return (IMessage[])_formatter.Deserialize(stream);
        }
    }
}