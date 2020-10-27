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
                string clientId = "0aa2670e-5b9d-4bb9-931a-7299bb0913c7";
                string tenantID = "20d0c53a-82ae-4d18-97e7-459eaa05756a";
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