//
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
//

using System.Threading.Tasks;

namespace SharePointPnP.ProvisioningApp.Synchronization
{
    public interface IAuthorProvider
    {
        Task<ITemplateAuthor> GetAuthorAsync(string path);
    }
}
