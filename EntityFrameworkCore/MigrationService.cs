using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace EntityFrameworkCore;

public class MigrationService(
    RFPSDbContext             context,
    ILogger<MigrationService> logger,
    IConfiguration            config)
{
    public async Task Migration(string fromMigration = null,
                                string toMigration   = null,
                                bool   idempotent    = false)
    {
        var migrator = context.GetService<IMigrator>();
        string script = migrator.GenerateScript(
                                                fromMigration,
                                                toMigration,
                                                idempotent
                                                    ? MigrationsSqlGenerationOptions.Idempotent
                                                    : MigrationsSqlGenerationOptions.Default);

        await using var connection = new NpgsqlConnection(config.GetConnectionString("DefaultConnection"));

        connection.Open();
        var batches = script.Split(
                                   new[] {
                                             "GO"
                                         },
                                   StringSplitOptions.RemoveEmptyEntries);


        var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            await context.Database.OpenConnectionAsync();

            foreach (var batch in batches)
            {

                var command = context
                              .Database.GetDbConnection()
                              .CreateCommand();

                command.CommandText = batch;
                command.Transaction = transaction.GetDbTransaction();

                await command.ExecuteNonQueryAsync();
            }

            await transaction.CommitAsync();
            logger.LogInformation("Transaction Committed Successfully.");
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            logger.LogError($"Migration Error: {e.ToString()}");
        }
        finally
        {
            await connection.CloseAsync();
        }
    }
}