using EventManagementSystemApi.Models;
using EventManagementSystemApi.Models.DTOs;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Claims;

namespace EventManagementSystemApi.Services
{
    public class EventService : IEventService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _contextAccessor;
        public EventService(AppDbContext dbContext, IHttpContextAccessor contextAccessor)
        {
            _context = dbContext;
            _contextAccessor = contextAccessor; 
        }


        public async Task CreateEventAsync(CreateEventDto dto)
        {
            var userId = _contextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var fullName = _contextAccessor.HttpContext?.User.FindFirstValue("FullName");

            if (userId == null)
            {
                throw new UnauthorizedAccessException("Error happened while accessing the user");
            }


            var newEvent = new Event
            {
                Title = dto.Title,
                Description = dto.Description,
                DateTime = dto.DateTime,
                Location = dto.Location,
                Capacity = dto.Capacity,
                Visibility = Boolean.Parse(dto.Visibility),
                CreatedByUserId = userId
                
            };

            _context.Events.Add(newEvent);

            var participant = new Participant
            {
                Event = newEvent,
                UserId = userId,
                FullName = fullName!,
                JoinedAt = DateTime.UtcNow,
            };

            _context.Participants.Add(participant);

            await _context.SaveChangesAsync(); 
            
        }

        public async Task<IEnumerable<EventDto>> GetPublicEventsAsync()
        {

            return await _context.Events
                .Where(e => e.Visibility)
                .Include(e => e.Participants)
                .Select(e => new EventDto
                {
                    Id = e.Id,
                    Title = e.Title,
                    Description = e.Description,
                    Location = e.Location,
                    DateTime = e.DateTime,
                    CreatedByUserId = e.CreatedByUserId,
                    Capacity = e.Capacity,
                    Participants = e.Participants.Select(p => new ParticipantDto
                    {
                        EventId = e.Id,
                        UserId = p.UserId,
                        JoinedAt = p.JoinedAt,
                        FullName = p.FullName
                        
                    }).ToList()
                })
                .ToListAsync();
        }

        public async Task<EventDto> GetEventDetailsAsync(int id)
        {
            var eventDetails = await _context.Events.Where(e => e.Id == id)
                .Include(e => e.Participants)
                .Select(e => new EventDto
                {
                    Id = e.Id,
                    Title = e.Title,
                    Description = e.Description,
                    Location = e.Location,
                    DateTime = e.DateTime,
                    CreatedByUserId = e.CreatedByUserId,
                    Capacity= e.Capacity,
                    Visibility = e.Visibility,
                    Participants = e.Participants.Select(p => new ParticipantDto
                    {
                        EventId = e.Id,
                        UserId = p.UserId,
                        JoinedAt = p.JoinedAt,
                        FullName = p.FullName,

                    }).ToList()
                }).FirstAsync();

            if (eventDetails == null)
                throw new KeyNotFoundException($"Event with ID {id} was not found");


            return eventDetails;
            
        }
        public async Task EditEventAsync(CreateEventDto dto, int id)
        {
            var _event = await _context.Events.FindAsync(id);
            if (_event  == null)
            {
                throw new KeyNotFoundException("Event not found");
            }

            var userId = _contextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (_event.CreatedByUserId != userId)
                throw new UnauthorizedAccessException("You are not allowed to edit this event");

            _event.Title = dto.Title;
            _event.Description = dto.Description;
            _event.Location = dto.Location;
            _event.DateTime = dto.DateTime;
            _event.Visibility = Boolean.Parse(dto.Visibility);
            _event.Capacity = dto.Capacity;

            await _context.SaveChangesAsync();
            
        }
        public async Task DeleteEventAsync(int id)
        {
            var userId = _contextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                throw new ValidationException("User not authorized");

            var _event = await _context.Events.Include(e => e.Participants).FirstOrDefaultAsync(e => e.Id == id);

            if (_event == null)
                throw new KeyNotFoundException("Event not found");


            if (userId != _event?.CreatedByUserId)
                throw new UnauthorizedAccessException("You are not allowed to delete this event");

            _context.Participants.RemoveRange(_event.Participants);
            _context.Events.Remove(_event);

            await _context.SaveChangesAsync();

        }

        public async Task<IEnumerable<EventDto>> GetUserEventsAsync()
        {
            var userId = _contextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                throw new UnauthorizedAccessException("User not authorized");

            return await _context.Events
                .Include(e => e.Participants)
                .Where(e => e.Participants.Any(p => p.UserId == userId))
                .Select(e => new EventDto
                {
                    Id = e.Id,
                    Title = e.Title,
                    Description = e.Description,
                    DateTime = e.DateTime,
                    Location = e.Location,
                    Capacity = e.Capacity,
                    CreatedByUserId = e.CreatedByUserId,
                    Visibility = e.Visibility,
                })
                .ToListAsync();
        }

        public async Task JoinEventAsync(int id)
        {
            var userId = _contextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var fullName = _contextAccessor.HttpContext?.User.FindFirstValue("FullName");

            if (userId == null)
                throw new UnauthorizedAccessException("User not authorized");

            if (await _context.Participants.AnyAsync(p => p.EventId == id && p.UserId == userId))
                throw new ValidationException("User already joined this event");

            await _context.Participants.AddAsync(new Participant
            {
                EventId = id,
                UserId = userId,
                FullName = fullName!,
                JoinedAt = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();
        }

        public async Task LeaveEventAsync(int id)
        {
            var userId = _contextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                throw new UnauthorizedAccessException("Error happened while accessing the user");


            var participant = await _context.Participants.FirstOrDefaultAsync(p => p.EventId == id && p.UserId == userId);

            if (participant == null)
                throw new ValidationException("User is not part of this event");



            _context.Participants.Remove(participant);

            await _context.SaveChangesAsync();

        }
    }
}
