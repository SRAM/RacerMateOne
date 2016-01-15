#include "stdafx.h"
//#include "ccourse.h"
//#include "globals.h"
#include "ccourse_utils.h"

//#include <config.h>

//#include <assert.h>
#define TESTING 1

//#define STRICT

//#include <_course.h>

#include <vdefines.h>
#include <globals.h>
#include <utils.h>
#include <genutils.h>

//#include <fatalerror.h>
//#include "computrainerbase.h"
//#include "mesh.h"
//#include "Sprite.h"
//#include "Mesh.h"
//#include "GLModel.h"
//#include <dprintf.h>
//#include <string.h>
//#include "computrainerbase.h"
//#include "globals.h"
//#include "bikeApp.h"
//#include "tools.h"
//#include "Landscape.h"
#include "Terrain.h"


extern char gstring[2048];

#define MAX_CRS_SEGMENT_LEN		100.0f
#define MAX_CRS_SEGMENT_ROT		90.0f

#define SECTION_MIN_LENGTH		5.0f
#define SECTION_MAX_LENGTH		1000.0f
#define SECTION_MAX_ROTATION	270.0f
#define SECTION_GRADE_MAX_D_100		(MAX_GRADE/100.0f)
#define SECTION_MAX_ROTATION_RATIO		(22.50f / 25.0f )
#define SIGN_TEX_NAME		"sign.bmp"

#pragma warning(disable:4244)	// float to double
#pragma warning(disable:4786)

#include <vector>
using namespace std;

static float ROAD_REPEAT = 24.0f;		// no effect

static bool sHitRight, cHitRight;
static bool _bLoading;		// WW: NC18 - Added to fix prefs buf. tlm20031219

#define HILL 1
#define MAX_TREEDATA		40

typedef struct{
	float color;
	float y;
} YColor;

typedef struct{
		unsigned char hr;
		unsigned char rpm;

		//	unsigned short  hr;

		unsigned short watts;
		float speed;
		DWORD disttime;
} PERFPOINTS;

typedef struct{
		unsigned char hr;			// real heartrate
		unsigned char rpm;		// real rpm
		unsigned short watts;		// real watts
		unsigned short speed;		// convert to float and divide this by 100 to get kph
		DWORD time;		// real milliseconds since race began
		short grade;		// convert to float and divide this by 100 to get grade
		float kilometers;	// real kilometers traveled at this point
} PERFPOINTSv3;// version 3

typedef struct{
		unsigned char hr;			// real heartrate
		unsigned char rpm;		// real rpm
		unsigned short watts;		// real watts
		unsigned short speed;		// convert to float and divide this by 100 to get kph
		DWORD time;		// real milliseconds since race began
		short grade;		// convert to float and divide this by 100 to get grade
		float kilometers;	// real kilometers traveled at this point
		unsigned char RightSS;	// right spinscan
		unsigned char LeftSS;		// left spinscan
} PERFPOINTSv4;// version 4

typedef struct{
		unsigned char hr;			// real heartrate
		unsigned char rpm;		// real rpm
		unsigned short watts;		// real watts
		unsigned short speed;		// convert to float and divide this by 100 to get kph
		DWORD time;		// real milliseconds since race began
		short grade;		// convert to float and divide this by 100 to get grade
		float kilometers;	// real kilometers traveled at this point
		unsigned char RightSS;	// right spinscan
		unsigned char LeftSS;		// left spinscan
		unsigned char LeftSplit;	// left split
} PERFPOINTSv5;// version 5 --- the latest version

struct TreeData {
		float width;
		float height;
		float w_adj_min; 
		float w_adj_max;
		float h_adj_min;
		float h_adj_max;
		float scale_adj;
		float offsetx;
		float offsety;
		int tex;
		float shadow_width_scale;
		float shadow_height_scale;
};


struct TreeGroupData{
	int start;
	int count;
};

//---------------------------------------------------------------------------------------
// the following are loaded from scene.txt ++++++++++++++++++++++++++++++++++++++++
//---------------------------------------------------------------------------------------

struct TreeData treeArr[MAX_TREEDATA] = {				// MAX_TREEDATA = 40

//   w 	  h	 wamin  wamax  hamin hamax  sc_a  offx  offy   tex 				  swscale shscale

	{ 5.0f, 5.0f, -0.2f, 0.2f, -0.2f, 0.2f, 0.1f, -1.0f, 0.4f, CCourse::TEX_TREE1, 0.9f, 0.6f },
	{ 5.0f, 5.0f, -0.2f, 0.2f, -0.2f, 0.2f, 0.1f, -1.0f, 0.4f, CCourse::TEX_TREE2, 0.9f, 0.6f },
	{ 3.5f, 9.0f, -0.2f, 0.2f, -0.2f, 0.2f, 0.1f, -1.0f, 0.4f, CCourse::TEX_TREE3, 0.9f, 0.6f },
	{ 3.5f, 9.0f, -0.2f, 0.2f, -0.2f, 0.2f, 0.1f, -1.0f, 0.4f, CCourse::TEX_TREE4, 0.9f, 0.6f }

};


int treeGroupCount = 2;
TreeGroupData treeGroup[10] = { 
	{ 0, 2 }, 
	{ 2, 2 } 
};

// MAX_STRIPS = 10

STRIP_INFO stripArrBase[MAX_STRIPS] = {
	
//		offset  div  low   high    zrepeat xrepeat cadd  treeok

	{ - 28.00f, 8, -4.0f, 4.00f, 64.0f, 1.00f, 0.00f, false },		// 64 TEX_EDGE_5_L
	{ - 16.00f, 4, -2.0f, 2.00f, 64.0f, 1.00f, 0.00f, true  },		// 64 TEX_EDGE_4_L
	{   -8.00f,	4,  0.0f, 0.01f, 64.0f, 1.00f, 0.00f, true  },		// 64 TEX_EDGE_3_L
	{   -3.00f,	1,  0.0f, 0.01f, 32.0f, 1.00f, 0.00f, false },		// 32 TEX_EDGE_2_L
	{   -1.75f, 1,	 0.0f, 0.00f, 16.0f, 1.00f, 0.25f, false },		// 16 TEX_EDGE_1_L
	// center of road
	{ 	  1.75f, 1,	 0.0f, 0.00f, 16.0f, 1.00f, 0.25f, false },		// 16 TEX_EDGE_1_R
	{ 	  3.00f, 1,	 0.0f, 0.01f, 32.0f, 1.00f, 0.00f, false },		// 32 TEX_EDGE_2_R
	{ 	  8.00f, 4,	 0.0f, 0.01f, 64.0f, 1.00f, 0.00f, true  },		// 64 TEX_EDGE_3_R
	{	 16.00f,	4, -2.0f, 2.00f, 64.0f, 1.00f, 0.00f, true  },		// 64 TEX_EDGE_4_R
	{	 28.00f,	8, -4.0f, 4.00f, 64.0f, 1.00f, 0.00f, true  },  		// 64 TEX_EDGE_5_R

};


//---------------------------------------------------------------------------------------
// end of  scene.txt data ++++++++++++++++++++++++++++++++++++++++
//---------------------------------------------------------------------------------------

STRIP_INFO *stripArr = stripArrBase;
int lSTRIPS = 10;
int lSTRIPS_H = 5;
int lSTRIP_START = 0;


float gMaxEdgeRadius = 29.0f;
float gMaxStripSize = 4.0f;
float gMaxEdgeStripSize = 32.0f;
float gTerrainGridSize = 200.0f;
float gTerrainBorder = 500.0f;
static float editRoadWidth = 4.0f;
static float editSpecialWidth = 15.0f;
static float minRadius = 4.0;
static float minEdgeRadius = 4.0;
static float maxEdgeRadius = 32.0f;

// TERRAIN HILL LIMITS.

float BEZ_INITIAL_YDELTA = 0.02f;
float BEZ_BOTTOM_DELTA_SCALE = 1.8f;
float BEZ_TOP_DELTA_SCALE = 0.55f;
float BEZ_ADDY_MIN = -0.5f;
float BEZ_ADDY_MAX = 2.0f;

static float gMaxBoarder = 150.0f;

float gMaxYExtent = 8.0f;

#define RSTRIP(c)	((lSTRIPS-1)-c)

char *orderArr[] = { /* 0000 */ "",
	/* 0001 */ "",
	/* 0010 */ "",
	/* 0011 */ "0001 0010",
	/* 0100 */ "",
	/* 0101 */ "0001 0100",
	/* 0110 */ "0010 0100",
	/* 0111 */ "0011 0101 0110 0001 0010",
	/* 1000 */ "",
	/* 1001 */ "0001 1000",
	/* 1010 */ "0010 1000",
	/* 1011 */ "0011 1001 1010 0001 0010",
	/* 1100 */ "0100 1000",
	/* 1101 */ "0101 1001 1100 1000 0100",
	/* 1110 */ "0110 1010 1100 1000 0100",
	/* 1111 */ "0111 1011 1101 1110 1010 1001 0110 0101",  };


// Tree Data

static char treepattern[] = {1, 1, 2, 1, 2, 2, 1, 1, 4, 3, 4, 4, 4, 0, 4, 3, 4, 2, 3, 1, 1, 2, 0, 1, 3, 4, 4 };

//static float treesPerMeter_Max = (4.0f/25.0f);

static float treesMinDivisionLength = 0.5f;

//#define NEWTREECOUNT

#ifdef NEWTREECOUNT
	static float treesMaxPerSection = 500.0f;
#else
	static float treesMaxPerSection = 25.0f;			// was 50, 100 may cause a blow up
#endif

static float tree1Width = 2.5f;
static float tree1Height = 4.5f;

static float treeMin = 0.0f;
static float treeMax = 0.0f;

#define MAX_TREEGROUP		10

struct ValNames{
		const char *name;
		float *fptr;
		float def;
};

static struct ValNames valNames[] = { 
	{ "VAL_TREES_PER_100_MIN", &treeMin,0 },
	{ "VAL_TREES_PER_100_MAX", &treeMax,25 }, 
	{ NULL, NULL } 
};


int CCourse::Section::n_sections = 0;					// counts section instances
CCourse::Section *CCourse::Section::ms_RenderList;


// this is a copy of dirs in ..../rd/lib/globals.cpp, so I can link 3dlib to the rd stuff:

char badirs[8][256] = {
	"",
	"",
	"Settings",
	"ReportTemplates",
	"Reports",
	"Courses",
	"Performances",
	"Debug"
};


/************************************************************************************
	constructor
************************************************************************************/


CCourse::CCourse(bool _metric, bool no3d, const char *scenery )  {

	metric = _metric;
	//logg = NULL;

	memset(curpath, 0, sizeof(curpath));
	memset(fullpath, 0, sizeof(fullpath));

	course_meters = 0.0f;

	assert(sizeof(PerformanceInfo)==236);

	has_crs = false;
	crs_nlegs = 0;
	crs = NULL;
	accum_meters = NULL;
	saved_file_size = 0;

	memset(m_Error, 0, sizeof(m_Error));
	m_TotalLaps = 0;
	m_LowY = 0;
	m_HighY = 0;
	m_CourseLength = 0;
	m_Dirty = 0;
	m_BikeDistance = 0;
	m_FarDistance = 0;
	m_SectionCount = 0;
	m_EdgeTotal = 0;
	m_pTerrainGrid = 0;
	m_BarTotal = 0;
	m_BarCount = 0;
	m_BarNext = 0;
	m_BarMax = 0;
	bp = 0;

	minlen = FLT_MAX;
	maxlen = -FLT_MAX;
	min_percent_grade = FLT_MAX;
	max_percent_grade = -FLT_MAX;
	minangle = FLT_MAX;
	maxangle = -FLT_MAX;
	minwind = FLT_MAX;
	maxwind = -FLT_MAX;
	minElevation = FLT_MAX;
	maxElevation = -FLT_MAX;
	maxElevGain = 0.0f;

	totalMeters = 0.0f;
	nlegs = 0;
	closed = false;

	m_pName = NULL;
	m_pFileName = NULL;

	//initStr(m_pName);
	//initStr(m_pFileName);

	m_pDescription = NULL;
	m_pCreator = NULL;



	memset(&m_SI, 0, sizeof(m_SI));

	// original blue sky color that was hard coded:

	sky_r = 94;
	sky_g = 148;
	sky_b = 250;
	sky_tiled = -3.0f;

	ms_usize = 20.0f;							// 20.0f, smaller = more repeats, passed to Terrain::

	int size = sizeof(*this);
	unsigned char *p = (unsigned char *) (this);
	p += size - sizeof(ms_vsize);
	assert(p = (unsigned char *)&ms_vsize);

	ms_vsize = 20.0f;							// 20.0f, smaller = more repeats, passed to Terrain::

	m_Offset = vector3df(0.0f, 0.0f, 0.0f);
	m_BaseRotation = 0.0f;

	m_bLoopClosed = m_bLooped = m_bReverse = m_bMirror = false;
	m_StartAt = m_EndAt = m_SecondStart = 0.0f;

	m_Version = 0;
	m_Finished = false;
	m_Length = 0.0f;

	laps = 1;

	m_EyeDistance = 0.0f;
	m_EyeNum = -1;

	int i;

	for (i = 0; i < TEX_MAX; i++)
		m_Tex[i] = NULL;
#ifndef IGNOREFORNOW
	m_pMeshDef = NULL;
	m_pMesh = NULL;
#endif
	for (i = 0; i < BIKE_HINTS; i++)  {
		m_pLocCache[i] = 0L;
	}

	m_pSceneNode=0;

	m_pNodeVisible=0;
	m_pNodeHidden=0;

	m_GridYArr = NULL;
	m_bBuildingEdgeMesh = false;
	m_bLoopClosed = m_bLooped = m_bReverse = m_bMirror = false;
	m_StartAt = m_EndAt = m_SecondStart = 0.0f;

	m_pStartSection = NULL;
	m_pFinishSection = NULL;
	m_pFinish = NULL;


	for (i = 0; i < MAX_RIDERS; i++)  {
		pinf[i].perfcount = 0;
		m_pPerfArr[i] = NULL;

		#ifdef VELOTRON
			#ifdef LOG_GEARING
				gearInfoArray[i] = NULL;
			#endif
		#endif

		#ifdef LOG_CALORIES
			calories[i] = NULL;
		#endif

		#ifdef PERFINFO2
			memset(&pinf2[i], 0, sizeof(pinf2));
			pinf2[i].version = 1;
		#endif

	}

	m_pSISection = NULL;
	m_pEdit = NULL;

	//m_Author = "";
	//m_Description = "";

	m_FrameNum = 0;


#ifdef BIKE_APP
	/*
	for (i = 0; i < TEX_MAX; i++)  {
		m_Tex[i] = NULL;
	}
	*/
	if (!no3d)
		SetLandscape2(scenery);		// stripArrBase gets loaded here

//	Inactivate();
#endif

	calcGrid();

#ifdef BIKE_APP
	m_FogOn = false;
	m_FogColor = RGBAToSColor(128, 128, 256, 0);
	m_FogDensity = 0.01f;

	//	SetPri(PRI_OVERLAY);

//#ifndef IGNOREFORNOW
	//m_pSign = new Model("BilBoard.x");
	//m_pSign->Inactivate();
	//m_pBanner = new Model("Banner.x");
	//m_pBanner->Inactivate();
//#endif
	// Terrain setup

	m_pEdge = NULL;
	m_pTerrainGrid = NULL;

	message_shown = false;

	bp = 1;

#endif

}

/************************************************************************************
	destructor
************************************************************************************/


CCourse::~CCourse()  {
	cleanup();
}


/************************************************************************************
	computes the %grade from the CRSLEG (crs[]) array

************************************************************************************/

float CCourse::computeGrade(float _meters)  {

	if (!has_crs)  {
		return 0.0f;
	}

	int i;
	//float accum_meters = 0.0f;

	for(i=0; i<crs_nlegs; i++)  {
		//accum_meters += crs[i].meters;
		if (_meters < accum_meters[i])  {
			break;
		}
	}

	if (i==crs_nlegs)  {
		return crs[i-1].grade;
	}

	return crs[i].grade;
}


/************************************************************************************

************************************************************************************/

void CCourse::calcGrid(float size)  {
	if (m_GridYArr)  {
		delete[] m_GridYArr;
		m_GridYArr = NULL;
	}

	Section *ps = m_Track.GetFirst();
	m_GridMin = vector3df(0.0f, 0.0f, 0.0f);
	m_GridMax = vector3df(0.0f, 0.0f, 0.0f);
	m_GridSquareSize = vector3df(size, 1.0f, size);
	if (!ps)  {
		m_GridWidth = m_GridHeight = 1;
	}
	else  {
		float t;
		for (; ps; ps = ps->Next())  {
			extRect2(m_GridMin, m_GridMax, ps->m_ExtMin);
			extRect2(m_GridMin, m_GridMax, ps->m_ExtMax);

			t = ps->m_LowY;

			if (t < m_GridMin.y)
				m_GridMin.y = t;

			t = ps->m_HighY;

			if (t > m_GridMax.y)
				m_GridMax.y = t;

			if (m_GridMin.z > 0.01f)
				m_GridMin.z = m_GridMin.z;
		}
	}

	m_GridMin.x = (float) (((int) m_GridMin.x / (int) m_GridSquareSize.x) - 1) * m_GridSquareSize.x;
	m_GridMin.z = (float) (((int) m_GridMin.z / (int) m_GridSquareSize.z) - 1) * m_GridSquareSize.z;
	m_GridMax.x = (float) (((int) m_GridMax.x / (int) m_GridSquareSize.x) + 1) * m_GridSquareSize.x;
	m_GridMax.z = (float) (((int) m_GridMax.z / (int) m_GridSquareSize.z) + 1) * m_GridSquareSize.z;

	// Round to x number of meters.

	vector3df v(m_GridMax - m_GridMin);
	m_GridWidth = (int) ((v.x + m_GridSquareSize.x) / m_GridSquareSize.x);
	m_GridHeight = (int) ((v.z + m_GridSquareSize.z) / m_GridSquareSize.z);
}


/************************************************************************************

************************************************************************************/

void CCourse::ClearTrack()  {

#ifdef BIKE_APP
//	ClearTerrain();
#endif

	Section *ps;
	while ((ps = m_Track.GetFirst()) != NULL)
		delete ps;
	ClearProfile();

	/*
	if(m_pMesh)
		delete m_pMesh;
	m_pMesh = NULL;
	*/

	m_pSISection = NULL;
	m_pStartSection = NULL;
	m_pFinishSection = NULL;
	laps = 1;

	m_Offset = vector3df(0.0f, 0.0f, 0.0f);
	m_BaseRotation = 0.0f;

	if (m_pCreator)  {
		delete m_pCreator;
		m_pCreator = NULL;
	}

	calcGrid();

}


//=============================================================================
// void CCourse::ClearProfile(int num)
//
//
//=============================================================================
/************************************************************************************

************************************************************************************/

void CCourse::ClearProfile(int num)  {

	if (_bLoading)  {		// WW: NC18 - Added to fix performance bug
		return;			// WW: NC18 - Added to fix performance bug
	}

	if (num < 0)  {
		for (int i = 0; i < MAX_RIDERS; i++)  {
			ClearProfile(i);
		}
	}
	else  {
		memset(pinf + num, 0, sizeof(pinf[num]));
		pinf[num].version = 2;		// 1;

		if (m_pPerfArr[num])  {
			delete[] m_pPerfArr[num];
			m_pPerfArr[num] = NULL;
		}

#ifdef VELOTRON
		#ifdef LOG_GEARING
		if (gearInfoArray[num])  {
			delete[] gearInfoArray[num];
			gearInfoArray[num] = NULL;
		}
		#endif
#endif

#ifdef LOG_CALORIES
	if (calories[num])  {
		delete[] calories[num];
		calories[num] = NULL;
	}
#endif

	}
}

/************************************************************************************

************************************************************************************/

void CCourse::SetLaps(int _laps)  {
	if (_laps < 1)
		_laps = 1;
	laps = _laps;
	return;
}

/************************************************************************************

************************************************************************************/

bool CCourse::IsRaceable()  {
	return (m_bLoopClosed || (m_pFinish != NULL));
}

/************************************************************************************

************************************************************************************/

bool CCourse::IsSaveable()  {
	Section *ps = m_Track.GetFirst();
	for (;
		  ps;
		  ps = ps->Next())  {
		if (ps->m_Type == Section::START)  {
			return (ps->Next() != NULL);
		}
	}
	return false;
}

/************************************************************************************

************************************************************************************/

bool CCourse::IsPrefSaveable(int num)  {
	if (!IsRaceable())
		return false;

	if (num < 0)  {
		for (int i=0; i<MAX_RIDERS; i++)  {
			if (pinf[i].perfcount > 0)
				return true;
		}
	}
	else  {
		return (pinf[num].perfcount > 0);
	}

	return false;
}



/************************************************************************************

************************************************************************************/

void CCourse::SetCourseName(const char *name)  {

	DELARR(m_pName);
	m_pName = new char[strlen(name) + 1];
	strcpy(m_pName, name);

	return;
}

/************************************************************************************

************************************************************************************/

void CCourse::SetCourseDescription(const char *desc)  {
	if (desc && (*desc == '\0'))
		desc = NULL;
	if (m_pDescription)
		delete m_pDescription;
	if (desc)  {
		m_pDescription = new char[strlen(desc) + 1];
		strcpy(m_pDescription, desc);
	}
	else
		m_pDescription = NULL;
}

/************************************************************************************

************************************************************************************/

void CCourse::SetCourseCreator(const char *desc)  {
	if (desc && (*desc == '\0'))
		desc = NULL;
	if (m_pCreator)
		delete m_pCreator;
	if (desc)  {
		m_pCreator = new char[strlen(desc) + 1];
		strcpy(m_pCreator, desc);
	}
	else
		m_pCreator = NULL;
}


/************************************************************************************

************************************************************************************/

const char * CCourse::IsLater(const char *pn0, const char *pn1)  {
#ifndef IGNOREFORNOW
	static char buf[MAX_PATH];
	char *name;

	WIN32_FIND_DATA fd[2];

	name = makeName(pn0, buf);
	HANDLE h0 = FindFirstFile(name, &fd[0]);
	if (h0 != INVALID_HANDLE_VALUE)
		FindClose(h0);

	name = makeName(pn1, buf);
	HANDLE h1 = FindFirstFile(name, &fd[1]);
	if (h1 != INVALID_HANDLE_VALUE)
		FindClose(h1);

	if ((h0 != INVALID_HANDLE_VALUE) && (h1 != INVALID_HANDLE_VALUE))  {
		// Check the file dates

		return (CompareFileTime(&fd[0].ftLastWriteTime, &fd[0].ftLastWriteTime) >= 0 ? pn0 : pn1);
	}
	else if (h0 != INVALID_HANDLE_VALUE)
		return pn0;
	else if (h1 != INVALID_HANDLE_VALUE)
		return pn1;
#endif
	return NULL;	// Neither.
}

/************************************************************************************

************************************************************************************/

void CCourse::finishTrack()  {
//#ifndef IGNOREFORNOW
	if (m_bLoopClosed)  {
		m_pStartSection = m_pFinishSection = NULL;
		return;
	}

	Section *ps = m_Track.GetFirst();
	m_pStartSection = ps;

	//for (;ps && (ps->GetStartDistance() < 0.0f);)  {
	while (ps && (ps->GetStartDistance() < 0.0f))  {
		ps = ps->Next();
	}

	if (ps)  {
		m_pStartSection = ps;
	}

	//for (;ps && !ps->IsFinishSection();)  {
	while (ps && !ps->IsFinishSection())  {
		ps = ps->Next();
	}

	if (ps)  {
		ps = ps->Prev();
	}


	/*
	if (!ps)  {
		ps = m_Track.GetLast();
	}
	*/

	m_pFinishSection = ps;

	// m_pFinish

	GetSectionInfo(true);

	//BuildTerrain();
//#endif
}

typedef struct 
{
	stringw Name;
	stringw Description;
	stringw FileName;
	stringw Type;
	int iType;
	int Laps;
	double EndAt;
	double StartAt;
	bool Mirror;
	bool Reverse;
	bool Looped;
	bool Modified;
} CourseInfo;

typedef struct 
{
	double Length;
	double Grade;
	double Wind;
	double Rot;
	CCourse::Section::Type Type;
} CourseSegment;

typedef struct 
{
	float startX;
	float startY;
	float startZ;
	int divisions;
	double **yArr;
	double **GArr;
} ExtCourseSegment;

typedef enum 
{
	Distance = 0x01,
	Watts = 0x02,
	Video = 0x04,
	ThreeD = 0x08,
	GPS = 0x10,
	Performance = 0x20
} CourseType;


float toFloat(stringw str)
{
	core::stringc c = str.c_str();
	return core::fast_atof(c.c_str());
}

float toDouble(stringw str)
{
	core::stringc c = str.c_str();
	return atof(c.c_str());
}

/************************************************************************************

************************************************************************************/
bool CCourse::addEndSegments()  
{
	Section *ps=NULL;
	Section *pshere=NULL;

	if(!m_bLoopClosed)
	{
		float cRndRot = 0.0f;
		cRndRot = 0.0f;
		pshere = m_Track.GetFirst();
		// Add Pre segments if not looped
		// if here randomize rotation and road
		if(m_bReverse)
		{
			float extraLength = 1500;
			float legLength = extraLength / MAX_CRS_SEGMENT_LEN;

			CourseSegment tcs;
			int icnt = 1;
			if(legLength > 1.0f)
				icnt = legLength + 1;
			else
				legLength = 1.0f;

			for(int j = 0; j < icnt; j++)
			{
				tcs.Grade = 0;
				tcs.Length = extraLength / legLength;
				tcs.Rot = 0;

				float xRandom = frand(0.0f,100.0f);
				float ratio = 90.0f;
				tcs.Type = (xRandom > ratio ) ? Section::STREIGHT : Section::CURVE;

				if(tcs.Type == Section::CURVE )
				{
					float xMult = xRandom < (ratio/2) ? -1 : 1;
					if(cRndRot < -30.0f)
						xMult = 1;
					else if(cRndRot > 30.0f)
						xMult = -1;
					tcs.Rot = frand(0.0f,45.0f) * xMult; 
					cRndRot += tcs.Rot;
				}


				ps = new Section(*this, tcs.Type, tcs.Length, tcs.Grade, tcs.Rot);
				if(j==0)
					ps->InsertBefore(*pshere);
				else
					ps->InsertAfter(*pshere);
				pshere = ps;
			}

			pshere = m_Track.GetFirst();
			pshere->m_StartDistance = -extraLength;
		}
		else
		{
			ps = new Section(*this, Section::STREIGHT, 506.0f, 0.0f, 0.0f, TEX_ROAD, -510.0f);
			ps->InsertBefore(*pshere);

			pshere = m_Track.GetFirst();
			pshere->m_StartDistance = -510.0f;
		}

		RedoCourse();

		cRndRot = 0.0f;
		// Add Post segments if not looped
		// if not 3dtype randomize rotation and road
		// if modified then ignore the curves
		if(!m_bReverse)
		{
			float extraLength = 1500;
			float legLength = extraLength / MAX_CRS_SEGMENT_LEN;

			CourseSegment tcs;
			int icnt = 1;
			if(legLength > 1.0f)
				icnt = legLength + 1;
			else
				legLength = 1.0f;

			for(int j = 0; j < icnt; j++)
			{
				tcs.Grade = 0;
				tcs.Length = extraLength / icnt;
				tcs.Rot = 0;

				float xRandom = frand(0.0f,100.0f);
				float ratio = 90.0f;
				tcs.Type = (xRandom > ratio ) ? Section::STREIGHT : Section::CURVE;

				if(tcs.Type == Section::CURVE )
				{
					float xMult = xRandom < (ratio/2) ? -1 : 1;
					if(cRndRot < -30.0f)
						xMult = 1;
					else if(cRndRot > 30.0f)
						xMult = -1;
					tcs.Rot = frand(0.0f,45.0f) * xMult; 
					cRndRot += tcs.Rot;
				}

				ps = new Section(*this, Section::EXTRA, tcs.Length, tcs.Grade, tcs.Rot);

				// check collision
				if(m_pStartSection)
				{
					Section *pc = m_pStartSection->Prev();
					if (pc)	
					{
						for (;pc; pc = pc->Prev())  
						{
							if (CheckCollision(ps, pc))
							{
								delete ps;
								ps = NULL;
								break;
							}
						}
					}
				}
				if(!ps)
					break;
			}
		}
		else
		{
			ps = new Section(*this, Section::EXTRA, 500.0f, 0, 0);
			// check collision
			if(m_pStartSection)
			{
				Section *pc = m_pStartSection->Prev();
				if (pc)	
				{
					for (;pc; pc = pc->Prev())  
					{
						if (CheckCollision(ps, pc))
						{
							delete ps;
							ps = NULL;
							break;
						}
					}
				}
			}
		}
	}
	return true;
}

/************************************************************************************

************************************************************************************/
bool CCourse::LoadAny(const char *basename)  {

	int randint = getRandomNumber() % 10;
	for(int r = 0; r < randint; r++)
	{
		float x = frand(0.0f, 100.0f);
		int i = getRandomNumber();
	}


	stringc fname(basename);
	if(0 >= strlen(basename))
	{
		basename = "default.rmc";
		char buf[MAX_PATH];
		char *name = makeName(basename, buf);
		strcpy(fullpath, name);				// curpath + filename
		strcpy(curpath, name);					// curpath part only
		strip_filename(curpath);

		ClearProfile();
		ClearTrack();
		SetCourseName(basename);
		DELARR(m_pFileName);
		m_pFileName = new char[strlen(basename) + 1];
		strcpy(m_pFileName, basename);

		Section *ps=NULL;
		m_pStartSection = NULL;
		m_pFinishSection = NULL;
		laps = 10;
		m_bLooped = true;
		m_EndAt = 100.0f;
		m_StartAt = 0.0f; 
		m_bLoopClosed = m_bReverse = m_bMirror = false;
		m_SecondStart = 0.0f;

		new Section(*this, Section::START, 4.0f, 0.0f, 0.0f, TEX_START, -4.0f);
		new Section(*this, Section::FINISH, 4.0f, 0.0f, 0.0f);

		ps = m_Track.GetLast();
		ps->m_Type = Section::FINISH;
		ps->m_SpecialTexture = TEX_FINISH;
		m_pFinishSection = ps;

		finishTrack();
		ReadyEdit();
		RedoCourse();

		if (m_bLoopClosed)
		{
			_bLoading = true;		// WW: NC18 - Added to fix performance bug tlm20031219
			EditDelete();
			EditCloseLoop(true);
			_bLoading = false;		// WW: NC18 - Added to fix performance bug tlm20031219
		}
		else
		{
			addEndSegments();
		}
		computeStats();
		return true;
	}
	else if(0 < fname.find(".rmp") || 0 < fname.find(".rmc"))
	{
		char buf[MAX_PATH];
		char *name = makeName(basename, buf);
		strcpy(fullpath, name);				// curpath + filename
		strcpy(curpath, name);					// curpath part only
		strip_filename(curpath);

		ClearProfile();
		ClearTrack();
		SetCourseName(basename);
		DELARR(m_pFileName);
		m_pFileName = new char[strlen(basename) + 1];
		strcpy(m_pFileName, basename);

		loadRMP(name);  

		finishTrack();
		ReadyEdit();
		RedoCourse();

		if (m_bLoopClosed)
		{
			calcGrid();
			updateSI(true);
			ClearProfile();

			m_bLoopClosed = false;
			FixCloseLoop(true);

			m_SI.flags |= SI_CLOSELOOP;
			for(int j = 0; j < 6; j++)
			{
				if(EditCloseLoop(true,true))
					break;
				Section *ps=NULL;
				ps = m_Track.GetLast();
				delete ps;
				calcGrid();
				GetSectionInfo();
				ClearProfile();	
			}
		}
		else
		{
			addEndSegments();
		}

		computeStats();

		return true;
	}
	else
	{
		return Load(basename);
	}
}
/*
bool CCourse::saveRMP(char *fname)  
{
}
*/

    const stringw xtagTimeAcc = L"TimeAccumulator";
    const stringw xtagLapTime = L"LapTime";
    const stringw xtagLap = L"Lap";
    const stringw xtagDistance = L"Distance";
    const stringw xtagLead = L"Lead";
    const stringw xtagGrade = L"Grade";
    const stringw xtagWind = L"Wind";
    const stringw xtagSpeed = L"Speed";
    const stringw xtagSpeed_Avg = L"Speed_Avg";
    const stringw xtagSpeed_Max = L"Speed_Max";
    const stringw xtagWatts = L"Watts";
    const stringw xtagWatts_Avg = L"Watts_Avg";
    const stringw xtagWatts_Max = L"Watts_Max";
    const stringw xtagWatts_Wkg = L"Watts_Wkg";
    const stringw xtagWatts_Load = L"Watts_Load";
    const stringw xtagHeartRate = L"HeartRate";
    const stringw xtagHeartRate_Avg = L"HeartRate_Avg";
    const stringw xtagHeartRate_Max = L"HeartRate_Max";
    const stringw xtagCadence = L"Cadence";
    const stringw xtagCadence_Avg = L"Cadence_Avg";
    const stringw xtagCadence_Max = L"Cadence_Max";
    const stringw xtagCalories = L"Calories";
    const stringw xtagPulsePower = L"PulsePower";
    const stringw xtagDragFactor = L"DragFactor";
    const stringw xtagSS = L"SpinScan";
    const stringw xtagSSLeft = L"SpinScanLeft";
    const stringw xtagSSRight = L"SpinScanRight";
    const stringw xtagSSLeftSplit = L"SpinScanLeftPowerSplit";
    const stringw xtagSSRightSplit = L"SpinScanRightPowerSplit";
    const stringw xtagSSLeftATA = L"SpinScanLeftAveTourqeAngle";
    const stringw xtagSSRightATA = L"SpinScanRightAveTourqeAngle";
    const stringw xtagSSLeft_Avg = L"SpinScanLeft_Avg";
    const stringw xtagSSRight_Avg = L"SpinScanRight_Avg";
    const stringw xtagPercentAT = L"PercentAnareobicThreshhold";
    const stringw xtagFrontGear = L"FrontGear";
    const stringw xtagRearGear = L"RearGear";
    const stringw xtagGearInches = L"GearInches";
    const stringw xtagRawSpinScan = L"RawSpinScan";
    const stringw xtagCadenceTiming = L"CadenceTiming";
    const stringw xtagTSS = L"TrainingStressScore";
    const stringw xtagIF = L"IntensityFactor";
    const stringw xtagNP = L"NormalizedPower";
    const stringw xtagBars = L"Bars";
    const stringw xtagBars_Shown = L"Bars_Shown";
    const stringw xtagAverageBars = L"AverageBars";
    const stringw xtagRiderName = L"RiderName";
    const stringw xtagCourseScreenX = L"CourseScreenX";
    const stringw xtagOrder = L"Order";
    const stringw xtagHardwareStatus = L"HardwareStatus";
    const stringw xtagStatsFlags = L"StatsFlags";
    const stringw xtagRRC = L"RollingCalibration";
    const stringw xtagTimeMS = L"TimeMilliseconds";
    const stringw xtagGender = L"Gender";
    const stringw xtagAge = L"Age";
    const stringw xtagHeight = L"Height";
    const stringw xtagWeight = L"Weight";
    const stringw xtagUpper_HeartRate = L"Upper_HeartRate";
    const stringw xtagLower_HeartRate = L"Lower_HeartRate";
    const stringw xtagCourseName = L"CourseName";
    const stringw xtagCourseType = L"CourseType";
    const stringw xtagLaps = L"Laps";
    const stringw xtagCourseLength = L"CourseLength";
    const stringw xtagRFDrag = L"RFDrag";
    const stringw xtagRFMeas = L"RFMeas";
    const stringw xtagWatts_Factor = L"Watts_Factor";
    const stringw xtagFTP = L"FunctionalThreshholdPower";
    const stringw xtagPerfCount = L"PerfCount";

    const stringw xtagRMX = L"RMX";
    const stringw xtagHeader = L"Header";
    const stringw xtagCourse = L"Course";
    const stringw xtagWattsType = L"WattsType";
    const stringw xtagVal = L"Val";
    const stringw xtagEndWatts = L"EndWatts";
    const stringw xtagStartWatts = L"StartWatts";
    const stringw xtagMinutes = L"Minutes";
    const stringw xtagCount = L"Count";
    const stringw xtagDistanceType = L"DistanceType";
    const stringw xtagLength = L"Length";
    const stringw xtagThreeDType = L"ThreeDType";
    const stringw xtagRotation = L"Rotation";
    const stringw xtagRCVType = L"RCVType";
    const stringw xtagGPSData = L"GPSData";
    const stringw xtagLooped = L"Looped";
    const stringw xtagModified = L"Modified";
    const stringw xtagReverse = L"Reverse";
    const stringw xtagMirror = L"Mirror";
    const stringw xtagEndAt = L"EndAt";
    const stringw xtagStartAt = L"StartAt";
    const stringw xtagType = L"Type";
    const stringw xtagFileName = L"FileName";
    const stringw xtagDescription = L"Description";
    const stringw xtagName = L"Name";
    const stringw xtagCourseInfo = L"CourseInfo";
    const stringw xtagInfo = L"Info";
    const stringw xtagDataFlags = L"DataFlags";
    const stringw xtagData = L"Data";
    const stringw xtagCompressType = L"CompressType";
    const stringw xtagCopyright = L"Copyright";
    const stringw xtagComment = L"Comment";
    const stringw xtagCreatorExe = L"CreatorExe";
    const stringw xtagDate = L"Date";
    const stringw xtagVersion = L"Version";
    const stringw xtagKeyFrame = L"KeyFrame";
    const stringw xtagStatFlags = L"StatFlags";

    const stringw xvalRacerMateOne = L"RacerMateOne";
    const stringw xvalCopyright = L"(c) 2011, RacerMateOne, Inc.";
    const stringw xvalNone = L"None";
    const stringw xvalTrue = L"true";
	const stringw xvalDistance = "Distance";
	const stringw xvalWatts = "Watts";
	const stringw xvalVideo = "Video";
	const stringw xvalThreeD = "ThreeD";
	const stringw xvalGPS = "GPS";
	const stringw xvalPerformance = "Performance";

/*
public struct RMPHeader
{
    public string CreatorExe; // 32 program that created
    public DateTime Date;  // date created
    public float Version;    // version of the this format
    public string Comment; // 32 description of this file
    public string Copyright;   // 32 RacerMate copyright
    public Int32 CompressType; // different compression type
}   
*/

bool CCourse::loadRMP(char *fname)  
{
	CourseInfo ci;
	CourseSegment cs;
	CourseSegment *pcs = NULL;


	stringw tstr = L"";
	int arrCnt=0;
	int idx = 0;
	int bp;
	// set a 10 level stack
	stringw curEl[10];
	int ilvl = -1;
	for(int i = 0; i < 10; i++)
		curEl[i] = L"";

	irr::io::IXMLReader* xml = D3DBase::GetDevice()->getFileSystem()->createXMLReader(fname);	//create xml reader
	if ( !xml )
		return false;

	stringw curNode=L"";;					//keep track of our currentsection

	//while there is more to read
	while(xml->read())
	{
		//check the node type
		switch(xml->getNodeType())
		{
		case irr::io::EXN_TEXT:
			{
				if(ilvl == 4 && curEl[ilvl-2].equals_ignore_case(ci.Type) && curEl[ilvl-1].equals_ignore_case(xtagVal))
				{
					if(curEl[ilvl].equals_ignore_case(xtagLength))
						cs.Length = toFloat(xml->getNodeData());
					else if(curEl[ilvl].equals_ignore_case(xtagGrade))
						cs.Grade = toFloat(xml->getNodeData());
					else if(curEl[ilvl].equals_ignore_case(xtagWind))
						cs.Wind = toFloat(xml->getNodeData());
					else if(curEl[ilvl].equals_ignore_case(xtagRotation))
					{
						cs.Rot = radToDeg(toFloat(xml->getNodeData()));
						if(0.001 < fabs(cs.Rot))
							cs.Type = Section::CURVE;
					}
				}
			}
			break;
		//we found a new element
		case irr::io::EXN_ELEMENT:
			{
				curNode = xml->getNodeName();
				ilvl++;
				curEl[ilvl] = curNode;

				switch(ilvl)
				{
				case 0:
					break;
				case 1:
					break;
				case 2:
					if(curEl[ilvl-2].equals_ignore_case(xtagRMX) && curEl[ilvl-1].equals_ignore_case(xtagCourse))
					{
						if(xtagInfo.equals_ignore_case(curNode))
						{
							ci.Name = xml->getAttributeValueSafe(xtagName.c_str());
							ci.Description = xml->getAttributeValueSafe(xtagDescription.c_str());
							ci.FileName = xml->getAttributeValueSafe(xtagFileName.c_str());

							ci.Type = xtagDistanceType;
							ci.iType = Distance;
							
							core::array<core::stringw> sArray;
							core::stringw sTypes = xml->getAttributeValueSafe(xtagType.c_str());
							sTypes.split(sArray,L" ,",2);

							ci.iType = 0;
							for(u32 i=0; i < sArray.size(); i++)
							{
								core::stringw sType = sArray[i];
								if(sType.equals_ignore_case(xvalDistance))
								{
									ci.iType |= Distance;
								}
								else if(sType.equals_ignore_case(xvalThreeD))
								{
									ci.iType |= ThreeD;
								}
								else if(sType.equals_ignore_case(xvalVideo))
								{
									ci.iType |= Video;
								}
								else if(sType.equals_ignore_case(xvalGPS))
								{
									ci.iType |= GPS;
								}
								else if(sType.equals_ignore_case(xvalPerformance))
								{
									ci.iType |= Performance;
								}
								else if(sType.equals_ignore_case(xvalWatts))
								{
									ci.iType |= Watts;
									// Don't let a Watt type be used on a 3D yet
									xml->drop();
									return false;
								}
							}

							if(ci.iType & Video)
								ci.iType = Video;

							if(ci.iType & ThreeD)
							{
								ci.Type = xtagThreeDType;
							}
							else if(ci.iType & Video)
							{
								ci.Type = xtagRCVType;
							}
							else if(ci.iType & GPS)
							{
								ci.Type = xtagGPSData;
							}
							else if(ci.iType & Watts)
							{
								ci.Type = xtagWattsType;
								// Don't let a Watt type be used on a 3D yet
								xml->drop();
								return false;
							}
							else
							{
								if(ci.iType == 0)
									ci.iType = Distance;
								ci.Type = xtagDistanceType;
							}

							tstr = xml->getAttributeValueSafe(xtagLooped.c_str());
							ci.Looped = tstr.equals_ignore_case(xvalTrue.c_str());
							ci.Laps = xml->getAttributeValueAsInt(xtagLaps.c_str());
							ci.EndAt = xml->getAttributeValueAsFloat(xtagEndAt.c_str());
							ci.StartAt = xml->getAttributeValueAsFloat(xtagStartAt.c_str());
							tstr = xml->getAttributeValueSafe(xtagMirror.c_str());
							ci.Mirror = tstr.equals_ignore_case(xvalTrue.c_str());
							tstr = xml->getAttributeValueSafe(xtagReverse.c_str());
							ci.Reverse = tstr.equals_ignore_case(xvalTrue.c_str());
							tstr = xml->getAttributeValueSafe(xtagModified.c_str());
							ci.Modified = tstr.equals_ignore_case(xvalTrue.c_str());
						}
						else if(ci.Type.equals_ignore_case(curNode))
						{
							arrCnt = xml->getAttributeValueAsInt(xtagCount.c_str());
							pcs = new CourseSegment[arrCnt];
						}
					}
					break;
				case 3:
					if(curEl[ilvl-2].equals_ignore_case(xtagCourse) && curEl[ilvl-1].equals_ignore_case(ci.Type))
					{
						if(xtagVal.equals_ignore_case(curNode))
						{
							cs.Length = 0;
							cs.Grade = 0;
							cs.Wind = 0;
							cs.Rot = 0;
							cs.Type = Section::STREIGHT;
						}
					}
				case 4:
					break;
				default:
					printf("%s not handled/n", curNode);
					break;
				}
				if(xml->isEmptyElement())
				{
					curEl[ilvl] = L"";
					ilvl--;
				}
			}
			break;
		//we found the end of an element
		case irr::io::EXN_ELEMENT_END:
			{
				curNode = curEl[ilvl];
				// at end of RMX/Course/Type/Val - save each segment dta
				if(ilvl == 3 && curEl[ilvl-1].equals_ignore_case(ci.Type) && curEl[ilvl].equals_ignore_case(xtagVal))
				{
					if(idx < arrCnt)
					{
						pcs[idx++] = cs;
					}
				}
				else  // at end of RMX/Course/Type - write out all saved segment data
				if(ilvl == 2 && curEl[ilvl-2].equals_ignore_case(xtagRMX) && curEl[ilvl-1].equals_ignore_case(xtagCourse))
				{
					if(curEl[ilvl].equals_ignore_case(ci.Type))
					{
						Section *ps=NULL;
						m_pStartSection = NULL;
						m_pFinishSection = NULL;
						laps = 1;

						m_bLoopClosed = m_bLooped = m_bReverse = m_bMirror = false;
						m_StartAt = m_EndAt = m_SecondStart = 0.0f;
						
						bool b3D = xtagThreeDType.equals_ignore_case(ci.Type);
						float cRndRot = 0.0f;

						//m_bLooped = ci.Looped;
						m_StartAt = ci.StartAt;
						m_EndAt = ci.EndAt;
						m_bReverse = ci.Reverse;
						m_bMirror = ci.Mirror;
						laps = ci.Laps;

						if(ci.Looped && b3D && !ci.Modified)
						{
						}
						else
						{
							ps = new Section(*this, Section::START, 4.0f, 0.0f, 0.0f, TEX_START, -4.0f);
							m_pStartSection = ps;
						}

						course_meters=0;

						float ratio = 90.0f;
						float xRandom = frand(0.0f,100.0f);
						float xMult = xRandom < (ratio/2) ? -1 : 1;
						float segmentRot = frand(MAX_CRS_SEGMENT_ROT/3,MAX_CRS_SEGMENT_ROT); 
						float segmentLen = frand(25.0f, MAX_CRS_SEGMENT_LEN / 2); 

						float lastGrade = 0;

						for (int i=0; i<arrCnt; i++)  
						{
							// if not 3dtype randomize rotation and road
							// if modified then ignore the curves
							if(!b3D || ci.Modified)
							{
								// if the last segment, subtract 4 and we will add at back the end 
								if(i==arrCnt-1 && pcs[i].Length > 4.0f)
								{
									pcs[i].Length -= 4.0f;
								}

								// Check if we can cut up the leg to shorter ones to make the course more interesting
								// If b3D, do not cut up
								bool bAddTransition = b3D ? false : ((lastGrade != pcs[i].Grade && pcs[i].Length > 20.0f) ? true : false);
								if(bAddTransition)
									pcs[i].Length -= 4.0f;

								float legLength = b3D ? 1 : pcs[i].Length / MAX_CRS_SEGMENT_LEN;

								CourseSegment tcs;
								int icnt = 1;
								if(legLength > 1.0f)
									icnt = legLength + 1;
								else
									legLength = 1.0f;

								if(bAddTransition)
								{
									ps = new Section(*this, Section::STREIGHT, 4.0f, pcs[i].Grade, 0);
									course_meters += ps->GetLength();
								}

								tcs.Grade = pcs[i].Grade; 
								tcs.Length = pcs[i].Length / icnt;

								// cap the rotation to 2 degrees per divisions - maybe
								float rotCap = min(MAX_CRS_SEGMENT_ROT, tcs.Length / 2);

								for(int j = 0; j < icnt; j++)
								{
									tcs.Rot = 0;
									xRandom = frand(0.0f,100.0f);
									tcs.Type = (course_meters < 20.0f || xRandom > ratio) ? Section::STREIGHT : Section::CURVE;

									if(tcs.Type == Section::CURVE )
									{
										if(segmentRot <= 0 || segmentLen <= 0 || fabs(cRndRot) > MAX_CRS_SEGMENT_ROT )
										{
											segmentLen = frand(25.0f, MAX_CRS_SEGMENT_LEN / 2); 
											segmentRot = frand(MAX_CRS_SEGMENT_ROT/3, MAX_CRS_SEGMENT_ROT); 
											xMult = xRandom < (ratio/2) ? -1 : 1;
											if(cRndRot < -MAX_CRS_SEGMENT_ROT)
												xMult = 1;
											else if(cRndRot > MAX_CRS_SEGMENT_ROT)
												xMult = -1;
										}
										float tRot = min(rotCap, segmentRot);
										segmentRot -= tRot;
										segmentLen -= tcs.Length;

	 									tcs.Rot =  tRot * xMult; 
										cRndRot += tcs.Rot;
									}

									ps = new Section(*this, tcs.Type, tcs.Length, tcs.Grade, tcs.Rot);
									course_meters += ps->GetLength();
								}
								lastGrade = pcs[i].Grade;
							}
							else
							{
								ps = new Section(*this, pcs[i].Type, pcs[i].Length, pcs[i].Grade, pcs[i].Rot);
								switch(ps->m_Type)  
								{
								case Section::STREIGHT:
								case Section::CURVE:
									course_meters += ps->GetLength();
									break;
								default:
									bp = 1;
									break;
								}
							}
						}
						// Assume these courses are real looped courses
						if(ci.Looped && b3D && !ci.Modified)
						{
							ps = m_Track.GetLast();
							ps->m_StartDistance = -4.0f;
							ps->m_Type = Section::START;
							ps->m_SpecialTexture = TEX_START;
							m_Track.AddHead(*ps);

							m_bLoopClosed = true;
						}
						else
						{
							ps = m_Track.GetLast();
							if(!b3D || (4.0f < ps->GetLength()) || 0 != ps->GetSectionRotation() || ci.Modified) // Add a finish road
							{				
								ps = new Section(*this, Section::FINISH, 4.0f, 0.0f, 0.0f);
								course_meters += ps->GetLength();
							}
							ps = m_Track.GetLast();
							ps->m_Type = Section::FINISH;
							ps->m_SpecialTexture = TEX_FINISH;
							m_pFinish = m_pFinishSection = ps;
							
						}
						
						if(ci.Laps > 1)
							m_bLooped = true;

						// default to zeros
						m_Offset.x = 0;
						m_Offset.y = 0;
						m_Offset.z = 0;
						m_BaseRotation = 0;

						
						delete[] pcs;
						pcs = NULL;
					}
				}

				curEl[ilvl] = L"";
				ilvl--;
			}
			break;
		}
	}
	// don't forget to delete the xml reader
	xml->drop();
	return true;
}

/************************************************************************************

************************************************************************************/

bool CCourse::loadNew(FILE *file)  {
	fseek(file, 4, SEEK_SET);								// seek past version number


	bool flag;
	if (m_Version==7)  {
		flag = true;			// encrypted
	}
	else  {
		flag = false;			// not encrypted
	}
	#ifdef DO_ENCRYPTION2
	SecFile f(file, flag);
	#else
	SecFile f(file);
	#endif

	bool gottrack = false;
	m_pStartSection = NULL;
	m_pFinishSection = NULL;
	laps = 1;
	m_bLoopClosed = m_bLooped = m_bReverse = m_bMirror = false;
	m_StartAt = m_EndAt = m_SecondStart = 0.0f;

	long offs;


	while (!f.eof() && f.IsOK())  {

		if (f.IsSection("C_TK") || f.IsSection("CTK2"))  {

			gottrack = true;
			int cnt = f.ReadLong();
			int i;
			Section *ps=NULL;


			course_meters = 0.0f;
			float miles = 0.0f;
			float ff;
			bool started = false;

			for (i=0; i<cnt; i++)  {
				if ( f.IsOK() )  {
					ps = new Section(*this, f);
					if (ps->IsFinishSection() && !m_pFinishSection)  {
						m_pFinishSection = ps;
					}

					switch(ps->m_Type)  {
						case Section::STREIGHT:
						case Section::CURVE:
							if (started)  {
								course_meters += ps->GetLength();
								miles = METERSTOMILES * course_meters;
							}
							else  {
								ff = ps->GetLength();		// 6.0f
							}
							bp = 0;
							break;

						case Section::START:
							ff = ps->GetLength();			// 4.0f
							started = true;
							bp = i;
							break;

						case Section::FINISH:
							ff = ps->GetLength();			// 4.0f
							bp = i;
							break;

						case Section::EXTRA:
							ff = ps->GetLength();			// 100.0f
							bp = i;
							break;

						default:
							bp = 1;
							break;
					}
				}
			}

			ps = m_Track.GetLast();

			if (ps==NULL)  {
#ifndef WEBAPP
				//throw (fatalError(__FILE__, __LINE__));
#else
				return false;
#endif
			}

			if (ps->m_Type == Section::CLOSELOOP)  {
				m_bLoopClosed = true;
			}
		}
		else if (f.IsSection("perf"))  {
			f.Read(pinf + 1, sizeof(pinf[1]));

			if (pinf[1].version==1 || pinf[1].version==2)  {
				m_pPerfArr[1] = new PerfPoint[pinf[1].perfcount];
				f.Read(m_pPerfArr[1], sizeof(PerfPoint) * pinf[1].perfcount);
			}
			gPerformanceOK = true;
		}

#ifdef VELOTRON
		else if (f.IsSection("GEAR"))  {

			int n = f.ReadLong();							// get the number of gear points


			if (gearInfoArray[1])  {
				#ifndef WEBAPP
				//throw (fatalError(__FILE__, __LINE__));
				#else
				return false;
				#endif
			}

			gearInfoArray[1] = new ComputrainerBase::GEARINFO[n];
			int sz = sizeof(ComputrainerBase::GEARINFO);
			sz *= n;
			f.Read(gearInfoArray[1], n * sizeof(ComputrainerBase::GEARINFO));
		}
	#else
		// computrainer:
		else if (f.IsSection("GEAR"))  {
		}
	#endif
#ifdef LOG_CALORIES
		else if (f.IsSection("CALS"))  {

			int n = f.ReadLong();							// get the number of gear points


			if (calories[1])  {
#ifndef WEBAPP
				//throw (fatalError(__FILE__, __LINE__));
#else
				return false;
#endif
			}

			calories[1] = new float[n];
			int sz = sizeof(float);
			sz *= n;
			f.Read(calories[1], n * sizeof(float));
		}
#endif

		else if (f.IsSection("LAPS"))  {
			SetLaps(f.ReadLong());
		}
		else if (f.IsSection("Name"))  {

			char buf[256];
			f.ReadString(buf);
			SetCourseName(buf);
		}
		else if (f.IsSection("Desc"))  {

			char buf[256];
			f.ReadString(buf);
			SetCourseDescription(buf);
		}
		else if (f.IsSection("CRTR"))  {


			char buf[256];
			f.ReadString(buf);
			SetCourseCreator(buf);
		}
		else if (f.IsSection("Wind"))  {

			int cnt = f.ReadLong();
			int i;
			float wind;
			Section *ps;
			for (i = 0,ps = m_Track.GetFirst(); i < cnt && ps; ps = ps->Next(),i++)  {
				wind = f.ReadFloat();
				ps->SetWind(wind);
			}

		}
		else if (f.IsSection("SSTR"))  {

			m_SecondStart = f.ReadFloat();
		}
		else if (f.IsSection("OFST"))  {

			m_Offset.x = f.ReadFloat();
			m_Offset.y = f.ReadFloat();
			m_Offset.z = f.ReadFloat();
			m_BaseRotation = f.ReadFloat();
		}
		else if (f.IsSection("CTPF"))  {
		}
		else if (f.IsSection("_CRS"))  {
			has_crs = true;
			crs_nlegs = f.ReadLong();

			crs = new CRSLEG[crs_nlegs];
			accum_meters = new float[crs_nlegs];

			float mtrs = 0.0f;

			for(int i=0; i<crs_nlegs; i++)  {
				crs[i].meters = f.ReadFloat();
				crs[i].grade = f.ReadFloat();
				crs[i].wind_kph = f.ReadFloat();
				mtrs += crs[i].meters;
				accum_meters[i] = mtrs;
				bp = i;
			}
			bp = 1;
		}

		else  {
#ifndef WEBAPP
			bp = 1;
			//throw (fatalError(__FILE__, __LINE__));
#else
			return false;
#endif
		}

		offs = ftell(file);

		f.NextSection();
	}


	return f.IsOK();
}					// loadNew()

/************************************************************************************

************************************************************************************/

bool CCourse::loadOld(FILE *file)  {
//#ifndef IGNOREFORNOW

	fseek(file, 0, SEEK_SET);						// go to the version offset
	BOOL finished;
	fread(&m_Version, sizeof(int), 1, file);
	fread(&finished, sizeof(BOOL), 1, file);m_Finished = (finished != FALSE);
	fread(&m_Length, sizeof(float), 1, file);

	int sections;
	fread(&sections, sizeof(int), 1, file);

	// Create the the different sections

	new Section(*this, Section::STREIGHT, 6.0f, 0.0f, 0.0f, TEX_ROAD, -10.0f);
	new Section(*this, Section::START, 4.0f, 0.0f, 0.0f, TEX_START, -4.0f);

	float deg = 0.0f;
	float grade = 0.0f;

	for (int i = 0; i < sections; i++)  {
		int type;
		float len;
		float gradeamount;
		float rotamount;
		fread(&type, sizeof(int), 1, file);
		fread(&len, sizeof(float), 1, file);
		fread(&gradeamount, sizeof(float), 1, file);
		fread(&rotamount, sizeof(float), 1, file);

		deg = rotamount * -572.965;
		grade = gradeamount / 100.0f;

		//if (m_Version < 5)
		//	len *= 2;

		if (type > HILL)
			new Section(*this, Section::CURVE, len, grade, deg);
		else if (type > -1)
			new Section(*this, Section::STREIGHT, len, grade, deg);
		else  {
			m_pFinishSection = new Section(*this, Section::FINISH, 4.0f, grade, deg, TEX_FINISH);
			if (len > 4.0f)  {
				new Section(*this, Section::FINISH, len - 4.0f, grade, deg);
			}
		}
	}


	fread(&m_TotalLaps, sizeof(int), 1, file);
	if (m_TotalLaps == 0)  {
		if (!m_pFinishSection)  {
			m_pFinishSection = new Section(*this, Section::FINISH, 2.0f, grade, deg, TEX_FINISH);
			new Section(*this, Section::FINISH, 10.0f, grade, deg);
		}

		Section *psec = new Section(*this, Section::STREIGHT, 2.0f, grade, 0.0f, TEX_OVERRUN);
	}
	else  {
		Section *ps = m_Track.GetLast();
		float len = 0.0f;
		while (ps && (len < 100.0f))  {
			len += ps->m_Length;
			delete ps;
			if (EditCloseLoop(true, true))
				break;
			ps = m_Track.GetLast();
		}
	}

	int ptstripsize;
	int totalverts;
	YColor *pointyval = 0L;

	fread(&ptstripsize, sizeof(int), 1, file);

	if (ptstripsize > 0)  {
		pointyval = (YColor *) calloc(ptstripsize, (sizeof(YColor)));
		fread(pointyval, ptstripsize * sizeof(YColor), 1, file);
		totalverts = ptstripsize - 11;
		if (m_Version < 2)  {
			// If version 1, delete mesh info so new one will be made.
			totalverts = 0;
			free(pointyval);
			pointyval = NULL;
		}


		free(pointyval);
		pointyval = NULL;
	}
	return true;
	// Dummy Read the edge data.
#ifndef IGNOREFORNOW

	int r_Ne;
	int Nn, Ne;

	fread(&r_Ne, sizeof(int), 1, file);
	if (r_Ne != 0)  {
		fread(&Nn, sizeof(int), 1, file);
		fseek(file, Nn * sizeof(nod), SEEK_CUR);
		fread(&Ne, sizeof(int), 1, file);
		fseek(file, Ne * sizeof(ele), SEEK_CUR);
	}

	// Performance points.

	if ((m_Version >= 3) && (m_Version <= 5))  {
		int cnt;

		PerformanceInfo &pi = pinf[0];
/* ect-todo
		#ifdef BIKE_APP
		RiderData &rd = gpBikeApp->GetDisplayPrefs().riderdata[0];
		#else
		//RiderData rd(metric);
		RiderData rd;
		#endif
		*/
		RiderData rd;

		strcpy(pi.name, rd.GetName());
		pi.gender = (unsigned char) rd.GetGender();
		pi.age = (unsigned char) rd.GetAge();
		pi.height = rd.GetHeight_Meters();
		pi.weight = rd.GetWeight_KPS();

		#ifndef VELOTRON
		pi.rfDrag = rd.GetRfDrag();
		pi.rfMeas = rd.GetRfMeas();
		#endif

		fread(&pi.lower_hr, 4, 1, file);
		fread(&pi.upper_hr, 4, 1, file);

		if ((fread(&cnt, sizeof(int), 1, file) == 1) && (m_Version >= 3))  {
			pinf[0].perfcount = cnt;

			int psize = 0;
			switch (m_Version)  {
				case 3:
					psize = sizeof(PERFPOINTSv3);break;
				case 4:
					psize = sizeof(PERFPOINTSv4);break;
				case 5:
					psize = sizeof(PERFPOINTSv5);break;
			}


			if ((psize > 0) && (cnt > 0))  {
				m_pPerfArr[0] = new PerfPoint[cnt];
				memset(m_pPerfArr[0], 0, sizeof(PerfPoint) * cnt);
				PerfPoint *p = m_pPerfArr[0];
				for (int i = 0; i < cnt; i++,p++)  {
					if (fread(p, psize, 1, file) != 1)  {
						ClearProfile();
						break;
					}
				}
			}
		}
		gPerformanceOK = true;
	}
	return true;
#endif
//#else
//	return false;
//#endif
}							// loadold()

/************************************************************************************

************************************************************************************/

bool CCourse::is_encrypted(char *fname)  {
	long ltemp;
	int status;
	bool e = false;

	FILE *stream = fopen(fname, "rb");
	status = fread(&ltemp, 4, 1, stream);			// read the version
	assert(status==1);

	if (ltemp!=7)  {
		fclose(stream);
		return false;
	}

	status = fread(&ltemp, 4, 1, stream);			// read the section type
	assert(status==1);

	switch(ltemp)  {
		case 0x4b545f43:									// "C_TK" backwards
		case 0x5350414c:									// "LAPS" backwards
		case 0x5453464f:									// "OFST" backwards
		case 0x66726570:									// "perf" backwards
			e = false;
			break;
		default:
			e = true;
			break;
	}
	fclose(stream);

	return e;
}

/************************************************************************************

************************************************************************************/

bool CCourse::Load(const char *basename)  {

	ClearProfile();

	char buf[MAX_PATH];
	char *name = makeName(basename, buf);
	strcpy(fullpath, name);				// curpath + filename
	strcpy(curpath, name);					// curpath part only
	strip_filename(curpath);


	FILE *file;

	bp = 1;


	// decrypt buf here, then re-encrypt later
	//dec(buf);

	file = fopen(name, "rb");
	if (!file)  {
		sprintf(m_Error, "Cannot open \"%s\".", name);
		return false;
	}
	ClearTrack();
	fread(&m_Version, sizeof(int), 1, file);

	SetCourseName(basename);
	//setString(m_pFileName, name);
	DELARR(m_pFileName);
	m_pFileName = new char[strlen(basename) + 1];
	strcpy(m_pFileName, basename);

	bool ans;
	bool encrypted = false;

	if (m_Version == 5)  {		// Possibly new.
		char c[5];
		fread(c, 4, 1, file);
		c[4] = '\0';
		if (strcmp(c, "CTPF") == 0)
			ans = loadNew(file);
		else
			ans = loadOld(file);
	}
	//else if (m_Version == COURSEFILE_VERSION)
	else if (m_Version == 6)  {
		ans = loadNew(file);
	}
	else if (m_Version==7)  {
		fclose(file);
		encrypted = is_encrypted(name);
		if (encrypted) {
			//doo(name);								// decrypt the file, only for computrainer, not velotron
			unencrypt(name);
			encrypted = false;
		}
		file = fopen(name, "rb");
		if (!file)  {
			sprintf(m_Error, "Cannot open \"%s\".", name);
			return false;
		}
		fseek(file, 4, SEEK_SET);
		ans = loadNew(file);
	}

	else if (m_Version < 0)  {
		sprintf(gstring, "version error: %ld", m_Version);
		//throw (fatalError(__FILE__, __LINE__, gstring));
	}

	//else if (m_Version < COURSEFILE_VERSION)  {
	else if (m_Version < 6)  {
		ans = loadOld(file);
	}

	fclose(file);

	if (m_Version==7)  {
		#ifndef VELOTRON
		if (encrypted) {
			doo(name);									// re-encrypt the file
		}
		#endif
	}

	// Determin the course length

	finishTrack();
	ReadyEdit();

#ifdef BIKE_APP
	RedoCourse();
#endif

//#ifndef IGNOREFORNOW

//#ifdef N915
	// WW: Added N915 - Fixes startup bug where the courses don't match.
	// ==============
	if ((m_bLoopClosed) && (m_Version==6 || m_Version==7))  {
		_bLoading = true;		// WW: NC18 - Added to fix performance bug tlm20031219
		EditDelete();
		EditCloseLoop(true);
		_bLoading = false;		// WW: NC18 - Added to fix performance bug tlm20031219
	}
	// ==============
//#endif
//#endif
	computeStats();

	return true;
}		// CCourse::Load()

void CCourse::HideAll(void)  {
//return;
	Section *ps;
	for (ps = m_Track.GetFirst(); ps; ps = ps->Next())  {
		ps->Show(false);
	}

	Terrain **ppn;
	int x, z;
	if (m_pTerrainGrid) {
		for (z = 0; z <= m_GridHeight; z++)  {
			//nodes = nodepart;
			for (x = 0; x <= m_GridWidth; x++)  {
				//if(x >= nodes)
				//	nodes += nodepart;
				ppn = m_pTerrainGrid + (x + z *m_GridWidth);
				if (*ppn)
					(*ppn)->Show(false);
			}
		}
	}
}


void CCourse::ShowVisible(ICameraSceneNode* cam, Section *psec, bool bShow)  {
	if(!bShow)
	{
		//Section::ClearRendered();
		//Terrain::ClearRendered();
		return;
	}

	{
		Section *ps;
		bool found;
		vector3df vrot,vdir,vobj,vcam,v,minv,maxv,center,vtarget;
		f32 dist, dot, r;

		f32 culldist = fardistCam * 2;
		f32 culldistM = culldist * mult;
		f32 d = culldistM;

		vcam = cam->getAbsolutePosition();
		vtarget = cam->getTarget();
		vcam.y = 0;
		vtarget.y = 0;
		vdir = vtarget - vcam;
		vdir.normalize();

		Section *psNext = Section::ms_RenderList;
		Section::ms_RenderList = NULL;
		for(Section *ps = psNext;ps;ps = psNext)
		{
			psNext = ps->m_pNextRender;

			vobj= (ps->GetExtOrigin() * mult);
			vobj.y = 0;
			r = (ps->GetExtRadius() * mult * 2);
			v = vobj - vcam;
			dist = v.getLength();
			if(dist - r < d)
			{
				if(r > dist)
				{
					ps->AddRender();
					continue;
				}
				else
				{
					dot = vdir.dotProduct(vobj);
					if(dot > 0)
					{
						ps->AddRender();
						continue;
					}
				}
			}
			// remove		
			ps->Show(false);
			ps->m_pNextRender = NULL;
			ps->m_bRender = false;
		}

		psec = m_Track.GetFirst();
		for (ps = psec; ps; ps = psNext)  {
			psNext = ps->Next();
			if(ps->m_bRender)
				continue;
			found = false;
			vobj= (ps->GetExtOrigin() * mult);
			vobj.y = 0;
			r = (ps->GetExtRadius() * mult * 2);
			v = vobj - vcam;
			dist = v.getLength();
			if(dist - r < d)
			{
				if(r > dist)
				{
					ps->Show(bShow);
					found = true;
				}
				else
				{
					dot = vdir.dotProduct(vobj);
					if(dot > 0)
					{
						ps->Show(bShow);
						found = true;
					}
				}
			}
		}

		Terrain *ptNext = Terrain::ms_RenderList;
		Terrain::ms_RenderList = NULL;
		for(Terrain *pt = ptNext;pt;pt = ptNext)
		{
			ptNext = pt->m_pNextRender;

			ISceneNode *tNode=pt->GetNode();
			if(!tNode)
				continue;

			core::aabbox3d<f32> box = tNode->getBoundingBox(); 
			tNode->getAbsoluteTransformation().transformBox(box); 
			vobj = box.getCenter(); 
			r = (float)vobj.getDistanceFrom(box.MinEdge) * 3; 
			vobj.y = 0;
			v = vobj - vcam;
			dist = v.getLength();
			if(dist - r < d)
			{
				//pt->AddRender();
				//continue;
				if(r > dist)
				{
					pt->AddRender();
					continue;
				}
				else
				{
					dot = vdir.dotProduct(vobj);
					if(dot > 0)
					{
						pt->AddRender();
						continue;
					}
				}
			}
			// remove		
			pt->Show(false);
			pt->m_pNextRender = NULL;
			pt->m_bRender = false;
		}

		//Terrain::ClearRenderList();
		if (m_pTerrainGrid)  {
			int fx, fz, tx, tz;
			int x, z;
			Terrain **ppn;

			center = vcam/mult + (vdir * (culldist/2));
			minv = center - vector3df(culldist,culldist,culldist);
			maxv = center + vector3df(culldist,culldist,culldist);
			GridXY(minv.x, minv.z, fx, fz);
			GridXY(maxv.x, maxv.z, tx, tz);			// fx, fz - tx, tz = (9, 10 - 12, 16), eg

			Terrain **ppstart = m_pTerrainGrid + (fx + fz*m_GridWidth);
			for (z=fz; z<=tz; z++, ppstart+=m_GridWidth)  {
				ppn = ppstart;
				for (x=fx; x<=tx; x++, ppn++)  {
					if (*ppn) { 
						if((*ppn)->m_bRender)
							continue;

						ISceneNode *tNode=(*ppn)->GetNode();
						if(!tNode)
							continue;

						core::aabbox3d<f32> box = tNode->getBoundingBox(); 
						tNode->getAbsoluteTransformation().transformBox(box); 
						vobj = box.getCenter(); 
						r = (float)vobj.getDistanceFrom(box.MinEdge) * 3; 
						vobj.y = 0;
						v = vobj - vcam;
						dist = v.getLength();
						if(dist - r < d)
						{
							//(*ppn)->Show(bShow);
							if(r > dist)
							{
								(*ppn)->Show(bShow);
								//found = true;
							}
							else
							{
								dot = vdir.dotProduct(vobj);
								if(dot > 0)
								{
									(*ppn)->Show(bShow);
									//found = true;
								}
							}
						}
						//if(!found && (dist > 2*d) )
						//	break;
					}
				}
			}
		}
	}
}

void CCourse::ClearModels()  {
	if(m_pSceneNode)
		m_pSceneNode->remove();
	m_pSceneNode = 0;
	m_pNodeVisible = 0;
	m_pNodeHidden = 0;
	Section::ClearRenderList();
	Terrain::ClearRenderList();
}

bool CCourse::ReadyModel()  {

	if(!m_pSceneNode)
		m_pSceneNode = D3DBase::addCustomSceneNode(mainNode);

	if(!m_pNodeVisible) {
		m_pNodeVisible = D3DBase::addCustomSceneNode(m_pSceneNode);
	}
	if(!m_pNodeHidden) {
		m_pNodeHidden = D3DBase::addCustomSceneNode(m_pSceneNode);
		m_pNodeHidden->setVisible(false);
	}


	Section *ps;
	for (ps = m_Track.GetFirst(); ps; ps = ps->Next())  {
		ps->ReadyModel();
	}

	int x,z;
	//int nodepart = m_GridWidth / 3;
	//int nodes = 0;
	Terrain **ppn;
	if (m_pTerrainGrid) {
		for (z = 0; z <= m_GridHeight; z++)  {
			//nodes = nodepart;
			for (x = 0; x <= m_GridWidth; x++)  {
				//if(x >= nodes)
				//	nodes += nodepart;
				ppn = m_pTerrainGrid + (x + z *m_GridWidth);
				if (*ppn)
					(*ppn)->ReadyModel(m_pNodeHidden);
			}
		}
	}
	return true;
}


/************************************************************************************

************************************************************************************/

void CCourse::calcTan()  {
	//std::vector<vector2df> tanarr;

	// x = time
	// y = height

	//vector3df &v = (pkeynext ? pkeynext->pos:(m_Loop ? m_KeyList[i].GetFirst()->pos:pkey->pos));
	//pkey->tan = ((1.0f - pkey->alpha)/2.0f) * (( pkey->pos - pkeyprev->pos ) + (v - pkey->pos));
	/*
		while(ps1 != ps2)
		{
			ps3 = ps2->Next();
			if (!ps3)
					ps3 = ps2;
			ps1 = ps2;
			ps2 = ps3;
		}
	*/
}


/************************************************************************************

************************************************************************************/

float CCourse::CourseDistance() const  {
	Section *ps = m_Track.GetLast();
	for (;
		  ps;
		  ps = ps->Prev())  {
		if(ps->IsFinishSection())
			return ps->m_StartDistance;
	}
	return 0.0f;
}


/************************************************************************************

************************************************************************************/

HRESULT CCourse::Open()  {
	return 0L;
}

/************************************************************************************

************************************************************************************/

HRESULT CCourse::Close()  {
#ifndef IGNOREFORNOW
	for (int i = 0; i < TEX_MAX;  i++)  {
		if (m_Tex[i])  {
			m_Tex[i]->Release();
			m_Tex[i] = 0L;
		}
	}
#else
	/*
	for (int i = 0; i < TEX_MAX;  i++)  {
		if (m_Tex[i])  {
			DELARR(m_Tex[i])
			m_Tex[i] = 0L;
		}
	}
	*/
#endif
	return 0L;
}

/************************************************************************************

************************************************************************************/

HRESULT CCourse::Restore()  {
	return 0L;
}

/************************************************************************************

************************************************************************************/

HRESULT CCourse::Invalidate()  {
	RedoCourse();
	return 0L;
}


/************************************************************************************

************************************************************************************/

void CCourse::RedoCourse()  {
	Section *ps;
	for (ps = m_Track.GetFirst(); ps; ps = ps->Next())  {
		#ifdef BIKE_APP
			ps->CloseModel();
		#endif
		ps->calcValues();
	}
}


/************************************************************************************

************************************************************************************/

void CCourse::ReadyEdit()  {
//#ifndef IGNOREFORNOW

	Section *ps;
	Section *pnext;
	float dirty = false;

	SidesOff();

	// Redo all the splits.
	// ====================

	for (ps = m_Track.GetFirst(); ps; ps = pnext)  {
		pnext = ps->Next();
		if (ps->m_Type == Section::SPLIT)  {
			Section *pm = ps->Prev();
			pm->m_Length += ps->GetLength();
			pm->m_DegRotation += ps->GetDegRotation();
			delete ps;
			ps = pm;
			dirty = true;
		}
	}
	RedoCourse();

	// Is there a finish line?

	for (ps = m_Track.GetFirst(); ps; ps = pnext)  {
		pnext = ps->Next();
		if (ps->m_SpecialTexture == TEX_OVERRUN)
			delete ps;
	}
	if (!m_Track.GetFirst())  {
		new Section(*this, Section::STREIGHT, 6.0f, 0.0f, 0.0f, TEX_ROAD, -100.0f);
		//cursec = 1;
		new Section(*this, Section::START, 4.0f, 0.0f, 0.0f, TEX_START, -4.0f);
		//cursec = 2;
	}
	m_pSISection = NULL;
	calcGrid();

//#endif
}


/************************************************************************************

************************************************************************************/

const SectionInfo & CCourse::GetSectionInfo(bool force)  {
	Section *ps = m_Track.GetLast();
	if ((ps == m_pSISection) && !force)
		return m_SI;		// Up to date.

	return updateSI();
}




/************************************************************************************

************************************************************************************/

bool CCourse::CheckCollision(Section *ps, Section *pc)  {
//#ifndef IGNOREFORNOW

	float t = ps->m_ExtOrigin.x - pc->m_ExtOrigin.x;
	double r = t *t;
	t = ps->m_ExtOrigin.z - pc->m_ExtOrigin.z;
	r += t * t;
	r = sqrt(r);
	if (r > (ps->m_ExtRadius + pc->m_ExtRadius))
		return false;

#ifndef IGNOREFORNOW
#ifdef BIKE_APP
	ps->ReadyEditModel();					// tlm: this is changing ps->m_StartWind!!!
	pc->ReadyEditModel();
#else
	ps->calcCArr();
	pc->calcCArr();
#endif
#else
	//ps->closeCArr();
	//ps->calcValues();													// tlm: this is changing m_StartWind!!!!
	ps->calcCArr();

	//pc->closeCArr();
	//pc->calcValues();													// tlm: this is changing m_StartWind!!!!
	pc->calcCArr();
#endif
	if (pc->Next() == ps)  {
		vector2df *psv1 = ps->m_pCArr + 1;
		vector2df *psv2 = ps->m_pCArr + 2;
		int imax = ps->m_TopPoints - 1;
		for (int i = 2;
			  i < imax;
			  i++,psv1 = psv2,psv2++)  {
			vector2df *pcv1 = pc->m_pCArr + (pc->m_TopPoints - 1);
			vector2df *pcv2 = pc->m_pCArr;
			for (int j = 0; j < pc->m_TopPoints; j++,pcv1 = pcv2,pcv2++)  {
				if (LinesIntersect(psv1->x, psv1->y, psv2->x, psv2->y, pcv1->x, pcv1->y, pcv2->x, pcv2->y))  {
					sHitRight = (i > ps->m_TopPoints / 2);
					cHitRight = (j > pc->m_TopPoints / 2);
					return true;
				}
			}
		}
	}
	else  {
		vector2df *psv1 = ps->m_pCArr + (ps->m_TopPoints - 1);
		vector2df *psv2 = ps->m_pCArr;
		for (int i = 0;
			  i < ps->m_TopPoints;
			  i++,psv1 = psv2,psv2++)  {
			vector2df *pcv1 = pc->m_pCArr + (pc->m_TopPoints - 1);
			vector2df *pcv2 = pc->m_pCArr;
			for (int j = 0;
				  j < pc->m_TopPoints;
				  j++,pcv1 = pcv2,pcv2++)  {
				if (LinesIntersect(psv1->x, psv1->y, psv2->x, psv2->y, pcv1->x, pcv1->y, pcv2->x,
										 pcv2->y))  {
					sHitRight = (i > ps->m_TopPoints / 2);
					cHitRight = (j > pc->m_TopPoints / 2);
					return true;
				}
			}
		}
	}
//#endif
	return false;
}

/************************************************************************************

************************************************************************************/

bool CCourse::CheckCollisionLine(Section *ps, float x1, float y1, float x2, float y2)  {
#ifndef IGNOREFORNOW

	vector2df sp(x1, y1);
	vector2df ep(x2, y2);
	vector2df cp(ps->m_ExtOrigin.x, ps->m_ExtOrigin.z);
	if (!LineHitCircle(sp, ep, cp, ps->m_ExtRadius))
		return false;

#ifdef BIKE_APP
	ps->ReadyEditModel();
#else
	ps->calcCArr();
#endif

	vector2df *psv1 = ps->m_pCArr + (ps->m_TopPoints - 1);
	vector2df *psv2 = ps->m_pCArr;
	for (int i = 0;
		  i < ps->m_TopPoints;
		  i++,psv1 = psv2,psv2++)  {
		if (LinesIntersect(psv1->x, psv1->y, psv2->x, psv2->y, x1, y1, x2, y2))
			return true;
	}
#endif
	return false;
}



/************************************************************************************

************************************************************************************/

SectionInfo & CCourse::updateSI(bool ignoreColl)  {
//#ifndef IGNOREFORNOW

	Section *ps = m_Track.GetLast();

	if (!ps)  {
		#ifdef BIKE_APP
			//gpCourse->ReadyEdit();
			ReadyEdit();
		#else
			ReadyEdit();
		#endif
		ps = m_Track.GetLast();
	}

	m_SI.grade = ps->get_grade_d_100();
	m_SI.deg = ps->GetDegRotation();
	m_SI.length = ps->GetLength();
	m_SI.startdist = ps->GetStartDistance();
	m_SI.startloc = ps->GetStartLoc();
	m_SI.enddist = ps->GetEndDistance();
	m_SI.endloc = ps->GetEndLoc();
	m_SI.elevation = ps->GetEndLoc().y;
	m_SI.heading = ffmod(radToDeg(ps->m_EndRotation), 360);
	if (m_SI.heading < 0)
		m_SI.heading += 360;

	m_SI.wind = ps->GetWind();								// wind is metric (km/hr)

	m_SI.flags = 0;

	if (!m_pFinishSection && !m_bLoopClosed)
		m_SI.flags |= SI_FINISH;

	if ((ps->GetStartDistance() >= 0.0f) &&
		 !ps->IsFinishSection() &&
		 (ps->m_Type != Section::CLOSELOOP))  {
		// Check for collision here.

		ps->SetEditMode((fabs(ps->m_Radius) < minRadius ? Section::PROBLEM : Section::EDIT));
		Section *pc = ps->Prev();


			if (pc)	{		// Don't check collsion with the previous one... It will technicly never collide.
			for (pc = pc->Prev(); pc; pc = pc->Prev())  {
				if (!ignoreColl && CheckCollision(ps, pc))  {						// tlm: this is changing ps->m_StartWind!!!
					ps->SetEditMode(Section::PROBLEM);
					break;
				}
			}
		}


		// ADJUST FLAGS

		float ratio = m_SI.deg / m_SI.length;

		m_SI.flags |= SI_GRADE | SI_WIND;

		if ((ps->m_Type == Section::CURVE) || (ps->m_Type == Section::STREIGHT))  {
			m_SI.flags |= SI_ROTATE;
			m_SI.maxrotation = radToDeg(m_SI.length / minRadius);
			if (m_SI.maxrotation > 270.0f)
				m_SI.maxrotation = 270.0f;
		}

		//else
		//	m_SI.maxrotation = 0.1f;

		if (ps->GetEditMode() == Section::EDIT)
			m_SI.flags |= SI_LONGER;

		if (m_SI.length > SECTION_MIN_LENGTH)
			m_SI.flags |= SI_SHORTER;

		m_SI.minlength = SECTION_MIN_LENGTH;
		m_SI.maxlength = (ratio != 0.0f ? SECTION_MAX_ROTATION / ratio : SECTION_MAX_LENGTH);
		if (m_SI.maxlength < 0)
			m_SI.maxlength = -m_SI.maxlength;
		if (m_SI.maxlength > SECTION_MAX_LENGTH)
			m_SI.maxlength = SECTION_MAX_LENGTH;
	}

	if (ps->GetStartDistance() >= 0.0f)
		m_SI.flags |= SI_DELETE;

	if ((ps->GetEditMode() != Section::PROBLEM) && (ps->m_Type != Section::CLOSELOOP))
		m_SI.flags |= SI_ADD;

	// Check for close loop here.

	if (EditCloseLoop(false))  {
		m_SI.flags |= SI_CLOSELOOP;
	}


	if (!gMetric)  {
		m_SI.minlength = METERS_TO_FEET(m_SI.minlength);
		m_SI.maxlength = METERS_TO_FEET(m_SI.maxlength);
		m_SI.length = METERS_TO_FEET(m_SI.length);
		m_SI.startdist = METERS_TO_FEET(m_SI.startdist);
		m_SI.elevation = METERS_TO_FEET(m_SI.elevation);

		//m_SI.startloc = METERS_TO_FEET(m_SI.startloc);

		m_SI.enddist = METERS_TO_FEET(m_SI.enddist);

		//m_SI.endloc = METERS_TO_FEET(m_SI.endloc);

		m_SI.wind = KM_TO_MILES(m_SI.wind);
	}

	if (!(m_SI.flags & (SI_WIND | SI_GRADE | SI_ROTATE | SI_LONGER | SI_SHORTER)))
		m_SI.length = 0;

	m_pSISection = ps;
//#endif
	return m_SI;
}


/************************************************************************************

************************************************************************************/

#ifndef IGNOREFORNOW
const SectionInfo & CCourse::EditDeg(float deg)  {

	Section *ps = m_Track.GetLast();
	if (ps != m_pSISection)
		updateSI();

	if (fabs(ps->GetDegRotation() - deg) < degToRad(0.1))
		return m_SI;

	if (fabs(deg) > m_SI.maxrotation)
		return m_SI;

	if (!(m_SI.flags & SI_ROTATE))
		return m_SI;

	ps->Set(deg, ps->GetLength());
	ClearProfile();
	calcGrid();
	return updateSI();
}

/************************************************************************************

************************************************************************************/

const SectionInfo & CCourse::EditLength(float len)  {
	if (!gMetric)
		len = FEET_TO_METERS(len);
	Section *ps = m_Track.GetLast();
	if (ps != m_pSISection)
		updateSI();

	if (fabs(ps->GetLength() - len) < 0.07f)
		return m_SI;

	if (((len > ps->GetLength()) && !(m_SI.flags && SI_LONGER)) ||
		 ((len < ps->GetLength()) && !(m_SI.flags && SI_SHORTER)))
		return m_SI;

	float ratio = ps->GetDegRotation() / ps->GetLength();
	ps->Set(ratio * len, len);
	calcGrid();
	//tlm20031223+++
	//ClearProfile();
	// WW: NC22 - Another fix so profiles still work.
	if (!m_bLoopClosed)  {
		for(;ps;ps = ps->Prev())  {
			if (ps == m_pFinish)  {
				break;
			}
		}
	}
	else  {
		ps = NULL;
	}

	if (!ps)  {		// WW: NC22  END ADDITION.
		ClearProfile();
	}

	//tlm20031223---

	return updateSI();
}

/************************************************************************************

************************************************************************************/

const SectionInfo & CCourse::EditGrade(float _grade_d_100)  {
	Section *ps = m_Track.GetLast();
	if (ps != m_pSISection)
		updateSI();

	if (!(m_SI.flags & SI_GRADE))
		return m_SI;

	if (fabs(ps->get_grade_d_100() - _grade_d_100) < 0.0005f)
		return m_SI;

	ps->set_grade_d_100(_grade_d_100);
	ClearProfile();
	return updateSI();
}


/************************************************************************************

************************************************************************************/

const SectionInfo & CCourse::EditWind(float wind)  {
	Section *ps = m_Track.GetLast();

	if (!gMetric)
		wind = MILES_TO_KM(wind);
	if (ps != m_pSISection)
		updateSI();

	//if (!(m_SI.flags & SI_WIND))
	//	return m_SI;

	if (fabs(ps->GetWind() - wind) < 0.05f)
		return m_SI;

	ps->SetWind(wind);
	ClearProfile();
	return updateSI();
}


/************************************************************************************

************************************************************************************/

bool CCourse::EditAdd(int dir)  {
	GetSectionInfo();
	if (!(m_SI.flags & SI_ADD))
		return false;

	if (dir < 0)
		dir = -1;
	else if (dir > 0)
		dir = 1;

	float len = (dir ? 25.0f : 50.0f);
	float rot = 22.5f;
	Section *ps = m_Track.GetLast();
	float grade = ps->get_grade_d_100();				// this is the ending percent grade / 100.0f
	float wind = ps->GetWind();
	for (;
		  ps && (ps->GetStartDistance() >= 0.0f);
		  ps = ps->Prev())  {
		if (((ps->m_DegRotation == 0.0f) && !dir) || ((ps->m_DegRotation != 0.0f) && dir))  {
			len = ps->GetLength();
			rot = ps->m_DegRotation;
			break;
		}
	}
	if (rot < 0.0f)
		rot = -rot;
	rot *= dir;
	if (rot != 0.0f)  {
		float r = fabs(len / degToRad(rot));
		if (r < minRadius)
			return false;
	}

	ps = new Section(*this, (dir ? Section::CURVE : Section::STREIGHT), len, grade, rot);
	ps->SetWind(wind);
	calcGrid();
	GetSectionInfo();
	ClearProfile();

	return true;
}

#endif
/************************************************************************************

************************************************************************************/

bool CCourse::EditDelete()  {
	bool recalc = false;
	GetSectionInfo();
	if (!(m_SI.flags & SI_DELETE))
		return false;
	Section *ps;
	if (m_bLoopClosed)  {
		// Unclose the loop and put the start sections back

		ps = m_Track.GetLast();
		ps->m_StartDistance = -4.0f;
		ps->m_Type = Section::START;
		m_Track.AddHead(*ps);
		ps = new Section(*this, Section::STREIGHT, 6.0f, 0.0f, 0.0f);
		ps->m_StartDistance = -10.0f;
		m_Track.AddHead(*ps);
		for (ps = m_Track.GetFirst();
			  ps;
			  ps = ps->Next())
			ps->calcValues();
		m_bLoopClosed = false;
		recalc = true;
	}
	ps = m_Track.GetLast();
	Section::Type type;
	do  {
		type = ps->m_Type;
		delete ps;
		ps = m_Track.GetLast();
	}
	while (((type == Section::FINISH) || (type == Section::CLOSELOOP) || (type == Section::EXTRA)) &&
			 ((ps->m_Type == Section::FINISH) ||
			  (ps->m_Type == Section::CLOSELOOP) ||
			  (ps->m_Type == Section::EXTRA)));

	if (recalc)  {
		for (ps = m_Track.GetFirst();
			  ps;
			  ps = ps->Next())
			ps->calcValues();
	}

	calcGrid();
	GetSectionInfo();

	ClearProfile();

	return true;
}


/************************************************************************************

************************************************************************************/

bool CCourse::undo(Section *ps)  {
	Section *pnext;
	if (ps)  {
		for (ps = ps->Next();
			  ps;
			  ps = pnext)  {
			pnext = ps->Next();
			delete ps;
		}
	}
	return false;
}


#ifndef IGNOREFORNOW
/************************************************************************************

************************************************************************************/

bool CCourse::EditNormalize()  {
	if (m_bLoopClosed)
		return false;

	Section *ps;
	Section *pnext;
	float grade = 0.0f;
	float lastgrade;
	for (ps = m_Track.GetFirst();
		  ps;
		  ps = pnext,lastgrade = grade)  {
		pnext = ps->Next();
		grade = ps->get_grade_d_100();
		if ((ps->m_Type != Section::STREIGHT) && (ps->m_Type != Section::CURVE))
			continue;
		if ((ps->GetLength() >= 100.0f) && (fabs(lastgrade - grade) > 0.005f))  {
			float ratio = ps->GetDegRotation() / ps->GetLength();
			Section *pnew = new Section(*this, Section::CURVE, 75.0f, grade, ratio * 75.0f);
			pnew->InsertBefore(*ps);
			ps->m_Length -= 75.0f;
			ps->m_DegRotation = ratio * ps->m_Length;
		}
	}
	for (ps = m_Track.GetFirst();
		  ps;
		  ps = ps->Next())
		ps->calcValues();
	calcGrid();
	updateSI();
	ClearProfile();

	return true;
}


/************************************************************************************

************************************************************************************/

bool CCourse::EditFinish()  {
	GetSectionInfo();
	if (!(m_SI.flags & SI_FINISH))
		return false;

	m_pFinishSection = new Section(*this, Section::FINISH, 4.0f, m_SI.grade, 0, TEX_FINISH);
	new Section(*this, Section::EXTRA, 100.0f, 0, 0);
	calcGrid();
	updateSI();
	ClearProfile();

	return true;
}

#endif
/************************************************************************************

************************************************************************************/

CCourse::Section * CCourse::splitSection(Section *ps, float splitlen)  {
	float deg = ps->GetDegRotation();
	float len = ps->GetLength();
	float ratio = splitlen / len;
	Section *pnew = new Section(*this, Section::CLOSELOOP, len *ratio, ps->get_grade_d_100(), deg *ratio);
	pnew->InsertBefore(*ps);
	ps->m_Length -= len * ratio;
	ps->m_DegRotation -= deg * ratio;
	return pnew;
}

static bool gradeOK;


/************************************************************************************

************************************************************************************/

float CCourse::doGrade(Section *pstart, Section *pend, float _grade_d_100)  {
	Section *ps = pstart;
	for (;
		  ps != pend;
		  ps = ps->Next())  {
		if (!ps->set_grade_d_100(_grade_d_100))  {
			gradeOK = false;
			return 0.0f;
		}
	}
	for (;
		  ps;
		  ps = ps->Next())  {
		if (!ps->set_grade_d_100(0))  {
			gradeOK = false;
			return 0.0f;
		}
	}
	ps = m_Track.GetLast();
	return ps->GetEndLoc().y;
}


/************************************************************************************

************************************************************************************/

bool CCourse::EditCloseLoop(bool create, bool force)  {
//#ifndef IGNOREFORNOW

	if (m_bLoopClosed)
		return false;

	if (create && !force)  {
		GetSectionInfo();
		if (!(m_SI.flags & SI_CLOSELOOP))
			return false;
	}
	Section *plast = m_Track.GetLast();
	Section *ps = plast;

	if ((m_SI.flags & SI_ADD) != SI_ADD)
		return false;

	// Must have a start line!

	Section *pstartsec = m_Track.GetFirst();
	for (;
		  pstartsec && (pstartsec->m_Type != Section::START);)
		pstartsec = pstartsec->Next();
	if (!pstartsec)
		return false;
	vector3df sv = plast->GetEndLoc();
	vector3df ev = pstartsec->GetStartLoc();
	float dz;
	float len;

	// Are we heading streight on?

	float deg = -NormalizeRotation(ps->GetEndRotation());
	if (fabs(deg) < 1.0f)  {
		dz = ev.z - sv.z;
		if ((dz < 5.0f) || (dz > 2000.0f))
			return undo(plast);

		if (fabs(sv.x) < 0.1f)
			ps = new Section(*this, Section::CLOSELOOP, dz, 0, radToDeg(deg));
		else  {
			float x = fabs(sv.x);
			float rm = dz;
			deg = acos((rm - x) / rm);

			if (sv.x > 0.0f)
				deg = -deg;
			len = (float) fabs(deg * rm / 2.0f);
			if (len > 2000.0f)
				return undo(plast);
			ps = new Section(*this, Section::CLOSELOOP, len, 0, radToDeg(deg));

			//ps = new Section( *this , Section::CLOSELOOP, len, 0, radToDeg(-deg) );
			//sv = ps->GetEndLoc();
			//dz = ev.z - sv.z;
			//if (dz > 2000.0f)
			//	return undo(plast);
			//if (dz >= 0.1f)
			//	ps = new Section( *this, Section::CLOSELOOP, dz, 0, 0 );
		}
		sv = ps->GetEndLoc();
		dz = ev.z - sv.z;
		deg = -NormalizeRotation(ps->GetEndRotation());
	}

	{
		Section *prevps = ps;
		dz = ev.z - sv.z;
		if ((dz < 1.0f) || (deg >= (PI * 0.95f)) || (deg < (-PI * 0.95f)))
			return undo(plast);
		float r = (float) (sv.x / (cos(deg) - 1));
		if (fabs(r) < minRadius)
			return undo(plast);		// Radius too steep to do anything here.
		len = fabs(r * deg);
		if (len > 2000.0f)
			return undo(plast);
		ps = new Section(*this, Section::CLOSELOOP, len, 0, radToDeg(deg));
		sv = ps->GetEndLoc();
		dz = ev.z - sv.z;
		if (dz < 0.1f)  {
			delete ps;
			ps = prevps;
			sv = ps->GetEndLoc();
			dz = ev.z - sv.z;

			// Too long... Try something different.

			float x = minRadius * (cos(deg) - 1);
			if (fabs(sv.x) < fabs(x))
				return undo(plast);
			len = fabs(tan(deg) * (sv.x - x));
			if (len > 2000.0f)
				return undo(plast);
			ps = new Section(*this, Section::CLOSELOOP, len, 0, 0);
			sv = ps->GetEndLoc();
			dz = ev.z - sv.z;
			r = (float) (sv.x / (cos(deg) - 1));
			if (fabs(r) < minRadius)
				return undo(plast);
			len = fabs(r * deg);
			if (len > 2000.0f)
				return undo(plast);
			ps = new Section(*this, Section::CLOSELOOP, len, 0, radToDeg(deg));
			sv = ps->GetEndLoc();
			dz = ev.z - sv.z;
		}
		if (dz > 2000.0f)
			return undo(plast);
		if (dz >= 0.1f)
			ps = new Section(*this, Section::CLOSELOOP, dz, 0, 0);
	}
	sv = ps->GetEndLoc();
	dz = ev.z - sv.z;
	if ((fabs(sv.x) > 0.1f) || (fabs(dz) > 0.1f))
		return undo(plast);

	// Need a 5 meter section at either end to do the grade stuff.

	Section *pstart = NULL;
	Section *pend = NULL;
	ps = plast->Next();

	//ps->calcValues();

	while (ps && (ps->GetLength() < 5.0f))
		ps = ps->Next();
	if (!ps)
		return undo(plast);
	if (ps->GetLength() < 10.0f)
		pstart = ps;
	else
		pstart = splitSection(ps, 5.0f);
	if (!pstart)
		return undo(plast);

	ps = m_Track.GetLast();
	while ((ps != pstart) && (ps->GetLength() < 5.0f))
		ps = ps->Prev();
	if (!ps)
		return undo(plast);
	if (ps->GetLength() < 10.0f)
		pend = ps;
	else  {
		pend = ps;
		ps = splitSection(pend, 5.0f);
	}
	if (!pend)
		return undo(plast);

	// Now check the grade.

	int maxtry = (create ? 100 : 1);
	float gradehigh = MAX_GRADE / 100.0f, gradelow = MIN_GRADE / 100.0f;
	for (ps = plast->Next();
		  ps != pstart;
		  ps = ps->Next())  {
		ps->calcValues();
	}

	gradeOK = true;
	float yh = doGrade(pstart, pend, gradehigh);
	float yl = doGrade(pstart, pend, gradelow);
	if (!gradeOK || (yh < -0.1f) || (yl > 0.1f))
		return undo(plast);

	for (ps = plast->Next();
		  ps;
		  ps = ps->Next())  {
		// Check for collision

		Section *pc = ps->Prev();
		if (pc)	// Don't check collsion with the previous one... It will technicly never collide.
		{
			for (pc = pc->Prev();
				  pc;
				  pc = pc->Prev())  {
				if ((pc == pstartsec) && !ps->Next())
					break;
				if (CheckCollision(ps, pc))
					return undo(plast);
				if (pc == pstartsec)
					break;
			}
		}
	}

	if (!create)
		undo(plast);
	else  {
		float gradehigh = MAX_GRADE / 100.0f, gradelow = MIN_GRADE / 100.0f;
		float grade = 0.0f;
		int cnt = 0;
		float y;
		do  {
			if (cnt++ > 100)
				undo(plast);
			y = doGrade(pstart, pend, grade);
			if (y < 0.0f)
				gradelow = grade;
			else
				gradehigh = grade;
			grade = (gradehigh + gradelow) / 2.0f;
		}
		while (fabs(y) > 0.001f);

		for (ps = m_Track.GetFirst();
			  ps != pstartsec;
			  ps = m_Track.GetFirst())
			delete ps;
		pstartsec->m_Type = Section::CLOSELOOP;
		m_Track.AddTail(*pstartsec);
		pstartsec->calcValues();

		m_bLoopClosed = true;
		calcGrid();
		updateSI();
		ClearProfile();

		float wind = plast->GetWind();
		float ewind = m_Track.GetFirst()->GetWind();

		float startdist = plast->GetStartDistance();
		float enddist = m_Track.GetLast()->GetEndDistance() - startdist;
		for (ps = plast->Next();
			  ps;
			  ps = ps->Next())  {
			float d = (ps->GetEndDistance() - startdist) / enddist;
			ps->SetWind((ewind - wind) * d + wind);
		}
		ps = m_Track.GetLast();
		ps->SetWind(m_Track.GetFirst()->GetWind());

		/*
							if (m_pFinishSection)
							{
								m_pFinishSection->m_SpecialTexture = 0;
								m_pFinishSection->m_Type = Section::CURVE;
								m_pFinishSection = NULL;
							}
							*/
	}
//#endif
	return true;
}

bool CCourse::FixCloseLoop(bool create, bool force)  {

	Section *pstartsec = m_Track.GetFirst();
	Section *ps = m_Track.GetLast();
	Section *pslast = ps->Prev(); 

	vector3df endloc = pslast->GetEndLoc();
	vector3df startloc = pstartsec->GetStartLoc();

	float ydiff = endloc.y - startloc.y;
	ps = pslast;
	if(ydiff < 0) 
	{
		// raise end up
		while(ydiff < 0)
		{
			vector3df s = ps->GetStartLoc();
			vector3df e = ps->GetEndLoc();
			float d = ps->GetLength();
			ydiff += d * SECTION_GRADE_MAX_D_100;
			ydiff += e.y - s.y;
			ps = ps->Prev();
		}
	}
	else
	if(ydiff > 0) 
	{
		// lower end up
		while(ydiff > 0)
		{
			vector3df s = ps->GetStartLoc();
			vector3df e = ps->GetEndLoc();
			float d = ps->GetLength();
			ydiff -= d * SECTION_GRADE_MAX_D_100;
			ydiff -= e.y - s.y;
			ps = ps->Prev();
		}
	}

	Section *plast = ps; 
	if(plast)
	{
		if(plast->Prev())
			plast = plast->Prev(); 
	}

	ps = m_Track.GetLast();
	do  {
		delete ps;
		ps = m_Track.GetLast();
	}
	while (ps != plast);

	bool recalc = true;
	if (recalc)  {
		for (ps = m_Track.GetFirst();
			  ps;
			  ps = ps->Next())
			ps->calcValues();
	}

	calcGrid();
	GetSectionInfo();
	ClearProfile();				   

	return true;
}



/************************************************************************************

************************************************************************************/

CCourse::Section *CCourse::RoadLocOld(
							 float dist, 
							 vector3df &loc, 
							 vector3df &vec, 
							 float &grade, 
							 int bikehint,
							 float *pangle, 
							 float *dangle, 
							 float *pwind,
							 Section *psec
							 )  {

	//Section *psec = 0L;
	//Section *csec=NULL;

	float len;

	Section *&csec = (bikehint >= BIKE_HINTS ? psec : m_pLocCache[bikehint]);
	//csec = (bikehint >= BIKE_HINTS ? psec : m_pLocCache[bikehint]);
	//if (bikehint >= BIKE_HINTS)  {
	//	csec = psec;
	//}
	//else  {
	//	csec = m_pLocCache[bikehint];
	//}


	len = m_Track.GetLast()->GetEndDistance();

	if (m_bLoopClosed)  {
		if (dist < 0.0f)  {
			dist += len;
		}
		else  {
			dist = (float) fmod((f64)dist, (f64)len);
		}
	}
	else if (dist > len - 10.0f)  {
		dist = len - 10.0f;
	}

	if (!csec)  {
		csec = m_Track.GetFirst();
	}

	if (!csec)  {
		grade = 0.0f;
		loc = vector3df(0.0f, 0.0f, 0.0f);
		vec = vector3df(1.0f, 0.0f, 0.0f);
	}

	while (csec)  {

		if (csec->RoadLoc(&csec, dist, loc, vec, grade, pangle, pwind))	{	// Keeps modifying CSec until we get to the proper track
			if (!csec)  {
				break;
			}
			if (dangle)  {
				*dangle = csec->GetSectionRotation();
			}
			break;
		}
		else  {
		}
	}
	return csec;
}			// RoadLoc()

CCourse::Section *CCourse::RoadLoc(
							 float fulldist, 
							 vector3df &loc, 
							 vector3df &vec, 
							 float &grade, 
							 int bikehint,
							 float *pangle, 
							 float *dangle, 
							 float *pwind,
							 Section *psec,
							 float *endoftrack
							 )  {

//	f32 coursedist = GetRaceLength();
	f32 bikeLength = 4.0f;
	f32 rlen = GetCourseLength();
	float len;
	f32 dist = fulldist;

	Section *&csec = (bikehint >= BIKE_HINTS ? psec : m_pLocCache[bikehint]);

	if(m_EndAt > 0)
	{
		f32 lapdist = m_EndAt - m_StartAt;
		f32 courselen = laps * lapdist;

		int curlap = (int)(dist / lapdist);
		if(dist <= courselen)
		{
			if(dist >= lapdist)
				dist = (f32)fmod((f64)dist, (f64)lapdist);
			if(curlap > 0 && dist < bikeLength)
			{
				dist += lapdist;
			}
		}
		else
		{
			dist = (dist - courselen) + lapdist;
		}
		dist += m_StartAt;
	}


	// Here, dist is now actual dist from physical course start
	bool bReverse = m_bReverse;
	float rdist = rlen - dist; 
	// Here, rdist is now actual dist from physical course end

	if(bReverse)
	{
		len = m_Track.GetFirst()->GetStartDistance();
		if (m_bLoopClosed)  {
			if (rdist < 0.0f)  {
				rdist += rlen;
			}
			else if (rdist >= rlen) {
				rdist = (float) fmod((f64)rdist, (f64)rlen);
			}
		}
		else if (rdist < len + bikeLength)  {
			rdist = len + bikeLength;
		}
		dist = rdist;
	}
	else
	{
		len = m_Track.GetLast()->GetEndDistance();
		if (m_bLoopClosed)  {
			if (dist < 0.0f)  {
				dist += len;
			}
			else if (dist >= len) {
				dist = (float) fmod((f64)dist, (f64)len);
			}
		}
		else if (dist > len - bikeLength)  {
			dist = len - bikeLength;
		}
	}


	if (!csec)  {
		csec = m_Track.GetFirst();
	}

	if (!csec)  {
		grade = 0.0f;
		loc = vector3df(0.0f, 0.0f, 0.0f);
		vec = vector3df(1.0f, 0.0f, 0.0f);
	}

	while (csec)  {

		if (csec->RoadLoc(&csec, dist, loc, vec, grade, pangle, pwind))	{	// Keeps modifying CSec until we get to the proper track
			if (!csec)  {
				break;
			}
			if (dangle)  {
				*dangle = csec->GetSectionRotation();
			}
			break;
		}
		else  {
		}
	}
	if(bReverse)
	{
		vec = -vec;
		grade = -grade;
		*pangle = (*pangle)+PI;
	}
	return csec;
}			// RoadLoc()

/************************************************************************************

************************************************************************************/

float CCourse::AdjustRiderDistance(float dist)  {
	if (m_bLoopClosed)  {
		return dist;
	}

	float len = m_Track.GetLast()->GetEndDistance();
	if (dist > len - 10.0f)  {
		dist = len - 10.0f;
	}
	return dist;
}


/************************************************************************************

************************************************************************************/

float CCourse::Rotation(float dist)  {
	Section *csec = m_Track.GetFirst();
	while (csec)  {
		if (dist < csec->m_StartDistance)
			csec = csec->Prev();
		else if (dist >= csec->m_StartDistance + csec->m_Length)
			csec = csec->Next();
		else  {
			float dlen = dist - csec->m_StartDistance;
			float percent = dlen / csec->m_Length;
			float r = (csec->m_Rotation !=
						  0.0f ?
						  csec->m_StartRotation +
						  csec->m_Rotation *percent :
						  csec->m_StartRotation);

			return radToDeg(r);
		}
	}
	return 0.0f;
}	


/************************************************************************************

************************************************************************************/

float CCourse::GetCourseLength() const  {
	if (m_pFinish)  {
		return m_pFinish->GetStartDistance();
	}
	Section *ps = m_Track.GetLast();
	if (ps)  {
		return ps->GetEndDistance();
	}
	return 0.0f;
}

float CCourse::GetEndOfTrack() const  {
	if (m_bLoopClosed)
		return FLT_MAX - 10.0f;
	Section *ps = m_Track.GetLast();
	if (ps)  {
		return ps->GetEndDistance();
	}
	return 0.0f;
}


/************************************************************************************

************************************************************************************/

float CCourse::GetRaceLength() const  {
	return laps * GetCourseLength();
}


/************************************************************************************

************************************************************************************/

int CCourse::GetLapNum(float len) const  {
	int laps = (int) floor(len / GetCourseLength()) + 1;
	return (laps < 1 ? 1 : laps);
}


/************************************************************************************

************************************************************************************/

float CCourse::GetLapPos(float dist) const  {
	float len = GetCourseLength();
	dist = fmod((f64)dist, (f64)len);
	return dist / len;
}

/************************************************************************************

************************************************************************************/

void CCourse::ClearAllSigns()  {
	Section *ps;
	for (ps = m_Track.GetFirst();
		  ps;
		  ps = ps->Next())  {
		ps->ClearAllSigns();
	}
}


/************************************************************************************

************************************************************************************/

bool CCourse::TurnOffABSSectionEnds()  {
	if (m_bLoopClosed)
		return false;

	Section *ps;
	for (ps = m_Track.GetFirst();
		  ps;
		  ps = ps->Next())  {
		ps->m_Flags &= ~SECTION_F_ABS_STARTLOC;
	}
	return true;
}


/************************************************************************************

************************************************************************************/

CCourse::Section * CCourse::GetPrevSection(Section *ps)	// Takes into account looping tracks.
{
	Section *pt = ps->Prev();
	return (pt ? pt : (m_bLoopClosed ? m_Track.GetLast() : NULL));
}


/************************************************************************************

************************************************************************************/

CCourse::Section::Section(CCourse &course, Type type, float len, float grade, float rot, int special, float startdist) :
m_Course(course) {
//m_Course(course), scene::ISceneNode(D3DBase::GetSceneManager()->getRootSceneNode(), D3DBase::GetSceneManager())  {

	//drop();
	n_sections++;
	m_YArr = NULL;
	m_GArr = NULL;
	init(type, len, grade, rot, special, startdist);

}


/************************************************************************************

************************************************************************************/

CCourse::Section::Section(CCourse &course, SecFile &sf) : 
m_Course(course) {
//m_Course(course), scene::ISceneNode(D3DBase::GetSceneManager()->getRootSceneNode(), D3DBase::GetSceneManager())  {

	//drop();
	n_sections++;

	m_YArr = NULL;
	m_GArr = NULL;
	m_FrameNum = -1;

	sf.Push();
	int sectype = 0;
	if (strcmp(sf.GetSectionName(), "sec2") == 0)
		sectype = 1;
	Type type = (Type) sf.ReadLong();
	float len = sf.ReadFloat();
	float grade = sf.ReadFloat();
	float rot = sf.ReadFloat();
	int special = sf.ReadLong();
	float startdist = sf.ReadFloat();

//long offs = ftell(sf.m_File);

	if ((special == 0) && (type == FINISH))
		type = CURVE;

	init(type, len, grade, rot, special, startdist);

	if (sectype == 1)  {
		m_Flags = sf.ReadLong();
		if (m_Flags & SECTION_F_ABS_STARTLOC)  {
			m_StartLoc.x = sf.ReadFloat();
			m_StartLoc.y = sf.ReadFloat();
			m_StartLoc.z = sf.ReadFloat();
		}
		if (m_Flags & SECTION_F_ABS_Y)  {
			m_Divisions = sf.ReadLong();
			initYArr();
			int i;
			for (i = 0; i < m_Divisions; i++)  {
				m_YArr[i] = sf.ReadFloat();
				m_GArr[i] = sf.ReadFloat();
			}
		}
		m_ActualLength = sf.ReadFloat();
		calcValues();

	}
	sf.Pop();
}

/************************************************************************************

************************************************************************************/

CCourse::Section::~Section()  {

	n_sections--;

	ClearAllSigns();
	if (m_Type == CLOSELOOP)
		m_Course.m_bLoopClosed = false;
	if (m_Course.m_pFinishSection == this)
		m_Course.m_pFinishSection = NULL;
	if (m_Course.m_pStartSection == this)
		m_Course.m_pStartSection = NULL;
	if (m_Course.m_pEdit == this)
		m_Course.m_pEdit = NULL;
	if (m_Course.m_pSISection == this)
		m_Course.m_pSISection = NULL;
	if (m_Course.m_pFinish == this)
		m_Course.m_pFinish = NULL;

	int i;

	// Remove it from the bike cache.

	for (i = 0; i < BIKE_HINTS; i++)  {
		if (m_Course.m_pLocCache[i] == this)  {
			m_Course.m_pLocCache[i] = 0L;
		}
	}

	#ifdef BIKE_APP
		CloseModel();
	#endif

	closeCArr();
	if (m_YArr)  {
		delete[] m_YArr;
		m_YArr = NULL;
	}

	if (m_GArr)  {
		delete[] m_GArr;
		m_GArr = NULL;
	}
	//drop();
}


/************************************************************************************
		grade is %grade/100
************************************************************************************/

void CCourse::Section::init(Type type, float len, float grade, float rot, int special,
									 float startdist)  {
	if ((rot > -0.0001f) && (rot < 0.0001f))
		rot = 0.0f;

	tree_group_index = 0;
	m_Flags = 0;
	m_ActualLength = 0.0f;

	//tlm+++

	m_Divisions = 0;
	m_NonDraw = 0;

	//tlm---

	//if ((rot == 0.0f) && (type == CURVE))
	//	type = STREIGHT;
	//else if ((rot != 0.0f) && (type == STREIGHT))
	//	type = CURVE;

	m_bLeftEdge = false;
	m_bRightEdge = false;

	m_EndStripValid = false;

	m_StartDistance = startdist;
	m_Type = type;
	m_Length = len;
	grade_d_100 = grade;
	m_DegRotation = rot;
	SetData(this);
	m_Course.m_Track.AddTail(*this);

	m_SpecialTexture = special;

	m_Wind = 0.0f;
	m_StartWind = 0.0f;

	m_YArr = 0L;
	m_GArr = 0L;

#ifndef IGNOREFORNOW
	m_pVBuf = 0L;
	m_pIBuf = 0L;
	m_pVBufEdit = NULL;
#else
//	m_pMesh1 = NULL;
//	m_pMesh2 = NULL;
	bMeshCreated = false;
#endif

	m_pRenderIns = 0L;

	m_TreeCount = 0;
	m_pTreeArr = NULL;

	m_pSectionNode = 0;


	m_pCArr = NULL;

	calcValues();

	m_EditMode = PROBLEM;
	SetEditMode(NORMAL);

	if (IsFinishSection())  {
		m_Course.m_pFinish = this;
	}
}


/************************************************************************************

************************************************************************************/

bool CCourse::Section::Save(SecFileWrite &sf)  {


	if (m_Flags & (SECTION_F_ABS_STARTLOC | SECTION_F_ABS_Y))  {
		sf.Push("sec2");
		sf.Write(m_Type);
		sf.WriteFloat(m_Length);
		sf.WriteFloat(grade_d_100);
		sf.WriteFloat(m_DegRotation);
		sf.Write(m_SpecialTexture);
		sf.WriteFloat((m_StartDistance >= 0.0f ? 0.0f : m_StartDistance));

		// Extended stuff.

		sf.Write(m_Flags);
		if (m_Flags & SECTION_F_ABS_STARTLOC)  {
			sf.WriteFloat(m_StartLoc.x);
			sf.WriteFloat(m_StartLoc.y);
			sf.WriteFloat(m_StartLoc.z);
		}
		if (m_Flags & SECTION_F_ABS_Y)  {
			sf.Write(m_Divisions);
			int i;
			for (i = 0;
				  i < m_Divisions;
				  i++)  {
				sf.WriteFloat(m_YArr[i]);
				sf.WriteFloat(m_GArr[i]);
			}
		}
		sf.WriteFloat(m_ActualLength);
		sf.Pop();
	}
	else  {
		sf.Push("sec");
		sf.Write(m_Type);
		sf.WriteFloat(m_Length);
		sf.WriteFloat(grade_d_100);
		sf.WriteFloat(m_DegRotation);
		sf.Write(m_SpecialTexture);
		sf.WriteFloat((m_StartDistance >= 0.0f ? 0.0f : m_StartDistance));
		sf.Pop();
	}
	return sf.IsOK();
}


/************************************************************************************

************************************************************************************/

void CCourse::Section::extAt(float dist)  {
	vector3df loc, vec;
	float grd;
	float w = gMaxStripSize;
	if (dist < 0)
		dist = 0;
	RoadLoc(NULL, m_StartDistance + dist, loc, vec, grd);
	vec *= w;
	vec.y = gMaxYExtent;
	extRect3(m_ExtMin, m_ExtMax, loc + vec);
	extRect3(m_ExtMin, m_ExtMax, loc - vec);
}


/************************************************************************************

************************************************************************************/

void CCourse::Section::initYArr()  {


	DELARR(m_YArr);
	m_YArr = new float[m_Divisions];

	DELARR(m_GArr);
	m_GArr = new float[m_Divisions];

	return;
}

/************************************************************************************

************************************************************************************/

float CCourse::Section::CalcEndHeight(float m)  {
	float d = m_Divisions - 1 ;
	float s = m_StartLoc.y;
	float l = m_Length;
	float g = start_grade_d_100;

	float ey = s + (l / d) * (d *g + (d * (d - 1) * 0.5f * ((m - g) / (d + 1))));

	//float ty = s + l * g + (l * d * m - l * d * g - l * m + l * g)/(2 * (d+1));

	float e = s + l *g + (l *m * (d - 1) + l *g * (1 - d)) / (2 * (d + 1));

	float ls = ((e - s - l *g) * 2 * (d + 1) - l *g * (1 - d)) / (l * (d - 1));
	float rs = m;

	//float ty =  m_StartLoc.y + m_Length * start_grade_d_100 + ((endgrade * m_Length * (d * d - d))/ (2.0f * (d * d + d)));

	return ey;
}

/************************************************************************************

************************************************************************************/

float CCourse::Section::CalcEndGrade(float ey)  {
	float gradehigh = 500.0f / 100.0f, gradelow = -500.0f / 100.0f;
	float grade = 0.0f;
	int cnt = 0;
	float y;
	do  {
		if (cnt++ > 100)
			break;
		y = CalcEndHeight(grade) - ey;
		if (y < 0.0f)
			gradelow = grade;
		else
			gradehigh = grade;
		grade = (gradehigh + gradelow) / 2.0f;
	}
	while (fabs(y) > 0.0001f);

	return grade;

	/*
		float d = m_Divisions - 1 ;
		if (d <= 1.0f)
				return (ey - m_StartLoc.y)/m_Length;
		float e = ey;
		float s = m_StartLoc.y;
		float l = m_Length;
		float g = start_grade_d_100;
		return  (float)(((e - s - l * g) * 2 * (d + 1) - l * g * (1 - d)) / (l * (d - 1)));
		*/
}


/************************************************************************************

************************************************************************************/

void CCourse::Section::calcValues()  {
	int i;

#ifdef BIKE_APP
	CloseModel();
#endif

	closeCArr();

	int orgdiv = m_Divisions;

	setStrips(m_bLeftEdge, m_bRightEdge);					// repoints stripArr to different offsets in stripArrBase

	m_Valid = true;

	if ((m_DegRotation < 0.1) && (m_DegRotation > -0.1))  {
		m_DegRotation = m_Rotation = 0.0f;
	}
	else  {
		m_Rotation = degToRad(m_DegRotation);
	}

	if (m_Rotation != 0.0f)  {
		if (m_Type == STREIGHT)
			m_Type = CURVE;
	}

	Section *ps = Prev();
	if (ps)  {
		m_StartDistance = ps->GetEndDistance();
		start_grade_d_100 = (ps ? ps->get_grade_d_100() : 0.0f);
	}
	else  {
		start_grade_d_100 = 0;
	}
	Section *prev = m_Course.GetPrevSection(this);
	//m_StartWind = (prev ? prev->GetWind() : 0.0f);
	if (prev)  {
		m_StartWind = prev->GetWind();				// tlm20040720
	}
	else  {
		m_StartWind = 0.0f;
	}

	// Figure the Rotation divisions. At 25 meters this is 1 division per 2 degrees.

	if (!(m_Flags & SECTION_F_ABS_Y))  {
		//float dv = (m_Length / 25.0) / 4.0f;

		int d = (int) fabs(m_DegRotation * (12.0f / 25.0f));

		// Figure the length divisions

		int ld = (int) (m_Length / 10.0);

		// Figure the grade divisions

		float gdif = grade_d_100 - start_grade_d_100;
		int g = 1 + (int) (fabs((gdif * 100.0f) / 2.0f));
		if (g > (int) m_Length)
			g = (int) m_Length;

		// Use the larger of the two divisions.

		m_Divisions = d;
		if (ld > m_Divisions)
			m_Divisions = ld;
		if (g > m_Divisions)
			m_Divisions = g;

		if (m_Divisions < 2)
			m_Divisions = 2;

		if (m_Divisions & 1)
			m_Divisions += 1;

		if (m_SpecialTexture != TEX_ROAD)	// Special piece of road.
			m_Divisions = 2;
	}

	//if ((start_grade_d_100 != m_Grade) && (m_Divisions <= 2))
	//	m_Divisions = 3;

	m_DivisionLength = m_Length / (m_Divisions - 1);

	// Calc the Edit model divisions
	  {
		m_RoadPoints = 2;
		int d = (int) fabs(m_DegRotation * (6.0f / 25.0f));
		if (d > m_RoadPoints)
			m_RoadPoints = d;
		float gdif = grade_d_100 - start_grade_d_100;
		d = 1 + (int) (fabs((gdif * 100.0f) / 4.0f));
		if (d > m_RoadPoints)
			m_RoadPoints = d;

		m_TopPoints = 2;
		d = (int) fabs(m_DegRotation * (4.0f / 25.0f));
		if (m_DegRotation && (d < 3))
			d = 3;
		if (!(d & 1))
			d++;
		if (d > m_TopPoints)
			m_TopPoints = d;

		m_TopPoints *= 2;	// 1 for each side.
		m_RoadPoints *= 2;
	}

	if (!(m_Flags & SECTION_F_ABS_Y) || !m_YArr || !m_GArr || (orgdiv != m_Divisions))  {
		m_Flags &= ~SECTION_F_ABS_Y;	// MAKE SURE THIS IS TURNED OFF - incase m_YArr or m_GArr are not set up.
		initYArr();
	}

	if (!(m_Flags & SECTION_F_ABS_STARTLOC))
		m_StartLoc = (ps ? ps->GetEndLoc() : vector3df(0.0f, 0.0f, 0.0f));
	m_StartRotation = (ps ? ps->GetEndRotation() : 0.0f);
	m_EndRotation = m_StartRotation + m_Rotation;

	if (m_Rotation != 0.0f)  {
		m_Radius = -(float) (m_Length / m_Rotation);

		// TODO: Do a check here and make sure the radius is larger than a minimum radius.

		m_Center.x = m_StartLoc.x - m_Radius * cos(m_StartRotation);
		m_Center.y = 0.0f;
		m_Center.z = m_StartLoc.z + m_Radius * sin(m_StartRotation);

		vector3df t;
		t.x = m_Center.x + m_Radius * cos(m_StartRotation);
		t.z = m_Center.z - m_Radius * sin(m_StartRotation);
		m_EndLoc.x = m_Center.x + m_Radius * cos(m_EndRotation);
		m_EndLoc.z = m_Center.z - m_Radius * sin(m_EndRotation);

		m_RotationStep = m_Rotation / (m_Divisions - 1);
	}
	else  {
		m_Radius = 10000.0f;
		m_EndLoc.x = m_Length * sin(m_EndRotation) + m_StartLoc.x;
		m_EndLoc.z = m_Length * cos(m_EndRotation) + m_StartLoc.z;
		m_RotationStep = 0.0f;
	}

	//

	if (!(m_Flags & SECTION_F_ABS_Y))  {
		// Calculating the z needs to be done iteritively due to the grade change

		float grade = start_grade_d_100;
		float delta = (grade_d_100 - start_grade_d_100) / m_Divisions;
		float dlen = m_Length / (m_Divisions - 1);
		float y = m_StartLoc.y;
		m_LowY = m_HighY = y;

		int d = m_Divisions - 1 ;

		//float endy = CalcEndHeight( m_Grade);

		for (i=0; i<m_Divisions; i++)  {			// There is probably some formula to do this but I'm too lazy to figure it out.
			m_YArr[i] = y;
			m_GArr[i] = grade;
			if (y < m_LowY)
				m_LowY = y;
			else if (y > m_HighY)
				m_HighY = y;
			y += grade * dlen;	// Grade * the division length.
			grade += delta;
		}
	}
	else  {
		m_LowY = m_HighY = m_StartLoc.y;
		for (i=1; i<m_Divisions; i++)  {
			float y = m_YArr[i];
			if (y < m_LowY)
				m_LowY = y;
			else if (y > m_HighY)
				m_HighY = y;
		}
	}

	m_EndLoc.y = m_YArr[m_Divisions - 1];
	m_StartTV = m_StartDistance / ROAD_REPEAT;
	m_StartTV = m_StartTV - ((int) m_StartTV);

	// Calculate the extents.

	m_ExtMin = m_ExtMax = m_StartLoc;
	extRect3(m_ExtMin, m_ExtMax, m_EndLoc);

	//extAt( 0 );
	//extAt( m_Length );
	// Does the rotation pass through any of the 90 degrees.

	float srot = (f32)fmod((f64)m_StartRotation, (f64)PI);
	if (m_Rotation > 0.0f)  {
		if ((srot < PI) && (srot + m_Rotation > PI))
			extAt(((PI - srot) / m_Rotation) * m_Length);
		if ((srot < PI / 2.0f) && (srot + m_Rotation > PI / 2.0f))
			extAt(((PI / 2.0F - srot) / m_Rotation) * m_Length);
		if ((srot < 0) && (srot + m_Rotation > 0))
			extAt((-srot / m_Rotation) * m_Length);
		if ((srot < -PI / 2.0f) && (srot + m_Rotation > -PI / 2.0f))
			extAt((-(srot + PI / 2.0F) / m_Rotation) * m_Length);
	}
	else if (m_Rotation < 0.0f)  {
		if (fabs(m_Rotation + PI) < 0.01f)
			m_Rotation = m_Rotation;
		if ((srot > -PI) && (srot + m_Rotation < -PI))
			extAt(((-PI - srot) / m_Rotation) * m_Length);
		if ((srot > -PI / 2.0f) && (srot + m_Rotation < -PI / 2.0f))
			extAt(((-PI / 2.0F - srot) / m_Rotation) * m_Length);
		if ((srot > 0) && (srot + m_Rotation < 0))
			extAt((srot / m_Rotation) * m_Length);
		if ((srot > PI / 2.0f) && (srot + m_Rotation < PI / 2.0f))
			extAt(((PI / 2.0F - srot) / m_Rotation) * m_Length);
	}

	m_ExtOrigin = (m_ExtMin + m_ExtMax) / 2;
	vector3df v = m_ExtMax - m_ExtOrigin;
	m_ExtRadius = v.getLength();

	// Trees

	if (m_DivisionLength >= treesMinDivisionLength)  {

		//tlm20051117+++
		m_TreeCount = (int) frand(treeMin * (m_Length / 100.0f), treeMax * (m_Length / 100.0f));		// tlm: changed treeMin to treeMax
		//m_TreeCount = (int) frand(treeMin, treeMax);
		//tlm20051117---

		if (m_TreeCount <= 0)  {
			m_TreeCount = (int) frand(0.0f, 4.0f);
		}

		if (m_TreeCount > treesMaxPerSection)  {				// allows max 50 trees per section
			m_TreeCount = treesMaxPerSection;
		}

#ifndef NEWTREECOUNT
		if (m_TreeCount > m_Divisions - 1)  {
			m_TreeCount = m_Divisions - 1;					// this is the real limiter
		}
#endif

	}

	// TODO: Calculate if it hit any of the other sections here.
}


#define ALLOC_CV(idx, pv, cnt)	{ \
	idx = pcv - cv;\
	pv = pcv;\
	pcv += cnt;\
}


/************************************************************************************

************************************************************************************/

void CCourse::Section::cmd_addStrip(short *&pcmd, int sv, int ev, int sidx, int eidx, WORD *idx)  {
	short i;
	*pcmd++ = (short) RINS_STRIP;
	*pcmd++ = (short) sv;
	*pcmd++ = (short) (ev - sv);
	*pcmd++ = (short) sidx;
	*pcmd++ = (i = (short) (eidx - sidx));
	while (i-- > 0)  {
		*idx++ = (WORD) (*idx - sv);
	}
}


/************************************************************************************

************************************************************************************/

void CCourse::Section::cmd_addTri(short *&pcmd, int sv, int ev, int sidx, int eidx, WORD *idx)  {
	short i;
	*pcmd++ = (short) RINS_TRI;
	*pcmd++ = (short) sv;
	*pcmd++ = (short) (ev - sv);
	*pcmd++ = (short) sidx;
	*pcmd++ = (i = (short) (eidx - sidx));
	while (i-- > 0)  {
		*idx++ = (WORD) (*idx - sv);
	}
}


/************************************************************************************

************************************************************************************/

void CCourse::Section::cmd_addTex(short *&pcmd, int tex)  {
	*pcmd++ = (short) RINS_TEX;
	*pcmd++ = (short) tex;
}



#define CVNUM(idx,addindex)		((idx)+m_Divisions*(addindex))


/************************************************************************************

************************************************************************************/

void CCourse::Section::bez(int addindex, StripData &sd, COURSEVERTEX *cv)  {
	int index = 1;	 // Don't do the first index.  Already has been done.
	int stopcount = m_Divisions;

	while (1)  {
		while (sd.n < sd.count)  {
			sd.addy += sd.ydelta * sd.updown;
			sd.ydelta *= BEZ_BOTTOM_DELTA_SCALE;

			cv[CVNUM(index, addindex)].v.y += sd.addy;//	ptstrip[index*11+addindex].vertex.y += addY;
			if (sd.updown > 0)  {
				cv[CVNUM(index, addindex)].color += sd.ydelta * 2; // ptstrip[index*11+addindex].color+=yDelta*2;
			}
			if (addindex == 2)  {
				cv[CVNUM(index, 3)].v.y += sd.addy / 8.0f; // ptstrip[index*11+3].vertex.y += addY/8;

				//		if(addY>=0)
				//		{

				cv[CVNUM(index, addindex)].color += sd.addy / 8; // ptstrip[index*11+addindex].color+=addY/8;
				cv[CVNUM(index, addindex)].color += sd.addy / 10; // ptstrip[index*11+1+addindex].color+=addY/10;

				//		}
			}
			if (addindex == 7) // addindex 8
			{
				cv[CVNUM(index, 7)].color += (0.2f - sd.addy / 8); // ptstrip[index*11+8].color+=(.2-addY/8);
				cv[CVNUM(index, 6)].color += (0.2f - sd.addy / 10); // ptstrip[index*11+7].color+=(.2-addY/10);
				cv[CVNUM(index, 6)].v.y += sd.addy / 8; // ptstrip[index*11+7].vertex.y += addY/8;
				if (cv[CVNUM(index, 7)].color < 0.5f) // if(ptstrip[index*11+8].color<.5)// Keep bightness above .5
				{
					cv[CVNUM(index, 7)].color = 0.5f; // ptstrip[index*11+8].color = .5;
				}
				cv[CVNUM(index, 5)].color += cv[CVNUM(index, 7)].color; // ptstrip[index*11+6].color+=ptstrip[index*11+8].color;
			}

			index++;
			sd.n++;
			if ((index >= stopcount) || (sd.ydelta > 1.0f))
				break;
		}
		sd.count = 0;	// So we will pass through that on the next section if it exits out before then.
		while (sd.n > 0)  {
			if (index >= stopcount)
				break;
			sd.addy += sd.ydelta * sd.updown;
			sd.ydelta *= BEZ_TOP_DELTA_SCALE;

			cv[CVNUM(index, addindex)].v.y += sd.addy;//	ptstrip[index*11+addindex].vertex.y += addY;
			if (sd.updown > 0)  {
				cv[CVNUM(index, addindex)].color += sd.ydelta * 2; // ptstrip[index*11+addindex].color+=yDelta*2;
			}
			if (addindex == 2)  {
				cv[CVNUM(index, 3)].v.y += sd.addy / 8.0f; // ptstrip[index*11+3].vertex.y += addY/8;

				//		if(addY>=0)
				//		{

				cv[CVNUM(index, addindex)].color += sd.addy / 8; // ptstrip[index*11+addindex].color+=addY/8;
				cv[CVNUM(index, addindex)].color += sd.addy / 10; // ptstrip[index*11+1+addindex].color+=addY/10;

				//		}
			}
			if (addindex == 7) // addindex 8
			{
				cv[CVNUM(index, 7)].color += (0.2f - sd.addy / 8); // ptstrip[index*11+8].color+=(.2-addY/8);
				cv[CVNUM(index, 6)].color += (0.2f - sd.addy / 10); // ptstrip[index*11+7].color+=(.2-addY/10);
				cv[CVNUM(index, 6)].v.y += sd.addy / 8; // ptstrip[index*11+7].vertex.y += addY/8;
				if (cv[CVNUM(index, 7)].color < 0.5f) // if(ptstrip[index*11+8].color<.5)// Keep bightness above .5
				{
					cv[CVNUM(index, 7)].color = 0.5f; // ptstrip[index*11+8].color = .5;
				}
				cv[CVNUM(index, 5)].color += cv[CVNUM(index, 7)].color; // ptstrip[index*11+6].color+=ptstrip[index*11+8].color;
			}

			index++;
			sd.n--;
		}
		if (index >= stopcount)
			break;
		sd.updown = -sd.updown;
		sd.count = 2 + (5 * ((float) rand() / (float) (RAND_MAX + 1)));
		if (sd.addy < BEZ_ADDY_MIN) // < -1.0
			sd.updown = 1;
		if (sd.addy > 2)	// > 4
			sd.updown = -1;
		sd.ydelta = BEZ_INITIAL_YDELTA;
		sd.n = 0;
	}
}


/************************************************************************************

************************************************************************************/

bool CCourse::Section::RoadLoc(
									Section **ppsec, 
									float dist, 
									vector3df &loc, 
									vector3df &vec,
									float &grade, 
									float *pangle, 
									float *pwind)  {

	//------------------------------------
	// Is the distance in this section?
	//------------------------------------

	if (ppsec)  {
		if (dist < m_StartDistance)  {
			if ((*ppsec = Prev()) != 0L)  {
				return false;
			}
			dist = m_StartDistance;
		}
		else if (dist >= m_StartDistance + m_Length)  {
			if ((*ppsec = Next()) != 0L)  {
				return false;
			}
			dist = m_StartDistance + m_Length - 0.01f;
		}
	}

	float dlen = dist - m_StartDistance;
	float percent = (m_Length < 0.0001f ? 0.0f : dlen / m_Length);

	float r = (m_Rotation != 0.0f ? m_StartRotation + m_Rotation *percent : m_StartRotation);

	if (pangle)  {
		*pangle = r;
	}

	//--------------------------------------
	// We can figure out the vector here
	//--------------------------------------

	vec.x = cos(r);
	vec.y = 0.0f;
	vec.z = -sin(r);

	int div = (int) (dlen / m_DivisionLength);

	if (div < m_Divisions - 1)  {
		float divpercent = (m_DivisionLength < 0.0001f ?  0.0f : (dlen - (div *m_DivisionLength)) / m_DivisionLength);
		loc.y = (m_YArr[div + 1] - m_YArr[div]) * divpercent + m_YArr[div];
		grade = (m_GArr[div + 1] - m_GArr[div]) * divpercent + m_GArr[div];
	}
	else  {
		grade = m_GArr[div];
		loc.y = m_YArr[div] + grade * (dlen - m_Length);
	}

	if (m_Rotation != 0.0f)  {
		loc.x = m_Center.x + m_Radius * vec.x;
		loc.z = m_Center.z + m_Radius * vec.z;
	}
	else  {
		// Streight track
		loc.x = -dlen * vec.z + m_StartLoc.x;
		loc.z = dlen * vec.x + m_StartLoc.z;
	}

	Section *pnext = Next();

	if (pnext && (pnext->m_Flags & SECTION_F_ABS_STARTLOC))  {
		loc += (pnext->m_StartLoc - m_EndLoc) * percent;
	}

	if (pwind)  {
		*pwind = m_StartWind; //  + percent * (m_Wind - m_StartWind);
	}
	return true;
}


/************************************************************************************

************************************************************************************/

void CCourse::Section::closeCArr()  {
	if (m_pCArr)  {
		delete[] m_pCArr;
		m_pCArr = NULL;
	}
}


/************************************************************************************

************************************************************************************/

void CCourse::Section::calcCArr()  {
	// Build the collision edges.

	if (m_pCArr)
		return;
	closeCArr();
	vector3df loc, vec;
	float dist, grade;

	int d = m_TopPoints / 2;
	float len = m_Length / (d - 1);
	dist = m_StartDistance;
	int i;

	m_pCArr = new vector2df[m_TopPoints];

	float lw = (m_bLeftEdge ? gMaxEdgeStripSize : gMaxStripSize);
	float rw = (m_bRightEdge ? gMaxEdgeStripSize : gMaxStripSize);

	vector3df v;
	vector2df *pc = m_pCArr;
	vector2df *pc2 = m_pCArr + m_TopPoints;

	for (i = 0;
		  i < d;
		  i++,dist += len)  {
		RoadLoc(NULL, dist, loc, vec, grade);
		v = loc + vec * rw;
		if (i == 0)
			m_ExtMin = m_ExtMax = v;
		else
			extRect3(m_ExtMin, m_ExtMax, v);
		pc->x = v.x;
		pc->y = v.z;
		pc++;
		v = loc - vec * lw;
		extRect3(m_ExtMin, m_ExtMax, v);
		pc2--;
		pc2->x = v.x;
		pc2->y = v.z;
	}
	m_ExtOrigin = (m_ExtMin + m_ExtMax) / 2;
	v = m_ExtMax - m_ExtOrigin;
	m_ExtRadius = v.getLength() + 100.0f;
}


/************************************************************************************

************************************************************************************/

void CCourse::Section::SetEditMode(EditMode em)  {
	if (m_EditMode == em)
		return;

#ifdef BIKE_APP
	CloseModel();
#endif

	m_EditMode = em;
	switch (em)  {
		case NORMAL:
			m_RoadColor = RGBAToSColor(255, 255, 255, 255);
			m_TopColor = RGBAToSColor(152, 157, 12, 50);
			if (this == m_Course.m_pEdit)
				m_Course.m_pEdit = NULL;
			break;
		case PROBLEM:
			m_RoadColor = RGBAToSColor(255, 0, 0, 255);
			m_TopColor = RGBAToSColor(152, 0, 0, 128);
			break;
		case EDIT:
			if (m_Course.m_pEdit)
				m_Course.m_pEdit->SetEditMode(NORMAL);
			m_RoadColor = RGBAToSColor(255, 255, 0, 255);
			m_TopColor = RGBAToSColor(152, 157, 12, 128);
			m_Course.m_pEdit = this;
			break;
	}
}


/************************************************************************************

************************************************************************************/

bool CCourse::Section::Set(float deg, float len)  {
	if ((deg > SECTION_MAX_ROTATION) || (deg < -SECTION_MAX_ROTATION) || (len < SECTION_MIN_LENGTH))
		return false;
	m_DegRotation = deg;
	m_Length = len;
	m_Flags &= ~(SECTION_F_ABS_STARTLOC | SECTION_F_ABS_Y);
	m_ActualLength = 0.0f;
	calcValues();
	return true;
}


/************************************************************************************
	grade is %grade / 100
************************************************************************************/

bool CCourse::Section::set_grade_d_100(float _grade_d_100)  {
	bool ans = true;
	if (_grade_d_100 < -SECTION_GRADE_MAX_D_100)  {
		ans = false;
		_grade_d_100 = -SECTION_GRADE_MAX_D_100;
	}
	else if (_grade_d_100 > SECTION_GRADE_MAX_D_100)  {
		ans = false;
		_grade_d_100 = SECTION_GRADE_MAX_D_100;
	}

	grade_d_100 = _grade_d_100;
	m_Flags &= ~(SECTION_F_ABS_STARTLOC | SECTION_F_ABS_Y);
	m_ActualLength = 0.0f;
	calcValues();
	return ans;
}


/************************************************************************************
	wind is METRIC!
************************************************************************************/

bool CCourse::Section::SetWind(float wind)  {
	bool ans = true;

	if (wind < -SECTION_WIND_MAX)  {						// -50.0f km/hr
		ans = false;
		wind = -SECTION_WIND_MAX;
	}
	else if (wind > SECTION_WIND_MAX)  {				// 50.0f km/hr
		ans = false;
		wind = SECTION_WIND_MAX;
	}

	m_Wind = wind;

	calcValues();
	return ans;
}


/************************************************************************************

************************************************************************************/

void CCourse::Section::ClearAllSigns()  {
#ifndef IGNOREFORNOW
	SignData *psign;
	while ((psign = m_SignList.GetFirst()) != NULL)
		delete psign;
#endif
}


/******************************************************************************
	counts the number of REAL course legs. Doesn't count lead in Sections,
	finish Sections, or post-finish Sections.
******************************************************************************/

int CCourse::countSections(void)  {
	Section *ps = NULL;
	int i;

	ps = GetFirstRealSection();
	if (ps==NULL)  {
		return 0;
	}


	int n = m_Track.Count();

	i = 0;

	while(1)  {
		i++;
		ps = ps->Next();
		if (ps==NULL)  {
			break;
		}
		if (ps->IsFinishSection())  {			// there is only one finish Section
			//bp = 1;

#ifdef TOPO_APP								// ????
			i++;									// tlm20060511
#endif
			break;
		}
	}

	return i;
}


/******************************************************************************

******************************************************************************/

void CCourse::Section::dump(int totalCount, int regularCount, FILE *stream, bool _metric, bool showMiles) {
	float len, sd, ed, totalLen, swnd, mwnd;
	float grd, elev, hdng, bend;
	vector3df startloc, endloc;
	float startmiles, endmiles;

	grd = 100.0f*get_grade_d_100();												// converted to percentage
	hdng = ffmod(radToDeg(m_EndRotation), 360);					// heading
	if (hdng < 0)  {
		hdng += 360;
	}
	bend = GetDegRotation();													// bend

	if (_metric)  {
		len = m_Length;
		elev = GetEndLoc().y;
		swnd = m_StartWind;
		mwnd = m_Wind;
		sd = m_StartDistance;
		startmiles = sd*METERSTOFEET/5280.0f;
		totalLen = m_Course.GetCourseLength();
	}
	else  {
		len = METERSTOFEET * m_Length;
		elev = METERSTOFEET * GetEndLoc().y;
		swnd = KMTOMILES * m_StartWind;
		mwnd = KMTOMILES * m_Wind;
		sd = METERSTOFEET * m_StartDistance;
		startmiles = sd/5280.0f;
		totalLen = METERSTOFEET * m_Course.GetCourseLength();
	}

	ed = sd + len;
	if (_metric)  {
		endmiles = sd*METERSTOFEET/5280.0f;
	}
	else  {
		endmiles = ed/5280.0f;
	}

	endloc = GetEndLoc();
	startloc = GetStartLoc();



	int rc = regularCount;

	if (m_Type==EXTRA)  {
		rc = 0;
	}

	if (_metric)  {
		if (showMiles)  {
			fprintf(stream, "%4d %4d %12.6f %12.6f %7.2f%% %12.6f %12.6f %12.6f %12.6f %12.6f %12.6f %12.6f %12.6f %12s",
				totalCount, 
				rc, 
				METERSTOFEET * m_StartDistance,
				len,
				grd,
				elev,
				hdng,
				bend,
				swnd,
				mwnd,
				startmiles, 
				endmiles, 
				endloc.y,
				type[m_Type] );
		}
		else  {
			fprintf(stream, "%4d %4d %12.6f %12.6f %12.6f%% %12.6f %12.6f %12.6f %12.6f %12.6f %12.6f %12.6f %12.6f %12s",
					totalCount, 
					rc, 
					METERSTOFEET * m_StartDistance,
					len,
					grd,
					elev,
					hdng,
					bend,
					swnd,
					mwnd,
					sd, 
					ed, 
					endloc.y,
					type[m_Type] );
		}
	}

	else  {
		if (showMiles)  {
			fprintf(stream, "%4d %4d %12.6f %12.6f %12.6f%% %12.6f %12.6f %12.6f %12.6f %12.6f      %12.6f  %12.6f %12.6f %12s",
					totalCount, 
					rc, 
					METERSTOFEET * m_StartDistance,
					len,
					grd,
					elev,
					hdng,
					bend,
					swnd,
					mwnd,
					startmiles, 
					endmiles, 
					endloc.y,
					type[m_Type] );
		}
		else  {
			if (stream)  {
				fprintf(stream, "%4d %4d %12.6f %12.6f %12.6f%% %12.6f %12.6f %12.6f %12.6f %12.6f %12.6f %12.6f %12.6f %12s",
					totalCount, 
					rc, 
					m_StartDistance,
					len,
					grd,
					elev,
					hdng,
					bend,
					swnd,
					mwnd,
					sd, 
					ed, 
					endloc.y,
					type[m_Type] );
			}
		}
	}

	if (stream)  {
		if (sd < 0.0)  {
			fprintf(stream, "_(lead_in)\n");
		}
		else if (IsFinishSection())  {
			fprintf(stream, "_(lead_out)\n");
		}
		else if (ed > totalLen)  {
			fprintf(stream, "_(lead_out)\n");
		}
		else if (m_Type==EXTRA)  {
			fprintf(stream, "_(lead_out)\n");
		}
		else  {
			fprintf(stream, "\n");
		}
	}

	return;
}


/******************************************************************************

******************************************************************************/

void CCourse::dump(char *fname, bool showMiles)  {
	FILE *stream = NULL;
	Section *ps = NULL;
	int totalSectionCount = 0;		// includes lead in and lead out Sections
	int regularSectionCount = 0;
	bool finished = false;

	stream = fopen(fname, "wt");

	fprintf(stream, "dump of %s\n", GetCourseName());
	fprintf(stream, "version = %d\n", GetVersion());
	fprintf(stream, "number of REAL Sections = %d\n", countSections());
	const char *cptr = GetCourseDescription();
	if (*cptr != 0)  {
		fprintf(stream, "description = %s\n", cptr);
	}
	else  {
		fprintf(stream, "description = n/a\n");
	}

	fprintf(stream, "length = %.2f meters (%.2f miles)\n", GetCourseLength(), METERSTOMILES*GetCourseLength() );
	//fprintf(stream, "\n   I  LEG      SD     LEN    GRADE    ELEV HEADING    BEND  SWND  MWND      SD      ED    ENDY         TYPE\n\n");
	fprintf(stream, "\n   I  LEG           SD          LEN         GRADE         ELEV      HEADING         BEND         SWND         MWND                SD            ED         ENDY         TYPE\n");
	if (metric)  {
		fprintf(stream, "                           (meters)                   (meters)    (DEGREES)                     (KPH)        (KPH)              (KM)          (KM)     (meters)\n\n");
	}
	else  {
		fprintf(stream, "                             (feet)                     (feet)    (DEGREES)                     (MPH)        (MPH)           (miles)       (miles)       (feet)\n\n");
	}

	ps = GetFirstSection();
	if (ps==NULL)  {
		fclose(stream);
#ifndef WEBAPP
		//throw (fatalError(__FILE__, __LINE__));
#else
		return;
#endif
	}

	totalSectionCount = 1;

	if (ps->GetStartDistance() >= 0.0f)  {
		regularSectionCount = 1;
	}

	ps->dump(totalSectionCount++, regularSectionCount, stream, metric, showMiles);

	//----------------------------------------------------------------------------------
	// dump and advance past the lead in sections. Also dumps the first real section.
	//----------------------------------------------------------------------------------

	bool startseparator = false;
	bool endseparator = false;

	for(;ps && (ps->GetStartDistance() < 0.0f);)  {		// <0 means that we're on the course "lead in"
		ps = ps->Next();
		if (ps->GetStartDistance() >= 0.0f)  {
			fprintf(stream, "+++\n");				// separator
			startseparator = true;
			regularSectionCount++;
		}
		ps->dump(totalSectionCount++, regularSectionCount, stream, metric, showMiles);
	}
	if (ps==NULL)  {
		fclose(stream);
#ifndef WEBAPP
		//throw (fatalError(__FILE__, __LINE__));
#else
		return;
#endif
	}

	if (!startseparator)  {
		fprintf(stream, "+++\n");				// separator
	}

	bool badNumber = false;


	while (1)  {
		ps = ps->Next();
		if (ps==NULL)  {
			break;
		}

		//float fdist = ps->GetStartDistance();

		//-------------------------------------------
		// check here for .3dc file errors, bugs
		//-------------------------------------------

		if (_isnan(ps->m_Length))  {				//1.#QNAN
			badNumber = true;;
		}
		if (!_finite(ps->m_Length))  {			// 1.#INDO
			badNumber = true;;
		}

		if (_isnan(ps->m_DivisionLength))  {				//1.#QNAN
			badNumber = true;;
		}
		if (!_finite(ps->m_DivisionLength))  {			// 1.#INDO
			badNumber = true;;
		}

		if (_isnan(ps->m_Radius))  {				//1.#QNAN
			badNumber = true;;
		}
		if (!_finite(ps->m_Radius))  {			// 1.#INDO
			badNumber = true;;
		}

		if (_isnan(ps->m_ExtRadius))  {				//1.#QNAN
			badNumber = true;;
		}
		if (!_finite(ps->m_ExtRadius))  {			// 1.#INDO
			badNumber = true;;
		}

		if (badNumber)  {
			bp = 1;
		}

		ps->dump(totalSectionCount++, regularSectionCount, stream, metric, showMiles);

		if (!ps->IsFinishSection())  {
			if (!finished)  {
				regularSectionCount++;
			}
		}
		else  {
			regularSectionCount++;					// only 1 finish section
			if (!finished)  {
				finished = true;
				fprintf(stream, "---\n");				// separator
				endseparator = true;
			}
		}

	}

	if (!endseparator)  {
		fprintf(stream, "---\n");				// separator
	}

	fprintf(stream, "\n\n");

	//----------------------------------------------------------------------------------
	// dump the tree info for debugging the trees
	//----------------------------------------------------------------------------------

	ps = GetFirstSection();

//	for(;ps && (ps->GetStartDistance() < 0.0f);)  {		// <0 means that we're on the course "lead in"
//		ps = ps->Next();
//		bp = 1;
//	}

	int cnt = 0;
	bp = 0;
	bool done = false;

	fprintf(stream, "cnt    length   m_Divisions   m_DivisionLength  m_TreeCount   tgix\n\n");

	while (1)  {
		if (ps==NULL)  {
			bp = 2;
			break;
		}
		if (ps->GetStartDistance() < 0.0f)  {
			ps = ps->Next();
			continue;
		}
		cnt++;
		if (ps->IsFinishSection())  {
			//bp = 1;
			done = true;
			break;
		}


		fprintf(stream, "%3d  %8.3f            %2d           %8.3f            %d   %d\n",
			cnt, 
			ps->m_Length, 
			ps->m_Divisions, 
			ps->m_DivisionLength, 
			ps->m_TreeCount,
			ps->tree_group_index
			);

		if (done)  {				// if we've processed the finish section, we're done
			break;
		}
		ps = ps->Next();
	}

	fprintf(stream, "\n\n");


	if (fname != NULL)  {
		fclose(stream);
	}

	return;
}

/**************************************************************

**************************************************************/

int CCourse::GetVersion(void)  {
	return m_Version;
}

/******************************************************************************

******************************************************************************/

bool CCourse::Section::SetLength( float _meters ) {
	m_Length = _meters;

	//m_Flags &= ~(SECTION_F_ABS_STARTLOC | SECTION_F_ABS_Y);
	m_ActualLength = 0.0f;

	calcValues();

	return true;
}

/******************************************************************************

******************************************************************************/

bool CCourse::Section::SetAngle( float _degrees ) {

	m_DegRotation = _degrees;

	//m_Flags &= ~(SECTION_F_ABS_STARTLOC | SECTION_F_ABS_Y);
	m_ActualLength = 0.0f;

	calcValues();

	return true;
}

/******************************************************************************

******************************************************************************/

void CCourse::cleanup(void)  {
//#ifndef IGNOREFORNOW

	ClearTrack();

	DELARR(crs);
	DELARR(accum_meters);

#ifdef BIKE_APP

//#ifndef IGNOREFORNOW
	//delete m_pSign;
	//delete m_pBanner;
//#endif

	delete m_pFileName;
	DELARR(m_pName);

	if (m_pDescription)
		delete m_pDescription;

	if (m_pCreator)
		delete m_pCreator;

	if (m_GridYArr)
		delete[] m_GridYArr;

#ifndef IGNOREFORNOW
	for (int i = 0;
		  i < TEX_MAX;
		  i++)  {
		if (m_Tex[i])
			m_Tex[i]->Release();
	}

	if (m_pMesh)
		delete m_pMesh;
	if (m_pMeshDef)
		delete m_pMeshDef;
#endif
	/*
	for (int i = 0; i < TEX_MAX;  i++)  {
		if (m_Tex[i])  {
			DELARR(m_Tex[i])
			m_Tex[i] = 0L;
		}
	}
	*/

	laps = 1;

	m_pStartSection = NULL;
	m_pFinishSection = NULL;
#else
	ClearTrack();

	// tlm20040130:
	DELARR(m_pFileName);
	DELARR(m_pName);

	DEL(m_pDescription);

	if (m_GridYArr)  {
		delete[] m_GridYArr;
	}

	laps = 1;

	if (m_pStartSection)  {
		m_pStartSection = NULL;
	}

	if (m_pFinishSection)  {
		m_pFinishSection = NULL;
	}


#endif
//#endif
	return;
}

/************************************************************************************

************************************************************************************/

void CCourse::unencrypt(const char *_fname)  {

	FILE *instream=NULL;
	FILE *outstream=NULL;
	int status;
	unsigned long u;
	unsigned char c;
	const char *x = {"x3x1c4n5"};
	char nn[256];
	long offs;

	crf.init();

	instream = fopen(_fname, "rb");
	if (instream==NULL)  {
		return;
	}

	fread(&u, 4, 1, instream);									// get the version
	if(u == 6) // if unencrypted then leave it alone
	{
		offs = ftell(instream);								// 524
		fclose(instream);
		return;
	}
	u = 6; // change the version to unencrypted

	strncpy(nn, _fname, sizeof(nn)-1);
	strip_filename(nn);
	strcat(nn, "\\");
	strcat(nn, x);

	outstream = fopen(nn, "wb");
	fwrite(&u, 4, 1, outstream);

	while(1)  {
		status = fgetc(instream);
		if (status == EOF)  {
			break;
		}
		c = (unsigned char) status;

		//s->doo(&c, 1);
		crf.doo(&c, 1);
		fputc(c, outstream);
	}

	offs = ftell(instream);								// 524
	if (fclose(instream))  {
		return;
	}

	offs = ftell(outstream);							// 524
	fclose(outstream);

	if (_unlink(_fname))  {								// unlink the encrypted file
		return;
	}

	if (rename(nn, _fname))  {
		return;
	}

	return;
}


/************************************************************************************

************************************************************************************/

void CCourse::doo(const char *_fname)  {
//#ifndef IGNOREFORNOW
	//CRF *s;
	FILE *instream=NULL;
	FILE *outstream=NULL;
	int status;
	unsigned long u;
	unsigned char c;
	const char *x = {"x3x1c4n5"};
	char nn[256];

	//s = new CRF();
	//s->init();
	crf.init();


	instream = fopen(_fname, "rb");
	if (instream==NULL)  {
		//DEL(s);
		#ifndef WEBAPP
			//throw (fatalError(__FILE__, __LINE__));
		#else
			return;
		#endif
	}


	#if defined(MULTI_APP) || defined(MULTIVID_APP)
	//#ifdef MULTI_APP
		nn[0] = 0;
	#else
		strncpy(nn, _fname, sizeof(nn)-1);
		strip_filename(nn);
		strcat(nn, "\\");
	#endif

	strcat(nn, x);

	outstream = fopen(nn, "wb");
	fread(&u, 4, 1, instream);									// get the version
	fwrite(&u, 4, 1, outstream);

	while(1)  {
		status = fgetc(instream);
		if (status == EOF)  {
			break;
		}
		c = (unsigned char) status;

		//s->doo(&c, 1);
		crf.doo(&c, 1);
		fputc(c, outstream);
	}

	//DEL(s);

	long offs;

	offs = ftell(instream);								// 524

	if (fclose(instream))  {
		#ifndef WEBAPP
			//throw (fatalError(__FILE__, __LINE__));
		#else
			return;
		#endif
	}

	offs = ftell(outstream);							// 524
	fclose(outstream);

	if (_unlink(_fname))  {								// unlink the encrypted file
		#ifndef WEBAPP
			//throw (fatalError(__FILE__, __LINE__));		// vista error
		#else
			return;
		#endif
	}

	if (rename(nn, _fname))  {
		#ifndef WEBAPP
			sprintf(gstring, "%s --- %s", nn, _fname);
			//throw (fatalError(__FILE__, __LINE__, gstring));
		#else
			return;
		#endif
	}
//#endif
	return;
}

/********************************************************************************************

********************************************************************************************/

#ifdef BIKE_APP

/********************************************************************************************
	read scene.txt to get the scenery definition.

	sets CCourse::m_Tex[TEX_MAX] to the correct texture files for this scenery pack

	ignores:
		VAL_TREES_PER_100_MAX	50
		VAL_TREES_PER_100_MIN	5

	resets (based on data in scene.txt):
		treeArr
		treeGroup
		treeGroupCount
		stripArrBase

********************************************************************************************/


bool CCourse::SetLandscape(const char *name)  {
//#ifndef IGNOREFORNOW

	char buf[256];
	char sbuf[256];
	bool getting_strip_info = false;
	int stripline=0;
	int status;
	char path[256];
	FILE *file;


//	sprintf(path, ".\\Media\\Data\\%s\\scene.txt", name);
//	sprintf(path, ".\\Media\\%s\\scene.txt", name);

	sprintf(path, ".\\Media\\%s.txt", name);

	file = fopen(path, "rt");

	if (!file)  {
		sprintf(gstring, "Can't open %s", path);
		throw (fatalError(__FILE__, __LINE__, gstring));
		//return false;
	}


	TreeData td;
	TreeGroupData tg;

	TreeData tdarr[MAX_TREEDATA];				// MAX_TREEDATA = 40
	TreeGroupData tgarr[MAX_TREEGROUP];		// MAX_TREEGROUP = 10

	int tdcount = 0;
	int tgcount = 0;
	int firstline = -1;
	int line = 0;
	char *argv[4];
	int argc;

	while (fgets(buf, 256, file))  {

		if (buf[0]==';')  {
			continue;								// skip comment lines
		}
		if (buf[0]=='#')  {
			continue;								// skip comment lines
		}

		if (buf[0]==0x0a)  {
			continue;								// skip blank lines
		}

		strcpy(sbuf, buf);
		argc = ParseLine(sbuf, 4, argv);

		if ((argc >= 2) && (strncmp(argv[0], "TEX_", 4) == 0))  {

			static char *texnames[] = { "TEX_ROAD", "TEX_EDGE_1_L", "TEX_EDGE_1_R", "TEX_EDGE_2_L",
												 "TEX_EDGE_2_R", "TEX_EDGE_3_L", "TEX_EDGE_3_R", "TEX_EDGE_4_L",
												 "TEX_EDGE_4_R", "TEX_EDGE_5_L", "TEX_EDGE_5_R", "TEX_START",
												 "TEX_FINISH", "TEX_OVERRUN", "TEX_TREE1", "TEX_TREE1_SHADOW",
												 "TEX_TREE2", "TEX_TREE2_SHADOW", "TEX_TREE3",
												 "TEX_TREE3_SHADOW", "TEX_TREE4", "TEX_TREE4_SHADOW",
												 "TEX_FAR_GROUND", "TEX_BACK_1", "TEX_BACK_2", "TEX_BACK_3",
												 "TEX_BACK_4","TEX_DOME", NULL };
			int cnt;
			char **ppname;

			for (ppname = texnames,cnt = 0; *ppname; ppname++,cnt++)  {
				if (strcmp(argv[0], *ppname) == 0)
					break;
			}

			DELARR(m_Tex[cnt]);
			if (*ppname)  {
				//sprintf(buf, ".\\Media\\%s\\%s", name, argv[1]);
				sprintf(buf, "Data/%s/%s", name, argv[1]);
				//sprintf(buf, "%s", argv[1]);
				m_Tex[cnt] = new char[strlen(buf) + 1];
				strcpy(m_Tex[cnt], buf);
				//m_Tex[cnt] = D3DBase::GetDriver()->getTexture(buf);
			}

			continue; // Got it .. continue.
		}


		/*********************************

		*********************************/

		if ((argc >= 2) && (strncmp(argv[0], "VAL_", 4) == 0))  {
			ValNames *pn = valNames;
			// this sets treeMin and treeMax
			for (; pn->name; pn++)  {
				if (_stricmp(pn->name, argv[0]) == 0)  {
					*pn->fptr = atof(argv[1]);
					break;
				}
			}
			continue;
		}

		line++;

		/*****************************************

		*****************************************/

		int ans = sscanf(buf, "%f,%f,%f,%f,%f,%f,%f,%f,%f,%d,%f,%f", 
									&td.width,								//  5				1.8
									&td.height,								//  5				2.5
									&td.w_adj_min,							// -1				-.2
									&td.w_adj_max,							//  1				.2
									&td.h_adj_min,							// -1.5			-.2
									&td.h_adj_max,							//  1.5			.2
									&td.scale_adj,							//   .1			.1
									&td.offsetx,							// -1				-1
									&td.offsety,							//   .4			.4
									&td.tex,									//  1				1
									&td.shadow_width_scale,				//   .9			.9
									&td.shadow_height_scale);			//   .6			.6
		if (ans == 12)  {
			if (firstline < 0)
				firstline = line;
			if (tdcount >= MAX_TREEDATA)  {
				fclose(file);
				return false;
			}
			td.tex = CCourse::TEX_TREE1 + (td.tex - 1) * 2;				// 14, 16, 18, 20
			tdarr[tdcount++] = td;
			continue;
		}

		/*************
			start,stop
				 0,   1
				 2,   3
		 *************/

		ans = sscanf(buf, "%d,%d", &tg.start, &tg.count);
		if (ans == 2)  {
			if ((tgcount >= MAX_TREEGROUP) || (firstline < 0))  {
				fclose(file);
				return false;
			}
			//tlm20051117+++
			//tg.count = tg.count - tg.start;
			tg.count = tg.count - tg.start + 1;
			//tlm20051117---
			tgarr[tgcount++] = tg;						// tgcount is the number of <start,stop> pairs in scene.txt
		}

		// get the sky color out of scene.txt in r, g, b format


		//sky_color: .37 .58 .98
		int pos;
		pos = indexIgnoreCase(buf, "sky_color:", 0);
		if (pos != -1)  {
			int status = sscanf(&buf[pos]+10, "%d, %d, %d", &sky_r, &sky_g, &sky_b);
			if (status != 3)  {
				sky_r = 94;
				sky_g = 148;
				sky_b = 250;
			}
			continue;
		}

		pos = indexIgnoreCase(buf, "skydome:", 0);
		if (pos != -1)  {
			int status = sscanf(&buf[pos]+8, "%f", &sky_tiled);
			if (status != 1)  {
				sky_tiled = -3.0f;
			}
			continue;
		}

		pos = indexIgnoreCase(buf, "texuv:", 0);
		if (pos != -1)  {
			int status = sscanf(&buf[pos]+6, "%f, %f", &ms_usize, &ms_vsize);
			if (status != 2)  {
				ms_usize = 10.0f;							// 20.0f, smaller = more repeats, passed to Terrain::
				ms_vsize = 10.0f;							// 20.0f, smaller = more repeats, passed to Terrain::
			}
			continue;
		}

		pos = indexIgnoreCase(buf, "max_trees_per_section:", 0);
		if (pos != -1)  {
			int status = sscanf(&buf[pos]+strlen("max_trees_per_section:"), "%f", &treesMaxPerSection);
			if (status != 1)  {
				treesMaxPerSection = 25;
			}
			if(treesMaxPerSection > 25)
				treesMaxPerSection = 25;
			continue;
		}

		//--------------------------------------
		// read the strip information:
		//--------------------------------------

		pos = indexIgnoreCase(buf, "strip information:", 0);
		if (pos != -1)  {
			getting_strip_info = true;
			stripline = 0;
			continue;
		}

		if (getting_strip_info)  {
			int i1, i2;
			float f1, f2, f3, f4, f5, f6;
			status = sscanf(buf, "%f, %d, %f, %f, %f, %f, %f, %d", &f1, &i1, &f2, &f3, &f4, &f6, &f5, &i2);
			if (status==8)  {
				stripArrBase[stripline].offset = f1;
				stripArrBase[stripline].div_divide = i1;
				stripArrBase[stripline].rndlow = f2;
				stripArrBase[stripline].rndhigh = f3;
				stripArrBase[stripline].texrepeat = f4;
				stripArrBase[stripline].x_repeat = f6;

				stripArrBase[stripline].coloradd = f5;
				if (i2==0)  {
					stripArrBase[stripline].treeok = false;
				}
				else  {
					stripArrBase[stripline].treeok = true;
				}

				stripline++;
				if (stripline==10)  {
					getting_strip_info = false;
				}
				continue;
			}

		}
	}		// while...


	fclose(file);

	memcpy(treeArr, tdarr, sizeof(TreeData) * tdcount);
	memcpy(treeGroup, tgarr, sizeof(TreeGroupData) * tgcount);
	treeGroupCount = tgcount;
#ifndef IGNOREFORNOW
	if (Landscape::Instance())  {
		Landscape::Instance()->Redo();
	}
#endif

//#endif
	return true;
}

bool CCourse::SetLandscape2(const char *name)  {
//#ifndef IGNOREFORNOW

	char *pbuf;

	char buf[256];
	char sbuf[256];
	bool getting_strip_info = false;
	int stripline=0;
	int status;
	char path[256];

	IReadFile *file;


//	sprintf(path, ".\\Media\\Data\\%s\\scene.txt", name);
//	sprintf(path, ".\\Media\\%s\\scene.txt", name);
//	sprintf(path, ".\\Media\\%s.txt", name);

	//sprintf(path, "Data/%s/scene.txt", name);
	sprintf(path, "%s/scene.txt", name);
	file = D3DBase::GetDevice()->getFileSystem()->createAndOpenFile(path);

	if (!file)  {
		sprintf(gstring, "Can't open %s", path);
		throw (fatalError(__FILE__, __LINE__, gstring));
		//return false;
	}


	TreeData td;
	TreeGroupData tg;

	TreeData tdarr[MAX_TREEDATA];				// MAX_TREEDATA = 40
	TreeGroupData tgarr[MAX_TREEGROUP];		// MAX_TREEGROUP = 10

	int tdcount = 0;
	int tgcount = 0;
	int firstline = -1;
	int line = 0;
	char *argv[4];
	int argc;

	s32 len; 
	len = file->getSize();
	pbuf = new char[len+1];
	if(len != file->read(pbuf, len))
		return false;
	file->drop();

	// default set in case values were not set.
	sky_r = 94;
	sky_g = 148;
	sky_b = 250;
	sky_tiled = -3.0f;
	ms_usize = 20.0f;							// 20.0f, smaller = more repeats, passed to Terrain::
	ms_vsize = 20.0f;							// 20.0f, smaller = more repeats, passed to Terrain::

	for (int j = 0; j < TEX_MAX; j++)
		DELARR(m_Tex[j]);

	char *p = pbuf;
	//while (fgets(buf, 256, file))  {
	while (len > 0)  {

		int i;
		for(i = 0; i<len; i++)
		{
			if(p[i] == 0x0a)
			{
				i++;
				break;
			}
		}
		len -= i;
		strncpy(buf,p,i);
		buf[i]='\0';
		p += i;

		if (buf[0]==';')  {
			continue;								// skip comment lines
		}
		if (buf[0]=='#')  {
			continue;								// skip comment lines
		}

		if (buf[0]==0x0a)  {
			continue;								// skip blank lines
		}

		strcpy(sbuf, buf);
		argc = ParseLine(sbuf, 4, argv);

		if ((argc >= 2) && (strncmp(argv[0], "TEX_", 4) == 0))  {

			static char *texnames[] = { "TEX_ROAD", "TEX_EDGE_1_L", "TEX_EDGE_1_R", "TEX_EDGE_2_L",
												 "TEX_EDGE_2_R", "TEX_EDGE_3_L", "TEX_EDGE_3_R", "TEX_EDGE_4_L",
												 "TEX_EDGE_4_R", "TEX_EDGE_5_L", "TEX_EDGE_5_R", "TEX_START",
												 "TEX_FINISH", "TEX_OVERRUN", "TEX_TREE1", "TEX_TREE1_SHADOW",
												 "TEX_TREE2", "TEX_TREE2_SHADOW", "TEX_TREE3",
												 "TEX_TREE3_SHADOW", "TEX_TREE4", "TEX_TREE4_SHADOW",
												 "TEX_FAR_GROUND", "TEX_BACK_1", "TEX_BACK_2", "TEX_BACK_3",
												 "TEX_BACK_4","TEX_DOME", NULL };
			int cnt;
			char **ppname;

			for (ppname = texnames,cnt = 0; *ppname; ppname++,cnt++)  {
				if (strcmp(argv[0], *ppname) == 0)
					break;
			}

			DELARR(m_Tex[cnt]);
			if (*ppname)  {
				//sprintf(buf, "Data/%s/%s", name, argv[1]);
				sprintf(buf, "%s/%s", name, argv[1]);
				m_Tex[cnt] = new char[strlen(buf) + 1];
				strcpy(m_Tex[cnt], buf);
			}

			continue; // Got it .. continue.
		}


		/*********************************

		*********************************/

		if ((argc >= 2) && (strncmp(argv[0], "VAL_", 4) == 0))  {
			ValNames *pn = valNames;
			// this sets treeMin and treeMax
			for (; pn->name; pn++)  {
				if (_stricmp(pn->name, argv[0]) == 0)  {
					*pn->fptr = atof(argv[1]);
					break;
				}
			}
			continue;
		}

		line++;

		/*****************************************

		*****************************************/

		int ans = sscanf(buf, "%f,%f,%f,%f,%f,%f,%f,%f,%f,%d,%f,%f", 
									&td.width,								//  5				1.8
									&td.height,								//  5				2.5
									&td.w_adj_min,							// -1				-.2
									&td.w_adj_max,							//  1				.2
									&td.h_adj_min,							// -1.5			-.2
									&td.h_adj_max,							//  1.5			.2
									&td.scale_adj,							//   .1			.1
									&td.offsetx,							// -1				-1
									&td.offsety,							//   .4			.4
									&td.tex,									//  1				1
									&td.shadow_width_scale,				//   .9			.9
									&td.shadow_height_scale);			//   .6			.6
		if (ans == 12)  {
			if (firstline < 0)
				firstline = line;
			if (tdcount >= MAX_TREEDATA)  {
				//fclose(file);
				//file->drop();
				delete[] pbuf;
				return false;
			}
			td.tex = CCourse::TEX_TREE1 + (td.tex - 1) * 2;				// 14, 16, 18, 20
			tdarr[tdcount++] = td;
			continue;
		}

		/*************
			start,stop
				 0,   1
				 2,   3
		 *************/

		ans = sscanf(buf, "%d,%d", &tg.start, &tg.count);
		if (ans == 2)  {
			if ((tgcount >= MAX_TREEGROUP) || (firstline < 0))  {
				//fclose(file);
				//file->drop();
				delete[] pbuf;
				return false;
			}
			//tlm20051117+++
			//tg.count = tg.count - tg.start;
			tg.count = tg.count - tg.start + 1;
			//tlm20051117---
			tgarr[tgcount++] = tg;						// tgcount is the number of <start,stop> pairs in scene.txt
		}

		// get the sky color out of scene.txt in r, g, b format


		//sky_color: .37 .58 .98
		int pos;
		pos = indexIgnoreCase(buf, "sky_color:", 0);
		if (pos != -1)  {
			int status = sscanf(&buf[pos]+10, "%d, %d, %d", &sky_r, &sky_g, &sky_b);
			if (status != 3)  {
				sky_r = 94;
				sky_g = 148;
				sky_b = 250;
			}
			continue;
		}

		pos = indexIgnoreCase(buf, "skydome:", 0);
		if (pos != -1)  {
			int status = sscanf(&buf[pos]+8, "%f", &sky_tiled);
			if (status != 1)  {
				sky_tiled = -3.0f;
			}
			continue;
		}

		pos = indexIgnoreCase(buf, "texuv:", 0);
		if (pos != -1)  {
			int status = sscanf(&buf[pos]+6, "%f, %f", &ms_usize, &ms_vsize);
			if (status != 2)  {
				ms_usize = 10.0f;							// 20.0f, smaller = more repeats, passed to Terrain::
				ms_vsize = 10.0f;							// 20.0f, smaller = more repeats, passed to Terrain::
			}
			continue;
		}

		pos = indexIgnoreCase(buf, "max_trees_per_section:", 0);
		if (pos != -1)  {
			int status = sscanf(&buf[pos]+strlen("max_trees_per_section:"), "%f", &treesMaxPerSection);
			if(status != 1 || treesMaxPerSection > 25)
				treesMaxPerSection = 25;
			continue;
		}

		//--------------------------------------
		// read the strip information:
		//--------------------------------------

		pos = indexIgnoreCase(buf, "strip information:", 0);
		if (pos != -1)  {
			getting_strip_info = true;
			stripline = 0;
			continue;
		}

		if (getting_strip_info)  {
			int i1, i2;
			float f1, f2, f3, f4, f5, f6;
			status = sscanf(buf, "%f, %d, %f, %f, %f, %f, %f, %d", &f1, &i1, &f2, &f3, &f4, &f6, &f5, &i2);
			if (status==8)  {
				stripArrBase[stripline].offset = f1;
				stripArrBase[stripline].div_divide = i1;
				stripArrBase[stripline].rndlow = f2;
				stripArrBase[stripline].rndhigh = f3;
				stripArrBase[stripline].texrepeat = f4;
				stripArrBase[stripline].x_repeat = f6;

				stripArrBase[stripline].coloradd = f5;
				if (i2==0)  {
					stripArrBase[stripline].treeok = false;
				}
				else  {
					stripArrBase[stripline].treeok = true;
				}

				stripline++;
				if (stripline==10)  {
					getting_strip_info = false;
				}
				continue;
			}

		}
	}		// while...


	//fclose(file);
	//file->drop();
	delete[] pbuf;


	memcpy(treeArr, tdarr, sizeof(TreeData) * tdcount);
	memcpy(treeGroup, tgarr, sizeof(TreeGroupData) * tgcount);
	treeGroupCount = tgcount;
#ifndef IGNOREFORNOW
	if (Landscape::Instance())  {
		Landscape::Instance()->Redo();
	}
#endif

//#endif
	return true;
}


/******************************************************************************

******************************************************************************/

void CCourse::ClearTerrain()  {

	Terrain::ClearAll();

	if (m_pEdge)  {
		delete[] m_pEdge;
		m_pEdge = NULL;
	}

	if (m_pTerrainGrid)  {
		delete[] m_pTerrainGrid;
		m_pTerrainGrid = NULL;
	}
}



/************************************************************************************

************************************************************************************/
#ifndef IGNOREFORNOW
bool CCourse::GetPerfRiderData(int num, struct RiderData &rd)  {
	bool m = gMetric;
	gMetric = true;
	PerformanceInfo &pi = pinf[num];
	rd.SetName(pi.name);
	rd.SetGender((RiderData::Gender) pi.gender);
	rd.SetAge(pi.age);
	rd.SetHeight(pi.height);
	rd.SetWeight(pi.weight);
	rd.SetLowerHeartRate(pi.lower_hr);
	rd.SetUpperHeartRate(pi.upper_hr);
	rd.watts_factor = pi.watts_factor;
	rd.ftp = pi.ftp;
	gMetric = m;
	return true;
}
#endif
/******************************************************************************
	called from BACOURSE
******************************************************************************/

void CCourse::ReadyRace()  {

	Section *ps;
	Section *pnext;
	bool dirty = false;

	for (ps = m_Track.GetFirst(); ps; ps = pnext)  {
		pnext = ps->Next();

		if ((ps->m_Type != Section::STREIGHT) && (ps->m_Type != Section::CURVE))
			continue;

		float deg = ps->GetDegRotation();

		/*
		if (fabs(deg) > 10.0f)  {
												float len = ps->GetLength()/2.0f;
												Section *pnew = new Section( *this, Section::SPLIT, len, ps->GetGrade(), deg/2.0f);
												pnew->InsertAfter( *ps );
												ps->m_Length -= len;
												ps->m_DegRotation -= deg/2.0f;
												dirty = true;
		}
	*/
	}


	RedoCourse();
	calcGrid();
	RedoCourse();
	BuildTerrain();


#ifndef IGNOREFORNOW
	// ADD IN THE SIGNS

	#define MAX_SIGNS 256
	  {
		int i;
		ClearAllSigns();
/*
		if (gpBikeApp->GetDisplayPrefs().displaysigns)  {
			char *tstr = new char[MAX_PATH *MAX_SIGNS];
			char **strarr = new char *[MAX_SIGNS];
			for (i=0; i<MAX_SIGNS; i++)  {
				strarr[i] = tstr + (i * MAX_PATH);
			}
*/
			int cnt = 0;
			WIN32_FIND_DATA fd;
			HANDLE h = FindFirstFile(".\\media\\signs\\*.bmp", &fd);

			if (h != INVALID_HANDLE_VALUE)  {
				do  {
					sprintf(strarr[cnt], ".\\media\\signs\\%s", fd.cFileName);
					cnt++;
					if (cnt >= MAX_SIGNS)
						break;
				} while (FindNextFile(h, &fd));
				FindClose(h);
			}

			qsort(strarr, cnt, sizeof(char *), cmpStr);

			FILE *file = fopen(".\\media\\Start.bmp", "r");
			if (file)  {
				fclose(file);
				AddSign(".\\media\\Start.bmp", 0.0f, m_pBanner, true);
			}
			file = fopen(".\\media\\Finish.bmp", "r");
			if (file)  {
				fclose(file);
				AddSign(".\\media\\Finish.bmp", GetCourseLength(), m_pBanner, true);
			}

			float len = GetCourseLength() / (cnt + 2);
			float tq;

			for (tq = len,i = 0; i < cnt; i++,tq += len)  {
				AddSign(strarr[i], tq, m_pSign, false);
			}

			delete[] strarr;
			delete[] tstr;
		}
	}
#endif
}		// ReadyRace


/******************************************************************************

******************************************************************************/

void CCourse::SetCourseFog()  {
#ifndef IGNOREFORNOW

	static float density = 0.01f;
	static DWORD color = RGBAToSColor(255, 255, 255, 0);

	if (m_FogOn)  {
		ms_pDevice->SetRenderState(D3DRS_FOGDENSITY, *(DWORD *) (&m_FogDensity));
		ms_pDevice->SetRenderState(D3DRS_FOGCOLOR, m_FogColor);
		ms_pDevice->SetRenderState(D3DRS_FOGTABLEMODE, D3DFOG_EXP);
		ms_pDevice->SetRenderState(D3DRS_FOGENABLE, TRUE);
	}
	else
		ms_pDevice->SetRenderState(D3DRS_FOGENABLE, FALSE);
#endif
}

/******************************************************************************

******************************************************************************/

HRESULT CCourse::Render(void)  {

	return 0L;
#ifndef IGNOREFORNOW
	int r = 5;

	if (m_pMesh)  {
		m_pMesh->Render();
	}

	D3DMATERIAL7 mat;
	ZeroMemory(&mat, sizeof(mat));
	mat.diffuse = D3DXCOLOR(1.0f, 1.0f, 1.0f, 1.0f);
	mat.ambient = D3DXCOLOR(0.4f, 0.4f, 0.4f, 1.0f);
	mat.power = 1.0f;
	ms_pDevice->SetMaterial(&mat);

	SetCourseFog();

	// Setup
	//ms_pDevice->SetVertexShader( FVF_COURSEVERTEX );

	//----------------------
	// draw the sections:
	//----------------------

	ms_pDevice->SetRenderState(D3DRS_LIGHTING, FALSE);
	ms_pDevice->SetRenderState(D3DRS_SPECULARENABLE, FALSE);

	// WW: Added N915 - Setup for Tree and road fix.
	// ==============
	ms_pDevice->SetTextureStageState( 0, D3DTSS_BORDERCOLOR, 0x00000000 );
	ms_pDevice->SetRenderState( D3DRENDERSTATE_ALPHAREF, 0x02 );
	//ms_pDevice->SetRenderState( D3DRENDERSTATE_ALPHAREF, 0x00 );								// no diff
	ms_pDevice->SetRenderState( D3DRENDERSTATE_ALPHAFUNC, D3DCMP_GREATER  );
	//ms_pDevice->SetRenderState( D3DRENDERSTATE_ALPHAFUNC, D3DCMP_GREATEREQUAL );		// no diff

	 // ==============
	//ms_pDevice->SetRenderState(D3DRENDERSTATE_ANTIALIAS, TRUE);		// tlm tried this, no difference
	//ms_pDevice->SetRenderState(D3DRENDERSTATE_EDGEANTIALIAS, TRUE);	// tlm tried this, no difference

	Section *ps;


	for (ps = m_Track.GetFirst(); ps; ps = ps->Next())  {
		//if (FRUSTUM_OUTSIDE != ms_pCamera->SphereInFrustum(ps->GetExtOrigin(), ps->GetExtRadius()))  {
		if (ms_pCamera->SphereInFulcrum2D(ps->GetExtOrigin(), ps->GetExtRadius()))  {
			ps->Render();
		}
		else  {
			if (ps->NoRender())  {
				ps->CloseModel();
			}
		}
	}


	/**************************************************************************************
		this draws the "far strip" after TEX_EDGE_5_R
		the texture repeat rate is controlled by Terrain::ms_usize, and Terrain::ms_vsize
	**************************************************************************************/

	ms_pDevice->SetTexture(0, m_Tex[TEX_FAR_GROUND]->GetTex());

	if (m_pTerrainGrid)  {
		Terrain::StartRender();
			vector3df minv, maxv;
			ms_pCamera->GetFulcrumBox(minv, maxv);
			int fx, fz, tx, tz;
			GridXY(minv.x, minv.z, fx, fz);
			GridXY(maxv.x, maxv.z, tx, tz);			// fx, fz - tx, tz = (9, 10 - 12, 16), eg
			int x, z;
			Terrain **ppstart = m_pTerrainGrid + (fx + fz*m_GridWidth);
			Terrain **ppt;
			for (z=fz; z<=tz; z++, ppstart+=m_GridWidth)  {
				ppt = ppstart;
				for (x=fx; x<=tx; x++, ppt++)  {
					if (*ppt && (*ppt)->InFulcrum())  {
						(*ppt)->Render();
					}
				}
			}
		Terrain::EndRender();
	}

	ms_pDevice->SetRenderState(D3DRS_LIGHTING, TRUE);
	ms_pDevice->SetRenderState(D3DRS_FOGENABLE, FALSE);

	m_EyeNum = -1;
#else
/*
	// Common material default
	video::IVideoDriver* driver = D3DBase::GetDriver();

	bool uselight = false;

	Material.TextureLayer[0].Texture = driver->getTexture("road1.tga");
	Material.setFlag(video::EMF_NORMALIZE_NORMALS,uselight);
	Material.setFlag(video::EMF_LIGHTING,uselight);
	Material.setFlag(video::EMF_FOG_ENABLE, fogOn);
	Material.setFlag(video::EMF_BACK_FACE_CULLING, true);

	driver->setMaterial(Material);

	driver->setTransform(video::ETS_WORLD, D3DBase::GetSceneManager()->getRootSceneNode()->getAbsoluteTransformation());
	Section *ps;
	for (ps = m_Track.GetFirst(); ps; ps = ps->Next())  {
		ps->render();
	}
*/
#endif
}

/******************************************************************************

******************************************************************************/

HRESULT CCourse::RenderEdit()  {

#ifndef IGNOREFORNOW
	//ms_pDevice->SetVertexShader( FVF_COURSEVERTEX );

	ms_pDevice->SetRenderState(D3DRS_LIGHTING, FALSE);
	ms_pDevice->SetRenderState(D3DRS_CULLMODE, D3DCULL_NONE);

	ms_pDevice->SetRenderState(D3DRS_ZBIAS, 0);

	ms_pDevice->SetRenderState(D3DRS_ZENABLE, TRUE);
	Section *ps;
	for (ps = m_Track.GetFirst();
		  ps;
		  ps = ps->Next())  {
		// Render until one of the road sections is NOT in the view.

		if (FRUSTUM_OUTSIDE != ms_pCamera->SphereInFrustum(ps->GetExtOrigin(), ps->GetExtRadius()))  {
		//if (ms_pCamera->SphereInFulcrum(ps->GetExtOrigin(), ps->GetExtRadius()))  {
			ps->RenderEditRoad();
		}
	}

	ms_pDevice->SetRenderState(D3DRS_ZENABLE, TRUE);
	for (ps = m_Track.GetFirst();
		  ps;
		  ps = ps->Next())  {
		vector3df v;
		v = ps->GetEndLoc();
		v.y = m_GridMin.y;
		gpSortedTri->DrawLine3D(v, ps->GetEndLoc(), RGBAToSColor(0, 0, 0, 255));
	}
	ms_pDevice->SetRenderState(D3DRS_ZENABLE, TRUE);

	ms_pDevice->SetRenderState(D3DRS_ZBIAS, 0);

	//ms_pDevice->SetVertexShader( FVF_COURSEVERTEX );

	ms_pDevice->SetRenderState(D3DRS_LIGHTING, FALSE);
	ms_pDevice->SetRenderState(D3DRS_ALPHABLENDENABLE, TRUE);
	ms_pDevice->SetTextureStageState(0, D3DTSS_COLOROP, D3DTOP_DISABLE);
	ms_pDevice->SetTextureStageState(0, D3DTSS_ALPHAOP, D3DTOP_DISABLE);
	ms_pDevice->SetTexture(0, NULL);

	ms_pDevice->SetRenderState(D3DRS_ZFUNC, D3DCMP_ALWAYS);

	for (ps = m_Track.GetFirst();
		  ps;
		  ps = ps->Next())  {
		// Render until one of the road sections is NOT in the view.

		vector3df v = ps->GetExtOrigin();
		v.y = 0.0f;

		//if (ms_pCamera->SphereInFulcrum2D(v, ps->GetExtRadius()))
		  {
			ps->RenderEditTop();
		}
	}

	ms_pDevice->SetRenderState(D3DRS_ZFUNC, D3DCMP_LESSEQUAL);

	ms_pDevice->SetRenderState(D3DRS_ALPHABLENDENABLE, FALSE);
	ms_pDevice->SetTextureStageState(0, D3DTSS_COLOROP, D3DTOP_MODULATE);
	ms_pDevice->SetTextureStageState(0, D3DTSS_ALPHAOP, D3DTOP_MODULATE);

	ms_pDevice->SetRenderState(D3DRS_LIGHTING, TRUE);
	ms_pDevice->SetRenderState(D3DRS_CULLMODE, D3DCULL_CCW);
#endif
	return 0L;
}


static	std::vector<COURSEVERTEX> *pLeftEdge;
static	std::vector<COURSEVERTEX> *pRightEdge;


/******************************************************************************
	add is 0 or 1
	draws the progress bar
******************************************************************************/

void CCourse::barAdd(int add, const char *message)  {


	m_BarCount += add;
	if (m_BarCount < m_BarNext)
		return;

	if (m_BarCount > m_BarTotal)
		m_BarCount = m_BarTotal;
	else if (m_BarCount < 0)
		m_BarCount = 0;

	float val = m_BarCount / m_BarTotal;

#ifndef IGNOREFORNOW
	BikeApp &ba = *gpBikeApp;
	ba.SpecialRenderStart();
	ms_pDevice->Clear(0L, NULL, D3DCLEAR_TARGET | D3DCLEAR_ZBUFFER, D3DCOLOR_XRGB(0, 0, 0), 1.0f, 0L);


	if (ba.m_pFont)  {
		ba.m_pFont->DrawCenteredFormated(320, 200, RGBAToSColor(255, 255, 255, 255), message,	(int) (val * 100));
	}

	//Box(100, 220, 540, 260, RGBAToSColor(255, 255, 255, 255));
	Box(100, 220, 540, 260, RGBAToSColor(255, 0, 0, 255));
	Box(104, 224, 536, 256, RGBAToSColor(0, 0, 0, 255));
	//Box(106, 226, (int) ((534 - 106) * val) + 106, 254, RGBAToSColor(255, 255, 255, 255));
	Box(106, 226, (int) ((534 - 106) * val) + 106, 254, RGBAToSColor(200, 200, 0, 255));
	gpBikeApp->SpecialRenderEnd();
#endif

	do  {
		m_BarNext += m_BarTotal / 100.0f;
	} while (m_BarNext <= m_BarCount);

	D3DBase::m_fBuild = barProgress();
}


/******************************************************************************

******************************************************************************/

void CCourse::barInit(int total, float max)  {
	m_BarTotal = total / max;
	m_BarMax = max;
	m_BarCount = 0.0f;
	m_BarNext = m_BarTotal / 100.0f;

	D3DBase::m_fBuild = barProgress();
}


/******************************************************************************

******************************************************************************/

void CCourse::barRescale(int total, float max)  {

	if (total < 1)
		total = 1;

	float t = m_BarCount / m_BarTotal;
	float r = m_BarNext / m_BarTotal;
	if (max < m_BarMax)
		max = m_BarMax + 0.01;
	m_BarTotal = total / (max - m_BarMax);
	m_BarMax = max;
	m_BarCount = m_BarTotal * t;
	m_BarNext = m_BarTotal * r;

	D3DBase::m_fBuild = barProgress();
}


/************************************************************************************

************************************************************************************/

bool CCourse::BuildTerrain(bool displaybar)  {

	int i, max;
	Section *ps;

	int sections = Renumber();			// just numbers ps->m_Num to the number that it is in

	barInit(sections + (sections * (sections + 1)) / 2, 0.60f);		// progress bar


	for (ps=m_Track.GetFirst(); ps; ps=ps->Next())  {
		ps->CloseModel();
		ps->closeCArr();
		ps->ReadyModel(true);				// <<<<<<<<<<<<<<<<<<<
		float t = ps->GetRotation();

		if (t < -0.01f)  {
			t = -(float) (ps->GetLength() / t);
			ps->m_bLeftEdge = (t > maxEdgeRadius);
			ps->m_bRightEdge = true;
		}
		else if (t > 0.01f)  {
			t = -(float) (ps->GetLength() / t);
			ps->m_bLeftEdge = true;
			ps->m_bRightEdge = (-t > maxEdgeRadius);
		}
		else  {
			ps->m_bLeftEdge = ps->m_bRightEdge = true;
		}

		//ps->m_bLeftEdge = ps->m_bRightEdge = true;

		ps->calcCArr();
	}


	//--------------------------------------------------
	// Fremove the edges that collide with others
	//--------------------------------------------------

	bp = 0;

	for (ps = m_Track.GetFirst(); ps; ps=ps->Next())  {

		Section *pc = ps->Prev();

		if (pc)	{		// Don't check collsion with the previous one...

			for (pc = pc->Prev(); pc; pc = pc->Prev())  {
				barAdd();
				int startv = (ps->m_bLeftEdge ? 8 : 0) |
								 (ps->m_bRightEdge ? 4 : 0) |
								 (pc->m_bLeftEdge ? 2 : 0) |
								 (pc->m_bRightEdge ? 1 : 0);

				if (startv && CheckCollision(ps, pc))  {
					bool bv;
					bool change;
					char *porder = orderArr[startv];

					while (*porder)  {
						bv = (*porder++ == '1');
						if (bv != ps->m_bLeftEdge)  {
							ps->m_bLeftEdge = bv;
							change = true;
						}
						else  {
							change = false;
						}

						bv = (*porder++ == '1');

						if (bv != ps->m_bRightEdge)  {
							ps->m_bRightEdge = bv;
							change = true;
						}
						if (change)  {
							ps->closeCArr();
							ps->calcCArr();
						}

						bv = (*porder++ == '1');
						if (bv != pc->m_bLeftEdge)  {
							pc->m_bLeftEdge = bv;
							change = true;
						}
						else  {
							change = false;
						}
						bv = (*porder++ == '1');
						if (bv != pc->m_bRightEdge)  {
							pc->m_bRightEdge = bv;
							change = true;
						}
						if (change)  {
							pc->closeCArr();
							pc->calcCArr();
						}

						if (!CheckCollision(ps, pc))  {
							break;
						}

						if (*porder)  {
							porder++;
						}
					}

					if (*porder == '\0')  {
						if (ps->m_bLeftEdge || ps->m_bRightEdge)  {
							ps->m_bLeftEdge = ps->m_bRightEdge = false;
							ps->closeCArr();
							ps->calcCArr();
						}
						if (pc->m_bLeftEdge || pc->m_bRightEdge)  {
							pc->m_bLeftEdge = pc->m_bRightEdge = false;
							pc->closeCArr();
							pc->calcCArr();
						}
					}
				}
			}
		}
	}

	bp = 1;

	for (ps = m_Track.GetFirst(); ps; ps=ps->Next())  {
		ps->CloseModel();
		ps->closeCArr();
		ps->ReadyModel(true);				// <<<<<<<<<<<<<<<<<<<
	}

	ClearTerrain();

	//---------------------------
	// Set each point to a box.
	//---------------------------

	calcGrid(100.0f);
	float width = m_GridMax.x - m_GridMin.x;
	float height = m_GridMax.z - m_GridMin.z;
	gTerrainGridSize = (float) ((int) (gMaxStripSize * 2.5f));
	int xx, yy, gsize;

	do  {
		if (gTerrainGridSize < 70.0f)
			gTerrainGridSize = 70.0f;
		else
			gTerrainGridSize += 20.0f;

		xx = width / gTerrainGridSize + 1;
		yy = height / gTerrainGridSize + 1;
		gsize = xx * yy;
	}	while (gTerrainGridSize <300.0f && (gsize> 4096));


	if (gTerrainGridSize > 300.0f)
		gTerrainGridSize = 300.0f;

	calcTerrainGrid(gTerrainGridSize, gTerrainBorder);

	m_bBuildingEdgeMesh = true;

	stripArr[0].offset += 0.01f;
	stripArr[lSTRIPS - 1].offset -= 0.01f;

	pLeftEdge = new std::vector<COURSEVERTEX>;
	pRightEdge = new std::vector<COURSEVERTEX>;

	//pStartEdge = new std::vector<COURSEVERTEX>;
	//pEndEdge = new std::vector<COURSEVERTEX>;


	for (ps = m_Track.GetFirst(); ps; ps=ps->Next())  {
		barAdd();
		ps->CloseModel();
		ps->ReadyModel();		// repoints stripArr to different offsets in stripArrBase	// <<<<<<<<<<<<<<<<<<<
	}

	// tlm: note stripArr may not be the same as stripArrBase now!! So why do this?

	stripArr[0].offset -= 0.01f;
	stripArr[lSTRIPS - 1].offset += 0.01f;

	m_bBuildingEdgeMesh = false;

//#ifndef IGNOREFORNOW
	//-------------------------------------------------------------------------------------------------
	// Now combine the edge points into a real list, If we are looping we get two lists but we will
	// be putting these into one array with two different lenghts.
	// Don't worry about the end sections - they are always flat anyway.
	//-------------------------------------------------------------------------------------------------

int cnt;

	 cnt = pLeftEdge->size() + pRightEdge->size();

	m_pEdge = new COURSEVERTEX[cnt];
	m_EdgeTotal = cnt;

	if (m_bLoopClosed)  {
		m_EdgeCount[0] = pLeftEdge->size();
		m_EdgeCount[1] = pRightEdge->size();
	}
	else  {
		m_EdgeCount[0] = cnt;
		m_EdgeCount[1] = 0;
	}

	COURSEVERTEX *pc = m_pEdge;

	for (i = 0,max = pLeftEdge->size(); i < max; i++)  {
		*pc++ = (*pLeftEdge)[i];
	}

	for (i = pRightEdge->size() - 1; i >= 0; i--)  {
		*pc++ = (*pRightEdge)[i];
	}


	// OK we don't need the edge anymore.

	delete pLeftEdge;
	delete pRightEdge;

	//delete pStartEdge;
	//delete pEndEdge;

	// tlm20051203+++
	// m_pTerrainGrid's boundary is overflowing below, so I tried allocating a little more space to keep that
	// from happening. Since I don't understand this code and am only trying to keep boundschecker from complaining
	// for now. this workaround worked.
	//max = m_GridWidth * m_GridHeight;
	max = (m_GridWidth+1) * (m_GridHeight+1);
	// tlm20051203---


	m_pTerrainGrid = new Terrain * [max];
	memset(m_pTerrainGrid, 0, sizeof(Terrain *) * max);

	int lchange = -1, rchange = -1;

	for (i = 0,pc = m_pEdge; i < m_EdgeCount[0]; i++,pc++)  {
		pc->cel = GridCel(pc->v.x, pc->v.z);
		if ((lchange < 0) && (pc->cel != m_pEdge->cel))  {
			lchange = i;
		}
	}

	for (i = 0; i < m_EdgeCount[1]; i++,pc++)  {
		pc->cel = GridCel(pc->v.x, pc->v.z);
		if ((rchange < 0) && (pc->cel != m_pEdge[m_EdgeCount[0]].cel))  {
			rchange = i;
		}
	}


	if ((lchange < 0) || ((rchange < 0) && (m_EdgeCount[1] > 0)))  {
		// There is only one grid... This is a special case.

		Terrain *pt = new Terrain(*this, m_GridMin, m_GridMax);

		barRescale(m_EdgeCount[0] + m_EdgeCount[1], 1.0f);

		pt->StartEnclosed();
		pt->AddBox();
		pt->BeginContour();
		for (i = 0,pc = m_pEdge;  i < m_EdgeCount[0];  i++)  {
			barAdd();
			pt->AddV(*pc++);
		}

		if (m_bLoopClosed)  {
			pt->EndContour();
			pt->BeginContour();
		}

		for (i = 0; i < m_EdgeCount[1]; i++)  {
			barAdd();
			pt->AddV(*pc++);
		}

		pt->EndContour();
		pt->EndEnclosed();

		// Now build the terrain.  Set it to all the segments and we are done.

		return true;	// We are done
	}

	int x, z;

	// The harder way of doing things.

	int leftstart = 0;
	barRescale(m_EdgeCount[0], 0.65f);

	addTerrain(m_pEdge, m_EdgeCount[0], lchange, leftstart);
	barRescale(m_EdgeCount[1], 0.70f);
	addTerrain(m_pEdge + m_EdgeCount[0], m_EdgeCount[1], rchange, leftstart - m_EdgeCount[0]);

	barRescale(Terrain::CountAll(), 0.85f);

	Terrain::FinishCutAll();

	Terrain *pt;
	int empty = 0;
	int maxloops = (int) (gTerrainBorder / gTerrainGridSize);
	int gridsize = m_GridWidth *m_GridHeight;
	Terrain **pptemp = new Terrain *[gridsize];

	barRescale(maxloops * m_GridHeight, 0.98f);

	do  {
		empty = 0;
		memcpy(pptemp, m_pTerrainGrid, sizeof(Terrain *) * gridsize);

		for (z = 0; z < m_GridHeight; z++)  {
			barAdd();
			for (x = 0; x < m_GridWidth; x++)  {
				Terrain **ppn = pptemp + (x + z *m_GridWidth);
				if (*ppn)
					continue;
				int carr[4];
				float yval[4];
				carr[0] = carr[1] = carr[2] = carr[3] = 0;
				yval[0] = yval[1] = yval[2] = yval[3] = 0.0f;
				COURSEVERTEX *pcv;
				int cnt = 0;

				if ((x > 0) && (pt = ppn[-1]))  {
					cnt++;
					carr[0]++;
					carr[3]++;
					pcv = pt->GetCornerPoints();
					yval[0] += pcv[1].v.y;
					yval[3] += pcv[2].v.y;
				}

				if ((x < m_GridWidth - 1) && (pt = ppn[1]))  {
					cnt++;
					carr[0]++;
					carr[1]++;
					pcv = pt->GetCornerPoints();
					yval[0] += pcv[3].v.y;
					yval[1] += pcv[2].v.y;
				}

				if ((z > 0) && (pt = ppn[-m_GridWidth]))  {
					cnt++;
					carr[0]++;
					carr[3]++;
					pcv = pt->GetCornerPoints();
					yval[0] += pcv[1].v.y;
					yval[3] += pcv[2].v.y;
				}

				if ((z < m_GridHeight - 1) && (pt = ppn[m_GridWidth]))  {
					cnt++;
					carr[3]++;
					carr[2]++;
					pcv = pt->GetCornerPoints();
					yval[3] += pcv[0].v.y;
					yval[2] += pcv[1].v.y;
				}

				if (cnt > 0)  {
					vector3df minv, maxv;
					minv.x = m_GridMin.x + m_GridSquareSize.x * x;
					minv.y = 0.0f;
					minv.z = m_GridMin.z + m_GridSquareSize.z * z;
					maxv = minv + m_GridSquareSize;

					pt = m_pTerrainGrid[x + z * m_GridWidth] = new Terrain(*this, minv, maxv);

					pt->InitCut();
					pt->FinishCut();
					pcv = pt->GetCornerPoints();
					for (int i = 0; i < 4; i++)  {
						if (carr[i] == 0)  {
							for (int j = 1; j < 4; j++)  {
								int t = (i + j) & 3;
								yval[i] += yval[t];
								carr[i] += carr[t];
							}
						}
						pcv[i].v.y = yval[i] / carr[i];
					}
				}
				else
					empty++;
			}
		}
	}


	while (empty && (maxloops-- > 0));
	delete[] pptemp;

	// Figure out the "corner points."

	barRescale(m_GridHeight, 1.1f);


	for (z = 0; z <= m_GridHeight; z++)  {
		barAdd();
		for (x = 0; x <= m_GridWidth; x++)  {
			vector3df *pv[4];
			int cnt = 0;

			Terrain **ppn = m_pTerrainGrid + (x + z *m_GridWidth);

			if (z < m_GridHeight)  {
				if ((x < m_GridWidth) && *ppn)
					pv[cnt++] = &((*ppn)->GetCornerPoints())[0].v;
				if ((x > 0) && ppn[-1])
					pv[cnt++] = &(ppn[-1]->GetCornerPoints())[1].v;
			}

			if (z > 0)  {
				ppn -= m_GridWidth;
				if ((x < m_GridWidth) && *ppn)
					pv[cnt++] = &((*ppn)->GetCornerPoints())[3].v;
				if ((x > 0) && ppn[-1])
					pv[cnt++] = &(ppn[-1]->GetCornerPoints())[2].v;
			}

			if (cnt > 1)  {
				int i;
				float y = pv[1]->y;

				for (i = 1; i < cnt; i++)
					y += pv[i]->y;

				y /= cnt;

				for (i = 0; i < cnt; i++)
					pv[i]->y = y;
			}
		}
	}

	m_BarNext = 0.0f;

	barAdd();

	//Terrain::DumpAll( "..\\TestCourse.obj" );
//#endif
	return true;
}		// bool CCourse::BuildTerrain(bool displaybar)

#ifndef WEBAPP

/************************************************************************************

************************************************************************************/

void CCourse::addTerrain(COURSEVERTEX *pstart, int count, int start, int leftstart)  {
//#ifndef IGNOREFORNOW

	if (count <= 0)
		return;
	Terrain *pt = NULL;
	COURSEVERTEX cv;
	COURSEVERTEX *pedge = pstart + start;
	COURSEVERTEX *pend = pstart + count;
	COURSEVERTEX *plast = pedge - 1;
	if (plast < pstart)
		pedge = pend - 1;
	int i;
	for (i = 0;
		  i <= count;
		  i++)  {
		barAdd();
		if (plast->cel != pedge->cel)  {
			// Start by copying out the last - this will be used to determine where we are entering or exiting a cel.

			cv = *plast;
			int gx, gz;
			int ngx=0, ngz=0;
			int dir = -1;
			GridXY(cv.v.x, cv.v.z, gx, gz);
			vector3df minv, maxv, ncv;
			while (1)  {
				float dist = 100000000.0f, d;
				minv.x = m_GridMin.x + m_GridSquareSize.x * gx;
				minv.y = 0.0f;
				minv.z = m_GridMin.z + m_GridSquareSize.z * gz;
				maxv = minv + m_GridSquareSize;

				// Try to exit this square.

				if (cv.v.x != pedge->v.x)  {
					if (pedge->v.x <= minv.x)  {
						d = (cv.v.x - minv.x) / (cv.v.x - pedge->v.x);
						if (d < dist)  {
							ngx = gx - 1;
							ngz = gz;
							dist = d;
							dir = 3;
						}
					}
					if (pedge->v.x >= maxv.x)  {
						d = (cv.v.x - maxv.x) / (cv.v.x - pedge->v.x);
						if (d < dist)  {
							ngx = gx + 1;
							ngz = gz;
							dist = d;
							dir = 1;
						}
					}
				}
				if (cv.v.z != pedge->v.z)  {
					if (pedge->v.z <= minv.z)  {
						d = (cv.v.z - minv.z) / (cv.v.z - pedge->v.z);
						if (d < dist)  {
							ngx = gx;
							ngz = gz - 1;
							dist = d;
							dir = 0;
						}
					}
					if (pedge->v.z >= maxv.z)  {
						d = (cv.v.z - maxv.z) / (cv.v.z - pedge->v.z);
						if (d < dist)  {
							ngx = gx;
							ngz = gz + 1;
							dist = d;
							dir = 2;
						}
					}
				}
				if ((gx == ngx) && (gz == ngz))
					break;
				if (dir < 0)
					dir = dir;
				COURSEVERTEX cvnew;
				cvnew = cv;
				cvnew.v = cv.v + (pedge->v - cv.v) * dist;

				// end the last one.

				if (pt)
					pt->EndCut(cvnew, dir, leftstart == 0);
				cv = cvnew;

				// Start the new one.

				gx = ngx;
				gz = ngz;
				Terrain **ppt = (m_pTerrainGrid + (gx + gz *m_GridWidth));
				if (!*ppt)  {
					// OK we need to make one.

					minv.x = m_GridMin.x + m_GridSquareSize.x * gx;
					minv.y = 0.0f;
					minv.z = m_GridMin.z + m_GridSquareSize.z * gz;
					maxv = minv + m_GridSquareSize;
					*ppt = new Terrain(*this, minv, maxv);
					(*ppt)->InitCut();

					/*
																			pt = *ppt;
																			pt->StartEnclosed();
																			pt->AddBox();
																			pt->EndEnclosed();
																			*/
				}
				pt = *ppt;
				if (i < count)
					pt->StartCut(cv, (dir + 2) & 3, leftstart == 0);	// ENTER THE OPPOSITE EDGE.
			}
			if (i != count)
				pt->AddCutV(*pedge);
		}
		else if (pt)
			pt->AddCutV(*pedge);
		plast = pedge;
		pedge++;
		if (pedge >= pend)
			pedge = pstart;
	}
//#endif
}

#endif		// #ifndef WEBAPP

/************************************************************************************

************************************************************************************/

void CCourse::Section::CloseModel()  {
#ifndef IGNOREFORNOW


	if (m_pVBuf)
		m_pVBuf->Release();
	if (m_pIBuf)
		delete[] m_pIBuf;
	if (m_pRenderIns)
		delete[] m_pRenderIns;
	m_pVBuf = 0L;
	m_pIBuf = 0L;
	m_pRenderIns = 0L;

	if (m_pVBufEdit)  {
		m_pVBufEdit->Release();
		m_pVBufEdit = 0L;
	}
#else
	/*
	if (m_pMeshBuf)
		m_pMeshBuf->remove();
	m_pMeshBuf = NULL;
	*/
//	if (m_pMesh1)
//		m_pMesh1->drop();
//	m_pMesh1 = NULL;
//	if (m_pMesh2)
//		m_pMesh2->drop();
//	m_pMesh2 = NULL;
	if (m_pTreeArr)  {
		for (int i=0; i<m_TreeCount; i++)  {
			//if (m_pTreeArr[i])
			//	m_pTreeArr[i]->remove();
			m_pTreeArr[i] = NULL;
		}
		delete[] m_pTreeArr;
		m_pTreeArr = NULL;
	}

	if(m_pSectionNode)
	{
		m_pSectionNode = 0;
	}

	bMeshCreated = false;
	if (m_pRenderIns)
		delete[] m_pRenderIns;
	m_pRenderIns = 0L;
#endif
}

/************************************************************************************

************************************************************************************/

bool CCourse::Section::NoRender()  {
	m_NonDraw--;
	if (m_NonDraw < 0)  {
		m_NonDraw = 0;
	}
	return (m_NonDraw == 0);
}

/************************************************************************************

************************************************************************************/
#ifndef IGNOREFORNOW
void CCourse::Section::OnRegisterSceneNode()
{
	if (IsVisible)
		SceneManager->registerNodeForRendering(this);

	ISceneNode::OnRegisterSceneNode();
}
#endif
void CCourse::Section::render()  {
	return;
#ifndef IGNOREFORNOW

#ifndef IGNOREFORNOW
	m_NonDraw = 100;
	m_FrameNum = m_Course.m_FrameNum;

	if (!ReadyModel())  {
		return E_FAIL;
	}

	LPDIRECT3DDEVICE7 dev = m_Course.ms_pDevice;

	//dev->SetStreamSource( 0, m_pVBuf, sizeof(COURSEVERTEX) );
	//dev->SetIndices( m_pIBuf, 0 );

	short *pcmd = m_pRenderIns;

	//ms_pDevice->SetRenderState(D3DRS_ZENABLE,FALSE);

	while (*pcmd != RINS_END)  {
		switch (*pcmd++)  {
			case RINS_END:
				break;

			case RINS_TEX:
				// WW: Added N915  - Fix for the road
				{
				//float bias = (*pcmd == TEX_ROAD ? -5.6f:-0.05f);
				float bias = (*pcmd == TEX_ROAD ? -.6f:-0.05f);			// tlm20050509
				dev->SetTextureStageState( 0, D3DTSS_MIPMAPLODBIAS, *((LPDWORD)&bias) );

				// tlm: experimenting
				//dev->SetTextureStageState(0, D3DTSS_MAXMIPLEVEL, 1);
				/*
				typedef enum _D3DTEXTUREMAGFILTER {
					 D3DTFG_POINT        = 1,					// can notice a difference
					 D3DTFG_LINEAR       = 2,					// about the same
					 D3DTFG_FLATCUBIC    = 3,					// ditto
					 D3DTFG_GAUSSIANCUBIC= 4,  
					 D3DTFG_ANISOTROPIC  = 5,					// looks like before
					 D3DTFG_FORCE_DWORD  = 0x7fffffff,  
				}				*/
				//dev->SetTextureStageState( 0, D3DTSS_MAGFILTER, D3DTFG_GAUSSIANCUBIC);		// not much difference here


				/*
				typedef enum _D3DTEXTUREMINFILTER {
					 D3DTFN_POINT        = 1,
					 D3DTFN_LINEAR       = 2,
					 D3DTFN_ANISOTROPIC  = 3,
					 D3DTFN_FORCE_DWORD  = 0x7fffffff,
				} 
				*/

				//dev->SetTextureStageState( 0, D3DTSS_MINFILTER, D3DTFN_ANISOTROPIC);			// can't see any difference

				/*
					typedef enum _D3DTEXTUREMIPFILTER {
						 D3DTFP_NONE         = 1,  
						 D3DTFP_POINT        = 2,  
						 D3DTFP_LINEAR       = 3,  
						 D3DTFP_FORCE_DWORD  = 0x7fffffff,  
					} D3DTEXTUREMIPFILTER;
				*/

				//dev->SetTextureStageState( 0, D3DTSS_MIPFILTER, D3DTFP_LINEAR);						// not much difference

				}
				// =======
				dev->SetTexture(0, m_Course.m_Tex[*pcmd++]->GetTex());
				break;

			case RINS_STRIP:
				// 	 0 		  1			2  			3
				// MinIndex,NumVertices,StartIndex,PrimitiveCount
				//dev->DrawIndexedPrimitiveVB( D3DPT_TRIANGLESTRIP,m_pVBuf,pcmd[0],pcmd[1],pcmd[2],pcmd[3],0 );

				dev->DrawIndexedPrimitiveVB(D3DPT_TRIANGLESTRIP, m_pVBuf, pcmd[0], pcmd[1],
													 m_pIBuf + pcmd[2], pcmd[3], 0);
				gStats.vertsrendered += pcmd[3];

				//dev->DrawIndexedPrimitiveVB( D3DPT_TRIANGLESTRIP, m_pVBuf,0,pcmd[0]+pcmd[1],m_pIBuf+pcmd[2],pcmd[3],0 );

				pcmd += 4;
				break;

			case RINS_STRIP_T:
				dev->DrawPrimitiveVB(D3DPT_TRIANGLELIST, m_pVBuf, *pcmd++, 4, 0);
				gStats.vertsrendered += 4;
				break;

			case RINS_TRI:
				// 	 0 		  1			2  			3
				// MinIndex,NumVertices,StartIndex,PrimitiveCount
				//dev->DrawIndexedPrimitive( D3DPT_TRIANGLELIST,pcmd[0],pcmd[1],pcmd[2],pcmd[3] );

				dev->DrawIndexedPrimitiveVB(D3DPT_TRIANGLELIST, m_pVBuf, pcmd[0], pcmd[1],
													 m_pIBuf + pcmd[2], pcmd[3], 0);
				gStats.vertsrendered += pcmd[3];

				//dev->DrawIndexedPrimitiveVB( D3DPT_TRIANGLELIST, m_pVBuf,0,pcmd[0]+pcmd[1],m_pIBuf+pcmd[2],pcmd[3],0 );

				pcmd += 4;
				break;
			case RINS_TREESTART:
//#ifdef N915
				// WW: Added N915  - Fix shadow rendering
				m_Course.ms_pDevice->SetRenderState( D3DRENDERSTATE_ALPHATESTENABLE , TRUE );
				m_Course.ms_pDevice->SetRenderState( D3DRS_ALPHABLENDENABLE, TRUE );
				// ============
				m_Course.ms_pDevice->SetRenderState( D3DRS_ZBIAS, 3 );
				m_Course.ms_pDevice->SetTextureStageState( 0, D3DTSS_ALPHAOP,   D3DTOP_MODULATE );
				m_Course.ms_pDevice->SetTextureStageState( 0, D3DTSS_ALPHAARG1, D3DTA_TEXTURE );
				m_Course.ms_pDevice->SetTextureStageState( 0, D3DTSS_ALPHAARG2, D3DTA_DIFFUSE );
				// WW: Modified N915  - Fix shadow rendering
				m_Course.ms_pDevice->SetTextureStageState( 0, D3DTSS_ADDRESSU,D3DTADDRESS_BORDER  );             
				m_Course.ms_pDevice->SetTextureStageState( 0, D3DTSS_ADDRESSV,D3DTADDRESS_BORDER  );             
				// ============
//#else
//				m_Course.ms_pDevice->SetRenderState(D3DRS_ALPHABLENDENABLE, TRUE);
//				m_Course.ms_pDevice->SetRenderState(D3DRS_ZBIAS, 3);
//				m_Course.ms_pDevice->SetTextureStageState(0, D3DTSS_ALPHAOP, D3DTOP_MODULATE);
//				m_Course.ms_pDevice->SetTextureStageState(0, D3DTSS_ALPHAARG1, D3DTA_TEXTURE);
//				m_Course.ms_pDevice->SetTextureStageState(0, D3DTSS_ALPHAARG2, D3DTA_DIFFUSE);
//				m_Course.ms_pDevice->SetTextureStageState(0, D3DTSS_ADDRESSU, D3DTADDRESS_CLAMP);
//				m_Course.ms_pDevice->SetTextureStageState(0, D3DTSS_ADDRESSV, D3DTADDRESS_CLAMP);
//#endif

				break;

			case RINS_TREEEND:
//#ifdef N915
				// WW: Added N915  - Fix shadow rendering
				m_Course.ms_pDevice->SetRenderState( D3DRENDERSTATE_ALPHATESTENABLE , FALSE );
				// ============
				m_Course.ms_pDevice->SetRenderState( D3DRS_ZBIAS, 0 );
				m_Course.ms_pDevice->SetRenderState( D3DRS_ALPHABLENDENABLE, FALSE );
				m_Course.ms_pDevice->SetTextureStageState( 0, D3DTSS_ADDRESSU,D3DTADDRESS_WRAP );
				m_Course.ms_pDevice->SetTextureStageState( 0, D3DTSS_ADDRESSV,D3DTADDRESS_WRAP );
//#else
//				m_Course.ms_pDevice->SetRenderState(D3DRS_ZBIAS, 0);
//				m_Course.ms_pDevice->SetRenderState(D3DRS_ALPHABLENDENABLE, FALSE);
//				m_Course.ms_pDevice->SetTextureStageState(0, D3DTSS_ADDRESSU, D3DTADDRESS_WRAP);
//				m_Course.ms_pDevice->SetTextureStageState(0, D3DTSS_ADDRESSV, D3DTADDRESS_WRAP);
//#endif

				break;
		}
	}


	//------------------------------------
	// draw the trees for this section:
	//------------------------------------

	if (m_pTreeArr)  {
		Sprite **ppsprite = m_pTreeArr;
		for (int i = 0; i < m_TreeCount; i++,ppsprite++)  {
			if (*ppsprite)  {
				(*ppsprite)->Render();
			}
		}
	}

	// Draw the signs.
	//Set3DRenderState();
	//ms_pDevice->SetRenderState(D3DRS_AMBIENT, D3DXCOLOR(0.8f, 0.8f, 0.8f, 1));
	for (SignData*psign = m_SignList.GetFirst(); psign; psign = psign->Next())  {
		if (psign->ptex)
			psign->pmodel->ReplaceTexture((psign->pmodel==m_Course.m_pSign ? SIGN_TEX_NAME : "Banner.bmp"), psign->ptex);
		psign->pmodel->Render(psign->mat);
	}
	//Clear3DRenderState();

	/*
			D3DXMATRIX r;
			D3DXMATRIX m;
			static float up = 1.0f;
			D3DXMatrixRotationY( &r, m_StartRotation );
			D3DXMatrixTranslation( &m, m_StartLoc.x,m_StartLoc.y + up,m_StartLoc.z);
			D3DXMatrixMultiply( &m, &r, &m );
			m_Course.m_pSign->Render( m );
			*/

	//ms_pDevice->SetRenderState(D3DRS_ZENABLE,TRUE);
#else
	m_NonDraw = 100;
	m_FrameNum = m_Course.m_FrameNum;

	if (!ReadyModel())  {
		return; // E_FAIL;
	}
/*
	u16 indices[] = {	0,2,3, 2,1,3, 1,0,3, 2,0,1	};
	video::IVideoDriver* driver = SceneManager->getVideoDriver();

	driver->setMaterial(Material);
	driver->setTransform(video::ETS_WORLD, AbsoluteTransformation);
	driver->drawVertexPrimitiveList(&Vertices[0], 4, &indices[0], 4, video::EVT_STANDARD, scene::EPT_TRIANGLES, video::EIT_16BIT);
*/
	//LPDIRECT3DDEVICE7 dev = m_Course.ms_pDevice;

	if(!m_pMeshBuf)
		return; 

	video::IVideoDriver* driver = D3DBase::GetDriver();
	m_pMeshBuf->Material = m_Course.Material;
	driver->setMaterial(m_Course.Material);

//	driver->drawMeshBuffer(m_pMeshBuf);
//	return;

	u16 indices[] = {0,1,2,3,4,5,6,7,8,9,10,11};
	short *pcmd = m_pRenderIns;

	while (*pcmd != RINS_END)  {
		switch (*pcmd++)  {
			case RINS_END:
				break;

			case RINS_TEX:
				// WW: Added N915  - Fix for the road
				if(false)
				{
				*pcmd++;
				}
				else
				{
				m_pMeshBuf->Material.TextureLayer[0].Texture = m_Course.m_Tex[*pcmd++];
				driver->setMaterial(m_pMeshBuf->Material);
				//float bias = (*pcmd == TEX_ROAD ? -5.6f:-0.05f);
				/*
				float bias = (*pcmd == TEX_ROAD ? -.6f:-0.05f);			// tlm20050509
				dev->SetTextureStageState( 0, D3DTSS_MIPMAPLODBIAS, *((LPDWORD)&bias) );
				dev->SetTexture(0, m_Course.m_Tex[*pcmd++]->GetTex());
				*/

				// tlm: experimenting
				//dev->SetTextureStageState(0, D3DTSS_MAXMIPLEVEL, 1);
				/*
				typedef enum _D3DTEXTUREMAGFILTER {
					 D3DTFG_POINT        = 1,					// can notice a difference
					 D3DTFG_LINEAR       = 2,					// about the same
					 D3DTFG_FLATCUBIC    = 3,					// ditto
					 D3DTFG_GAUSSIANCUBIC= 4,  
					 D3DTFG_ANISOTROPIC  = 5,					// looks like before
					 D3DTFG_FORCE_DWORD  = 0x7fffffff,  
				}				*/
				//dev->SetTextureStageState( 0, D3DTSS_MAGFILTER, D3DTFG_GAUSSIANCUBIC);		// not much difference here


				/*
				typedef enum _D3DTEXTUREMINFILTER {
					 D3DTFN_POINT        = 1,
					 D3DTFN_LINEAR       = 2,
					 D3DTFN_ANISOTROPIC  = 3,
					 D3DTFN_FORCE_DWORD  = 0x7fffffff,
				} 
				*/

				//dev->SetTextureStageState( 0, D3DTSS_MINFILTER, D3DTFN_ANISOTROPIC);			// can't see any difference

				/*
					typedef enum _D3DTEXTUREMIPFILTER {
						 D3DTFP_NONE         = 1,  
						 D3DTFP_POINT        = 2,  
						 D3DTFP_LINEAR       = 3,  
						 D3DTFP_FORCE_DWORD  = 0x7fffffff,  
					} D3DTEXTUREMIPFILTER;
				*/

				//dev->SetTextureStageState( 0, D3DTSS_MIPFILTER, D3DTFP_LINEAR);						// not much difference

				}
				// =======

				//dev->SetTexture(0, m_Course.m_Tex[*pcmd++]->GetTex());
				break;

			case RINS_STRIP:
				if(FALSE)
				{
				pcmd += 4;
				}
				else
				{
				driver->drawVertexPrimitiveList(&m_pMeshBuf->Vertices[pcmd[0]], pcmd[1], &m_pMeshBuf->Indices[pcmd[2]], ((pcmd[3]/3)*3), 
					video::EVT_STANDARD, scene::EPT_TRIANGLE_STRIP, video::EIT_16BIT);
				//driver->drawVertexPrimitiveList(&m_pMeshBuf->Vertices[pcmd[0]], pcmd[1], &m_pMeshBuf->Indices[pcmd[2]], pcmd[3]/3, 
				//	video::EVT_STANDARD, scene::EPT_TRIANGLES, video::EIT_16BIT);
				// 	 0 		  1			2  			3
				// MinIndex,NumVertices,StartIndex,PrimitiveCount
				//dev->DrawIndexedPrimitiveVB(D3DPT_TRIANGLESTRIP, m_pVBuf, pcmd[0], pcmd[1],m_pIBuf + pcmd[2], pcmd[3], 0);
				//gStats.vertsrendered += pcmd[3];

				//dev->DrawIndexedPrimitiveVB( D3DPT_TRIANGLESTRIP,m_pVBuf,pcmd[0],pcmd[1],pcmd[2],pcmd[3],0 );
				//dev->DrawIndexedPrimitiveVB( D3DPT_TRIANGLESTRIP, m_pVBuf,0,pcmd[0]+pcmd[1],m_pIBuf+pcmd[2],pcmd[3],0 );
				pcmd += 4;
				}
				break;

			case RINS_STRIP_T:
				if(TRUE)
				{
				*pcmd++;
				}
				else
				{
				driver->drawVertexPrimitiveList(&m_pMeshBuf->Vertices[*pcmd++], 3, &indices[0], 1, 
					video::EVT_STANDARD, scene::EPT_TRIANGLES, video::EIT_16BIT);

				//dev->DrawPrimitiveVB(D3DPT_TRIANGLELIST, m_pVBuf, *pcmd++, 4, 0);
				//gStats.vertsrendered += 4;
				}
				break;

			case RINS_TRI:
				if(TRUE)
				{
				pcmd += 4;
				}
				else
				{
				driver->drawVertexPrimitiveList(&m_pMeshBuf->Vertices[pcmd[0]], pcmd[1], &m_pMeshBuf->Indices[pcmd[2]], pcmd[3], 
					video::EVT_STANDARD, scene::EPT_TRIANGLES, video::EIT_16BIT);
				// 	 0 		  1			2  			3
				// MinIndex,NumVertices,StartIndex,PrimitiveCount
				//dev->DrawIndexedPrimitiveVB(D3DPT_TRIANGLELIST, m_pVBuf, pcmd[0], pcmd[1],
				//									 m_pIBuf + pcmd[2], pcmd[3], 0);
				//gStats.vertsrendered += pcmd[3];

				//dev->DrawIndexedPrimitive( D3DPT_TRIANGLELIST,pcmd[0],pcmd[1],pcmd[2],pcmd[3] );
				//dev->DrawIndexedPrimitiveVB( D3DPT_TRIANGLELIST, m_pVBuf,0,pcmd[0]+pcmd[1],m_pIBuf+pcmd[2],pcmd[3],0 );

				pcmd += 4;
				}
				break;
			case RINS_TREESTART:
#ifndef IGNOREFORNOW
//#ifdef N915
				// WW: Added N915  - Fix shadow rendering
				m_Course.ms_pDevice->SetRenderState( D3DRENDERSTATE_ALPHATESTENABLE , TRUE );
				m_Course.ms_pDevice->SetRenderState( D3DRS_ALPHABLENDENABLE, TRUE );
				// ============
				m_Course.ms_pDevice->SetRenderState( D3DRS_ZBIAS, 3 );
				m_Course.ms_pDevice->SetTextureStageState( 0, D3DTSS_ALPHAOP,   D3DTOP_MODULATE );
				m_Course.ms_pDevice->SetTextureStageState( 0, D3DTSS_ALPHAARG1, D3DTA_TEXTURE );
				m_Course.ms_pDevice->SetTextureStageState( 0, D3DTSS_ALPHAARG2, D3DTA_DIFFUSE );
				// WW: Modified N915  - Fix shadow rendering
				m_Course.ms_pDevice->SetTextureStageState( 0, D3DTSS_ADDRESSU,D3DTADDRESS_BORDER  );             
				m_Course.ms_pDevice->SetTextureStageState( 0, D3DTSS_ADDRESSV,D3DTADDRESS_BORDER  );             
				// ============
//#else
//				m_Course.ms_pDevice->SetRenderState(D3DRS_ALPHABLENDENABLE, TRUE);
//				m_Course.ms_pDevice->SetRenderState(D3DRS_ZBIAS, 3);
//				m_Course.ms_pDevice->SetTextureStageState(0, D3DTSS_ALPHAOP, D3DTOP_MODULATE);
//				m_Course.ms_pDevice->SetTextureStageState(0, D3DTSS_ALPHAARG1, D3DTA_TEXTURE);
//				m_Course.ms_pDevice->SetTextureStageState(0, D3DTSS_ALPHAARG2, D3DTA_DIFFUSE);
//				m_Course.ms_pDevice->SetTextureStageState(0, D3DTSS_ADDRESSU, D3DTADDRESS_CLAMP);
//				m_Course.ms_pDevice->SetTextureStageState(0, D3DTSS_ADDRESSV, D3DTADDRESS_CLAMP);
//#endif
#endif
				break;

			case RINS_TREEEND:
#ifndef IGNOREFORNOW
//#ifdef N915
				// WW: Added N915  - Fix shadow rendering
				m_Course.ms_pDevice->SetRenderState( D3DRENDERSTATE_ALPHATESTENABLE , FALSE );
				// ============
				m_Course.ms_pDevice->SetRenderState( D3DRS_ZBIAS, 0 );
				m_Course.ms_pDevice->SetRenderState( D3DRS_ALPHABLENDENABLE, FALSE );
				m_Course.ms_pDevice->SetTextureStageState( 0, D3DTSS_ADDRESSU,D3DTADDRESS_WRAP );
				m_Course.ms_pDevice->SetTextureStageState( 0, D3DTSS_ADDRESSV,D3DTADDRESS_WRAP );
//#else
//				m_Course.ms_pDevice->SetRenderState(D3DRS_ZBIAS, 0);
//				m_Course.ms_pDevice->SetRenderState(D3DRS_ALPHABLENDENABLE, FALSE);
//				m_Course.ms_pDevice->SetTextureStageState(0, D3DTSS_ADDRESSU, D3DTADDRESS_WRAP);
//				m_Course.ms_pDevice->SetTextureStageState(0, D3DTSS_ADDRESSV, D3DTADDRESS_WRAP);
//#endif
#endif
				break;
		}
	}

#ifndef IGNOREFORNOW

	//------------------------------------
	// draw the trees for this section:
	//------------------------------------
	
	if (m_pTreeArr)  {
		Sprite **ppsprite = m_pTreeArr;
		for (int i = 0; i < m_TreeCount; i++,ppsprite++)  {
			if (*ppsprite)  {
				(*ppsprite)->Render();
			}
		}
	}

	// Draw the signs.
	//Set3DRenderState();
	//ms_pDevice->SetRenderState(D3DRS_AMBIENT, D3DXCOLOR(0.8f, 0.8f, 0.8f, 1));
	for (SignData*psign = m_SignList.GetFirst(); psign; psign = psign->Next())  {
		if (psign->ptex)
			psign->pmodel->ReplaceTexture((psign->pmodel==m_Course.m_pSign ? SIGN_TEX_NAME : "Banner.bmp"), psign->ptex);
		psign->pmodel->Render(psign->mat);
	}
	//Clear3DRenderState();

	/*
			D3DXMATRIX r;
			D3DXMATRIX m;
			static float up = 1.0f;
			D3DXMatrixRotationY( &r, m_StartRotation );
			D3DXMatrixTranslation( &m, m_StartLoc.x,m_StartLoc.y + up,m_StartLoc.z);
			D3DXMatrixMultiply( &m, &r, &m );
			m_Course.m_pSign->Render( m );
			*/

	//ms_pDevice->SetRenderState(D3DRS_ZENABLE,TRUE);
#endif
#endif
	return;

#endif
}

/************************************************************************************

************************************************************************************/
#ifndef IGNOREFORNOW

//tlm20050419, error in this function in failed to create application computer:

void CCourse::Section::RenderEditRoad()  {


	if (ReadyEditModel())  {
		//m_Course.ms_pDevice->SetStreamSource( 0, m_pVBufEdit, sizeof(COURSEVERTEX) );

		m_Course.ms_pDevice->SetTexture(0, m_Course.m_Tex[m_SpecialTexture]->GetTex());
		m_Course.ms_pDevice->DrawPrimitiveVB(D3DPT_TRIANGLESTRIP, m_pVBufEdit, 0, m_RoadPoints, 0);
		gStats.vertsrendered += m_RoadPoints;

		//m_Course.ms_pDevice->SetTexture( 0, NULL );
		//m_Course.ms_pDevice->DrawPrimitive( D3DPT_LINELIST, m_RoadPoints, 1 );
	}
}

/************************************************************************************

************************************************************************************/

void CCourse::Section::RenderEditTop()  {
	if (ReadyEditModel())  {
		//m_Course.ms_pDevice->SetStreamSource( 0, m_pVBufEdit, sizeof(COURSEVERTEX) );

		m_Course.ms_pDevice->DrawPrimitiveVB(D3DPT_TRIANGLESTRIP, m_pVBufEdit, m_RoadPoints + 2,
														 m_TopPoints, 0);
		gStats.vertsrendered += m_TopPoints;
	}
}
#endif
/************************************************************************************

************************************************************************************/

#ifndef IGNOREFORNOW
bool CCourse::Section::AddSign(const char *texname, float dist, IBillboardSceneNode *pmodel, bool center)  {
	if ((dist < m_StartDistance) || (dist > (m_StartDistance + m_Length)))
		return false;
	SignData *psign = new SignData();

	psign->seg = (int) ((dist - m_StartDistance) / m_DivisionLength);
	if (psign->seg >= m_Divisions - 1)
		psign->seg = m_Divisions - 2;
	if (psign->seg < 0)
		psign->seg = 0;

	psign->dist = m_StartDistance + psign->seg * m_DivisionLength;

	if (texname)
		psign->ptex = ms_TM.Open(texname);

	psign->pmodel = pmodel;
	psign->center = center;
	m_SignList.AddHead(*psign);
	CloseModel();
	return true;
}
#endif


/************************************************************************************

************************************************************************************/

HRESULT CCourse::Update(int advtime)  {
	Section *ps;
	for (ps = m_Track.GetFirst(); ps; ps = ps->Next())  {
		if (ps->m_FrameNum != m_FrameNum)  {
			ps->CloseModel();
		}
	}
	m_FrameNum++;

	return S_OK;
}




/************************************************************************************

************************************************************************************/

bool CCourse::Section::ReadyEditModel()  {
#ifndef IGNOREFORNOW
	if (m_pVBufEdit)
		return true;
	CloseModel();
	closeCArr();

	calcValues();													// tlm: this is changing m_StartWind!!!!

	int cnt = m_RoadPoints + m_TopPoints + 2;

	HRESULT hr;
	D3DVERTEXBUFFERDESC desc;
	desc.dwSize = sizeof(desc);
	desc.dwCaps = D3DVBCAPS_WRITEONLY | gVBufFlags;
	desc.dwFVF = FVF_COURSEVERTEX;
	desc.dwNumVertices = cnt;

	hr = gpD3D->CreateVertexBuffer(&desc, &m_pVBufEdit, 0);
	if (hr != S_OK)
		return false;

	COURSEVERTEX *pvdest;
	hr = m_pVBufEdit->Lock(DDLOCK_WRITEONLY, (LPVOID *) &pvdest, 0);
	if (hr != S_OK)  {
		CloseModel();
		return false;
	}

	vector3df loc, vec;
	float dist, grade;

	int i;
	int d = m_RoadPoints / 2;
	float len = m_Length / (d - 1);
	dist = m_StartDistance;

	COURSEVERTEX v1, v2;
	v1.diffuse = v2.diffuse = m_RoadColor;
	v1.tu1 = 0.0f;
	v2.tu1 = 1.0f;

	float repeat = stripArr[lSTRIPS_H].texrepeat;
	float w;

	float tv;
	float tadd;

	float ewind = GetWind();
	float wind = m_StartWind;
	float wadd;

	if (m_SpecialTexture == TEX_ROAD)  {
		w = editRoadWidth;
		tv = m_StartDistance / repeat;
		tv = tv - ((int) tv);
		float tvend = tv + (m_Length / repeat);
		tadd = (tvend - tv) / d;
	}
	else  {
		w = editSpecialWidth;
		tv = 1.0f;
		tadd = -1.0f / (d - 1);
	}

	for (i = 0;
		  i < d;
		  i++,dist += len,tv += tadd)  {
		RoadLoc(NULL, dist, loc, vec, grade);

		v1.v = loc - vec * w;
		v2.v = loc + vec * w;
		v1.tv1 = v2.tv1 = tv;
		*pvdest++ = v1;
		*pvdest++ = v2;
	}

	v1.v = loc;
	v1.diffuse = RGBAToSColor(0, 0, 0, 1);
	*pvdest++ = v1;
	v1.v.y = m_Course.m_GridMin.y;
	*pvdest++ = v1;

	calcCArr();

	vector2df *pv = m_pCArr;
	vector2df *pv2 = m_pCArr + m_TopPoints;
	v1.diffuse = m_TopColor;
	v1.v.y = -0.1f;
	wadd = (ewind - wind) / ((m_TopPoints / 2) - 1);

	for (i = 0;
		  i < m_TopPoints / 2;
		  i++,wind += wadd)  {
		if ((m_EditMode == NORMAL) || (m_EditMode == EDIT))
			v1.diffuse = v2.diffuse = windColor(wind);

		v1.v.x = pv->x;
		v1.v.z = pv->y;
		*pvdest++ = v1;
		pv++;
		pv2--;
		v1.v.x = pv2->x;
		v1.v.z = pv2->y;
		*pvdest++ = v1;
	}

	m_pVBufEdit->Unlock();
#endif
	return true;
}

/************************************************************************************

************************************************************************************/

#ifndef IGNOREFORNOW
bool CCourse::AddSign(const char *texname, float dist, IBillboardSceneNode *pmodel, bool center)  {
	Section *ps;
	for (ps = m_Track.GetFirst();
		  ps;
		  ps = ps->Next())  {
		if ((dist >= ps->m_StartDistance) && (dist < (ps->m_StartDistance + ps->m_Length)))  {
			return ps->AddSign(texname, dist, pmodel, center);
		}
	}
	return false;
}
#endif


void CCourse::Section::Show(bool bShow)
{
	if(gpCourse != NULL && m_pSectionNode != 0)
	{
		if(bShow)
		{
			if(m_pSectionNode->getParent() != gpCourse->m_pNodeVisible)
			{
				gpCourse->m_pNodeVisible->addChild(m_pSectionNode);
				AddRender();
			}
		}
		else
		{
			if (m_pSectionNode->getParent() != gpCourse->m_pNodeHidden)
				gpCourse->m_pNodeHidden->addChild(m_pSectionNode);
		}
	}
}

/************************************************************************************
	called 3 times during buildterrain()
	then called by Render(), but only executed the first time render() calls it
************************************************************************************/

bool CCourse::Section::ReadyModel(bool doends)  {
	HRESULT hr=0;

	//tlm20060516+++
	// this limits the number of m_Divisions of a section, eg, importing a 20 mile single leg from a .crs file
	static COURSEVERTEX cv[1024 * 32];		//	= new COURSEVERTEX[ m_Divisions * (STRIPS) * 2 + m_TreeCount * 4 ];
	static WORD idx[1024 * 32];				// = new WORD[ m_Divisions * STRIPS * 3 + m_TreeCount * 4];
	static short cmd[600 + (50 * 4)];


	COURSEVERTEX *mcv = cv;
	COURSEVERTEX *pcvtemp = cv;

	STRIP_INFO *psinfo;
	float grade;
	vector3df loc, vec;
	int i;
	COURSEVERTEX *pv1;
	COURSEVERTEX *pv2;
	COURSEVERTEX *pv3;


	if (!m_Course.m_bBuildingEdgeMesh)  {
		// If we are not building an edge mesh - then we need to close all models.
		// secondarlay we can build if already exists then we can just return;
		if(bMeshCreated)
			return true;			// <<<<<<<<<<<<<<<<<<< USUALLY RETURNS HERE <<<<<<<<<<<<<<<<<<<<<

		//if (m_pVBuf)  {		// Is the model already up?
		//	return true;			// <<<<<<<<<<<<<<<<<<< USUALLY RETURNS HERE <<<<<<<<<<<<<<<<<<<<<
		//}
	}


	if (doends)  {
		bool le = m_bLeftEdge;
		bool re = m_bRightEdge;
		m_bLeftEdge = m_bRightEdge = true;
		calcValues();						// computes m_TreeCount for this section
		m_bLeftEdge = le;
		m_bRightEdge = re;
	}
	else  {
		calcValues();			// changes stripArr[0].offset! computes m_TreeCount for this section
	}

	StripData sdata[MAX_STRIPS];

	short *pcmd = cmd;
	WORD *pidx = idx;
	WORD *pstartidx;

	// Is the start data ready to go?

	Section *ps = Prev();

	if (ps)  {
		if (!ps->m_EndStripData)  {
			ps->ReadyModel();
		}

		for (i = 0; i < lSTRIPS; i++)
			cv[i * m_Divisions] = ps->m_EndV[i + lSTRIP_START];

		memcpy(sdata, ps->m_EndStripData, sizeof(ps->m_EndStripData));
		m_LastTreePattern = ps->m_LastTreePattern;
		m_StartLeft = ps->m_EndLeft;
		m_StartRight = ps->m_EndRight;
	}
	else  {
		RoadLoc(NULL, m_StartDistance, loc, vec, grade);
		for (i = 0; i < MAX_STRIPS; i++)  {
			sdata[i].color = 0.6f;
			sdata[i].count = 2;
			sdata[i].ydelta = 0.01f; // 0.02f;
			sdata[i].addy = 0.0f;
			sdata[i].updown = 1;
			sdata[i].n = 0;
		}

		for (i = 0,psinfo = stripArr; i < lSTRIPS; i++,psinfo++)  {
			cv[i * m_Divisions].v = loc + vec * psinfo->offset;
			cv[i * m_Divisions].color = 0.6f;
		}
		m_LastTreePattern = 0;
		m_StartLeft = -gMaxBoarder;
		m_StartRight = gMaxBoarder;
	}

	m_EndLeft = (m_DegRotation < 0.0f ? -m_Radius : m_StartLeft - m_Length);
	m_EndRight = (m_DegRotation > 0.0f ? m_Radius : m_StartRight - m_Length);

	Section *pnext = Next();
	if (pnext)  {
		float t = (m_DegRotation < 0.0f ? -pnext->m_Radius : -gMaxBoarder);
		if (t > m_EndLeft)
			m_EndLeft = t;
		t = (m_DegRotation > 0.0f ? pnext->m_Radius : gMaxBoarder);
		if (t < m_EndRight)
			m_EndRight = t;
	}

	if (m_EndLeft < -gMaxBoarder)
		m_EndLeft = -gMaxBoarder;

	if (m_EndRight > gMaxBoarder)
		m_EndRight = gMaxBoarder;

	// Generate the points for all the strips.

	float color=0.6f;
	float len = m_Length / (m_Divisions - 1);
	float dist = m_StartDistance + len;
	int j;
	COURSEVERTEX *pcv;

	float left = m_StartLeft;
	float right = m_StartRight;
	float leftadd = (m_EndLeft - m_StartLeft) / (m_Divisions - 1);
	float rightadd = (m_EndRight - m_StartRight) / (m_Divisions - 1);

	srand((int) (m_StartDistance * 2.3512));	// So we are the same every time this piece is generated.

	for (i=1; i<m_Divisions; i++, dist+=len)  {
		RoadLoc(NULL, dist, loc, vec, grade);
		color = 0.6f;
		if (grade > 0.0f)
			color += grade * (100.0f / 40.0f);

		psinfo = stripArr;
		pcv = cv + i;

		// ===== GENERAL ====

		for (j = 0; j < lSTRIPS; j++, psinfo++,pcv += m_Divisions)  {
			pcv->v = loc + vec * psinfo->offset;
			pcv->color = color + psinfo->coloradd;
			pcv->v.y += frand(psinfo->rndlow, psinfo->rndhigh);
		}

	}

	// SMOOTH HERE

	if (m_bLeftEdge || doends)  {
		srand((int) (m_StartDistance * 3.14));	// So we are the same every time this piece is generated.
		bez((lSTRIPS_H - 3), sdata[lSTRIPS_H - 3], cv);
		for (i = 1; i<m_Divisions; i++)
			cv[CVNUM(i, (lSTRIPS_H - 4))].v.y = cv[CVNUM(i, (lSTRIPS_H - 3))].v.y; // ptstrip[n+1].vertex.y =ptstrip[n+2].vertex.y;
		bez((lSTRIPS_H - 4), sdata[lSTRIPS_H - 4], cv);

		for (i = 1; i < m_Divisions; i++)
			cv[CVNUM(i, (lSTRIPS_H - 5))].v.y = cv[CVNUM(i, (lSTRIPS_H - 4))].v.y; // ptstrip[n+0].vertex.y =ptstrip[n+1].vertex.y;
		bez((lSTRIPS_H - 5), sdata[lSTRIPS_H - 5], cv); //		bez(0);
	}
	else
		cv[CVNUM(m_Divisions - 1, 0)] = m_EndV[3];

	if (m_bRightEdge || doends)  {
		srand((int) (m_StartDistance * 18.93));	// So we are the same every time this piece is generated.
		bez((lSTRIPS_H + 2), sdata[lSTRIPS_H + 2], cv);	// bez(8)
		for (i = 1; i < m_Divisions; i++)
			cv[CVNUM(i, (lSTRIPS_H + 3))].v.y = cv[CVNUM(i, (lSTRIPS_H + 2))].v.y; //	ptstrip[n+9].vertex.y =ptstrip[n+8].vertex.y;
		bez((lSTRIPS_H + 3), sdata[lSTRIPS_H + 3], cv); // bez(9);
		for (i = 1; i < m_Divisions; i++)
			cv[CVNUM(i, (lSTRIPS_H + 4))].v.y = cv[CVNUM(i, (lSTRIPS_H + 3))].v.y; // ptstrip[n+10].vertex.y =ptstrip[n+9].vertex.y;
		bez((lSTRIPS_H + 4), sdata[lSTRIPS_H + 4], cv); //		bez(10);
	}
	else
		cv[CVNUM(m_Divisions - 1, lSTRIPS_H + 1)] = m_EndV[6];

	if (!Next())  {
		// Set all y's to the save value on the last number.

		float y = cv[CVNUM(m_Divisions - 1, lSTRIPS_H)].v.y;
		for (i = 1; i < lSTRIPS - 1; i++)  {
			pv1 = cv + CVNUM((m_Divisions - 1), i);
			pv1->v.y = y;
			pv1->color = 0.6f;
		}
	}

	// Liniarize the End edges.
	/*
		static edgeLength = 10.0f;
		int smoothdiv = (int)(m_Length / edgeLength);
		if (smoothdiv < 2)
				smoothdiv = 2;
		smooth( cv, smoothdiv, m_Divisions );
		smooth( cv + CVNUM(0,STRIPS-1),smoothdiv,m_Divisions);
		*/

	// Copy data back to the End V

	if (doends)  {
		for (i = 0; i < lSTRIPS; i++)  {
			m_EndV[i] = cv[i * m_Divisions + m_Divisions - 1];
			m_EndStripData[i] = sdata[i];
		}
		m_EndStripValid = true;

		// Copy it back just to make sure.

		ps = Prev();
		if (ps)  {
			for (i = 0; i < lSTRIPS; i++)
				cv[i * m_Divisions] = ps->m_EndV[i];
		}
		return false;
	}

	ps = Prev();
	pv1 = cv;
	pv2 = pv1 + (m_Divisions - 1);
	for (i = 0; i < lSTRIPS; i++,pv1 += m_Divisions,pv2 += m_Divisions)  {
		if (ps)
			*pv1 = ps->m_EndV[i + lSTRIP_START];
		*pv2 = m_EndV[i + lSTRIP_START];
	}

	// Calculate more acurate extentions.

	m_ExtMin = m_ExtMax = cv[0].v;
	for (pv1 = cv; pv1 != pcv; pv1++)  {
		extRect3(m_ExtMin, m_ExtMax, pv1->v);
	}
	m_ExtOrigin = (m_ExtMin + m_ExtMax) / 2;
	vector3df v = m_ExtMax - m_ExtOrigin;
	m_ExtRadius = v.getLength() + 100.0f;

	// We don't need to go any farther now if we are just building the mesh.


	if (m_Course.m_bBuildingEdgeMesh)  {
		for (pv1 = cv; pv1 != pcv; pv1++)  {
			pv1->pdata = (void *)this;
		}

		// We got everyting we need for the edge points at this points.  Too bad everything
		// needed to be calculated.

		ps = Prev();
		if (!ps && m_Course.m_bLoopClosed)
			ps = m_Course.m_Track.GetLast();
		if (ps)  {
			if (m_bLeftEdge && !ps->m_bLeftEdge)  {
				pLeftEdge->push_back(cv[CVNUM(0, 2)]);
				pLeftEdge->push_back(cv[CVNUM(0, 1)]);
				pLeftEdge->push_back(cv[CVNUM(0, 0)]);
			}
			if (m_bRightEdge && !ps->m_bRightEdge)  {
				pRightEdge->push_back(cv[CVNUM(0, lSTRIPS_H + 2)]);
				pRightEdge->push_back(cv[CVNUM(0, lSTRIPS_H + 3)]);
				pRightEdge->push_back(cv[CVNUM(0, lSTRIPS_H + 4)]);
			}
		}
		else  {
			if (m_bLeftEdge)  {
				pLeftEdge->push_back(cv[CVNUM(0, 4)]);
				pLeftEdge->push_back(cv[CVNUM(0, 3)]);
				pLeftEdge->push_back(cv[CVNUM(0, 2)]);
				pLeftEdge->push_back(cv[CVNUM(0, 1)]);
				pLeftEdge->push_back(cv[CVNUM(0, 0)]);
			}
			else  {
				pLeftEdge->push_back(cv[CVNUM(0, 1)]);
				pLeftEdge->push_back(cv[CVNUM(0, 0)]);
			}
			if (m_bRightEdge)  {
				pRightEdge->push_back(cv[CVNUM(0, lSTRIPS_H)]);
				pRightEdge->push_back(cv[CVNUM(0, lSTRIPS_H + 1)]);
				pRightEdge->push_back(cv[CVNUM(0, lSTRIPS_H + 2)]);
				pRightEdge->push_back(cv[CVNUM(0, lSTRIPS_H + 3)]);
				pRightEdge->push_back(cv[CVNUM(0, lSTRIPS_H + 4)]);
			}
			else  {
				pRightEdge->push_back(cv[CVNUM(0, lSTRIPS_H)]);
				pRightEdge->push_back(cv[CVNUM(0, lSTRIPS_H + 1)]);
			}
		}
		pv1 = cv + CVNUM(1, 0);
		pv2 = cv + CVNUM(1, lSTRIPS - 1);
		for (i = 1; i < m_Divisions; i++)  {
			pLeftEdge->push_back(*pv1++);
			pRightEdge->push_back(*pv2++);
		}
		ps = Next();
		if (!ps && m_Course.m_bLoopClosed)
			ps = m_Course.m_Track.GetFirst();
		int div = m_Divisions - 1;
		if (ps)  {
			if (m_bLeftEdge && !ps->m_bLeftEdge)  {
				pLeftEdge->push_back(cv[CVNUM(div, 0)]);
				pLeftEdge->push_back(cv[CVNUM(div, 1)]);
				pLeftEdge->push_back(cv[CVNUM(div, 2)]);
				pLeftEdge->push_back(cv[CVNUM(div, 3)]);
			}
			if (m_bRightEdge && !ps->m_bRightEdge)  {
				pRightEdge->push_back(cv[CVNUM(div, lSTRIPS_H + 4)]);
				pRightEdge->push_back(cv[CVNUM(div, lSTRIPS_H + 3)]);
				pRightEdge->push_back(cv[CVNUM(div, lSTRIPS_H + 2)]);
				pRightEdge->push_back(cv[CVNUM(div, lSTRIPS_H + 1)]);
			}
		}
		else  {
			if (m_bLeftEdge)  {
				pLeftEdge->push_back(cv[CVNUM(div, 1)]);
				pLeftEdge->push_back(cv[CVNUM(div, 2)]);
				pLeftEdge->push_back(cv[CVNUM(div, 3)]);
				pLeftEdge->push_back(cv[CVNUM(div, 4)]);
			}
			else
				pLeftEdge->push_back(cv[CVNUM(div, 1)]);
			if (m_bRightEdge)  {
				pRightEdge->push_back(cv[CVNUM(div, lSTRIPS_H + 3)]);
				pRightEdge->push_back(cv[CVNUM(div, lSTRIPS_H + 2)]);
				pRightEdge->push_back(cv[CVNUM(div, lSTRIPS_H + 1)]);
				pRightEdge->push_back(cv[CVNUM(div, lSTRIPS_H)]);
			}
			else
				pRightEdge->push_back(cv[CVNUM(div, lSTRIPS_H)]);
		}

		//delete[] cv;
		//delete[] cmd;
		//delete[] idx;

		return true;
	}

	if(!m_Course.m_pSceneNode || !m_Course.m_pNodeHidden || !m_Course.m_pNodeVisible)
		return false;

	if(!m_pSectionNode)
	{
		m_bRender = false;
		m_pSectionNode = D3DBase::addCustomSceneNode(m_Course.m_pNodeHidden);
		//m_pSectionNode = D3DBase::addCustomSceneNode(m_Course.m_pNodeVisible);
		if(!m_pSectionNode)
			return false;
	}

	// Create the geometry.

	pcv = cv + lSTRIPS * m_Divisions;

	int minv, maxv;

	/*
	from scene.txt:

	 -28.00, 8, -4.0, 4.00, 64.0, 0.00, 0		64
	 -16.00, 4, -2.0, 2.00, 64.0, 0.00, 1		64
	  -8.00, 4,  0.0, 0.01, 64.0, 0.00, 1		64 left strip 3, tex_edge_3_l
	  -3.00, 1,  0.0, 0.01, 32.0, 0.00, 0		32 left strip 2, tex_edge_2_l
	  -1.75, 1,  0.0, 0.00, 16.0,  .25, 0		16 road, left & right road edges (strip 1), small repeats --> more repetition

	; center of road (theoretically)

		1.75, 1,  0.0, 0.00,  1.0, 0.25, 0		repetition NOT USED
		3.00, 1,  0.0, 0.01,  1.0, 0.00, 0		repetition NOT USED
		8.00, 4,  0.0, 0.01, 64.0, 0.00, 1		right strip 2, tex_edge_2_r
	  16.00, 4, -2.0, 2.00, 64.0, 0.00, 1		right strip 3 (uses tex_edge_3_r)
	  28.00, 8, -4.0, 4.00, 64.0, 0.00, 1		right strip 4 (& 5?!) (dark purple, also dark blue strip overlaid!, varies with repeat value)


		static STRIP_INFO stripArrBase[MAX_STRIPS] = {
			
		//		offset  div  low   high    repeat cadd  treeok

			{ - 28.00f, 8, -4.0f, 4.00f, 64.0f, 0.00f, false },		// 64 TEX_EDGE_5_L
			{ - 16.00f, 4, -2.0f, 2.00f, 64.0f, 0.00f, true  },		// 64 TEX_EDGE_4_L
			{   -8.00f,	4,  0.0f, 0.01f, 64.0f, 0.00f, true  },		// 64 TEX_EDGE_3_L
			{   -3.00f,	1,  0.0f, 0.01f, 32.0f, 0.00f, false },		// 32 TEX_EDGE_2_L
			{   -1.75f, 1,	 0.0f, 0.00f, 16.0f, 0.25f, false },		// 16 TEX_EDGE_1_L
			// center of road
			{ 	  1.75f, 1,	 0.0f, 0.00f, 16.0f, 0.25f, false },		// 16 TEX_EDGE_1_R
			{ 	  3.00f, 1,	 0.0f, 0.01f, 32.0f, 0.00f, false },		// 32 TEX_EDGE_2_R
			{ 	  8.00f, 4,	 0.0f, 0.01f, 64.0f, 0.00f, true  },		// 64 TEX_EDGE_3_R
			{	 16.00f,	4, -2.0f, 2.00f, 64.0f, 0.00f, true  },		// 64 TEX_EDGE_4_R
			{	 28.00f,	8, -4.0f, 4.00f, 64.0f, 0.00f, true  },  		// 64 TEX_EDGE_5_R


		};


	*/


	//----------------------------------------------------
	// the road itself texturing is controlled here
	//----------------------------------------------------

	int cnt = -1;
	// lSTRIPS_H = 2, uses stripArrBase[4] for both road edges
	// stripArr == stripArrBase[3]

	for (i=lSTRIPS_H-2; i<lSTRIPS_H+2; i++, cnt++)  {
		reStrip(cv + m_Divisions*i, NULL, m_Divisions, cnt, stripArr[lSTRIPS_H-1].texrepeat);
	}

	//-------------------
	// left road half:
	//-------------------

	pstartidx = pidx;
	buildStrip(pidx, minv, maxv, m_Divisions * (lSTRIPS_H - 2), m_Divisions * (lSTRIPS_H - 1), m_Divisions);
	cmd_addTex(pcmd, TEX_EDGE_1_L);
	cmd_addStrip(pcmd, minv, maxv, pstartidx - idx, pidx - idx, pstartidx);

	//-------------------
	// right road edge:
	//-------------------

	pstartidx = pidx;
	cmd_addTex(pcmd, TEX_EDGE_1_R);
	buildStrip(pidx, minv, maxv, m_Divisions * lSTRIPS_H, m_Divisions * (lSTRIPS_H + 1), m_Divisions);
	cmd_addStrip(pcmd, minv, maxv, pstartidx - idx, pidx - idx, pstartidx);

	//-------------------------------------------------------------------
	// Road
	// We may need to re-tu/tv the road if this is a speical texture.
	//-------------------------------------------------------------------

	int s1 = m_Divisions * (lSTRIPS_H - 1);
	int s2 = m_Divisions *lSTRIPS_H;
	pv1 = cv + s1;
	pv2 = cv + s2;

	if (m_SpecialTexture != TEX_ROAD)  {
		ALLOC_CV(s1, pv1, m_Divisions);
		ALLOC_CV(s2, pv2, m_Divisions);
		reStrip(pv1, cv + m_Divisions * (lSTRIPS_H - 1), m_Divisions, 0.0f, 0.0f);
		reStrip(pv2, cv + m_Divisions * lSTRIPS_H, m_Divisions, 1.0f, 0.0f);
	}

	pstartidx = pidx;
	buildStrip(pidx, minv, maxv, s1, s2, m_Divisions);
	cmd_addTex(pcmd, m_SpecialTexture);
	cmd_addStrip(pcmd, minv, maxv, pstartidx - idx, pidx - idx, pstartidx);


	//------------------------------
	// construct the left strips:
	//------------------------------

	short tc = TEX_EDGE_2_L;

	for (i = (lSTRIPS_H - 2); i > 0; i--, tc+=2)  {

		ALLOC_CV(s1, pv1, m_Divisions);	// Restrip both - need to make a new strip for 1 of them.
		s2 = m_Divisions * (i - 1);
		pv2 = cv + s2;
		reStrip(pv1, cv+m_Divisions*i, m_Divisions, 0.0f, stripArr[i].texrepeat);		// uses stripArrBase[3...1] for texrepeat
		reStrip(pv2, NULL, m_Divisions, 1.0f, stripArr[i].texrepeat);

		pstartidx = pidx;
		cmd_addTex(pcmd, tc);
		buildStrip(pidx, minv, maxv, s2, s1, m_Divisions);
		cmd_addStrip(pcmd, minv, maxv, pstartidx - idx, pidx - idx, pstartidx);
	}

	//------------------------------
	// construct the right strips:
	//------------------------------

	tc = TEX_EDGE_2_R;

	// here stripArr == stripArrBase
	// doesn't draw TEX_EDGE_5_R strip!!! Why???
	// lSTRIPS_H = 5
	// streipArr = stripArrBase[0]

	for (i = lSTRIPS_H + 2; i < lSTRIPS; i++,tc += 2)  {
	//for (i = lSTRIPS_H + 1; i < lSTRIPS; i++,tc += 2)  {								// tlm, lSTRIPS_H = 5, lSTRIPS = 10
		ALLOC_CV(s1, pv1, m_Divisions);			// Restrip both - need to make a new strip for 1 of them.
		s2 = m_Divisions * i;
		pv2 = cv + s2;

		reStrip(pv1, cv + m_Divisions*(i-1), m_Divisions, 0.0f, stripArr[i].texrepeat);		// uses stripArrBase[7...9] for texrepeat

		// we can control the left-right repetition here by adjusting the 4th parameter. This should for now be tied
		// to ...texrepeat
		//reStrip(pv2, NULL, m_Divisions, 1.0f, stripArr[i].texrepeat);			// was 1.0f
		reStrip(pv2, NULL, m_Divisions, stripArr[i].x_repeat, stripArr[i].texrepeat);			// was 1.0f

		pstartidx = pidx;
		cmd_addTex(pcmd, tc);
		buildStrip(pidx, minv, maxv, s1, s2, m_Divisions);
		cmd_addStrip(pcmd, minv, maxv, pstartidx - idx, pidx - idx, pstartidx);

		if (tc > TEX_EDGE_5_R)
			tc = TEX_EDGE_5_R;
	}



//#ifndef IGNOREFORNOW
	//=========================================================================================
	// TREES
	//=========================================================================================

	// tlm20060517+++
	// don't do this because the timeGetTime() will return the same number for 10-15 ms!!!!
//	srand(timeGetTime());			// tlm20051117, added so that we'd get a random tree layout
	// tlm20060517---


	// m_TreeCount is calcuated in calcValues()

//	if ((m_TreeCount > 0) && !m_SignList.GetFirst())  {	 // Don't do trees if there is a sign.

		int okpos[MAX_STRIPS];
		int okcount = 0;

		for (i=0; i<lSTRIPS; i++)  {
			if (stripArr[i].treeok)  {
				float lsq = (cv[CVNUM(0, i)].v - cv[CVNUM(1, i)].v).getLengthSQ();
				if (lsq >= (1.5f * 1.5f))  {
					okpos[okcount++] = i;
				}
			}
		}


		if (okcount > 0)  {
			m_pTreeArr = new IBillboardSceneNode *[m_TreeCount];
			*pcmd++ = RINS_TREESTART;

			int *inpos = new int[m_Divisions];
			memset(inpos, 0, sizeof(int) * m_Divisions);

			// select a random tree group for this section (we probably only have 1, sice we only have 4 trees anyway!!!

			tree_group_index = rand() % treeGroupCount;
			TreeGroupData &tg = treeGroup[tree_group_index];		// tg is either (0, 2) or (2, 2) (start, count)

			int tree_index = -1;

			//--------------------------------------------------------
			// select a random tree for each tree in this section:
			//--------------------------------------------------------

			for (i=0; i<m_TreeCount; i++)  {
				int d = rand() % (m_Divisions - 1);
				if (inpos[d] != 0)  {
					m_pTreeArr[i] = NULL;
					continue;
				}

				tree_index = tg.start + (rand() % tg.count);

				assert(tree_index>=0 && tree_index <= 3);		// since we only have 4 trees and only one group

				//TreeData &td = treeArr[tg.start + (rand() % tg.count)];
				TreeData &td = treeArr[tree_index];			// pick the random tree
				//m_pTreeArr[i]->Inactivate();

				//Sprite &ts = *m_pTreeArr[i];


				int pos = rand() % okcount;
				inpos[d] = pos;
				if (((d > 0) && (inpos[d - 1] == pos)) || (inpos[d + 1] == pos))  {
					pos += ((rand() & 1) ? 1 : -1);
					if (pos < 0)
						pos = okcount - 1;
					else if (pos >= okcount)
						pos = 0;
				}
				int strip = okpos[pos];			// <<<<<<<<<<<<<<<<<

				float len = stripArr[strip - 1].offset - stripArr[strip].offset;
				pv1 = cv + CVNUM(i, strip - 1);
				pv2 = cv + CVNUM(i, strip);

				/*
					float width;						controls the width of the tree
					float height;						controls the heigth of the tree
					float w_adj_min;					random variation range of tree width (tree width +/- [w_adj_min, w_adj_max] )
					float w_adj_max;
					float h_adj_min;					random variation range of tree height (tree height +/- [h_adj_min, h_adj_max] )
					float h_adj_max;

					float scale_adj;					1/2 the random variation amount from 1.0 scale (+ or -), controls the global scaling for the tree
															further scale by other random numbers, small changes here may not always be noticeable.
					float offsetx;						controls placement of the tree relative to the tree shadow. Ie, the shadow is fixed, but you
															can adjust the tree left-right to make it 'fit' the shadow.
					float offsety;						NOT USED
					int tex;								which tree texture to use (1-4)
					float shadow_width_scale;		controls width of shadow
					float shadow_height_scale;		controls heigth of shadow
				*/

				tc = td.tex; // TEX_TREE1;					// tc is just a texture index
				float ox = td.offsetx;






				float width		= td.width + frand(td.w_adj_min, td.w_adj_max );
				float height	= td.height + frand(td.h_adj_min, td.h_adj_max );
				float scale		= (1+td.scale_adj/2.0f)-frand(0.0f,td.scale_adj);
				float variance1 = -0.5f + frand(0.0f,1.0f);
				float variance2 = -0.5f + frand(0.0f,1.0f);
				float w = (width + variance2)*scale;
				float h = (height + variance2)*scale;

				float r = degToRad(frand(-0.5f,0.5f));
				color = 0.6f + frand(-0.2f,0.2f);
				int flipped = (rand() % 9)/5;
				float flipx2 = ((flipped == 0) ? 0.98f : 0.01f);
				float flipx1 = ((flipped == 0) ? 0.01f : 0.98f);
				
				/*
				//vector3df v = pv2->v + (pv1->v - pv2->v)*(ox/len);
				vector3df vt = pv2->v + (pv1->v - pv2->v)*(ox/len);
				vector3df v(vt.x, (vt.y+h/2) - frand(0.15f,1.0f), vt.z);
				//vector3df v(vt.x + variance1 * 1.5, (vt.y+h/2) - frand(0.15f,1.0f), vt.z);
				*/
				vector3df vt = pv2->v + (pv1->v - pv2->v)*(ox/len);
				vector3df v(vt.x, vt.y - frand(0.05f,0.5f), vt.z);
				//vector3df v = pv2->v + (pv1->v - pv2->v)*(ox/len);

				// Create the billboard tree
				SColor clr(255,255*color,255*color,255*color);
				CSpriteSceneNode *csnode = new CSpriteSceneNode(m_pSectionNode, D3DBase::GetSceneManager(), -1, 
					v*mult, dimension2d<f32>(w,h)*mult,rect<f32>(flipx1,0.01f,flipx2,0.98f),
					clr,clr);
				csnode->drop();
				csnode->setMaterialFlag(video::EMF_LIGHTING, false);
				csnode->setMaterialTexture(0, D3DBase::GetDriver()->getTexture(m_Course.m_Tex[tc]));
				csnode->setMaterialType(video::EMT_TRANSPARENT_ALPHA_CHANNEL);
				csnode->getMaterial(0).MaterialTypeParam = 0.1f;

				m_pTreeArr[i] = csnode;

				// Add the tree shadow
				float sx = -td.shadow_width_scale * w;
				float sy = td.shadow_height_scale * h;

				float ux = len / sx;
				float uxs = 0.0f;
				float rx = (sx - len) + td.offsetx;
				if (rx > 0.0f)  {			// Note that both are negative.
					float rm = frand( 0.0f, rx);
					float mv = (ux - 1.0f) * (rm / rx);
					ux -= mv;
					uxs -= mv;
					// uxs = ;
					ox -= rm;
				}
				//if (sx < len)
				//	sx = len;
				if (sy > m_DivisionLength)
					sy = m_DivisionLength;

				cmd_addTex( pcmd, tc+1 );		// The shadow texture
				COURSEVERTEX *psv;
				ALLOC_CV( s1, psv, 4 );
				sy = m_DivisionLength / sy;
				float yadd = 0.1f;
				if (ux < 1.0f)
					ux = 1.0f;
				if (sy < 1.0f)
					sy = 1.0f;

				psv[0] = pv1[0]; psv[0].v.y += yadd;		psv[0].tu1 = ux;  psv[0].tv1 = 0.0f;
				psv[1] = pv1[1]; psv[1].v.y += yadd;		psv[1].tu1 = ux;  psv[1].tv1 = sy;
				psv[2] = pv2[0]; psv[2].v.y += yadd;		psv[2].tu1 = uxs; psv[2].tv1 = 0.0f;
				psv[3] = pv2[1]; psv[3].v.y += yadd;		psv[3].tu1 = uxs; psv[3].tv1 = sy;
#ifndef IGNOREFORNOW
				float ox = td.offsetx;
				//float oy = td.offsety;


//#ifdef N915
				// WW: Modified N915: Fix for the tree shadows.
				// ============================================
				float width		= td.width + frand(td.w_adj_min, td.w_adj_max );
				float height	= td.height + frand(td.h_adj_min, td.h_adj_max );
				float scale		= (1+td.scale_adj/2.0f)-frand(0.0f,td.scale_adj);
				float variance1 = -0.5f + frand(0.0f,1.0f);
				float variance2 = -0.5f + frand(0.0f,1.0f);
				float w = (width + variance1)*scale;
				float h = (height + variance2)*scale;

				float r = degToRad(frand(-0.5f,0.5f));
				float color = pv1->color + 0.3f;
				if (color > 1.0f)
					color = 1.0f;

				float sx = -td.shadow_width_scale * w;
				float sy = td.shadow_height_scale * h;

				float ux = len / sx;
				float uxs = 0.0f;
				float rx = (sx - len) + td.offsetx;
				if (rx > 0.0f)  {			// Note that both are negative.
					float rm = frand( 0.0f, rx);
					float mv = (ux - 1.0f) * (rm / rx);
					ux -= mv;
					uxs -= mv;
					// uxs = ;
					ox -= rm;
				}
				//if (sx < len)
				//	sx = len;
				if (sy > m_DivisionLength)
					sy = m_DivisionLength;


				vector3df v = pv2->v + (pv1->v - pv2->v)*(ox/len);
				//v += (pv2[1].v - pv2[0].v)*(oy/m_DivisionLength);


				// ect - set trees to use z buffer
				//ts.Set(m_Course.m_Tex[tc],v.x,v.y-0.4f,v.z,w,h,false );
				//ts.SetTint( color,color,color,1 );
				//ts.SetRot( r );

				// Add the shadow
				cmd_addTex( pcmd, tc+1 );		// The shadow texture
				COURSEVERTEX *psv;
				ALLOC_CV( s1, psv, 4 );
				sy = m_DivisionLength / sy;
				float yadd = 0.01f;
				if (ux < 1.0f)
					ux = 1.0f;
				if (sy < 1.0f)
					sy = 1.0f;

				psv[0] = pv1[0];psv[0].v.y += yadd;		psv[0].tu1 = ux;psv[0].tv1 = 0.0f;
				psv[1] = pv1[1];psv[1].v.y += yadd;		psv[1].tu1 = ux;psv[1].tv1 = sy;
				psv[2] = pv2[0];psv[2].v.y += yadd;		psv[2].tu1 = uxs;psv[2].tv1 = 0.0f;
				psv[3] = pv2[1];psv[3].v.y += yadd;		psv[3].tu1 = uxs;psv[3].tv1 = sy;
				// =========================
				// WW: End modification N915
//#else
				/*
				float width = td.width + frand(td.w_adj_min, td.w_adj_max);
				float height = td.height + frand(td.h_adj_min, td.h_adj_max);
				float scale = (1 + td.scale_adj / 2.0f) - frand(0.0f, td.scale_adj);
				float variance1 = -0.5f + frand(0.0f, 1.0f);
				float variance2 = -0.5f + frand(0.0f, 1.0f);
				float w = (width + variance1) * scale;
				float h = (height + variance2) * scale;

				float r = degToRad(frand(-0.5f, 0.5f));
				float color = pv1->color + 0.3f;
				if (color > 1.0f)
					color = 1.0f;

				float sx = -td.shadow_width_scale *w;
				float sy = td.shadow_height_scale *h;

				float rx = 0.0f;
				if (sx > -len)  {
					rx = frand(0.0f, len + sx);
					ox += rx;
				}

				//if (sx < len)
				//	sx = len;

				if (sy > m_DivisionLength)
					sy = m_DivisionLength;

				vector3df v = pv2->v + (pv1->v - pv2->v) * (ox / len);
				v += (pv2[1].v - pv2[0].v) * (oy / m_DivisionLength);

				ts.Set(m_Course.m_Tex[tc], v.x, v.y - 0.4f, v.z, w, h);
				ts.SetTint(color, color, color, 1);
				ts.SetRot(r);

				// Add the shadow

				COURSEVERTEX *psv;
				ALLOC_CV(s1, psv, 4);
				sx = len / sx;
				sy = m_DivisionLength / sy;
				static float yadd = 0.0f;
				psv[0] = pv1[0];psv[0].v.y += yadd;psv[0].tu1 = sx  ;psv[0].tv1 = 0.0f;
				psv[1] = pv1[1];psv[1].v.y += yadd;psv[1].tu1 = sx  ;psv[1].tv1 = sy;
				psv[2] = pv2[0];psv[2].v.y += yadd;psv[2].tu1 = 0.0f;psv[2].tv1 = 0.0f;
				psv[3] = pv2[1];psv[3].v.y += yadd;psv[3].tu1 = 0.0f;psv[3].tv1 = sy;
				*/
//#endif

				/*
					static COURSEVERTEX cv[1024 * 32];		//	= new COURSEVERTEX[ m_Divisions * (STRIPS) * 2 + m_TreeCount * 4 ];
					static WORD idx[1024 * 32];				// = new WORD[ m_Divisions * STRIPS * 3 + m_TreeCount * 4];
					static short cmd[600 + (50 * 4)];
					id is the id number of this section (0 ... n_sections-1 )
				*/

#endif

				cmd_addTex(pcmd, tc + 1);
				pstartidx = pidx;
				buildStrip(pidx, minv, maxv, s1, s1 + 2, 2);
				cmd_addStrip(pcmd, minv, maxv, pstartidx - idx, pidx - idx, pstartidx);
			}			// for (i=0; i<m_TreeCount; i++)

			*pcmd++ = RINS_TREEEND;
			delete[] inpos;
		}
//	}					// doing trees

#ifndef IGNOREFORNOW
	// Place The Signs.
	SignData *psign;

	for (psign = m_SignList.GetFirst(); psign; psign = psign->Next())  {
		int strip;
		if (psign->center)
			strip = lSTRIPS_H;
		else  {
			if (m_DegRotation < -3.0f)
				strip = lSTRIPS_H - 2;
			else if (m_DegRotation > 3.0f)
				strip = lSTRIPS_H + 2;
			else
				strip = ((rand() % 10) < 5 ? lSTRIPS_H - 2 : lSTRIPS_H + 2);
		}
		pv1 = cv + CVNUM(psign->seg, strip - 1);
		pv2 = cv + CVNUM(psign->seg, strip);
		vector3df v = pv2->v + (pv1->v - pv2->v) / 2;
		v.y = (pv1->v.y > pv2->v.y ? pv1->v.y : pv2->v.y);

		D3DXMATRIX r;
		D3DXMATRIX m;
		float yaw = m_StartRotation + (m_EndRotation - m_StartRotation) * ((psign->dist - m_StartDistance) / m_Length);
		D3DXMatrixRotationY(&r, yaw);
		D3DXMatrixTranslation(&m, v.x, v.y, v.z);
		D3DXMatrixMultiply(&psign->mat, &r, &m);
	}

	//===================================================================================
	// END OF MODEL
	//===================================================================================
#endif

	*pcmd++ = RINS_END;
	m_VertCount = pcv - cv;
	m_IndexCount = pidx - idx;

	// Finaly build the vertex buffers.

	//HRESULT hr;
	hr = 0;
#ifndef IGNOREFORNOW

	// ====================

	D3DVERTEXBUFFERDESC desc;
	desc.dwSize = sizeof(desc);
	desc.dwCaps = D3DVBCAPS_WRITEONLY | gVBufFlags;
	desc.dwFVF = FVF_COURSEVERTEX;
	desc.dwNumVertices = m_VertCount;

	hr = gpD3D->CreateVertexBuffer(&desc, &m_pVBuf, 0);

	//hr = m_Course.ms_pDevice->CreateVertexBuffer(sizeof(COURSEVERTEX)*m_VertCount, D3DUSAGE_WRITEONLY,
	//											 FVF_COURSEVERTEX,D3DPOOL_MANAGED, &m_pVBuf );

	if (hr != 0)
		goto xit;

	COURSEVERTEX *vert_dest;
	hr = m_pVBuf->Lock(DDLOCK_WRITEONLY, (LPVOID *) &vert_dest, 0);

	//hr = m_pVBuf->Lock(0,0,(BYTE **)&vert_dest,0);

	if (hr != 0)
		goto xit;

	pv2 = cv;
	pv1 = vert_dest;
	for (i = 0; i < (int)m_VertCount; i++)  {
		if (pv2->color > 1.0f)
			pv2->color = 1.0f;
		pv2->diffuse = D3DXCOLOR(pv2->color, pv2->color, pv2->color, 1.0f);
		*pv1++ = *pv2++;
	}
	m_pVBuf->Unlock();

	// ====================

	// ====================

	m_pIBuf = new WORD[m_IndexCount];

	CopyMemory(m_pIBuf, idx, sizeof(WORD) * m_IndexCount);

	// ====================

	m_pRenderIns = new short[pcmd - cmd];
	CopyMemory(m_pRenderIns, cmd, sizeof(short) * (pcmd - cmd));

xit:
#else

	video::SMaterial tMaterial = m_Course.Material;
	bool bTrees = false;
	SMesh *pMesh = new SMesh();
	SMesh *pMesh2 = new SMesh();
	if(pMesh && pMesh2)
	{
		m_pRenderIns = new short[pcmd - cmd];
		CopyMemory(m_pRenderIns, cmd, sizeof(short) * (pcmd - cmd));		

		u16 indices[] = {0,1,2,3,4,5,6,7,8,9,10,11};
		short *pcmd = m_pRenderIns;
		int texIdx = 0;
		float ftu,ftv;
		float ftux, ftvx;

		while (*pcmd != RINS_END)  {
			switch (*pcmd++)  {
				case RINS_END:
					//Msg("RINS_END");
					break;

				case RINS_TEX:
					//Msg("RINS_TEX %d",pcmd[0]);
					if(false)
					{
						*pcmd++;
					}
					else
					{
						//video::IVideoDriver* driver = D3DBase::GetDriver();
						texIdx = *pcmd++;
						if(texIdx==TEX_START || texIdx==TEX_FINISH)
							texIdx=TEX_ROAD;
						tMaterial.TextureLayer[0].Texture = D3DBase::GetDriver()->getTexture(m_Course.m_Tex[texIdx]);

						switch(texIdx)
						{
						default:
						case TEX_ROAD:
						case TEX_START:
						case TEX_FINISH:
							ftux = 1.0f; 
							ftvx = 1.0f;
							break;

//#ifdef NOTSKIP

						case TEX_EDGE_1_L:
						case TEX_EDGE_1_R:
							ftux = 1.0f; 
							ftvx = 2.0f;
							break;
						case TEX_EDGE_2_L:
							ftux = 1.0f; 
							ftvx = 4.0f;
							break;
						case TEX_EDGE_2_R:
							ftux = 1.0f; 
							ftvx = 4.0f;
							break;
						case TEX_EDGE_3_L:
						case TEX_EDGE_3_R:
						case TEX_EDGE_4_L:
						case TEX_EDGE_4_R:
						case TEX_EDGE_5_L:
						case TEX_EDGE_5_R:
							ftux = 2.0f; 
							ftvx = 4.0f;
							break;
						case TEX_OVERRUN:
							ftux = 8.0f; 
							ftvx = 1.0f;
							break;
						case TEX_FAR_GROUND:
							ftux = 16.0f; 
							ftvx = 16.0f;
							break;
//#endif
						}
						//driver->setMaterial(m_Course.Material);
						//float bias = (*pcmd == TEX_ROAD ? -5.6f:-0.05f);
						/*
						float bias = (*pcmd == TEX_ROAD ? -.6f:-0.05f);			// tlm20050509
						dev->SetTextureStageState( 0, D3DTSS_MIPMAPLODBIAS, *((LPDWORD)&bias) );
						dev->SetTexture(0, m_Course.m_Tex[*pcmd++]->GetTex());
						*/
					}
					break;

				case RINS_STRIP:
					//Msg("RINS_STRIP %d %d %d %d",pcmd[0],pcmd[1],pcmd[2],pcmd[3]);
					if(false)
					{
						pcmd += 4;
					}
					else
					{
						int vnum = pcmd[1];
						int inum = pcmd[3];
						SMeshBuffer *pMeshBuf = new SMeshBuffer();
						pMeshBuf->Material = tMaterial;
						pMeshBuf->setPrimitiveType(scene::EPT_TRIANGLE_STRIP);
						if(bTrees)
						{
							pMesh2->addMeshBuffer(pMeshBuf);
						}
						else
						{
							pMesh->addMeshBuffer(pMeshBuf);
						}
						// to simplify things we drop here but continue using buf
						pMeshBuf->drop();

						pv2 = &cv[pcmd[0]];
						pMeshBuf->Vertices.set_used(vnum);
						for (i = 0; i < (int)vnum; i++)  {
							if (pv2->color > 1.0f)
								pv2->color = 1.0f;
							u32 diffuse = SColorf(pv2->color, pv2->color, pv2->color, 1.0f).toSColor().color;
							S3DVertex& v = pMeshBuf->Vertices[i];
							v.Pos.set(pv2->v * mult);
							v.Normal.set(0,1,0);
							v.Color=diffuse;
							ftu = pv2->tu1 * ftux;
							ftv = pv2->tv1 * ftvx;
							v.TCoords.set(ftu, ftv);
							//Msg("i=%d,x=%f,y=%f,z=%f",i,pv2->v.x,pv2->v.y,pv2->v.z);
							pv2++;
						}
						pMeshBuf->Indices.set_used(inum);
						for (i = 0; i < (int)inum; i++)  {
							pMeshBuf->Indices[i]=idx[pcmd[2]+i];
							//Msg("i=%d,idx=%d",i,pMeshBuf->Indices[i]);
						}

						pMeshBuf->recalculateBoundingBox();
						pcmd += 4;

					//driver->drawVertexPrimitiveList(&m_pMeshBuf->Vertices[pcmd[0]], pcmd[1], &m_pMeshBuf->Indices[pcmd[2]], pcmd[3], 
					//	video::EVT_STANDARD, scene::EPT_TRIANGLE_STRIP, video::EIT_16BIT);

					// 	 0 		  1			2  			3
					// MinIndex,NumVertices,StartIndex,PrimitiveCount
					//dev->DrawIndexedPrimitiveVB(D3DPT_TRIANGLESTRIP, m_pVBuf, pcmd[0], pcmd[1],m_pIBuf + pcmd[2], pcmd[3], 0);
					//gStats.vertsrendered += pcmd[3];
					}
					break;

				case RINS_STRIP_T:
					//Msg("RINS_STRIP_T %d",pcmd[0]);
					if(false)
					{
						*pcmd++;
					}
					else
					{
						SMeshBuffer *pMeshBuf = new SMeshBuffer();
						pMeshBuf->Material = tMaterial;
						pMeshBuf->setPrimitiveType(scene::EPT_TRIANGLE_STRIP);
						if(bTrees)
						{
							pMesh2->addMeshBuffer(pMeshBuf);
						}
						else
						{
							pMesh->addMeshBuffer(pMeshBuf);
						}
						// to simplify things we drop here but continue using buf
						pMeshBuf->drop();
						int vnum = 4;
						pMeshBuf->Vertices.set_used(vnum);
						pv2 = &cv[*pcmd++];
						for (i = 0; i < (int)(vnum); i++)  {
							if (pv2->color > 1.0f)
								pv2->color = 1.0f;
							u32 diffuse = SColorf(pv2->color, pv2->color, pv2->color,1.0f).toSColor().color;
							S3DVertex& v = pMeshBuf->Vertices[i];
							v.Pos.set(pv2->v * mult);
							v.Normal.set(0,1,0);
							v.Color=diffuse;
							ftu = pv2->tu1 * ftux;
							ftv = pv2->tv1 * ftvx;
							v.TCoords.set(ftu, ftv);
							//Msg("i=%d,x=%f,y=%f,z=%f",i,pv2->v.x,pv2->v.y,pv2->v.z);
							pv2++;
						}
						pMeshBuf->Indices.set_used(vnum);
						for (i = 0; i < (int)(vnum); i++)  {
							pMeshBuf->Indices[i]=i;
							//Msg("i=%d,idx=%d",i,pMeshBuf->Indices[i]);
						}
						pMeshBuf->recalculateBoundingBox();
						//driver->drawVertexPrimitiveList(&m_pMeshBuf->Vertices[*pcmd++], 3, &indices[0], 1, 
						//	video::EVT_STANDARD, scene::EPT_TRIANGLES, video::EIT_16BIT);

						//dev->DrawPrimitiveVB(D3DPT_TRIANGLELIST, m_pVBuf, *pcmd++, 4, 0);
						//gStats.vertsrendered += 4;
					}
					break;

				case RINS_TRI:
					//Msg("RINS_TRI %d %d %d %d",pcmd[0],pcmd[1],pcmd[2],pcmd[3]);
					if(false)
					{
						pcmd += 4;
					}
					else
					{
						SMeshBuffer *pMeshBuf = new SMeshBuffer();
						pMeshBuf->Material = tMaterial;
						if(bTrees)
						{
							pMesh2->addMeshBuffer(pMeshBuf);
						}
						else
						{
							pMesh->addMeshBuffer(pMeshBuf);
						}
						// to simplify things we drop here but continue using buf
						pMeshBuf->drop();
						pMeshBuf->Vertices.set_used(pcmd[1]);
						pv2 = &cv[pcmd[0]];
						for (i = 0; i < (int)pcmd[1]; i++)  {
							if (pv2->color > 1.0f)
								pv2->color = 1.0f;
							u32 diffuse = SColorf(pv2->color, pv2->color, pv2->color,1.0f).toSColor().color;
							S3DVertex& v = pMeshBuf->Vertices[i];
							v.Pos.set(pv2->v * mult);
							v.Normal.set(0,1,0);
							v.Color=diffuse;
							ftu = pv2->tu1 * ftux;
							ftv = pv2->tv1 * ftvx;
							v.TCoords.set(ftu, ftv);
							//Msg("i=%d,x=%f,y=%f,z=%f",i,pv2->v.x,pv2->v.y,pv2->v.z);
							pv2++;
						}
						pMeshBuf->Indices.set_used(pcmd[3]);
						for (i = 0; i < (int)pcmd[3]; i++)  {
							pMeshBuf->Indices[i]=idx[pcmd[2]+i];
							//Msg("i=%d,idx=%d",i,pMeshBuf->Indices[i]);
						}
						pMeshBuf->recalculateBoundingBox();
						pcmd += 4;
						//driver->drawVertexPrimitiveList(&m_pMeshBuf->Vertices[pcmd[0]], pcmd[1], &m_pMeshBuf->Indices[pcmd[2]], pcmd[3], 
						//	video::EVT_STANDARD, scene::EPT_TRIANGLES, video::EIT_16BIT);
						// 	 0 		  1			2  			3
						// MinIndex,NumVertices,StartIndex,PrimitiveCount
						//dev->DrawIndexedPrimitiveVB(D3DPT_TRIANGLELIST, m_pVBuf, pcmd[0], pcmd[1],
						//									 m_pIBuf + pcmd[2], pcmd[3], 0);
					}
					break;
				case RINS_TREESTART:
					//Msg("RINS_TREESTART");
					bTrees = true;
					break;

				case RINS_TREEEND:
					//Msg("RINS_TREEEND");
					bTrees = false;
					break;
			}
		}

		// road, edges
		IMeshSceneNode* meshnode = 0;
		pMesh->recalculateBoundingBox();

		meshnode = D3DBase::GetSceneManager()->addMeshSceneNode(pMesh, m_pSectionNode, 2000);
		ITriangleSelector* selector = D3DBase::createTriangleSelector(pMesh, meshnode);
		meshnode->setTriangleSelector(selector);
		selector->drop();

		pMesh->drop();
		//meshnode->setScale(core::vector3df(mult,mult,mult));
		meshnode->setPosition(core::vector3df(0,0,0));

		meshnode->setMaterialFlag(video::EMF_FOG_ENABLE, fogOn);
		meshnode->setMaterialFlag(video::EMF_BACK_FACE_CULLING, true);
		bool uselight = false;
		meshnode->setMaterialFlag(video::EMF_NORMALIZE_NORMALS,uselight);
		meshnode->setMaterialFlag(video::EMF_LIGHTING,uselight);
		meshnode->setMaterialFlag(video::EMF_ANISOTROPIC_FILTER, true);
		

		// tree shadows
		pMesh2->recalculateBoundingBox();
		meshnode = D3DBase::GetSceneManager()->addMeshSceneNode(pMesh2, m_pSectionNode);
		pMesh2->drop();
		//meshnode->setScale(core::vector3df(mult,mult,mult));
		meshnode->setPosition(core::vector3df(0,0,0));

		meshnode->setMaterialFlag(video::EMF_FOG_ENABLE, fogOn);
		meshnode->setMaterialFlag(video::EMF_BACK_FACE_CULLING, true);
//		bool uselight = false;
		meshnode->setMaterialFlag(video::EMF_NORMALIZE_NORMALS,uselight);
		meshnode->setMaterialFlag(video::EMF_LIGHTING,uselight);
		meshnode->setMaterialFlag(video::EMF_ANISOTROPIC_FILTER, true);

		meshnode->setMaterialFlag(video::EMF_TEXTURE_WRAP,(bool)video::ETC_CLAMP_TO_BORDER);
		meshnode->setMaterialType(video::EMT_TRANSPARENT_ALPHA_CHANNEL);
		meshnode->getMaterial(0).MaterialTypeParam = 0.1f;
	}
	//m_pSectionNode->recalculateBoundingBox();
	//m_pSectionNode->setAutomaticCulling(scene::EAC_BOX);
	bMeshCreated = true;

#endif
	if (hr != 0)
		CloseModel();

	//delete[] cv;
	//delete[] cmd;
	//delete[] idx;
	return (hr == 0);
}

/************************************************************************************

************************************************************************************/

void CCourse::Section::reStrip(COURSEVERTEX *pv, COURSEVERTEX *pvs, int cnt, float tu, float repeat)  {
	float startv;
	float endv;

	if (repeat > 0.0f)  {
		startv = m_StartDistance / repeat;
		startv = startv - ((int) startv);
		endv = startv + (m_Length / repeat);
	}
	else  {
		startv = 0.99f;
		endv = 0.0f;
	}

	float vadv = (endv - startv) / (cnt - 1);


	for (int i=0; i<cnt; i++, startv+=vadv, pv++)  {
		if (pvs)  {
			*pv = *pvs++;
		}
		pv->tu1 = tu;
		pv->tv1 = startv;
	}

	return;
}

/************************************************************************************

************************************************************************************/
#ifndef IGNOREFORNOW

bool CCourse::Export(const char *name)  {
	FILE *file = fopen(name, "w");
	if (!file)
		return false;

	char buf[MAX_PATH];

	const char *pn = strrchr(name, '\\');
	if (!pn)
		pn = strrchr(name, ':');
	if (!pn)
		pn = name;
	else
		pn++;

	strcpy(buf, pn);
	char *pext = strrchr(buf, '.');
	if (pext)
		*pext = '\0';
	pext = buf;
	while (*pext)  {
		if (*pext == '_')
			*pext = ' ';
		pext++;
	}
	fprintf(file, "[COURSE HEADER]\nUNITS = %s\n", (gMetric ? "METRIC" : "ENGLISH"));
	fprintf(file, "DESCRIPTION = %s\n", buf);

	if (!pn)
		pn = "Unknown";
	fprintf(file, "FILE NAME = %s\n", pn);
	fprintf(file, "; DISTANCE\tGRADE\tWIND\n");
	fprintf(file, "; \n[END COURSE HEADER]\n\n");

	fprintf(file, "[COURSE DATA]\n");
	Section *ps = m_Track.GetFirst();
	if (ps)  {
		while (ps && (ps->GetStartDistance() < 0.0f))
			ps = ps->Next();
		for (;
			  ps;
			  ps = ps->Next())  {
			if (ps == m_pFinishSection)
				break;
			float len = ps->GetLength();
			if (gMetric)
				len /= 1000.0f;
			else
				len = METERS_TO_MILES(len);
			fprintf(file, "%.3f\t%.2f\t0\n", len, ps->get_grade_d_100() * 100);
		}
	}
	fprintf(file, "[END COURSE DATA]\n");
	fclose(file);
	return true;
}
#endif

/************************************************************************************

************************************************************************************/

void CCourse::calcTerrainGrid(float size, float extend)  {
	Section *ps = m_Track.GetFirst();
	m_GridSquareSize = vector3df(size, 1.0f, size);
	m_GridMin = vector3df(-1.0f, 0.0f, -1.0f);
	m_GridMax = vector3df(1.0f, 0.0f, 1.0f);

	for (;
		  ps;
		  ps = ps->Next())  {
		float t;
		extRect2(m_GridMin, m_GridMax, ps->m_ExtMin);
		extRect2(m_GridMin, m_GridMax, ps->m_ExtMax);

		t = ps->m_LowY;
		if (t < m_GridMin.y)
			m_GridMin.y = t;
		t = ps->m_HighY;
		if (t > m_GridMax.y)
			m_GridMax.y = t;
	}

	m_GridMin.x -= extend;
	m_GridMin.z -= extend;
	m_GridMax.x += extend;
	m_GridMax.z += extend;

	m_GridWidth = (m_GridMax.x - m_GridMin.x) / m_GridSquareSize.x;
	m_GridHeight = (m_GridMax.z - m_GridMin.z) / m_GridSquareSize.z;
	if (m_GridWidth < 1)
		m_GridWidth = 1;
	if (m_GridHeight < 1)
		m_GridHeight = 1;

	vector3df v = (m_GridMin + m_GridMax) / 2.0f;

	extend = (m_GridSquareSize.x * m_GridWidth) / 2.0f;
	m_GridMin.x = v.x - extend;
	m_GridMax.x = v.x + extend;

	extend = (m_GridSquareSize.z * m_GridHeight) / 2.0f;
	m_GridMin.z = v.z - extend;
	m_GridMax.z = v.z + extend;
}

/************************************************************************************
	entry:
		ALL VARIABLES ARE METRIC!!!

************************************************************************************/
#ifndef IGNOREFORNOW

bool CCourse::EditRandom(float _maxlen, float _maxgrade, float _maxwind)  {
	float tgrade, twind;
	float basedeg;
	float mindeg, maxdeg;
	float avelen;
	float slen, elen, tlen;
	float tdeg, d;
	float len, grade, wind;
	Section *last_section = NULL;
	Section *ps = NULL;

	// copy to local variables!

	len = _maxlen;
	grade = _maxgrade;
	wind = _maxwind;

	if (len < 100)  {
		len = 100.0f;
	}

	if (grade < 0.0f)  {
		grade = 0.0f;
	}

	last_section = m_Track.GetLast();
	// m_SI

//	bool first = false;
//	if (last_section->m_Type == Section::START)  {
//		bp = 1;
//		first = true;
//		float f = last_section->m_Wind;
//		f = last_section->m_StartWind;
//	}

	basedeg = radToDeg(last_section->GetEndRotation());
	mindeg = basedeg - 80.0f;
	maxdeg = basedeg + 80.0f;

	avelen = len / 10;

	if (avelen > 500.0f)  {
		avelen = 500.0f;
	}
	else if (avelen < 50.0f)  {
		avelen = 50.0f;
	}

	slen = avelen * 0.5f;
	elen = avelen * 1.5f;

	srand(timeGetTime());

	twind = (wind > 0 ? frand(-wind, wind) : 0.0f);		// twind is metric, tlm20040719, using the same wind for the entire section!!!
	if (last_section->m_Type == Section::START)  {
		last_section->m_Wind = twind;							// to cause the beginning wind in the course to show up
	}


	while (1)  {

		if (len <= 0)  {
			//throw (fatalError(__FILE__, __LINE__));
		}

		GetSectionInfo();									// same data as Section *last_section

		if (!(m_SI.flags & SI_ADD))  {
			bp = 1;
			return false;
		}

		//-------------------------
		// Add a staight section
		//-------------------------

		tlen = (frand(slen, elen) + frand(slen, elen)) / 2.0f;
		if (tlen > len - 10.0f)  {
			tlen = len;
		}

		tgrade = (grade > 0 ? frand(-grade, grade) / 100.0f : 0.0f);
		//twind = (wind > 0 ? frand(-wind, wind) : 0.0f);									// twind is metric, tlm20040719

		ps = new Section(
						*this,					// CCourse *
						Section::CURVE,		// type
						tlen,						// length to add
						tgrade,					// grade
						0.0f						// angle, 0 = straight
						// int special = TEX_ROAD
						// float startdist = 0.0f
						);
		//cursec++;
		ps->SetWind(twind);																// twind is metric
		ps->m_StartWind = twind;

		if (!(updateSI().flags & SI_ADD))  {
			bp = 1;
			break;
		}

		len -= tlen;

		if (len <= 0)  {
			bp = 1;
			break;
		}

		//-------------------------
		// Add a curved section
		//-------------------------

		tlen = frand(20.0f, 50.0f);
		tdeg = frand(5.0f, 35.0f) * ((rand() % 100) > 50 ? -1.0f : 1.0f);

		d = radToDeg(ps->GetEndRotation()) + tdeg;

		if ((d < mindeg) || (d > maxdeg))  {
			tdeg = -tdeg;
		}

		if (tlen > len - 10.0f)  {
			tlen = len;
		}

		d = radToDeg(tlen / minRadius);

		if (tdeg > d)  {
			tdeg = d;
		}
		else if (tdeg < -d)  {
			tdeg = -d;
		}

		tgrade = (grade > 0 ? frand(-grade, grade) / 100.0f : 0.0f);

		ps = new Section(*this, Section::CURVE, tlen, tgrade, tdeg);
		//cursec++;
		ps->SetWind(twind);								// tlm20050718
		if (!(updateSI().flags & SI_ADD))  {
			bp = 1;
			break;
		}

		len -= tlen;

		if (len <= 0)  {
			bp = 1;
			break;
		}
	}

	calcGrid();
	ClearProfile();

	return true;
}
#endif


#endif			// ifdef BIKE_APP


/*************************************************************

  in Course.h:

typedef struct  {
	char fname[256];
	long maxlaps;
	long minlaps;
	float mindist;
	float maxdist;
	double mindpm;			// "degrees per meter"
	double maxdpm;
	DWORD version;
	long nsecs;
	double totalmeters;
	bool closed;
	float difficulty;			// 0-4 = easy, 4-8 = medium, 8-10 = hard, 10+ = very hard
	int laps;
} STATS;

*************************************************************/

void CCourse::computeStats(void)  {
//#ifndef IGNOREFORNOW

	//	float endgrade, endrotation, endDistance, extRadius, sectionRotation;
	//	float len, startDistance, actualLength, degRotation, rotation;

	int i;
	float angle, wind, grade_d_100;
	vector3df v1, v2, v3;

	float altgain;
	float dy2;
	int firstx = -1;
	float total_meters = 0.0f;
	double theta;
	float meters;
	float elevation = 0.0f;
	float dy;
	int bestx=-1;
	float last_elevation;
	int type;

	float total_percent_grade = 0.0f;
	float percent_grade = 0.0f;
	//double totalGrade = 0.0;
	//double totalGrade2;

	double totalWind = 0.0;
	vector3df	loc, vec;
	float dx = totalMeters / 1000.0f;
	float dist;
	float degrees;
	int gradecount;



	course_meters = totalMeters = GetCourseLength();

	CCourse::Section *ps = GetFirstSection();
	if (ps==NULL)  {
		cleanup();
		//throw (fatalError(__FILE__, __LINE__));
	}

	nlegs = 0;
	elevation = 0.0f;
	last_elevation = -1000000.0f;
	minElevation = 0.0f;
	maxElevation = 0.0f;
	altgain = 0.0f;
	dy2 = 0.0f;

	//mapvalues(-1.0f, 1.0f, (float)(-PI/4.0), (float)(PI/4.0), &m, &b);
	//nlegs = countSections();

	i = 0;


	do  {

		if (ps->GetStartDistance() < 0.0f)  {
			ps = ps->Next();
			if (ps==NULL)  {
				break;
			}
			continue;
		}

		if (ps->IsFinishSection())  {			// there is only one finish section
			bp = 1;
			break;
		}

		type = ps->getType();

		if (type != Section::STREIGHT && type != Section::CURVE && type != Section::CLOSELOOP)  {
			ps = ps->Next();
			if (ps==NULL)  {
				break;
			}
			continue;
		}


		if (firstx==-1)  {
			firstx = i;
		}
				
		grade_d_100 = ps->get_grade_d_100();
		percent_grade = 100.0f * grade_d_100;

		meters = ps->GetLength();
		wind = ps->GetWind();
		angle = ps->GetDegRotation();

		// can't do min & max grade here because it varies thoughout the section!! percent_grade is only the
		// ENDING grade for this section!

		min_percent_grade = min(min_percent_grade, percent_grade );
		max_percent_grade = max(max_percent_grade, percent_grade );
		minwind = min(minwind, wind);
		maxwind = max(maxwind, wind);
		minlen = MIN(meters, minlen);
		maxlen = MAX(meters, maxlen);
		minangle = MIN(angle, minangle);
		maxangle = MAX(angle, maxangle);

		total_percent_grade += percent_grade;
		total_meters += meters;

		theta = atan(grade_d_100);
		//theta = m*grade_d_100 + b;								// this is grade divided by 100
		if (theta < .000001 && theta > -.000001)  {
			theta = 0.0;									// round off to 0 if it really is 0 tlm20040330
		}
					
		dy = meters * sin(theta);
						
		elevation += dy;					// elevation is in meters
		if (elevation<.000001 && elevation>-.000001)  {
			elevation = 0.0;
		}
				
		minElevation = MIN(minElevation, elevation);
		maxElevation = MAX(maxElevation, elevation);

		if (elevation >= maxElevation)  {
			bestx = i+1;
		}
		else if (bestx==-1)  {
			bestx = i;
		}
				
		if (i==0)  {
			dy2 = elevation;
		}
		else  {
			dy2 = elevation - last_elevation;
		}
						
		if (dy2 > 0)  {
			altgain += dy2;
		}
					
		last_elevation = elevation;

		ps = ps->Next();
		if (ps==NULL)  {
			break;
		}
		i++;
		bp = i;
	} while(1);


//	nlegs = i+1;
	nlegs = i;

//	assert(nlegs==countSections());

	closed = IsLoopClosed();

	maxElevGain = altgain;					// climbing meters

	//min_percent_grade *= 100.0f;			// tlm20060601
	//max_percent_grade *= 100.0f;			// tlm20060601

	//-------------------------------------------------------------
	// find the averages, based on distance, not on leg count:
	//-------------------------------------------------------------


	i = 0;
	gradecount = 0;
	double total_grade_d_100_2 = 0.0;
	double total_grade_d_100 = 0.0;
	dx = totalMeters / 1000.0f;
	dist = 0.0f;
	float m,b;
	mapvalues(0.0f, 999.0f, 0.0f, totalMeters, &m, &b);

	min_percent_grade = FLT_MAX;
	max_percent_grade = -FLT_MAX;

	//for(dist=0.0f; dist<=totalMeters; dist+=dx)  {
	//for(i=0;i<1000;i++)  {
	for(i=0;i<=totalMeters;i++)  {
		dist = m*i + b;
		//dist = i*dx;
		/*
		if (i==999)  {
			dist = totalMeters;
		}
		else  {
			dist += dx;
		}
		*/
		RoadLoc(dist, loc, vec, grade_d_100, 0, &angle, &degrees, &wind);
		total_grade_d_100 += grade_d_100;
		percent_grade = 100.0f*grade_d_100;
		min_percent_grade = min(min_percent_grade, percent_grade );
		max_percent_grade = max(max_percent_grade, percent_grade );

		if (grade_d_100>0)  {
			total_grade_d_100_2 += grade_d_100;
			gradecount++;
		}

		totalWind += wind;
	}

	avg_percent_grade = (float) (total_grade_d_100 / (double)i );
	avg_percent_grade *= 100.0f;			// tlm20060601

	if (gradecount > 0)  {
		avg_positive_percent_grade = (float) (100.0f * (total_grade_d_100_2 / (double)gradecount ) );
	}
	else  {
		avg_positive_percent_grade = 0.0f;
	}

	avgwind = (float) (totalWind / (double)i );



	// compute the difficulty level (0.0f to 100.0f)
	//difficulty = 50.0f;

//#endif

	return;

}						// computeStats()


#ifdef BIKE_APP


#ifndef IGNOREFORNOW

/******************************************************************************

******************************************************************************/

bool CCourse::save_performance(int num, class ComputrainerBase &ct)  {
	int cnt;
	PerfPoint *ptemp = NULL;

	//if (logg) logg->write(10,0,1,"      _c: sp1\n");

	#ifdef THREEDV2
		//return false;
	#endif

	/*
	if (m_pPerfArr[num])  {
		//delete[] m_pPerfArr[num];
		//m_pPerfArr[num] = NULL;
		//throw (fatalError(__FILE__, __LINE__));
	}
	*/

	bool ans = ct.MakePerfPointArray(ptemp, (int &)cnt );		// creates ptemp[]
	if (!ans || (cnt == 0))  {
		if (ptemp)  {
			delete[] ptemp;
		}
		return false;
	}

	//if (logg) logg->write(10,0,1,"      _c: sp2\n");

	#ifdef VELOTRON
		#ifdef LOG_GEARING
			if (gearInfoArray[num])  {
				//delete[] gearInfoArray[num];
				//gearInfoArray[num] = NULL;
				//throw (fatalError(__FILE__, __LINE__));
			}
			ComputrainerBase::GEARINFO *ptemp2=NULL;
			int cnt2;
			ans = ct.makeGearInfoArray(ptemp2, (int &) cnt2);
			if (!ans || (cnt2 == 0))  {
				if (ptemp)  {
					delete[] ptemp;
					ptemp = NULL;
				}
				if (ptemp2)  {
					delete[] ptemp2;
					ptemp2 = NULL;
				}
				return false;
			}
			if (cnt != cnt2)  {
				FILE *stream = fopen("counterror.txt", "wt");
				fprintf(stream, "perfpoint count = %d\n", cnt);
				fprintf(stream, "gearing count = %d\n", cnt2);
				fclose(stream);
				#ifndef WEBAPP
					//throw (fatalError(__FILE__, __LINE__));
				#else
					return false;
				#endif
			}
			//gearInfoArray[num] = ptemp2;
		#endif
	#endif

	#ifdef LOG_CALORIES
		float *ftemp=NULL;
		int cnt3;
		ans = ct.makeCaloriesArray(ftemp, (int &) cnt3);
		if (!ans || (cnt3 == 0))  {
			if (ptemp)  {
				delete[] ptemp;
				ptemp = NULL;
			}

			#ifdef VELOTRON
				if (ptemp2)  {
					delete[] ptemp2;
					ptemp2 = NULL;
				}
			#endif

			if (ftemp)  {
				delete[] ftemp;
				ftemp = NULL;
			}
			return false;
		}

		if (cnt != cnt3)  {
			FILE *stream = fopen("counterror.txt", "wt");
			fprintf(stream, "perfpoint count = %d\n", cnt);
			fprintf(stream, "calories count = %d\n", cnt3);
			fclose(stream);
			#ifndef WEBAPP
				//throw (fatalError(__FILE__, __LINE__, "see counterror.txt"));
			#else
				return false;
			#endif
		}
		//gearInfoArray[num] = ptemp2;
	#endif

	//if (logg) logg->write(10,0,1,"      _c: sp3\n");

	ClearProfile(num);
	//if (logg) logg->write(10,0,1,"      _c: sp4\n");

	PerformanceInfo &pi = pinf[num];

	pi.perfcount = cnt;
	m_pPerfArr[num] = ptemp;

	#ifdef VELOTRON
		#ifdef LOG_GEARING
			gearInfoArray[num] = ptemp2;
		#endif
	#endif

	#ifdef LOG_CALORIES
		calories[num] = ftemp;
	#endif

	strcpy(pi.name, ct.GetName());
	pi.gender = (unsigned char) ct.GetGender();
	pi.age = (unsigned char) ct.GetAge();
	pi.height = ct.GetHeight_Meters();
	pi.weight = ct.GetWeight_KPS();
	pi.upper_hr = ct.GetUpperHeartRate();
	pi.lower_hr = ct.GetLowerHeartRate();

	SYSTEMTIME &si = ct.GetStartTime();
	pi.year = (long) si.wYear;
	pi.month = (unsigned char) si.wMonth;
	pi.day = (unsigned char) si.wDay;
	pi.hour = (unsigned char) si.wHour;
	pi.minute = (unsigned char) si.wMinute;

	pi.length = GetCourseLength();
	pi.racetime = m_pPerfArr[num][pi.perfcount - 1].disttime;

	pi.watts_factor = ct.watts_factor;

	#ifndef VELOTRON
		pi.rfDrag = ct.GetRfDrag();
		pi.rfMeas = ct.GetRfMeas();
	#endif

	//if (logg) logg->write(10,0,1,"      _c: sp5\n");

	return true;
}
#endif		// #ifdef BIKE_APP
#endif
/************************************************************************************
	lastcrs.3dc
************************************************************************************/

bool CCourse::SavePerf(int num, const char *prefpath, PerformanceInfo *pperfi, PerfPoint *perfs, UINT mode)  {

	char basename[MAX_PATH];
	char buf[MAX_PATH];
	char curdir[256];

	// is this a prefs file?

	strcpy(basename, prefpath);
	strip_path(basename);
	strcpy(buf, basename);

	char *pstr = (char *)strrchr(basename, '.');
	bool perf = 0;

	if (pstr)  {
		perf = (_stricmp(pstr + 1, "3dp") == 0);
	}

	FILE *file = fopen(prefpath, "wb");

	if (!file)  {
		sprintf(gstring, "Can't open %s", prefpath);
		//MessageBox(NULL, gstring, "Error", MB_OK);
		return false;
	}

	if (m_pName)  {		// tlm20040208
		if (((*m_pName == '\0') || !perf) && ((_stricmp(basename, "Lastcrs.3dc") != 0) && (_stricmp(basename, "LASTPERF.3DP") != 0)))  {
			SetCourseName(basename);
		}
	}

	// Keeping the old version while makeing a new version.
	// The new version will not crash the old version.

	int version;

	bool lastfile = false;
	int pos;

	bp = 1;

	pos = indexIgnoreCase((char *)basename, "lastcrs.3dc", 0);
	if (pos != -1)  {
		lastfile = true;
		if (m_Version==0)  {
			m_Version = 6;
		}
	}

	if (lastfile)  {
		version = m_Version;
	}
	else  {

		/*
		#ifndef VELOTRON
			#ifdef MULTI_APP
				version = 7;
			#else
				#ifndef TOPO_APP
					#ifndef WEBAPP
						if (reg->registered)  {
							version = 6;								// unencrypted
						}
						else  {
							version = 7;								// encrypted
						}
					#else
						version = 7;
					#endif
				#else
					version = 7;								// encrypted
				#endif
			#endif
		#else
			version = 7;									// always version 7 for velotron
		#endif
		*/
//		version = m_Version;

		version = 6;

	}

	if (perf)  {
		bp = 1;
	}

	//if (logg) logg->write(10,0,1,"\t\t\t_Course::Save: 3\n");

	SecFileWrite f(file);


	bool special = false;
	Section *ps;
	int cnt = 0;

	if(mode == ThreeD)
	{
		ps = m_Track.GetFirst();
		for (;ps; ps=ps->Next())  {
			cnt++;
			if (ps->m_Flags & (SECTION_F_ABS_STARTLOC | SECTION_F_ABS_Y))
				special = true;
		}
	}
	else
	{
		cnt = 1;
		version = 6;
	}
	//if (logg) logg->write(10,0,1,"\t\t\t_Course::Save: 4\n");

	// WRITE THE VERSION - The very first thing.
	// =========================================


	f.Write(version);		// Version

	// this contains dummy data for the two sectons.

	if (num>=0)  {
		if (pperfi->perfcount > 0)  {
			if (perf)  {
				//RiderData rd;
				//rd = gpBikeApp->GetDisplayPrefs().riderdata[num];
				bp = 1;
			}

			f.Push("perf");

			f.Write(pperfi, sizeof(PerformanceInfo));
			f.Write(perfs, pperfi->perfcount * sizeof(PerfPoint));

			f.Pop();


			#ifdef VELOTRON
				#ifdef LOG_GEARING			// store the gear information as a separate section
					if (gearInfoArray[num])  {
						f.Push("GEAR");
						int n = pperfi->perfcount;
						f.Write(n);
						f.Write(gearInfoArray[num], n * sizeof(ComputrainerBase::GEARINFO));
						f.Pop();
					}
				#endif
			#endif

			#ifdef LOG_CALORIES
				if (calories[num])  {
					f.Push("CALS");
					int n = pperfi->perfcount;
					f.Write(n);
					f.Write(calories[num], n * sizeof(float));
					f.Pop();
				}
			#endif
		}
	}				// if (num>=0)  {


	//if (logg) logg->write(10,0,1,"\t\t\t_Course::Save: 5\n");

	if(mode == ThreeD)
	{
		// Track Section.

		f.Push((special ? "CTK2" : "C_TK"));

		// Save ALL the sections.

		f.Write(cnt);

		for (ps = m_Track.GetFirst(); ps; ps = ps->Next())  {
			if (!ps->Save(f))
				break;
		}
		f.Pop();

		// Laps

		f.Push("LAPS");
		f.Write(getLaps());
		f.Pop();


		if (m_pName)  {		// tlm20040208
			f.Push("Name");
			f.WriteString(m_pName);
			f.Pop();
		}


		if (m_pDescription)  {
			f.Push("Desc");
			f.WriteString(m_pDescription);
			f.Pop();
		}


		//if (logg) logg->write(10,0,1,"\t\t\t_Course::Save: 6\n");

		if (m_pCreator)  {
			f.Push("CRTR");
			f.WriteString(m_pCreator);
			f.Pop();
		}

		f.Push("Wind");
		f.Write(cnt);

		for (ps = m_Track.GetFirst(); ps; ps = ps->Next())  {
			f.WriteFloat(ps->GetWind());
		}

		f.Pop();


		f.Push("OFST");
		f.WriteFloat(m_Offset.x);
		f.WriteFloat(m_Offset.y);
		f.WriteFloat(m_Offset.z);
		f.WriteFloat(m_BaseRotation);
		f.Pop();

		if (m_SecondStart != 0.0f)  {
			f.Push("SSTR");
			f.WriteFloat(m_SecondStart);
			f.Pop();
		}
	}
	else
	{
		// Track Section.
		f.Push((special ? "CTK2" : "C_TK"));
		// Save ALL the sections.
		f.Write(cnt);
		f.Push("sec");
		f.Write(Section::START);
		f.WriteFloat(10);
		f.WriteFloat(0);
		f.WriteFloat(0);
		f.Write(0);
		f.WriteFloat(0);
		f.Pop();
		f.Pop();

		// Laps

		f.Push("LAPS");
		f.Write(0);
		f.Pop();

		f.Push("Wind");
		f.Write(cnt);
		f.WriteFloat(0);
		f.Pop();


		f.Push("OFST");
		f.WriteFloat(0);
		f.WriteFloat(0);
		f.WriteFloat(0);
		f.WriteFloat(0);
		f.Pop();
	}
	//if (logg) logg->write(10,0,1,"\t\t\t_Course::Save: 7\n");


	fclose(file);

	// now encrypt the file if it is version 7:

	if (version==7)  {
		doo(prefpath);
	}
	// tlm20060627---
	saved_file_size = filesize_from_name(prefpath);

	//if (logg) logg->write(10,0,1,"\t\t\t_Course::Save: 8, size = %ld\n", saved_file_size );

	return f.IsOK();
}			// Save()


bool CCourse::Save(const char *basename, int num)  {

	char buf[MAX_PATH];
	char curdir[256];

	//if (logg) logg->write(10,0,1,"\t\t\t_Course::Save: %s, %d\n", basename, num);

#ifndef IGNOREFORNOW
#ifdef BIKE_APP
	GetCurrentDirectory(256, curdir);
	strcat(curdir, "\\Rider Performance");

	if (!direxists(curdir))  {
		CreateDirectory(curdir, NULL);
	}
#endif
#endif

	// is this a prefs file?

	strcpy(buf, basename);
	char *pstr = (char *)strrchr(basename, '.');
	bool perf = 0;

	if (pstr)  {
		perf = (_stricmp(pstr + 1, "3dp") == 0);
	}


	char *name = makeName(basename, buf);


	FILE *file = fopen(name, "wb");

	if (!file)  {
		sprintf(gstring, "Can't open %s", name);
		//MessageBox(NULL, gstring, "Error", MB_OK);
		return false;
	}

	//if (logg) logg->write(10,0,1,"\t\t\t_Course::Save: 2\n");

	if (m_pName)  {		// tlm20040208
		if (((*m_pName == '\0') || !perf) && ((_stricmp(basename, "Lastcrs.3dc") != 0) && (_stricmp(basename, "LASTPERF.3DP") != 0)))  {
			SetCourseName(basename);
		}
	}

	// Keeping the old version while makeing a new version.
	// The new version will not crash the old version.

	int version;

	bool lastfile = false;
	int pos;

	bp = 1;

	pos = indexIgnoreCase((char *)basename, "lastcrs.3dc", 0);
	if (pos != -1)  {
		lastfile = true;
		if (m_Version==0)  {
			m_Version = 6;
		}
	}

	if (lastfile)  {
		version = m_Version;
	}
	else  {

		/*
		#ifndef VELOTRON
			#ifdef MULTI_APP
				version = 7;
			#else
				#ifndef TOPO_APP
					#ifndef WEBAPP
						if (reg->registered)  {
							version = 6;								// unencrypted
						}
						else  {
							version = 7;								// encrypted
						}
					#else
						version = 7;
					#endif
				#else
					version = 7;								// encrypted
				#endif
			#endif
		#else
			version = 7;									// always version 7 for velotron
		#endif
		*/
//		version = m_Version;

		version = 6;

	}

	if (perf)  {
		bp = 1;
	}

	//if (logg) logg->write(10,0,1,"\t\t\t_Course::Save: 3\n");

	SecFileWrite f(file);


	bool special = false;
	Section *ps;
	int cnt = 0;
	ps = m_Track.GetFirst();

	for (;ps; ps=ps->Next())  {
		cnt++;
		if (ps->m_Flags & (SECTION_F_ABS_STARTLOC | SECTION_F_ABS_Y))
			special = true;
	}

	//if (logg) logg->write(10,0,1,"\t\t\t_Course::Save: 4\n");

	// WRITE THE VERSION - The very first thing.
	// =========================================


	f.Write(version);		// Version

	// this contains dummy data for the two sectons.

	if (num>=0)  {
		if (pinf[num].perfcount > 0)  {
			if (perf)  {
				//RiderData rd;
				//rd = gpBikeApp->GetDisplayPrefs().riderdata[num];
				bp = 1;
			}

			f.Push("perf");
			f.Write(pinf + num, sizeof(pinf[num]));
			f.Write(m_pPerfArr[num], pinf[num].perfcount * sizeof(PerfPoint));

			f.Pop();


			#ifdef VELOTRON
				#ifdef LOG_GEARING			// store the gear information as a separate section
					if (gearInfoArray[num])  {
						f.Push("GEAR");
						int n = pinf[num].perfcount;
						f.Write(n);
						f.Write(gearInfoArray[num], n * sizeof(ComputrainerBase::GEARINFO));
						f.Pop();
					}
				#endif
			#endif

			#ifdef LOG_CALORIES
				if (calories[num])  {
					f.Push("CALS");
					int n = pinf[num].perfcount;
					f.Write(n);
					f.Write(calories[num], n * sizeof(float));
					f.Pop();
				}
			#endif
		}
	}				// if (num>=0)  {


	//if (logg) logg->write(10,0,1,"\t\t\t_Course::Save: 5\n");

	// Track Section.

	f.Push((special ? "CTK2" : "C_TK"));

	// Save ALL the sections.

	f.Write(cnt);

	for (ps = m_Track.GetFirst(); ps; ps = ps->Next())  {
		if (!ps->Save(f))
			break;
	}
	f.Pop();

	// Laps

	f.Push("LAPS");
	f.Write(getLaps());
	f.Pop();


	if (m_pName)  {		// tlm20040208
		f.Push("Name");
		f.WriteString(m_pName);
		f.Pop();
	}


	if (m_pDescription)  {
		f.Push("Desc");
		f.WriteString(m_pDescription);
		f.Pop();
	}


	//if (logg) logg->write(10,0,1,"\t\t\t_Course::Save: 6\n");

	if (m_pCreator)  {
		f.Push("CRTR");
		f.WriteString(m_pCreator);
		f.Pop();
	}

	f.Push("Wind");
	f.Write(cnt);

	for (ps = m_Track.GetFirst(); ps; ps = ps->Next())  {
		f.WriteFloat(ps->GetWind());
	}

	f.Pop();


	f.Push("OFST");
	f.WriteFloat(m_Offset.x);
	f.WriteFloat(m_Offset.y);
	f.WriteFloat(m_Offset.z);
	f.WriteFloat(m_BaseRotation);
	f.Pop();

	if (m_SecondStart != 0.0f)  {
		f.Push("SSTR");
		f.WriteFloat(m_SecondStart);
		f.Pop();
	}

	//if (logg) logg->write(10,0,1,"\t\t\t_Course::Save: 7\n");


	fclose(file);

	// now encrypt the file if it is version 7:

	if (version==7)  {
		doo(name);
	}
	// tlm20060627---
	saved_file_size = filesize_from_name(name);

	//if (logg) logg->write(10,0,1,"\t\t\t_Course::Save: 8, size = %ld\n", saved_file_size );

	return f.IsOK();
}			// Save()

/************************************************************************************

************************************************************************************/


#if defined(BIKE_APP) || defined(MULTI_APP) || defined(MULTIVID_APP) || defined(TOPO_APP)

#ifndef IGNOREFORNOW
bool CCourse::myImport(const char *name)  {
	char buf[256];
	ClearTrack();
	FILE *file = fopen(name, "r");

	if (!file)  {
		return false;
	}

	DELARR(m_pName);
	m_pName = new char[strlen(name) + 1];
	strcpy(m_pName, name);

	int sec = 0;
	bool metric = true;
	ReadyEdit();
	Section *psec = m_Track.GetLast();

	float deg = 0.0f;
	float lastdeg = 0.0f;
	float changedist = 100.0f;
	float cdeg = 0.0f;

	srand(timeGetTime());


	while (fgets(buf, 256, file) != NULL)  {
		buf[255] = '\0';
		char *ps;

		if ((ps = strchr(buf, ';')))
			*ps = '\0';

		ps = Strip(buf);

		if (_stricmp(ps, "[COURSE HEADER]") == 0)
			sec = 1;
		else if (_stricmp(ps, "[END COURSE HEADER]") == 0)
			sec = 0;
		else if (_stricmp(ps, "[COURSE DATA]") == 0)
			sec = 2;
		else if (_stricmp(ps, "[END COURSE DATA]") == 0)
			sec = 0;
		else if (sec == 1)  {
			char *t;
			char *d;
			if (splitLine(ps, t, d))  {
				if (_stricmp(t, "UNITS") == 0)  {
					if (_stricmp(d, "ENGLISH") == 0)
						metric = false;
				}
				else if (_stricmp(t, "DESCRIPTION") == 0)  {
					// 	m_Description = t;
				}
				else if (_stricmp(t, "FILE NAME") == 0)  {
					// todo - do something with this.
				}
			}
		}
		else if (sec == 2)  {
			char *argv[10];
			int argc = ParseLine(buf, 10, argv);
			if (argc >= 2)  {
				float dist = (float) atof(argv[0]);
				float dist2;

				if (metric)  {
					dist = dist * 1000.0f;
				}
				else  {
					dist = MILES_TO_METERS(dist);
				}

				//tlm20060516+++
				dist2 = METERSTOMILES * dist;

				if (dist2 > 5.0f)  {
					if (!message_shown)  {
						message_shown = true;
						if (gMetric)  {
							sprintf(gstring, "This .crs file contains leg(s) longer than 17903.9 meters\nand may cause this software to run out of memory");
						}
						else  {
							sprintf(gstring, "This .crs file contains leg(s) longer than 5 miles\nand may cause this software to run out of memory");
						}
						MessageBox(NULL, gstring, "Info", MB_OK);
						//return false;
					}
				}
				//tlm20060516---

				float grade = (float) atof(argv[1]) / 100.0f;

				changedist -= dist;
				float newdeg = deg + dist*cdeg;

				if ((changedist >= 0) && (newdeg >= -89.0f) && (newdeg <= 89.0f))  {
					// Just keep going.
				}
				else  {
					// OK select a new section

					if ((dist < 100.0f) && (cdeg != 0.0f) && (frand(0.0f, 1.0f) > 0.5f))  {
						cdeg = 0.0f;
						changedist = (frand(0.0f, 1.0f) >
										  0.9f ?
										  frand(10.0f, 100.0f) :
										  frand(10.0f, 30.0f)) -
										 dist;
					}
					else  {
						float dneg = deg - -89.0f;
						float dpos = 89.0f - deg;
						float dir = frand(-(dneg / (89.0f * 2.0f)), (dpos / (89.0f * 2.0f)));
						float mdeg = (dir < 0.0f ? dneg : dpos);
						float rdeg = radToDeg(dist / 10.0f);

						if (mdeg > rdeg)  {
							mdeg = rdeg;
						}

						float d = frand(0.0f, mdeg);
						changedist = frand(50.0f, 100.0f);

						if (dist > changedist)  {
							changedist = dist;
						}

						if (dir < 0.0f)  {
							d = -d;
						}

						cdeg = d / changedist;
						changedist -= dist;
					}
				}

				if (dist > 0.1f)  {
					Section *ps = new Section(*this, Section::CURVE, dist, grade, dist *cdeg);
					deg = radToDeg( ps->GetEndRotation() );
				}

			}
		}
	}

	updateSI();

	if (m_SI.flags & SI_FINISH)  {
		EditFinish();
	}

	finishTrack();
	fclose(file);
	return IsRaceable();
}				// Import()
#endif
#ifdef BIKE_APP

/****************************************************************************************
	prints out basic info about perf info stored in this course file
****************************************************************************************/

void CCourse::analyze_perfs(void)  {
#ifndef IGNOREFORNOW
	int n1, n2;
	PerfPoint *pp = NULL;
	PerformanceInfo *pi = NULL;
	RiderData rd;
	static int calls = 0;
	FILE *stream = NULL;
	int i, j;
	float mph, watts;
	float f1, f2;

	if (calls==0)  {
		stream = fopen("analyze.txt", "wt");
		fprintf(stream, "call             name    count      mph    watts          time\n");
	}
	else  {
		stream = fopen("analyze.txt", "a+t");
	}

	calls++;

	if (calls==2)  {
		bp = 0;
	}


	fprintf(stream, "\n");

	/*

	*/

	//xxx

	for(i=0; i<2; i++)  {
		watts = 0.0f;
		mph = 0.0f;
		assert(m_pPerfArr[i]==GetPerfPointArray(i));
		assert(GetPerfPointCount(i)==pinf[i].perfcount);

		GetPerfRiderData(i, rd);
		pi = GetPerfInfo(i);
		pp = GetPerfPointArray(i);
		if (pp)  {
			watts = pp->watts;
			mph = 160.0f*pp->speed;
			for(j=0; j<pinf[i].perfcount; j++)  {
				f1 = pp->watts;
				f2 = 160.0f*pp->speed;
				bp = j;
				pp++;
			}
			pp = GetPerfPointArray(i);
		}
		else  {
			assert(i==0);
			bp = 3;
		}

		if (rd.name[0]==0)  {
			strcpy(rd.name, "none");
		}
		fprintf(stream, "%4d  %15s      %3d   %6.1f   %6.1f    %10ld\n", 
					calls, 
					rd.name, 
					pinf[i].perfcount,
					mph,
					watts,
					pinf[i].racetime
				);
		bp = i;
	}

	fclose(stream);
#endif
	return;
}
#endif		// #ifdef BIKE_APP
#endif		// #if defined(BIKE_APP) || defined(MULTI_APP) || defined(TOPO_APP)

/************************************************************************************
	totalMeters
	course_meters
	nlegs
	m_Track
************************************************************************************/

#define _COURSE_DUMPNAME ".\\debug\\x22.x"


int CCourse::make_crs(const char *_outfname)  {
	typedef struct  {
		int i;
		int leg;
		float sd;
		float len;
		float grade;
		float elev;
		float heading;
		float bend;
		float swnd;
		float mwnd;
		float sd2;
		float ed;
		float endy;
		char type[32];
	} DATA;
	DATA tdata;
	std::vector<DATA> data;
	int n;
	FILE *instream, *outstream;
	char *cptr;
	char buf[256];

/*
   I  LEG           SD          LEN         GRADE         ELEV      HEADING         BEND         SWND         MWND                SD            ED         ENDY         TYPE
                             (feet)                     (feet)    (DEGREES)                     (MPH)        (MPH)           (miles)       (miles)       (feet)

   3    1     0.000000   404.975403     0.000000%     0.000000     0.000000     0.000000     0.000000     0.000000          0.000000      0.076700     0.000000     STREIGHT
   4    1   404.975396    82.020996     0.000000%     0.000000   337.499664   -22.500336     0.000000     0.000000          0.076700      0.092234     0.000000        CURVE
   5    2   486.996368    42.023636     0.000000%     0.000000   337.499664     0.000000     0.000000     0.000000          0.092234      0.100193     0.000000     STREIGHT
   6    3   529.020014   446.997253     3.549998%     6.800741   337.499664     0.000000     0.000000     0.000000          0.100193      0.184852     2.072866     STREIGHT
...
  27   24  5081.274478    82.020996     8.049995%    56.493832   223.148422    22.500336     0.000000     0.000000          0.962363      0.977897    17.219320        CURVE
  28   25  5163.295476    92.000000     8.049995%    63.899834   248.386230    25.237814     0.000000     0.000000          0.977897      0.995321    19.476669        CURVE
  29   26  5255.295561    13.123360     8.049995%    64.956268   248.386230     0.000000     0.000000     0.000000          0.995321      0.997807    19.798670       FINISH_(lead_out)
*/

	dump(_COURSE_DUMPNAME, true);

	instream = fopen(_COURSE_DUMPNAME, "rt");

	// skip to the data:
	char desc[256];

	while(1)  {
		cptr = fgets(buf, 255, instream);
		if (cptr==NULL)  {
			break;
		}

		if (indexIgnoreCase(buf, "description = ") != -1)  {
			n = sscanf(buf, "description = %s", desc);
			if (n!=1)  {
				strcpy(desc, get_full_path());
				strip_path(desc);
				strip_extension(desc);
			}
			else  {
				if (!strcmp(desc, "n/a"))  {
					strcpy(desc, get_full_path());
					strip_path(desc);
					strip_extension(desc);
				}
			}
		}
		else if (indexIgnoreCase(buf, "+++") != -1)  {
			break;
		}
	}


	while(1)  {
		cptr = fgets(buf, 255, instream);
		if (cptr==NULL)  {
			break;
		}
		if (indexIgnoreCase(buf, "---") != -1)  {						// done?
			break;
		}

		// read the data here:
		n = sscanf(buf, "%d %d %f %f %f%% %f %f %f %f %f %f %f %f %s",
			&tdata.i,
			&tdata.leg,
			&tdata.sd,
			&tdata.len,
			&tdata.grade,
			&tdata.elev,
			&tdata.heading,
			&tdata.bend,
			&tdata.swnd,
			&tdata.mwnd,
			&tdata.sd2,
			&tdata.ed,
			&tdata.endy,
			tdata.type
			);
		if (n != 14)  {
			FCLOSE(instream);
			return 1;
		}
		data.push_back(tdata);
	}

	FCLOSE(instream);

	n = data.size();

	outstream = fopen(_outfname, "wt");
	if (outstream==NULL)  {
		return 1;
	}

	fprintf(outstream, "\n");

	fprintf(outstream, "[COURSE HEADER]\n");
	if (metric)  {
		fprintf(outstream, "UNITS = METRIC\n");
	}
	else  {
		fprintf(outstream, "UNITS = ENGLISH\n");
	}
	fprintf(outstream, "DESCRIPTION = %s\n", desc);
	fprintf(outstream, "FILE NAME = %s\n", _outfname);
	fprintf(outstream, "[END COURSE HEADER]\n\n");

	fprintf(outstream, "[COURSE DATA]\n");

	//;DISTANCE	GRADE		WIND
	// distance is in feet or meters, wind is in mph or kph

	int i;

	for(i=0;i<n;i++)  {
		tdata = data[i];
		if (metric)  {
			fprintf(outstream, "%12.6f     %12.6f     %12.6f\n", data[i].len/1000.0f, data[i].grade, data[i].swnd);				// mwnd?
		}
		else  {
			fprintf(outstream, "%12.6f     %12.6f     %12.6f\n", data[i].len/5280.0f, data[i].grade, data[i].swnd);				// mwnd?
		}
	}

	fprintf(outstream, "[END COURSE DATA]\n\n");

	//fprintf(outstream, "; total 2d distance = %.6lf KM\n", total2DMeters / 1000.0);
	//fprintf(outstream, "; total 3d distance = %.6lf KM\n\n", total3DMeters / 1000.0);

	FCLOSE(outstream);
	_unlink(_COURSE_DUMPNAME);
	data.clear();

	return 0;
}

