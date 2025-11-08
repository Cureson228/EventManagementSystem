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


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {   
            
            modelBuilder.ConfigureEventModel();
            modelBuilder.ConfigureParticipantModel();
            modelBuilder.Ignore<User>();

        }
    }
}
