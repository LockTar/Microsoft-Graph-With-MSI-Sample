# Microsoft-Graph-With-MSI-Sample

Sample application that uses the Microsoft Graph C# sdk for getting and writing information to the Microsoft Graph like users, applications and groups.

## Getting started

1. Open the solution in Visual Studio (sample created with VS2022)
2. Open in Visual Studio: tools -> options -> Azure Service Authentication -> Account Selection. Select the correct account as which user you want to login.
3. Run the application

## FAQ

### I get forbidden error
Make sure your user has the correct permissions like Directory.ReadWrite.All.
See for more information on how to give the MSI of your app service the correct permissions this [blog post](https://www.rahulpnath.com/blog/how-to-authenticate-with-microsoft-graph-api-using-managed-service-identity/#using-managed-service-identity).
