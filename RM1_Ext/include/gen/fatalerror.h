#ifndef _FATAL_ERROR_H_
#define _FATAL_ERROR_H_

#include <windows.h>
#include <stdio.h>


class fatalError  {

	private:
		const char *filename;
		int line;
		char str[256];
		fatalError &operator = (const fatalError&);		// unimplemented

	public:
		fatalError(const char *_filename, int _line, const char *msg=NULL);
		virtual ~fatalError();
};

#endif


