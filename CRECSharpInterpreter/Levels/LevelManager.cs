﻿using Newtonsoft.Json;
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

        public void LoadLevel(Level level)
        {
            Variable[] stackVariables = level.initialStacks![0];
            RealVariable[] realStackVariables = new RealVariable[stackVariables.Length];
            for (int i = 0; i < stackVariables.Length; i++)
            {
                Variable variable = stackVariables[i];
                VarType varType = VarType.GetVarTypeFromSlug(variable.type!)!;
                string name = variable.name!;
                object? value = variable.value;
                if (value is long)
                    value = Convert.ToInt32(value);
                bool initialised = variable.initialised;
                RealVariable realVariable = new(varType, name, value, initialised);
                realStackVariables[i] = realVariable;
            }
            Variable[] heapVariables = level.initialHeaps![0];
            RealVariable[] realHeapVariables = new RealVariable[heapVariables.Length];
            for (int i = 0; i < heapVariables.Length; i++)
            {
                Variable variable = heapVariables[i];
                VarType varType = VarType.GetVarTypeFromSlug(variable.type!)!;
                object? value = variable.value;
                int? length = variable.length;
                RealVariable realVariable = value != null ? new RealVariable(varType, value) : new HeapLengthVariable() { Value = length! };
                realHeapVariables[i] = realVariable;
            }
            Memory.preloadedStackVariables = realStackVariables;
            Memory.preloadedHeapVariables = realHeapVariables;
        }

        public void LoadLevel(int id)
        {
            Level level = GetLevel(id);
            LoadLevel(level);
        }
    }
}
