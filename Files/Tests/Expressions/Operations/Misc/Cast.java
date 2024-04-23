int myInt = ( int ) 1 ;
double myDouble = ( double ) 1.0 ;
boolean myB = (boolean)true ;
char myChar = ( char ) 'a' ;

int [ ] intArr = new int [ 0 ] ;
String myStr = "" ;
String [ ] strArr = new String [ 0 ] ;

int [ ] intArr2 = ( int [ ] ) intArr ;
String myStr2 = ( String ) myStr ;
String [ ] strArr2 = ( String [ ] ) strArr ;

int [ ] intArr3 = ( int [ ] ) null ;
String myStr3 = ( String ) null ;
String [ ] strArr3 = ( String [ ] ) null ;

double myDouble2 = ( double ) 2 ;
int myInt2 = ( int ) 2.0 ;

int myInt3 = ( int ) ( int ) 5 ;
int myInt4 = ( int ) ( int ) ( int ) 5 ;
int myInt5 = ( int ) ( double ) ( int ) 5.0 ;