using Azure.Identity;
using Microsoft.Kiota.Abstractions.Authentication;

namespace Core.Helpers
{
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
