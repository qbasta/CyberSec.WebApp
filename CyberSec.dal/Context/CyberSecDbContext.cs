using CyberSec.dal.Entities;
using Microsoft.EntityFrameworkCore;

namespace CyberSec.dal.Context;

public class CyberSecDbContext : DbContext
{
    public CyberSecDbContext(DbContextOptions<CyberSecDbContext> options) : base(options)
    {
    }

    public DbSet<Message> Messages { get; set; } 
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Konfiguracja dla User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasMaxLength(36);
            entity.Property(e => e.Username).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Password).HasMaxLength(255).IsRequired();
            entity.HasIndex(e => e.Username).IsUnique();
        });

        // Konfiguracja dla Message
        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasMaxLength(36);
            entity.Property(e => e.Content).IsRequired();
            entity.Property(e => e.UserId).HasMaxLength(36).IsRequired();
            entity.Property(e => e.CreatedDateTime).IsRequired();
        });

        // Konfiguracja dla Permission
        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasMaxLength(36);
            entity.Property(e => e.UserId).HasMaxLength(36).IsRequired();
            entity.Property(e => e.MessageId).HasMaxLength(36).IsRequired();
            entity.Property(e => e.CanModify).IsRequired();
        });

        // Konfiguracja relacji Users -> Messages (1:N)
        modelBuilder.Entity<Message>()
            .HasOne(m => m.User)
            .WithMany(u => u.Messages)
            .HasForeignKey(m => m.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Konfiguracja relacji Users -> Permissions (1:N)
        modelBuilder.Entity<Permission>()
            .HasOne(p => p.User)
            .WithMany(u => u.Permissions)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Konfiguracja relacji Messages -> Permissions (1:N)
        modelBuilder.Entity<Permission>()
            .HasOne(p => p.Message)
            .WithMany(m => m.Permissions)
            .HasForeignKey(p => p.MessageId)
            .OnDelete(DeleteBehavior.Cascade);

        base.OnModelCreating(modelBuilder);
    }
}