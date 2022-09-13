//
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
//

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
using System.Net;

namespace SharePointPnP.ProvisioningApp.WebApp
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            // https://stackoverflow.com/questions/73110391/idx10803-unable-to-create-to-obtain-configuration-from-https-login-microsof
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;

            IdentityModelEventSource.ShowPII = true; //Add this line

            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            var provisioningScope = ConfigurationManager.AppSettings["SPPA:ProvisioningScope"];
            var provisioningEnvironment = ConfigurationManager.AppSettings["SPPA:ProvisioningEnvironment"];

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                // AuthenticationType = "Cookies", // DefaultAuthenticationTypes.ApplicationCookie,
                // LoginPath = new PathString("/Home/Login"),
                // Provider = new CookieAuthenticationProvider { OnResponseSignIn = OnResponseSignIn },
                // CookieDomain = "",
                // CookieName = "ourCookieName",
                CookiePath = $"/{provisioningScope}",
                // CookiePath = $"/",

                // CookieManager = new SystemWebCookieManager()    // <-- new
                // CookieManager = new SystemWebChunkingCookieManager(),   // <-- new
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

                    // ResponseType = "code id_token",
                    // SignInAsAuthenticationType = "cookie",
                    // SignInAsAuthenticationType = "Cookies",
                    // UseTokenLifetime = false,
                    // ClientSecret = "secret",


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
                        RedirectToIdentityProvider = async n =>
                        {   // https://www.scottbrady91.com/aspnet/refreshing-your-legacy-aspnet-identityserver-client-applications

                            await Task.CompletedTask;

                            /*
                            if (n.ProtocolMessage.RequestType == OpenIdConnectRequestType.Authentication)
                            {
                                // generate code verifier and code challenge
                                var codeVerifier = CryptoRandom.CreateUniqueId(32);

                                string codeChallenge;
                                using (var sha256 = SHA256.Create())
                                {
                                    var challengeBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(codeVerifier));
                                    codeChallenge = Base64Url.Encode(challengeBytes);
                                }

                                // set code_challenge parameter on authorization request
                                n.ProtocolMessage.SetParameter("code_challenge", codeChallenge);
                                n.ProtocolMessage.SetParameter("code_challenge_method", "S256");

                                // remember code verifier in cookie (adapted from OWIN nonce cookie)
                                // see: https://github.com/scottbrady91/Blog-Example-Classes/blob/master/AspNetFrameworkPkce/ScottBrady91.BlogExampleCode.AspNetPkce/Startup.cs#L85
                                // RememberCodeVerifier(n, codeVerifier);
                            }
                            */
                        },
                        AuthorizationCodeReceived = async (context) =>
                        {
                            // Upon successful sign in, get the access token & cache it using MSAL
                            var clientApp = MsalAppBuilder.BuildConfidentialClientApplication();
                            var result = await clientApp
                                                .AcquireTokenByAuthorizationCode(AuthenticationConfig.GetGraphScopes(), context.Code)
                                                .ExecuteAsync();

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
