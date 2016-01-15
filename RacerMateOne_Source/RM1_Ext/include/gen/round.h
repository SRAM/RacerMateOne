#ifndef ROUND__H
#define ROUND__H

#include <math.h>

#if defined(__cplusplus) && __cplusplus

/***********************************************************************************

***********************************************************************************/

inline int myrint(double x)  {
	return (int)(x > 0 ? x+0.5 : x-0.5);
}

/***********************************************************************************

***********************************************************************************/

// round to integer

inline int iround(double x)  {
	//return (int)floor(x + ((x >= 0) ? 0.5 : -0.5));
	if (x>0)  {
		return ((int) (.5 + x));
	}
	else  {
		return ((int) (-.5 + x));
	}
}

// round number n to d decimal points

inline double fround(double n, unsigned d)  {
	return floor(n * pow(10.0, (int)d) + .5) / pow(10., (int)d);
}

#else

/*
** NOTE: These C macro versions are unsafe since arguments are referenced
**       more than once.
**
**       Avoid using these with expression arguments to be safe.
*/

/*
** round to integer
*/

#define iround(x) floor((x) + ((x) >= 0 ? 0.5 : -0.5))

/*
** round number n to d decimal points
*/

#define fround(n,d) (floor((n)*pow(10.,(d))+.5)/pow(10.,(d)))

#endif

#endif /* ROUND__H */


