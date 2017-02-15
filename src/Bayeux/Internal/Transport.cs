using System.Threading;
using System.Threading.Tasks;

namespace Bayeux.Internal
{
    internal abstract class Transport
    {
        public abstract Task<TransportResponse> Send(Message message, CancellationToken token);
    }
}
