int _number = 1;
int secondNumber = _number;
secondNumber = 3;
_number = /*multi-line comment*/secondNumber;
/* multi-line comment */
secondNumber=_number;

char[] helloChars = new char[] { 'h', 'e', 'l', 'l', 'o' };
char[] helloCharsCopy = new char[helloChars.Length];
for (int i = 0; i < helloChars.Length; i = i + 1)
    helloCharsCopy[i] = helloChars[i];

bool arraysAreEqual = true;
while (true)
{
    if (helloChars.Length != helloCharsCopy.Length)
    {
        arraysAreEqual = false;
        break;
    }
    for (int i = 0; i < helloChars.Length; i = i + 1)
    {
        if (helloChars[i] != helloCharsCopy[i])
        {
            arraysAreEqual = false;
            break;
        }
    }
    break;
}

char[] helloBackwards = new char[helloChars.Length];
for (int i = 0; i < helloChars.Length; i = i + 1)
    helloBackwards[i] = helloChars[helloChars.Length - i - 1];

for (int i = 0;; i = i + 1)
{
    if (helloChars[i] == 'o')
        break;
    if (helloChars[i] == 'l')
        continue;
    while (true)
        break;
}

string helloString = new string(helloChars);
helloChars = null;
helloString = null;

//__________________________________________

bool breakTest = true;
while (breakTest)
    break;

int continueTest = 3;
while (continueTest > 0)
{
    int decrement = 1;
    continueTest = continueTest - decrement;
    continue;
    continueTest = continueTest + decrement;
}

int toInitialise;
int anInt = 5;
while (true)
{
    int someInt = anInt;
    if (someInt > 7)
    {
        int[] ints = new int[] { someInt, 1 };
        int second = ints[1];
        if (second >= 1)
        {
            if (someInt == 8)
            {
                anInt = anInt + 1;
                continue;
            }
            toInitialise = 5;
            break;
        }
    }
    someInt = someInt + 1;
    anInt = someInt;
}


//_________________________________

bool whileTest = true;
while (whileTest)
    whileTest = false;

int whileTest2 = 4;
while (whileTest2 > 0)
    whileTest2 = whileTest2 - 1;

int[] copyArray = new int[] { 1, 2, 3 };
int[] pasteArray = new int[copyArray.Length];
int i = 0;
while (i < copyArray.Length)
{
    pasteArray[i] = copyArray[i];
    i = i + 1;
}


//______________________________________

if (false)
    string myString = "false";
else if (false)
    string myString = "false again";
else if (false)
    string myString = "false false false";
else if (true)
    string myString = "finally true";
else
    string myString = "all were false";

//______________________________
if (1 < 2)
    int myInt = 0;
else
    int myInt = 1;

if (2 < 1)
    int myInt = 0;
else
    int myInt = 1;

if (true)
{
    int myInt = 0;
}
else int myInt = 1;

if (false)
{
    int myInt = 0;
}
else int myInt = 1;

if (true)
{
    int myInt = 0;
}
else
{
    int myInt = 1;
}

if (false)
{
    int myInt = 0;
}
else
{
    int myInt = 1;
}

if (true) { } else { }
if (false) { } else { }
//_______________
if (3 < 5)
    if (true || false)
        double pi = 3.14;

if (false)
    if (true);

if (true) if (true) if (true) if (true);

;;;;;

bool ifBracesTest = true;
if (ifBracesTest) { }
if (ifBracesTest) { ; }
if (ifBracesTest)
{
    int digit1 = 3;
    int digit2 = 1;
    int digit3 = 4;
    int[] pi = new int[] { digit1, digit2, digit3 };
    if (true)
        int digit4 = 1;
    if (true)
    {
        int digit5 = 5;
        int digit6 = 9;
    }
    if (false)
        int digit7 = 2;
}

if (!ifBracesTest)
{
    int digit1 = 5;
}

// This is a comment.
int number__ = 5;
// this is another comment
int thirdNumber = _number;
int numbernumber = -193214;
thirdNumber = numbernumber;

bool boolean = false;
/*multi
line
comment*/
bool boolean2 = true;
boolean2 = boolean;
boolean = true;

numbernumber = 17;

char character = '\n';
char character2 = 'a';
char character3 = ';';
char character4 = '}';

double num;
num = 5.3;
double num2 = num;

int integer;
integer = 0;
_number = integer;

int[] intArray = new int[5];
int[] intArray2 = intArray;
int[] intArray3 = new int[3];
int[] intArray4 = intArray2;

char[] charArray;
charArray = new char[0];
char[] charArray2 = new char[4];

double[] doubleArray = new double[numbernumber];

int[] intArray5 = new int[] { 59, -17, thirdNumber };

int anotherNumber = intArray5[1];
char[] anotherCharArray = new char[] { 'a', 'b', 'c' };
int index = 2;
char letterC = anotherCharArray[index];

anotherCharArray = null;

intArray5[2] = 137;

intArray = null;
intArray2 = intArray3;
intArray4 = null;

bool[] boolArray = new bool[] { true, false, true };

bool[] boolArray2 = new bool[] { };

double[] doubleArray2 = new double[] { 17.3 };
double[] doubleArray3 = new double[] { };

int[] test = null;
int[] arrayarray = test;

int newInt = boolArray.Length;
bool[] boolArrayCopy = new bool[boolArray.Length];
boolArrayCopy[0] = boolArray[0];
boolArrayCopy[1] = boolArray[1];
boolArrayCopy[2] = boolArray[2];

int addInt = +5;
int addInt2 = 5 + 5 - 2 - 2 + 3;
int addInt3 = +addInt + addInt2 + 17 + 2;

int[] addIntArray = new int[2 + 1];
int[] addIntArray2 = new int[] { +addInt, 17+addInt2, 2 - 5 + addInt, 3 };

int mult = 5 * 3;
int mult2 = 5 * 3 + 2;
int mult3 = 2 + 5 * 3;
int mult4 = 2 + 5 * 3 - 7 - 2 * 3 * 2 + 5;

int div = 5 / 2 * 3 * 2 - 6 / 2 * 3;

double doubleCalc = 5.3 / 7.2 * 1.9 - 2.3 + 1.4 + 1.3 - 2.1 * 5.9;

bool comparison = 5 < 3;
bool comparison2 = 3 < 5;
bool comparison3 = 5 > 3;
bool comparison4 = 3 > 5;
bool comparison5 = 5 < 5;
bool comparison6 = 5 > 5;

bool comparison7 = 5.0 < 3.0;
bool comparison8 = 3.0 < 5.0;
bool comparison9 = 5.0 > 3.0;
bool comparison10 = 3.0 > 5.0;
bool comparison11 = 5.0 < 5.0;
bool comparison12 = 5.0 > 5.0;

bool comparison13 = addInt < addInt2;
bool comparison14 = addInt > addInt2;

bool comparison15 = 5 + 3 * 7 < 37 - 5 * 2;
bool comparison16 = 5 + 3 * 7 + 1 < 37 - 5 * 2;

int bracketTest = ((((((5))))));
int bracketTest2 = (2 + 5) * 3;
int bracketTest3 = 5 * (3 + 2);

int operationTest = + + + + + 5;
int operationTest2 = - + - + - 5;
int operationTest3 = + - + - + 5;

bool negationTest = !true;
bool negationTest2 = !false;
bool negationTest3 = !negationTest;
bool negationTest4 = !negationTest2;
bool negationTest5 = !(5 < 3);
bool negationTest6 = !(3 < 5);

bool lessEqual = 5 <= 3;
bool lessEqual2 = 5 <= 5;
bool lessEqual3 = 5 <= 6;
bool lessEqual4 = 5.3 <= 5.6;
bool lessEqual5 = 5.6 <= 5.3;
bool lessEqual6 = 5.6 <= 5.6;
bool lessEqual7 = 6 >= 5;
bool lessEqual8 = 6 >= 6;
bool lessEqual9 = 6 >= 7;
bool lessEqual10 = 6.3 >= 5.3;
bool lessEqual11 = 6.3 >= 6.3;
bool lessEqual12 = 6.3 >= 7.3;

bool logic = true & true;
bool logic2 = true && true;
bool logic3 = true & false;
bool logic4 = false & true;
bool logic5 = false && false;
bool logic6 = true || false;
bool logic7 = false | true;
bool logic8 = false || false;
bool logic9 = false || false;
bool logic10 = true ^ true;
bool logic11 = false ^ true;
bool logic12 = true ^ false;
bool logic13 = false ^ false;

bool logicTest = !(52 - 3 * 6 < 35 + 5) ^ (logic && (logic10 || boolArrayCopy[index - 2]));

bool equal = 5 == 5;
bool equal2 = 5 == 6;
bool equal3 = 5.0 == 5.0;
bool equal4 = 5.0 == 6.0;
bool equal5 = 'a' == 'a';
bool equal6 = 'a' == 'b';
bool equal7 = false == false;
bool equal8 = false == true;
bool equal9 = intArray2 == intArray3;
bool equal10 = boolArray == boolArrayCopy;
bool equal11 = boolArray == boolArrayCopy == (intArray2 == intArray3);
bool equal12 = boolArray == boolArrayCopy == !(intArray2 == intArray3);

bool equal13 = 5 != 5;
bool equal14 = 5 != 6;
bool equal15 = boolArray == boolArrayCopy != (intArray2 == intArray3);

int remainder = 5 % 2;
int remainder2 = 2 + 7 % 3;

bool cast = (bool)(5 == 5);
bool cast2 = (bool)false;
int cast3 = (int)5.3;
double cast4 = (double)5;
char cast5 = (char)'a';
bool cast6 = (bool)((int)5.3 == 5);
int[] cast7 = (int[]) new int[] { 5, 3 };
int[] cast8 = (int[])null;
cast7 = (int[])null;

bool nullCheck = null == null;
bool nullCheck2 = cast7 == null;
bool nullCheck3 = null == cast7;
bool nullCheck4 = null != null;
bool nullCheck5 = boolArray == null;

string myString;
myString = "Hello, World!";
myString = "Goodbye!";
string myString2 = myString;
string myString3 = "Goodbye!";
bool stringComp = myString == myString2;
bool stringComp2 = myString == myString3;
string myString4 = null;
string myString5 = myString4;
myString4 = "semi;colon";

char strElement = myString3[2];

string[] stringArr = new string[5];
string[] animals = new string[] { "Cat", "Dog", "Rabbit" };
string animal = animals[1];
animals = null;
animal = null;

int[] dummy = new int[0];

string colour = "Blue";
string[] colours = new string[] { colour, "Red", "Green" };
colour = null;
colours = null;

string number = "One";
string[] numbers = new string[] { number, "Two", "Three" };
char thirdNumberSecondLetter = numbers[2][1];
numbers[0] = null;
number = numbers[1];
string strNumber2 = numbers[0];

string[] strArr = new string[2];
strArr[0] = "Hello";

string strLenTest = "Hi";
int stringLength = strLenTest.Length;

string myString__ = "hello";
string myString2__ = (string)myString__;
myString__ = null;
myString2__ = "hi";

int myInt;
if (false)
	myInt = 0;
else if (false)
    myInt = 1;
else if (false)
    myInt = 2;
else if (false)
    myInt = 3;
else
    myInt = 4;
int myInt2 = myInt;

string[] myStringArr = new string[] { new string(new char[] { 'H', 'e', 'l', 'l', 'o', ',', ' ' }), "World!" };