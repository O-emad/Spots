// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace ExtraSW.IDP
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            { 
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResource("roles", "Your role(s)",
                    new List<string>(){ "role" })
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            { 
                new ApiScope("categoryapi", "Category API"),
                new ApiScope("idpapi", "IDP API")
            };

        public static IEnumerable<ApiResource> ApiResources =>
            new ApiResource[]
            {
                new ApiResource
                {
                    Name = "categoryapicollection",
                    DisplayName = "Category API Collection",
                    Scopes =
                    {
                        "categoryapi"
                    },
                    ShowInDiscoveryDocument = true
                },
                
                new ApiResource
                {
                    Name = "idpapicollection",
                    DisplayName = "IDP API Collection",
                    Scopes =
                    {
                        "idpapi"
                    },
                    ShowInDiscoveryDocument = true
                }
            };

        public static IEnumerable<Client> Clients =>
            new Client[]
            {   new Client
            {
                ClientName = "Admin Panel",
                ClientId = "adminpanelclient",
                AllowedGrantTypes = GrantTypes.Code,
                RequirePkce = true,
                RedirectUris = new List<string>()
                {
                    "https://localhost:44343/signin-oidc"
                    //"http://localhost:57749/signin-oidc"
                },
                PostLogoutRedirectUris = new List<string>()
                {
                    "https://localhost:44343/signout-callback-oidc"
                    //"http://localhost:57749/signout-callback-oidc"
                },
                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    "roles",
                    "categoryapi",
                    "idpapi"
                },
                ClientSecrets =
                {
                    new Secret("secret".Sha256())
                }
                
            }
            };
    }
}