using System;
using System.IO;
using System.Linq;

namespace CRETestCsJavaConverter
{
    public class Program
    {

        // * find each .cs file in the test directory (including subdirectories)
        // * make a copy of the .cs file and rename it to a .java file
        // * within the .java file, make the following replacements:
        // * * "bool" => "boolean"
        // * * "string" => "String"
        // * * "Length" => "length"
        // * if at this point the .java file contains "length", print the path of this file
        // * * this is so that we can manually inspect whether this length refers to a string length or an array length
        // * * if it is a string length, brackets "()" need to be appended as this should be a method
        //
        // execute this program from the command line to see the console output
        // use no-overwrite argument to prevent existing files from being overwritten
        private static void Main(string[] args)
        {
            if (args.Length > 0 && args[0] == "no-overwrite")
                overwrite = false;
            Console.WriteLine("Copying and converting .cs files to .java...");
            #if DEBUG
            string directory = @"..\..\..\..\Files\Tests";
            #elif RELEASE
            string directory = @"Files\Tests";
            #endif
            ConvertDirectoriesToJava(new string[] { directory });
            Console.WriteLine("Done");
        }

        private static bool overwrite = true;

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
                try
                {
                    File.Copy(csFile, javaFile, overwrite);
                }
                catch
                {
                    continue;
                }
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
