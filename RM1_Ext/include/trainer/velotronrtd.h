
#ifndef _VELOTRONRTD_H_
#define _VELOTRONRTD_H_

#include <windows.h>

#include <config.h>

#ifdef VELOTRON

#include <vdefines.h>

#include <signalStrengthMeter.h>

#include <serial.h>
#include <ini.h>
#include <physics.h>
#include <heartrate.h>
#include <crf.h>
#include <velotrondecoder.h>
#include <ini.h>
#include <hue.h>

#include <rtd.h>

class velotronRTD : public RTD {

	friend class Debugger;
	friend class Calibrator;

	private:
		#define RXSIZE GRXSIZE		// 128
		#define UPDATE_MS 47			// was 50, but made 47 due to beat problems

		typedef struct  {
			unsigned long headerKey;			// not encrypted
			unsigned long type;					// encrypted with headerKey
			unsigned long len;					// encrypted with headerKey
			unsigned long dataKey;				// encrypted with headerKey
			unsigned long crc;					// encrypted with headerKey
		} SECTION_HEADER;

		long userOffset;
		HUE hue;

		TCRF data_estate;						// state of the encryptor at the start of the data in the file
		TCRF user_estate;						// state of the encryptor at the user data offset

		#ifdef COMPAT_3DP
			std::vector <float> gears;
		#endif


#ifndef COMPAT_3DP
		virtual void makeLogPacket(void);
		virtual void createOutputFile(void);
#endif

		DWORD logtime;
		DWORD LOGTIME;							// how often we should
		VEL_LOGPACKET lp;
		char lastsavepath[256];


		void setPicCurrent(unsigned short _currentCountToPic);
		long dataEnd;
		INITIAL_VELOTRON_CONDITION ic;
		virtual char * save(bool lastperf=false, bool _showDialog=true);
		bool rpmNotification;
		float polarsig;
		double polarStrength;
		double elStrength;
		void init(void);

		Ini *ini;
		LPFilter *rpmFilter;					// rpm filter for ergo mode since it doesn't come from physics' rpmFilter
		LPFilter *mphFilter;
		LPFilter *polarFilter;
		signalStrengthMeter *earLobeSSM;
		signalStrengthMeter *polarSSM;

		Heartrate *hr;
		BOOL packetsAvailable(void);
		unsigned char code[PACKETLEN];
		unsigned char packet[PACKETLEN];
		void destroy(void);
		DWORD syncErrors;
		DWORD checksumErrors;
		unsigned short rxinptr, rxoutptr;
		unsigned char *rxq;
		//Bike *bike;
		unsigned char b[3];
		unsigned short lastPicCurrentCount;
		DWORD version;
		unsigned long timeout;
#ifndef NO_PERFS
		void test_tmp(void);
#endif


	public:

		velotronRTD(int _appcode, Bike *, const char *, char *, Course *course, bool _metric, User *_user=NULL, float _weight=0.0f, char *_dataFileName=NULL, char *_logfname="ds.log", int _id=0);
		virtual ~velotronRTD(void);
		Serial *get_port(void)  { return port; }			// used by 3d software
		double getDisplayedWatts(void);
		void setmiles(double _miles);
		void set_start_miles(double _start_miles);

		int getNextRecord(DWORD _delay);
		int getPreviousRecord(DWORD _delay);
		virtual int updateHardware(bool _force=false);
		void startCal(unsigned short picCurrentCount);
		Physics *physics;
		void emergencyStop(int flag);
		void setGrade(double _grade);
		void gradeUp(void);
		void gradeDown(void);
		void setWind(double _wind);
		virtual void reset(void);
		virtual void setTimeout(unsigned long _timeout)  {
			timeout = _timeout;
			return;
		}

		void reset_rxq(void)  {
			memset(rxq, 0, RXSIZE*PACKETLEN);
			rxinptr = rxoutptr = 0;
			return;
		}
		void kill_keys(void);


		double getGrade(void)  { return physics->getGrade(); }
		virtual double getWatts(void);
		virtual float getConstantWatts(void);



		void pause(void);
		void resume(void);

		void start(void);

#ifdef TAKING_CURVES
		virtual void incGradeDelta(void);
		virtual void decGradeDelta(void);
#endif

		void setPhysicsMode(int);

		void setConstantForce(double _constantForce);
		void setConstantWatts(float _manualWatts);
		void setConstantCurrent(float _constantCurrent);
		virtual void flush(void);
};

#endif			// ifdef VELOTRON

#endif		// #ifndef _VELOTRONRTD_H_
