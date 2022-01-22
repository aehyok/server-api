using DVS.Common.Mapping;
using DVS.Core.Domains.Common;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Core.Mapping.Common
{
   public  class ModuleDictionaryMap
    {
        public class GISCameraMap : DvsMapBase<ModuleDictionary>
        {
            public override void Configure(EntityTypeBuilder<ModuleDictionary> builder)
            {
                base.Configure(builder);
            }
        }
    }
}
