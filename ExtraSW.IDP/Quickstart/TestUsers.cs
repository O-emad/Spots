// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityModel;
using IdentityServer4.Test;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Json;
using IdentityServer4;

namespace IdentityServerHost.Quickstart.UI
{
    public class TestUsers
    {
        public static List<TestUser> Users
        {
            get
            {
                var address = new
                {
                    street_address = "One Hacker Way",
                    locality = "Heidelberg",
                    postal_code = 69118,
                    country = "Germany"
                };
                
                return new List<TestUser>
                {
                    new TestUser
                    {
                        SubjectId = "818727",
                        Username = "Admin",
                        Password = "msicx61",
                        Claims =
                        {
                            new Claim(JwtClaimTypes.Name, "Alice Smith"),
                            new Claim(JwtClaimTypes.GivenName, "Alice"),
                            new Claim(JwtClaimTypes.FamilyName, "Smith"),
                            new Claim(JwtClaimTypes.Email, "AliceSmith@email.com"),
                            new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                            new Claim(JwtClaimTypes.WebSite, "http://alice.com"),
                            new Claim(JwtClaimTypes.Address, JsonSerializer.Serialize(address),
                            IdentityServerConstants.ClaimValueTypes.Json),
                            new Claim("role", "Admin")
                        }
                    },
                    new TestUser
                    {
                        SubjectId = "818277",
                        Username = "Vendor",
                        Password = "msicx61",
                        Claims =
                        {
                            new Claim(JwtClaimTypes.Name, "Will Smith"),
                            new Claim(JwtClaimTypes.GivenName, "Will"),
                            new Claim(JwtClaimTypes.FamilyName, "Smith"),
                            new Claim(JwtClaimTypes.Email, "WillSmith@email.com"),
                            new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                            new Claim(JwtClaimTypes.WebSite, "http://Will.com"),
                            new Claim(JwtClaimTypes.Address, JsonSerializer.Serialize(address),
                            IdentityServerConstants.ClaimValueTypes.Json),
                            new Claim("role", "Vendor")
                        }
                    }
                };
            }
        }
    }
}