using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;

namespace GroupsAndUsers
{
    class Program
    {
        static Dictionary<string, string> LoadDeviceCodeAppSettings()
        {
            Dictionary<string, string> result = null;
            do {
                // Get config ftom Environment variable
                // ex. appId="xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx"
                // ex. scopes="Directory.Read.All;User.Read"
                string appId = Environment.GetEnvironmentVariable("appId");
                string scopes = Environment.GetEnvironmentVariable("scopes");
                if (string.IsNullOrEmpty(appId) == false &&
                    string.IsNullOrEmpty(scopes) == false)
                {
                    result = new Dictionary<string, string>()
                    {
                        {"appId", appId},
                        {"scopes", scopes}
                    };
                    break;
                } 
                // Get config ftom appsettings.json
                var appConfig = new ConfigurationBuilder()
                    .AddUserSecrets<Program>()
                    .Build();
                appId = appConfig["appId"];
                scopes = appConfig["scopes"];               
                if (string.IsNullOrEmpty(appId) == false &&
                    string.IsNullOrEmpty(scopes) == false)
                {
                    result = new Dictionary<string, string>()
                    {
                        {"appId", appId},
                        {"scopes", scopes}
                    };
                    break;
                }
            } while (false);
            return result;
        }

        static Dictionary<string, string> LoadClientSecretAppSettings()
        {
            Dictionary<string, string> result = null;
            do {
                // Get config ftom Environment variable
                // ex. appId="xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx"
                // ex. scopes="https://graph.microsoft.com/.default"
                // ex. tenantId="xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx"
                // ex. clientSecret="xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx"
                string appId = Environment.GetEnvironmentVariable("appId");
                string scopes = Environment.GetEnvironmentVariable("scopes");
                string tenantId = Environment.GetEnvironmentVariable("tenantId");
                string clientSecret = Environment.GetEnvironmentVariable("clientSecret");
                if (string.IsNullOrEmpty(appId) == false &&
                    string.IsNullOrEmpty(scopes) == false &&
                    string.IsNullOrEmpty(tenantId) == false &&
                    string.IsNullOrEmpty(clientSecret) == false)
                {
                    result = new Dictionary<string, string>()
                    {
                        {"appId", appId},
                        {"scopes", scopes},
                        {"tenantId", tenantId},
                        {"clientSecret", clientSecret}
                    };
                    break;
                } 
                // Get config ftom appsettings.json
                var appConfig = new ConfigurationBuilder()
                    .AddUserSecrets<Program>()
                    .Build();
                appId = appConfig["appId"];
                scopes = appConfig["scopes"];
                tenantId = appConfig["tenantId"];
                clientSecret = appConfig["clientSecret"];
                if (string.IsNullOrEmpty(appId) == false &&
                    string.IsNullOrEmpty(scopes) == false &&
                    string.IsNullOrEmpty(tenantId) == false &&
                    string.IsNullOrEmpty(clientSecret) == false)
                {
                    result = new Dictionary<string, string>()
                    {
                        {"appId", appId},
                        {"scopes", scopes},
                        {"tenantId", tenantId},
                        {"clientSecret", clientSecret}
                    };
                    break;
                }
            } while (false);
            return result;
        }

        static void ListGroups()
        {
            var groups = GraphHelper.GetGroupsAsync().Result;

            Console.WriteLine("Groups:");

            foreach (var group in groups)
            {
                Console.WriteLine($"+Id: {group.Id}");
                Console.WriteLine($"  displayName: {group.DisplayName}");
                var directoryObjects = GraphHelper.GetGroupMembersAsync(group.Id).Result;
                foreach (Microsoft.Graph.User directoryObject in directoryObjects)
                {
                    Console.WriteLine($"   +Id: {directoryObject.Id}");
                    Console.WriteLine($"    displayName: {directoryObject.DisplayName}");
                    Console.WriteLine($"    userPrincipalName: {directoryObject.UserPrincipalName}");
                }
            }
        }

        static void Main(string[] args)
        {
            bool devcode = true;
            IAuthenticationProvider authProvider = null;

            if (devcode == true)
            {
                // Device Code
                var appConfig = LoadDeviceCodeAppSettings();
                if (appConfig == null)
                {
                    Console.WriteLine("Missing or invalid appsettings.json");
                    return;
                }
                var appId = appConfig["appId"];
                var scopesString = appConfig["scopes"];
                var scopes = scopesString.Split(';');
                // Initialize the auth provider with values from appsettings.json
                authProvider = new DeviceCodeAuthProvider(appId, scopes);
            } else {
                // Client Secret
                var appConfig = LoadClientSecretAppSettings();
                if (appConfig == null)
                {
                    Console.WriteLine("Missing or invalid appsettings.json");
                    return;
                }
                var appId = appConfig["appId"];
                var scopesString = appConfig["scopes"];
                var scopes = scopesString.Split(';');
                var tenantId = appConfig["tenantId"];
                var clientSecret = appConfig["clientSecret"];
                // Initialize the auth provider with values from appsettings.json
                authProvider = new ClientSecretAuthProvider(appId, scopes, tenantId, clientSecret);
            }
            // Initialize Graph client
            GraphHelper.Initialize(authProvider);
            // Get groups and users
            ListGroups();
        }
    }
}
