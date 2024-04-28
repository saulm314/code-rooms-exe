using Newtonsoft.Json;
using System;
using System.IO;
using RealVariable = CRECSharpInterpreter.Variable;

namespace CRECSharpInterpreter.Levels
{
    public class LevelManager
    {
        public LevelManager()
        {
            string[] files = Directory.GetFiles(@"..\..\..\..\Files\Levels");
            Levels = new Level[files.Length];
            for (int i = 0; i < files.Length; i++)
                Levels[i] = JsonConvert.DeserializeObject<Level>(File.ReadAllText(files[i]))!;
        }

        public static LevelManager Instance { get; } = new();

        public Level[] Levels { get; init; }

        public Level GetLevel(int id)
        {
            foreach (Level level in Levels)
                if (level.id == id)
                    return level;
            throw new Exception($"Level with id {id} not found");
        }

        public void LoadLevel(Level level, int cycle = 0)
        {
            Variable[] stackVariables = level.initialStacks![cycle];
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
            Variable[] heapVariables = level.initialHeaps![cycle];
            RealVariable[] realHeapVariables = new RealVariable[heapVariables.Length];
            for (int i = 0; i < heapVariables.Length; i++)
            {
                Variable variable = heapVariables[i];
                VarType varType = VarType.GetVarTypeFromSlug(variable.type!)!;
                object? value = GetConvertedValue(variable.value, varType);
                int? length = GetConvertedValue(variable.length, VarType.@int) as int?;
                RealVariable realVariable = value != null ? new RealVariable(varType, value) : new HeapLengthVariable() { Value = length! };
                realHeapVariables[i] = realVariable;
            }
            Memory.preloadedStackVariables = realStackVariables;
            Memory.preloadedHeapVariables = realHeapVariables;
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

        public int GetCycleCount(Level level)
        {
            return level.initialStacks!.Length;
        }

        public int GetCycleCount(int id)
        {
            Level level = GetLevel(id);
            return level.initialStacks!.Length;
        }
    }
}
