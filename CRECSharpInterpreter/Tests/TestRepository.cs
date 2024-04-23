using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace CRECSharpInterpreter.Tests
{
    public static class TestRepository
    {
        static TestRepository()
        {
            string directory = @"..\..\..\..\Files\Tests";
            AddTestsFromDirectories(new string[] { directory });
        }

        private static void AddTestsFromDirectories(string[] directories)
        {
            foreach (string directory in directories)
            {
                string[] files = Directory.GetFiles(directory);
                foreach (string file in files)
                    AddTestFromFile(file);
                string[] subdirectories = Directory.GetDirectories(directory);
                AddTestsFromDirectories(subdirectories);
            }
        }

        private static void AddTestFromFile(string file)
        {
            int fileNameIndex = file.LastIndexOf('\\') + 1;
            string testName;
            string testPathNoExt;
            if (file.EndsWith(".cs"))
            {
                testName = file[fileNameIndex..(file.Length - 3)];
                testPathNoExt = file[0..(file.Length - 3)];
            }
            else if (file.EndsWith(".java"))
            {
                testName = file[fileNameIndex..(file.Length - 5)];
                testPathNoExt = file[0..(file.Length - 5)];
            }
            else
                return;
            for (int i = Tests.Count - 1; i >= 0; i--)
                if (Tests[i].PathNoExt == testPathNoExt)
                    return;
            if (testName.StartsWith("BadC"))
            {
                BadTest badTest = new(testPathNoExt, Error.Compile);
                Tests.Add(badTest);
                return;
            }
            if (testName.StartsWith("BadR"))
            {
                BadTest badTest = new(testPathNoExt, Error.Run);
                Tests.Add(badTest);
                return;
            }
            AddGoodTests(testName, testPathNoExt);
        }

        private static void AddGoodTests(string testName, string testPathNoExt)
        {
            string testNameNoUnderscores = testName.Replace("_", string.Empty);
            Assembly assembly = Assembly.GetExecutingAssembly();
            Type[] assemblyTypes = assembly.GetTypes();
            Predicate<Type> testPredicate = type =>
                type.Namespace == "CRECSharpInterpreter.Tests.Good" &&
                type.Name == testNameNoUnderscores;
            Type? testType = Array.Find(assemblyTypes, testPredicate);
            if (testType == null)
            {
                AddGoodTestNotFound(testPathNoExt);
                return;
            }
            ConstructorInfo? constructor = testType.GetConstructor(new Type[] { typeof(string) });
            if (constructor == null)
            {
                AddGoodTestNotFound(testPathNoExt);
                return;
            }
            object testObj = constructor.Invoke(new object?[] { testPathNoExt });
            if (testObj is not ITest)
            {
                AddGoodTestNotFound(testPathNoExt);
                return;
            }
            ITest test = (ITest)testObj;
            Tests.Add(test);
        }

        private static void AddGoodTestNotFound(string testName)
        {
            Tests.Add(new GoodTestNotFound(testName));
        }

        public static List<ITest> Tests { get; } = new();

        public static int SuccessfulTests { get; set; } = 0;
        public static int TotalTests { get; set; } = 0;
    }
}
