boolean whileTest = true ;
while ( whileTest )
    whileTest = false ;

int whileTest2 = 4 ;
while ( whileTest2 > 0 )
    whileTest2 = whileTest2 - 1 ;

int [ ] copyArray = new int [ ] { 1 , 2 , 3 } ;
int [ ] pasteArray = new int [ copyArray . length ] ;
int index = 0 ;
while ( index < copyArray . length )
{
    pasteArray [ index ] = copyArray [ index ] ;
    index = index + 1 ;
}

char [ ] helloChars = new char [ ] { 'h' , 'e' , 'l' , 'l' , 'o' } ;
char [ ] helloCharsCopy = new char [ helloChars . length ] ;
for ( int i = 0 ; i < helloChars . length ; i = i + 1 )
    helloCharsCopy [ i ] = helloChars [ i ] ;

boolean arraysAreEqual = true ;
while ( true )
{
    for ( int i = 0 ; i < 2 ; i = i + 1 )
        continue ;
    if ( helloChars . length != helloCharsCopy . length )
    {
        arraysAreEqual = false ;
        break ;
    }
    for ( int i = 0 ; i < helloChars . length ; i = i + 1 )
    {
        if ( helloChars [ i ] != helloCharsCopy [ i ] )
        {
            arraysAreEqual = false ;
            break ;
        }
    }
    break ;
}

char [ ] helloBackwards = new char [ helloChars . length ] ;
for ( int i = 0 ; i < helloChars . length ; i = i + 1 )
    helloBackwards [ i ] = helloChars [ helloChars . length - i - 1 ] ;

for ( int i = 0 ; ; i = i + 1 )
{
    if ( helloChars [ i ] == 'o' )
        break ;
    if ( helloChars [ i ] == 'l' )
        continue ;
    while ( true )
        break ;
}

boolean breakTest = true ;
while ( breakTest )
    break ;

int continueTest = 3 ;
while ( continueTest > 0 )
{
    int decrement = 1 ;
    continueTest = continueTest - decrement ;
    continue ;
    continueTest = continueTest + decrement ;
}

int toInitialise ;
int anInt = 5 ;
while ( true )
{
    int someInt = anInt ;
    if ( someInt > 7 )
    {
        int [ ] ints = new int [ ] { someInt , 1 } ;
        int second = ints [ 1 ] ;
        if ( second >= 1 )
        {
            if ( someInt == 8 )
            {
                anInt = anInt + 1 ;
                continue ;
            }
            toInitialise = 5 ;
            break ;
        }
    }
    someInt = someInt + 1 ;
    anInt = someInt ;
}