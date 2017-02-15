using System;
using System.Threading;

// ReSharper disable once CheckNamespace
namespace Bayeux.Internal
{
    internal sealed class SemaphoreScope : IDisposable
    {
        private readonly SemaphoreSlim _semaphore;

        public SemaphoreScope(SemaphoreSlim semaphore)
            : this(semaphore, CancellationToken.None)
        {
        }

        public SemaphoreScope(SemaphoreSlim semaphore, CancellationToken token)
        {
            _semaphore = semaphore;
            _semaphore.Wait(token);
        }

        public void Dispose()
        {
            _semaphore.Release();
        }
    }
}
