// star 1
double combinedArea = width * height + width2 * height2;

// star 2
double temp = combinedArea * 10.0;
double temp2 = temp + 0.5;
double temp3 = (double)(int)temp2;
combinedArea = temp3 / 10.0;

// star 3
int isOdd = myInt % 2;