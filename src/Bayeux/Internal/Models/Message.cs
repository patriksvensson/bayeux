using System.Collections.Generic;
using Newtonsoft.Json;

// ReSharper disable once CheckNamespace
namespace Bayeux.Internal
{
    internal sealed class Message : IBayeuxMessage
    {
        [JsonProperty(PropertyName = "channel")]
        public string Channel { get; set; }

        [JsonProperty(PropertyName = "clientId")]
        public string ClientId { get; set; }

        [JsonProperty(PropertyName = "successful")]
        internal bool Successful { get; set; }

        [JsonProperty(PropertyName = "advice")]
        internal Advice Advice { get; set; }

        [JsonProperty(PropertyName = "version")]
        internal string Version { get; set; }

        [JsonProperty(PropertyName = "minimumVersion")]
        internal string MinimumVersion { get; set; }

        [JsonProperty(PropertyName = "supportedConnectionTypes")]
        internal string[] SupportedConnectionTypes { get; set; }

        [JsonProperty(PropertyName = "connectionType")]
        internal string ConnectionType { get; set; }

        [JsonProperty(PropertyName = "subscription")]
        internal string Subscription { get; set; }

        [JsonProperty(PropertyName = "error")]
        public string Error { get; set; }

        [JsonProperty(PropertyName = "ext")]
        public Dictionary<string, object> Extension { get; set; }

        [JsonProperty(PropertyName = "data")]
        public object Data { get; set; }
    }
}
