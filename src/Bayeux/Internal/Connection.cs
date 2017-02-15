using System;
using System.Collections.Generic;
using Bayeux.Diagnostics;

namespace Bayeux.Internal
{
    internal sealed class Connection
    {
        private readonly Broker _broker;
        private readonly ConnectionHeartbeat _heartbeat;

        public Connection(Uri endpoint, MessageQueue queue, IEnumerable<BayeuxProtocolExtension> extensions, IBayeuxLogger logger)
        {
            _broker = new Broker(new LongPollingTransport(endpoint, logger), extensions);
            _heartbeat = new ConnectionHeartbeat(_broker, queue);
        }

        public void Connect()
        {
            if (!_heartbeat.IsRunning)
            {
                // Send handshake to server.
                var handshake = _broker.SendHandshake().Result;
                if (handshake?.Response == null || !handshake.Response.Successful)
                {
                    var message = $"Could not connect to server. {handshake?.Response?.Error}".Trim();
                    throw new InvalidOperationException(message);
                }

                // Create a new cancellation token source.
                var context = new ConnectionHeartbeatContext(handshake.Response.ClientId);
                _heartbeat.Start(context);
            }
        }

        public void Disconnect()
        {
            if (_heartbeat.IsRunning)
            {
                _heartbeat.Stop();
            }
        }

        public void Subscribe(string channel)
        {
            if (!_heartbeat.IsRunning)
            {
                throw new InvalidOperationException("Not connected to server.");
            }
            _broker.SendSubscribe(_heartbeat.ClientId, channel).Wait();
        }
    }
}
