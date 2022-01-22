using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;
using Pomelo.EntityFrameworkCore.MySql.Migrations;
using System.Linq;

namespace Lychee.EntityFramework.MySql
{
    public class RemoveForeignKeyMigrationsSqlGenerator : MySqlMigrationsSqlGenerator
    {
        public RemoveForeignKeyMigrationsSqlGenerator(MigrationsSqlGeneratorDependencies dependencies, IMigrationsAnnotationProvider migrationsAnnotations, IMySqlOptions options)
            : base(dependencies, migrationsAnnotations, options)
        {
        }

        protected override void Generate(CreateTableOperation operation, IModel model, MigrationCommandListBuilder builder, bool terminate = true)
        {
            if (operation.ForeignKeys.Any())
            {
                operation.ForeignKeys.Clear();
            }

            base.Generate(operation, model, builder);
        }
    }
}