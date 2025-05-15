using Azure;
using Azure.AI.OpenAI;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Options;
using OpenAI.Chat;

namespace EvokeApi.AzureAi
{
    public class AzureAiService : IAiService
    {
        private readonly AzureOpenAIClient _azureClient;
        private readonly ChatClient _chatClient;
        private readonly AzureAiServiceOptions _options;
        public AzureAiService(IOptions<AzureAiServiceOptions> options)
        {
            _azureClient = new(
                new Uri(options.Value.Endpoint),
                new AzureKeyCredential(options.Value.ApiKey));

            _chatClient = _azureClient.GetChatClient(options.Value.DeploymentName);
            _options = options.Value;
        }

        public async Task<string> CompletionAsync(string chatMsg)
        {
            List<ChatMessage> messages = new List<ChatMessage>()
            {
            new SystemChatMessage(_options.SystemMessage),
            new UserChatMessage(chatMsg),
        };

            var resp = await _chatClient.CompleteChatAsync(messages);

            return resp.Value.Content[0].Text;
        }
    }
}