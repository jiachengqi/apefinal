using System;
using apenew.Models;
using Microsoft.EntityFrameworkCore;

namespace apenew
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Assessment> Assessments { get; set; }
        public DbSet<Capability> Capabilities { get; set; }
        public DbSet<Pin> Pins { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Capability>()
                .HasOne(c => c.Assessment)
                .WithMany(a => a.Capabilities)
                .HasForeignKey(c => c.AssessmentId);
            
            modelBuilder.Entity<Pin>()
                .HasOne(p => p.Assessment)
                .WithMany(a => a.Pins)
                .HasForeignKey(p => p.AssessmentId)
                .OnDelete(DeleteBehavior.Cascade);
            
            modelBuilder.Entity<Pin>()
                .HasOne(p => p.Capability)
                .WithMany()
                .HasForeignKey(p => p.CapabilityId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

