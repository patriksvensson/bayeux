using System;
using System.Threading;

namespace Bayeux.Internal
{
    internal sealed class ConnectionHeartbeatContext
    {
        private readonly CancellationTokenSource _source;

        public string ClientId { get; set; }
        public CancellationToken Token => _source.Token;

        public ConnectionHeartbeatContext(string clientId)
        {
            _source = new CancellationTokenSource();
            ClientId = clientId;
        }

        public void Cancel()
        {
            _source.Cancel();
        }

        public void ThrowIfCancellationRequested(TimeSpan timeout)
        {
            if (_source.Token.WaitHandle.WaitOne(timeout))
            {
                _source.Token.ThrowIfCancellationRequested();
            }
        }
    }
}
