using EventManagementSystemApi.Extensions;
using Microsoft.EntityFrameworkCore;

namespace EventManagementSystemApi.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            :base(options) { }
        


        public DbSet<Event> Events { get; set; }

        public DbSet<Participant> Participants { get; set; }

        public DbSet<Tag> Tags { get; set; }
        public DbSet<EventTag> EventsTags { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {   
            
            modelBuilder.ConfigureEventModel();
            modelBuilder.ConfigureParticipantModel();
            modelBuilder.ConfigureTagModel();
            modelBuilder.ConfigureEventTagModel();
            modelBuilder.Ignore<User>();

        }
    }
}
