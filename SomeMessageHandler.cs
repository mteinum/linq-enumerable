using NServiceBus;

namespace NServiceBusDemo
{
    public class SomeMessageHandler : IHandleMessages<SomeMessage>
    {
        public void Handle(SomeMessage message)
        {
            var bus = StructureMap.ObjectFactory.GetInstance<IBus>();
            bus.Send(new AnotherMessage { Text = message.Text });
        }
    }
}