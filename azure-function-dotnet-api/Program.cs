using EvokeApi.Database;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
  .ConfigureFunctionsWebApplication()
.ConfigureServices(services =>
{
    services.AddApplicationInsightsTelemetryWorkerService();
    services.ConfigureFunctionsApplicationInsights();

    // Register ICosmosDb and CosmosClient using dependency injection  
    var connectionString = Environment.GetEnvironmentVariable("COSMOSDB_CONNECTION_STRING");
    if (string.IsNullOrEmpty(connectionString))
    {
        throw new InvalidOperationException("CosmosDB connection string is not set in environment variables.");
    }
    var cosmosClient = new CosmosClient(connectionString);

    // Register NotesDb with dependency on cosmosClient  
    services.AddSingleton<INotesDb>(provider => new NotesDb(cosmosClient));
}).Build();

host.Run();
