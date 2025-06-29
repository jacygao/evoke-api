using Newtonsoft.Json;

namespace EvokeApi.AzureAi
{
    public class Completion
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }
    }
}