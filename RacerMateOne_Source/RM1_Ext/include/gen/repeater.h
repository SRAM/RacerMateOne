#pragma once

#include <windows.h>


/**************************************************************************

**************************************************************************/

class Repeater  {

	private:
		DWORD now;
		DWORD lastnow;
		DWORD initial_timeout;
		DWORD timeout;
		
	public:
		Repeater(DWORD _timeout=1000L);
		~Repeater();
		bool doit(void);
		
};


