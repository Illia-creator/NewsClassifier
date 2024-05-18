using Python.Runtime;

internal class Program
{
    private static void Main(string[] args)
    {
        static void RunScript(string scriptName)
        {
            if (string.IsNullOrEmpty(scriptName))
            {
                throw new ArgumentNullException("The script name is null or empty.");
            }
            Runtime.PythonDLL = @"C:\Users\sie29\AppData\Local\Programs\Python\Python312\python312.dll";
            PythonEngine.Initialize();
            using (Py.GIL())
            {
                dynamic sys = Py.Import("sys");
                sys.path.append(@"C:\Users\sie29\source\repos\NewsClassifier\src\NewsClassifier.Classifier\testFol");
                var pythonScript = Py.Import(scriptName);
                var result = pythonScript.InvokeMethod("calculateAccuracy");
            }
        }

        RunScript("pythonskript");
    }
}