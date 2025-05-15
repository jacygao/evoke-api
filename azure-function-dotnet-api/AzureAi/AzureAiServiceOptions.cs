namespace EvokeApi.AzureAi
{
    public class AzureAiServiceOptions
    {
        public string Endpoint { get; set; }
        public string ApiKey { get; set; }
        public string DeploymentName { get; set; }
        public string ModelVersion { get; set; }

        public string SystemMessage { get; set; } = "You are a note taking assistant. You will help the user to complete their notes based on the partial information given. responses should be concise and clear. the completed notes should be created in a way to help users memorize important content.";
    }
}