using System.Collections.Concurrent;
using System.Threading;

namespace Bayeux.Internal
{
    internal sealed class MessageQueue
    {
        private readonly BlockingCollection<IBayeuxMessage> _queue;

        public MessageQueue()
        {
            _queue = new BlockingCollection<IBayeuxMessage>(new ConcurrentQueue<IBayeuxMessage>());
        }

        public void Enqueue(IBayeuxMessage @event)
        {
            if (!_queue.IsAddingCompleted)
            {
                _queue.Add(@event);
            }
        }

        public IBayeuxMessage Dequeue(CancellationToken token)
        {
            return _queue.TryTake(out IBayeuxMessage entry, Timeout.Infinite, token) ? entry : null;
        }
    }
}
