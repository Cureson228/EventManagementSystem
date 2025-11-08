using EventManagementSystemApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventManagementSystemApi.Controllers
{
    [ApiController]
    [Route("/api/users")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IEventService _eventService;
        public UsersController(IEventService eventService)
        {
            _eventService = eventService;
        }


        [HttpGet("me/events")]
        public async Task<IActionResult> GetUserEvents()
        {
            var events = await _eventService.GetUserEventsAsync();
            return Ok(events);
        }
    }
}
