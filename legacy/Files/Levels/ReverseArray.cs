// 2 stars (uncomment)
/*int length = characters.Length;
char[] reverseCharacters = new char[length];
for (int i = 0; i < length; i = i + 1)
{
    int antiIndex = length - i - 1;
    reverseCharacters[antiIndex] = characters[i];
}
for (int i = 0; i < length; i = i + 1)
{
    characters[i] = reverseCharacters[i];
}*/

// 3 stars
int length = characters.Length;
int halfLength = length / 2;
for (int i = 0; i < halfLength; i = i + 1)
{
    int antiIndex = length - i - 1;
    char temp = characters[antiIndex];
    characters[antiIndex] = characters[i];
    characters[i] = temp;
}