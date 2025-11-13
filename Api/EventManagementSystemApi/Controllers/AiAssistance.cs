using EventManagementSystemApi.Models;
using EventManagementSystemApi.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;


namespace EventManagementSystemApi.Controllers
{
    [ApiController]
    [Route("/api/assistance")]
    public class AiAssistance : ControllerBase
    {

        private readonly IEventService _eventService;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IConfiguration _configuration;
        public AiAssistance(
            IEventService eventService,
            IHttpContextAccessor contextAccessor,
            IConfiguration configuration)
        {
            _eventService = eventService;
            _contextAccessor = contextAccessor;
            _configuration = configuration;
        }

        public record AskRequest(string Question);

        [HttpPost("ask")]
        public async Task<IActionResult> Ask([FromBody] AskRequest askRequest)
        {
            var userId = _contextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            var userEvents = _eventService.GetUserEventsAsync();

            var prompt = $@"
            You are an AI assistant that answers questions about calendar events.
            You can only use the provided event data.
            
            User question:
            ${askRequest.Question}

            User events data:

            {JsonSerializer.Serialize(userEvents)}
            Answer concisely and clearly based on the given data only.
            If the question cannot be answered from the data, say:
            'Sorry, I didn’t understand that. Please try rephrasing your question.'";

            var client = new HttpClient();

            if (string.IsNullOrEmpty(askRequest.Question))
                return BadRequest(new { answer = "Please ask a valid question" });

            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _configuration["GROQ_API_KEY"]);

            var body = new
            {
                model = "llama-3.3-70b-versatile", 
                messages = new[]
                {
                new { role = "system", content = "You are an AI assistant that answers questions about events." },
                new { role = "user", content = prompt }
                }
            };

            var response = await client.PostAsync(
                "https://api.groq.com/openai/v1/chat/completions",
                new StringContent(JsonSerializer.Serialize(body), System.Text.Encoding.UTF8, "application/json")
            );

            var json = await response.Content.ReadAsStringAsync();
            var parsed = JsonDocument.Parse(json);
            var answer = parsed.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            return Ok(new { answer });
        }
    }
}
