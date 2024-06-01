 using Python.Runtime;
using System.Diagnostics;
using System.Text;

internal class Program
{
    public static void Main(string[] args)
    {
        string pythonExePath = @"C:\Users\sie29\AppData\Local\Programs\Python\Python312\python.exe";
        string scriptPath = @"C:\Users\sie29\source\repos\NewsClassifier\src\NewsClassifier.Classifier\testFol\pythonskript.py";

        // Пример вызова классификации несколько раз
        string[] newsTexts = { " БпЛА у Шепетівському районі Хмельницької області", "Курс Північно-Західний.1х БпЛА з Житомирщини у напрямку", " Рівненщини.Курс Західний. 3" };
        foreach (string newsText in newsTexts)
        {
            string result = CallPythonScript(pythonExePath, scriptPath, newsText.Trim());
            Console.WriteLine($"Classification Result for \"{newsText}\": {result}");
        }

        // Прекращение работы приложения
        Environment.Exit(0);
    }

    public static string CallPythonScript(string pythonExePath, string scriptPath, string newsText)
    {
        ProcessStartInfo start = new ProcessStartInfo();
        start.FileName = pythonExePath;
        start.Arguments = $"\"{scriptPath}\" \"{newsText}\"";
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








