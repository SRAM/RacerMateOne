
#ifndef _BIKE_H_
#define _BIKE_H_

#include <windows.h>

#include <stdio.h>

#include <defines.h>
#include <config.h>
#include <vdefines.h>

#ifdef VELOTRON
	#include <ini.h>
	#include <crf.h>
#endif

class Bike  {

	private:
		#define MAX_DEFAULT_GEARS 24
		#define MAX_FRONT_GEARS 3
		#define MAX_REAR_GEARS 10

		HINSTANCE hinstance;
		HWND phwnd;

		#ifdef VELOTRON
			typedef struct  {
				double gearRatio;
				double tireDiameter;
				int rearTeeth;
				int frontTeeth;
				double PERIOD_TO_RPM;
				double COUNT6;
				double weight;
				double length;
				int nvirtualFrontTeeth;
				int nvirtualRearTeeth;
				int virtualFrontTeethArray[MAX_FRONT_GEARS];
				int virtualRearTeethArray[MAX_REAR_GEARS];
				int virtualFrontIndex;
				int virtualRearIndex;
				int gearIndex;
				int gearIndex2;
				int segments, holesPerSegment, totalHoles;
				double rearTireCircumferenceInMiles;
				double rearTireCircumferenceInFeet;
				double timePerPicCount;
				int ngears;								// number of virtual gears
				int ngears2;
				double K0;
				double K1;
				double gear;
				bool use_default_gears;
				int virtualFrontGear;
				int virtualRearGear;
				unsigned char reserved[300];
			} STATE;											// 512 bytes

			int id;
			char str[256];
			static BOOL CALLBACK GearEntryDialogProc(HWND hwndDlg, UINT iMessage, WPARAM wParam, LPARAM lParam);
			bool upshiftkey;
			bool downshiftkey;
			DWORD edgetime;
			DWORD now;
			bool shifted;
			Ini *ini;
			void setupVirtualGears(void);
			void setupDefaultGears(void);
		#endif
		FILE *logstream;

	public:
		Bike(int _id=0);
		virtual ~Bike();



		#ifdef VELOTRON
			bool realtime;
			STATE state;
			double *gears;							// virtual gears array
			double *gears2;						// default gears array
			int rear_default_gear;
			int front_default_gear;

			void computeGearRatio(void);

			void write(FILE *stream);		// saves the bike state
			void read(FILE *stream);		// reads the bike state

			double getWeight(void);
			int getFrontTeeth(void);
			int getRearTeeth(void);
			void setFrontTeeth(int _frontTeeth);

			void gearUp(void);
			void gearDown(void);
			void updateGearEntries(void);
			void GearEntry(HINSTANCE _hinstance, HWND _phwnd);
			bool changed;

			int getgearIndex(void)  {return state.gearIndex;}
			void setgearIndex(int _i)  { state.gearIndex = _i; }

			int getgearIndex2(void)  {return state.gearIndex2;}
			void setgearIndex2(int _i)  { state.gearIndex2 = _i; }

			int getDefaultGearIndex(void)  { return state.gearIndex2; }
			float getDefaultGear(void)  { return (float)gears2[state.gearIndex2]; }

			int getVirtualFrontGear(void)  { return state.virtualFrontGear; }
			int getVirtualRearGear(void)  { return state.virtualRearGear; }
			float getVirtualGear(void)  { return (float)gears[state.gearIndex];  }

			void frontUp(void);
			void rearUp(void);
			void frontDown(void);
			void rearDown(void);

			void upkeydown(void);
			int upkeyup(void);
			void downkeydown(void);
			int downkeyup(void);
			int run(void);
			int setGear(int &_index);

		#endif
};

#endif		// #ifndef _BIKE_H_

