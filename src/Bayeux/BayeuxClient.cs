﻿using System;
using Bayeux.Internal;

namespace Bayeux
{
    public sealed class BayeuxClient : IDisposable
    {
        private readonly MessageQueue _queue;
        private readonly Connection _connection;
        private readonly MessageRouter _router;

        public BayeuxClient(Uri endpoint)
            : this(new BayeuxClientSettings(endpoint))
        {
        }

        public BayeuxClient(BayeuxClientSettings settings)
        {
            _queue = new MessageQueue();
            _connection = new Connection(settings.Endpoint, _queue, settings.Extensions, settings.Logger);
            _router = new MessageRouter();
        }

        void IDisposable.Dispose()
        {
            Disconnect();
        }

        public void Connect()
        {
            _connection.Connect();
            _router.Start(_queue);
        }

        public void Disconnect()
        {
            _connection.Disconnect();
            _router.Stop();
        }

        public void Subscribe(string channel, Action<IBayeuxMessage> callback)
        {
            _connection.Subscribe(channel);
            _router.Subscribe(channel, callback);
        }
    }
}
