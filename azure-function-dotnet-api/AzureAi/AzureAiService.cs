using Azure;
using Azure.AI.OpenAI;
using Microsoft.CognitiveServices.Speech;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
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

        public async Task<Completion> CompletionAsync(string sysMsg, string chatMsg)
        {
            List<ChatMessage> messages = new List<ChatMessage>()
            {
            new SystemChatMessage(sysMsg),
            new UserChatMessage(chatMsg),
            };
            var resp = await _chatClient.CompleteChatAsync(messages);

            // Parse the JSON result
            var json = resp.Value.Content[0].Text;
            var result = JsonConvert.DeserializeObject<Completion>(json);

            if (result != null && result.Success)
            {
                if (!string.IsNullOrEmpty(result.Content) && !string.IsNullOrEmpty(result.Title))
                {
                    return result;
                }
                else
                {
                    throw new ArgumentException(result.Content ?? "Empty content/title returned from AI service.");
                }
            }
            else
            {
                var errorMsg = result?.Content ?? "Unknown error";
                throw new Exception($"AI service error: {errorMsg}");
            }
        }

        public async Task<byte[]> SpeechAsync(string text)
        {
            var config = SpeechConfig.FromEndpoint(new Uri(_options.SpeechEndpoint), _options.SpeechKey);
            config.SpeechSynthesisVoiceName = _options.SpeechVoicename;

            using var synthesizer = new SpeechSynthesizer(config);
            synthesizer.SynthesisCompleted += (s, e) => outputSpeechSynthesisResult(e.Result, text);
            using var result = await synthesizer.SpeakTextAsync(text);
            if (result.Reason == ResultReason.Canceled)
            {
                var cancellation = SpeechSynthesisCancellationDetails.FromResult(result);
                Console.WriteLine($"Speech synthesis canceled: {cancellation.ErrorDetails}");
            }
            
            return result.AudioData;
        }

        private void outputSpeechSynthesisResult(SpeechSynthesisResult speechSynthesisResult, string text)
        {
            switch (speechSynthesisResult.Reason)
            {
                case ResultReason.SynthesizingAudioCompleted:
                    Console.WriteLine($"Speech synthesized for text: [{text}]");
                    break;
                case ResultReason.Canceled:
                    var cancellation = SpeechSynthesisCancellationDetails.FromResult(speechSynthesisResult);
                    Console.WriteLine($"CANCELED: Reason={cancellation.Reason}");

                    if (cancellation.Reason == CancellationReason.Error)
                    {
                        Console.WriteLine($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                        Console.WriteLine($"CANCELED: ErrorDetails=[{cancellation.ErrorDetails}]");
                        Console.WriteLine($"CANCELED: Did you set the speech resource key and endpoint values?");
                    }
                    break;
                default:
                    break;
            }
        }
    }
}