namespace EventManagementSystemApi.Models.DTOs
{
    public class ParticipantDto
    {
        public string UserId { get; set;}
        public int EventId { get; set;}
        public string FullName { get; set; }
        public DateTime JoinedAt {  get; set;}
    }
}
