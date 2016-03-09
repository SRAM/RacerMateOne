
#ifndef _SMAV_H_
#define _SMAV_H_

#include <windows.h>

/**************************************************************************

**************************************************************************/

class sMav  {

	private:
		float *raw;								// raw data
		float *fval;							// filtered values

		bool dosigma;
		double *squares;
		double sigma;
		double squares_sum;

		int len;
		float sum;
		int k;
		int shiftlen;
		float curval;

	public:
		sMav(int n, bool dosigma=false);
		~sMav();
		float compute(float input);
		float first(void);
		float last(void);
		void reset(float val=0.0f);		// seeds the moving average
		void setShift(int n=0);					// sets the shift value from 0 to len-1
		inline float get_curval(void)  { return curval; }
		inline double get_sigma(void)  { return sigma; }

};

#endif		// #ifndef _X_H_

