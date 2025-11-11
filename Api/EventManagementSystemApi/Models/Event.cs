namespace EventManagementSystemApi.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DateTime { get; set; }
        public string Location { get; set; }
        public int? Capacity { get; set; }
        public bool Visibility { get; set; }
        public string CreatedByUserId { get; set; }
        public User CreatedByUser { get; set; }
        
        public ICollection<Participant> Participants { get; set; }
        public ICollection<EventTag> EventTags { get; set; }


    }
}
