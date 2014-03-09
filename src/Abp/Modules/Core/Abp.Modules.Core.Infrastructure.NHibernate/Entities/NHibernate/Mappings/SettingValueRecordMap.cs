﻿using Abp.Configuration;
using Abp.Domain.Entities.Mapping;
using Abp.Modules.Core.Entities.NHibernate.Mappings.Extensions;

namespace Abp.Modules.Core.Entities.NHibernate.Mappings
{
    public class SettingValueRecordMap : EntityMap<SettingValueRecord, long>
    {
        public SettingValueRecordMap()
            : base("AbpSettings")
        {
            Map(x => x.UserId);
            Map(x => x.Name);
            Map(x => x.Value);
            this.MapAudited();
        }
    }
}