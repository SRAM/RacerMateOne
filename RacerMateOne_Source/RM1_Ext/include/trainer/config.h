
#ifndef _MYCONFIG_H_
#define _MYCONFIG_H_

#include "../gen/defines.h"				// using explicit path for resources compiling to work

#ifdef TOPO_APP
	#error "TOPO_APP"
#endif

#ifdef BIKE_APP
	#error "BIKE_APP"
#endif

#ifdef MULTI_APP
	#error "MULTI_APP"
#endif

#ifdef LODE_APP
	#error "LODE_APP"
#endif


#ifdef WINGATE_APP
	#error "WINGATE_APP"
#endif

#ifdef HICAP_APP
	#error "HICAP_APP"
#endif

#ifdef INTERNET_APP
	#error "INTERNET_APP"
#endif

//----------------------------------------------------------------
// only one of these must be defined, or nothing for coaching 1.5
//----------------------------------------------------------------

//#define CONTROL_APP				// control file generator for VIDEO_APP
//#define MULTIVID_APP
//#define MULTI_APP
#define BIKE_APP				// building for 3d software, to include the correct resource files
//#define PATENT_APP
//#define LODE_APP					// for metabolic cart / lode emulator
//#define VIDEO_APP

//#define COACHING_APP
//#define WINGATE_APP

//#define TOPO_APP				// course creator & garmin2
//#define HICAP_APP
//#define INTERNET_APP
//#define MISC_APP

//#define DO_DRAG

#define LOG_CALORIES
//#define PERFINFO2


#if defined TOPO_APP														// topo software
	#define RESOURCE_PATH "x:\\_f\\topo2.0\\resource.h"
	//#define RESOURCE_PATH "x:\\_f\\3dv3\\resource.h"
	#define CLASSNAME		"FloEarthClass"
	#define WINDOWNAME	"Course Creator"

#elif defined MULTI_APP													// multi rider software
	#define CLASSNAME		"MultiRiderClass"
	#define WINDOWNAME	"Multi Rider Window"
	//#define VELOTRON	// NOTE! CHANGE THE RC FILE IF YOU CHANGE THIS!!!
	#define RESOURCE_PATH "x:\\_f\\multi3\\resource.h"

#elif defined MULTIVID_APP													// multi rider software
	#define CLASSNAME		"MRVidClass"
	#define WINDOWNAME	"Multi Rider Video Window"
	//#define VELOTRON	// NOTE! CHANGE THE RC FILE IF YOU CHANGE THIS!!!
	#define RESOURCE_PATH "x:\\_f\\multivid\\resource.h"
	#define COMPAT_3DP		// we will do 3dp compatible performance logging
	///////#define NO_PERFS				// DON'T SAVE PERFORMANCE FILES

#elif defined BIKE_APP													// 3d software
	#define NEW_TRAINER_WAY
	#define RESOURCE_PATH ".\\resource.h"
	#define CLASSNAME	"BikeClass"
	#define COMPAT_3DP		// we will do 3dp compatible performance logging
	#ifdef _DEMO
		#error "_DEMO"
	#endif
	//#define _DEMO		// FOR DEMO VERSION ONLY!!
	// if VELOTRON is defined, select _velotron.rc in the resource files, otherwise select _bike.rc for computrainer!!!!
	// DO THIS FOR THE RELEASE VERSION AS WELL!!!
	//#define VELOTRON
	#ifndef VELOTRON
		#ifdef INTERNET
			#error "INTERNET ALREADY DEFINED"
		#endif
		//////////////////////////////#define INTERNET			// for internet racing
	#endif
	#define THREEDV2
	//#define SHOW_PP
	#define WINDOWNAME	"3D Software"
	#ifndef _DEMO
		//#define TOYOTA
	#endif
#elif defined WINGATE_APP
	#define CLASSNAME		"WGClass"
	#define VELOTRON
	#define COMPAT_3DP		// we will do 3dp compatible performance logging
	#define RESOURCE_PATH "x:\\_f\\lib\\velib\\resources.h"
	#define WINDOWNAME	"Wingate"

//#elif defined COMPUTRAINER												// computrainer 1.5
//	#define CLASSNAME		"AppsClass"
//	#define RESOURCE_PATH "x:\\flo4\\vel2\\resource.h"
//	#define WINDOWNAME	"Computrainer Coaching Software"


#elif defined LODE_APP
	#define CLASSNAME		"LodeClass"
	#define VELOTRON
	#define RESOURCE_PATH "x:\\_f\\coaching\\resource.h"		// fryk
	#define WINDOWNAME	"Lode"

#elif defined HICAP_APP													// hicap tester
	#define CLASSNAME		"HicapClass"
	#define WINDOWNAME	"Hicap Window"
	#define VELOTRON	// NOTE! CHANGE THE RC FILE IF YOU CHANGE THIS!!!

#elif defined VIDEO_APP
	#define CLASSNAME	"vid_class"
	#define WINDOWNAME	"vidwindow"
	#define RESOURCE_PATH "x:\\_f\\movie\\resource.h"
	#define COMPAT_3DP		// we will do 3dp compatible performance logging
	#define VELOTRON	// NOTE! CHANGE THE RC FILE IF YOU CHANGE THIS!!!
	#define MERGE

#elif defined PATENT_APP
	#define CLASSNAME	"vid_class"
	#define WINDOWNAME	"vidwindow"
	#define RESOURCE_PATH "x:\\_f\\patent\\resource.h"
//#else		// coaching
#elif defined COACHING_APP
	#define HANDLE_LAPS

	#define CLASSNAME		"AppsClass"
	#define VELOTRON	// NOTE! CHANGE THE RC FILE IF YOU CHANGE THIS!!!  (FOR BOTH RELEASE AND DEBUG MODES!!!!)
	//#define COMPAT_3DP		// we will do 3dp compatible performance logging

	#ifdef VISTA
		#define RESOURCE_PATH "x:\\_f\\coaching\\resource.h"
	#else
		#define RESOURCE_PATH "x:\\_f\\coaching\\resource.h"
	#endif

	// #define HOSPITAL

	#ifdef HOSPITAL
		#define WINDOWNAME	"Hospital Software"
	#else
		#ifndef VELOTRON
			#define WINDOWNAME	"CompuTrainer CS 1.6"
		#else
			#define WINDOWNAME	"Velotron Coaching Software (CS)"
		#endif
	#endif

	//#define DO_HISTOGRAMS

#elif defined MISC_APP
	#ifdef VISTA
		#define RESOURCE_PATH "x:\\_f\\coaching\\resource.h"
	#else
		#define RESOURCE_PATH "x:\\_f\\coaching\\resource.h"
	#endif
#else
	#error "NO APP DEFINED IN CONFIG.H"
#endif

#ifdef VELOTRON
	#define LOG_GEARING
#endif

#define CONFIG_TRAP			// so main.rc won't compile unless config.h is included

#ifdef WIN32
	#ifndef __WIN32__
		#define __WIN32__				// for xbase64
	#endif

	#ifndef __MSVC__				// for xbase64
		#define __MSVC__				// for xbase64
	#endif
#endif

#endif			// #ifndef _MYCONFIG_H_

