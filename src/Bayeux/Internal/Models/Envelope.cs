using System.Collections.Generic;
using Newtonsoft.Json;

// ReSharper disable once CheckNamespace
namespace Bayeux.Internal
{
    internal sealed class Envelope : List<Message>
    {
        public Envelope()
        {
        }

        public Envelope(Message message)
        {
            Add(message);
        }

        public string Serialize()
        {
            return JsonConvert.SerializeObject(this, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Include
            });
        }
    }
}
