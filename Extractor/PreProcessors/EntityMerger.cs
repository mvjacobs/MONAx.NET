using System.Linq;
using ikvm.extensions;
using Newtonsoft.Json.Linq;
using String = System.String;

namespace Extractor.PreProcessors
{
    class EntityMerger
    {
        public string MergeTextRazorWordsEntities(string textTextRazor)
        {
            dynamic json = JObject.Parse(textTextRazor);

            var sentences = json.response.sentences;
            var entities = (json.response.entities) as JArray;

            foreach (var sentence in sentences)
            {
                foreach (var word in sentence.words)
                {
                    if (entities == null) break;

                    var nerhash = entities.FirstOrDefault(e => e["matchingTokens"].Children().Contains((JValue)word.position));

                    if (nerhash == null) continue;

                    if (nerhash["type"] != null && nerhash["type"].Count() != 0)
                    {
                        word["dbpedia_types"] = nerhash["type"];
                    }

                    if (nerhash["freebaseTypes"] != null && nerhash["freebaseTypes"].Count() != 0)
                    {
                        word["freebase_types"] = nerhash["freebaseTypes"];
                    }

                    if (nerhash["freebaseId"] != null && !String.IsNullOrEmpty(nerhash["freebaseId"].toString()))
                    {
                        word["freebase_uri"] = "http://freebase.com" + nerhash["freebaseId"];
                    }

                    if (nerhash["wikiLink"] != null && !String.IsNullOrEmpty(nerhash["wikiLink"].ToString()))
                    {
                        word["dbpedia_uri"] = nerhash["wikiLink"].ToString().Replace("http://en.wikipedia.org/wiki/", "http://dbpedia.org/resource/");
                    }
                }
            }

            return json.ToString();
        }

        public string MergeTextRazorCoreNlp(string textTextRazor, string textCoreNlp)
        {
            dynamic jsonTextRazor = JObject.Parse(textTextRazor);
            var textRazorSentences = jsonTextRazor.response.sentences as JArray;

            dynamic jsonCoreNlp = JObject.Parse(textCoreNlp);
            var coreNlpSentences = jsonCoreNlp.sentences;

            foreach (var coreNlpSentence in coreNlpSentences)
            {
                foreach (var coreNlpword in coreNlpSentence.tokens)
                {
                    if (textRazorSentences == null) continue;

                    foreach (var textRazorSentence in textRazorSentences)
                    {
                        var textRazorWords = textRazorSentence["words"];

                        var textRazorToken =
                                textRazorWords.FirstOrDefault(
                                    t =>
                                        (int) t["startingPos"] >= (int)coreNlpword["begin"] &&
                                        (int) t["endingPos"] <= (int)coreNlpword["end"]);

                        if (textRazorToken == null) continue;

                        if (textRazorToken["stem"] != null) coreNlpword["stem"] = textRazorToken["stem"];
                        if (textRazorToken["dbpedia_types"] != null) coreNlpword["dbpedia_types"] = textRazorToken["dbpedia_types"];
                        if (textRazorToken["freebase_types"] != null) coreNlpword["freebase_types"] = textRazorToken["freebase_types"];
                        if (textRazorToken["dbpedia_uri"] != null) coreNlpword["dbpedia_uri"] = textRazorToken["dbpedia_uri"];
                        if (textRazorToken["freebase_uri"] != null) coreNlpword["freebase_uri"] = textRazorToken["freebase_uri"];
                    }
                }
            }

            return jsonCoreNlp.ToString();
        }
    }
}
