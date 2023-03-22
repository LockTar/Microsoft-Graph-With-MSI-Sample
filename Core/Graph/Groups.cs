using Core.Helpers;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Graph.Models.ODataErrors;
using System.Net;

namespace Core.Graph
{
    public static class Groups
    {
        public static async Task<Group> DisplayGroupAsync(GraphServiceClient graphClient, Group group, bool writeJsonObjectsToOutput = true)
        {
            group = await graphClient.Groups[group.Id].GetAsync();

            if (writeJsonObjectsToOutput)
            {
                Console.WriteLine();
                Console.WriteLine("Group in JSON:");
                Console.WriteLine(group.ToFormattedJson());
            }

            return group;
        }

        public static async Task ListGroupsAsync(GraphServiceClient graphClient, bool writeJsonObjectsToOutput)
        {
            var groupCollectionResponse = await graphClient.Groups.GetAsync();

            Console.WriteLine();
            Console.WriteLine($"Number of groups on first page are: {groupCollectionResponse.Value.Count}");

            if (writeJsonObjectsToOutput)
            {
                Console.WriteLine();
                Console.WriteLine("Groups (first page) in JSON:");
                Console.WriteLine(groupCollectionResponse.Value.Select(s => new { s.DisplayName, s.Id }).ToFormattedJson());
            }
        }

        public static async Task<Group> GetOrCreateGroupIfNotExistAsync(GraphServiceClient graphClient, string groupName)
        {
            Group? group = null;

            var groupCollectionResponse = await graphClient.Groups
                            .GetAsync(requestConfig =>
                            {
                                requestConfig.QueryParameters.Select = new string[] { "id", "displayName" };
                                requestConfig.QueryParameters.Filter = $"displayName eq '{groupName}'";
                            });

            if (groupCollectionResponse.Value.Count > 1)
            {
                throw new InvalidProgramException("Multiple groups with same name!!!!");
            }
            else if (groupCollectionResponse.Value.Count == 0)
            {
                var newGroup = new Group()
                {
                    DisplayName = groupName,
                    Description = "This is a test group",
                    MailEnabled = false,
                    MailNickname = groupName,
                    SecurityEnabled = true,
                };

                // Group doesn't exist. Create it.
                Console.WriteLine($"Create group '{groupName}'");
                group = await graphClient.Groups.PostAsync(newGroup);
                Console.WriteLine($"Created group '{group.DisplayName}' with id {group.Id}");
            }
            else
            {
                // Group already exist.
                group = groupCollectionResponse.Value.First();
                Console.WriteLine($"Group '{group.DisplayName}' with id {group.Id} found in the AAD.");
            }

            Console.WriteLine("Group information:");
            Console.WriteLine($"Id: {group.Id}");
            Console.WriteLine($"Displayname: {group.DisplayName}");
            Console.WriteLine($"Description: {group.Description}");

            return group;
        }

        public static async Task DeleteGroupAsync(GraphServiceClient graphClient, Group group)
        {
            Console.WriteLine($"\nGoing to delete group '{group.DisplayName}' with id {group.Id}");

            await graphClient.Groups[group.Id].DeleteAsync();

            Console.WriteLine($"\nGroup '{group.DisplayName}' deleted!");
        }

        public static async Task ListGroupOwnersAsync(GraphServiceClient graphClient, Group group, bool writeJsonObjectsToOutput)
        {
            var result = await graphClient.Groups[group.Id]
                            .GetAsync(requestConfig =>
                            {
                                requestConfig.QueryParameters.Select = new string[] { "id", "displayName" };
                                requestConfig.QueryParameters.Expand = new string[] { "owners" };
                            });

            Console.WriteLine();
            Console.WriteLine($"Group owners (first page) in JSON ({result.Owners.Count})");

            if (writeJsonObjectsToOutput)
            {
                Console.WriteLine(result.Owners.ToFormattedJson());
            }
        }

        public static async Task ListGroupMembersAsync(GraphServiceClient graphClient, Group group, bool writeJsonObjectsToOutput)
        {
            var result = await graphClient.Groups[group.Id]
                            .GetAsync(requestConfig =>
                                requestConfig.QueryParameters.Expand = new string[] { "members" });

            Console.WriteLine();
            Console.WriteLine($"Group members (first page) in JSON ({result.Members.Count})");

            if (writeJsonObjectsToOutput)
            {
                Console.WriteLine(result.Members.ToFormattedJson());
            }
        }

        public static async Task AddGroupOwnerAsync(GraphServiceClient graphClient, Group group, string ownerToAdd)
        {
            // Get user to add
            var user = await graphClient.Users[ownerToAdd]
                .GetAsync(requestConfig =>
                    requestConfig.QueryParameters.Select = new string[] { "id", "displayName" });

            try
            {
                var ownerOfGroup = await graphClient
                    .Groups[group.Id]
                    .Owners
                    .GetAsync(requestConfig =>
                    {
                        requestConfig.QueryParameters.Filter = $"id eq '{ownerToAdd}'";
                    });

                Console.WriteLine($"User {user.DisplayName} - {user.Id} already owner of group '{group.DisplayName}'");
            }
            catch (ODataError odataError) when (odataError.ResponseStatusCode.Equals(HttpStatusCode.NotFound))
            {
                //Console.WriteLine(odataError.Error.Code);
                //Console.WriteLine(odataError.Error.Message);

                Console.WriteLine($"Add user {user.Id} as owner to group '{group.DisplayName}'");               

                await graphClient.Groups[group.Id]
                    .Owners
                    .Ref
                    .PostAsync(
                        new ReferenceCreate
                        {
                            OdataId = "https://graph.microsoft.com/v1.0/directoryObjects/" + user.Id
                        });

                Console.WriteLine($"User {user.Id} added as owner to group '{group.DisplayName}'");
            }
        }

        public static async Task AddGroupMemberAsync(GraphServiceClient graphClient, Group group, string memberToAdd)
        {
            // Get user to add
            var user = await graphClient.Users[memberToAdd]
                .GetAsync(requestConfig =>
                    requestConfig.QueryParameters.Select = new string[] { "id", "displayName" });

            try
            {
                var memberOfGroup = await graphClient
                    .Groups[group.Id]
                    .Members
                    .GetAsync(requestConfig =>
                    {
                        requestConfig.QueryParameters.Filter = $"id eq '{memberToAdd}'";
                    });

                Console.WriteLine($"User {user.Id} already member of group '{group.DisplayName}'");
            }
            catch (ODataError odataError) when (odataError.ResponseStatusCode.Equals(HttpStatusCode.NotFound))
            {
                //Console.WriteLine(odataError.Error.Code);
                //Console.WriteLine(odataError.Error.Message);

                Console.WriteLine($"Add user {user.Id} as member to group '{group.DisplayName}'");

                await graphClient.Groups[group.Id]
                    .Members
                    .Ref
                    .PostAsync(
                        new ReferenceCreate
                        {
                            OdataId = "https://graph.microsoft.com/v1.0/directoryObjects/" + user.Id
                        });

                Console.WriteLine($"User {user.Id} added as member to group '{group.DisplayName}'");
            }
        }
    }
}
