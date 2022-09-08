//
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
//

using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin.Security.ActiveDirectory;
using Owin;
using SharePointPnP.ProvisioningApp.Infrastructure.Security;

namespace SharePointPnP.ProvisioningApp.WebApi
{
    /// <summary>
    /// 
    /// </summary>
    public partial class Startup
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        public void ConfigureAuth(IAppBuilder app)
        {
            app.UseWindowsAzureActiveDirectoryBearerAuthentication(
                new WindowsAzureActiveDirectoryBearerAuthenticationOptions
                {
                    Tenant = "common", // To support multi-tenant
                    TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidAudience = AuthenticationConfig.Audience,
                        ValidateIssuer = false, // To support multi-tenant
                    }
                });
        }
    }
}