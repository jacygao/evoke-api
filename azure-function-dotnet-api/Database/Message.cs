using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace EvokeApi.Database
{
    public class Message
    {
        [JsonProperty("id")]
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonProperty("userId")]
        [JsonPropertyName("userId")]
        public string UserId { get; set; }

        [JsonProperty("content")]
        [JsonPropertyName("content")]
        public string Content { get; set; }
    }
}
