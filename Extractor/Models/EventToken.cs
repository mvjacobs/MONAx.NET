using Newtonsoft.Json.Linq;

namespace Extractor.Models
{
    class EventToken
    {
        public int? Index { get; set; }

        public int? Begin { get; set; }

        public int? End { get; set; }

        public string Text { get; set; }

        public string Lemma { get; set; }

		public string Pos  { get; set; }

        public string Stem { get; set; }

        public string NamedEntityTag { get; set; }

        public string NormalizedNamedEntityTag { get; set; }

        public string Timex { get; set; }

        public JArray DbPediaTypes { get; set; }

        public JArray FreeBaseTypes { get; set; }

        public string DbPediaUri { get; set; }

        public string FreeBaseUri { get; set; }
    }
}
