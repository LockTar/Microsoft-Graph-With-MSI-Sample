using Azure.Identity;
using Microsoft.Graph;
using System.Net.Http.Headers;

namespace Core.Helpers
{
    public static class GraphClientHelper
    {
        public static async Task<GraphServiceClient> InitializeGraphClientWithMsiAsync()
        {
            var credential = new DefaultAzureCredential();
            var tokenResult = await credential.GetTokenAsync(new Azure.Core.TokenRequestContext(new string[] { "https://graph.microsoft.com" }));

            string token = tokenResult.Token;
            Console.WriteLine("MSI token that is used in the GraphServiceClient:");
            Console.WriteLine(token);
            Console.WriteLine("\n\n");

            var client = new GraphServiceClient(
                new DelegateAuthenticationProvider((requestMessage) =>
                {
                    requestMessage
                        .Headers
                        .Authorization = new AuthenticationHeaderValue("Bearer", token);

                    return Task.CompletedTask;
                }));

            return client;
        }
    }
}
