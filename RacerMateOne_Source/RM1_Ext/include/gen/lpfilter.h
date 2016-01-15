
#ifndef _LPFILTER_H_
#define _LPFILTER_H_

#include <windows.h>

class LPFilter  {

	private:
		double fval;
		LPFilter(const LPFilter&);
		LPFilter &operator = (const LPFilter&);		// unimplemented

	public:
		double n;
		LPFilter(void);
		LPFilter(int _n);
		virtual ~LPFilter(void);
		double calc(double in);
		void setTC(double _n);
		double getTC(void);
		void setfval(double _fval);
		double getfval(void)  {return fval;}
		void reset(void);
};

#endif		// #ifndef _X_H_

