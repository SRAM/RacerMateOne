
#ifndef _HUE_H_
#define _HUE_H_


#include <stdio.h>

/**************************************************************************

**************************************************************************/

class HUE  {

	public:
		#define SZ 23
		HUE(void);
		~HUE();
		void write(FILE *outstream);
		unsigned char *read(FILE *stream);
		int getversion(void);

	private:
		unsigned char buf[SZ];
		int version;
		static const unsigned char v2[23];				// version 2
		static const unsigned char v3[23];				// version 3

};
#endif		// #ifndef _HUE_H_

