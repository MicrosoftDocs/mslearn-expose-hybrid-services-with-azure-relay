using System;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.Azure.Relay;

namespace sender
{
    class Program
    {
        // Details of the Azure Relay
        private const string RelayNamespace = "creditcheck.servicebus.windows.net";
        private const string ConnectionName = "creditcheckconnection";
        private const string KeyName = "RootManageSharedAccessKey";
        private const string Key = "<Your key here>";
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
            var client = new HttpClient();
            var request = new HttpRequestMessage()
            {
                RequestUri = uri,
                Method = HttpMethod.Get,
                Content = new StringContent(name)
            };
            request.Headers.Add("ServiceBusAuthorization", token);

            // Send the request
            var response = await client.SendAsync(request);

            // Display the result
            Console.WriteLine(await response.Content.ReadAsStringAsync());

            // Wait for the user to press return
            Console.ReadLine();
        }
    }
}
