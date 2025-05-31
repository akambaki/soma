using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Soma.Platform.Core.Models;

namespace Soma.Platform.Core.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        // Configure ApplicationUser entity
        builder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.VerificationToken).HasMaxLength(500);
            entity.Property(e => e.GoogleId).HasMaxLength(100);
            entity.Property(e => e.AppleId).HasMaxLength(100);
            entity.Property(e => e.ProfileImageUrl).HasMaxLength(500);
            
            // Index for performance
            entity.HasIndex(e => e.GoogleId);
            entity.HasIndex(e => e.AppleId);
            entity.HasIndex(e => e.VerificationToken);
        });
    }
}