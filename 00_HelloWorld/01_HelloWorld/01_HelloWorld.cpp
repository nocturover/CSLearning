#include <iostream>

int main()
{
	int num;
	std::cout << "Input ID : ";
	std::cin >> num;
	std::cout << std::endl;

	if (typeid(num) == typeid(int) && num != 0) {
		std::cout << "connected By ID : " << num << std::endl;
	}

}
