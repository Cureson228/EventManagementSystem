using EventManagementSystemApi.Models;
using EventManagementSystemApi.Models.DTOs;

namespace EventManagementSystemApi.Services
{
    public interface IEventService
    {
        
        Task CreateEventAsync(CreateEventDto dto);

        Task<string[]> GetAllTagsAsync();
        Task<string[]> GetEventTagsAsync(int id);

        Task<IEnumerable<EventDto>> GetPublicEventsAsync(string[]? Tags = null);

        Task<EventDto> GetEventDetailsAsync(int id);
        Task EditEventAsync(CreateEventDto dto, int id);
        Task DeleteEventAsync(int id);
        Task<IEnumerable<EventDto>> GetUserEventsAsync();
        Task JoinEventAsync(int id);

        Task LeaveEventAsync(int id);
    }
}
