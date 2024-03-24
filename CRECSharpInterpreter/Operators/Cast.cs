using System;
using System.Collections.Generic;

namespace CRECSharpInterpreter.Operators
{
    public class Cast : ISpecificOperator
    {
        private Cast(Operand rightOperand, VarType returnType)
        {
            RightOperand = rightOperand;
            ReturnType = returnType;
        }

        public Operand? LeftOperand { get; } = null;
        public Operand? RightOperand { get; init; }
        public VarType? ReturnType { get; init; }

        // for most use cases we could simply return rightValue without doing anything
        // then at some point some other specific operator's Calculate method will do the cast anyway
        // however the equality and inequality operators do not cast
        // so if we do not convert manually, we may run into trouble with an expression such as:
        // 5.0 == (double)5
        // this *may* work, but it is safer to explicitly convert
        public object? Calculate(object? leftValue, object? rightValue)
        {
            Operand rightOperand = (Operand)RightOperand!;
            if (rightOperand._VarType == null)
                return null;
            if (rightOperand._VarType.IsArray)
                return rightValue;
            return Convert.ChangeType(rightValue, ReturnType!.SystemType);
        }

        public static ISpecificOperator[] GetCastsForReturnType(VarType returnType)
        {
            bool isReferenceType = returnType._Storage == VarType.Storage.Reference;
            int trivialCastCount = isReferenceType ? 2 : 1;
            Cast[] trivialCasts = new Cast[trivialCastCount];
            trivialCasts[0] = new(new(returnType), returnType);
            if (isReferenceType)
                trivialCasts[1] = new(new(null), returnType);
            try
            {
                Operand[] permittedNonTrivialInputs = PermittedNonTrivialCasts[returnType];
                Cast[] casts = new Cast[permittedNonTrivialInputs.Length + trivialCastCount];
                for (int i = 0; i < permittedNonTrivialInputs.Length; i++)
                    casts[i] = new(permittedNonTrivialInputs[i], returnType);
                trivialCasts.CopyTo(casts, permittedNonTrivialInputs.Length);
                return casts;
            }
            catch (KeyNotFoundException)
            {
                return trivialCasts;
            }
        }

        // a trivial cast is defined as a cast from a type to the same type,
        //      or a cast from null to any reference type
        public static Dictionary<VarType, Operand[]> PermittedNonTrivialCasts { get; } = new()
        {
            [VarType.@int] = new Operand[] { new(VarType.@double) },
            [VarType.@double] = new Operand[] { new(VarType.@int) },
            [VarType.@bool] = Array.Empty<Operand>(),
            [VarType.@char] = Array.Empty<Operand>()
        };
    }
}
