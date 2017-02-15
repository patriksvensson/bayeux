using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bayeux.Diagnostics;
using Newtonsoft.Json;

// ReSharper disable once CheckNamespace
namespace Bayeux.Internal
{
    internal sealed class LongPollingTransport : Transport
    {
        private readonly Uri _endpoint;
        private readonly IBayeuxLogger _logger;
        private readonly HttpClient _client;

        public LongPollingTransport(Uri endpoint, IBayeuxLogger logger)
        {
            _endpoint = endpoint;
            _logger = logger ?? new DefaultLogger();
            _client = new HttpClient { Timeout = TimeSpan.FromMilliseconds(Timeout.Infinite) };
        }

        public override async Task<TransportResponse> Send(Message message, CancellationToken token)
        {
            // Create the request.
            _logger.Write(BayeuxLogLevel.Debug, "[REQUEST] Channel = {0}", message.Channel);
            var request = CreateRequest(message);

            // Get the response.
            var response = await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, token);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                _logger.Write(BayeuxLogLevel.Warning, "[REQUEST] Received HTTP Status Code {0} for channel {1}.", response.StatusCode, message.Channel);
                return null;
            }

            var stream = await response.Content.ReadAsStreamAsync();

            var result = new TransportResponse();
            using (var reader = new StreamReader(stream))
            {
                while (true)
                {
                    var json = await reader.ReadLineAsync().WithCancellation(token);
                    if (json == null)
                    {
                        break;
                    }

                    var replies = JsonConvert.DeserializeObject<Envelope>(json);
                    foreach (var reply in replies)
                    {
                        if (reply.Channel == message.Channel)
                        {
                            _logger.Write(BayeuxLogLevel.Debug, "[RESPONSE] Channel = {0}, ClientId = {1}, Error = {2}", reply.Channel, reply.ClientId, reply.Error ?? "No error");
                            result.Response = reply;
                        }
                        else
                        {
                            if (result.Messages == null)
                            {
                                result.Messages = new List<Message>();
                            }
                            _logger.Write(BayeuxLogLevel.Debug, "[MESSAGE] Channel = {0}", reply.Channel);
                            result.Messages.Add(reply);
                        }
                    }
                }
            }
            return result;
        }

        private HttpRequestMessage CreateRequest(Message message)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, _endpoint);
            var json = new Envelope(message).Serialize();
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            return request;
        }
    }
}
