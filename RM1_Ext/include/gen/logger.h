
#ifndef _LOGGER_H_
#define _LOGGER_H_

#include <stdio.h>
#include <stdlib.h>
#include <windows.h>
#include <defines.h>

class Logger  {

	private:
		char currentDirectory[_MAX_PATH];
		char file[64];
		char stub[32];
		BOOL closed;
		void cleanup_logfiles(void);
		Logger(const Logger&);						// unimplemented
		Logger &operator = (const Logger&);		// unimplemented

	public:

		Logger(const char *stub);
		virtual ~Logger(void);
		void write(int level, int printdepth, int reset, const char *format, ...);
		void write(const char *format, ...);
		void close(void);
		void dump(unsigned char *mb, int cnt);
		void flush(void);
		int loglevel;
		FILE *stream;
		
};

#endif		// #ifndef _LOGGER_H_

