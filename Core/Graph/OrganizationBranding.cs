using Core.Helpers;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Graph.Models.ODataErrors;

namespace Core.Graph
{
    public static class OrganizationBranding
    {
        public static async Task DisplayBrandingAsync(GraphServiceClient graphClient, bool writeJsonObjectsToOutput = true)
        {
            var result = await graphClient.Organization
                .GetAsync(requestConfig =>
                    requestConfig.QueryParameters.Expand = new string[] { "branding" });

            if (writeJsonObjectsToOutput)
            {
                Console.WriteLine();
                Console.WriteLine("Branding in JSON:");
                Console.WriteLine(result.ToFormattedJson());
            }
        }
    }
}
