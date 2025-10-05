using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Context;

[ExcludeFromCodeCoverage]
public class SqlServerDataContext : DbContext
{

    public SqlServerDataContext() { }

    public SqlServerDataContext(DbContextOptions<SqlServerDataContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SqlServerDataContext).Assembly);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.EnableSensitiveDataLogging();
        base.OnConfiguring(optionsBuilder);
    }
}
