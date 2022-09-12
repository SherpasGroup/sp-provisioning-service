//
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
//

using System.Web.Http;

namespace SharePointPnP.ProvisioningApp.WebApp.Controllers
{
    [Authorize]
    public class UserProfileController : ApiController
    {
        [HttpGet()]
        [Route("UserProfile/Username")]
        public string GetUserName()
        {
            var result = string.Empty;

            if (System.Threading.Thread.CurrentPrincipal != null &&
                System.Threading.Thread.CurrentPrincipal.Identity != null &&
                System.Threading.Thread.CurrentPrincipal.Identity.IsAuthenticated)
            {
                result = System.Threading.Thread.CurrentPrincipal.Identity.Name;
            }

            return result;
        }
    }
}
