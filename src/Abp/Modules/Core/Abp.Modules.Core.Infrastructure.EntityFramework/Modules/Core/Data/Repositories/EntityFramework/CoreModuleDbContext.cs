using System.Data.Entity;
using Abp.Configuration;
using Abp.Domain.Repositories.EntityFramework;
using Abp.Security.Permissions;
using Abp.Security.Roles;
using Abp.Security.Tenants;
using Abp.Security.Users;

namespace Abp.Modules.Core.Data.Repositories.EntityFramework
{
    public class CoreModuleDbContext : AbpDbContext
    {
        public virtual IDbSet<AbpUser> AbpUsers { get; set; }
        
        public virtual IDbSet<UserLogin> UserLogins { get; set; }
        
        public virtual IDbSet<AbpRole> AbpRoles { get; set; }
        
        public virtual IDbSet<UserRole> UserRoles { get; set; }
        
        public virtual IDbSet<AbpTenant> AbpTenants { get; set; }
        
        public virtual IDbSet<Permission> Permissions { get; set; }
        
        public virtual IDbSet<Setting> Settings { get; set; }
    }
}