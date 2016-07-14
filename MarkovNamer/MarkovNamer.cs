using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace April32
{
    /// <summary>
    /// Implementation of Markov Chains algorithm for unique words generation in a given language
    /// </summary>
    [Serializable]
    public class MarkovNamer
    {
        public static string Vowels = "aeiouyаеёиоуэюяéà";

        private readonly List<WordPart> _begins = new List<WordPart>();
        private readonly Dictionary<string, List<WordPart>> _middles = new Dictionary<string, List<WordPart>>();
        private readonly Dictionary<string, List<WordPart>> _ends = new Dictionary<string, List<WordPart>>();

        /// <summary>
        /// Saves binary state of a MarkovNamer to a file
        /// </summary>
        /// <param name="lb"></param>
        /// <param name="filePath"></param>
        public static void SaveBinary(MarkovNamer lb, string filePath)
        {
            var binaryFormatter = new BinaryFormatter();

            using (var fs = File.OpenWrite(filePath))
            using (var gzip = new GZipStream(fs, CompressionMode.Compress))
            {
                binaryFormatter.Serialize(gzip, lb);
            }
        }

        /// <summary>
        /// Loads binary state of a MarkovNamer
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static MarkovNamer LoadBinary(string filePath)
        {
            MarkovNamer lb;
            var binaryFormatter = new BinaryFormatter();
            using (var fs = File.OpenRead(filePath))
            using (var gzip = new GZipStream(fs, CompressionMode.Decompress))
            {
                lb = (MarkovNamer)binaryFormatter.Deserialize(gzip);
            }

            return lb;
        }

        /// <summary>
        /// Creates a MarkovNamer from a WordParts collection (used internally)
        /// </summary>
        /// <param name="wordsParts"></param>
        public MarkovNamer(IEnumerable<WordPart> wordsParts)
        {
            foreach (var wordsPart in wordsParts)
            {
                if (wordsPart.IsBeginning)
                    _begins.Add(wordsPart);

                if (!wordsPart.IsBeginning && !wordsPart.IsEnding)
                {
                    List<WordPart> list;
                    if (!_middles.TryGetValue(wordsPart.Vow1, out list))
                        _middles.Add(wordsPart.Vow1, new List<WordPart>(new[] { wordsPart }));
                    else
                    {
                        list.Add(wordsPart);
                    }
                }

                if (wordsPart.IsEnding)
                {
                    List<WordPart> list;
                    if (!_ends.TryGetValue(wordsPart.Vow1, out list))
                        _ends.Add(wordsPart.Vow1, new List<WordPart>(new[] { wordsPart }));
                    else
                    {
                        list.Add(wordsPart);
                    }
                }
            }
        }

        /// <summary>
        /// Creates a new instance of MarkovNamer from existing words list file
        /// </summary>
        /// <param name="dictionaryPath"></param>
        /// <returns></returns>
        public static MarkovNamer CreateFromDictionary(string dictionaryPath)
        {
            var wordsParts = new HashSet<WordPart>();

            using (var fs = File.OpenRead(dictionaryPath))
            using (var reader = new StreamReader(fs))
            {
                while (!reader.EndOfStream)
                {
                    var word = reader.ReadLine().ToLower();

                    for (int i = 0; i < word.Length; i++)
                    {
                        if (!char.IsLetter(word[i]))
                        {
                            word = word.Substring(0, i);
                            break;
                        }
                    }

                    if (string.IsNullOrEmpty(word))
                        continue;

                    var list = new List<WordPart>();

                    var pos = 0;
                    while (true)
                    {
                        var wordPart = TakeWordPartAt(word, pos);

                        list.Add(wordPart);
                        if (!wordPart.IsEnding)
                            pos += wordPart.Length - wordPart.Vow2.Length;

                        if (wordPart.IsEnding)
                            break;
                    }

                    foreach (var wordPart in list)
                    {
                        if (!wordsParts.Contains(wordPart))
                            wordsParts.Add(wordPart);
                    }
                }
            }

            return new MarkovNamer(wordsParts);
        }

        private static WordPart TakeWordPartAt(string word, int pos)
        {
            var wp = new WordPart();

            if (pos == 0)
                wp.IsBeginning = true;

            for (var i = pos; i < word.Length; i++)
            {
                var lastLetter = i == word.Length - 1;

                var c = word[i];

                var vowel = IsVowel(c);

                if (vowel)
                {
                    if (wp.Mid == null)
                    {
                        wp.Vow1 += c;
                        if (lastLetter)
                            wp.IsEnding = true;
                    }
                    else
                    {
                        wp.Vow2 += c;
                        if (lastLetter)
                            wp.IsEnding = true;
                    }
                }
                else
                {
                    if (wp.Vow2 == null)
                    {
                        if (wp.Vow1 == null)
                            wp.Vow1 = "";

                        wp.Mid += c;
                        if (lastLetter)
                            wp.IsEnding = true;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return wp;
        }
        
        /// <summary>
        /// Checks if a letter is a vowel, use Vowels static property to change behaviour
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool IsVowel(char c)
        {
            return Vowels.Contains(c);
        }

        /// <summary>
        /// Generate a random word from 2 parts
        /// </summary>
        /// <returns></returns>
        public string GenWord()
        {
            return GenWord(2, new Random());
        }

        /// <summary>
        /// Generate a random word from 
        /// </summary>
        /// <param name="partsCount"></param>
        /// <returns></returns>
        public string GenWord(int partsCount)
        {
            return GenWord(partsCount, new Random());
        }

        /// <summary>
        /// Generates a random word
        /// </summary>
        /// <param name="partsCount">How many parts to take during word generation</param>
        /// <param name="r">Random instance to use during name generation</param>
        /// <returns></returns>
        public string GenWord(int partsCount, Random r)
        {
            if (partsCount < 2)
                throw new ArgumentOutOfRangeException(nameof(partsCount), "Use at least 2 parts to generate a word");
            
            var parts = new List<WordPart>();

            while (parts.Count < partsCount - 1)
            {
                var beg = _begins[r.Next(0, _begins.Count)];
                parts.Add(beg);

                for (int i = 0; i < partsCount - 2; i++)
                {
                    List<WordPart> possibleParts;

                    var last = parts.Last();

                    if (last.Vow2 == null)
                    {
                        parts.Clear();
                        break;
                    }

                    if (!_middles.TryGetValue(last.Vow2, out possibleParts))
                    {
                        parts.Clear();
                        break;
                    }

                    parts.Add(possibleParts[r.Next(0, possibleParts.Count)]);
                }

                if (parts.Count == 0)
                    continue;

                List<WordPart> possibleEndings;

                var lastP = parts.Last();

                if (lastP.Vow2 == null)
                {
                    parts.Clear();
                    continue;
                }

                if (!_ends.TryGetValue(lastP.Vow2, out possibleEndings))
                {
                    parts.Clear();
                    continue;
                }

                parts.Add(possibleEndings[r.Next(0, possibleEndings.Count)]);

            }

            return parts.First().Vow1 + string.Join("", parts.Select(wp => wp.Mid + wp.Vow2));

        }

        /// <summary>
        /// Generate a word corresponding to a string specified (md5, uuid)
        /// </summary>
        /// <param name="partsCount">How much parts to take</param>
        /// <param name="baseString">A string to use as a seed for generation</param>
        /// <returns></returns>
        public string GenWord(int partsCount, string baseString)
        {
            return GenWord(partsCount, new Random(baseString.GetHashCode()));
        }
    }
}