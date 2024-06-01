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
        private readonly string filePath = @"C:\Users\sie29\source\repos\NewsClassifier\src\NewsClassifier.Classifier\testFol\practics_dataset.csv";

        public async Task AddDataToDataset(string newsClass, string newsText)
        {
            using (StreamWriter writer = new StreamWriter(filePath, append: true, encoding: System.Text.Encoding.UTF8))
            {
                string sanitizedNewsText = Regex.Replace(newsText, @"[^а-яА-ЯєЄґҐіІїЇйЙ\s]", " ");

                sanitizedNewsText = Regex.Replace(sanitizedNewsText, @"\s+", " ").Trim();

                sanitizedNewsText = sanitizedNewsText.ToLower();

                string line = $"{newsClass}: {sanitizedNewsText}";
                await writer.WriteLineAsync(line);
            }
        }
    }
}
