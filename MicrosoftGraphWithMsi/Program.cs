﻿using System;
using System.Threading.Tasks;
using Microsoft.Graph;
using MicrosoftGraphWithMsi.Graph;
using MicrosoftGraphWithMsi.Helpers;

namespace MicrosoftGraphWithMsi
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            const string groupName = "AAATest";
            bool writeJsonObjectsToOutput = true;

            Console.WriteLine("Hello World!\n");

            GraphServiceClient graphClient = await GraphClientHelper.InitializeGraphClientWithMsiAsync();

            // User
            await Users.DisplayLoggedInUserInfoAsync(graphClient, writeJsonObjectsToOutput);

            WriteSectionDevider();

            // Groups
            Group group = await Groups.GetOrCreateGroupIfNotExistAsync(graphClient, groupName);
            await Groups.DisplayGroupAsync(graphClient, group, writeJsonObjectsToOutput);
            await Groups.AddGroupMemberAsync(graphClient, group, "1dbbdd07-9978-489f-b676-6c084a890b49");
            await Groups.AddGroupOwnerAsync(graphClient, group, "bf41f70e-be3c-473a-b594-1e7c57b28da4");
            await Groups.ListGroupMembersAsync(graphClient, group, writeJsonObjectsToOutput);
            await Groups.ListGroupOwnersAsync(graphClient, group, writeJsonObjectsToOutput);
            //await Groups.DeleteGroupAsync(graphClient, group);

            WriteSectionDevider();

            Console.WriteLine("Bye!!!\n");
        }        

        private static void WriteSectionDevider()
        {
            Console.WriteLine("\n===============================================================================================\n");
            Console.WriteLine("Press any key to continue...\n\n");
            Console.ReadKey();
        }
    }
}
