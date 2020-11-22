using IdentityServer4.Models;
using System.Collections.Generic;

namespace Identity.Data
{
    internal class Resources
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new[]
            {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResources.Email(),
            new IdentityResource
            {
                Name = "role",
                UserClaims = new List<string> {"role"}
            },
            new IdentityResource
            {
                Name = "mmm",
                UserClaims = new List<string> {"mmm"}
            }
        };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new[]
            {
            new ApiResource
            {
                Name = "api1",
                DisplayName = "API #1",
                Description = "Allow the application to access API #1 on your behalf",
                Scopes = new List<string> {"api1.read", "api1.write"},
                ApiSecrets = new List<Secret> {new Secret("ScopeSecret".Sha256())},
                UserClaims = new List<string> {"role"}
            }
        };
        }

        //Scopes represent what the user is authorizing the client application to do on their behalf. 
        //That does not mean that the user is allowed to perform an action.
        public static IEnumerable<ApiScope> GetApiScopes()
        {
            return new[]
            {
            new ApiScope("api1.read", "Read Access to API #1"),
            new ApiScope("api1.write", "Write Access to API #1")
        };
        }
    }
}
