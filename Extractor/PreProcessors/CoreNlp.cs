using System;
using System.Collections;
using System.IO;
using edu.stanford.nlp.ling;
using edu.stanford.nlp.pipeline;
using edu.stanford.nlp.semgraph;
using edu.stanford.nlp.time;
using edu.stanford.nlp.util;
using java.util;
using Newtonsoft.Json.Linq;
using ArrayList = java.util.ArrayList;

namespace Extractor.PreProcessors
{
    class CoreNlp
    {
        /// <summary>
        /// Natural Language Processing Pipeline
        /// </summary>
        public static StanfordCoreNLP Pipeline { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public CoreNlp()
        {
            // Annotation pipeline configuration
            var props = new Properties();
            props.setProperty("annotators", "tokenize, ssplit, pos, lemma, ner, parse, dcoref");
            props.setProperty("sutime.binders", "0");

            // We should change current directory, so StanfordCoreNLP could find all the model files automatically 
            var curDir = Environment.CurrentDirectory;
            Directory.SetCurrentDirectory(Config.StanfordNlpPath);
            Pipeline = new StanfordCoreNLP(props);
            Directory.SetCurrentDirectory(curDir);
        }

        /// <summary>
        /// Process a given text of an article to find annotations
        /// </summary>
        /// <param name="text">Source text of the article</param>
        /// <param name="creationDate">Publication date of the article</param>
        /// <returns></returns>
        public string Process(string text, DateTime creationDate)
        {
            // Annotation pipeline
            var annotation = new Annotation(text);
            annotation.set(new CoreAnnotations.DocDateAnnotation().getClass(), creationDate.ToString("yyyy-MM-dd"));
            Pipeline.annotate(annotation);

            // Process sentences into tokens and dependencies
            var sentences = annotation.get(new CoreAnnotations.SentencesAnnotation().getClass()) as ArrayList;

            return GetSentences(sentences).ToString();
        }

        /// <summary>
        /// Get tokens and dependencies from a list of sentences
        /// </summary>
        /// <param name="sentences">source sentences (list)</param>
        /// <returns>A list of annotated sentences</returns>
        private static JObject GetSentences(IEnumerable sentences)
        {
            dynamic nlpObject = new JObject();
            dynamic nlpSentences = new JArray();

            var key = 0;
            foreach (CoreMap sentence in sentences)
            {
                // Get all tokens from a sentence
                var tokens = GetTokens(sentence);
                if (tokens.Count == 0) continue;

                // Get all dependencies from a sentence
                var dependencies = GetDependencies(sentence);
                if (dependencies.Count == 0) continue;

                // Add tokens and dependencies to sentence
                dynamic nlpSentence = new JObject();
                nlpSentence.key = key;
                nlpSentence.tokens = tokens;
                nlpSentence.dependencies = dependencies;

                // Add sentence to sentences object
                nlpSentences.Add(nlpSentence);
                
                key++;
            }

            nlpObject.sentences = nlpSentences;

            return nlpObject;
        }

        /// <summary>
        /// Tokenize a sentence
        /// </summary>
        /// <param name="sentence">Source sentence</param>
        /// <returns>List of tokens</returns>
        private static JArray GetTokens(TypesafeMap sentence)
        {
            dynamic nlpTokens = new JArray();

            var tokens = sentence.get(new CoreAnnotations.TokensAnnotation().getClass()) as ArrayList;
            if (tokens == null || tokens.isEmpty()) return nlpTokens;

            foreach (CoreMap token in tokens)
            {
                dynamic nlpToken = new JObject();
                nlpToken.index = int.Parse(token.get(new CoreAnnotations.IndexAnnotation().getClass()).ToString());
                nlpToken.begin = int.Parse(token.get(new CoreAnnotations.CharacterOffsetBeginAnnotation().getClass()).ToString());
                nlpToken.end = int.Parse(token.get(new CoreAnnotations.CharacterOffsetEndAnnotation().getClass()).ToString());
                nlpToken.text = token.get(new CoreAnnotations.OriginalTextAnnotation().getClass()) as string;
                nlpToken.lemma = token.get(new CoreAnnotations.LemmaAnnotation().getClass()) as string;
                nlpToken.pos = token.get(new CoreAnnotations.PartOfSpeechAnnotation().getClass()) as string;
                nlpToken.stem = token.get(new CoreAnnotations.StemAnnotation().getClass()) as string;
                nlpToken.named_entity_tag = token.get(new CoreAnnotations.NamedEntityTagAnnotation().getClass()) as string != "O" ? 
                    token.get(new CoreAnnotations.NamedEntityTagAnnotation().getClass()) as string : null;
                nlpToken.normalized_named_entity_tag = token.get(new CoreAnnotations.NormalizedNamedEntityTagAnnotation().getClass()) as string;
                nlpToken.timex = token.get(new TimeAnnotations.TimexAnnotation().getClass()) != null ? token.get(new TimeAnnotations.TimexAnnotation().getClass()).ToString() : null;

                nlpTokens.Add(nlpToken);
            }

            return nlpTokens;
        }

        /// <summary>
        /// Find dependencies between words in sentence
        /// </summary>
        /// <param name="sentence">Source sentence</param>
        /// <returns>List of dependencies</returns>
        private static JArray GetDependencies(TypesafeMap sentence)
        {
            dynamic nlpDependencies = new JArray();

            var dependencies = sentence.get(
                    new SemanticGraphCoreAnnotations.CollapsedCCProcessedDependenciesAnnotation().getClass()) as
                    SemanticGraph;
            if (dependencies == null || dependencies.isEmpty()) return nlpDependencies;
            
            var dependenciesArray = dependencies.edgeListSorted().toArray();
            foreach (SemanticGraphEdge dependency in dependenciesArray)
            {
                dynamic nlpDependency = new JObject();

                nlpDependency.type = dependency.getRelation().toString();
                nlpDependency.governor = dependency.getGovernor().index();
                nlpDependency.dependent = dependency.getDependent().index();

                nlpDependencies.Add(nlpDependency);
            }

            return nlpDependencies;
        }
    }
}
