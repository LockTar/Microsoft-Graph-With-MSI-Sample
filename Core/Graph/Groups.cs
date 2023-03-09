using Core.Helpers;
using Microsoft.Graph;
using Microsoft.Graph.Models;

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
                                requestConfig.QueryParameters.Orderby = new string[] { "displayName" };
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
                    SecurityEnabled = true,
                    MailEnabled = false,
                    MailNickname = groupName
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
            if (writeJsonObjectsToOutput)
            {
                group = await graphClient.Groups[group.Id]
                                .GetAsync(requestConfig =>
                                    requestConfig.QueryParameters.Expand = new string[] { "owners" });

                Console.WriteLine();
                Console.WriteLine("Group owners (first page) in JSON:");
                Console.WriteLine(group.Owners.ToFormattedJson());
            }
        }

        public static async Task ListGroupMembersAsync(GraphServiceClient graphClient, Group group, bool writeJsonObjectsToOutput)
        {
            if (writeJsonObjectsToOutput)
            {
                group = await graphClient.Groups[group.Id]
                                .GetAsync(requestConfig =>
                                    requestConfig.QueryParameters.Expand = new string[] { "members" });

                Console.WriteLine();
                Console.WriteLine("Group members (first page) in JSON:");
                Console.WriteLine(group.Members.ToFormattedJson());
            }
        }

        public static async Task AddGroupOwnerAsync(GraphServiceClient graphClient, Group group, string ownerToAdd)
        {
            // Get user to add
            var user = await graphClient.Users[ownerToAdd]
                .GetAsync(requestConfig =>
                    requestConfig.QueryParameters.Select = new string[] { "id", "displayName" });

            var memberOf = await graphClient
                .Users[ownerToAdd]
                .MemberOf
                .GetAsync(c =>
                {
                    c.QueryParameters.Filter = $"id eq '{group.Id}'";
                });

            //var ownerOfGroup = await graphClient
            //    .Groups[group.Id]
            //    .Owners
            //    .GetAsync(requestConfig =>
            //    {
            //        //requestConfig.QueryParameters.Expand = new string[] { "owners" };
            //        requestConfig.QueryParameters.Filter = $"id eq '{ownerToAdd}'";
            //        requestConfig.QueryParameters.
            //        //requestConfig.QueryParameters.Orderby = new string[] { "displayName" };
            //    });

            if (memberOf == null)
            {
                Console.WriteLine($"Add user {user.Id} as owner to group '{group.DisplayName}'");
                ReferenceCreate referenceCreate = new ReferenceCreate();
                referenceCreate.OdataId = user.Id;

                await graphClient.Groups[group.Id]
                    .Owners
                    .Ref
                    .PostAsync(referenceCreate);

                Console.WriteLine($"User {user.Id} added as owner to group '{group.DisplayName}'");
            }
            else
            {
                Console.WriteLine($"User {user.DisplayName} - {user.Id} already owner of group '{group.DisplayName}'");
            }
        }

        //public static async Task AddGroupMemberAsync(GraphServiceClient graphClient, Group group, string memberToAdd)
        //{
        //    // Get user to add
        //    var user = await graphClient.Users[memberToAdd]
        //        .GetAsync(requestConfig =>
        //            requestConfig.QueryParameters.Select = new string[] { "id", "displayName" });

        //    try
        //    {
        //        var memberOfGroup = await graphClient
        //            .Groups[group.Id]
        //            .Members[memberToAdd]
        //            .GetAsync();

        //        Console.WriteLine($"User {user.Id} already member of group '{group.DisplayName}'");
        //    }
        //    catch (ServiceException ex)
        //    {
        //        if (ex.StatusCode == HttpStatusCode.NotFound)
        //        {
        //            Console.WriteLine($"Add user {user.Id} as member to group '{group.DisplayName}'");
        //            await graphClient.Groups[group.Id]
        //                .Members
        //                .References
        //                .Request()
        //                .AddAsync(user);

        //            Console.WriteLine($"User {user.Id} added as member to group '{group.DisplayName}'");
        //        }
        //    }
        //}
    }
}
