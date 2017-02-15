using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Bayeux.Internal
{
    internal sealed class MessageRouter : TaskWrapper<MessageQueue>
    {
        private readonly SemaphoreSlim _semaphore;
        private readonly Dictionary<string, List<Action<IBayeuxMessage>>> _callbacks;

        public MessageRouter()
        {
            _semaphore = new SemaphoreSlim(1, 1);
            _callbacks = new Dictionary<string, List<Action<IBayeuxMessage>>>(StringComparer.OrdinalIgnoreCase);
        }

        public void Subscribe(string channel, Action<IBayeuxMessage> callback)
        {
            using (new SemaphoreScope(_semaphore))
            {
                if (!_callbacks.ContainsKey(channel))
                {
                    _callbacks.Add(channel, new List<Action<IBayeuxMessage>>());
                }
                _callbacks[channel].Add(callback);
            }
        }

        // ReSharper disable once FunctionNeverReturns
        protected override Task Run(MessageQueue queue, CancellationToken token)
        {
            while (true)
            {
                var message = queue.Dequeue(token);
                if (message != null)
                {
                    using (new SemaphoreScope(_semaphore))
                    {
                        // ReSharper disable once CollectionNeverUpdated.Local
                        if (_callbacks.TryGetValue(message.Channel, out List<Action<IBayeuxMessage>> callbacks))
                        {
                            // TODO: Support channel glob patterns.
                            foreach (var callback in callbacks)
                            {
                                token.ThrowIfCancellationRequested();
                                callback(message);
                            }
                        }
                    }
                }

                token.ThrowIfCancellationRequested();
            }
        }
    }
}
