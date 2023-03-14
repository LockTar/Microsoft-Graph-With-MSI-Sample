using Core.Helpers;
using Microsoft.Graph;
using Microsoft.Graph.Models;

namespace Core.Graph
{
    public static class OrganizationBranding
    {
        public static async Task DisplayBrandingAsync(GraphServiceClient graphClient, string organizationId, bool writeJsonObjectsToOutput = true)
        {
            var result = await graphClient.Organization[organizationId]
                .Branding
                .GetAsync(requestConfig =>
                {
                    requestConfig.QueryParameters.Select = new string[] { "BackgroundColor" }; // doesn't work (yet)???
                });

            if (writeJsonObjectsToOutput)
            {
                Console.WriteLine();
                Console.WriteLine("Branding in JSON:");
                Console.WriteLine(result.ToFormattedJson());
            }
        }

        public static async Task UpdateBrandingAsync(GraphServiceClient graphClient, string organizationId, bool writeJsonObjectsToOutput = true)
        {
            var branding = new OrganizationalBranding();
            var seconds = DateTime.UtcNow.Second;

            if (seconds >= 0 && seconds <= 30)
            {
                branding.BackgroundColor = "#99CCFF";
            }
            else
            {
                branding.BackgroundColor = "#2969E8";
            }

            await graphClient.Organization[organizationId]
                .Branding
                .PatchAsync(branding);

            if (writeJsonObjectsToOutput)
            {
                Console.WriteLine();
                Console.WriteLine("Changed branding to:");
                await DisplayBrandingAsync(graphClient, organizationId, writeJsonObjectsToOutput);
            }
        }
    }
}
