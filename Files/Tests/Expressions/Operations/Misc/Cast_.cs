int myInt=(int)1;
double myDouble=(double)1.0;
bool myB=(bool)true;
char myChar=(char)'a';

int[]intArr=new int[0];
string myStr="";
string[]strArr=new string[0];

int[]intArr2=(int[])intArr;
string myStr2=(string)myStr;
string[]strArr2=(string[])strArr;

int[]intArr3=(int[])null;
string myStr3=(string)null;
string[]strArr3=(string[])null;

double myDouble2=(double)2;
int myInt2=(int)2.0;
int myInt3=(int)2.7;

int myInt4=(int)(int)5;
int myInt5=(int)(int)(int)5;
int myInt6=(int)(double)(int)5.0;