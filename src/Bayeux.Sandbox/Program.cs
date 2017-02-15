using System;

namespace Bayeux.Sandbox
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Create the client settings.
            var endpoint = new Uri("http://localhost:8000/faye");
            var settings = new BayeuxClientSettings(endpoint)
            {
                Logger = new ConsoleLogger()
            };

            // Create the client.
            using (var client = new BayeuxClient(settings))
            {
                // Connect to server.
                client.Connect();

                // Subscribe to channel.
                client.Subscribe("/test", message => Console.WriteLine("Message received: {0}", message.Channel));

                // Wait for exit.
                Console.WriteLine("Press ANY key to quit.");
                Console.ReadKey(true);
            }
        }
    }
}