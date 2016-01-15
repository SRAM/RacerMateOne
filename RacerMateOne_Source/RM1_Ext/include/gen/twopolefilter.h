
#ifndef _TWOPOLEFILTER_H_
#define _TWOPOLEFILTER_H_

#include <windows.h>


class twoPoleFilter  {

	public:
		typedef struct  {
			float fval;
			float a;
		} STATE;

		double n;

		twoPoleFilter(int _n);
		virtual ~twoPoleFilter(void);
		double calc(double in);
		void setState(double _fval, double _a);
		void getState(double *_fval, double *_a);
		void reset(void);

	private:
		double fval;
		double a;
		double k;
		twoPoleFilter(const twoPoleFilter&);
		twoPoleFilter &operator = (const twoPoleFilter&);		// unimplemented

};

#endif		// #ifndef _X_H_
