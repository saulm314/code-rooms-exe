int _number = 1;
int secondNumber = _number;
secondNumber = 3;
_number = secondNumber;
secondNumber=_number;

int number__ = 5;

int thirdNumber = _number;
int numbernumber = -193214;
thirdNumber = numbernumber;

bool boolean = false;
bool boolean2 = true;
boolean2 = boolean;
boolean = true;

numbernumber = 17;

char character = '\n';
char character2 = 'a';

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