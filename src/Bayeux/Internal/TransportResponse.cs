using System.Collections.Generic;

namespace Bayeux.Internal
{
    internal sealed class TransportResponse
    {
        public Message Response { get; set; }
        public List<Message> Messages { get; set; }
    }
}
