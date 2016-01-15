#ifndef _OLDSS_H
#define _OLDSS_H

#include <defines.h>


class OldSS  {

	friend class PreVelotronData;

	public:
		typedef struct  {
			float raw;					// raw value
			float fval;					// filtered value
			float a;						// for filter
			double degrees;
			double theta;				// angle for this thrust (radians)
			double r;					// thrust
			double sin;					// sine of that angle
			double cos;					// cos of that angle
			short color;				// color of bar
			int x;						// polar x coord
			int y;						// polar y coord
			int oldx;
			int oldy;
		} THRUST2;

		OldSS(void);
		~OldSS(void);

	private:
		unsigned long filtercount;
		void filter(int filter_type, float dt);
		float unpack(unsigned short p);
		int lata;
		int rata;
		THRUST2 thrust[48];
		unsigned char raw[24];
		unsigned char raw_display[24];
		float leftss,oldleftss;
		float rightss,oldrightss;
		float ss,oldss;
		float leftwatts,rightwatts,oldleftwatts,oldrightwatts;
		float leftpower;
		float rightpower;
		float total_ss;
		float total_rss;
		float total_lss;
		float average_ss;
		float average_lss;
		float average_rss;

};

#endif

