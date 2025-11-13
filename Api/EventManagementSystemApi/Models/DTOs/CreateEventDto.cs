namespace EventManagementSystemApi.Models.DTOs
{
    public class CreateEventDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DateTime { get; set; }
        public string Location { get; set; }
        public int? Capacity { get; set; }
        public string Visibility { get; set; }
        public string[]? Tags { get; set; }

    }
}
