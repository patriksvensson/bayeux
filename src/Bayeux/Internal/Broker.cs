using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Bayeux.Internal
{
    internal sealed class Broker
    {
        private readonly Transport _transport;
        private readonly List<BayeuxProtocolExtension> _extensions;
        private readonly SemaphoreSlim _semaphore;
        private string _clientId;

        public Broker(Transport transport, IEnumerable<BayeuxProtocolExtension> extensions)
        {
            _transport = transport;
            _extensions = new List<BayeuxProtocolExtension>(extensions ?? Enumerable.Empty<BayeuxProtocolExtension>());
            _semaphore = new SemaphoreSlim(1, 1);
            _clientId = null;
        }

        public void SetClientId(string clientId)
        {
            using (new SemaphoreScope(_semaphore))
            {
                _clientId = clientId;
            }
        }

        public async Task<TransportResponse> Send(Message message, CancellationToken token)
        {
            using (new SemaphoreScope(_semaphore))
            {
                // Set the message client ID.
                message.ClientId = _clientId;

                // Add extensions to message.
                if (_extensions.Count > 0)
                {
                    foreach (var extension in _extensions)
                    {
                        if (extension.TryExtendOutgoing(message, out object obj))
                        {
                            if (message.Extension == null)
                            {
                                message.Extension = new Dictionary<string, object>();
                            }
                            message.Extension[extension.Name] = obj;
                        }
                    }
                }
            }

            return await _transport.Send(message, token);
        }
    }
}
