// Globals.cpp: implementation of the Globals class.
//
//////////////////////////////////////////////////////////////////////
//#define STRICT
//#define D3D_OVERLOADS

#include "stdafx.h"
#include <config.h>
#include "Globals.h"

//		#ifndef VELOTRON
//			int trainers[COMM_PORT_MAX];
//			int n_trainers;
//			HANDLE hEvent1;
//			void check_for_computrainers(void (__cdecl *)(void *));
//		#endif

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

//class SortedTri *gpSortedTri;

//class BikeApp *gpBikeApp;

class CCourse *gpCourse;

char gBaseDirectory[MAX_PATH];
char gCourseDirectory[MAX_PATH];
char gPerformanceDirectory[MAX_PATH];
char gIniFileName[MAX_PATH];

char gScratchBuf[MAX_PATH];
char gTitle[MAX_PATH];
char gVersionNum[64] = "3.65";
char gstring[2048];
//D3DXMATRIX gIdentity;


//DWORD gVBufFlags = D3DVBCAPS_SYSTEMMEMORY; // DWORD				gVBufFlags;

int gWidth;
int gHeight;
float gScaleX;
float gScaleY;

COLORREF skin_colors[N_SKIN_COLORS] = {
//D3DCOLOR_RGBA skin_colors[N_SKIN_COLORS] = {
	RGB(245, 217, 190),				// light skin
	RGB(244, 188, 165),				// med light skin
	RGB(200, 156, 127),				// med skin
	RGB(159, 114, 81),				// dark
	RGB(100, 64, 56),				// my negro
	RGB(230, 196, 127)				// chinese?
};

// 1.0f, .9f, .8f, .7f, .6f, .85f

float skin_intensities[N_SKIN_COLORS] = {
	1.0f,
	0.9f,
	0.8f,
	0.7f,
	0.6f,
	0.85f
};

/*
Light Skin			245, 217, 190
Med Light Skin		244, 188, 165
Med Skin				200, 156, 127
Dark					159, 114, 81
*/

HINSTANCE ghInstance;

//const char gCoursePath[] = "..\\Courses\\*.3dc";
//const char gPerformancePath[] = "..\\Rider Performance\\*.3dp";
const char gCoursePath[] = ".\\Courses\\*.3dc";
const char gPerformancePath[] = ".\\Rider Performance\\*.3dp";
const char report_path[] = ".\\reports\\*.html";

//LPDIRECTDRAW7 gpDD;
//LPDIRECT3D7 gpD3D;
//LPDIRECT3DDEVICE7 gpDevice;


bool gPerformanceOK = false;


#ifdef VELOTRON
bool gVelotron = true;
#else
bool gVelotron = false;
#endif

bool gMetric = true;
bool do_drag=false;

float gWeightRatio = 0.75f;
float gHeightRatio = 0.5f;


FTime gFTime = {0L, 0L, 0L, 0L, 0L, 0.0, 0L, 0.0, 85, 85*0.001f, false, 0L, 0L, 0L, 0L, 0L, 0L, 1.0f, 0L};

AvgFilter gIdleTime;

