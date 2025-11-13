namespace EventManagementSystemApi.Models
{
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<EventTag> EventTags { get; set; }
    }
}
