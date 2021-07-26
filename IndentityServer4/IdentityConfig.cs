using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System;
using System.Collections.Generic;
using IdentityServer4;
using System.Security.Claims;
using System.Text;

namespace IdentityServer4
{
    public class IdentityConfig
    {
        public static List<TestUser> GetUsers() =>
          new List<TestUser>
          {
              new TestUser
              {
                  SubjectId = "1",
                  Username = "Mick",
                  Password = "MickPassword",
                  Claims = new List<Claim>
                  {
                      new Claim(JwtClaimTypes.Role, "Admin")
                  }
              },
              new TestUser
              {
                  SubjectId = "2",
                  Username = "Jane",
                  Password = "JanePassword",
                  Claims = new List<Claim>
                  {
                      new Claim("given_name", "Jane"),
                      new Claim("family_name", "Downing"),
                      new Claim("address", "Long Avenue 289"),
                      new Claim(JwtClaimTypes.Role, "Member")
                  }
              }
          };


        // Resources and Api want to protect
        public static IEnumerable<IdentityResource> GetIdentityResourceResources()
        {
            var customProfile = new IdentityResource(
            name: "custom.profile",
            displayName: "Custom profile",
            userClaims: new[] { "role" }); 

            return new List<IdentityResource>
            {        new IdentityResources.OpenId(),
                    new IdentityResources.Profile(),
                    customProfile
            };
        }

/*        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("api1", "My API",new List<string>(){JwtClaimTypes.Role})
            };
        }*/

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new ApiScope("api1", new List<string>(){JwtClaimTypes.Role} )
               // new ApiScope("scope2"),
            };


        public static IEnumerable<Client> Clients =>
            new Client[]
            {   
                 //Protecting an API using Client Credentials
                // m2m client credentials flow client
                new Client
                {
                    ClientId = "m2m.client",
                    ClientName = "Client Credentials Client",


                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = { new Secret("511536EF-F270-4058-80CA-1C89C192F69A".Sha256()) },

                    AllowedScopes = { "api1", "openid", "roles"}
                },

                // OpenID Connect implicit flow client (MVC)
                //https://localhost:4200 : My Client is a Angular application served on port 4200 (if present)
                new Client
                {
                    ClientName = "MVC Client",
                   ClientId = "mvc-client",
                   AllowedGrantTypes = GrantTypes.Implicit,
                   RedirectUris = new List<string>{ "https://localhost:4200/signin-oidc" },
                   RequirePkce = false,
                   AllowAccessTokensViaBrowser =true,
                    PostLogoutRedirectUris = new[]{
                        "https://localhost:4200/" },
                   AllowedScopes =
                   {
                       IdentityServerConstants.StandardScopes.OpenId,
                       IdentityServerConstants.StandardScopes.Profile,
                       "api1",
                       "roles"
                   },
                   ClientSecrets = { new Secret("Secret".Sha512()) },
                   //PostLogoutRedirectUris = new List<string> { "https://localhost:4200/signout-callback-oidc" },
                   RequireConsent = true
                },


                // interactive client using code flow + pkce
                new Client
                {
                    ClientId = "interactive",
                    ClientSecrets = { new Secret("49C1A7E1-0C79-4A89-A3D6-A37998FB86B0".Sha256()) },

                    AllowedGrantTypes = GrantTypes.Code,
                    // where to redirect to after login
                    RedirectUris = { "https://localhost:4200/signin-oidc" },
                     // where to redirect to after logout
                    FrontChannelLogoutUri = "https://localhost:4200/signout-oidc",
                    PostLogoutRedirectUris = { "https://localhost:4200/signout-callback-oidc" },

                    AllowOfflineAccess = true,
                    AllowedScopes = { "openid", "profile", "scope2" }
                },

                //Protecting an API using Passwords
                // not recommend
                new Client
                {
                    ClientId = "ro.client",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,

                    ClientSecrets =
                    {
                        new Secret("Secret".Sha256())
                    },
                    AllowedScopes = {"api1", "custom.profile", IdentityServerConstants.StandardScopes.Profile, IdentityServerConstants.StandardScopes.OpenId}
                }
            };
    }
}
