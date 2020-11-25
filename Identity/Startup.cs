using System.Linq;
using System.Reflection;
using Identity.Data;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Identity
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            const string connectionString = "xxx";
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            services.AddIdentityServer()
                //.AddInMemoryClients(Clients.Get())
                //.AddInMemoryIdentityResources(Data.Resources.GetIdentityResources())
                //.AddInMemoryApiResources(Data.Resources.GetApiResources())
                //.AddInMemoryApiScopes(Data.Resources.GetApiScopes())
                .AddTestUsers(Users.Get())
                /*When you go to create and use your own signing credentials, do so using a tool such as OpenSSL or the New-SelfSignedCertificate PowerShell command. 
                 * You can use an X509 certificate, but there’s typically no need to have it issued by a Global CA. You’re only interested in the private key here.*/
                .AddDeveloperSigningCredential()
                //To solve the oracle database (older verisons) object length restriction
                //Add ApplicationPersistedGrantDbContext and ApplicationConfigurationDbContext
                .AddOperationalStore<ApplicationPersistedGrantDbContext>(options => options.ConfigureDbContext = builder => builder.UseOracle(
                                                                                                            connectionString, 
                                                                                                            sqlOptions => sqlOptions.MigrationsAssembly(migrationsAssembly)))
                .AddConfigurationStore<ApplicationConfigurationDbContext>(options => options.ConfigureDbContext = builder => builder.UseOracle(
                                                                                                            connectionString,
                                                                                                            sqlOptions => sqlOptions.MigrationsAssembly(migrationsAssembly)));
            services.AddControllersWithViews();


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //InitializeDbTestData(app);

            app.UseStaticFiles();
            app.UseRouting();

            app.UseIdentityServer();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => endpoints.MapDefaultControllerRoute());
        }

        private static void InitializeDbTestData(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                //serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();
                //serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>().Database.Migrate();
                //serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>().Database.Migrate();

                var context = serviceScope.ServiceProvider.GetRequiredService<ApplicationConfigurationDbContext>();//<ConfigurationDbContext>();

                if (!context.Clients.Any())
                {
                    foreach (var client in Clients.Get())
                    {
                        context.Clients.Add(client.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.IdentityResources.Any())
                {
                    foreach (var resource in Data.Resources.GetIdentityResources())
                    {
                        context.IdentityResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.ApiScopes.Any())
                {
                    foreach (var scope in Data.Resources.GetApiScopes())
                    {
                        context.ApiScopes.Add(scope.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.ApiResources.Any())
                {
                    foreach (var resource in Data.Resources.GetApiResources())
                    {
                        context.ApiResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }

                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
                if (!userManager.Users.Any())
                {
                    foreach (var testUser in Users.Get())
                    {
                        var identityUser = new IdentityUser(testUser.Username)
                        {
                            Id = testUser.SubjectId
                        };

                        userManager.CreateAsync(identityUser, "Password123!").Wait();
                        userManager.AddClaimsAsync(identityUser, testUser.Claims.ToList()).Wait();
                    }
                }
            }
        }
    }
}
