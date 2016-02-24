using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace InOutEmote.Utils
{
    class WordsVariants
    {
        private string wordsVariantsFilePath = "wordsVariants.txt";
        private List<WordVariantsEntry> _wordVariants = new List<WordVariantsEntry>();

        private class WordVariantsEntry : Utils.JsonSerializable
        {
            public string OriginalWord
            {
                get; set;
            }
            public List<string> Variants
            {
                get; set;
            }
        }

        public static void Main(string[] args)
        {
            //_wordVariants = new List<WordVariantsEntry>()
            //{
            //    new WordVariantsEntry() { OriginalWord = "Test", Variants = new List<string>() { "Testen", "en test" } },
            //    new WordVariantsEntry() { OriginalWord = "Prova", Variants = new List<string>() { "Proven", "en prove" } }
            //};
            //using (TextWriter writer = new StreamWriter(wordsVariantsFilePath))
            //{
            //    foreach(var variant in _wordVariants)
            //    {
            //        writer.WriteLine(variant.SerializeToJson());
            //    }
            //}
            //_wordVariants.Clear();

            WordsVariants test = new WordsVariants();
            test.LoadWordsVariants();
            var tagsAndValues = new Dictionary<string, string>();
            test.AddWordsVariants(ref tagsAndValues);
        }

        public WordsVariants()
        {
            LoadWordsVariants();
            if (File.Exists(debugFilePath)) File.Delete(debugFilePath);
        }

        private void LoadWordsVariants()
        {
            if (File.Exists(wordsVariantsFilePath))
            {
                Debug("Loading WordVariants");
                using (StreamReader reader = new System.IO.StreamReader(wordsVariantsFilePath))
                {
                    string line = reader.ReadLine();
                    while (line != null && line != "")
                    {
                        var variant = Utils.JsonSerializable.DeserializeFromJson<WordVariantsEntry>(line);
                        _wordVariants.Add(variant);
                        line = reader.ReadLine();
                        Debug(variant.OriginalWord+": "+variant.Variants[0]+", "+variant.Variants[1]);
                    }
                }
            }
            else
            {
                Debug("Couldn't find variants file at: "+wordsVariantsFilePath);
            }
            Debug("Finished Loading\n\n\n");
        }

        /// <summary>
        /// Used to add variants of some words that are not known to Enercities to the tags and values' dictionary used by skene to replace tags in the utterances.
        /// This is used mainly for the Swedish translation. In swedish there are suffixes and prefixes that must be add to words relatively to their position in the phrase,
        /// so here we add some variants for some words that can be used in the utterances
        /// </summary>
        /// <returns>A list of tags and values to be used for Skene utterances</returns>
        public void AddWordsVariants(ref Dictionary<string, string> tagsAndValues)
        {
            Debug("Looking for variants in tags and values: ");
            foreach (var wv in _wordVariants)
            {
                int i = 0;
                Debug("Checking presence of word " + wv.OriginalWord +" in tagsAndValues: ");
                string debugString ="";
                foreach(var tagandval in tagsAndValues)
                {
                    debugString = debugString+"; "+tagandval.Key + ": " + tagandval.Value;
                }
                Debug(debugString);
                var matchingTagAndValue = tagsAndValues.Where(x => x.Value!=null?x.Value.ToLowerInvariant() == wv.OriginalWord.ToLowerInvariant():false);
                if (matchingTagAndValue.Any())                           
                {
                    string tagForThisValue = matchingTagAndValue.First().Key.Replace("/", "");          // The tag without the slashes
                    Debug("Word" + wv.OriginalWord+" FOUND bound to tag "+tagForThisValue);
                    foreach (string variant in wv.Variants)
                    {
                        i++;
                        Debug("Adding " + variant + " value for /" + tagForThisValue + "_variant" + i + "/ tag");
                        tagsAndValues.Add("/" + tagForThisValue + "_variant" + i + "/", variant);
                    }
                } else
                {
                    Debug(wv.OriginalWord+" NOT found");
                }
            }
            Debug("Finished looking for variants.\n\n\n");
        }


        private string debugFilePath = "wordsVariantsDebug.log";
        private void Debug(string message)
        {
            using (StreamWriter writer = new StreamWriter(debugFilePath, true))
            {
                writer.WriteLine(DateTime.Now.TimeOfDay + " " + message);
            }
        }

    }
}
