#ifndef COURSE_H_							// don't use _COURSE_H_ because _course.h uses that!!
#define COURSE_H_

#include <float.h>

#include <config.h>
#include <defines.h>
#include <vdefines.h>

#include <crf.h>
#include <logger.h>

#if defined VIDEO_APP
#elif defined MULTIVID_APP
#else
#endif


#define STATBACKGROUNDCOLOR  RGB(0, 0, 100)

#define DUMPBACKGROUNDCOLOR  RGB(100, 100, 200)
#define DUMPFGCOLOR  RGB(250, 250, 200)

#ifdef TOPO_APP
//#define EDIT_TITLE "    Leg              Distance                 Grade             Wind                  Accum. Dist."
//#define EDIT_TITLE "Edit Course"

typedef struct  {
	float length;
	float grade;
	float wind;
	int n;						// the leg number
} EDITLEG;
#endif


class Course  {
	friend class scrollingChart;
	friend class demoSource;

	public:
		typedef struct  {
			char fname[256];
			double minElevation;
			double maxElevation;
			long maxlaps;
			long minlaps;
			long maxsecs;
			long minsecs;
			double minlen;
			double maxlen;
			double mingrade;
			double maxgrade;
			double minangle;
			double maxangle;
			double mindist;
			double maxdist;
			double mindpm;			// "degrees per meter"
			double maxdpm;
			unsigned long version;
			long nsecs;
			double totalmeters;
			double total_miles;
			bool closed;
			double avggrade;			// total average grade
			double avggrade2;			// average grade for grades above 0 only
			double avgelev;
			double max_meters_gain;
			double difficulty;			// 0-4 = easy, 4-8 = medium, 8-10 = hard, 10+ = very hard
			double minwind;
			double maxwind;
			double avgwind;
			int laps;
			double total_up_feet;
			int nlegs;
			bool looped;
		} STATS;

		enum  {								// units enum
			FEET,
			METERS,
			MILES,
			KM
		};
		typedef struct  {
			char desc[256];
			char fname[256];
			char units[16];
			DWORD nlegs;
		} OLD_COURSE_HEADER;					//COURSE_HEADER;

		typedef struct  {
			float length;					// miles * 100
			float grade;					// percent * 10
			float wind;						// signed mph
		} OLD_COURSE_DATA;

		enum  {
			UNDEFINED,
			NONERGO,
			ERGO,
			THREED,
			VIDEO,
			EMPTY									// dummy course holder for spinscan-created files and manual charts
		};

		Course(void);
		Course(RECT *_courserect, char *_fname=NULL);


	#ifndef VIDEO_APP
		#ifndef MULTIVID_APP
			#ifndef PATENT_APP
//				Course(char * _fname, BOOL _enhanced, LPDIRECT3DDEVICE7 _pd3dDevice, RECT *_courserect);
			#endif
		#endif
	#endif
		virtual ~Course();

		virtual void reset_wind_lookup(void) { return; }

		double getmingrade(void)  { return cs.mingrade; }
		double getavggrade(void)  { return cs.avggrade; }
		double getmaxgrade(void)  { return cs.maxgrade; }

		double getminwind(void)  { return cs.minwind; }
		double getavgwind(void)  { return cs.avgwind; }
		double getmaxwind(void)  { return cs.maxwind; }
		double get_max_meters_gain(void)  { return cs.max_meters_gain; }
		bool get_looped(void)  { return looped; }

		char *getdesc(void)  {
			return header.desc;
		}
		char *getfilename(void)  {
			return fname;
		}

		char *getsignature(void)  {
			return signature;
		}

		virtual float getMinX(void)  {
			return 0.0f;
		}
		virtual float getMaxX(void)  {
			return 0.0f;
		}
		virtual float getMinY(void)  {
			return 0.0f;
		}
		virtual float getMaxY(void)  {
			return 0.0f;
		}

		virtual int *get_x_array(void)  {
			return NULL;
		}
		virtual float *get_y_array(void)  {
			return NULL;
		}
		virtual int getnpoints(void)  {
			return 0;
		}
		virtual int getnlegs(void)  {
			return 0;
		}

		static void background(HDC hdc, RECT *courserect);
		void setLaps(long _laps) { laps = _laps; }
		long getLaps(void) { return laps; }
		virtual POINT *getGradeChangeCoordinates(int *n)  {
			*n = 0;
			return NULL;
		}

		float mxToGDI, bxToGDI;
		float mxToPhys, bxToPhys;

		virtual int draw(HDC, HFONT, bool, double, bool _drawProfile=true, bool floodfill=true, int style=0, float _minxxscale=-FLT_MAX, float _maxxscale=-FLT_MAX, float _minyscale=-FLT_MAX, float _maxyscale=-FLT_MAX)  { return 0; }
		virtual void resize(RECT waverect)  { return; }
		virtual float getGrade(double _miles, int *_lastGrade_i=NULL)  { return 0.0f; }
		virtual float getGrade(int)  { return 0.0f; }			// gets the grade for leg 'i'

		virtual float getWind(double dist, int *_lastWind_i=NULL)  {
			return 0.0f;
		}

		virtual float getTotalLen(void)  { return 0.0f; }		// returns feet or minutes
		inline int getTotalLegs(void)  {								// accounts for laps!!!
			return totalLegs; 
		}

		virtual float getLoad(int i)  { return 0.0f; }
		virtual double getLoad(double _minutes)  { return 0.0; }

		virtual int gety(double)  { return 0; }
		virtual void writeEncrypted(FILE *outstream, CRF *s, FILE *_dbgstream=NULL);		// serializes the course to outstream

		virtual void exportLegs(FILE *stream)  { return; }

		char *getFileName(void);

		void writeEncryptedHeader(FILE *outstream, CRF *s);
		void readEncryptedHeader(FILE *instream, CRF *s);


		float *grade;
		float *wind;
		float *load;
		OLD_COURSE_HEADER header;
		virtual void test(void)  { return; }

#ifdef TOPO_APP
		virtual int edit(HICON icon) {
			bp = 1;
			return 0;
		}
		virtual int showInfo(void) {
			bp = 1;
			return 1;
		}
#endif


		virtual double getSummedMeters(void)  {
			return 0.0;
		}

		double getTotalMeters(void)  {
			return totalMeters;
		}

		int type;
		virtual void reset(void)  {
			bp = 1;
		}

		RECT *getRect(void)  { return courserect; }

		float getTotalMiles(void)  { return (float)totalMiles; }

		float getkm(void)  {
			return (float) (totalMeters / 1000.0);
		}



	private:
		char signature[32+1];							// md5 hash string of course for security
		BOOL enhanced;
		void init(void);
		void destroy(void);
#ifdef TOPO_APP
		void editLeg(void);
#endif
		bool changed;
		bool savequestion(void);

	protected:
		bool warned;
		int firstrow;
		int totalLegs;						// accounts for laps
		FILE *dbgstream;
		char fname[256];
		long laps;
		bool looped;
		Logger *logg;
		char string[256];
		FILE *stream;
		char fileline[256];
		int W;
		bool course_metric;			// passed into constructor
		RECT *courserect;
		RECT waverect;

		int bp;
		float myToGDI, byToGDI;
		float myToPhys, byToPhys;
		STATS cs;

		HWND statwnd;
		HBRUSH statbgbrush;
		HWND createStatWnd(void);
		static LRESULT CALLBACK statWndProc(HWND hwnd, UINT uMsg, WPARAM wParam, LPARAM lParam);
		void paintstats(HWND hwnd, HDC hdc);
		HINSTANCE hInstance;
		HWND phwnd;					// hwnd is the parent window
		HBRUSH oldbrush;

		double totalMeters;
		double totalMiles;
		double totalFeet;

		HDC dumphdc;
		HWND hlist;
		HBRUSH dumpbgbrush;
		HWND containerwnd;
		
#ifdef TOPO_APP
		static LRESULT CALLBACK containerWndProc(HWND hwnd, UINT uMsg, WPARAM wParam, LPARAM lParam);
		static BOOL CALLBACK legDialogProc(HWND hwndDlg, UINT iMessage, WPARAM wParam, LPARAM lParam);

		EDITLEG editleg;
		virtual void loadEditLeg(int i)  {
			editleg.length = 0.0f;
			editleg.grade = 0.0f;
			editleg.wind = 0.0f;
		}
		virtual void save_leg_to_memory(int i) {}
#endif

		bool closed;
		char saveName[_MAX_PATH];
		void saveAs();
		char defaultSavePath[_MAX_PATH];

	public:
		virtual void save(char *)  {};

};


#endif		// #ifndef _COURSE_H_


