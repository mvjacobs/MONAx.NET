using Extractor.PreProcessors;
using Newtonsoft.Json.Linq;

namespace Extractor
{
    class Program
    {
        public static AlchemyApi AlchemyApi = new AlchemyApi(Config.AlchemyApiKey);

        static void Main()
        {
            dynamic jsonText = JObject.Parse(AlchemyApi.GetTextUrl(Config.SourceUri));

            var text = jsonText.text.ToString();

            PreProcessor.PreProcess(Config.SourceUri, text);
        }
    }
}
