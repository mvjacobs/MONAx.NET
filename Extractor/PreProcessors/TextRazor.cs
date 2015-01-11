using RestSharp;

namespace Extractor.PreProcessors
{
    public class TextRazor
    {
        private string ApiKey { get; set; }

        public TextRazor(string apiKey)
        {
            ApiKey = apiKey;
        }

        public string GetExtractedText(string extractors, string text)
        {
            var client = new RestClient("https://api.textrazor.com");

            var request = new RestRequest(Method.POST);

            request.AddParameter("apiKey", ApiKey); // adds to POST or URL querystring based on Method
            request.AddParameter("extractors", extractors);
            request.AddParameter("text", text);

            var response = client.Execute(request);
            return response.Content;
        }
    }
}
