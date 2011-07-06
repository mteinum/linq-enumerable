using System;
using log4net;
using NServiceBus;

namespace NServiceBusDemo
{
    [Serializable]
    public class SomeMessage : IMessage
    {
        public string Text;
    }

    [Serializable]
    public class AnotherMessage : IMessage
    {
        public string Text;
    }
}