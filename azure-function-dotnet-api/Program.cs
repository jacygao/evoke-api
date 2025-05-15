using EvokeApi.Database;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using EvokeApi.AzureAi;
using Microsoft.Extensions.Options;

var host = new HostBuilder()
 .ConfigureFunctionsWebApplication()
 .ConfigureAppConfiguration((context, config) =>
 {
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
     services.AddSingleton<INotesDb>(provider => {
         var options = provider.GetRequiredService<IOptions<AzureCosmosDbOptions>>().Value;
         return new NotesDb(new CosmosClient(options.ConnectionString));
         });
 }).Build();

host.Run();