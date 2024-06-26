﻿using Newtonsoft.Json;
using System;
using System.Reflection;

namespace CRECSharpInterpreter.Levels
{
    public class Level
    {
        private ILevelTest GetLevelTest()
        {
            if (slug == null)
                return new DummyLevelTest();
            Assembly assembly = Assembly.GetExecutingAssembly();
            Type[] assemblyTypes = assembly.GetTypes();
            Predicate<Type> testPredicate = type =>
                type.Namespace == "CRECSharpInterpreter.Levels.Tests" &&
                type.Name == slug;
            Type? testType = Array.Find(assemblyTypes, testPredicate);
            if (testType == null)
                return new DummyLevelTest();
            ConstructorInfo? constructor = testType.GetConstructor(Array.Empty<Type>());
            if (constructor == null)
                return new DummyLevelTest();
            object testObj = constructor.Invoke(Array.Empty<object?>());
            if (testObj is not ILevelTest)
                return new DummyLevelTest();
            return (ILevelTest)testObj;
        }

        public int id;
        public string? name;
        public string? slug;
        public int maxStars;
        public Variable[][]? initialStacks;
        public Variable[][]? initialHeaps;

        [JsonIgnore]
        public ILevelTest LevelTest => _levelTest ??= GetLevelTest();
        
        [JsonIgnore]
        private ILevelTest? _levelTest;

        [JsonIgnore]
        public string? Description { get; set; }
    }
}
