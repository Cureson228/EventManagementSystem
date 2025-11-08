using Microsoft.AspNetCore.Identity;

namespace EventManagementSystemApi.Models
{
    public class User : IdentityUser
    {
        public string FullName { get; set; }
        public DateTime CreatedAt { get; set; }

        public ICollection<Event> CreatedEvents { get; set; }
        public ICollection<Participant> Participants { get; set; }
    }
}
