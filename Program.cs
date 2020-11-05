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
                User user = await getUser();
                DumpOptions dumpOptions = new DumpOptions
                {
                    IgnoreDefaultValues = true
                };
                var userDump = ObjectDumper.Dump(user, dumpOptions);
                Console.WriteLine($@"{userDump}");
                Console.ReadKey();
            }).Wait();

            async static Task<User> getUser()
            {
                // https://docs.microsoft.com/en-us/windows/communitytoolkit/services/graphlogin
                string clientId = "your-clientId";
                string tenantID = "your-tenatId";
                string[] scopes = new string[] { "user.read" };

                IPublicClientApplication publicClientApplication = PublicClientApplicationBuilder
                            .Create(clientId)
                            .WithTenantId(tenantID)
                            .Build();
                UsernamePasswordProvider authProvider = new UsernamePasswordProvider(publicClientApplication, scopes);

                GraphServiceClient graphClient = new GraphServiceClient(authProvider);
                Console.Write("Email:");
                string email = Console.ReadLine();
                Console.Write("Password:");
                string password = Console.ReadLine();
                Console.Write("Getting user info...");
                SecureString securedPassword = new NetworkCredential(email, password).SecurePassword;
                User me = await graphClient.Me.Request()
                                .WithUsernamePassword(email, securedPassword)
                                .GetAsync();
                return me;
            }
        }
    }
}
