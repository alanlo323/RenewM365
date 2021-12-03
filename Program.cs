using Microsoft.Graph;
using Microsoft.Graph.Auth;
using Microsoft.Identity.Client;
using System;
using System.Net;
using System.Security;
using System.Threading.Tasks;

namespace RenewM365
{
    class Program
    {
        static void Main(string[] args)
        {
            Task.Run(async () =>
            {
                bool success = false;
                do
                {
                    User user = await getUser();
                    if (user != null)
                    {
                        success = true;

                        DumpOptions dumpOptions = new DumpOptions
                        {
                            IgnoreDefaultValues = true
                        };
                        var userDump = ObjectDumper.Dump(user, dumpOptions);

                        Console.Clear();

                        Console.WriteLine();
                        Console.WriteLine($@"{userDump}");
                        Console.WriteLine($@"Call Graph API successfully, press any key to close this windows");
                        Console.ReadKey();
                    }
                    else
                    {
                        Console.WriteLine();
                        Console.WriteLine($@"Cannot get user info, press any key to retry");
                        Console.ReadKey();

                        Console.Clear();
                    }
                } while (!success);
            }).Wait();

            async static Task<User> getUser()
            {
                try
                {
                    // https://docs.microsoft.com/en-us/windows/communitytoolkit/services/graphlogin

                    //  Get Application (client) ID from Azure Portal - App registrations :
                    //  https://portal.azure.com/#blade/Microsoft_AAD_RegisteredApps/ApplicationsListBlade
                    Console.Write($@"Enter Application (client) ID:");
                    string clientId = Console.ReadLine();

                    //  Get TenantId inside application - Directory (tenant) ID
                    Console.Write($@"Enter Directory (tenant) ID:");
                    string tenantId = Console.ReadLine();

                    string[] scopes = new string[] { "user.read" };

                    IPublicClientApplication publicClientApplication = PublicClientApplicationBuilder
                                .Create(clientId)
                                .WithTenantId(tenantId)
                                .Build();
                    UsernamePasswordProvider authProvider = new UsernamePasswordProvider(publicClientApplication, scopes);
                    GraphServiceClient graphClient = new GraphServiceClient(authProvider);

                    Console.Write("Enter Email:");
                    string email = Console.ReadLine();

                    Console.Write("Enter Password:");
                    string password = Console.ReadLine();

                    Console.Clear();

                    Console.WriteLine("Getting user info...");

                    SecureString securedPassword = new NetworkCredential(email, password).SecurePassword;
                    User me = await graphClient.Me.Request()
                                    .WithUsernamePassword(email, securedPassword)
                                    .GetAsync();
                    return me;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                return null;
            }
        }
    }
}
