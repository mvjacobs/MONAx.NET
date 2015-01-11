using System.IO;

namespace Extractor.PreProcessors
{
    static class PreProcessor
    {
        public static AlchemyApi AlchemyApi = new AlchemyApi(Config.AlchemyApiKey);

        public static void PreProcess(string textUrl, string textBody)
        {
            var textDate = PubDateFinder.GetDateUrl(textUrl);

            var coreNlp = new CoreNlp();

            var textCoreNlp = coreNlp.Process(textBody, textDate);

            var textRazor = new TextRazor(Config.TextRazorApiKey);

            dynamic jsonTextRazor = textRazor.GetExtractedText("entities,words,dependency-trees", textBody);
       
            var entityMerger = new EntityMerger();

            var textMergedWithEntities = entityMerger.MergeTextRazorWordsEntities(jsonTextRazor);

            var textMergedWithCoreNlp = entityMerger.MergeTextRazorCoreNlp(textMergedWithEntities, textCoreNlp);

            File.WriteAllText(Config.JsonFile, textMergedWithCoreNlp);
        }
    }
}
