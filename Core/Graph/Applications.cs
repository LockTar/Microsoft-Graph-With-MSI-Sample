using Microsoft.Graph;
using Core.Helpers;
using Microsoft.Graph.Models;
using Microsoft.Graph.Models.ODataErrors;

namespace Core.Graph
{
    public static class Applications
    {
        public static async Task<Application> DisplayApplicationAsync(GraphServiceClient graphClient, Application application, bool writeJsonObjectsToOutput = true)
        {
            application = await graphClient.Applications[application.Id].GetAsync();

            if (writeJsonObjectsToOutput)
            {
                Console.WriteLine();
                Console.WriteLine("Application in JSON:");
                Console.WriteLine(application.ToFormattedJson());
            }

            return application;
        }

        public static async Task ListApplicationsAsync(GraphServiceClient graphClient, bool writeJsonObjectsToOutput)
        {
            var applicationCollectionResponse = await graphClient.Applications.GetAsync();

            Console.WriteLine();
            Console.WriteLine($"Number of applications on first page {applicationCollectionResponse.Value.Count}");

            if (writeJsonObjectsToOutput)
            {
                Console.WriteLine();
                Console.WriteLine("Applications (first page) in JSON:");
                Console.WriteLine(applicationCollectionResponse.Value.Select(s => new { s.DisplayName, s.AppId, s.Id }).ToFormattedJson());
            }
        }

        public static async Task<Application> GetOrCreateApplicationIfNotExistAsync(GraphServiceClient graphClient, string applicationName)
        {
            Application? application = null;

            var applicationCollectionResponse = await graphClient.Applications
                            .GetAsync(requestConfig =>
                            {
                                requestConfig.QueryParameters.Select = new string[] { "id", "displayName" };
                                requestConfig.QueryParameters.Filter = $"displayName eq '{applicationName}'";
                            });

            if (applicationCollectionResponse.Value.Count > 1)
            {
                throw new InvalidProgramException("Multiple applications with same name!!!!");
            }
            else if (applicationCollectionResponse.Value.Count == 0)
            {
                Application newApplication = new()
                {
                    DisplayName = applicationName,
                    Description = "This is a test application"
                };

                // Application doesn't exist. Create it.
                Console.WriteLine($"Create application '{applicationName}'");
                application = await graphClient.Applications.PostAsync(newApplication);
                Console.WriteLine($"Created application '{application.DisplayName}' with id {application.Id}");
            }
            else
            {
                // Application already exist.
                application = applicationCollectionResponse.Value.First();
                Console.WriteLine($"Application '{application.DisplayName}' with id {application.Id} found in the AAD.");
            }

            Console.WriteLine("Application information:");
            Console.WriteLine($"Id: {application.Id}");
            Console.WriteLine($"Displayname: {application.DisplayName}");
            Console.WriteLine($"Description: {application.Description}");

            return application;
        }

        public static async Task DeleteApplicationAsync(GraphServiceClient graphClient, Application application)
        {
            Console.WriteLine($"\nGoing to delete application '{application.DisplayName}' with id {application.Id}");

            await graphClient.Applications[application.Id].DeleteAsync();

            Console.WriteLine($"\nApplication '{application.DisplayName}' deleted!");
        }

        public static async Task ListApplicationOwnersAsync(GraphServiceClient graphClient, Application application, bool writeJsonObjectsToOutput)
        {
            var result = await graphClient.Applications[application.Id]
                            .GetAsync(requestConfig =>
                            {
                                requestConfig.QueryParameters.Select = new string[] { "id", "displayName" };
                                requestConfig.QueryParameters.Expand = new string[] { "owners" };
                            });

            Console.WriteLine();
            Console.WriteLine($"Application owners (first page) in JSON ({result.Owners.Count})");

            if (writeJsonObjectsToOutput)
            {
                Console.WriteLine(result.Owners.ToFormattedJson());
            }
        }

        public static async Task AddApplicationOwnerAsync(GraphServiceClient graphClient, Application application, string ownerToAdd)
        {
            // Get user to add
            var user = await graphClient.Users[ownerToAdd]
                .GetAsync(requestConfig =>
                    requestConfig.QueryParameters.Select = new string[] { "id", "displayName" });

            try
            {
                var ownerOfApplication = await graphClient
                    .Applications[application.Id]
                    .Owners
                    .GetAsync(requestConfig =>
                    {
                        requestConfig.QueryParameters.Filter = $"id eq '{ownerToAdd}'";
                    });

                Console.WriteLine($"User {user.DisplayName} - {user.Id} already owner of application '{application.DisplayName}'");
            }
            catch (ODataError odataError) when (odataError.Error.Code.Equals("Request_ResourceNotFound"))
            {
                //Console.WriteLine(odataError.Error.Code);
                //Console.WriteLine(odataError.Error.Message);

                Console.WriteLine($"Add user {user.Id} as owner to application '{application.DisplayName}'");
                ReferenceCreate referenceCreate = new ReferenceCreate();
                referenceCreate.OdataId = "https://graph.microsoft.com/v1.0/directoryObjects/" + user.Id;

                await graphClient.Applications[application.Id]
                    .Owners
                    .Ref
                    .PostAsync(referenceCreate);

                Console.WriteLine($"User {user.Id} added as owner to application '{application.DisplayName}'");
            }
        }
    }
}
