using Microsoft.Azure.Cosmos;

namespace EvokeApi.Database
{
    public class NotesDb : INotesDb
    {
        private readonly string _databaseName = "studybox";
        private readonly string _containerName = "notes";

        private readonly CosmosClient _cosmosClient;
        private readonly Container _container;

        public NotesDb(CosmosClient cosmosClient)
        {
            _cosmosClient = cosmosClient;
            _container = _cosmosClient.GetContainer(_databaseName, _containerName);
        }

        public async Task InsertAsync(Note note, string partitionKey)
        {
            note.Id = Guid.NewGuid().ToString();
            
            try
            {
                await _container.CreateItemAsync(note, new PartitionKey(partitionKey));
            }
            catch (CosmosException ex)
            {
                throw ex;
            }
        }

        public async Task<Note> GetByIdAsync(string id, string partitionKey)
        {
            try
            {
                ItemResponse<Note> response = await _container.ReadItemAsync<Note>(id.ToString(), new PartitionKey(partitionKey));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return default;
            }
        }

        public async Task<IEnumerable<Note>> GetAll(string userId)
        {
            var queryDefinition = new QueryDefinition("SELECT * FROM c WHERE c.UserId = @userId")
                .WithParameter("@userId", userId);
            var queryIterator = _container.GetItemQueryIterator<Note>(queryDefinition);
            var results = new List<Note>();

            while (queryIterator.HasMoreResults)
            {
                var response = await queryIterator.ReadNextAsync();
                results.AddRange(response);
            }

            return results;
        }
    }
}
