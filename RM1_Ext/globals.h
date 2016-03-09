// Globals.h: interface for the Globals class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_GLOBALS_H__481C4804_24F8_11D5_8F41_DDB7D6D4F362__INCLUDED_)
#define AFX_GLOBALS_H__481C4804_24F8_11D5_8F41_DDB7D6D4F362__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

//#define  D3D_OVERLOADS
//#include <d3d.h>

#include <windows.h>
/*
#include <config.h>
#include <d3dx.h>
//#include "D3DBase.h"
#include "d3dtypes.h"
#include "DX8_Supp.h"
#include "D3DMath.h"
#include "D3DUtil.h"
#include "d3dfile.h"
*/
//#include "D3DBase.h"
//#include "d3dfont.h"

#define N_SKIN_COLORS 6
//extern COLORREF skin_colors[N_SKIN_COLORS];
//extern D3DCOLOR_RGBA skin_colors[N_SKIN_COLORS];
extern float skin_intensities[N_SKIN_COLORS];

//#define WM_USER_COMPUTRAINER_CHANGE		(WM_USER+1)
#define WM_USER_RESIZE_MAIN				(WM_USER+1)
#define IDD_RENDER						(WM_USER+5)

//extern LPDIRECTDRAW7 gpDD;
//extern LPDIRECT3D7 gpD3D;
//extern LPDIRECT3DDEVICE7 gpDevice;

extern DWORD gVBufFlags;


//extern class SortedTri *gpSortedTri;
//extern class BikeApp *gpBikeApp;

extern class CCourse *gpCourse;

extern const char gCoursePath[];
extern const char gPerformancePath[];
extern const char report_path[];

extern char gBaseDirectory[MAX_PATH];
extern char gCourseDirectory[MAX_PATH];
extern char gPerformanceDirectory[MAX_PATH];
extern char gIniFileName[MAX_PATH];


extern char gScratchBuf[MAX_PATH];
extern char gTitle[MAX_PATH];
extern char gVersionNum[];

//extern D3DXMATRIX gIdentity;

extern int gWidth;
extern int gHeight;
extern float gScaleX;
extern float gScaleY;

extern HINSTANCE ghInstance;

extern bool gPerformanceOK;		// Used when the performace has finished a race.

inline float ScaleX(float x)  {
	return x * gScaleX;
}
inline float ScaleY(float y)  {
	return y * gScaleY;
}


inline float UnscaleX(float x)  {
	return x / gScaleX;
}
inline float UnscaleY(float y)  {
	return y / gScaleY;
}

extern bool gVelotron;
extern bool gMetric;

extern float gWeightRatio;
extern float gHeightRatio;

typedef struct FTime
{
	DWORD			sysTime;	// Actual System clock time.
	DWORD			renderTime;	// Actual render count.
	DWORD			updateTime;	// Actual update count.

	DWORD			advTime;	// Non - paused advance time.

	DWORD			simTime;	// Simulation Time.
	double			simSec;
	DWORD			frameTime;	// Frame time - for this frame.
	float			frameSec;

	DWORD			MaxAdvTime; // = 85;     // 12 fps
	float			MaxAdvFrameSec; // = MaxAdvTime*0.001f; // one sec 
	BOOL			bFastAdvance;

	DWORD			elapsedtime;
    unsigned long	fTime;
    unsigned long	newTime;
    unsigned long	oldTime;
    unsigned long	runTime;
    unsigned long	idleTime;
	float			runPercent;

	UINT idx;
	float renderFPS;
	DWORD elapse[10];
} FTime;

extern FTime gFTime;
extern AvgFilter gIdleTime;

#endif // !defined(AFX_GLOBALS_H__481C4804_24F8_11D5_8F41_DDB7D6D4F362__INCLUDED_)
