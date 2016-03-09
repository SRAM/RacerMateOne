#ifndef _PHYSICS_H_
#define _PHYSICS_H_

#include <windows.h>
#include <config.h>

#ifdef VELOTRON

#include <defines.h>

#include <ini.h>
#include <lpfilter.h>
#include <bike.h>
#include <float2d.h>
#include <vdefines.h>


#define NEWHYSTERESIS
#define TABLEVER 2
//#ifdef DOSS
#define DO93_STUFF

#define NWATTVALS 33
#define NRPMVALS 7

#include <logger.h>

class Physics  {
	friend class velotronRTD;
	friend class Calibrator;
	friend class Wingate;
	friend class velotronDecoder;
	friend class Polar;
	friend class Bars;
	friend class SSView;
	friend class scrollingChart;
	friend class SS;

	private:

		#define NEW_PHYSICS_TIMER		// tlm20040612, timer fix for Australians

		double measured_ergo_watts;

		double watts_factor;
		double net_watts;
		double wf_watts;
		double flywheel_watts;
		DWORD startTime;
		DWORD pausedTime;				// accumulates paused time
		DWORD pauseStartTime;

		int formula;
		double constantTorque;
		double torqueFP;							// torque in foot pounds
		double torqueNM;							// torque in newton meters
		double flywheelRadius;

		float constantCurrent;
		float constantWatts;

		int mode;
		void runWindLoad(void);
		unsigned long counter;						// for debugging
		unsigned long loggingStartTime;
		unsigned long loggingEndTime;
		double lowCurrent(double _rpm, double _watts);

		void cleanup(void);
		double lookupRPM, rpmLookupFactor;
		void logData(void);
		void manualLoadControl(void);
		void ergoDegauss(void);
		int ergoDegaussAmplitude;
		int ergoDegaussPeriod;
		int ergoDegaussCounter;
		int ergoPolarity;
		unsigned short ergoDegaussValue;
		void windDegauss(void);
		unsigned short windDegaussAmplitude;
		int windDegaussPeriod;
		int windDegaussCounter;
		int windPolarity;
		unsigned short windDegaussValue;
		BOOL degaussEnabled;
		bool ccd;							// constant current degaussing

		void runConstantTorque(void);
		bool paused;
		double e;
		double ie;
		double mamps;
		double hysteresis;
		double lastcurrent;
		double slope, lastslope;
		void adjustCurrent(void);
		Ini *ini;
		float2d *currents;
		void runConstantCurrent(void);
		double gain;
		char string[256];
		int attackCounter;
		int decayCounter;
		double wattseconds;
		double cerr;
		LPFilter *dwfilter;
		LPFilter *picCurrentFilter;
		LPFilter *accfilter;

#ifdef DOSS
		LPFilter *ss_acc_filter;							// for spinscan
		double ss_accel;				// low pass filtered rawAcceleration for spinscan
		double ss_inertialForce;
		double ss_force;
#endif

		LPFilter *rpmFilter;
		LPFilter *mphFilter;
		double Cd;
		BOOL testing;
		double G;
		double I;
		double flywheelMass;
		Logger *log;
		int bp;
		double mCurrent, bCurrent;
		BOOL first;
		double LMEF;
		double Kw;
		double TOFPS;
		double TOWATTS;
		double DLOC_SCL;
		double POUNDS_TO_SLUGS;
		double CALL_HZ;
		DWORD now;
		DWORD then;
		double getTableCurrent3(double _rpm, double _watts);
		double getTableWatts(double _rpm, double _amps);
		int map(double x1, double x2, double y1, double y2, double *m, double *b);
		double k1;
		double kf;

	public:
		enum  {
			WIND_LOAD_MODE,
			CONSTANT_TORQUE_MODE,
			CONSTANT_WATTS_MODE,
			CONSTANT_CURRENT_MODE
		};
		#define MINGRADE -10
		#define MAXGRADE 25

		typedef struct  {
			double acc;
			double rpm;
			double mph;
			double watts;
			double displayedWatts;
			double grade;
			unsigned char reserved[204];
		} STATE;

		void setDistance(double _meters);
		void set_weight_lbs(double _d);
		void setmiles(double _miles);


		void inc_factor(void)  {
			watts_factor += .01;
			if (watts_factor > 2.0)  {
				watts_factor = 2.0;
			}
			k1 = watts_factor * 23.7e-6;
			kf = 60*33000*k1/5280;					// .0088875
			return;
		}
		void dec_factor(void)  {
			watts_factor -= .01;
			if (watts_factor < 0.0)  {
				watts_factor = 0.0;
			}
			k1 = watts_factor * 23.7e-6;
			kf = 60*33000*k1/5280;					// .0088875
			return;
		}
		double inline get_measured_ergo_watts(void)  { return measured_ergo_watts; }
		double getLowTableWatts(double _rpm, double _amps);

		STATE state;
		inline double getWindPowerConstant(void)  {
			return k1;
		}
		inline double getWindForceConstant(void)  {
			return kf;
		}
		inline double get_watts_factor(void)  {
			return watts_factor;
		}

		double constantForce;
		bool wingate_inertial_force;
		bool wingate_flip_sign;
		double ifgain;

		void setwgif(bool _in)  {
			wingate_inertial_force = _in;
		}

		void setflip(bool _flip)  {
			wingate_flip_sign = _flip;
		}

		void setifgain(double _gain)  {
			ifgain = _gain;
		}

		void set_watts_factor(double _watts_factor)  {
			watts_factor = _watts_factor;
			if (watts_factor > 2.0)  {
				watts_factor = 2.0;
			}
			else if (watts_factor < 0.0)  {
				watts_factor = 0.0;
			}
			k1 = watts_factor * 23.7e-6;
			kf = 60*33000*k1/5280;					// .0088875
			return;
		}

		bool calibrating;
		Physics( Bike *_bike, double _personweight, double _watts_factor=1.0);
		virtual ~Physics(void);
		BOOL computeAcceleration(void);
		Bike *bike;
		void run(void);
		void test(void);

		void crowbarRPM(void);
		void updatePicCurrent(void);
		double personweight;
		double draftwind;
		double kilometers;
		BOOL logging;
		unsigned short currentCountToPic;
		double current;
		double currentFromPic;
		double mph;
		double mphFromPedals;
		double weight;

#ifdef DO93_STUFF
		double fps93;
		double lastfps93;
		double rawAcceleration93;
		double dv93;
		double faccel93;
		LPFilter *accfilter93;
		double if93;
		double force93;
#endif

		double fps;
		double ipm;			// inches per minute
		double fpsFromPedals;
		double masterCurveForce;
		double totalwind;
		double gradeForce;
		double force;
		double inertialForce;
		double lastfps;
		double dt;
		double windForce;
		double tireForce;
		BOOL ifenabled;
		void computeMass(void);
		double omega;
		double omega_0;
		double lastOmega;
		double alpha;
		double torque;
		double power;
		double riderInput;
		double dv;
		double mass;
		unsigned short mcur;
		LPFilter *wfilter;
		LPFilter *fwfilter1;
		LPFilter *fwfilter2;
		int decayDelay;
		int attackDelay;
		double decayFactor;
		double attackFactor;
		double Q;
		double clippingCurrent;
		void gradeUp(void);
		void gradeDown(void);
		BOOL diffrpm;
		DWORD rpmDiffCount;
		double sumAverageMPH, sumAverageWatts, sumAverageRPM;
		DWORD averageMPHcount, averageWattsCount, averageRPMcount;
		int stopFlag;
		double pgain;
		double igain;
		void reset(void);
		double getGrade(void)  { return grade; }
		double rpmFromHoles;			// the raw (from the pic) rpm computed from the flywheel holes
		double rpmFromPedals;		// the raw (from the pic) rpm computed from the cadence sensor
		double frpm;					// low pass filtered 'rpmFromHoles'
		double fmph;					// low pass filtered 'mph', which is computed from 'rpmFromHoles'
		double grade;					// grade in percent, 1% = '1', eg.
		double wind;					// wind speed in mph, - for headwind, + for tailwind
		double feet;					// distance in feet moved, computed from 'rpmFromHoles', gear, etc.
		double watts;					// lp filtered raw watts
		double displayedWatts;		// further lp filtering of watts, suitable for human display

		double averageMPH;
		double averageWatts;
		double averageRPM;
		double miles;					// same as 'feet' above converted to miles
		double Calories;				// Food Calories computed from highly LP filtered watts (displayedWatts)

		double peakMPH;
		double peakRPM;
		double peakWatts;
		double rawAcceleration;		// raw acceleration as it comes from the PIC in feet per second per second
		double faccel;					// low pass filtered rawAcceleration
		DWORD ms;						// current time in miliseconds
		void resetIntegrators(void);
		void pause(void);
		void resume(void);
		double getAcceleration(void);
		double getRPM(void);
		double getMPH(void);
		double getWatts(void);
		float getConstantWatts(void);
		double getDisplayedWatts(void);
		double getPicCurrent(void);

		void setMode(int);
		void setConstantCurrent(float _constantCurrent);
		void setConstantWatts(float _constantWatts);

		void setConstantTorque(double _constantTorque);
		void writeState(FILE *stream);
		void readState(FILE *stream);

};

#else
	#define MINGRADE -10
	#define MAXGRADE 25
#endif		// #ifdef VELOTRON
#endif		// #ifndef _PHYSICS_H_

