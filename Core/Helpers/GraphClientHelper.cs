using Azure.Identity;
using Microsoft.Graph;
using Microsoft.Kiota.Abstractions.Authentication;

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

            TokenProvider accessTokenProvider = new TokenProvider(credential);
            var authenticationProvider = new BaseBearerTokenAuthenticationProvider(accessTokenProvider);
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
        private readonly DefaultAzureCredential _credential;

        public TokenProvider(DefaultAzureCredential credential)
        {
            _credential = credential;
        }

        public async Task<string> GetAuthorizationTokenAsync(
            Uri uri, 
            Dictionary<string, object> additionalAuthenticationContext = default,
            CancellationToken cancellationToken = default)
        {            
            var tokenResult = await _credential.GetTokenAsync(new Azure.Core.TokenRequestContext(new string[] { "https://graph.microsoft.com" }));

            var token = tokenResult.Token;

            return token;
        }

        public AllowedHostsValidator AllowedHostsValidator { get; }
    }
}
