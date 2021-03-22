using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TreeSize__FindFile.ModelView
{
    class WorkWithFile
    {
        static public void SaveToFile(string saveParam)
        {
            string path = Environment.CurrentDirectory + @"\ParamSearch.txt";
            File.WriteAllText(path, saveParam);
        }

        static public string ReadPath()
        {
            string[] allLines;

            string result="", pathToFile = Environment.CurrentDirectory + @"\ParamSearch.txt";
            if (System.IO.File.Exists(pathToFile))
            {
                allLines = File.ReadAllLines(pathToFile);
                result = allLines[0];
            }
            return result;
        }

        static public string ReadNameFile()
        {
            string[] allLines;

            string result = "", pathToFile = Environment.CurrentDirectory + @"\ParamSearch.txt";
            try 
            {
                allLines = File.ReadAllLines(pathToFile);
                result = allLines[1];
            
                return result;
            }
            catch
            {
                return result;
            }
        }
    }
}
