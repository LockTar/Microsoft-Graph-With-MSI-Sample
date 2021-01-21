using Microsoft.Graph;
using MicrosoftGraphWithMsi.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MicrosoftGraphWithMsi.Graph
{
    internal static class Users
    {
        internal static async Task<User> GetLoggedInUser(GraphServiceClient graphClient)
        {
            return await graphClient.Me
                            .Request()
                            .GetAsync();
        }

        internal static async Task DisplayLoggedInUserInfoAsync(GraphServiceClient graphClient, bool writeJsonObjectsToOutput = true)
        {
            User me = await GetLoggedInUser(graphClient);

            Console.WriteLine("Logged in user:");
            Console.WriteLine($"Displayname: {me.DisplayName}");

            if (writeJsonObjectsToOutput)
            {
                Console.WriteLine();
                Console.WriteLine("User in JSON:");
                string json = me.ToFormattedJson();
                Console.WriteLine(json);
            }
        }
    }
}
