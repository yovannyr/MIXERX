using Microsoft.EntityFrameworkCore;
using MIXERX.Core.Models;

namespace MIXERX.Infrastructure.Data.AppDbContext;

public class LibraryContext : DbContext
{
    public DbSet<Track> Tracks { get; set; }
    public DbSet<Crate> Crates { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MIXERX", "library.db");
        Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);
        optionsBuilder.UseSqlite($"Data Source={dbPath}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Track>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.FilePath).IsUnique();
            entity.Property(e => e.Title).HasMaxLength(500);
            entity.Property(e => e.Artist).HasMaxLength(500);
            entity.Property(e => e.Album).HasMaxLength(500);
        });

        modelBuilder.Entity<Crate>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.HasMany(e => e.Tracks);
        });
    }
}
