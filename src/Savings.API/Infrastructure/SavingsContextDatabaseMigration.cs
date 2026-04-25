using Microsoft.EntityFrameworkCore;

namespace Savings.API.Infrastructure
{
    internal static class SavingsContextDatabaseMigration
    {
        public static void MigrateDatabase(this SavingsContext context)
        {
            MarkInitialMigrationAppliedForLegacyDatabase(context);
            context.Database.Migrate();
        }

        private static void MarkInitialMigrationAppliedForLegacyDatabase(SavingsContext context)
        {
            context.Database.OpenConnection();
            try
            {
                if (TableExists(context, "Configurations") && !TableExists(context, "__EFMigrationsHistory"))
                {
                    context.Database.ExecuteSqlRaw("""
                        CREATE TABLE "__EFMigrationsHistory" (
                            "MigrationId" TEXT NOT NULL CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY,
                            "ProductVersion" TEXT NOT NULL
                        );
                        """);
                    context.Database.ExecuteSqlRaw("""
                        INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
                        VALUES ('20260218115546_InitialCreate', '10.0.1');
                        """);
                }
            }
            finally
            {
                context.Database.CloseConnection();
            }
        }

        private static bool TableExists(SavingsContext context, string tableName)
        {
            using var command = context.Database.GetDbConnection().CreateCommand();
            command.CommandText = """
                SELECT COUNT(*)
                FROM sqlite_master
                WHERE type = 'table' AND name = $tableName;
                """;

            var tableNameParameter = command.CreateParameter();
            tableNameParameter.ParameterName = "$tableName";
            tableNameParameter.Value = tableName;
            command.Parameters.Add(tableNameParameter);

            return Convert.ToInt64(command.ExecuteScalar()) > 0;
        }
    }
}
