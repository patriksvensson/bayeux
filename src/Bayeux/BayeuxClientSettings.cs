using System;
using System.Collections.Generic;
using Bayeux.Diagnostics;

namespace Bayeux
{
    public sealed class BayeuxClientSettings
    {
        public Uri Endpoint { get; }
        public IBayeuxLogger Logger { get; set; }
        public ICollection<BayeuxProtocolExtension> Extensions { get; }

        public BayeuxClientSettings(Uri endpoint)
        {
            Endpoint = endpoint;
            Extensions = new List<BayeuxProtocolExtension>();
        }
    }
}