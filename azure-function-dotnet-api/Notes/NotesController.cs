using EvokeApi.AzureAi;
using EvokeApi.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace EvokeApi.Notes
{
    public class NotesController
    {
        private readonly ILogger<NotesController> _logger;
        private readonly INotesDb _notesDb;
        private static readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        private readonly IAiService _aiService;

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

            // Deserialize the request body to a Note object  
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Note? note;
            try
            {
                note = JsonSerializer.Deserialize<Note>(requestBody, _jsonSerializerOptions);
                if (note == null)
                {
                    return new BadRequestObjectResult("Invalid request body.");
                }

                note.Content = await _aiService.CompletionAsync(note.Content);

                await _notesDb.InsertAsync(note, note.UserId);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize request body.");
                return new BadRequestObjectResult("Invalid JSON format.");
            }

            return new OkResult();
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
    }
}
