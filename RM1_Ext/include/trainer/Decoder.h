
#ifndef _DECODER_H_
#define _DECODER_H_


#include <windows.h>

#include <config.h>
#include <defines.h>
#include <vdefines.h>
#include <logger.h>


#include <heartrate.h>
#include <physics.h>
#include <course.h>											// <<<<<<<<<<<<<<<<< culprit
#include <user.h>
#include <ss.h>
#include <formula.h>

typedef struct  {
	float min_hr; float max_hr; float avg_hr;
	float min_rpm; float max_rpm; float avg_rpm;
	float min_mps; float max_mps; float avg_mps;							// meters per second
	float min_pp; float max_pp; float avg_pp;
	float min_wpkg; float max_wpkg; float avg_wpkg;

	float min_watts; float max_watts; float avg_watts;
	float min_lwatts; float max_lwatts; float avg_lwatts;
	float min_rwatts; float max_rwatts; float avg_rwatts;

	float min_ss; float max_ss; float avg_ss;
	float min_lss; float max_lss; float avg_lss;
	float min_rss; float max_rss; float avg_rss;

	float min_lata; float max_lata; float avg_lata;
	float min_rata; float max_rata; float avg_rata;

	float calories;
	float meters;							// meters pedaled after started
	DWORD ms;								// miliseconds while started

} MINMAXAVG;


class Decoder  {
	friend class RPT;
	friend class dataSource;
	friend class handlebarRTD;
	friend class PreVelotronData;
	friend class RTD;
	friend class velotronMFD;
	friend class velotronRTD;
	friend class velotronSFD;
	friend class jimData;
	friend class MFD;
	friend class scrollingChart;
	friend class Ssv;
	friend class spinDown;
	friend class Rider;
	friend class Rider2;
	friend class simRTD;
	friend class demoSource;
	friend class BACourse;
	friend class BAEditor;
	friend class Computrainer;
	friend class Internet;
	friend class BikeApp;
	friend class ComputrainerBase;
	friend class CompuDelayPacer;
	friend class CompuPerf;
	friend class VIDPERF;

	protected:
		float startMiles;
		bool can_do_drag_factor;
		float watts_factor;
		bool ss_found;
		virtual void set_minmax(void);
		virtual void peaks_and_averages(void);
		bool hrvalid;;

		#ifdef LOGKEYS
		#error "LOGKEYS is defined"
		#endif

		#define LOGKEYS

		int id;
		MINMAXAVG mma;				// saved peaks and averages that don't get reset so a report can be printed after reset

		Formula *formula;
		int min_hr;
		float min_pp;
		float max_pp;
		float min_wpkg;					// watts per kg
		float max_wpkg;					// watts per kg
		float avg_wpkg;
		float min_rpm;
		float min_mph;
		float min_watts;
		float min_ss;
		float max_ss;
		float min_lss;
		float max_lss;
		float min_rss;
		float max_rss;
		float min_lwatts;
		float max_lwatts;
		float avg_lwatts;
		float min_rwatts;
		float max_rwatts;
		float avg_rwatts;
		int min_lata;
		int max_lata;
		float avg_lata;
		int min_rata;
		int max_rata;
		float avg_rata;
		float wpkg;								// watts per kg
		float kgs;


		float us, lastus, dus;
		int resets;
		DWORD totalMS;
		float totalLen;
		float lbs;
		void computePP(void);
		double totalMPH;
		double totalWatts;
		double totalRPM;
		double totalHR;
		float total_pedal_rpm;
		float totalLSS;
		float totalRSS;
		float totalSS;
		float totalPP;
		float totalLATA;
		float totalRATA;
		float total_lwatts;
		float total_rwatts;
		float total_wpkg;


		DWORD hravgcounter;
		DWORD inzonecount;
		DWORD averageCounter;
		void logmeta(void);
		Course *course;
		User *user;
		
		BOOL logging;
		char string[256];
		Logger *log;
		bool started;
		bool finished;
		bool finishEdge;
		bool paused;

	public:
		Decoder(int _segments=72, Course *_course = NULL, User *_user = NULL, int _id=0);
		virtual ~Decoder();
		virtual double inline get_measured_ergo_watts(void)  { return 0.0; }
		void setlbs(float _lbs)  { lbs = _lbs; }
		inline bool is_started(void)  { return started; }
		inline bool is_paused(void)  { return paused; }
		inline bool is_finished(void)  { return finished; }
		inline bool get_finish_edge(void)  { return finishEdge; }
		void set_finish_edge(bool b)  { finishEdge = b; }
		void set_finished(bool b)  { finished = b; }
		virtual void set_weight_lbs(double _d)  { return; }
		void setid(int _id)  { id = _id; }
		void set_course(Course *_course);
		virtual void set_miles(float _miles) {
			meta.miles = _miles;
		}

#ifdef VELOTRON
		virtual bool get_physics_pause(void)  { return false; }
		virtual void set_physics_pause(bool _b)  { return; }
#endif

		virtual void decode(unsigned char *packet, DWORD _ms=0)  {
			bp = 1;
			return;
		}

		bool get_can_do_drag_factor(void)  {
			return can_do_drag_factor;
		}

		int get_resets(void)  {
			return resets;
		}

		Formula *get_formula(void)  { return formula; }
		void fill_in_mma(void);

		#define DECODER_FUNC_PTR

		#ifdef VELOTRON
			virtual void set_grade(float _grade)  { return; }
		#endif

		#ifdef DECODER_FUNC_PTR
			void *object;
			void (*keydownfunc)(void *object, int index, int _key);	// function pointer to application level function
			void (*keyupfunc)  (void *object, int index, int _key);	// function pointer to application level function
		#endif

		virtual void update(void) { return; }					// added so demoSource's demoDecoder could do peaks, avgs, integration
		virtual void update(float) { return; }					// used by compupacer
		virtual void integrate(void) { return; }		// added for 3d software to stop jerking

		unsigned char lastkeys;
		unsigned char keys;
		virtual void reset(void);
		bool getFinished(void)  { return finished; }
		bool getStarted(void)  { return started; }
		bool getPaused(void)  { return paused; }

		void setPausedFlag(bool _paused)  { paused = _paused; }
		void set_started(void)  { started = true; }
		void set_total_len(float _totalLen)  {			// miles or minutes
			totalLen = _totalLen;
			return;
		}

		virtual void setDistance(float _meters)  {				// for compatibility with 3d
			meters = _meters;
			meta.feet = (float)METERSTOFEET * meters;
			meta.miles = (float)METERSTOMILES * meters;
			return;
		}

		int bp;
		unsigned char flags;
		int status;
		double dt;
		double dseconds;						// seconds as a double
		double lastseconds;

		unsigned long dms;
		unsigned long ms;
		long period;

		SS *ss;

		METADATA meta;
		DWORD packets;
		float rawspeed;						// for 3d compatibility
		float meters;							// for 3d compatibility
		double mps;								// meters per second
		float wind;								// for 3d compatibility, wind speed in kph

		unsigned short losig;			// 10 bit data now, raw signal from velotron
		unsigned short hisig;			// 10 bit data now, raw signal from velotron
		bool hrPluggedIn;
		double lastmph;
		int polarbit;

		unsigned short hbstatus;
		unsigned short version;

		double draft_wind;
		double weight;
		double drag_aerodynamic;
		double drag_tire;
		double targetpower;
		unsigned short voltsCount;
		double supplyVoltage;
		double amps;
		DWORD caltime;
		int hours;
		int minutes;
		int seconds;
		int tenths;
		void clear_averages(void);
		bool ssok;

	private:
		float ppd;
		void init_course(void);

	public:
		inline float get_watts_factor(void)  { return watts_factor; }
		virtual void set_watts_factor(float _f);
};


#endif		// #ifndef _X_H_

