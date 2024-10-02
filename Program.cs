using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace BookInfoExtractorAsynchronous
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Stopwatch watch = Stopwatch.StartNew();
            Console.OutputEncoding = Encoding.UTF8;

            Dictionary<string, int> wordOccurrences = new Dictionary<string, int>();

            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string fileName = "Dot-Hutchison_-_Ljatoto_na_angelite_-_11889-b.txt";
            string filePath = $"{baseDirectory}{fileName}";

            if (!File.Exists(filePath))
            {
                Console.WriteLine("The specified file does not exist.");
                return;
            }

            try
            {
                string text = File.ReadAllText(filePath);
                string cleanedText = CleanText(text);

                //Convert letters to lowercase
                cleanedText = cleanedText.ToLower();
                string[] words = cleanedText.Split(new char[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string word in words)
                {
                    if (word.Length >= 3)
                    {
                        if (!wordOccurrences.ContainsKey(word))
                        {
                            wordOccurrences.Add(word, 1);
                        }
                        else
                        {
                            wordOccurrences[word] = wordOccurrences[word] + 1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while processing the file:");
                Console.WriteLine(ex.Message);
            }

            Task<List<KeyValuePair<string, int>>> findFiveMostFrequentTask = FindFiveMostCommontWords(wordOccurrences, 5);
            Task<List<KeyValuePair<string, int>>> findFiveLeastFrequentTask = FindFiveLeastCommontWords(wordOccurrences, 5);
            Task<string> findShortestWordTask = FindShortestWord(wordOccurrences);
            Task<string> findLongestWordTask = FindLongestWord(wordOccurrences);
            Task<long> findTotalWordCountTask = FindTotalWordCount(wordOccurrences);
            Task<long> findUniqueWordCountTask = FindUniqueWordCount(wordOccurrences);
            Task<long> findTotalLettersCountTask = FindLetterCount(wordOccurrences);
            await Task.WhenAll(findFiveMostFrequentTask, findFiveLeastFrequentTask, findShortestWordTask, findLongestWordTask, findTotalWordCountTask, findUniqueWordCountTask, findTotalLettersCountTask);

            List<KeyValuePair<string, int>> fiveMostFrequent = findFiveMostFrequentTask.Result;
            List<KeyValuePair<string, int>> fiveLeastFrequent = findFiveLeastFrequentTask.Result;
            string shortestWord = findShortestWordTask.Result;
            string longestWord = findLongestWordTask.Result;
            long totalWordCount = findTotalWordCountTask.Result;
            long uniqueWordCount = findUniqueWordCountTask.Result;
            long totalLettersCount = findTotalLettersCountTask.Result;

            decimal averageWordLength = Math.Round((decimal)totalLettersCount / totalWordCount, 2);


            Console.WriteLine($"Total word count - {totalWordCount}");
            Console.WriteLine($"Unique word count - {uniqueWordCount}");
            Console.WriteLine($"Shortest word - {shortestWord}");
            Console.WriteLine($"Longest word - {longestWord}");
            Console.WriteLine($"Average word length - {averageWordLength}");

            Console.WriteLine("Five most common words: ");

            foreach (KeyValuePair<string, int> word in fiveMostFrequent)
            {
                Console.WriteLine($"{word.Key} - {word.Value}");
            }

            Console.WriteLine("Five least common words: ");

            foreach (KeyValuePair<string, int> word in fiveLeastFrequent)
            {
                Console.WriteLine($"{word.Key} - {word.Value}");
            }

            watch.Stop();
            var elapsedMilliseconds = watch.ElapsedMilliseconds;
            Console.WriteLine($"Info extraction complete in {elapsedMilliseconds}ms. Press any key to exit.");
            Console.ReadKey();
        }

        static string CleanText(string text)
        {
            string pattern = @"[^a-zA-Z\u0400-\u04FF0-9\s]";
            return Regex.Replace(text, pattern, " ");
        }

        public static Task<string> FindLongestWord(Dictionary<string, int> wordDict)
        {
            return Task.Run(() =>
            {
                string longestWord = null;
                foreach (var word in wordDict)
                {
                    //Initially assign the longest word to the first word in the dictionary
                    if (longestWord == null)
                    {
                        longestWord = word.Key;
                    }

                    if (word.Key.Length > longestWord.Length)
                    {
                        longestWord = word.Key;
                    }
                }
                return longestWord;
            });
        }

        public static Task<string> FindShortestWord(Dictionary<string, int> wordDict)
        {
            return Task.Run(() =>
            {
                string shortestWord = null;
                foreach (var word in wordDict)
                {
                    //Initially assign the shortest word to the first word in the dictionary
                    if (shortestWord == null)
                    {
                        shortestWord = word.Key;
                    }

                    if (word.Key.Length < shortestWord.Length)
                    {
                        shortestWord = word.Key;
                    }
                }
                return shortestWord;
            });
        }

        public static Task<List<KeyValuePair<string, int>>> FindFiveMostCommontWords(Dictionary<string, int> wordDict, int numberOfWords)
        {
            return Task.Run(() =>
            {
                // Initialize the result list
                List<KeyValuePair<string, int>> result = new List<KeyValuePair<string, int>>();

                // Initialize counts for comparison
                for (int i = 0; i < numberOfWords; i++)
                {
                    result.Add(new KeyValuePair<string, int>(null, int.MinValue));
                }

                foreach (var word in wordDict)
                {
                    string currentWord = word.Key;
                    int currentCount = word.Value;

                    for (int i = 0; i < result.Count; i++)
                    {
                        if (currentCount > result[i].Value)
                        {
                            // Shift elements to the right to make space
                            for (int j = result.Count - 1; j > i; j--)
                            {
                                result[j] = result[j - 1];
                            }

                            // Insert the new word and count
                            result[i] = new KeyValuePair<string, int>(currentWord, currentCount);
                            break;
                        }
                    }
                }

                return result;
            });
        }

        public static Task<List<KeyValuePair<string, int>>> FindFiveLeastCommontWords(Dictionary<string, int> wordDict, int numberOfWords)
        {
            return Task.Run(() =>
            {
                // Initialize the result list
                List<KeyValuePair<string, int>> result = new List<KeyValuePair<string, int>>();

                // Initialize counts for comparison
                for (int i = 0; i < numberOfWords; i++)
                {
                    result.Add(new KeyValuePair<string, int>(null, int.MaxValue));
                }

                foreach (var word in wordDict)
                {
                    string currentWord = word.Key;
                    int currentCount = word.Value;

                    for (int i = 0; i < result.Count; i++)
                    {
                        if (currentCount < result[i].Value)
                        {
                            // Shift elements to the right to make space
                            for (int j = result.Count - 1; j > i; j--)
                            {
                                result[j] = result[j - 1];
                            }

                            // Insert the new word and count
                            result[i] = new KeyValuePair<string, int>(currentWord, currentCount);
                            break;
                        }
                    }
                }

                return result;
            });
        }

        public static Task<long> FindTotalWordCount(Dictionary<string, int> wordDict)
        {
            return Task.Run(() =>
            {
                long totalWordsCount = 0;
                foreach (var word in wordDict)
                {
                    totalWordsCount += word.Value;
                }

                return totalWordsCount;
            });
        }

        public static Task<long> FindUniqueWordCount(Dictionary<string, int> wordDict)
        {
            return Task.Run(() =>
            {
                long uniqueWordsCount = 0;
                foreach (var word in wordDict)
                {
                    uniqueWordsCount++;
                }

                return uniqueWordsCount;
            });
        }

        public static Task<long> FindLetterCount(Dictionary<string, int> wordDict)
        {
            return Task.Run(() =>
            {
                long totalLettersCount = 0;
                foreach (var word in wordDict)
                {
                    totalLettersCount += word.Key.Length * word.Value;
                }
                return totalLettersCount;
            });
        }
    }
}