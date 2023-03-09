using Core.Graph;
using Core.Helpers;
using Microsoft.Graph;

const string groupName = "AAATest";
const string applicationName = "AAATestApplication";
bool writeJsonObjectsToOutput = false;

Console.WriteLine("Hello Microsoft Graph demo!\n");

GraphServiceClient graphClient = await GraphClientHelper.InitializeGraphClientWithMsiAsync();

// Users
await Users.DisplayLoggedInUserInfoAsync(graphClient, writeJsonObjectsToOutput);

// Display number of users
await Users.DisplayNumberOfUsersAsync(graphClient);

// Display all users with page iterator
await Users.DisplayUsersAsync(graphClient, writeJsonObjectsToOutput);

// Show random user (maybe doesn't exist) to test retry functionality.
////await Users.DisplayUserInfoAsync(graphClient, Guid.NewGuid().ToString(), writeJsonObjectsToOutput);

WriteSectionDevider();

// Groups
await Groups.ListGroupsAsync(graphClient, writeJsonObjectsToOutput);
////Group group = await Groups.GetOrCreateGroupIfNotExistAsync(graphClient, groupName);
////await Groups.DisplayGroupAsync(graphClient, group, writeJsonObjectsToOutput);
////await Groups.AddGroupMemberAsync(graphClient, group, "1dbbdd07-9978-489f-b676-6c084a890b49");
////await Groups.AddGroupOwnerAsync(graphClient, group, "bf41f70e-be3c-473a-b594-1e7c57b28da4");
////await Groups.ListGroupMembersAsync(graphClient, group, writeJsonObjectsToOutput);
////await Groups.ListGroupOwnersAsync(graphClient, group, writeJsonObjectsToOutput);
////await Groups.DeleteGroupAsync(graphClient, group);

WriteSectionDevider();

// Applications
await Applications.ListApplicationsAsync(graphClient, writeJsonObjectsToOutput);
////Application application = await Applications.GetOrCreateApplicationIfNotExistAsync(graphClient, applicationName);
////await Applications.DisplayApplicationAsync(graphClient, application, writeJsonObjectsToOutput);
////await Applications.AddApplicationOwnerAsync(graphClient, application, "d7fa49d4-38d8-427b-9199-193a5e0923f4");
////await Applications.ListApplicationOwnersAsync(graphClient, application, writeJsonObjectsToOutput);
////await Applications.DeleteApplicationAsync(graphClient, application);

WriteSectionDevider();

Console.WriteLine("Bye!!!\n");

static void WriteSectionDevider()
{
    Console.WriteLine("\n===============================================================================================\n");
    Console.WriteLine("Press any key to continue...\n\n");
    Console.ReadKey();
}