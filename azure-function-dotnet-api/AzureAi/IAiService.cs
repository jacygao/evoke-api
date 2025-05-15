namespace EvokeApi.AzureAi
{
    public interface IAiService
    {
        public Task<string> CompletionAsync(string chatMsg);
    }
}
