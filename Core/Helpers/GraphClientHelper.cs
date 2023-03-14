using Azure.Core;
using Azure.Identity;
using Core.Logging;
using Microsoft.Graph;
using Microsoft.Graph.Authentication;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary.Middleware;
using System.Net.Http;

namespace Core.Helpers
{
    public static class GraphClientHelper
    {
        private const string TenantId = "";
        private const string ClientId = "";
        private const string ClientSecret = "";

        public static async Task<GraphServiceClient> InitializeGraphClientWithMsiAsync(bool enableHttpRequestHandler)
        {
            ////DefaultAzureCredentialOptions options = new DefaultAzureCredentialOptions()
            ////{
            ////    ExcludeAzurePowerShellCredential = true,
            ////    ExcludeEnvironmentCredential = true,
            ////    ExcludeInteractiveBrowserCredential = true,
            ////    ExcludeManagedIdentityCredential = true,
            ////    ExcludeSharedTokenCacheCredential = true,
            ////    ExcludeVisualStudioCodeCredential = true,
            ////    ExcludeVisualStudioCredential = true,
            ////    ExcludeAzureCliCredential = false
            ////};
            ////var credential = new DefaultAzureCredential(options);

            var credential = new DefaultAzureCredential();
            var tokenResult = await credential.GetTokenAsync(new Azure.Core.TokenRequestContext(new string[] { "https://graph.microsoft.com" }));

            string token = tokenResult.Token;
            Console.WriteLine("MSI token that is used in the GraphServiceClient:");
            Console.WriteLine(token);
            Console.WriteLine("\n\n");

            TokenProvider accessTokenProvider = new TokenProvider(credential);
            var authProvider = new BaseBearerTokenAuthenticationProvider(accessTokenProvider);

            HttpClient httpClient = GetHttpClientWithDefaultAndCustomHandlers(enableHttpRequestHandler);

            var graphClient = new GraphServiceClient(httpClient, authProvider);

            return graphClient;
        }

        public static async Task<GraphServiceClient> InitializeGraphClientWithClientCredentialsAsync(bool enableHttpRequestHandler)
        {
            string[] scopes;
            ClientSecretCredential clientSecretCredential;
            SetupClientSecretCredentialAndScopes(out scopes, out clientSecretCredential);

            var authProvider = new AzureIdentityAuthenticationProvider(clientSecretCredential, scopes: scopes);

            HttpClient httpClient = GetHttpClientWithDefaultAndCustomHandlers(enableHttpRequestHandler);

            var graphClient = new GraphServiceClient(httpClient, authProvider);

            return graphClient;
        }

        public static async Task<Microsoft.Graph.Beta.GraphServiceClient> InitializeGraphBetaClientWithClientCredentialsAsync(bool enableHttpRequestHandler)
        {
            string[] scopes;
            ClientSecretCredential clientSecretCredential;
            SetupClientSecretCredentialAndScopes(out scopes, out clientSecretCredential);

            var authProvider = new AzureIdentityAuthenticationProvider(clientSecretCredential, scopes: scopes);

            HttpClient httpClient = GetHttpClientWithDefaultAndCustomHandlers(enableHttpRequestHandler);

            var graphClient = new Microsoft.Graph.Beta.GraphServiceClient(httpClient, authProvider);

            return graphClient;
        }

        private static void SetupClientSecretCredentialAndScopes(out string[] scopes, out ClientSecretCredential clientSecretCredential)
        {
            // The client credentials flow requires that you request the
            // /.default scope, and preconfigure your permissions on the
            // app registration in Azure. An administrator must grant consent
            // to those permissions beforehand.
            scopes = new[] { "https://graph.microsoft.com/.default" };

            // Multi-tenant apps can use "common",
            // single-tenant apps must use the tenant ID from the Azure portal
            var tenantId = TenantId;

            // Values from app registration
            var clientId = ClientId;
            var clientSecret = ClientSecret;

            // using Azure.Identity;
            var options = new TokenCredentialOptions
            {
                AuthorityHost = AzureAuthorityHosts.AzurePublicCloud
            };

            // https://learn.microsoft.com/dotnet/api/azure.identity.clientsecretcredential
            clientSecretCredential = new ClientSecretCredential(
                tenantId, clientId, clientSecret, options);
        }

        private static HttpClient GetHttpClientWithDefaultAndCustomHandlers(bool enableHttpRequestHandler)
        {
            var handlers = GraphClientFactory.CreateDefaultHandlers();

            // Sample code to remove a default handler
            ////var compressionHandler = handlers.Where(h => h is CompressionHandler).FirstOrDefault();
            ////handlers.Remove(compressionHandler);

            if (enableHttpRequestHandler)
            {
                // Add logging handler before the compression handler. Compression handler is created by CreateDefaultHandlers()
                HttpRequestMessageLoggingHandler loggingHandler = new();
                handlers.Insert(0, loggingHandler);
            }

            // Add a new one
            // ChaosHandler simulates random server failures
            ////handlers.Add(new ChaosHandler());

            var httpClient = GraphClientFactory.Create(handlers);
            return httpClient;
        }
    }
}
