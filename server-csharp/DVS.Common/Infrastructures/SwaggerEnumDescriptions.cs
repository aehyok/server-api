using DVS.Common.Models;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DVS.Common.Infrastructures
{
    public class SwaggerEnumDescriptions : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            foreach (var item in swaggerDoc.Components.Schemas)
            {
                if (item.Value.Enum != null && item.Value.Enum.Count > 0)
                {
                    item.Value.Description += ": " + DescribeEnum(item.Key);
                }
            }
        }

        private string DescribeEnum(string key)
        {
            var types = typeof(DvsEntityBase).Assembly.GetTypes();

            var type = types.Where(a => a.Name == key).FirstOrDefault();

            if (type == null)
            {
                return string.Empty;
            }

            var enumDescriptions = new List<string>();

            foreach (var value in Enum.GetValues(type))
            {
                enumDescriptions.Add($"{(int)value}={Enum.GetName(type, value)}");
            }

            return string.Join(',', enumDescriptions);
        }
    }
}