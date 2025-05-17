using EvokeApi.AzureAi;
using EvokeApi.Database;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

var host = new HostBuilder()
 .ConfigureFunctionsWebApplication()
.ConfigureAppConfiguration((context, config) =>
{
    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
    config.AddEnvironmentVariables();
})
 .ConfigureServices((context, services) =>
 {
     services.AddApplicationInsightsTelemetryWorkerService();
     services.ConfigureFunctionsApplicationInsights();

     // Bind Azure AI Service options from appsettings or environment variables  
     services.Configure<AzureAiServiceOptions>(context.Configuration.GetSection("AzureAiServiceOptions"));

     services.AddSingleton<IAiService, AzureAiService>();

     // Load CosmosDB connection string from environment variables  
     services.Configure<AzureCosmosDbOptions>(context.Configuration.GetSection("AzureCosmosDbOptions"));

     // Register NotesDb with dependency on cosmosClient  
     services.AddSingleton<INotesDb>(provider =>
     {
         var options = provider.GetRequiredService<IOptions<AzureCosmosDbOptions>>().Value;
         if (string.IsNullOrEmpty(options.ConnectionString))
         {
             throw new InvalidOperationException("Azure Cosmos DB connection string is not configured. Please check your appsettings or environment variables.");
         }
         return new NotesDb(new CosmosClient(options.ConnectionString));
     });
 }).Build();

host.Run();