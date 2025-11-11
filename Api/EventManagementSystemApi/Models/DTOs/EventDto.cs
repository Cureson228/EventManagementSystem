namespace EventManagementSystemApi.Models.DTOs
{
    public class EventDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public DateTime DateTime { get; set; }
        public string CreatedByUserId { get; set; }
        public int? Capacity { get; set;}
        public bool Visibility { get; set; }
        public List<ParticipantDto> Participants { get; set; }
        public List<string> EventTags { get; set; }
    }
}
