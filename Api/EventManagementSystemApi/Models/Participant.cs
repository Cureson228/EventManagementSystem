namespace EventManagementSystemApi.Models
{
    public class Participant
    {
        public int EventId { get; set; }
        public string UserId { get; set; }
        public string FullName { get; set; }
        public  User User { get; set; }
        public Event Event { get; set; }

        public DateTime JoinedAt { get; set; }
    }
}
