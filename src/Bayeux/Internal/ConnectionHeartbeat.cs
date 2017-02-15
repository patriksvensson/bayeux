using System.Threading;
using System.Threading.Tasks;

namespace Bayeux.Internal
{
    internal sealed class ConnectionHeartbeat : TaskWrapper<ConnectionHeartbeatContext>
    {
        private readonly Broker _broker;
        private readonly MessageQueue _queue;

        public string ClientId { get; set; }

        public ConnectionHeartbeat(Broker broker, MessageQueue queue)
        {
            _broker = broker;
            _queue = queue;
        }

        protected override void Setup(ConnectionHeartbeatContext context)
        {
            ClientId = context.ClientId;
        }

        protected override async Task Run(ConnectionHeartbeatContext context, CancellationToken token)
        {
            try
            {
                while (true)
                {
                    // Connect and do long polling.
                    var result = await _broker.SendConnect(context.ClientId, token);
                    FlushMessages(result, token);

                    // TODO: Respect advice from server.
                    token.ThrowIfCancellationRequested();
                }
            }
            finally
            {
                // Send disconnect and flush messages.
                var result = await _broker.SendDisconnect(ClientId);
                FlushMessages(result, CancellationToken.None);
            }
        }

        protected override void Teardown(ConnectionHeartbeatContext context)
        {
            // Reset client ID.
            ClientId = null;
        }

        private void FlushMessages(TransportResponse result, CancellationToken token)
        {
            if (result?.Messages != null)
            {
                foreach (var message in result.Messages)
                {
                    token.ThrowIfCancellationRequested();
                    _queue.Enqueue(message);
                }
            }
        }
    }
}
