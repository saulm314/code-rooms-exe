int [ ] intArr = new int [ ] { 5 , 6 } ;
intArr = null ;

int [ ] intArr2 = new int [ ] { 7 , 8 } ;
int [ ] intArr3 = intArr2 ;
intArr2 = null ;

int [ ] intArr4 = new int [ ] { 9 , 10 } ;
int [ ] intArr5 = intArr4 ;
intArr4 = null ;
intArr5 = null ;

if ( true )
    int [ ] intArr6 = new int [ ] { 11 , 12 } ;

for ( int i = 0 ; i < 3 ; i = i + 1 )
    int [ ] intArr7 = new int [ ] { 13 + i , 14 + i } ;

if ( false ) ;
else int [ ] intArr8 = new int [ ] { 18 , 19 } ;

while ( true )
{
    int [ ] intArr9 = new int [ ] { 20 , 21 } ;
    break ;
}

String myStr = "Hello" ;
myStr = null ;

String myStr2 = "Hi" ;
myStr2 = "Bye" ;

String [ ] animals = new String [ ] { "Cat" , "Dog" , "Rabbit" } ;
String animal = animals [ 1 ] ;
animals = null ;

String [ ] animals2 = new String [ ] { "Bird" , "Fox" } ;
String animal2 = animals2 [ 1 ] ;
animals2 = null ;
animal2 = null ;

String [ ] animals3 = new String [ ] { "Moose" } ;
String animal3 = animals3 [ 0 ] ;
animals3 [ 0 ] = null ;

String animal4 = "Bat" ;
String [ ] animals4 = new String [ ] { animal4 } ;
animal4 = null ;

String animal5 = "Mouse" ;
String [ ] animals5 = new String [ ] { animal5 } ;
animal5 = null ;
animals5 = null ;

char [ ] charArr = new char [ ] { 'a' , 'b' , 'c' } ;
String abc = new String ( charArr ) ;
charArr = null ;

String xyz = new String ( new char [ ] { 'x' , 'y' , 'z' } ) ;
String [ ] pqr = new String [ ] { new String ( new char [ ] { 'p' , 'q' } ) , "r" } ;

boolean check = "==" == "==" ;