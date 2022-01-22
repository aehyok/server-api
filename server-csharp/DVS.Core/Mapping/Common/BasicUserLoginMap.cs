﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DVS.Core.Domains.Common;
using Lychee.EntityFramework;
using Microsoft.EntityFrameworkCore;
using DVS.Common.Mapping;

namespace DVS.Core.Mapping.Common
{
    public class BasicUserLoginMap : DvsMapBase<BasicUserLogin>
    {
        public override void Configure(EntityTypeBuilder<BasicUserLogin> builder)
        {

            builder.Ignore(a => a.CreatedBy);
            builder.Ignore(a => a.UpdatedBy);
            builder.Ignore(a => a.IsDeleted);
            base.Configure(builder);
        }
    }
}
