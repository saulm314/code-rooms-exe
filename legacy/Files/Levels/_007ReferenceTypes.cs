// star 1
bool[] myBools = new bool[4];

// star 2
myBools[3] = true;

// star 3
char[] myChars = new char[] { 'x', 'y', 'z' };
char[] myChars2 = myChars;

// star 4
char[] myCharsDifferent = new char[] { 'x', 'y', 'z' };

// feeling of accomplishment
bool same = myChars == myChars2;
bool same2 = myChars == myCharsDifferent;
bool same3 = myChars2 == myCharsDifferent;