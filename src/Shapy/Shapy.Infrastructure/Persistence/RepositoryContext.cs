using Microsoft.EntityFrameworkCore;

public class RepositoryContext : DbContext
{
    public RepositoryContext(DbContextOptions<RepositoryContext> options)
    : base(options)
    {

    }

    // public DbSet<SomeEntity> SomeEntity { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}