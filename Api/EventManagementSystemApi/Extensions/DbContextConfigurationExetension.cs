using EventManagementSystemApi.Models;
using Microsoft.EntityFrameworkCore;

namespace EventManagementSystemApi.Extensions
{
    public static class DbContextConfigurationExetension
    {
        public static void ConfigureEventModel(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Event>(entity =>
            {
                entity.ToTable("Events");

                entity.HasKey(property => property.Id);

                entity.Property(property => property.Title).IsRequired().HasMaxLength(50);

                entity.Property(property => property.Description).HasMaxLength(500);

                entity.Property(property => property.Location).IsRequired().HasMaxLength(50);

                entity.Property(property => property.DateTime).IsRequired();


                entity.HasOne(property => property.CreatedByUser).WithMany(user => user.CreatedEvents).OnDelete(DeleteBehavior.Restrict);
            });
        }

        public static void ConfigureParticipantModel(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Participant>(entity =>
            {
                entity.ToTable("Participants");

                entity.HasKey(property => new { property.EventId, property.UserId });

                entity.Property(property => property.JoinedAt).IsRequired();

                entity.HasOne(property => property.User).WithMany(user => user.Participants).HasForeignKey(p => p.UserId).OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(property => property.Event).WithMany(e => e.Participants).HasForeignKey(property => property.EventId).OnDelete(DeleteBehavior.Cascade);
            });
        }

        public static void ConfigureUserModel(this ModelBuilder modelBuilder)
        {
            
            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(property => property.FullName).IsRequired().HasMaxLength(100);

                entity.Property(property => property.CreatedAt).IsRequired();

                
            });
            modelBuilder.Ignore<Event>().Ignore<Participant>();
        }
    }
}
