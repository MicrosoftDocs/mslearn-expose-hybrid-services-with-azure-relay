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
        private const string RelayNamespace = "creditcheck.servicebus.windows.net";
        private const string ConnectionName = "creditcheckconnection";
        private const string KeyName = "RootManageSharedAccessKey";
        private const string Key = "<your key here>";

        static void Main(string[] args)
        {
            Console.WriteLine("Starting credit check listener.");
            RunAsync().GetAwaiter().GetResult();
        }

        private static async Task RunAsync()
        {
            var cts = new CancellationTokenSource();
            var tokenProvider = TokenProvider.CreateSharedAccessSignatureTokenProvider(KeyName, Key);
            var listener = new HybridConnectionListener(new Uri(string.Format("sb://{0}/{1}", RelayNamespace, ConnectionName)), tokenProvider);

            // Subscribe to the status events
            listener.Connecting += (o, e) => { Console.WriteLine("Connecting"); };
            listener.Offline += (o, e) => { Console.WriteLine("Credit check service is offline"); };
            listener.Online += (o, e) => { Console.WriteLine("Credit check service is online"); };

            // Create an array of credit status values
            List<string> creditStatuses = new List<string>();
            creditStatuses.Add("Good");
            creditStatuses.Add("Some issues");
            creditStatuses.Add("Bad");

            // Provide an HTTP request handler
            listener.RequestHandler = (context) =>
            {
                // Obtain the name from the request
                TextReader tr = new StreamReader(context.Request.InputStream);
                String requestedName = tr.ReadToEnd();
                Console.WriteLine("A request was received to check credit for: " + requestedName);

                // Pick a status at random
                Random r = new Random();
                int index = r.Next(creditStatuses.Count);

                // Formulate and send the response
                context.Response.StatusCode = HttpStatusCode.OK;
                context.Response.StatusDescription = "Credit check successful";
                using (var sw = new StreamWriter(context.Response.OutputStream))
                {
                    sw.WriteLine("Credit check for {0}: {1}", requestedName, creditStatuses[index]);
                }

                // Close the context
                context.Response.Close();
            };

            // Open the listener            
            await listener.OpenAsync();
            Console.WriteLine("Server listening");

            // Start a new thread that will continuously read the console.
            await Console.In.ReadLineAsync();

            // Close the listener after you exit the processing loop.
            await listener.CloseAsync();
        }
    }
}
