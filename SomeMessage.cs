using System;
using NServiceBus;

namespace NServiceBusDemo
{
    [Serializable]
    public class SomeMessage : IMessage
    {
        public string Text;
    }
}