using System.Threading.Tasks;
using SharePoint.Portal.Web.Data;
using SharePoint.Portal.Web.Models;

namespace SharePoint.Portal.Web.Business.Implementation
{
    public class PageTemplateService : IPageTemplateService
    {
        public PortalDbContext DbContext { get; set; }

        public PageTemplateService(PortalDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public async Task<PageTemplate> GetPageTemplate(string id)
        {
            return await DbContext
                            .PageTemplates
                            .FindAsync(id);
        }
    }
}
