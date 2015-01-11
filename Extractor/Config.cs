namespace Extractor
{
    static class Config
    {
        /// <summary>
        /// URL to the source of a news article
        /// (must contain a publishing date in META tag or URL)
        /// </summary>
        public static string SourceUri
        {
            get { return "http://www.nytimes.com/2015/01/10/world/europe/charlie-hebdo-paris-shooting.html?hp&action=click&pgtype=Homepage&module=span-ab-top-region&region=top-news&WT.nav=top-news"; }
        }

        /// <summary>
        /// API key for AlchemyAPI
        /// http://www.alchemyapi.com
        /// </summary>
        public static string AlchemyApiKey
        {
            get { return "1ad2c7f540e986878a2d825efda2d933f7ce61b9"; }
        }

        /// <summary>
        /// API key for Textrazor
        /// http://www.textrazor.com
        /// </summary>
        public static string TextRazorApiKey
        {
            get { return "e569a939399816b2717230d7ef0a01d21c7f58973f0dcc96f6fdd9ed"; }
        }

        /// <summary>
        /// Local path to the Stanford Core NLP models
        /// http://nlp.stanford.edu/software/corenlp.shtml
        /// </summary>
        public static string StanfordNlpPath
        {
            get  { return @"c:\Projects\stanford-corenlp-3.5.0-models\"; }
        }

        /// <summary>
        /// Local path for the JSON output file
        /// </summary>
        public static string JsonFile
        {
            get { return @"c:\entities.json"; }
        }
    }
}
