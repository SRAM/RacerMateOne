
#ifndef _VDEFINES_H_
#define _VDEFINES_H_

#include "config.h"								// using quotes so resources will compile
#include <vector>

#define GRXSIZE 512
#define PACKETLEN 10
#define TENBIT
#define NEWERGO

#ifdef VELOTRON
	//#define DOUPKEY

	#ifdef LOG_GEARING
#pragma pack(push, 1)						// for old borland compatibility
		typedef struct  {
			unsigned char gear;
			unsigned char frontGear;
			unsigned char rearGear;
			unsigned char fill;									// to get a 4-byte structure
		} GEARINFO;
#pragma pack(pop)
	#endif

#endif


#ifndef VELOTRON
	#define MAX_EXPORT_ITEMS 19
#else
	#define MAX_EXPORT_ITEMS 22			// added "gear", gearing, cadence, pp, spinscan bars... added drag factor
#endif


#define COMMSTRING "NO COMMUNICATION DETECTED"
#define OFFSTRING "OFF"
//#define FINISHEDSTRING "TEST OVER"

#define MINWEIGHT 30
#define MAXWEIGHT 350

#define VELOTRONMAXCHANS 7
#define HBMAXCHANS 10


enum  {
	IDLE,
	HALT
};

enum  {
	SOURCE_VELOTRON,
	SOURCE_COMPUTRAINER,
	SOURCE_VELOTRON_FILE,
	SOURCE_CDF_FILE,
	SOURCE_JIM_FILE,
	SOURCE_PACER
};

#define BACKGROUND_COLOR  RGB(0, 0, 100)
#define STATUS_BACKGROUND RGB(0, 200, 200)
#define STATUS_FOREGROUND RGB( 0, 0, 0)
//#define MYCLASSNAME "Render Window"
//#define TEMPRTDNAME ".rtd"					// won't work for multi-rider

#define WFILTERCONSTANT 1200

typedef struct  {
	#ifdef VELOTRON
		float bars[72];							// stored as 10 * torque
	#else
		float bars[24];
	#endif

	#ifndef VELOTRON
		unsigned short rfdrag;
		unsigned short rfmeas;
	#endif

	unsigned long time;
	float mph;
	float watts;
	float hr;
	float rpm;
	float ss;
	float lss;
	float rss;
	float lwatts;
	float rwatts;
	float pp;							// pulse power
	int lata;
	int rata;
	float miles;
	float feet;
	unsigned short hrflags;
	unsigned short rpmflags;
	unsigned short minhr;				// what the handlebar sends back! meta.lower_hr is the setting sent to the hb
	unsigned short maxhr;				// what the handlebar sends back! meta.upper_hr is the setting sent to the hb
	float grade;							// -100.00 to +100.00
	float wind;
	float averageMPH;
	float averageWatts;
	float averageRPM;
	float averageHR;
	float averagePP;

	float inZoneHr;

	float peakMPH;
	float peakWatts;
	float peakHR;
	float peakRPM;
	float peakPP;

	float load;
	float faccel;
	float rawAcceleration;
	unsigned char tdc;
	unsigned char hrNoiseCode;
	unsigned short mcur;
	unsigned short volts;
	float rawRPM;
	float calories;
	float pedalrpm;

	unsigned char runSwitch;
	unsigned char brakeSwitch;

	unsigned char keys;					// the 6 hb keys + the polar heartrate bit
	unsigned char keydown;
	unsigned char keyup;
	unsigned char spareChar;
	float minutes;
	unsigned char gear;					// 0 - 255
	unsigned char rpmValid;
	unsigned char virtualFrontGear;
	unsigned char virtualRearGear;

	float average_lss;
	float average_rss;
	float average_ss;
	bool rescale;								// spinscan rescale flag
	float maxforce;							// spinscan force to scale to if rescale flag is set
} METADATA;

	typedef struct  {
		unsigned short bars[72];							// stored as 10 * torque
		unsigned char tdc;

		float miles;							// distance in miles
		unsigned long ms;						// time in milliseconds

		// 0 - 255 rpm

		unsigned char rpm;
		unsigned char pedalrpm;
		unsigned char avgrpm;
		unsigned char maxrpm;

		// 0 - 255 hr

		unsigned char hr;
		unsigned char avghr;
		unsigned char maxhr;

		// watts: 0 - 65535 watts

		unsigned short watts;
		unsigned short avgwatts;
		unsigned short maxwatts;

		// 0 - 6553.5 mph: stored as mph * 10

		unsigned short mph;
		unsigned short avgmph;
		unsigned short maxmph;

		short grade;			// -10.0 to +25.0, stored as grade*10
		short wind;				// -3200.0 + 3200.0, stored as wind_mph*10
		unsigned char gear;	// 0 - 255
		unsigned char frontgear;
		unsigned char reargear;

		unsigned char ss;
		unsigned char lss;
		unsigned char rss;
		unsigned char avgss;
		unsigned char avglss;
		unsigned char avgrss;

		unsigned char lpower;
		unsigned char rpower;

		unsigned char inzone_hr;		// 0 - 100
		unsigned short pounds;			// 0 - 6553.5 pounds, stored as pounds*10

		float pp;
		float calories;

		unsigned char lata;
		unsigned char rata;

	} VEL_LOGPACKET;			// 128 bytes


#define KEY1		0x02
#define KEY2		0x04
#define KEY3		0x08
#define KEY4		0x10
#define KEY5		0x20
#define KEY6		0x40

	typedef struct  {
		bool metric;
		int appcode;
		float manualWattsStep;
		unsigned char reserved[500];
	} PRE_CONDITION;				// this MUST be 512 bytes long!! assert(sizeof(PRE_CONDITION)==512) !!

	typedef struct  {
		unsigned long n_file_records;
		unsigned char reserved[508];
	} POST_CONDITION;				// this MUST be 512 bytes long!! assert(sizeof(POST_CONDITION)==512) !!

enum  {
	REALTIME,
	FILEMODE
};

#ifdef VELOTRON

	typedef struct  {
		bool metric;
		int appcode;
		short year;
		char month;
		char day;
		char hour;
		char minute;
		char second;
		unsigned char reserved[497];
	} INITIAL_VELOTRON_CONDITION;

	typedef struct {
		unsigned char c[23];
	} OLD_HUE;						// obfuscated "unencrypted header", public because 'User' class uses it

#endif

typedef struct  {
	float meters;
	float grade;						// this is %grade (-15.00 to +15.00)
	float wind_kph;
} CRSLEG;


#endif		// _VDEFINES_H_
