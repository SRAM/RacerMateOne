
#ifndef _SS_H_
#define _SS_H_

#include <windows.h>

#include <config.h>


#include <logger.h>
#include <smav.h>
#include <lpfilter.h>
#include <twopolefilter.h>
#include <vdefines.h>

#ifdef VELOTRON
#include <ini.h>
#endif


#define CUTOFF_RPM 30


typedef struct  {
	float raw;
	float fval;
	float a;
	double center_degrees;
	double beginning_radians;			// for computing lata & rata
	double center_radians;			// for computing lata & rata
	double r;
	double sin;
	double cos;
	short color;
	int x;
	int y;
	int oldx;
	int oldy;
} THRUST;



class SS {
	friend class Polar;
	friend class Bars;
	friend class scrollingChart;
	friend class SC;
	friend class velotronDecoder;
	friend class Ssv;
	friend class dataSource;
	friend class velotronSFD;
	friend class jimData;
	friend class MFD;
	friend class handlebarDecoder;
	friend class internetDecoder;
	friend class Rider;
	friend class Rider2;
	friend class BACourse;

	private:

		double MIN_SS_FORCE;
		double MAX_SS_FORCE;


		bool reset_flag;
		float average_lss;
		float average_rss;
		float average_ss;

		double accum_ss;
		double accum_lss;
		double accum_rss;

		DWORD lastavgtime;
		unsigned long averageCounter;
		LPFilter *wattsFilter;
		void filter(void);
		void load(double rpm);
		int bp;
		float *lastTwoRevForces;
		void init(void);
		double averageForce;
		BOOL peddlingTimeout;
		BOOL primed;
		int inptr;
		int abovecount, belowcount;
		float thisforce;
		float *force;
		unsigned long loadcount;
		BOOL firstValidTDC;
		int middle;
		int *counts;
		DWORD ssrec;
		float TCD;
		float TCN;
		sMav *rpmMav;
		DWORD lastFilterTime;
		twoPoleFilter **tpf;

#ifdef VELOTRON
		Ini *ini;
#endif

	public:

		SS(int _SEGMENTS);
		~SS();
		void start(void);
		void set_reset(bool _b)  { reset_flag = _b; }

		float run(double _watts, double _force, double _rpm, unsigned char _tdc);

		Logger *log;
		void snapshot(char *_fname);
		int SSShift;
		inline int get_shift(void) { return SSShift; }
		void set_shift(int _i) { SSShift = _i; }
		float minlthrust, minrthrust;

		float totalss;
		float leftss;
		float rightss;
		float leftwatts;
		float rightwatts;
		float lata;								//leftAta;
		float rata;								//rightAta;
		int ilata;
		int irata;

#ifdef VELOTRON
		float filteredTorques[72];
		float thisRevForces[72];
		THRUST thrust[72];
#else
		float filteredTorques[48];
		float thisRevForces[48];
		THRUST thrust[48];
#endif

		float this_rev_max;
		double maxlthrust;
		double maxrthrust;
		bool metric;
		double degperseg;
		int segments;
		bool rescale;
		int size;
		void computeAverages(void);
		void reset(void);
};

#endif		// #ifndef _X_H_

