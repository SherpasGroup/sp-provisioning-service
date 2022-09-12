//
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
//

using Owin;

namespace SharePointPnP.ProvisioningApp.WebApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
