using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using Abp.Application.Session;
using Abp.Dependency;
using Abp.Runtime.Security;
using Abp.Security.Tenants;
using Abp.Security.Users;
using Microsoft.AspNet.Identity;

namespace Abp.Application
{
    /// <summary>
    /// 
    /// </summary>
    public class AbpSession : IAbpSession, ISingletonDependency
    {
        public long? UserId
        {
            get
            {
                var userId = Thread.CurrentPrincipal.Identity.GetUserId();
                if (userId == null)
                {
                    return null;
                }

                return Convert.ToInt32(userId);
            }
        }

        public int? TenantId
        {
            get
            {
                var claimsPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
                if (claimsPrincipal == null)
                {
                    return null;
                }

                var claim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == AbpClaimTypes.TenantId);
                if (claim == null)
                {
                    return null;
                }

                return Convert.ToInt32(claim.Value);
            }
        }
    }
}