using RestSharp;

namespace Extractor.PreProcessors
{
    class AlchemyApi
    {
        public RestClient Client = new RestClient("http://access.alchemyapi.com/calls");

        public string ApiKey { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="apiKey">Key to access the Alchemy API</param>
        public AlchemyApi(string apiKey)
        {
            ApiKey = apiKey;
        }

        public string GetAuthorUrl(string url)
        {
            var request = new RestRequest("/url/URLGetAuthor", Method.GET);

            request.AddParameter("apikey", ApiKey); // adds to POST or URL querystring based on Method
            request.AddParameter("outputMode", "json");
            request.AddParameter("url", url);

            var response = Client.Execute(request);
            return response.Content;
        }

        public string GetAuthorHtml(string html)
        {
            var request = new RestRequest("/html/HTMLGetAuthor", Method.POST);

            request.AddParameter("apikey", ApiKey); // adds to POST or URL querystring based on Method
            request.AddParameter("outputMode", "json");
            request.AddParameter("html", html);

            var response = Client.Execute(request);
            return response.Content;
        }

        public string GetTextUrl(string url)
        {
            var request = new RestRequest("/url/URLGetText", Method.POST);

            request.AddParameter("apikey", ApiKey); // adds to POST or URL querystring based on Method
            request.AddParameter("outputMode", "json");
            request.AddParameter("url", url);

            var response = Client.Execute(request);
            return response.Content;
        }

        public string GetTextHtml(string html)
        {
            var request = new RestRequest("/html/HTMLGetText", Method.POST);

            request.AddParameter("apikey", ApiKey); // adds to POST or URL querystring based on Method
            request.AddParameter("outputMode", "json");
            request.AddParameter("html", html);

            var response = Client.Execute(request);
            return response.Content;
        }
    }
}
