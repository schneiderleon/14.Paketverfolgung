using Microsoft.EntityFrameworkCore;
using Paketverfolgung.Models;

namespace Paketverfolgung;

public class PaketverfolgungContext : DbContext
{
    public DbSet<Kunde> Kunden => Set<Kunde>();
    public DbSet<Bestellung> Bestellungen => Set<Bestellung>();

    
    public const string ConnectionString =
        @"Server=(localdb)\MSSQLLocalDB;Database=PaketverfolgungDB;Trusted_Connection=True;TrustServerCertificate=True";

    public PaketverfolgungContext() { }
    public PaketverfolgungContext(DbContextOptions<PaketverfolgungContext> options)
        : base(options) { }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(ConnectionString);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Kunde>(e =>
        {
            e.ToTable("Kunde");
            e.HasKey(x => x.KundeID);
            e.Property(x => x.Name).HasMaxLength(100).IsRequired();
            e.Property(x => x.Adresse).HasMaxLength(200);
            e.Property(x => x.EMail).HasMaxLength(100);
            e.Property(x => x.Telefonnummer).HasMaxLength(50);
        });

        modelBuilder.Entity<Bestellung>(e =>
        {
            e.ToTable("Bestellung");
            e.HasKey(x => x.BestellungID);
            e.Property(x => x.Status).HasMaxLength(50).IsRequired();
            e.Property(x => x.Produktname).HasMaxLength(100).IsRequired();

            e.HasOne(x => x.Kunde)
             .WithMany(k => k.Bestellungen)
             .HasForeignKey(x => x.KundeID)
             .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
