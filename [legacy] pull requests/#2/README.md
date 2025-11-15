Merge commit: https://github.com/saulm314/code-rooms-exe/commit/b9a45eda3e494f70a07165b4f3a9b2253d4fc3c8

# Comment

With this merge we add a choice of Java vs. C# syntax.

The interpreter semantics behave identically in both cases, and it should be noted that the semantics adhere neither to Java nor C#. The only difference is in the syntax.

Syntax that is not the same in the C# and Java versions:
- bool, boolean
- string, String
- Length, length()

Everything else is identical.

It should also be noted that Java does not support reading the character of a string via an indexer, i.e. myString[0], but we keep this feature for convenience.
