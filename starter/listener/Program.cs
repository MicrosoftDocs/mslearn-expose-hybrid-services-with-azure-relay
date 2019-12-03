using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Relay;
using System.Net;

namespace listener
{
    class Program
    {
        // Details of the Azure Relay

        static void Main(string[] args)
        {
            Console.WriteLine("Starting credit check listener.");
            RunAsync().GetAwaiter().GetResult();
        }

        private static async Task RunAsync()
        {
            // Create the listener
            var cts = new CancellationTokenSource();
            var tokenProvider = TokenProvider.CreateSharedAccessSignatureTokenProvider(KeyName, Key);
            var listener = new HybridConnectionListener(new Uri(string.Format("sb://{0}/{1}", RelayNamespace, ConnectionName)), tokenProvider);

            // Subscribe to the status events

            // Create an array of credit status values
            List<string> creditStatuses = new List<string>();
            creditStatuses.Add("Good");
            creditStatuses.Add("Some issues");
            creditStatuses.Add("Bad");

            // Provide an HTTP request handler
            listener.RequestHandler = (context) =>
            {
                // Obtain the name from the request

                // Pick a status at random
                Random r = new Random();
                int index = r.Next(creditStatuses.Count);

                // Formulate and send the response

                // Close the context
                context.Response.Close();
            };

            // Open the listener

            // Start a new thread that will continuously read the console.
            await Console.In.ReadLineAsync();

            // Close the listener after you exit the processing loop.
            await listener.CloseAsync();
        }
    }
}
