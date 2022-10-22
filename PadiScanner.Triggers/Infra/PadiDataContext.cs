using Microsoft.EntityFrameworkCore;
using PadiScanner.Triggers.Data;

namespace PadiScanner.Triggers.Infra;

public class PadiDataContext : DbContext
{
    public PadiDataContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<PredictionHistory> Predictions => Set<PredictionHistory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // predictions entity
        modelBuilder.Entity<PredictionHistory>()
            .Property(x => x.Id)
            .HasMaxLength(26)
            .HasConversion<UlidConverter>();
        modelBuilder.Entity<PredictionHistory>()
            .Property(x => x.Probabilities)
            .HasConversion<ProbabilitiesConverter>();
        modelBuilder.Entity<PredictionHistory>()
            .HasOne(x => x.Uploader)
            .WithMany(x => x.Predictions)
            .HasForeignKey(x => x.UploaderId);

        // users entity
        modelBuilder.Entity<User>()
            .Property(x => x.Id)
            .HasMaxLength(26)
            .HasConversion<UlidConverter>();
    }
}
