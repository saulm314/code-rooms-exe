int[]intArr=new int[]{5,6};
intArr=null;

int[]intArr2=new int[]{7,8};
int[]intArr3=intArr2;
intArr2=null;

int[]intArr4=new int[]{9,10};
int[]intArr5=intArr4;
intArr4=null;
intArr5=null;

if(true)
    int[]intArr6=new int[]{11,12};

for(int i=0;i<3;i=i+1)
    int[]intArr7=new int[]{13+i,14+i};

if(false);
else int[]intArr8=new int[]{18,19};

while(true)
{
    int[]intArr9=new int[]{20,21};
    break;
}

string myStr="Hello";
myStr=null;

string myStr2="Hi";
myStr2="Bye";

string[]animals=new string[]{"Cat","Dog","Rabbit"};
string animal=animals[1];
animals=null;

string[]animals2=new string[]{"Bird","Fox"};
string animal2=animals2[1];
animals2=null;
animal2=null;

string[]animals3=new string[]{"Moose"};
string animal3=animals3[0];
animals3[0]=null;

string animal4="Bat";
string[]animals4=new string[]{animal4};
animal4=null;

string animal5="Mouse";
string[]animals5=new string[]{animal5};
animal5=null;
animals5=null;

char[]charArr=new char[]{'a','b','c'};
string abc=new string(charArr);
charArr=null;

string xyz=new string(new char[]{'x','y','z'});
string[]pqr=new string[]{new string(new char[]{'p','q'}),"r"};

bool check="=="=="==";