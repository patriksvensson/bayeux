using Newtonsoft.Json;

// ReSharper disable once CheckNamespace
namespace Bayeux.Internal
{
    internal sealed class Advice
    {
        [JsonProperty(PropertyName = "reconnect")]
        public string Reconnect { get; set; }

        [JsonProperty(PropertyName = "interval")]
        public int Interval { get; set; }

        [JsonProperty(PropertyName = "timeout")]
        public int Timeout { get; set; }
    }
}
