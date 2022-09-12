﻿//
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
//
using Microsoft.Identity.Client;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;
using System.Configuration;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;
using System.Web.Mvc;
using SharePointPnP.ProvisioningApp.WebApp.Controllers;
using SharePointPnP.ProvisioningApp.Infrastructure.Security;
using SharePointPnP.ProvisioningApp.WebApp.Utils;
using Microsoft.IdentityModel.Logging;
using Microsoft.Owin;

namespace SharePointPnP.ProvisioningApp.WebApp
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            IdentityModelEventSource.ShowPII = true; //Add this line

            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            var provisioningScope = ConfigurationManager.AppSettings["SPPA:ProvisioningScope"];
            var provisioningEnvironment = ConfigurationManager.AppSettings["SPPA:ProvisioningEnvironment"];

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                // AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                // LoginPath = new PathString("/Home/Login"),
                // Provider = new CookieAuthenticationProvider { OnResponseSignIn = OnResponseSignIn },
                // CookieDomain = "",
                // CookieName = "cookieName",
                CookiePath = $"/{provisioningScope}"
            });

            app.UseOAuth2CodeRedeemer(
                new OAuth2CodeRedeemerOptions
                {
                    ClientId = AuthenticationConfig.ClientId,
                    ClientSecret = AuthenticationConfig.ClientSecret,
                    RedirectUri = AuthenticationConfig.RedirectUri
                });

            app.UseOpenIdConnectAuthentication(
                new OpenIdConnectAuthenticationOptions
                {
                    // RequireHttpsMetadata = false,

                    Authority = AuthenticationConfig.Authority,
                    ClientId = AuthenticationConfig.ClientId,
                    RedirectUri = AuthenticationConfig.RedirectUri,
                    PostLogoutRedirectUri = AuthenticationConfig.RedirectUri,
                    Scope = $"{AuthenticationConfig.BasicSignInScopes} {AuthenticationConfig.GraphScopes}",
                    TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        // instead of using the default validation (validating against a single issuer value, as we do in line of business apps), 
                        // we inject our own multitenant validation logic
                        ValidateIssuer = false,
                    },
                    Notifications = new OpenIdConnectAuthenticationNotifications()
                    {
                        //SecurityTokenValidated = (context) => 
                        //{
                        //    return Task.FromResult(0);
                        //},
                        AuthorizationCodeReceived = async (context) =>
                        {
                            // Upon successful sign in, get the access token & cache it using MSAL
                            IConfidentialClientApplication clientApp = MsalAppBuilder.BuildConfidentialClientApplication();
                            AuthenticationResult result = await clientApp.AcquireTokenByAuthorizationCode(AuthenticationConfig.GetGraphScopes(), context.Code).ExecuteAsync();
                        },
                        AuthenticationFailed = (context) =>
                        {
                            var httpContext = (HttpContextBase)context.OwinContext.Environment["System.Web.HttpContextBase"];

                            var routeData = new RouteData();
                            routeData.Values["controller"] = "Home";
                            routeData.Values["action"] = "Error";
                            routeData.Values["exception"] = context.Exception;

                            IController errorController = DependencyResolver.Current.GetService<HomeController>();
                            var requestContext = new RequestContext(httpContext, routeData);
                            errorController.Execute(requestContext);

                            // context.OwinContext.Response.Redirect("/Home/Error");
                            context.HandleResponse(); // Suppress the exception
                            //context.OwinContext.Response.Write(context.Exception.ToString());
                            return Task.FromResult(0);
                        }
                    }
                });
        }

        private void OnResponseSignIn(CookieResponseSignInContext context)
        {
            var debug = false;
            if (debug)
            {
                //set cookie domain
                context.Options.CookieDomain = context.OwinContext.Request.Uri.Host;

                //context.Options.CookieDomain ="www.website.com";
                //set cookie path
                context.Options.CookiePath = "/Business";
            }
        }
    }
}
