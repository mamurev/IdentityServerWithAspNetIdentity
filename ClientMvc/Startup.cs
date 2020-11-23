using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ClientMvc
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            //This tells your application to use cookie authentication for everything(the DefaultScheme).
            //So, if you call sign in, sign out, challenge, etc. then this is the scheme that will be used. 
            //This local cookie is necessary because even though you’ll be using IdentityServer to authenticate the user and create a Single Sign-On(SSO) session, 
            //every individual client application will maintain its own, shorter-lived session.
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "cookie";
                options.DefaultChallengeScheme = "oidc";
            })
            .AddCookie("cookie")
            .AddOpenIdConnect("oidc", options =>
            {
                options.Authority = "https://localhost:5000";
                options.ClientId = "oidcClient";
                options.ClientSecret = "SuperSecretPassword";

                /*
                By default, the ASP.NET Core OpenID Connect handler will use the implicit flow with the form post response mode. 
                The implicit flow is in the process of being deprecated, and the form post response is becoming unreliable thanks to 3rd party cookies policies being rolled out by browsers. 
                As a result, you have updated these to use the authorization code flow, PKCE, and the query string response mode.
                 */
                options.ResponseType = "code";
                //Using Pkce (pixy) to prevent authorization code interception attack
                //https://www.youtube.com/watch?v=Gtbm5Fut-j8&ab_channel=SaschaPreibisch
                options.UsePkce = true;
                options.ResponseMode = "query";

                //By default, the redirect URL will use the /signin-oidc path. This is fine; however, you’ll need to add a unique callback path for each authentication handler you have in your application
                // options.CallbackPath = "/signin-oidc"; // default redirect URI

                // options.Scope.Add("oidc"); // default scope
                // options.Scope.Add("profile"); // default scope
                options.Scope.Add("api1.read");
                //SaveTokens causes the identity and access tokens to be saved, accessible using code such as HttpConect.GetTokenAsync("access_token")
                options.SaveTokens = true;
            });

        }




        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => endpoints.MapDefaultControllerRoute());
            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}
            //else
            //{
            //    app.UseExceptionHandler("/Home/Error");
            //    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            //    app.UseHsts();
            //}
            //app.UseHttpsRedirection();
            //app.UseStaticFiles();

            //app.UseRouting();

            //app.UseAuthorization();

            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllerRoute(
            //        name: "default",
            //        pattern: "{controller=Home}/{action=Index}/{id?}");
            //});
        }
    }
}
