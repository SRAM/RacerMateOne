#pragma once

#ifdef WIN32
	#ifndef STRICT
		#define STRICT
	#endif
	#include <windows.h>
#endif

/************************************************************************

************************************************************************/

class float2d  {

	protected:

	private:
		int rows;
		int cols;
		float2d(const float2d&);
		float2d &operator = (const float2d&);		// unimplemented

	public:
		float2d(void);
		float2d(int _rows, int _cols);
		virtual ~float2d(void);
		float **v;

};



