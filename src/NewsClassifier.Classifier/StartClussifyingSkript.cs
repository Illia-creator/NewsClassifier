using Python.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NewsClassifier.Classifier
{
    public class PythonEngineManager : IDisposable
    {
        private static bool isInitialized = false;

        public PythonEngineManager()
        {
            if (!isInitialized)
            {
                Runtime.PythonDLL = @"C:\Users\sie29\AppData\Local\Programs\Python\Python312\python312.dll";
                PythonEngine.Initialize();
                isInitialized = true;
            }
        }

        public void Dispose()
        {
            if (isInitialized)
            {
                try
                {
                    PythonEngine.Shutdown();
                    isInitialized = false;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error during PythonEngine shutdown: " + ex.Message);
                }
            }
        }
    }

    public class NewsClassifierCalculate
    {
        public string ClassifyNews(string newsText)
        {
            string result = null;

            try
            {
                using (Py.GIL())
                {
                    dynamic sys = Py.Import("sys");
                    sys.path.append(@"C:\Users\sie29\source\repos\NewsClassifier\src\NewsClassifier.Classifier\testFol");
                    dynamic pythonScript = Py.Import("pythonskript");
                    PyObject pyText = new PyString(newsText);
                    result = pythonScript.get_classify_news_label(pyText).ToString();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            return result;
        }
    }
}
