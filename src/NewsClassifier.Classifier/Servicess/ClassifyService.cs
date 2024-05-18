using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsClassifier.Classifier.Servicess
{
    public class ClassifyService
    {
        private readonly string filePath = @"C:\Users\sie29\source\repos\NewsClassifier\src\NewsClassifier.Classifier\testFol\news.csv";
        public async Task AddDataToDataset(string newsClass, string newsText)
        {
            using (StreamWriter writer = new StreamWriter(filePath, append: true, encoding: System.Text.Encoding.UTF8))
            {
                string sanitizedNewsText = newsText.Replace("\n", " ").Replace("\r", " ").Replace("\t", " ");

                string line = $"{newsClass}\t{sanitizedNewsText}";
                await writer.WriteLineAsync(line);
            }

        }
    }
}
