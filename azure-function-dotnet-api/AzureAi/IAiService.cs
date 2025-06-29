namespace EvokeApi.AzureAi
{
    public interface IAiService
    {
        public Task<Completion> CompletionAsync(string sysMsg, string chatMsg);

        public Task<byte[]> SpeechAsync(string text);
    }
}
