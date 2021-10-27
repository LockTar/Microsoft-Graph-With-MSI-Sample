using Azure.Identity;
using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace MicrosoftGraphWithMsi.Helpers
{
    internal static class GraphClientHelper
    {
        internal static async Task<GraphServiceClient> InitializeGraphClientWithMsiAsync()
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
