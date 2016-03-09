#ifndef _CRF_H_
#define _CRF_H_

#include <stdio.h>

#define DEBUG_RC4

#define KEYLEN 64
#define SEEDLEN 7


typedef struct {
	unsigned char s[256];
	int i;
	int j;
} TCRF;


class CRF  {

	private:
		TCRF x;
		char seedstr[SEEDLEN];
		CRF(const CRF&);
		CRF &operator = (const CRF&);		// unimplemented
		bool disabled;

	public:

		CRF(bool _disabled=false);
		virtual ~CRF(void);
		void setDis(bool _dis)  {
			disabled = _dis;
		}
		bool getDis(void)  {
			return disabled;
		}
		void init(unsigned char *seed, unsigned char *password);
		void init(void);
		void generate_seedstr(void);
		void doo(unsigned char *buf, int len);
		void unwind(int n);
		bool do_file(char *infile, char *outfile);
		bool initialized(void);
		void save_encrypt_state(void);
		void restore_encrypt_state(void);
		int geti(void);
		int getj(void);
		void getstate(TCRF *);
		void setstate(TCRF *_x);

};


#endif
