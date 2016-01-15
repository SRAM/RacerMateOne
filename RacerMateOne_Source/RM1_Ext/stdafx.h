// stdafx.h : include file for standard system include files,
// or project specific include files that are used frequently, but
// are changed infrequently
//

#pragma once

// TODO: reference additional headers your program requires here
#define WIN32_LEAN_AND_MEAN             // Exclude rarely-used stuff from Windows headers
// Windows Header Files:
#include <windows.h>

#include <stdio.h>

// C RunTime Header Files
#include <stdlib.h>
#include <malloc.h>
#include <memory.h>
#include <tchar.h>

#include "tools.h"
#include "coursefile.h"
#include "D3DBase.h"

#include <defines.h>
#include <config.h>
#include <vdefines.h>
#include <assert.h>

#define IGNOREFORNOW



#define IFC(x) { hr = (x); if (FAILED(hr)) goto Cleanup; }
#define IFCOOM(x) { if ((x) == NULL) { hr = E_OUTOFMEMORY; IFC(hr); } }
//#define SAFE_RELEASE(x) { if (x) { x->Release(); x = NULL; } }
#define RGBAToSColor(r, g, b, a) SColor(a, r, g, b)
#define RGBAToSColorf(r, g, b, a) SColorf(a, r, g, b)

void DebugLog(CHAR *szMsg);
void Msg(CHAR *szFormat, ...);
char * Strip(char *buf);
bool LinesIntersect(float x1,float y1,float x2,float y2,float x3,float y3,float x4,float y4);
short mapvalues(float x1,float x2,float y1,float y2,float *m,float *b) ; // ect-todo partial
int ParseLine(char * wstring, int maxarg, char **argarr);
float NormalizeRotation(float deg);
float MoveTo(float org, float dest, float amount);
float AngleDiff(float a1, float a2);
void addCommas( const core::stringw& s, core::stringw& ns );
//-----------------------------------------------------------------------------
// Debug printing support
//-----------------------------------------------------------------------------

HRESULT _DbgOut( char*, DWORD, HRESULT hr=0L, char*str=NULL );

#if defined(DEBUG) | defined(_DEBUG)
    #define DEBUG_MSG(str)    _DbgOut( __FILE__, (DWORD)__LINE__, 0, str )
    #define DEBUG_ERR(hr,str) _DbgOut( __FILE__, (DWORD)__LINE__, hr, str )
    #define fatalError(a,b,c)    _DbgOut( a, b, 0, c )
#else
    #define DEBUG_MSG(str)    (0L)
    #define DEBUG_ERR(hr,str) (hr)
    #define fatalError(a,b,c)   (0L)
#endif

#define ROT_MOVESPEED	0.02f
#define MAX_LEAN_ANGLE	degToRad(18.0f) //degToRad(22.0f)

extern char gBasePath[];

extern char gstring[];

extern const char *modelNames[];
extern const char *modelTex[];
extern scene::ISceneNode* customNode;
extern scene::ISceneNode* mainNode;
extern ISceneNode* courseNode;
extern ISceneNode* treesNode;
extern ISceneNode* skydome;

// Shadow stuff
extern video::ITexture* riderShadow;
extern scene::ICameraSceneNode* shadowCam;
extern bool bRealShadows;
extern int tw, th;
extern int inc_w, inc_h; 
extern video::ITexture* riderShadows[];
extern float inc_rot;
extern f32 sroty;
extern vector3df sscale;
extern int sframes;

extern f32 mult;
extern f32	m_MaxLeanAngle;
extern f32	m_XMax;
extern f32 bikeLength;

extern float riderStart[];
extern float riderLanes[];
extern int startLane[]; 

extern int numRiders;
extern int targetLanes[];
extern int orderDist[];
extern int orderLane[];

extern float collFactor;
extern float speedFactor;

extern int maxspeed;
extern SColor matColor;
extern int matKey;
extern RIDER_COLORS matType;
extern bool bChanged; 

extern bool camfollow;
extern bool demomode;
extern bool showinfo;
extern CState state;
extern u32 demoStart;
extern int numViews;
extern int numriders;
extern int numtest;
extern int camtype;
extern f32 fardist;
extern f32 distCam;
extern f32 distCamM;
extern bool demomode;
extern f32 fardistCam;
extern f32 lastn;
extern f32 demoAngleFOV;
extern f32 angleFOV;
extern f32 lastAngleFOV;
extern f32 camh;
extern bool fogOn;
extern bool large;
extern bool bInit;
extern bool bReady;
extern bool bTest;
extern bool bSceneNodeUpdated;
extern IImage *skyImg;
extern bool bDemoRider;
extern int gamemode;
extern u32 holdChange;
extern float eyeAdjust;

extern scene::ISceneCollisionManager* collMan;
extern scene::IMetaTriangleSelector * meta;

#include "globals.h"
#include "CCourse.h"
#include "Rider.h"

#include "Keycode.h"	
#include "version.h"	
