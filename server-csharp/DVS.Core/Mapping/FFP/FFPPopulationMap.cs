using DVS.Common.Mapping;
using DVS.Core.Domains.FFP;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Core.Mapping.FFP
{
    public class FFPPopulationMap : DvsMapBase<FFPPopulation>
    {
        public override void Configure(EntityTypeBuilder<FFPPopulation> builder)
        {
            base.Configure(builder);
        }
    }
}
