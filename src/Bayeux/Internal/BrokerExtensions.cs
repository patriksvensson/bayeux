using System.Threading;
using System.Threading.Tasks;

namespace Bayeux.Internal
{
    internal static class BrokerExtensions
    {
        public static async Task<TransportResponse> SendHandshake(this Broker broker)
        {
            var message = new Message
            {
                Channel = "/meta/handshake",
                Version = "1.0",
                MinimumVersion = "1.0",
                SupportedConnectionTypes = new[] { "long-polling" }
            };
            return await broker.Send(message, CancellationToken.None);
        }

        public static async Task<TransportResponse> SendConnect(this Broker broker, string clientId, CancellationToken token)
        {
            var message = new Message()
            {
                Channel = "/meta/connect",
                ClientId = clientId,
                ConnectionType = "long-polling"
            };
            return await broker.Send(message, token);
        }

        public static async Task<TransportResponse> SendSubscribe(this Broker broker, string clientId, string channel)
        {
            var message = new Message
            {
                Channel = "/meta/subscribe",
                ClientId = clientId,
                Subscription = channel
            };
            return await broker.Send(message, CancellationToken.None);
        }

        public static async Task<TransportResponse> SendDisconnect(this Broker broker, string clientId)
        {
            var message = new Message
            {
                ClientId = clientId,
                Channel = "/meta/disconnect"
            };
            return await broker.Send(message, CancellationToken.None);
        }
    }
}
