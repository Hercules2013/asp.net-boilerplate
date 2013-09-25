using Abp.Data.Repositories;
using Abp.Modules.Core.Domain.Entities;

namespace Abp.Modules.Core.Data.Repositories
{
    /// <summary>
    /// Used to perform <see cref="Tenant"/> related database operations.
    /// </summary>
    public interface ITenantRepository : IRepository<Tenant, int>
    {

    }
}