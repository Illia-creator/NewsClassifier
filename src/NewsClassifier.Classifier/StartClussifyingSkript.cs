using NewsClassifier.Classifier.Servicess;
using Python.Runtime;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NewsClassifier.Classifier
{
    public static class PythonHelper
    {
        public static string CallScript(string pythonExePath, string scriptPath, string arguments)
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = pythonExePath;
            start.Arguments = $"\"{scriptPath}\" \"{arguments}\"";
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            start.RedirectStandardError = true;
            start.StandardOutputEncoding = Encoding.UTF8;  // Указываем UTF-8 кодировку для стандартного вывода
            start.StandardErrorEncoding = Encoding.UTF8;   // Указываем UTF-8 кодировку для стандартной ошибки
            start.CreateNoWindow = true;

            using (Process process = Process.Start(start))
            {
                using (System.IO.StreamReader reader = process.StandardOutput)
                {
                    string result = reader.ReadToEnd().Trim();
                    return result;
                }
            }
        }
    }
}
