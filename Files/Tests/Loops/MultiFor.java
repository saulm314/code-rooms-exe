int myInt = 0 ;
int myInt2 = 0 ;
for ( int i = 0 ; i < 3 ; i = i + 1 )
{
    myInt = myInt + 1 ;
    myInt2 = myInt2 + 1 ;
}

int myInt3 = 0 ;
int myInt4 = 0 ;
for ( int i = 0 ; false ; i = i + 1 )
{
    myInt3 = myInt3 + 1 ;
    myInt4 = myInt4 + 1 ;
}

for ( ; false ; ) { }