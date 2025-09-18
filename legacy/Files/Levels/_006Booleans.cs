// star 1
int remainder = myInt % 2;
bool isOdd = remainder == 1;

// star 2
bool isEven = !isOdd;

// star 3
int remainder2 = myInt2 % 2;
bool isOdd2 = remainder2 == 1;
bool isEven2 = !isOdd2;

bool bothAreOdd = isOdd & isOdd2;
bool atLeastOneIsOdd = isOdd | isOdd2;

// star 4
bool firstIsSmallerThanSecond = myInt < myInt2;