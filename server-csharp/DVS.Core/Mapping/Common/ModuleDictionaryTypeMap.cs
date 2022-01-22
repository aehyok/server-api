using DVS.Common.Mapping;
using DVS.Core.Domains.Common;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Core.Mapping.Common
{
   public  class ModuleDictionaryTypeMap:DvsMapBase<ModuleDictionaryType>
    {
        public override void Configure(EntityTypeBuilder<ModuleDictionaryType> builder)
        {
            base.Configure(builder);
        }
    }
}
