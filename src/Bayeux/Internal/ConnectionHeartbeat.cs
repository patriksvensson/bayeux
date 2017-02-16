using System.Threading;
using System.Threading.Tasks;

namespace Bayeux.Internal
{
    internal sealed class ConnectionHeartbeat : TaskWrapper<ConnectionHeartbeatContext>
    {
        private readonly Broker _broker;
        private readonly MessageQueue _queue;

        public ConnectionHeartbeat(Broker broker, MessageQueue queue)
        {
            _broker = broker;
            _queue = queue;
        }

        protected override async Task Run(ConnectionHeartbeatContext context, CancellationToken token)
        {
            try
            {
                while (true)
                {
                    // Connect and do long polling.
                    var result = await _broker.SendConnect(token);
                    FlushMessages(result, token);

                    // TODO: Respect advice from server.
                    token.ThrowIfCancellationRequested();
                }
            }
            finally
            {
                // Send disconnect and flush messages.
                var result = await _broker.SendDisconnect();
                FlushMessages(result, CancellationToken.None);
            }
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
