using EvokeApi.AzureAi;
using EvokeApi.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;

namespace EvokeApi.Notes
{
    public class NotesController
    {
        private readonly ILogger<NotesController> _logger;
        private readonly INotesDb _notesDb;
        private static readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        private readonly IAiService _aiService;

        private readonly string _userIdHeader = "X-USER-AUTH";

        public NotesController(ILogger<NotesController> logger, INotesDb noteDb, IAiService aiService)
        {
            _logger = logger;
            _notesDb = noteDb;
            _aiService = aiService;
        }

        [Function("CreateNotes")]
        public async Task<IActionResult> CreateNotes([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "notes")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            // Retrieve userId from request headers  
            if (!req.Headers.TryGetValue(_userIdHeader, out var jwt))
            {
                return new BadRequestObjectResult("Missing or invalid 'userId' header.");
            }

            if (string.IsNullOrEmpty(jwt))
            {
                return new BadRequestObjectResult("Missing or invalid 'userId' header.");
            }

            // Deserialize the request body to a Note object  
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Note? note;
            try
            {
                note = JsonSerializer.Deserialize<Note>(requestBody, _jsonSerializerOptions);
                if (note == null || string.IsNullOrEmpty(note.Content))
                {
                    return new BadRequestObjectResult("Invalid request body.");
                }

                note.UserId = GetUserEmail(jwt!);

                await _notesDb.InsertAsync(note, note.UserId);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize request body.");
                return new BadRequestObjectResult("Invalid JSON format.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating note.");
                return new BadRequestObjectResult(StatusCodes.Status500InternalServerError);
            }
            return new OkObjectResult(new {});
        }

        [Function("GetNoteById")]
        public async Task<IActionResult> GetNoteById([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "notes/{id}")] HttpRequest req, string id)
        {
            _logger.LogInformation("Processing request to get note by ID: {Id}", id);

            try
            {
                var note = await _notesDb.GetByIdAsync(id, id);
                if (note == null)
                {
                    return new NotFoundObjectResult($"Note with ID {id} not found.");
                }

                return new OkObjectResult(note);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving note with ID: {Id}", id);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [Function("ListNotes")]
        public async Task<IActionResult> ListNotes([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "notes")] HttpRequest req)
        {
            _logger.LogInformation("Processing request to list notes.");

            // Retrieve userId from request headers  
            if (!req.Headers.TryGetValue(_userIdHeader, out var jwt))
            {
                return new BadRequestObjectResult("Missing or invalid 'userId' header.");
            }

            if (string.IsNullOrEmpty(jwt)) {
                return new BadRequestObjectResult("Missing or invalid 'userId' header.");
            }

            var userId = GetUserEmail(jwt!);

            try
            {
                var notes = await _notesDb.GetAll(userId);
                return new OkObjectResult(notes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving notes for userId: {UserId}", userId!);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        private string GetUserEmail(string jwtToken)
        {
            // Decode JWT token to extract email  
            string? userId;
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(jwtToken);
                userId = token.Claims.FirstOrDefault(c => c.Type == "email")?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    throw new ArgumentException("Email claim not found in JWT token.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to decode JWT token.");
                throw;
            }

            _logger.LogInformation("User ID (email): {UserId}", userId);
            return userId;
        }

        [Function("CompleteNote")]
        public async Task<IActionResult> CompleteNote([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "notes/complete")] HttpRequest req)
        {
            _logger.LogInformation("Processing request to complete a note.");

            // Retrieve userId from request headers
            if (!req.Headers.TryGetValue(_userIdHeader, out var jwt))
            {
                return new BadRequestObjectResult("Missing or invalid 'userId' header.");
            }

            if (string.IsNullOrEmpty(jwt))
            {
                return new BadRequestObjectResult("Missing or invalid 'userId' header.");
            }

            var translator = new TranslatorOptions("Spanish", "English");

            // Deserialize the request body to extract the user message
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Message? userMessage;
            try
            {
                userMessage = JsonSerializer.Deserialize<Message>(requestBody, _jsonSerializerOptions);
                if (userMessage == null)
                {
                    return new BadRequestObjectResult("Invalid request body.");
                }

                // Pass the user message to the AI service for completion
                var completionResult = await _aiService.CompletionAsync(translator.SystemMessage, userMessage.Content);

                var note = new Note();
                note.Content = completionResult.Content;
                note.Title = completionResult.Title;
                // Return the AI service response
                return new OkObjectResult(note);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize request body.");
                return new BadRequestObjectResult("Invalid JSON format.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing the completion request.");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [Function("ReadNotes")]
        public async Task<IActionResult> ReadNotes([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "notes/read")] HttpRequest req, string id)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            // Retrieve userId from request headers  
            if (!req.Headers.TryGetValue(_userIdHeader, out var jwt))
            {
                return new BadRequestObjectResult("Missing or invalid 'userId' header.");
            }

            if (string.IsNullOrEmpty(jwt))
            {
                return new BadRequestObjectResult("Missing or invalid 'userId' header.");
            }

            // Deserialize the request body to a Note object  
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Note? note;
            try
            {
                note = JsonSerializer.Deserialize<Note>(requestBody, _jsonSerializerOptions);
                if (note == null || string.IsNullOrEmpty(note.Content) || string.IsNullOrEmpty(note.Title))
                {
                    return new BadRequestObjectResult("Invalid request.");
                }

                // Pass the user message to the AI service for completion

                var audio = await _aiService.SpeechAsync(note.Title);
                if (audio == null || audio.Length == 0)
                {
                    return new BadRequestObjectResult("Failed to generate speech for the note.");
                }

                // Return the audio content as a FileContentResult
                return new FileContentResult(audio, "audio/mpeg")
                {
                    FileDownloadName = "note_audio.mp3"
                };
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize request body.");
                return new BadRequestObjectResult("Invalid JSON format.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while reading note.");
                return new BadRequestObjectResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
