
#pragma once
#pragma warning(disable:4996 4995 4005 4822)

#include <windows.h>


class SE_Exception  {
	private:
		unsigned int nSE;
		DWORD address;
		SE_Exception &operator = (const SE_Exception&);		// unimplemented (gives error in vista)

	public:
		SE_Exception(unsigned int n, DWORD _address) {
			nSE = n;
			address = _address;
		}

    	virtual ~SE_Exception() {}

    	unsigned int getSeNumber(void) {
			return nSE; 
		}

    	DWORD getAddress(void) {
			return address; 
		}
};


