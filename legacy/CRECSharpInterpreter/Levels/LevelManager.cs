using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using RealVariable = CRECSharpInterpreter.Variable;

namespace CRECSharpInterpreter.Levels
{
    public class LevelManager
    {
        #if !CREBLAZOR
        public LevelManager()
        {
            #if DEBUG
            string[] files = Directory.GetFiles(@"..\..\..\..\Files\Levels");
            #elif RELEASE
            string[] files = Directory.GetFiles(@"Files\Levels");
            #endif
            string[] jsonFiles = files
                .Select(file => file)
                .Where(file => file.EndsWith(".json"))
                .ToArray();
            Levels = new Level[jsonFiles.Length];
            for (int i = 0; i < jsonFiles.Length; i++)
                Levels[i] = JsonConvert.DeserializeObject<Level>(File.ReadAllText(jsonFiles[i]))!;
            string[] textFiles = files
                .Select(file => file)
                .Where(file => file.EndsWith(".txt"))
                .ToArray();
            foreach (string textFile in textFiles)
            {
                string title = textFile.Remove(textFile.Length - 4);
                foreach (Level level in Levels)
                {
                    if (title.EndsWith(level.slug!))
                    {
                        level.Description = File.ReadAllText(textFile);
                        continue;
                    }
                }
            }
        }

        public static LevelManager Instance { get; } = new();
        #elif CREBLAZOR
        public LevelManager(Level[] levels) => Levels = levels;
        #endif

        public Level[] Levels { get; init; }

        public int CurrentLevel { get; private set; }

        public Level GetLevel(int id)
        {
            foreach (Level level in Levels)
                if (level.id == id)
                    return level;
            throw new Exception($"Level with id {id} not found");
        }

        public void LoadLevel(Level level, int cycle = 0)
        {
            Variable[] stackVariables = level.initialStacks?[cycle] ?? Array.Empty<Variable>();
            RealVariable[] realStackVariables = new RealVariable[stackVariables.Length];
            for (int i = 0; i < stackVariables.Length; i++)
            {
                Variable variable = stackVariables[i];
                VarType varType = VarType.GetVarTypeFromSlug(variable.type!)!;
                string name = variable.name!;
                object? value = GetConvertedValue(variable.value, varType);
                bool initialised = variable.initialised;
                RealVariable realVariable = new(varType, name, value, initialised);
                realStackVariables[i] = realVariable;
            }
            Variable[] heapVariables = level.initialHeaps?[cycle] ?? Array.Empty<Variable>();
            RealVariable[] realHeapVariables = new RealVariable[heapVariables.Length];
            for (int i = 0; i < heapVariables.Length; i++)
            {
                Variable variable = heapVariables[i];
                VarType varType = VarType.GetVarTypeFromSlug(variable.type!)!;
                object? value = GetConvertedValue(variable.value, varType);
                int? length = GetConvertedValue(variable.length, VarType.@int) as int?;
                int? references = GetConvertedValue(variable.references, VarType.@int) as int?;
                RealVariable realVariable = value != null ? new RealVariable(varType, value) : new HeapLengthVariable()
                {
                    referenceCount = (int)references!,
                    Value = (int)length!
                };
                realHeapVariables[i] = realVariable;
            }
            Memory.preloadedStackVariables = realStackVariables;
            Memory.preloadedHeapVariables = realHeapVariables;
            CurrentLevel = level.id;
        }

        private object? GetConvertedValue(object? oldValue, VarType varType)
        {
            if (oldValue is long)
                return Convert.ToInt32(oldValue);
            if (oldValue is string str)
                return str[0];
            if (varType == VarType.@double && oldValue != null)
                return (double)oldValue!;
            return oldValue;
        }

        public void LoadLevel(int id, int cycle = 0)
        {
            Level level = GetLevel(id);
            LoadLevel(level, cycle);
        }

        public int GetCycleCount(Level? level = null)
        {
            level ??= GetLevel(CurrentLevel);
            return level.initialStacks?.Length ?? 1;
        }

        public int GetCycleCount(int id)
        {
            Level level = GetLevel(id);
            return level.initialStacks!.Length;
        }

        public ILevelTest GetLevelTest(Level? level = null)
        {
            level ??= GetLevel(CurrentLevel);
            return level.LevelTest;
        }

        public ILevelTest GetLevelTest(int id)
        {
            Level level = GetLevel(id);
            return level.LevelTest;
        }
    }
}
