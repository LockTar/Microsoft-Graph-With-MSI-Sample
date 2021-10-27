using Microsoft.Graph;
using MicrosoftGraphWithMsi.Helpers;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MicrosoftGraphWithMsi.Graph
{
    internal static class Groups
    {
        internal static async Task<Group> DisplayGroupAsync(GraphServiceClient graphClient, Group group, bool writeJsonObjectsToOutput = true)
        {
            group = await graphClient.Groups[group.Id]
                            .Request()
                            .GetAsync();

            if (writeJsonObjectsToOutput)
            {
                Console.WriteLine();
                Console.WriteLine("Group in JSON:");
                Console.WriteLine(group.ToFormattedJson());
            }

            return group;
        }

        internal static async Task ListGroupsAsync(GraphServiceClient graphClient, bool writeJsonObjectsToOutput)
        {
            if (writeJsonObjectsToOutput)
            {
                var groups = await graphClient.Groups
                                .Request()
                                .GetAsync();

                Console.WriteLine();
                Console.WriteLine("Groups (first page) in JSON:");
                Console.WriteLine(groups.CurrentPage.ToFormattedJson());
            }
        }

        internal static async Task<Group> GetOrCreateGroupIfNotExistAsync(GraphServiceClient graphClient, string groupName)
        {
            Group group = null;

            var groupsCollectionPage = await graphClient.Groups
                            .Request()
                            .Filter($"displayName eq '{groupName}'")
                            .GetAsync();

            if (groupsCollectionPage.CurrentPage.Count > 1)
            {
                throw new InvalidProgramException("Multiple groups with same name!!!!");
            }
            else if (groupsCollectionPage.CurrentPage.Count == 0)
            {
                // Group doesn't exist. Create it.
                Console.WriteLine($"Create group '{groupName}'");
                group = await graphClient.Groups
                            .Request()
                            .AddAsync(new Group()
                            {
                                DisplayName = groupName,
                                Description = "This is a test group",
                                SecurityEnabled = true,
                                MailEnabled = false,
                                MailNickname = groupName
                            });
                Console.WriteLine($"Created group '{group.DisplayName}' with id {group.Id}");
            }
            else
            {
                // Group already exist.
                group = groupsCollectionPage.CurrentPage[0];
                Console.WriteLine($"Group '{group.DisplayName}' with id {group.Id} found in the AAD.");
            }

            Console.WriteLine("Group information:");
            Console.WriteLine($"Id: {group.Id}");
            Console.WriteLine($"Displayname: {group.DisplayName}");
            Console.WriteLine($"Description: {group.Description}");

            return group;
        }

        internal static async Task DeleteGroupAsync(GraphServiceClient graphClient, Group group)
        {
            Console.WriteLine($"\nGoing to delete group '{group.DisplayName}' with id {group.Id}");

            await graphClient.Groups[group.Id]
                            .Request()
                            .DeleteAsync();

            Console.WriteLine($"\nGroup '{group.DisplayName}' deleted!");
        }

        internal static async Task ListGroupOwnersAsync(GraphServiceClient graphClient, Group group, bool writeJsonObjectsToOutput)
        {
            if (writeJsonObjectsToOutput)
            {
                group = await graphClient.Groups[group.Id]
                                .Request()
                                .Expand("owners")
                                .GetAsync();

                Console.WriteLine();
                Console.WriteLine("Group owners (first page) in JSON:");
                Console.WriteLine(group.Owners.CurrentPage.ToFormattedJson());
            }
        }

        internal static async Task ListGroupMembersAsync(GraphServiceClient graphClient, Group group, bool writeJsonObjectsToOutput)
        {
            if (writeJsonObjectsToOutput)
            {
                group = await graphClient.Groups[group.Id]
                                .Request()
                                .Expand("members")
                                .GetAsync();

                Console.WriteLine();
                Console.WriteLine("Group members (first page) in JSON:");
                Console.WriteLine(group.Members.CurrentPage.ToFormattedJson());
            }
        }

        internal static async Task AddGroupOwnerAsync(GraphServiceClient graphClient, Group group, string ownerToAdd)
        {
            // Get user to add
            var user = await graphClient.Users[ownerToAdd]
             .Request()
               .Select("id")
                   .GetAsync();

            try
            {
                var ownerOfGroup = await graphClient
                    .Groups[group.Id]
                    .Owners[ownerToAdd]
                    .Request()
                    .GetAsync();

                Console.WriteLine($"User {user.Id} already owner of group '{group.DisplayName}'");
            }
            catch (ServiceException ex)
            {
                if (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    Console.WriteLine($"Add user {user.Id} as owner to group '{group.DisplayName}'");
                    await graphClient.Groups[group.Id]
                        .Owners
                        .References
                        .Request()
                        .AddAsync(user);

                    Console.WriteLine($"User {user.Id} added as owner to group '{group.DisplayName}'");
                }
            }
        }

        internal static async Task AddGroupMemberAsync(GraphServiceClient graphClient, Group group, string memberToAdd)
        {
            // Get user to add
            var user = await graphClient.Users[memberToAdd]
             .Request()
               .Select("id")
                   .GetAsync();

            try
            {
                var memberOfGroup = await graphClient
                    .Groups[group.Id]
                    .Members[memberToAdd]
                    .Request()
                    .GetAsync();

                Console.WriteLine($"User {user.Id} already member of group '{group.DisplayName}'");
            }
            catch (ServiceException ex)
            {
                if (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    Console.WriteLine($"Add user {user.Id} as member to group '{group.DisplayName}'");
                    await graphClient.Groups[group.Id]
                        .Members
                        .References
                        .Request()
                        .AddAsync(user);

                    Console.WriteLine($"User {user.Id} added as member to group '{group.DisplayName}'");
                }
            }
        }
    }
}
