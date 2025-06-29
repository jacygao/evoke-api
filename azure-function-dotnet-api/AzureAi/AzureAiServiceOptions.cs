namespace EvokeApi.AzureAi
{
    public class AzureAiServiceOptions
    {
        public string Endpoint { get; set; }
        public string ApiKey { get; set; }
        public string DeploymentName { get; set; }
        public string ModelVersion { get; set; }
        public string TargetLanguage { get; set; } = "Spanish";
        public string TranslatedLanguage { get; set; } = "English";
        public string SpeechKey { get; set; }
        public string SpeechEndpoint { get; set; }
        public string SpeechVoicename { get; set; } = "es-ES-ArabellaMultilingualNeural";
    }
}