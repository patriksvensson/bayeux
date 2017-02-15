using System;
using System.Threading;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace Bayeux.Internal
{
    internal abstract class TaskWrapper<T>
        where T : class
    {
        private readonly ManualResetEvent _started;
        private readonly ManualResetEvent _stopped;
        private CancellationTokenSource _source;

        public bool IsRunning => _started.WaitOne(0);

        protected TaskWrapper()
        {
            _started = new ManualResetEvent(false);
            _stopped = new ManualResetEvent(true);
        }

        public void Start(T context)
        {
            if (_stopped.WaitOne(0))
            {
                _source = new CancellationTokenSource();
                Task.Factory.StartNew(async state => await Execute(state), context, TaskCreationOptions.LongRunning);
                _started.WaitOne();
            }
        }

        public void Stop()
        {
            if (_started.WaitOne(0))
            {
                _source.Cancel();
                _stopped.WaitOne();
            }
        }

        protected virtual void Setup(T context)
        {
        }

        protected abstract Task Run(T context, CancellationToken token);

        protected virtual void Teardown(T context)
        {
        }

        private async Task Execute(object state)
        {
            var context = state as T;

            try
            {
                Setup(context);

                _stopped.Reset();
                _started.Set();

                await Run(context, _source.Token);
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                Teardown(context);

                _stopped.Set();
                _started.Reset();
            }
        }
    }
}
