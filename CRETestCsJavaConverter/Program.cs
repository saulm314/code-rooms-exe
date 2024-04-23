using System;
using System.IO;
using System.Linq;

namespace CRETestCsJavaConverter
{
    public class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Copying and converting .cs files to .java...");
            string directory = @"..\..\..\..\Files\Tests";
            ConvertDirectoriesToJava(new string[] { directory });
            Console.WriteLine("Done");
        }

        private static void ConvertDirectoriesToJava(string[] directories)
        {
            foreach (string directory in directories)
            {
                string[] files = Directory.GetFiles(directory);
                ConvertFilesToJava(files);
                string[] subdirectories = Directory.GetDirectories(directory);
                ConvertDirectoriesToJava(subdirectories);
            }
        }

        private static void ConvertFilesToJava(string[] files)
        {
            string[] csFiles = files.Select(file => file).Where(file => file.EndsWith(".cs")).ToArray();
            foreach (string csFile in csFiles)
            {
                string javaFile = csFile[0..(csFile.Length - 2)] + "java";
                File.Copy(csFile, javaFile, true);
                ConvertJavaFile(javaFile);
            }
        }

        private static void ConvertJavaFile(string javaFile)
        {
            string contents = File.ReadAllText(javaFile);
            contents = contents.Replace("bool", "boolean");
            contents = contents.Replace("string", "String");
            contents = contents.Replace("Length", "length");
            if (contents.Contains("length"))
                Console.WriteLine($"Contains \"length\": {javaFile}");
            File.WriteAllText(javaFile, contents);
        }
    }
}
