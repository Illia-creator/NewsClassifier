using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NewsClassifier.Classifier.Servicess
{
    public class ClassifyService
    {
        private readonly string filePath = @"C:\Users\sie29\source\repos\NewsClassifier\src\NewsClassifier.Classifier\testFol\testNews.csv";
        string[] wordsToRemove = { "хуйовий", "підписатися", "знову летить", "запропонувати новину", "реклама", "автострахування", "страхування житла", "телеграмна служба новин", "надіслати новину", "", "", "", "" };
     
        public async Task AddDataToDataset(string newsClass, string newsText)
        {
            using (StreamWriter writer = new StreamWriter(filePath, append: true, encoding: System.Text.Encoding.UTF8))
            {
                string sanitizedNewsText = Regex.Replace(newsText, @"\r?\n", " "); // Remove paragraph breaks
                sanitizedNewsText = Regex.Replace(sanitizedNewsText, @"[^\w\sа-яА-ЯєЄґҐіІїЇйЙ]", " "); // Remove punctuation
                sanitizedNewsText = Regex.Replace(sanitizedNewsText, @"\d+", " ");
                sanitizedNewsText = Regex.Replace(sanitizedNewsText, @"\s+", " ").Trim(); // Remove extra spaces
                sanitizedNewsText = sanitizedNewsText.ToLower();
                sanitizedNewsText = RemoveWordsAndPhrases(sanitizedNewsText); // Use sanitized text here

                string line = $"{newsClass}: {sanitizedNewsText}";
                await writer.WriteLineAsync(line);
            }
        }

        private string RemoveWordsAndPhrases(string text)
        {
            string pattern = @"\b(" + string.Join("|", wordsToRemove.Select(Regex.Escape)) + @")\b";
            string sanitizedText = Regex.Replace(text, pattern, "", RegexOptions.IgnoreCase);
            return sanitizedText;
        }

        //public async Task AddDataToDataset(string newsClass, string newsText)
        //{
        //    using (StreamWriter writer = new StreamWriter(filePath, append: true, encoding: System.Text.Encoding.UTF8))
        //    {
        //        string sanitizedNewsText = Regex.Replace(newsText, @"[^а-яА-ЯєЄґҐіІїЇйЙ\s]", " ");

        //        sanitizedNewsText = Regex.Replace(sanitizedNewsText, @"\s+", " ").Trim();

        //        sanitizedNewsText = sanitizedNewsText.ToLower();

        //        string line = $"{newsClass}: {sanitizedNewsText}";
        //        await writer.WriteLineAsync(line);
        //    }
        //}
    }
}
