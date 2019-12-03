using System;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.Azure.Relay;

namespace sender
{
    class Program
    {
        // Details of the Azure Relay
        static void Main(string[] args)
        {
            Console.WriteLine("Starting credit check sender...");
            RunAsync().GetAwaiter().GetResult();
        }

        private static async Task RunAsync()
        {
            // Get a name from the user
            Console.WriteLine("Enter a name to check:");
            String name = Console.ReadLine();

            var tokenProvider = TokenProvider.CreateSharedAccessSignatureTokenProvider(KeyName, Key);
            var uri = new Uri(string.Format("https://{0}/{1}", RelayNamespace, ConnectionName));
            var token = (await tokenProvider.GetTokenAsync(uri.AbsoluteUri, TimeSpan.FromHours(1))).TokenString;
            
            // Create an HttpClient and formulate the request

            // Send the request

            // Display the result

            // Wait for the user to press return
            Console.ReadLine();
        }
    }
}
