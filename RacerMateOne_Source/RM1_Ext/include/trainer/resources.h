#ifndef _RESOURCES_H_
	#define _RESOURCES_H_

	#include "config.h"
	#include "vdefines.h"


	#define CHART_ICON                  101
	#define IDR_MAIN_ACCEL2              113
	#define IDR_CMD_CFG                 129
	#define CHART_MENU                  144
	#define IDD_DRIVERINFO              146
	#define IDC_PREV                    147
	#define IDC_NEXT                    148
	#define IDC_RADIO_DEVICE            149
	#define IDC_RADIO_HOST              150
	#define IDC_DWVENDORID              151
	#define IDC_DWDEVICEID              152
	#define IDC_DWSUBSYS                153
	#define IDC_DWREVISION              154
	#define IDC_VERSION2                155
	#define IDC_DEVICEID                156
	#define IDC_DESCRIPTION             157
	#define IDC_FILENAME2                158
	#define IDC_COUNT                   159
	#define IDC_GUID                    160
	#define IDC_STATIC_WHQLLEVEL        161

	#ifndef VIDEO_APP
		#define IDC_USER_NAME					300
		#define IDC_USER_AGE						301
		#define IDC_USER_WEIGHT					302
		#define IDD_LBS							303
		#define IDD_KGS							304
		#define IDD_SEX_MALE						305
		#define IDD_SEX_FEMALE					306
		#define IDC_USER_LOWER_HR				307
		#define IDC_USER_UPPER_HR				308
		#define IDC_DRAG							309		// tlm20080703 for wingate
	#endif			// #ifndef VIDEO_APP

	#ifndef VELOTRON
//		#define IDC_FTP							310		// tlm20080703 for wingate
	#else
		#ifdef VISTA
			//xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
			//#define IDC_DRAG                        44009
			//#define IDC_FTP                         44010
		#else
			#define IDC_USER_NAME					300
			#define IDC_USER_AGE						301
			#define IDC_USER_WEIGHT					302
			#define IDD_SEX_MALE						305
			#define IDD_SEX_FEMALE					306
			#define IDD_LBS							303
			#define IDD_KGS							304
			#define IDC_USER_LOWER_HR				307
			#define IDC_USER_UPPER_HR				308
			#define IDC_DRAG							309
			#define IDC_FTP							310
		#endif
	#endif

	#define CM_BAR_SPINSCAN					22000
	#define CM_SCROLL							22001
	#define CM_PHYSICS_TEST					22002
	#define CM_SOURCE_REALTIME				22004
	#define CM_SOURCE_FILE					22005
	#define CM_ZOOM							22006
	#define CM_POLAR_SPINSCAN				22007
	#define ID_BUTTON1						22016
	#define ID_BUTTON2						22017
	#define CM_SPEED							22018
	#define CM_FFTPLOT						22019
	#define IDM_VERSION						22020

	#ifndef VELOTRON
		#define CM_REGISTER				      22022
	#endif

	/*
	#define IDC_LEG_LENGTH				700
	#define IDC_LEG_GRADE				701
	#define IDC_LEG_WIND					702
	*/

	#define CM_IDLE							22023
	#define CM_RECORD							22034
	#define CM_COMM_TEST						22035
	#define ID_VELOTRON_BASE				23000
	#define ID_COMPUTRAINER_BASE			23050
	#define IDC_EXPORT2						24000		// reserve block of 30 here
	#define IDC_TOSS							24030
	#define IDM_BUILD_DATE					24031

	#define IDD_PORT_1        				24032
	#define IDD_PORT_2       				24033
	#define IDD_PORT_3        				24034
	#define IDD_PORT_4        				24035
	#define IDD_PORT_5        				24036
	#define IDD_PORT_6        				24037
	#define IDD_PORT_7       				24038
	#define IDD_PORT_8        				24039
	#define IDD_PORT_9       				24040
	#define IDD_PORT_10       				24041

	#define IDD_PORT_11       				24042
	#define IDD_PORT_12       				24043
	#define IDD_PORT_13       				24044
	#define IDD_PORT_14       				24045
	#define IDD_PORT_15       				24046
	#define IDD_PORT_16       				24047
	#define IDD_PORT_17       				24048
	#define IDD_PORT_18       				24049
	#define IDD_PORT_19       				24050
	#define IDD_PORT_20       				24051

	#define CHART_SAVE						400
	#define CHART_EXPORT_DATA				401
	//#define CHART_EXPORT_FORM				402
	//#define CHART_EXPORT_CD					403
	#define CHART_EDIT_EXPORT				404
	#define CHART_EXIT						405
	#define CHART_REALTIME					406
	#define CHART_NEW							407
	#define CHART_OPEN						408
	#define CHART_SETUP						409
	#define CHART_FILE						410
	#define CHART_VIEW						411
	#define CHART_TEST_COMM					412
	#define CHART_METRIC						415
	#define CHART_SAVE_REPORT				423
	#define CHART_START_CHART				416
	//#define CHART_REGISTER					417
	#define CHART_ABOUT						418
	#define CHART_SCREEN_REPORT			419
	#define CHART_PRINTER_REPORT			420
	#define CHART_SELECT_REPORT_FILE		421
	#define CHART_START_SPINSCAN			422
	//#define CHART_DEFAULT_GEAR				423

	#ifndef BIKE_APP
		#define IDC_SERIALNO					424
		#define IDC_REGISTRATION			425
	#endif

	#define IDC_DIAMETER					426

	//#ifdef VELOTRON
	#define ENTER_CAL 428
	#define CHART_CALIBRATE						427
	#define GEAR_ENTRY							429
	#define CHART_GET_VELOTRON_SETTINGS		440
	#define WINGATE						441
	#define WINGATE_SETUP				442
	//#endif

	#define SCREEN_CAPTURE				431

	//#ifdef VELOTRON
		#define CHART_OPTIONS				432
		#define CHART_START_MANUAL_CHART		430
	//#endif

	#ifdef VELOTRON
		#define IDC_FRONT1						450					// inside sprocket
		#define IDC_FRONT2						451
		#define IDC_FRONT3						452					// outside sprocket

		#define IDC_REAR1							453					// inside sprocket
		#define IDC_REAR2							454
		#define IDC_REAR3							455
		#define IDC_REAR4							456
		#define IDC_REAR5							457
		#define IDC_REAR6							458
		#define IDC_REAR7							459
		#define IDC_REAR8							460
		#define IDC_REAR9							461
		#define IDC_REAR10						462					// outside sprocket
		#define IDC_USE_DEFAULT_VELIB			463					// tlm20050519
		#define IDC_SPROCKET						464					// outside sprocket
		//#define IDC_USE_DEFAULT_GEAR			464					// tlm20050519

		#define ID_PRGB							470
		#define IDC_FACTORY_CALIBRATION		471

		// wingate class items:
		#define IDC_WINGATE_TOOL_BAR			500					// wingate toolbar
		#define IDC_WINGATE_STATUS_BAR		501					// wingate toolbar
		#define IDB_WINGATE_START				502
		#define IDB_NEXT							503
		#define IDB_STOP							504
		#define IDB_RESET							505
		#define IDB_PRINT							506
		#define IDB_LOADUP						507
		#define IDB_LOADDOWN						508
		#define IDB_WINGATE_START_DISABLED	509
		#define IDC_WINGATE_START				510
		#define IDC_STOP							511
		#define IDC_RESET							512
		#define IDC_PRINT							513
		#define IDC_STATUS_BAR					514
		#define IDC_LOADUP						515
		#define IDC_LOADDOWN						516
	#endif

	#define KILL_APP 0

	//enum {
	//	KILL_APP
	//	//ABC_XYZZY_XYZ			// needed now because enums need more that 1 item
	//};

#endif			// #ifndef _RESOURCES_H_
