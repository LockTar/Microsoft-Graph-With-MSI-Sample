using Core.Helpers;
using Microsoft.Graph;

namespace Core.Graph
{
    public static class OrganizationBranding
    {
        public static async Task DisplayBrandingAsync(GraphServiceClient graphClient, bool writeJsonObjectsToOutput = true)
        {
            var result = await graphClient.Organization["f6f46aaf-ecdb-47b2-a816-cdc942b1b411"]
                .Branding
                .GetAsync(requestConfig =>
                {
                    //requestConfig.QueryParameters.Select = new string[] { "branding" };
                    //requestConfig.QueryParameters.Expand = new string[] { "branding" };
                });

            if (writeJsonObjectsToOutput)
            {
                Console.WriteLine();
                Console.WriteLine("Branding in JSON:");
                Console.WriteLine(result.ToFormattedJson());
            }
        }
    }
}
