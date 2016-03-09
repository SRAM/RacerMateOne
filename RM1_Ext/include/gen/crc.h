#ifndef _CRC_H_
#define _CRC_H_

#include <windows.h>

class CRC  {

	private:
		unsigned short updcrc(unsigned short crc, short c);
		CRC(const CRC&);
		CRC &operator = (const CRC&);		// unimplemented

	public:
		CRC(unsigned char *buf, int len);
		virtual ~CRC(void);
		unsigned short crc;
		void reset(void);
		void compute(unsigned char *buf, int len);
		inline unsigned short getcrc(void)  { return crc; }
};

#endif		// #ifndef _CRC_H_

