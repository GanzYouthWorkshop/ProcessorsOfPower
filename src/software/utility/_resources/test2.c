#define TEST 1
#define DEBUG

void main()
{
	int test = 1;
	int *ptr = &test;

	*ptr = 5;

	if (*ptr == 5)
	{
		test = 3;
	}

	return test;
}