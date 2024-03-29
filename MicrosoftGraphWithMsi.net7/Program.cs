﻿using Core.Graph;
using Core.Helpers;
using Microsoft.Graph;
using Microsoft.Graph.Models;

const string groupName = "AAATest";
const string applicationName = "AAATestApplication";
const string TestOrganizationId = "f6f46aaf-ecdb-47b2-a816-cdc942b1b411";

bool writeJsonObjectsToOutput = false;
bool enableHttpRequestLogger = false;

Console.WriteLine("Hello Microsoft Graph demo!\n");

var graphClient = await GraphClientHelper.InitializeGraphClientWithMsiAsync(enableHttpRequestLogger);
//var graphClient = await GraphClientHelper.InitializeGraphClientWithClientCredentialsAsync(enableHttpRequestLogger);
//var graphClientBeta = await GraphClientHelper.InitializeGraphBetaClientWithClientCredentialsAsync(enableHttpRequestLogger);

// Users
await Users.DisplayLoggedInUserInfoAsync(graphClient, writeJsonObjectsToOutput);

// Display number of users in tenant
await Users.DisplayNumberOfUsersAsync(graphClient);

// Display all users with page iterator
////await Users.DisplayUsersAsync(graphClient, writeJsonObjectsToOutput);

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
////await Applications.AddApplicationOwnerAsync(graphClient, application, "bf41f70e-be3c-473a-b594-1e7c57b28da4"");
////await Applications.ListApplicationOwnersAsync(graphClient, application, writeJsonObjectsToOutput);
////await Applications.DeleteApplicationAsync(graphClient, application);

////WriteSectionDevider();

// Organization branding
////await OrganizationBranding.DisplayBrandingAsync(graphClient, TestOrganizationId, writeJsonObjectsToOutput);
////await OrganizationBranding.UpdateBrandingAsync(graphClient, TestOrganizationId, writeJsonObjectsToOutput);

WriteSectionDevider();

Console.WriteLine("Bye!!!\n");

static void WriteSectionDevider()
{
    Console.WriteLine("\n===============================================================================================\n");
    Console.WriteLine("Press any key to continue...\n\n");
    Console.ReadKey();
}