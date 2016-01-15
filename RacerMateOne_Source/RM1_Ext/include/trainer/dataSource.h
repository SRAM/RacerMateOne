
#ifndef _X_H_
#define _X_H_

#include <windows.h>

#include <config.h>
#include <logger.h>
#include <defines.h>
#include <vdefines.h>
#include <decoder.h>									// <<<<<<<<<<<<< culprit
#include <course.h>
#include <user.h>
#include <bike.h>

//#define NEWMETRIC

class dataSource {
	friend class Bars;
	friend class Polar;
	friend class Report;
	friend class Rider2;
	friend class scrollingChart;
	friend class Ssv;
	friend class BikeApp;
	friend class BACourse;
	friend class Mbox;

	private:
		double MIN_SS_FORCE;
		double MAX_SS_FORCE;
		int abovecount;
		int belowcount;
		char logfname[32];
		void init(void);

	protected:
		bool initialized;
		bool registered;
		Bike *bike;
		unsigned long bytes_out;
		unsigned long bytes_in;
		bool beepflag;
		float anerobic_threshold;
		VEL_LOGPACKET lp;
		void makeMeta(void);

		int appcode;
		void beep(void);
		unsigned long lastbeeptime;

		HWND phwnd;					// in case we want to pass in the parent window for a progress bar, eg

		float ppd;
		float lbs;
		Logger *log;
		char string[256];
		DWORD resetTime;
		int bp;
		int lastix;
#ifndef NEWMETRIC
		bool dsmetric;				// the metric menu state, now using the global gMetric!!!
#endif
		Course *course;
		long file_size;
		long firstRecordOffset;
		bool last_connected;
		unsigned long shutDownDelay;
		char export_name[256];

	public:

#ifndef NO_PERFS
		virtual int addcourse(const char *_control_file=NULL)  { return 0; }
#endif

		virtual char *getvidbase(void)  { return NULL; }
		virtual char *getvidfile(void)  { return NULL; }
		inline virtual bool is_realtime(void)  { return false; }
		inline bool is_initialized(void)  { return initialized; }

		void set_course(Course *_course);
		void sr(bool _r)  { registered = _r; }
		float get_np(void);
		float get_tss(void);
		float get_if(void);
		float get_avg_formula_watts(void);

		virtual void keydown(WPARAM wparam) { return; }
		bool get_metric(void)  {
#ifdef NEWMETRIC
			return gMetric;
#else
			return dsmetric;
#endif
		}

		#ifdef DO_HISTOGRAMS
		enum  {											// histogram index's
			HISTOGRAM_RPM,
			HISTOGRAM_HR,
			HISTOGRAM_SPEED,
			HISTOGRAM_WATTS,
			HISTOGRAM_NUMBER_OF_INDICIES			// keep at the end
		};
		static const char *histoName[HISTOGRAM_NUMBER_OF_INDICIES];
		#endif

		inline unsigned long get_bytes_in(void)  {return bytes_in;}
		inline unsigned long get_bytes_out(void)  {return bytes_out;}

		void set_total_len(float _totalLen)  {			// miles or minutes
			decoder->set_total_len(_totalLen);
			return;
		}

		Course::OLD_COURSE_HEADER cheader;			// COURSE_HEADER
		USER_DATA udata;
		USER_DATA *get_udata(void)  {return &udata;}

		inline void flush_keys(void)  {
			decoder->meta.keys = 0;
			decoder->meta.keydown = 0;
			decoder->meta.keyup = 0;
#ifndef BIKE_APP
			decoder->lastkeys = 0;
#endif
			return; 
		};

		Course::OLD_COURSE_HEADER *get_course_header(void)  { return &cheader; }
		Course *get_course(void)  { return course;  }

		/*
		this is the code stored in filemode data only. In realtime mode, appcode = -1
		This appcode is set from the INITIAL_CONDITION structure in the data file header.
		*/
		enum  {
			REGULAR_CHARTS_APPCODE,		// charts in windload mode
			ERGO_CHARTS_APPCODE,			// charts in ergo mode
			MANUAL_CHARTS_APPCODE,		// manual charts
			SPINSCAN_APPCODE,				// spinscan
			CALIBRATOR_APPCODE,
			DEBUGGER_APPCODE,
			WINGATE_APPCODE,
			THREE_D_APPCODE,
			MULTI_APPCODE,
			METABOLIC_APPCODE,
			INTERNET_APPCODE,
			SERVICE_APPCODE,				// for running ctbg as a background service
			VIDEO_APPCODE,
			SPINDOWN_APPCODE,
			MULTIVID_APPCODE
		};
		Decoder *decoder;
		int readRate;
		bool connected;
		bool connection_changed;
		virtual void setTimeout(unsigned long timeout)  {
			return;
		}
		virtual char * getFirstName(void) { return NULL; }
		virtual char * getLastName(void) { return NULL; }

		virtual void start(void)  {
			return;
		}
		virtual void finish(void)  {
			return;
		}
		virtual int myexport(char *_fname) {return -1;}		// unimplemented by default
		DWORD records;				// 0 for realtime

		dataSource(HWND _phwnd=NULL, int _appcode=-1, const char *_logfname="ds.log", Bike *_bike=NULL);
		dataSource(int _appcode, Course *_course);		// constructor for video app

		virtual ~dataSource(void);

		virtual int getNextRecord(DWORD _delay) { return -1; }
		virtual int getPreviousRecord(DWORD _delay) { return -1; }
		virtual int getRecord(int k) {return -1;}
		virtual int get_nRecords(void) {return records;}

		#ifdef TAKING_CURVES
		virtual void incGradeDelta(void)  { return; }
		virtual void decGradeDelta(void)  { return; }
		#endif

		virtual bool out_of_data(void) { return false; }
		virtual BOOL packetsAvailable(void) { return FALSE; }
		virtual int seek(long rec) { return -1; }
		virtual int updateHardware(bool _force=false) { return 0; }
		virtual long getOffset(void) { return -1; }
		virtual void remap(RECT physrect, float mRecsToPhys, float bRecsToPhys) { return; }
		virtual void startCal(void) { return; };
		virtual void emergencyStop(int flag) { return; }
		virtual void gradeUp(void) { return; }
		virtual void gradeDown(void) { return; }
		virtual void reset(void);
		virtual double getGrade(void) { return 0.0; }
		virtual Course *getCourse(void) { return NULL; }		// eg, CDFData creates a course, otherwise, return NULL
		virtual int read(int k) { return 0; }						// read record k (file mode only)
		virtual int jumpmiles(float _miles) { return 0; };
		virtual void pause(void) { 
			return; 
		}
		inline bool is_paused(void)  {
			return decoder->paused;
		}
		inline bool is_started(void)  {
			return decoder->started;
		}
		inline bool is_finished(void)  {
			return decoder->finished;
		}
		virtual void resume(void) { return; }
		virtual void test(void) { return; }					// general purpose routing for testing, doesn't do anything
																		// in particular, can do anything the I want the child
																		// class to do.
		virtual double getTotalMiles(void)  {return 0.0; /*return totalMiles*/}		// hack for .3dp files  embedded course length did not match the pinfo header course length.
		virtual unsigned char getControlByte(void)  {
			return 0;
		}
		virtual float get_start_miles(void)  { return 0.0f; }
		inline bool hrbeep_enabled(void)  {
			return beepflag;
		}

		//----------------
		// setters:
		//----------------

		void setAnerobicThreshold(float _f)  {
			anerobic_threshold = _f;
		}
		void setFinished(void)  {
			decoder->finished = true;
			return;
		}
		void set_hrbeep(bool _b)  {
			beepflag = _b;
		}

		virtual void setConstantCurrent(float _constantCurrent)  { return; }
		virtual void setConstantWatts(float _constantWatts)  { return; }
		virtual void setwind(float _wind)  { return; }
		virtual void setdraftwind(float _wind)  { return; }
		virtual void set_miles(float _miles)  {
			//decoder->meta.miles = _miles;
			decoder->set_miles(_miles);
			return;
		}

		//-----------------
		// getters
		//-----------------

		char *get_export_name(void)  {
			return export_name;
		}

		float getAT(void)  {
			return anerobic_threshold;
		}
		virtual float getdraftwind(void)  {
			return 0.0f;
		}

		virtual bool is_perf(void)  { return false; }

		float getRPM(void)  { 
			return decoder->meta.rpm;
		}
		virtual double getWatts(void)  { return 0.0; }
		virtual float getConstantWatts(void)  { return 0.0f; }
		float gethr(void)  {
			return decoder->meta.hr;
		}
		
		virtual void clear_averages(void)  {
			return;
		}

		virtual void flush(void)  { return; }
		DWORD nextReadTime;
		DWORD lastReadTime;

		double xdelta;
		long left;
		long right;
		long cursorLeft;
		long cursorRight;
		long cursorWid;
		long winWid;

		bool eof;
		bool lasteof;
		bool eofedge;				// goes true once when we hit eof
		bool bof;
		bool lastbof;
		bool bofedge;				// goes true once when we hit bof

		virtual char * save(bool lastperf=false, bool _showDialog=true) { return NULL; }
		virtual void setPhysicsMode(int)  { return; }
		virtual void setShutDownDelay(unsigned long _ms)  { return; }

#ifdef DO_HISTOGRAMS
		virtual float *get_array(int _index) { return NULL; }
#endif

};
#endif		// #ifndef _X_H_

