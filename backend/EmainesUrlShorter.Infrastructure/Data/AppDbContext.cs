using EmainesUrlShorter.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmainesUrlShorter.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<ShortLink> ShortLinks { get; set; }
    public DbSet<LinkAccess> LinkAccesses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ShortLink>()
            .HasIndex(s => s.Code)
            .IsUnique();

        modelBuilder.Entity<LinkAccess>()
            .HasOne(l => l.ShortLink)
            .WithMany(s => s.Accesses)
            .HasForeignKey(l => l.LinkId);
    }
}
