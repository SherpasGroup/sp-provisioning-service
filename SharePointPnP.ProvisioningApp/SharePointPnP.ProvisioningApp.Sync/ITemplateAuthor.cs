//
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
//

namespace SharePointPnP.ProvisioningApp.Synchronization
{
    public interface ITemplateAuthor
    {
        string Name { get; }
        string Email { get; }
        string Link { get; }
    }
}
