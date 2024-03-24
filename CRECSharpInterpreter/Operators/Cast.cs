using System;
using System.Collections.Generic;

namespace CRECSharpInterpreter.Operators
{
    public class Cast : ISpecificOperator
    {
        private Cast(VarType rightType, VarType returnType)
        {
            RightType = rightType;
            ReturnType = returnType;
        }

        public VarType LeftType { get; } = null;
        public VarType RightType { get; init; }
        public VarType ReturnType { get; init; }

        // for most use cases we could simply return rightValue without doing anything
        // then at some point some other specific operator's Calculate method will do the cast anyway
        // however the equality and inequality operators do not cast
        // so if we do not convert manually, we may run into trouble with an expression such as:
        // 5.0 == (double)5
        // this *may* work, but it is safer to explicitly convert
        public object Calculate(object leftValue, object rightValue)
        {
            if (RightType == null)
                return null;
            if (RightType.IsArray)
                return rightValue;
            return Convert.ChangeType(rightValue, ReturnType.SystemType);
        }

        public static ISpecificOperator[] GetCastsForReturnType(VarType returnType)
        {
            bool isReferenceType = returnType._Storage == VarType.Storage.Reference;
            int trivialCastCount = isReferenceType ? 2 : 1;
            Cast[] trivialCasts = new Cast[trivialCastCount];
            trivialCasts[0] = new(returnType, returnType);
            if (isReferenceType)
                trivialCasts[1] = new(null, returnType);
            try
            {
                VarType[] permittedNonTrivialInputTypes = PermittedNonTrivialCasts[returnType];
                Cast[] casts = new Cast[permittedNonTrivialInputTypes.Length + trivialCastCount];
                for (int i = 0; i < permittedNonTrivialInputTypes.Length; i++)
                    casts[i] = new(permittedNonTrivialInputTypes[i], returnType);
                trivialCasts.CopyTo(casts, permittedNonTrivialInputTypes.Length);
                return casts;
            }
            catch (KeyNotFoundException)
            {
                return trivialCasts;
            }
        }

        // a trivial cast is defined as a cast from a type to the same type,
        //      or a cast from null to any reference type
        public static Dictionary<VarType, VarType[]> PermittedNonTrivialCasts { get; } = new()
        {
            [VarType.@int] = new VarType[] { VarType.@double },
            [VarType.@double] = new VarType[] { VarType.@int },
            [VarType.@bool] = new VarType[] { },
            [VarType.@char] = new VarType[] { }
        };
    }
}
