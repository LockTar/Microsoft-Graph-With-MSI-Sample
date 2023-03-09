using Azure.Identity;
using Microsoft.Graph;
using Microsoft.Kiota.Abstractions.Authentication;
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

            var authenticationProvider = new BaseBearerTokenAuthenticationProvider(new TokenProvider());
            var graphServiceClient = new GraphServiceClient(authenticationProvider);

            //var graphServiceClient = new GraphServiceClient(
            //    new DelegateAuthenticationProvider((requestMessage) =>
            //    {
            //        requestMessage
            //            .Headers
            //            .Authorization = new AuthenticationHeaderValue("Bearer", token);

            //        return Task.CompletedTask;
            //    }));

            return graphServiceClient;
        }
    }

    public class TokenProvider : IAccessTokenProvider
    {
        public async Task<string> GetAuthorizationTokenAsync(
            Uri uri, 
            Dictionary<string, object> additionalAuthenticationContext = default,
            CancellationToken cancellationToken = default)
        {
            var credential = new DefaultAzureCredential();
            var tokenResult = await credential.GetTokenAsync(new Azure.Core.TokenRequestContext(new string[] { "https://graph.microsoft.com" }));

            var token = tokenResult.Token;

            // get the token and return it in your own way
            return token;
        }

        public AllowedHostsValidator AllowedHostsValidator { get; }
    }
}
