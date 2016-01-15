
//#pragma once
//#include <windows.h>

#ifndef _MY_MD5_H_
#define _MY_MD5_H_

#include <stdio.h>



/************************************************************************

************************************************************************/

typedef unsigned char *POINTER;		// POINTER defines a generic pointer type
typedef unsigned long int UINT4;		// UINT4 defines a four byte word

// MD5 context

typedef struct {
  UINT4 state[4];                                   /* state (ABCD) */
  UINT4 count[2];        /* number of bits, modulo 2^64 (lsb first) */
  unsigned char buffer[64];                         /* input buffer */
} MD5_CTX;

// Constants for MD5Transform routine.

#define S11 7
#define S12 12
#define S13 17
#define S14 22
#define S21 5
#define S22 9
#define S23 14
#define S24 20
#define S31 4
#define S32 11
#define S33 16
#define S34 23
#define S41 6
#define S42 10
#define S43 15
#define S44 21

/*
static void MD5Transform(UINT4 [4], unsigned char [64]);
static void Encode(unsigned char *, UINT4 *, unsigned int);
static void Decode(UINT4 *, unsigned char *, unsigned int);
static void MD5_memcpy(POINTER, POINTER, unsigned int);
static void MD5_memset(POINTER, int, unsigned int);
*/

static unsigned char PADDING[64] = {
  0x80, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
  0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
  0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
};

// F, G, H and I are basic MD5 functions.

#define F(x, y, z) (((x) & (y)) | ((~x) & (z)))
#define G(x, y, z) (((x) & (z)) | ((y) & (~z)))
#define H(x, y, z) ((x) ^ (y) ^ (z))
#define I(x, y, z) ((y) ^ ((x) | (~z)))

// ROTATE_LEFT rotates x left n bits.

#define ROTATE_LEFT(x, n) (((x) << (n)) | ((x) >> (32-(n))))

// FF, GG, HH, and II transformations for rounds 1, 2, 3, and 4.
// Rotation is separate from addition to prevent recomputation.

#define FF(a, b, c, d, x, s, ac) { \
 (a) += F ((b), (c), (d)) + (x) + (UINT4)(ac); \
 (a) = ROTATE_LEFT ((a), (s)); \
 (a) += (b); \
  }
#define GG(a, b, c, d, x, s, ac) { \
 (a) += G ((b), (c), (d)) + (x) + (UINT4)(ac); \
 (a) = ROTATE_LEFT ((a), (s)); \
 (a) += (b); \
  }
#define HH(a, b, c, d, x, s, ac) { \
 (a) += H ((b), (c), (d)) + (x) + (UINT4)(ac); \
 (a) = ROTATE_LEFT ((a), (s)); \
 (a) += (b); \
  }
#define II(a, b, c, d, x, s, ac) { \
 (a) += I ((b), (c), (d)) + (x) + (UINT4)(ac); \
 (a) = ROTATE_LEFT ((a), (s)); \
 (a) += (b); \
  }


#define TEST_BLOCK_LEN 1000
#define TEST_BLOCK_COUNT 1000



class MD5  {
	private:
		void init(MD5_CTX *);
		void update(MD5_CTX *, unsigned char *, unsigned int);
		//void final(unsigned char [16], MD5_CTX *);
		void final(MD5_CTX *);
		void transform(UINT4 state[4], unsigned char block[64]);
		void memcpy(POINTER output, POINTER input, unsigned int len);
		void memset(POINTER output, int value, unsigned int len);
		void encode(unsigned char *output, UINT4 *input, unsigned int len);
		void decode(UINT4 *output, unsigned char *input, unsigned int len);
		void timeTrial(void);
		void testSuite(void);
		void file(char *filename);
		void filter(void);
		//void print(unsigned char digest[16]);
		void print(void);
		FILE *outstream;
		void string(char *string);
		MD5 &operator = (const MD5&);		// unimplemented


	public:
		MD5(void);
		virtual ~MD5();
		int test(int argc, char *argv[]);
		unsigned char digest[16];
		void string(unsigned char *string, int len);

};


#endif


