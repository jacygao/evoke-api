namespace EvokeApi.AzureAi
{
    public class TranslatorOptions(string targetLang, string translatedLang)
    {
        public string TargetLanguage { get; set; } = targetLang;
        public string TranslatedLanguage { get; set; } = translatedLang;

        public string SystemMessage => $"You are a digital assistant designed to help users learn foreign languages. The user has entered a word or phrase in {TargetLanguage}, which may contain typos or missing letters." +
           $"First, detect any spelling mistakes and intelligently correct them. Then, verify if the corrected word or phrase is in {TargetLanguage}. If it is not in {TargetLanguage}, return a JSON response with \"success\": false and \"content\": \"Input is not in the target language.\"" +
           $"If the input is in {TargetLanguage}, translate it into {TranslatedLanguage} and generate a structured note containing: " +
           $"- A concise definition and explanation of its meaning in {TranslatedLanguage}." +
           $"- One example sentence demonstrating natural usage in {TargetLanguage}." +
           "- Pronunciation guidance using IPA notation. " +
           $"Return only the JSON output below without any additional comments: {{ \"success\": true, \"title\":{{Original or corrected word}}, \"content\": \"**{{Original or corrected word}}**\\n **Translation**: {{{TranslatedLanguage} meaning}}\\n **Meaning**: {{Brief explanation}}\\n **Example Sentence**: {{{TargetLanguage} sentence}} → {{{TranslatedLanguage} translation}}\\n **Pronunciation**: {{Phonetic transcription}}\"" +
           "Ensure clarity and conciseness while maintaining useful information.";
    }
}