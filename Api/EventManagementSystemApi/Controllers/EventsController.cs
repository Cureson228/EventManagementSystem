using EventManagementSystemApi.Models.DTOs;
using EventManagementSystemApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventManagementSystemApi.Controllers
{

    [ApiController]
    [Authorize]
    [Route("/api/events")]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;
        public EventsController(IEventService eventService)
        {
            _eventService = eventService;
        }
        
        [HttpPost("create")]
        public async Task<IActionResult> CreateEvent([FromBody] CreateEventDto dto)
        {
            await _eventService.CreateEventAsync(dto);
            return Ok();
        }
        [AllowAnonymous]
        [HttpGet("tags")]
        public async Task<IActionResult> GetAllTags()
        {
            var tagsList = await _eventService.GetAllTagsAsync();
            return Ok(tagsList);
        }
        [AllowAnonymous]
        [HttpGet("tags/{id}")]
        public async Task<IActionResult> GetEventTags([FromRoute]int id)
        {
            var eventTags = await _eventService.GetEventTagsAsync(id);
            return Ok(eventTags);
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetPublicEvents([FromQuery]string[]? tags = null)
        {
            var eventList = await _eventService.GetPublicEventsAsync(tags);
            return Ok(eventList);
        }
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEventDetails([FromRoute]int id)
        {
           var eventDetails = await _eventService.GetEventDetailsAsync(id);
           return Ok(eventDetails);
        }
        [HttpPatch("{id}")]
        public async Task<IActionResult> EditEvent([FromBody] CreateEventDto dto, [FromRoute] int id)
        {
            await _eventService.EditEventAsync(dto, id);        
            return Ok();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            await _eventService.DeleteEventAsync(id);
            return Ok();
        }
        [HttpPost("{Id}/join")]
        public async Task<IActionResult> JoinEvent([FromRoute] int id)
        {
            await _eventService.JoinEventAsync(id);
            return Ok();
        }
        [HttpPost("{id}/leave")] 
        public async Task<IActionResult> LeaveEvent([FromRoute] int id)
        {
            await _eventService.LeaveEventAsync(id);
            return Ok();
              
        }

    }
}
