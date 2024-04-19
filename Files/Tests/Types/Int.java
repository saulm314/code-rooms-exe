int myInt = 0;
int myInt2 = 5;
int myInt3 = 2147483647;

// this is 1 more than the actual C# min value
// for our interpreter, it will be impossible to use the actual int min value
// this is because the interpreter does not interpret this as one negative number,
//      but rather as a positive number with a negation operator, whose value is calculated at runtime
// but positive 2147483648 is bigger than the max value, hence it is not possible to obtain the min value directly
// for the purposes of this application, this minor detail is irrelevant and we will simply ignore it
int myInt4 = -2147483647;
int myInt5;