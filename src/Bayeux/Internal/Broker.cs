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

        public Broker(Transport transport, IEnumerable<BayeuxProtocolExtension> extensions)
        {
            _transport = transport;
            _extensions = new List<BayeuxProtocolExtension>(extensions ?? Enumerable.Empty<BayeuxProtocolExtension>());
        }

        public async Task<TransportResponse> Send(Message message, CancellationToken token)
        {
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
            return await _transport.Send(message, token);
        }
    }
}
