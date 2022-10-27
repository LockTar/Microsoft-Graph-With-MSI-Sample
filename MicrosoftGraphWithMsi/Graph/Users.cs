using Microsoft.Graph;
using MicrosoftGraphWithMsi.Helpers;
using System;
using System.Threading.Tasks;

namespace MicrosoftGraphWithMsi.Graph
{
    internal static class Users
    {
        internal static async Task DisplayLoggedInUserInfoAsync(GraphServiceClient graphClient, bool writeJsonObjectsToOutput = true)
        {
            User user = await graphClient.Me
                            .Request()
                            .GetAsync();

            Console.WriteLine("Logged in user:");
            PrintUserInformation(user, writeJsonObjectsToOutput);
        }

        internal static async Task DisplayUserInfoAsync(GraphServiceClient graphClient, string userId, bool writeJsonObjectsToOutput = true)
        {
            User user = await graphClient.Users[userId]
                            .Request()
                            .GetAsync();

            Console.WriteLine("User information:");
            PrintUserInformation(user, writeJsonObjectsToOutput);
        }
     
        private static void PrintUserInformation(User user, bool writeJsonObjectsToOutput)
        {
            Console.WriteLine($"Displayname: {user.DisplayName}");

            if (writeJsonObjectsToOutput)
            {
                Console.WriteLine();
                Console.WriteLine("User in JSON:");
                string json = user.ToFormattedJson();
                Console.WriteLine(json);
            }
        }
    }
}
