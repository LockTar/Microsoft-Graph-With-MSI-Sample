using Microsoft.Graph;
using Core.Helpers;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Core.Graph
{
    public static class Applications
    {
        public static async Task<Application> DisplayApplicationAsync(GraphServiceClient graphClient, Application application, bool writeJsonObjectsToOutput = true)
        {
            application = await graphClient.Applications[application.Id]
                            .Request()
                            .GetAsync();

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
            var applications = await graphClient.Applications
                            .Request()
                            .GetAsync();

            Console.WriteLine();
            Console.WriteLine($"Number of applications on first page {applications.Count}");

            if (writeJsonObjectsToOutput)
            {
                Console.WriteLine();
                Console.WriteLine("Applications (first page) in JSON:");
                Console.WriteLine(applications.CurrentPage.Select(s => new { s.DisplayName, s.AppId, s.Id }).ToFormattedJson());
            }
        }

        public static async Task<Application> GetOrCreateApplicationIfNotExistAsync(GraphServiceClient graphClient, string applicationName)
        {
            Application application = null;

            var applicationsCollectionPage = await graphClient.Applications
                            .Request()
                            .Filter($"displayName eq '{applicationName}'")
                            .GetAsync();

            if (applicationsCollectionPage.CurrentPage.Count > 1)
            {
                throw new InvalidProgramException("Multiple applications with same name!!!!");
            }
            else if (applicationsCollectionPage.CurrentPage.Count == 0)
            {
                // Application doesn't exist. Create it.
                Console.WriteLine($"Create application '{applicationName}'");
                application = await graphClient.Applications
                            .Request()
                            .AddAsync(new Application()
                            {
                                DisplayName = applicationName,
                                Description = "This is a test application"
                            });
                Console.WriteLine($"Created application '{application.DisplayName}' with id {application.Id}");
            }
            else
            {
                // Application already exist.
                application = applicationsCollectionPage.CurrentPage[0];
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

            await graphClient.Applications[application.Id]
                            .Request()
                            .DeleteAsync();

            Console.WriteLine($"\nApplication '{application.DisplayName}' deleted!");
        }

        public static async Task ListApplicationOwnersAsync(GraphServiceClient graphClient, Application application, bool writeJsonObjectsToOutput)
        {
            if (writeJsonObjectsToOutput)
            {
                application = await graphClient.Applications[application.Id]
                                .Request()
                                .Expand("owners")
                                .GetAsync();

                Console.WriteLine();
                Console.WriteLine("Application owners (first page) in JSON:");
                Console.WriteLine(application.Owners.CurrentPage.ToFormattedJson());
            }
        }

        public static async Task AddApplicationOwnerAsync(GraphServiceClient graphClient, Application application, string ownerToAdd)
        {
            // Get user to add
            var user = await graphClient.Users[ownerToAdd]
             .Request()
               .Select("id")
                   .GetAsync();

            try
            {
                var ownerOfApplication = await graphClient
                    .Applications[application.Id]
                    .Owners[ownerToAdd]
                    .Request()
                    .GetAsync();

                Console.WriteLine($"User {user.Id} already owner of application '{application.DisplayName}'");
            }
            catch (ServiceException ex)
            {
                if (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    Console.WriteLine($"Add user {user.Id} as owner to application '{application.DisplayName}'");
                    await graphClient.Applications[application.Id]
                        .Owners
                        .References
                        .Request()
                        .AddAsync(user);

                    Console.WriteLine($"User {user.Id} added as owner to application '{application.DisplayName}'");
                }
            }
        }
    }
}
