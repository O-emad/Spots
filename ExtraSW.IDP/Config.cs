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
                new IdentityResource("level", "your level",
                    new List<string>(){ "level" })
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            { 
                new ApiScope("categoryapi", "Category API")
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
                    "https://localhost:44341/signin-oidc"
                },
                PostLogoutRedirectUris = new List<string>()
                {
                    "https://localhost:44341/signout-callback-oidc"
                },
                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    "level",
                    "categoryapi"
                },
                ClientSecrets =
                {
                    new Secret("secret".Sha256())
                }
            }
            };
    }
}