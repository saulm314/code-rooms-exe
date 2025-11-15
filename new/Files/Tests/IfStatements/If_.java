int myInt=0;
if(false)
    myInt=1;
else if(false)
    myInt=2;
else if(false)
    myInt=3;
else if(true)
    myInt=4;
else
    myInt=5;

int myInt2=0;
if(true)
    if(true)
        myInt2=1;

int myInt3=0;
if(true)
    if(false)
        myInt3=1;
    else
        myInt3=2;

int myInt4=0;
if(false)
    if(false)
        myInt4=1;
    else
        myInt4=2;
else
    myInt4=3;

int myInt5=0;
if(true)if(true)if(true)if(true)myInt5=1;

int myInt6=0;
if(true)
{
    int[]intArr=new int[]{};
    myInt6=1;
}