
#ifndef _AMPLIFIER_H__
#define _AMPLIFIER_H_

#include <lpfilter.h>

class Amplifier  {

	private:
		double gain;
		double avg;
		double output;
		double m;
		double b;
		LPFilter *filter;
		Amplifier(const Amplifier&);
		Amplifier &operator = (const Amplifier&);		// unimplemented

	public:
		Amplifier(double _gain);
		virtual ~Amplifier(void);
		double calc(double in);
};

#endif		// #ifndef _X_H_

