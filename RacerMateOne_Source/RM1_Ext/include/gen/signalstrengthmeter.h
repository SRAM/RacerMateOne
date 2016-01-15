
#ifndef _SIGNALSTRENGTHMETER_H_
#define _SIGNALSTRENGTHMETER_H_

#include <float.h>
#include <defines.h>
#include <lpfilter.h>


/***********************************************************************************

***********************************************************************************/

class signalStrengthMeter  {

	private:
		double strength;
		double min;
		double max;
		int n;
		int samplecount;
		LPFilter *filter;
		bool first;
		signalStrengthMeter(const signalStrengthMeter&);
		signalStrengthMeter &operator = (const signalStrengthMeter&);		// unimplemented

	public:
		signalStrengthMeter(int _n);
		virtual ~signalStrengthMeter();
		double compute(double in);
};

#endif		// #ifndef _SIGNALSTRENGTHMETER_H_
