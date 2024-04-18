int _number = 1;
int secondNumber = _number;
secondNumber = 3;
_number = /*multi-line comment*/secondNumber;
/* multi-line comment */
secondNumber=_number;

char[] helloChars = new char[] { 'h', 'e', 'l', 'l', 'o' };
char[] helloCharsCopy = new char[helloChars.length];
for (int i = 0; i < helloChars.length; i = i + 1)
    helloCharsCopy[i] = helloChars[i];

boolean arraysAreEqual = true;
while (true)
{
    if (helloChars.length != helloCharsCopy.length)
    {
        arraysAreEqual = false;
        break;
    }
    for (int i = 0; i < helloChars.length; i = i + 1)
    {
        if (helloChars[i] != helloCharsCopy[i])
        {
            arraysAreEqual = false;
            break;
        }
    }
    break;
}

char[] helloBackwards = new char[helloChars.length];
for (int i = 0; i < helloChars.length; i = i + 1)
    helloBackwards[i] = helloChars[helloChars.length - i - 1];

for (int i = 0;; i = i + 1)
{
    if (helloChars[i] == 'o')
        break;
    if (helloChars[i] == 'l')
        continue;
    while (true)
        break;
}

String helloString = new String(helloChars);
helloChars = null;
helloString = null;

//__________________________________________

boolean breakTest = true;
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

boolean whileTest = true;
while (whileTest)
    whileTest = false;

int whileTest2 = 4;
while (whileTest2 > 0)
    whileTest2 = whileTest2 - 1;

int[] copyArray = new int[] { 1, 2, 3 };
int[] pasteArray = new int[copyArray.length];
int i = 0;
while (i < copyArray.length)
{
    pasteArray[i] = copyArray[i];
    i = i + 1;
}


//______________________________________

if (false)
    String myString = "false";
else if (false)
    String myString = "false again";
else if (false)
    String myString = "false false false";
else if (true)
    String myString = "finally true";
else
    String myString = "all were false";

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

boolean ifBracesTest = true;
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

boolean boolean_ = false;
/*multi
line
comment*/
boolean boolean2 = true;
boolean2 = boolean_;
boolean_ = true;

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

boolean[] boolArray = new boolean[] { true, false, true };

boolean[] boolArray2 = new boolean[] { };

double[] doubleArray2 = new double[] { 17.3 };
double[] doubleArray3 = new double[] { };

int[] test = null;
int[] arrayarray = test;

int newInt = boolArray.length;
boolean[] boolArrayCopy = new boolean[boolArray.length];
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

boolean comparison = 5 < 3;
boolean comparison2 = 3 < 5;
boolean comparison3 = 5 > 3;
boolean comparison4 = 3 > 5;
boolean comparison5 = 5 < 5;
boolean comparison6 = 5 > 5;

boolean comparison7 = 5.0 < 3.0;
boolean comparison8 = 3.0 < 5.0;
boolean comparison9 = 5.0 > 3.0;
boolean comparison10 = 3.0 > 5.0;
boolean comparison11 = 5.0 < 5.0;
boolean comparison12 = 5.0 > 5.0;

boolean comparison13 = addInt < addInt2;
boolean comparison14 = addInt > addInt2;

boolean comparison15 = 5 + 3 * 7 < 37 - 5 * 2;
boolean comparison16 = 5 + 3 * 7 + 1 < 37 - 5 * 2;

int bracketTest = ((((((5))))));
int bracketTest2 = (2 + 5) * 3;
int bracketTest3 = 5 * (3 + 2);

int operationTest = + + + + + 5;
int operationTest2 = - + - + - 5;
int operationTest3 = + - + - + 5;

boolean negationTest = !true;
boolean negationTest2 = !false;
boolean negationTest3 = !negationTest;
boolean negationTest4 = !negationTest2;
boolean negationTest5 = !(5 < 3);
boolean negationTest6 = !(3 < 5);

boolean lessEqual = 5 <= 3;
boolean lessEqual2 = 5 <= 5;
boolean lessEqual3 = 5 <= 6;
boolean lessEqual4 = 5.3 <= 5.6;
boolean lessEqual5 = 5.6 <= 5.3;
boolean lessEqual6 = 5.6 <= 5.6;
boolean lessEqual7 = 6 >= 5;
boolean lessEqual8 = 6 >= 6;
boolean lessEqual9 = 6 >= 7;
boolean lessEqual10 = 6.3 >= 5.3;
boolean lessEqual11 = 6.3 >= 6.3;
boolean lessEqual12 = 6.3 >= 7.3;

boolean logic = true & true;
boolean logic2 = true && true;
boolean logic3 = true & false;
boolean logic4 = false & true;
boolean logic5 = false && false;
boolean logic6 = true || false;
boolean logic7 = false | true;
boolean logic8 = false || false;
boolean logic9 = false || false;
boolean logic10 = true ^ true;
boolean logic11 = false ^ true;
boolean logic12 = true ^ false;
boolean logic13 = false ^ false;

boolean logicTest = !(52 - 3 * 6 < 35 + 5) ^ (logic && (logic10 || boolArrayCopy[index - 2]));

boolean equal = 5 == 5;
boolean equal2 = 5 == 6;
boolean equal3 = 5.0 == 5.0;
boolean equal4 = 5.0 == 6.0;
boolean equal5 = 'a' == 'a';
boolean equal6 = 'a' == 'b';
boolean equal7 = false == false;
boolean equal8 = false == true;
boolean equal9 = intArray2 == intArray3;
boolean equal10 = boolArray == boolArrayCopy;
boolean equal11 = boolArray == boolArrayCopy == (intArray2 == intArray3);
boolean equal12 = boolArray == boolArrayCopy == !(intArray2 == intArray3);

boolean equal13 = 5 != 5;
boolean equal14 = 5 != 6;
boolean equal15 = boolArray == boolArrayCopy != (intArray2 == intArray3);

int remainder = 5 % 2;
int remainder2 = 2 + 7 % 3;

boolean cast = (boolean)(5 == 5);
boolean cast2 = (boolean)false;
int cast3 = (int)5.3;
double cast4 = (double)5;
char cast5 = (char)'a';
boolean cast6 = (boolean)((int)5.3 == 5);
int[] cast7 = (int[]) new int[] { 5, 3 };
int[] cast8 = (int[])null;
cast7 = (int[])null;

boolean nullCheck = null == null;
boolean nullCheck2 = cast7 == null;
boolean nullCheck3 = null == cast7;
boolean nullCheck4 = null != null;
boolean nullCheck5 = boolArray == null;

String myString;
myString = "Hello, World!";
myString = "Goodbye!";
String myString2 = myString;
String myString3 = "Goodbye!";
boolean stringComp = myString == myString2;
boolean stringComp2 = myString == myString3;
String myString4 = null;
String myString5 = myString4;
myString4 = "semi;colon";

char strElement = myString3[2];

String[] stringArr = new String[5];
String[] animals = new String[] { "Cat", "Dog", "Rabbit" };
String animal = animals[1];
animals = null;
animal = null;

int[] dummy = new int[0];

String colour = "Blue";
String[] colours = new String[] { colour, "Red", "Green" };
colour = null;
colours = null;

String number = "One";
String[] numbers = new String[] { number, "Two", "Three" };
char thirdNumberSecondLetter = numbers[2][1];
numbers[0] = null;
number = numbers[1];
String strNumber2 = numbers[0];

String[] strArr = new String[2];
strArr[0] = "Hello";

String strLenTest = "Hi";
int stringLength = strLenTest.length();

String myString__ = "hello";
String myString2__ = (String)myString__;
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

String[] myStringArr = new String[] { new String(new char[] { 'H', 'e', 'l', 'l', 'o', ',', ' ' }), "World!" };