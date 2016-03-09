
#ifndef _RTD_H_
#define _RTD_H_

#include <windows.h>

#include <string.h>

#include <config.h>

#include <serial.h>

#include <datasource.h>
#include <user.h>
#include <course.h>
#include <bike.h>

#ifdef COMPAT_3DP
	#ifdef VIDEO_APP
		#include "..\3d\coursefile.h"
	#else
		#include <coursefile.h>
	#endif
#endif


class RTD : public dataSource  {
	friend class Rider2;

	protected:
		#ifdef COMPAT_3DP
			PerformanceInfo pinf;
			PerfPoint pp;
			long perflenoffset;
			long perfoffset;
			long pinfoffset;
			void write_long_section(char *str, long i);
			void write_float_section(const char *str, float f);
			void write_floats_section(const char *str, std::vector <float> &vec);
			void write_string_section(char *secstr, char *str);
			std::vector <float> winds;
			std::vector <float> calories;

			#ifdef VELOTRON
				GEARINFO tmpgi;
				vector<GEARINFO> gi;
				void write_gear_section(void);
			#endif

			#ifndef NO_PERFS
				virtual void makelogpacket(void) { return; }
				virtual void createOutputFile(void) {return;  }
			#endif
		#endif

		bool show_errors;
		bool finalized;
		void destruct(void);

#ifndef NO_PERFS
		char dataFileName[256];
		void resetOutputFile(void);
		long dataStart;
#endif

		User *user;
		virtual void reset(void);
		bool saved;
#ifndef NO_PERFS
		virtual char * save(bool _showDialog=true) { return NULL; }
#endif
		DWORD lastCommTime;
		int CLEN;
		unsigned char *workbuf;
		int packetIndex;
		DWORD lastHWTime;
#ifndef NO_PERFS
		FILE *outstream;				// our stream to save real time data
		void close_outstream(void);
		char outfilename[256];		// the name of our file, eg, "1.tmp"
#endif
		int comportnum;
		int id;
		CRF *sout;								// encryptor for saving data files


	public:

		RTD(
				const char *_comportstr, 
				const char *_baudstr, 
				bool _metric, 
				User *_user, 
				Course *_course,
				int _appcode,
				const char *_dataFileName,
				const char *_logfname,
				int _id,								// the id of the computrainer or velotron, 0 = the first one, 1 = second one
				Bike *bike,
				bool _show_errors=true
			);

		virtual ~RTD(void);
			Serial *port;

#ifdef COMPAT_3DP
#ifndef NO_PERFS
		virtual int addcourse(const char *_control_file);
		void finalize_3dp(void);
#endif
#endif

#ifndef NO_PERFS
		const char * get_outfilename(void)  { return outfilename; }
#endif

		inline virtual bool is_realtime(void)  { return true; }

		unsigned long recordsOut;
		Course *getCourse(void) { return course;}
		virtual void setShutDownDelay(unsigned long _ms);

#ifndef NO_PERFS
		char *getDataFileName(void)  {
			return dataFileName;
		}
		void setDataFileName(char *_fname)  {
			strncpy(dataFileName, _fname, sizeof(dataFileName)-1);
			return;
		}
#endif

		virtual void pause(void) {
			return;
		}
};
#endif		// #ifndef _RTD_H_

