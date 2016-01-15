#pragma once

/** Example 013 Render To Texture

This tutorial shows how to render to a texture using Irrlicht. Render to
texture is a feature with which it is possible to create nice special effects.
In addition, this tutorial shows how to enable specular highlights.

In the beginning, everything as usual. Include the needed headers, ask the user
for the rendering driver, create the Irrlicht Device:
*/

#include <irrlicht.h>
#include "CCustomTriangleSelector.h"
#include "CSkyDomeTiledSceneNode.h"
#include "CSkywallTiledSceneNode.h"
#include "CSpriteSceneNode.h"
#include "CShadowSceneNode.h"
#include "CDecalSceneNode.h"
#include "CCustomSceneNode.h"


/*
#ifdef _MSC_VER
#pragma comment(lib, "Irrlicht.lib")
#endif
*/
extern int camtype;

//Namespaces for the engine
using namespace irr;
using namespace core;
using namespace video;
using namespace scene;
using namespace gui;
using namespace io;

#define MAX_RIDERS 8

enum GAME_MODE {
	GAME_NORMAL,
	GAME_LEAD,
	GAME_MAX
};

enum MAIN_CAM {
	RIDER_CAM,
	MAYA_CAM,
	FPS_CAM,
	MAX_CAM
};

enum RIDER_COLORS {
	RIDER_BIKE=0,
	RIDER_TIRES,
	RIDER_SKIN,
	RIDER_HAIR,
	RIDER_HELMET,
	RIDER_JERSEY_TOP,
	RIDER_JERSEY_BOTTOM,
	RIDER_SHOES,
	RIDER_COLORS_MAX
};

enum CState {
	 PRESTART,
	 START_3,
	 START_2,
	 START_1,
	 START_0,	// Racing (Race Started)
	 START_NOW, // Bypass prestart and start the race
	 			
	 RACE,		// Racing (Race running)
	 PAUSE,
	 			 // 
	 FINISH,
	 DEMORIDER
};

enum FrustumIntersection
{
	FRUSTUM_INSIDE,
	FRUSTUM_OUTSIDE,
	FRUSTUM_INTERSECTS,
};

FrustumIntersection SphereInFrustum(const SViewFrustum * pvf, const vector3df &vPosition, float radius ) ;

vector3df SpringDamp(
	vector3df curPos,
	vector3df dstPos,
	vector3df prevDstPos,
	f32 deltaTime,
	f32 springK,
	f32 dampK,
	f32 springLen);

class Rider;
class D3DView;
class D3DBase
{
public:
	D3DBase(bool bWPF=true);
	virtual ~D3DBase(void);

    static bool Enter(bool bOut=false);
    static void Leave();

    static HRESULT Create(bool bWPF=true,HWND useHWND=NULL);
    static HRESULT Init(bool bWPF=true);
	static HRESULT UpdateTime(void);
    static HRESULT Update();
    static HRESULT Render();
    //static HRESULT Render1();
    static HRESULT Render2();
    static HRESULT Destroy();

    static HRESULT CreateShadow();
    static HRESULT CreateSpriteRider(int irider);

	static HRESULT RenderScene(void);
	static HRESULT PreRender(void);

    static HRESULT GetSurface(void **ppSurface);
	static HRESULT EnsureHWND();

	static gui::IGUIWindow * showAboutText();
	static HRESULT PreStartRace();
	static HRESULT StartRace();
	static HRESULT PauseRace();
	static HRESULT RunRace();
	static HRESULT ResetRace(DWORD time = 5000);
	static HRESULT SetRidersState(CState st);

	static HRESULT AddRider(int inum);
	static HRESULT AddRiderViews(int inum=1);
	static HRESULT ChangeRiderModel(int irider = 0);
	static HRESULT AddView();
	static HRESULT ChangeCam();
	static HRESULT SetNumViews(int num=1);
	static HRESULT ParseSysCmds(const char *cmdstring);
	static HRESULT ParseRiderCmds(int irider, const char *cmdstring);
	static HRESULT ParseGetSysCmds(const char *cmdstring, void **ptrresult);
	static HRESULT ParseGetRiderCmds(int irider, const char *cmdstring, void **ptrresult);
	static HRESULT ShowDemoRider(bool bShow);
	static HRESULT SetGameMode(int imode);

	static HRESULT LoadCourse(const char *coursename=NULL, int iscenery=-1);
	static HRESULT Convert(const char *name);

	static HRESULT LoadCourseProgress(void **ptrresult) {(*ptrresult) = (void*)(int)((m_fProgress + m_fBuild) * 100); return S_OK; }
	static HRESULT SavePerf(int irider, const char *perfpath, PerformanceInfo *ppi, PerfPoint **perf, UINT coursetype);

	static IrrlichtDevice *GetDevice() {return device;}
	static video::IVideoDriver* GetDriver(){return driver;}
	static gui::IGUIEnvironment* GetEnv(){return env;}
	static HWND GetWindow() {return m_hwnd;}
	static scene::ISceneManager* GetSceneManager() {return smgr;}
	static scene::ICameraSceneNode* GetMainCamera() {return ctrlcamera[camtype];}
	static void SetNumRiders(int num);
	static void GetFullWindowSize(UINT &w, UINT &h){w = m_uFullWidth; h = m_uFullHeight;}
	static void SetFullWindowSize(UINT w, UINT h) {m_uFullWidth = w; m_uFullHeight = h;}
	static void GetWindowSize(UINT &w, UINT &h){w = m_uWidth; h = m_uHeight;}
	static void SetWindowSize(UINT &w, UINT &h);
	static bool IsWPF() {return m_bWPF;}
	static void SetActive(int num = -1, bool bVisible = true);

	static ITriangleSelector* createTriangleSelector(const IMesh* mesh, ISceneNode* node);
	static CCustomSceneNode* addCustomSceneNode(ISceneNode* parent=0, s32 id=-1);
	static scene::ISceneNode* addSkyDomeSceneNode(ISceneNode* parent=0, s32 id=-1);

	static bool GetHeightFromWorld(vector3df& vpos, CCustomSceneNode *pnode);

//	static _Course GetCourse() {return m_pCourse;}
	static float m_fBuild;
	static float m_fProgress;

private:

	static bool m_bWPF;

    static HWND m_hwnd;

//	static _Course m_pCourse;

    static ECOLOR_FORMAT m_ColorFormat;
    static UINT m_uFullColorBit;

    static UINT m_uFullWidth;
    static UINT m_uFullHeight;
    static UINT m_uWidth;
    static UINT m_uHeight;
    static UINT m_uNumSamples;
    static bool m_fUseAlpha;
    static bool m_fSurfaceSettingsChanged;


	// ask user for driver
	static IrrlichtDevice *device;
	static video::E_DRIVER_TYPE driverType;
	static video::IVideoDriver* driver;
	static scene::ISceneManager* smgr;
	static scene::ISceneManager* smgr2;
	static gui::IGUIEnvironment* env;


	static scene::ICameraSceneNode* camera[MAX_RIDERS];
	static scene::ICameraSceneNode* ctrlcamera[MAX_CAM];

	// create test cube
	static scene::ISceneNode* test;
	// let the cube rotate and set some light settings
	static scene::ISceneNodeAnimator* anim;
	// create render target
	static video::ITexture* rt;
	static video::ITexture* mainrt;
	static video::ITexture* mainrt2;
	static scene::ICameraSceneNode* fixedCam;
	static gui::IGUISkin* skin;
	static gui::IGUIFont* font;
	static gui::IGUIStaticText* text;
	static gui::IGUIStaticText* text2;
	static int lastFPS;
	// display frames per second in window title
	static int fps;



public:
	//static scene::IAnimatedMeshSceneNode* fairy;

	static Rider *rider[MAX_RIDERS];
	static D3DView *view[MAX_RIDERS];
	static scene::IAnimatedMeshSceneNode* bikerider[MAX_RIDERS];
	static scene::IAnimatedMeshSceneNode* testrider[MAX_RIDERS];
	static int leadRider;
	static int prevLeadRider;
	static vector3df leadTarget;
	static vector3df prevLeadTarget;
	static bool bDraftingEnabled;
};

class D3DView 
{
public:
	//void *CreateView( int x, int y, int width, int height , bool show)
	D3DView(int x=0, int y=0, int width=1, int height=1, bool show=false, int iRider=-1, int iCamera=-1)
	{
		m_bActive = true;
		m_bShow = show;	
		m_ViewRect = rect<s32>(x,y,x+width, y+height);
		m_iCamera = iCamera;
		m_iRider = iRider;
	}

	~D3DView()
	{
	}

	//void MoveView( void *pview, int x, int y, int width, int height )
	void MoveView( int x, int y, int width, int height )
	{
		m_ViewRect = rect<s32>(x,y,x+width, y+height);
	}

	//void DeleteView( void *pview )
	void SetActive(bool active)
	{
		m_bActive = active;
	} 
	 
	bool GetActive()
	{
		return m_bActive;
	} 
	 
	//void ShowView( void *pview, bool show )
	void SetVisible(bool show )
	{
		m_bShow = show;	
	}
	 
	bool GetVisible()
	{
		return (m_bShow && m_bActive && m_iCamera >= 0 && m_iRider >= 0);	
	}
	 
	//const char *GetCamera( void *pview )
	int GetCameraIdx()
	{
		return m_iCamera;
	}
	 
	int GetRiderIdx()
	{
		return m_iRider;
	}

	//void SetCamera( void *pview, const char *cameraname, void *prider = NULL )
	void SetCameraIdx( int iCamera )
	{
		m_iCamera = iCamera;
	}
	void SetRiderIdx( int iRider )
	{
		m_iRider = iRider;
	}
	rect<s32> &GetViewRect()
	{
		return m_ViewRect;
	}

	bool m_bActive;
	bool m_bShow;
	int m_iRider;
	int m_iCamera;
	rect<s32> m_ViewRect;
};

class Tester
{
	int iView;
	int iCamera;
	int iRider;
	int iModel;
	char scratchBuf[MAX_PATH];

public:
	void Init(int imodel=7,int x=0, int y=0, int width=800, int height=600, bool show=true);
	int CreateView(int x, int y, int width, int height, bool show);
	int MoveView(int x, int y, int width, int height);
	int DeleteView();
	int ShowView();
	int GetCamera();
	int SetCamera(int iCamera);
	int GetRider();
	int SetRider(int iRider);
	int NextCameraRider();
	int NextCamera();
	int CreateRider();
	int DeleteRider();
	int SetRiderSpeed(float speed );
	int SetRiderRPM(float rpm);
	int SetRiderColor(int idx, int r, int g, int b);
	int SetRiderModel(int idx);
	int SetRiderDistance(float distance);
	bool IsRiderDrafting();
	char *GetSceneryName(int i);
	bool ShowDemoRider(bool bShow);
};



