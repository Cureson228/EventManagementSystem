using EventManagementSystemApi.Models;
using EventManagementSystemApi.Models.DTOs;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
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
                CreatedByUserId = userId,
                EventTags = new List<EventTag>()
                
            };


            if (dto.Tags != null && dto.Tags.Length > 0)
            {
                var normalizedTags = dto.Tags
                    .Where(t => !string.IsNullOrEmpty(t))
                    .Select(t => t.Trim().ToLower())
                    .Distinct()
                    .ToArray();

                foreach(var normalizedTag in normalizedTags)
                {
                    var existingTag = await _context.Tags
                        .FirstOrDefaultAsync(t => t.Name.ToLower() == normalizedTag);

                    Tag tagEntity;

                    if (existingTag != null)
                    {
                        tagEntity = existingTag;
                    }
                    else
                    {
                        tagEntity = new Tag() { Name = normalizedTag };
                        _context.Tags.Add(tagEntity);
                        await _context.SaveChangesAsync();
                    }
                    _context.EventsTags.Add(new EventTag
                    {
                        Event = newEvent,
                        TagId = tagEntity.Id,
                    });
                   
                }
                 
            }


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
        public async Task<string[]> GetAllTagsAsync()
        {
            return await _context.Tags
                .Select(t => char.ToUpper(t.Name[0]) + t.Name.Substring(1).ToLower())
                .ToArrayAsync();
        }
        public async Task<string[]> GetEventTagsAsync(int id)
        {
            return await _context.EventsTags.Where(et => et.EventId == id)
                .Select(et => char.ToUpper(et.Tag.Name[0]) + et.Tag.Name.Substring(1).ToLower()).ToArrayAsync();  
        }

        public async Task<IEnumerable<EventDto>> GetPublicEventsAsync(string[]? Tags = null)
        {

            var query = _context.Events
                .AsNoTracking()
                .Include(e => e.Participants)
                .Include(e => e.EventTags)
                    .ThenInclude(et => et.Tag)
                .Where(e => e.Visibility)
                .AsQueryable();


            if (Tags != null && Tags.Length > 0)
            {
                var toLowerTags = Tags.Select(t => t.ToLower()).ToArray();

                query = query.Where(e => toLowerTags.All(tag => e.EventTags.Any(et => et.Tag.Name.ToLower() == tag)));
            }

            return await query
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

                    }).ToList(),
                    EventTags = e.EventTags.Select(et => et.Tag.Name).ToList()
                    
                }).FirstAsync();

            if (eventDetails == null)
                throw new KeyNotFoundException($"Event with ID {id} was not found");


            return eventDetails;
            
        }
        public async Task EditEventAsync(CreateEventDto dto, int id)
        {
            var _event = await _context.Events.Include(e => e.EventTags).ThenInclude(et => et.Tag).FirstOrDefaultAsync(e => e.Id == id);
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
            try
            {
                var existingTags = _event.EventTags.Select(et => et.Tag.Name.ToLower()).ToList();
                var newTags = dto.Tags?.Select(t => t.ToLower()).ToList() ?? new List<string>();

                var tagsToRemove = _event.EventTags.Where(et => !newTags.Contains(et.Tag.Name.ToLower())).ToList();

                foreach (var et in tagsToRemove)
                    _event.EventTags.Remove(et);

                var tagsToAdd = newTags.Except(existingTags).ToList();
                foreach (var tagName in tagsToAdd)
                {
                    var tagEntity = await _context.Tags
                        .FirstOrDefaultAsync(t => t.Name.ToLower() == tagName);

                    if (tagEntity == null)
                    {
                        tagEntity = new Tag { Name = tagName };
                        _context.Tags.Add(tagEntity);
                        await _context.SaveChangesAsync(); 
                    }

                    _event.EventTags.Add(new EventTag
                    {
                        Event = _event,
                        TagId = tagEntity.Id
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }

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
                    Participants = e.Participants.Select(p => new ParticipantDto
                    {
                        EventId = e.Id,
                        UserId = p.UserId,
                        JoinedAt = p.JoinedAt,
                        FullName = p.FullName

                    }).ToList(),
                    EventTags =  e.EventTags
                    .Select(et => char.ToUpper(et.Tag.Name[0]) + et.Tag.Name.Substring(1).ToLower()).ToList()

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
