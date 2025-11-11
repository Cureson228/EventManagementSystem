using EventManagementSystemApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace EventManagementSystemApi.Data
{
    
    public static class SeedData
    {
        private record SeedUser(string FullName, string Email, string Password);

        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var services = scope.ServiceProvider;

            var userDbContext = services.GetRequiredService<UserDbContext>();
            var appDbContext = services.GetRequiredService<AppDbContext>();
            var userManager = services.GetRequiredService<UserManager<User>>();


            await userDbContext.Database.MigrateAsync();
            await appDbContext.Database.MigrateAsync();

            if (!userDbContext.Users.Any() && !appDbContext.Events.Any())
            {

                var user1 = new User
                {
                    FullName = "Andrew Tate",
                    Email = "andrewtate@gmail.com",
                    UserName = "andrewtate@gmail.com"
                };
                await userManager.CreateAsync(user1, "Tate12345");

                var user2 = new User
                {
                    FullName = "Tristan Tate",
                    Email = "tristantate@gmail.com",
                    UserName = "tristantate@gmail.com"
                };
                await userManager.CreateAsync(user2, "Tate12345");

                var user3 = new User
                {
                    FullName = "Elon Musk",
                    Email = "elonmusk@gmail.com",
                    UserName = "elonmusk@gmail.com"
                };
                await userManager.CreateAsync(user3, "Musk12345");

                var user4 = new User
                {
                    FullName = "Jeff Bezos",
                    Email = "jeffbezos@gmail.com",
                    UserName = "jeffbezos@gmail.com"
                };
                await userManager.CreateAsync(user4, "Bezos12345");

                var user5 = new User
                {
                    FullName = "Mark Zuckerberg",
                    Email = "markzuckerberg@gmail.com",
                    UserName = "markzuckerberg@gmail.com"
                };
                await userManager.CreateAsync(user5, "Zuck12345");


                var event1 = new Event
                {
                    Title = "Whatever podcast's with Andrew Tate",
                    Description = "Join and visit upcoming whatever's podcast with a special guest Andrew Tate!",
                    Location = "Dubai",
                    DateTime = DateTime.UtcNow.AddMonths(3).AddDays(3).AddHours(18).AddMinutes(30),
                    Capacity = 10,
                    Visibility = true,
                    CreatedByUserId = user1.Id,
                    Participants = new List<Participant>()
                };

                var event2 = new Event
                {
                    Title = "Tech Innovations Summit",
                    Description = "A conference about the latest technology innovations in AI and space.",
                    Location = "San Francisco",
                    DateTime = DateTime.UtcNow.AddMonths(2).AddDays(5).AddHours(15).AddMinutes(30),
                    Capacity = 50,
                    Visibility = true,
                    CreatedByUserId = user3.Id,
                    Participants = new List<Participant>()
                };

                var event3 = new Event
                {
                    Title = "Startup Networking Evening",
                    Description = "Network with top entrepreneurs and investors.",
                    Location = "New York",
                    DateTime = DateTime.UtcNow.AddMonths(1).AddDays(10).AddHours(20),
                    Capacity = 30,
                    Visibility = true,
                    CreatedByUserId = user4.Id,
                    Participants = new List<Participant>()
                };

                event1.Participants.Add(new Participant
                {
                    Event = event1,
                    FullName = user1.FullName,
                    UserId = user1.Id,
                    JoinedAt = DateTime.UtcNow
                });
                event1.Participants.Add(new Participant
                {
                    Event = event1,
                    FullName = user2.FullName,
                    UserId = user2.Id,
                    JoinedAt = DateTime.UtcNow
                });

                event2.Participants.Add(new Participant
                {
                    Event = event2,
                    FullName = user3.FullName,
                    UserId = user3.Id,
                    JoinedAt = DateTime.UtcNow
                });
                event2.Participants.Add(new Participant
                {
                    Event = event2,
                    FullName = user5.FullName,
                    UserId = user5.Id,
                    JoinedAt = DateTime.UtcNow
                });

                event3.Participants.Add(new Participant
                {
                    Event = event3,
                    FullName = user4.FullName,
                    UserId = user4.Id,
                    JoinedAt = DateTime.UtcNow
                });
                event3.Participants.Add(new Participant
                {
                    Event = event3,
                    FullName = user2.FullName,
                    UserId = user2.Id,
                    JoinedAt = DateTime.UtcNow
                });

                appDbContext.Events.AddRange(event1, event2, event3);

                await appDbContext.SaveChangesAsync();

                var tag1 = new Tag { Name = "Podcast" };
                var tag2 = new Tag { Name = "Business" };
                var tag3 = new Tag { Name = "Tech" };

                

                appDbContext.Tags.AddRange(tag1, tag2, tag3);

                await appDbContext.SaveChangesAsync();

                var eventTags = new List<EventTag>()
                {
                    new EventTag {EventId = 1 , TagId = 1},
                    new EventTag {EventId = 1, TagId = 2},
                    new EventTag {EventId = 2, TagId = 3},
                    new EventTag {EventId = 3 , TagId = 3},
                };

                appDbContext.EventsTags.AddRange(eventTags);

                await appDbContext.SaveChangesAsync();

            }
        }
    }
}
