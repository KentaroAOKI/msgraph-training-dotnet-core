using System;
using Microsoft.Extensions.Configuration;

namespace GroupsAndUsers
{
    class Program
    {
        static IConfigurationRoot LoadAppSettings()
        {
            IConfigurationRoot result = null;
            var appConfig = new ConfigurationBuilder()
                .AddUserSecrets<Program>()
                .Build();
            if (string.IsNullOrEmpty(appConfig["appId"]) == false &&
                string.IsNullOrEmpty(appConfig["scopes"]) == false)
            {
                result = appConfig;
            }
            return result;
        }

        static void ListGroups()
        {
            var groups = GraphHelper.GetGroupsAsync().Result;

            Console.WriteLine("Groups:");

            foreach (var group in groups)
            {
                Console.WriteLine($"+Id: {group.Id}");
                Console.WriteLine($"  DisplayName: {group.DisplayName}");
                var directoryObjects = GraphHelper.GetGroupMembersAsync(group.Id).Result;
                foreach (Microsoft.Graph.User directoryObject in directoryObjects)
                {
                    Console.WriteLine($"   +Id: {directoryObject.Id}");
                    Console.WriteLine($"    DisplayName: {directoryObject.DisplayName}");
                    Console.WriteLine($"    Mail: {directoryObject.Mail}");
                }
            }
        }

        static void Main(string[] args)
        {
            var appConfig = LoadAppSettings();
            if (appConfig == null)
            {
                Console.WriteLine("Missing or invalid appsettings.json");
                return;
            }
            var appId = appConfig["appId"];
            var scopesString = appConfig["scopes"];
            var scopes = scopesString.Split(';');
            // Initialize the auth provider with values from appsettings.json
            var authProvider = new DeviceCodeAuthProvider(appId, scopes);
            // Request a token to sign in the user
            var accessToken = authProvider.GetAccessToken().Result;
            // Initialize Graph client
            GraphHelper.Initialize(authProvider);
            // Get signed in user
            var user = GraphHelper.GetMeAsync().Result;
            Console.WriteLine($"Welcome {user.DisplayName}!\n");
            // Get groups and users
            ListGroups();
        }
    }
}




