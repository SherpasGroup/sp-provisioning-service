//
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
//
using System;
using System.Linq;
using System.Web.Http;
using Microsoft.AspNet.OData;
using SharePointPnP.ProvisioningApp.DomainModel;
using SharePointPnP.ProvisioningApp.WebApi.Components;

namespace SharePointPnP.ProvisioningApp.WebApi.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Authorize]
    public class CategoriesController : ODataController
    {
        readonly ProvisioningAppDBContext dbContext = new ProvisioningAppDBContext();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [EnableQuery]
        public IQueryable<Category> Get()
        {
            // Manage Authorization Checks
            ApiSecurityHelper.CheckRequestAuthorization(true);

            return dbContext.Categories;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [EnableQuery]
        public SingleResult<Category> Get([FromODataUri] String key)
        {
            // Manage Authorization Checks
            ApiSecurityHelper.CheckRequestAuthorization(true);

            IQueryable<Category> result = dbContext.Categories.Where(c => c.Id == key);
            return SingleResult.Create(result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            dbContext.Dispose();
            base.Dispose(disposing);
        }
    }
}