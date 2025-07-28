using Microsoft.EntityFrameworkCore;
using Shapy.Domain.Models;

namespace Shapy.Infrastructure.Persistence
{
    public class RepositoryContext : DbContext
    {
        public RepositoryContext(DbContextOptions<RepositoryContext> options)
        : base(options)
        {

        }

        public DbSet<Project> Project { get; set; }
        public DbSet<User> User { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                // Convert table name to camelCase
                var tableName = entity.GetTableName();
                if (!string.IsNullOrEmpty(tableName))
                {
                    entity.SetTableName(ToCamelCase(tableName));
                }

                // Convert property (column) names to camelCase
                foreach (var property in entity.GetProperties())
                {
                    property.SetColumnName(ToCamelCase(property.Name));
                }

                // Convert key names to camelCase
                foreach (var key in entity.GetKeys())
                {
                    key.SetName(ToCamelCase(key.GetName()!));
                }

                // Convert foreign key constraint names to camelCase
                foreach (var fk in entity.GetForeignKeys())
                {
                    fk.SetConstraintName(ToCamelCase(fk.GetConstraintName()!));
                }

                // Convert index names to camelCase
                foreach (var index in entity.GetIndexes())
                {
                    index.SetDatabaseName(ToCamelCase(index.GetDatabaseName()!));
                }
            }
        }

         private static string ToCamelCase(string input)
        {
            if (string.IsNullOrEmpty(input) || char.IsLower(input[0]))
                return input;

            if (input.Length == 1)
                return input.ToLowerInvariant();

            return char.ToLowerInvariant(input[0]) + input.Substring(1);
        }
    }
}