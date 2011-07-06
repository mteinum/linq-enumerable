using log4net;
using NServiceBus;

namespace NServiceBusDemo
{
    public class AnotherMessageHandler : IMessageHandler<AnotherMessage>
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(AnotherMessageHandler));

        public void Handle(AnotherMessage message)
        {
            Logger.DebugFormat("Message received: {0}", message.Text);
        }
    }
}