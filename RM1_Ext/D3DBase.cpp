#include "StdAfx.h"
#include "D3DBase.h"
#include "globals.h"
#include "CCourse.h"

#define DOCOLLISION 
const static TCHAR szAppName[] = TEXT("RacerMateOne");

char gBasePath[MAX_PATH] = ".";

float collFactor = 1.8f;

HWND D3DBase::m_hwnd=0;
UINT D3DBase::m_uNumSamples=0;
bool D3DBase::m_fUseAlpha=false;
bool D3DBase::m_fSurfaceSettingsChanged=true;
ECOLOR_FORMAT D3DBase::m_ColorFormat=ECF_A8R8G8B8;
UINT D3DBase::m_uFullColorBit=32;
UINT D3DBase::m_uFullWidth=800;
UINT D3DBase::m_uFullHeight=600;
UINT D3DBase::m_uWidth=800;
UINT D3DBase::m_uHeight=600;
float D3DBase::m_fBuild=0.0f;
float D3DBase::m_fProgress=0.0f;


video::SExposedVideoData videodata;
IrrlichtDevice *D3DBase::device=0;
video::E_DRIVER_TYPE D3DBase::driverType=video::EDT_DIRECT3D9;;
video::IVideoDriver* D3DBase::driver=0;
scene::ISceneManager* D3DBase::smgr=0;
gui::IGUIEnvironment* D3DBase::env=0;
//scene::IAnimatedMeshSceneNode* D3DBase::fairy;
Rider *D3DBase::rider[MAX_RIDERS];
D3DView *D3DBase::view[MAX_RIDERS];
scene::IAnimatedMeshSceneNode* D3DBase::bikerider[MAX_RIDERS];
scene::IAnimatedMeshSceneNode* D3DBase::testrider[MAX_RIDERS];
int D3DBase::leadRider ;
int D3DBase::prevLeadRider;
vector3df D3DBase::leadTarget;
vector3df D3DBase::prevLeadTarget;
bool D3DBase::bDraftingEnabled = true;


scene::ICameraSceneNode* D3DBase::camera[MAX_RIDERS];
scene::ICameraSceneNode* D3DBase::ctrlcamera[MAX_CAM];
scene::ICameraSceneNode* maincamera=0;

scene::ISceneNode* D3DBase::test=0;
scene::ISceneNodeAnimator* D3DBase::anim=0;

video::ITexture* D3DBase::mainrt=0;
video::ITexture* D3DBase::mainrt2=0;

gui::IGUIImage *mainImg=0;
IImage *skyImg=0;
IImage *skyMapImg=0;
SMeshBuffer* skyBuffer = 0;

video::ITexture* D3DBase::rt=0;
scene::ICameraSceneNode* D3DBase::fixedCam=0;

gui::IGUISkin* D3DBase::skin=0;
gui::IGUIFont* D3DBase::font=0;
gui::IGUIStaticText* D3DBase::text=0;
gui::IGUIStaticText* D3DBase::text2=0;
int D3DBase::lastFPS;
int D3DBase::fps;

bool D3DBase::m_bWPF = true;
bool showinfo = true;
bool demomode = false;
bool bShowRiders = true;
bool bShowTrees = true;
bool bShowDome = true;
bool bShowCourse = true;
bool bShowScene = true;
bool bTest = false;
bool bRendering = false;
bool bBusy = false;
bool bDemoRider = false;
u32 holdChange = 0;
int gamemode = GAME_NORMAL; //GAME_LEAD; //
int mainTargetLane = 1;
u32 m_desiredLaneNeedChange = 0;

SColor BGColor = SColor(255,0,0,0);
//SColor BGColor = SColor(255,110,155,30);

scene::ICameraSceneNode* shadowCam = 0;
scene::ISceneNode* shadowCamNode = 0;
scene::ISceneCollisionManager* collMan = 0;
//scene::IMetaTriangleSelector * meta = 0;

//video::ITexture* riderShadow = 0;
bool initShadow = false;
//int tw = 512, th = 512;
//int inc_w = 64, inc_h = 64; 
//float inc_rot = 5.625f;
int tw = 512, th = 1024;
int inc_w = 128, inc_h = 128; 
float inc_rot = 11.25f; //5.625f;
//int tw = 1024, th = 1024;
//int inc_w = 128, inc_h = 128; 
//float inc_rot = 5.625f;
int sframes = 12; //24;
bool bRealShadows = false;
video::ITexture* riderShadows[24];
f32 sroty = 45.0f;
vector3df sscale = vector3df(1.18f,1.18f,1.18f);

int courseidx = -1;
const char *coursenames[] = {""};
/*
const char *coursenames[] = {
".\\media\\courses\\__CRS Test_crs.rmc", 
".\\media\\courses\\__CRS Test Mod_crs.rmc", 
".\\media\\courses\\serpent.3dc",
".\\media\\courses\\c.3dc",
".\\media\\courses\\__SimpleLoop2_3dc.rmc",
".\\media\\courses\\Last3D.rmc", 
".\\media\\courses\\2008 USAC Masters Championship Course (4.8M_7.8K)_3dc.rmc",
".\\media\\courses\\Six Gap Century (Loop_98M_157.6K)_3dc.rmc",
".\\media\\courses\\IM Coeur d'Alene One Loop (55.9M_90K)_3dc.rmc",
".\\media\\courses\\Killer Loop 1 Lap (2.67M_4.3K)_3dc.rmc",
".\\media\\courses\\Easyloop (1.9M_3.06K).3dc",
".\\media\\courses\\testA.3dc",
".\\media\\courses\\__SimpleLoop2_3dc.rmc",
".\\media\\courses\\__SimpleLoop_3dc.rmc",
".\\media\\courses\\Lastcrs.3dc",
".\\media\\courses\\Killer Loop 1 Lap (2.67M_4.3K)_3dc.rmc",
".\\media\\courses\\Killer Loop 1 Lap (2.67M_4.3K).3dc",
".\\media\\courses\\Easyloop (1.9M_3.06K)_3dc.rmp", 
".\\media\\courses\\__SimpleLoop.3dc",
".\\media\\courses\\Easyloop (1.9M_3.06K).3dc",
".\\media\\courses\\winsta32.3dc",
".\\media\\courses\\plain.3dc"};
*/
const io::IFileList *sceneryList;
int scenecnt=0;
int sceneidx=15;

vector3df countdownVec(-0.3f, -0.4f, -17.0f);
video::ITexture* three21 = 0;
f32 three21x[] = {256,64,0, .163f, .165f, .344f, .348f, .468f, .472f, 1};
int three21i = 4;

ISceneNode* courseNode = 0;
ISceneNode* treesNode = 0;
ISceneNode* skydome = 0;

core::stringw MessageText=L"";
core::stringw Caption=L"";

gui::IGUIWindow * msgbox = NULL;

scene::ISceneNode* customNode = 0;
scene::ISceneNode* shadowNode = 0;
scene::ISceneNode* mainNode = 0;

scene::ISceneNode* pBannerEnd = 0;
scene::ISceneNode* pBannerStart = 0;

scene::ISceneNode* pEndLine = 0;
scene::ISceneNode* pStartLine = 0;
scene::CDecalSceneNode* pEndNode = 0;
scene::CDecalSceneNode* pStartNode = 0;

scene::ISceneNode* SkyBox = 0;
scene::ISceneNode* lightNode = 0;
scene::ILightSceneNode* sunNode = 0;
CSpriteSceneNode * countDownSprite = 0;
CSpriteSceneNode * testSprite = 0;

bool testing;
f32 mult = 20.0f;

f32 m_MaxLeanAngle=MAX_LEAN_ANGLE;
f32 m_XMax = 1.4f;
f32 bikeLength = 1.0f; //1.8f;

float laneWidth = 0.4f;
float riderLanes[MAX_RIDERS] = { -1.4f, -1.0f, -0.6f, -0.2f, 0.2f, 0.6f, 1.0f, 1.4f}; 
int startLane[MAX_RIDERS] = { 2, 5, 3, 4, 0, 7, 1, 6}; 

int numRiders=1;
int targetLanes[MAX_RIDERS];
int orderDist[MAX_RIDERS];
int orderLane[MAX_RIDERS];

float speedFactor=0.3f;
int maxspeed=48;

CState state = PRESTART;
u32 demoStart = 0;
int numViews = 1;
int numriders = 1;
int numtest = 1; 
int camtype = RIDER_CAM; 

f32 demoAngleFOV = 45.0f;
#if 1
bool bToggleFOV = true;
f32 angleFOV = 45.0f;
f32 distCam = 10.0f;
f32 fardistCam = 200.0f;
f32 camh = 1.0f * mult;
float eyeAdjust = 5.19f;
#else
bool bToggleFOV = false;
f32 angleFOV = 15.0f; // 45.0f;
f32 distCam = 22.0f; // 10.0f;
f32 fardistCam = 300.0f; // 200.0f
f32 camh = 0.6f * mult;
float eyeAdjust = 12.0f;
#endif

f32 lastAngleFOV = angleFOV;
f32 distCamM = distCam * mult;
f32 lastn = distCam;
f32 angleLight = -30.0f;
f32 lastAngleLight = angleLight;
bool fogOn = false;
f32 fardist = fardistCam * mult;
f32 lastfardistCam = fardistCam;

SColor matColor(255,255,0,255);
SColor lastmatColor = matColor;
int matKey = 0;
RIDER_COLORS matType = RIDER_HELMET;
int scrollVal = 0;


//  RIDER_BIKE=0,
//  RIDER_TIRES,
//  RIDER_SKIN,
//  RIDER_HAIR,
//  RIDER_HELMET,
//  RIDER_JERSEY_TOP,
//  RIDER_JERSEY_BOTTOM,
//  RIDER_SHOES,
char *matNames[RIDER_COLORS_MAX] = {"Bike","Tire","Skin","Hair","Helmet","JerseyTop","JerseyBottom","Shoes"};
u32 matColors[RIDER_COLORS_MAX] = {0xff808080,0xff808080,0xff808080,0xff808080,0xff808080,0xff808080,0xff808080,0xff808080};

bool large = true; 
//bool large = false;
bool bInit=false;
bool bReady=false;
bool bChanged = true; 
bool bSceneNodeUpdated = false;



void Tester::Init(int imodel, int x, int y, int width, int height, bool show)
{
	
	iCamera = 0;
	iModel = imodel;
	iRider = CreateRider();
	iView = CreateView(x,y,width,height,show);
}
int Tester::CreateView(int x, int y, int width, int height, bool show)
{
	void *ptr = 0;
	sprintf(scratchBuf,"_CreateView %d,%d,%d,%d,%d,%d,%d", x, y, width, height, show ? 1 : 0, iRider, iCamera);
	D3DBase::ParseGetSysCmds(scratchBuf, &ptr);
	iView = (int)ptr;
	return iView;
}
int Tester::MoveView(int x, int y, int width, int height)
{

	void *ptr = 0;
	sprintf(scratchBuf,"_MoveView %d,%d,%d,%d,%d", iView, x, y, width, height);
	D3DBase::ParseGetSysCmds(scratchBuf, &ptr);
	return (int)ptr;
}
int Tester::DeleteView()
{

	void *ptr = 0;
	sprintf(scratchBuf,"_DeleteView %d", iView);
	D3DBase::ParseGetSysCmds(scratchBuf, &ptr);
	return (int)ptr;
}
int Tester::ShowView()
{

	void *ptr = 0;
	sprintf(scratchBuf,"_ShowView %d", iView);
	D3DBase::ParseGetSysCmds(scratchBuf, &ptr);
	return (int)ptr;
}
int Tester::GetCamera()
{

	void *ptr = 0;
	sprintf(scratchBuf,"_GetCameraView %d", iView);
	D3DBase::ParseGetSysCmds(scratchBuf, &ptr);
	return (int)ptr;
}
int Tester::GetRider()
{

	void *ptr = 0;
	sprintf(scratchBuf,"_GetRiderView %d", iView);
	D3DBase::ParseGetSysCmds(scratchBuf, &ptr);
	return (int)ptr;
}
int Tester::SetCamera(int iCamera)
{

	void *ptr = 0;
	sprintf(scratchBuf,"_SetCameraView %d,%d", iView, iCamera);
	D3DBase::ParseGetSysCmds(scratchBuf, &ptr);
	return (int)ptr;
}
int Tester::SetRider(int iCamera)
{

	void *ptr = 0;
	sprintf(scratchBuf,"_SetRiderView %d,%d", iView, iRider);
	D3DBase::ParseGetSysCmds(scratchBuf, &ptr);
	return (int)ptr;
}
int Tester::NextCameraRider()
{

	void *ptr = 0;
	sprintf(scratchBuf,"_NextCameraRider %d", iRider);
	D3DBase::ParseGetSysCmds(scratchBuf, &ptr);
	return (int)ptr;
}
int Tester::NextCamera()
{

	void *ptr = 0;
	sprintf(scratchBuf,"_NextCameraView %d", iView);
	D3DBase::ParseGetSysCmds(scratchBuf, &ptr);
	return (int)ptr;
}
int Tester::CreateRider()
{

	void *ptr = 0;
	sprintf(scratchBuf,"_CreateRider %d,%d,%d,%d,%d,%f,%f,%f,%ul",0, iModel, 0, 0, 0, 0, 0, 0, 0);
//      return (int)D3DBase::ParseGetSysCmds(string.Format("_CreateRider {0},{1},{2},{3},{4},{5},{6},{7}",
//          ridermodel, bikemodel, tiremodel, bike_frame_size, tire_size, rider_height, rider_weight, color));
	D3DBase::ParseGetSysCmds(scratchBuf, &ptr);
	iRider = (int)ptr;
	return iRider;
}
int Tester::DeleteRider()
{

	void *ptr = 0;
	sprintf(scratchBuf,"_DeleteRider %d", iRider);
	D3DBase::ParseGetSysCmds(scratchBuf, &ptr);
	return (int)ptr;
}
int Tester::SetRiderSpeed(float speed )
{

	void *ptr = 0;
	sprintf(scratchBuf,"_SetRiderSpeed %d,%f", iRider, speed);
	D3DBase::ParseGetSysCmds(scratchBuf, &ptr);
	return (int)ptr;
}
int Tester::SetRiderRPM(float rpm)
{

	void *ptr = 0;
	sprintf(scratchBuf,"_SetRiderRPM %d,%f", iRider, rpm);
	D3DBase::ParseGetSysCmds(scratchBuf, &ptr);
	return (int)ptr;
}
int Tester::SetRiderColor(int idx, int r, int g, int b)
{
	void *ptr = 0;
	sprintf(scratchBuf,"_SetRiderColor %d,%d,%d,%d,%d", iRider, idx, r, g, b);
	D3DBase::ParseGetSysCmds(scratchBuf, &ptr);
	return (int)ptr;
}
int Tester::SetRiderModel(int idx)
{
	void *ptr = 0;
	sprintf(scratchBuf,"_SetRiderModel %d,%d", iRider, idx);
	D3DBase::ParseGetSysCmds(scratchBuf, &ptr);
	return (int)ptr;
}
int Tester::SetRiderDistance(float distance)
{

	void *ptr = 0;
	sprintf(scratchBuf,"_SetRiderDistance %d,%f", iRider, distance);
	D3DBase::ParseGetSysCmds(scratchBuf, &ptr);
	return (int)ptr;
}
bool Tester::IsRiderDrafting()
{

	void *ptr = 0;
	sprintf(scratchBuf,"_IsRiderDrafting %d", iRider);
	D3DBase::ParseGetSysCmds(scratchBuf, &ptr);
	return (0 != (int)ptr);
}
char *Tester::GetSceneryName(int i)
{
	void *ptr = 0;
	sprintf(scratchBuf,"_GetSceneryName %d", i);
	D3DBase::ParseGetSysCmds(scratchBuf, &ptr);
	char *p = (char *)ptr;
	return p;
}
bool Tester::ShowDemoRider(bool bShow)
{
	void *ptr = 0;
	sprintf(scratchBuf,"_ShowDemoRider %d", bShow ? 1 : 0);
	D3DBase::ParseGetSysCmds(scratchBuf, &ptr);
	return (0 != (int)ptr);
}


/*
To get all the events sent by the GUI Elements, we need to create an event
receiver. This one is really simple. If an event occurs, it checks the id of
the caller and the event type, and starts an action based on these values. For
example, if a menu item with id GUI_ID_OPEN_MODEL was selected, if opens a file-open-dialog.
*/
class MyEventReceiver : public IEventReceiver
{
public:
	bool OnEvent(const SEvent& event)
	{
		// Escape swaps Camera Input
		if (event.EventType == EET_KEY_INPUT_EVENT &&
			event.KeyInput.PressedDown == false)
		{
			if ( OnKeyUp(event.KeyInput.Key,event.KeyInput.Control,event.KeyInput.Shift) )
				return true;
		}
		// Escape swaps Camera Input
		else if (event.EventType == EET_MOUSE_INPUT_EVENT &&
			event.MouseInput.Wheel != 0.0f)
		{
			if ( OnMouseWheel(event) )
				return true;
		}
		else if (event.EventType == EET_GUI_EVENT)
		{
			//s32 id = event.GUIEvent.Caller->getID();
			//IGUIEnvironment* env = Device->getGUIEnvironment();

			switch(event.GUIEvent.EventType)
			{
			case EGET_FILE_SELECTED:
				{
					// load the model file, selected in the file open dialog
					IGUIFileOpenDialog* dialog =
						(IGUIFileOpenDialog*)event.GUIEvent.Caller;
					stringc name = stringc(dialog->getFileName()).c_str();
					D3DBase::LoadCourse(name.c_str());
				}
				break;
			}
		}
		return false;
	}

	/*
		Handle key-up events
	*/
	bool OnMouseWheel(const SEvent& event)
	{ 
		f32 delta = (0.0f < event.MouseInput.Wheel ? 1.0f : -1.0f);
		if(event.MouseInput.Shift)
		{
			f32 tAngleFOV = angleFOV + delta;
			if(tAngleFOV > 60)
			{
				tAngleFOV = 60;
			}
			else if(tAngleFOV < 15)
			{
				tAngleFOV = 15;
			}
			if(tAngleFOV != angleFOV)
			{
				angleFOV = tAngleFOV;               
				bChanged = true;
				return true;
			}
		}
		if(event.MouseInput.Control)
		{
			f32 tN = distCam + delta;
			if(tN > 20)
			{
				tN = 20;
			}
			else if(tN < 1)
			{
				tN = 1;
			}
			if(tN != distCam)
			{
				distCam = tN;               
				distCamM = distCam * mult;              
				bChanged = true;
				return true;
			}
		}
		switch(scrollVal)
		{
		case 0:
			{
				int t;
				if(matKey == 0)
				{
					t = matColor.getRed();
					t += (int)delta;
					if(t > 0xFF)
						t = 0xFF;
					else if(t < 0)
						t = 0;
					matColor.setRed(t); 
				}
				else
				if(matKey == 1)
				{
					t = matColor.getGreen();
					t += (int)delta;
					if(t > 0xFF)
						t = 0xFF;
					else if(t < 0)
						t = 0;
					matColor.setGreen(t); 
				}
				else
				{
					t = matColor.getBlue();
					t += (int)delta;
					if(t > 0xFF)
						t = 0xFF;
					else if(t < 0)
						t = 0;
					matColor.setBlue(t); 
				}
				bChanged = true;
				return true;
			}
			break;
		case 1:
			{
				f32 tAngleLight = angleLight + delta;
				if(tAngleLight > 180)
				{
					tAngleLight = 180;
				}
				else if(tAngleLight < -180)
				{
					tAngleLight = -180;
				}
				if(tAngleLight != angleLight)
				{
					angleLight = tAngleLight;               
					bChanged = true;
					return true;
				}
			}
			break;
		case 2:
			{
				f32 tfardistCam = fardistCam + delta;
				if(tfardistCam != fardistCam)
				{
					fardistCam = tfardistCam;               
					bChanged = true;
					return true;
				}
			}
			break;
		default:
			break;
		}
		return false;
	}

	/*
		Handle key-up events
	*/
	bool OnKeyUp(irr::EKEY_CODE keyCode, bool bCtrl, bool bShift)
	{
		
		// any key
		if(msgbox && msgbox->isVisible())
		{
			msgbox->setVisible(false);
			return true;
		}

		switch (keyCode)
		{
		case irr::KEY_KEY_0:
			bShowScene = !bShowScene;
			return true;
			break;

		case irr::KEY_KEY_1:
		case irr::KEY_KEY_2:
		case irr::KEY_KEY_3:
		case irr::KEY_KEY_4:
		case irr::KEY_KEY_5:
		case irr::KEY_KEY_6:
		case irr::KEY_KEY_7:
		case irr::KEY_KEY_8:
			{
				char scratchBuf[MAX_PATH];
				if(bCtrl)
				{
					//D3DBase::SetNumViews((keyCode - irr::KEY_KEY_1) + 1);
					sprintf(scratchBuf,"view %d",(keyCode - irr::KEY_KEY_1) + 1);
					D3DBase::ParseSysCmds(scratchBuf);
				}
				else
				{
					//D3DBase::AddRiderViews((keyCode - irr::KEY_KEY_1) + 1);
					sprintf(scratchBuf,"+riderviews %d",(keyCode - irr::KEY_KEY_1) + 1);
					D3DBase::ParseSysCmds(scratchBuf);
				}
				return true;
			}
			break;

		case irr::KEY_KEY_9:
			bTest = !bTest;
			
			return true;
			break;

		case irr::KEY_PLUS:
			{
				matKey++;
				if(matKey > 2)
					matKey = 0;
				bChanged = true;
			}
			return true;
			break;

		case irr::KEY_MINUS:
			//if(false)
			{
				int t;
				matColors[matType] = matColor.color;
				t = matType;

				if(t == RIDER_HELMET)
					t = RIDER_SKIN;
				else if(t == RIDER_SKIN)
					t = RIDER_HAIR;
				else if(t == RIDER_HAIR)
					t = RIDER_HELMET;
				else
					t = RIDER_HELMET;

				matType = (RIDER_COLORS)t;
				matColor.color = matColors[matType];
				lastmatColor = matColor;
				bChanged = true;
				return true;
			}
			break;

		case irr::KEY_KEY_A:
			{
				char scratchBuf[MAX_PATH];
				int inum=numriders + 1;
				if(inum > MAX_RIDERS) inum=1;
				//D3DBase::AddRider(inum);
				sprintf(scratchBuf,"+rider %d",inum);
				D3DBase::ParseSysCmds(scratchBuf);
				return true;
			}
			break;

		case irr::KEY_KEY_B:
			{
				bShowDome = !bShowDome;
				skydome->setVisible(bShowDome);
			}
			return true;
			break;

		case irr::KEY_KEY_C:
			D3DBase::ChangeCam();
			return true;
			break;

		case irr::KEY_KEY_D:
			{
				demomode = !demomode;
				D3DBase::ResetRace(300);
			}
			return true;
			break;

		case irr::KEY_KEY_F:
			{
				void *ptr = 0;
				char scratchBuf[MAX_PATH];
				sprintf(scratchBuf,"_NextCameraRider %d", 0);
				D3DBase::ParseGetSysCmds(scratchBuf, &ptr);
				return true;
			}
			break;

		case irr::KEY_KEY_G:
			D3DBase::PreStartRace();
			return true;
			break;

		case irr::KEY_KEY_J:
			{
				bShowRiders = !bShowRiders;
				D3DBase::SetActive(-1,bShowRiders);
			}
			return true;
			break;

		case irr::KEY_KEY_L:
			{
				if(bShift)
				{
					if(bCtrl)
					{
						sceneidx--;
						if(sceneidx < 0)
							sceneidx = scenecnt-1;
						D3DBase::LoadCourse();
					}
					else
					{
						courseidx--;
						if(courseidx < 0)
							courseidx = 4;
						D3DBase::LoadCourse();
					}
				}
				else
				{
					if(bCtrl)
					{
						sceneidx++;
						if(sceneidx >= scenecnt)
							sceneidx = 0;
						D3DBase::LoadCourse();
					}
					else
					{
						courseidx++;
						if(courseidx > 4)
							courseidx = 0;
						D3DBase::LoadCourse();
					}
				}
			}
			return true;
			break;

		case irr::KEY_KEY_O:
			{
				if(bCtrl)
				{
					D3DBase::GetEnv()->addFileOpenDialog(L"Please choose a course file.");
				}
				else
				{

					void *ptr = 0;
					char scratchBuf[MAX_PATH];
					sprintf(scratchBuf,"_SetRiderModel %d,%d", 0, rand() % MODEL_MAX);
					D3DBase::ParseGetSysCmds(scratchBuf, &ptr);
	/*
					void *ptr = 0;
					char scratchBuf[MAX_PATH];
					sprintf(scratchBuf,"_SetRiderColor %d,%d,%d,%d,%d", 0, rand() % RIDER_COLORS_MAX, rand() % 256, rand() % 256, rand() % 256);
					D3DBase::ParseGetSysCmds(scratchBuf, &ptr);
					*/
				}

				return true;
			}
			break;

		case irr::KEY_KEY_M:
			{
				void *ptr = 0;
				char scratchBuf[MAX_PATH];

				int inum = gamemode + 1;
				if(inum >= GAME_MAX)
					inum = GAME_NORMAL;
				sprintf(scratchBuf,"_SetGameMode %d", inum);
				D3DBase::ParseGetSysCmds(scratchBuf, &ptr);
			}
			break;

		case irr::KEY_KEY_N:
			{
				/*
				char scratchBuf[MAX_PATH];
				int inum=numriders + 1;
				if(inum > MAX_RIDERS) inum=1;
				sprintf(scratchBuf,"+rider %d",inum);
				D3DBase::ParseSysCmds(scratchBuf);
				return true;
				*/
			}
			break;

		case irr::KEY_KEY_P:
			D3DBase::PauseRace();
			return true;
			break;

		case irr::KEY_KEY_R:
			D3DBase::ResetRace(300);
			return true;
			break;

		case irr::KEY_KEY_H:
			{
				char scratchBuf[MAX_PATH];
				sprintf(scratchBuf,"toggleFOV");
				D3DBase::ParseSysCmds(scratchBuf);
			}
			return true;
			break;

		case irr::KEY_KEY_T:
			D3DBase::ChangeRiderModel();
			return true;
			break;

		case irr::KEY_KEY_U:
			{
				bDemoRider = !bDemoRider;
				void *ptr = 0;
				char scratchBuf[MAX_PATH];
				sprintf(scratchBuf,"_ShowDemoRider %d", bDemoRider ? 1 : 0);
				D3DBase::ParseGetSysCmds(scratchBuf, &ptr);
				//bShowTrees = !bShowTrees;
				//treesNode->setVisible(bShowTrees);
			}
			return true;
			break;

		case irr::KEY_KEY_V:
			{
				char scratchBuf[MAX_PATH];
				//D3DBase::AddView();
				sprintf(scratchBuf,"+view");
				D3DBase::ParseSysCmds(scratchBuf);
				return true;
			}
			break;

		case irr::KEY_KEY_X:
			scrollVal++;
			if(scrollVal > 2)
				scrollVal = 0;
			return true;
			break;

		case irr::KEY_KEY_Z:
			{
				bShowCourse = !bShowCourse;
				if(gpCourse)
				{
					courseNode = gpCourse->GetCourseNode();
					if(courseNode)
						courseNode->setVisible(bShowCourse);
				}
			}
			return true;
			break;

		case irr::KEY_ESCAPE:
			camtype=MAYA_CAM;
			bChanged = true;
			return true;
			break;

		case irr::KEY_SPACE:
			{
				if(msgbox && !msgbox->isVisible())
					msgbox->setVisible(true);
			}
			return true;
			break;
			
		default:
			break;
		}
		return false;
	}
};

vector3df SpringDamp(
	vector3df curPos,
	vector3df dstPos,
	vector3df prevDstPos,
	f32 deltaTime,
	f32 springK,
	f32 dampK,
	f32 springLen)
{
	vector3df disp;
	vector3df vel;
	f32 forceMag;

	//Calc spring force
	disp = curPos - dstPos;
	vel = (prevDstPos - dstPos) * deltaTime;
	f32 displen = disp.getLength();
	forceMag = 0;
	if(displen > 0)
		forceMag = springK * (springLen - displen) + dampK * (disp.dotProduct(vel) / displen);
	// apply spring force
	disp.normalize();
	disp *= forceMag * deltaTime;
	return curPos += disp;
}

/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 
Summary: Checks whether a sphere is inside the camera’s view frustum. 
Parameters: 
[in] pPosition - Position of the sphere. 
[in] radius - Radius of the sphere. 
Returns: TRUE if the sphere is in the frustum, FALSE otherwise 
* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */ 
FrustumIntersection SphereInFrustum(const SViewFrustum * pvf, const vector3df &vPosition, float radius ) 
{
	int p;
	for ( p = 0; p < 6; p++ ) 
	{
		float dist = pvf->planes[p].getDistanceTo(vPosition);
		if(p < 2)
			dist = -dist;
		// Outside the frustum, reject it! 
		if ( dist + radius < 0) 
			return FRUSTUM_OUTSIDE;
		// if all distance from inside is less than inradius, then it is completely inside 
		if (fabs(dist) < radius)
			return FRUSTUM_INTERSECTS;
	} 
	return FRUSTUM_INSIDE;
}


MyEventReceiver receiver;


D3DBase::D3DBase(bool bWPF)
{
	Init(m_bWPF);
}

D3DBase::~D3DBase(void)
{
}

//+-----------------------------------------------------------------------------
//
//  Member:
//      CRendererManager::EnsureHWND
//
//  Synopsis:
//      Makes sure an HWND exists if we need it
//
//------------------------------------------------------------------------------
HRESULT
D3DBase::EnsureHWND()
{
	HRESULT hr = S_OK;

	if (!m_hwnd)
	{
		WNDCLASS wndclass;

		wndclass.style = CS_HREDRAW | CS_VREDRAW ;
		wndclass.lpfnWndProc = DefWindowProc;
		wndclass.cbClsExtra = 0;
		wndclass.cbWndExtra = 0;
		wndclass.hInstance = NULL;
		wndclass.hIcon = LoadIcon(NULL, IDI_APPLICATION);
		wndclass.hCursor = LoadCursor(NULL, IDC_ARROW);
		wndclass.hbrBackground = (HBRUSH) GetStockObject (WHITE_BRUSH);
		wndclass.lpszMenuName = NULL;
		wndclass.lpszClassName = szAppName;

		if (!RegisterClass(&wndclass))
		{
			IFC(E_FAIL);
		}

		m_hwnd = CreateWindow(szAppName,
							TEXT("RacerMateOne"),
							WS_OVERLAPPEDWINDOW ,
							0,                   // Initial X
							0,                   // Initial Y
							0,                   // Width
							0,                   // Height
							NULL,
							NULL,
							NULL,
							NULL);
	}

Cleanup:
	return hr;
}

HRESULT D3DBase::Init(bool bWPF)
{
	HRESULT hr = S_OK;
	m_bWPF = bWPF;   
	m_hwnd=NULL;
	m_uWidth=800;
	m_uHeight=600;
	m_uNumSamples=0;
	m_fUseAlpha=false;
	m_fSurfaceSettingsChanged=true;

	device=NULL;
	driverType = video::EDT_DIRECT3D9;
	driver=NULL;
	smgr=NULL;
	env=NULL;
	for (int i=0; i<MAX_RIDERS; i++)
	{
		camera[i]=0;
		rider[i]=NULL;
	}
	maincamera=NULL;
	test=NULL;
	anim=NULL;
	rt=NULL;
	mainrt=NULL;
	mainrt2=NULL;
	fixedCam=NULL;
	text=NULL;
	skin=NULL;
	font=NULL;
	lastFPS = -1;
	bReady=false;
	bInit=false;
	return hr;
}

HRESULT D3DBase::Destroy(void)
{
	HRESULT hr = S_OK;
	delete gpCourse;
	gpCourse = NULL;
	if(device)
		device->drop();
	device=0;
	if(m_hwnd)
		CloseWindow(m_hwnd);
	m_hwnd=0;
	Init(m_bWPF);
	return hr;
}

/*
The three following functions do several stuff used by the mesh viewer. The
first function showAboutText() simply displays a messagebox with a caption and
a message text. The texts will be stored in the MessageText and Caption
variables at startup.
*/
gui::IGUIWindow * D3DBase::showAboutText()
{
	// create modal message box with the text
	// loaded from the xml file.
	core::stringw str = L"Version ";
	str.append(core::stringw(gVersionNum));
	str += L"\n";
	str += MessageText;

	return D3DBase::device->getGUIEnvironment()->addMessageBox(
		Caption.c_str(), str.c_str(), true, NULL);
}

void D3DBase::SetNumRiders(int num) 
{
	numriders = num;
	for(int i = 0; i<MAX_RIDERS; i++)
	{
		if(i < num)
			rider[i]->setVisible(true);
		else
			rider[i]->setVisible(false);
	}
}

void D3DBase::SetActive(int num, bool bVisible) 
{
	if(num < 0)
	{
		for(int i = 0; i<MAX_RIDERS; i++)
		{
			if(i < numriders)
				rider[i]->setVisible(bVisible);
			else
				rider[i]->setVisible(false);
		}
	}
	else
	{
		rider[num]->setVisible(bVisible);
	}
}


HRESULT D3DBase::PreStartRace()
{
	HRESULT hr = S_OK;
	if(state == PRESTART)
	{
		state = START_3;
		three21i = 0;
		countDownSprite->setPosition(vector3df(0,0,20.0f));
		countDownSprite->setMaterialTexture(0, three21);
		countDownSprite->setVisible(true);

		demoStart = gFTime.simTime + 3000;
		for (int i = 0; i < MAX_RIDERS; i++) 
		{
			rider[i]->m_state = state;
			rider[i]->m_speed = 0;
		}
	}
	else 
		StartRace();

	return hr;
}

HRESULT D3DBase::StartRace()
{
	HRESULT hr = S_OK;
	if(state == RACE)
	{
		state = PAUSE;
	}
	else if(state == PAUSE)
	{
		state = RACE;
	}
	else if(state < RACE)
	{
		state = START_NOW;
	}
	else
	{
		ResetRace();
	}
	SetRidersState(state);
	return hr;
}

HRESULT D3DBase::PauseRace()
{
	HRESULT hr = S_OK;
	if(state == RACE)
	{
		state = PAUSE;
	}
	SetRidersState(state);
	return hr;
}

HRESULT D3DBase::RunRace()
{
	HRESULT hr = S_OK;
	if(state == PAUSE)
	{
		state = RACE;
	}
	SetRidersState(state);
	return hr;
}

HRESULT D3DBase::SetRidersState(CState st)
{
	HRESULT hr = S_OK;
	for (int i = 0; i < MAX_RIDERS; i++) 
	{
		rider[i]->SetState(st);
	}
	return hr;
}


HRESULT D3DBase::ResetRace(DWORD time)
{
	HRESULT hr = S_OK;
	if(bDemoRider)
	{
		state = DEMORIDER;
		demoStart = 0;
	}
	else
	{
		switch(gamemode)
		{
		default:
		case GAME_LEAD:
		case GAME_NORMAL:
			state = PRESTART;
			demoStart = gFTime.simTime + time;
			break;
		}
	}
	for (int i = 0; i < MAX_RIDERS; i++) 
	{
		rider[i]->Reset(state);
	}
	return hr;
}

HRESULT D3DBase::AddRider(int inum)
{
	HRESULT hr = S_OK;
	numriders = max(min(inum,MAX_RIDERS),1);
	for(int i = 0; i<MAX_RIDERS; i++)
	{
		if(i < numriders)
		{
			if(!rider[i]->isVisible())
			{
				numtest++;
				if(numtest>=MAX_RIDER_TYPES)
					numtest=1;
				rider[i]->useRider(numtest);
				rider[i]->setVisible(true);
				rider[i]->m_bShow=true;

				rider[i]->m_Colors[RIDER_JERSEY_TOP]= SColor(255,rand() % 255 + 1,rand() % 255 + 1,rand() % 255 + 1).color;
				rider[i]->m_Colors[RIDER_JERSEY_BOTTOM]= SColor(255,rand() % 255 + 1,rand() % 255 + 1,rand() % 255 + 1).color;
				rider[i]->m_Colors[RIDER_BIKE]= SColor(255,rand() % 255 + 1,rand() % 255 + 1,rand() % 255 + 1).color;
				rider[i]->m_Colors[RIDER_HELMET]= SColor(255,rand() % 255 + 1,rand() % 255 + 1,rand() % 255 + 1).color;
				int skin = rand() % 180 + 60;
				rider[i]->m_Colors[RIDER_SKIN]= SColor(255,skin,skin,skin).color;
			}
		}
		else
		{
			rider[i]->useRider(0);
			rider[i]->setVisible(false);
			rider[i]->m_bShow=false;
		}
	}
	bChanged = true;
	return hr;
}

HRESULT D3DBase::AddRiderViews(int inum)
{
	HRESULT hr = S_OK;
	inum = max(min(inum,MAX_RIDERS),1);
	numViews = inum;
	numriders = inum;
	for(int i = 0; i<MAX_RIDERS; i++)
	{
		if(i < numriders)
		{
			if(!rider[i]->isVisible())
			{
				numtest++;
				if(numtest>=MAX_RIDER_TYPES)
					numtest=1;
				rider[i]->useRider(numtest);
				rider[i]->setVisible(true);
				rider[i]->m_bShow=true;

				rider[i]->m_Colors[RIDER_JERSEY_TOP]= SColor(255,rand() % 255 + 1,rand() % 255 + 1,rand() % 255 + 1).color;
				rider[i]->m_Colors[RIDER_JERSEY_BOTTOM]= SColor(255,rand() % 255 + 1,rand() % 255 + 1,rand() % 255 + 1).color;
				rider[i]->m_Colors[RIDER_BIKE]= SColor(255,rand() % 255 + 1,rand() % 255 + 1,rand() % 255 + 1).color;
				rider[i]->m_Colors[RIDER_HELMET]= SColor(255,rand() % 255 + 1,rand() % 255 + 1,rand() % 255 + 1).color;
				int skin = rand() % 180 + 60;
				rider[i]->m_Colors[RIDER_SKIN]= SColor(255,skin,skin,skin).color;
			}
		}
		else
		{
			rider[i]->useRider(0);
			rider[i]->setVisible(false);
			rider[i]->m_bShow=false;
		}
	}
	bChanged = true;
	return hr;
}

HRESULT D3DBase::ChangeRiderModel(int i)
{
	HRESULT hr = S_OK;
	numtest++;
	if(numtest>=MAX_RIDER_TYPES)
		numtest=0;
	
	D3DBase::rider[i]->useRider(numtest);

	rider[i]->m_Colors[RIDER_JERSEY_TOP]= SColor(255,rand() % 255 + 1,rand() % 255 + 1,rand() % 255 + 1).color;
	rider[i]->m_Colors[RIDER_JERSEY_BOTTOM]= SColor(255,rand() % 255 + 1,rand() % 255 + 1,rand() % 255 + 1).color;
	rider[i]->m_Colors[RIDER_BIKE]= SColor(255,rand() % 255 + 1,rand() % 255 + 1,rand() % 255 + 1).color;
	rider[i]->m_Colors[RIDER_HELMET]= SColor(255,rand() % 255 + 1,rand() % 255 + 1,rand() % 255 + 1).color;
	int skin = rand() % 180 + 60;
	rider[i]->m_Colors[RIDER_SKIN]= SColor(255,skin,skin,skin).color;
	bChanged = true;
	return hr;
}


HRESULT D3DBase::AddView()
{
	HRESULT hr = S_OK;
	numViews++;
	if(numViews > numriders)
		numViews = 1;
	return hr;
}

HRESULT D3DBase::ChangeCam()
{
	HRESULT hr = S_OK;
	camtype++;
	if(camtype>=MAX_CAM)
		camtype=RIDER_CAM;
	bChanged = true;
	return hr;
}

HRESULT D3DBase::SetNumViews(int num)
{
	HRESULT hr = S_OK;
	numViews = num;
	//if(numViews >= numriders)
	//  numViews = numriders;
	return hr;
}

HRESULT D3DBase::SavePerf(int irider, const char *perfpath, PerformanceInfo *ppi, PerfPoint **perf, UINT coursetype)
{
	HRESULT hr = -1L;
	strcpy(gScratchBuf, perfpath);
	//strcat(gScratchBuf, "\\LastPerf.3dp");
	
	if(!gpCourse)
		gpCourse = new CCourse(false,false,"standard");

	hr = gpCourse->SavePerf(irider, gScratchBuf, ppi, *perf, coursetype);
	return hr;
}

HRESULT D3DBase::Convert(const char *name)  
{
	HRESULT hr = -1L;

	unsigned char *pbuf;
	int version;

	IReadFile *file;
	file = D3DBase::GetDevice()->getFileSystem()->createAndOpenFile(name);
	if (!file)  
		return hr;
	s32 len; 
	len = file->getSize();
	pbuf = new unsigned char[len+1];

	if(4 != file->read(&version, 4))
	{
		file->drop();
		return hr;
	}
	len -= 4;
	if (version != 7)  
	{
		file->drop();
		return hr;
	}
	if(len != file->read(pbuf, len))
	{
		file->drop();
		return hr;
	}
	file->drop();

	version = 6;
	{
		CRF crf;
		crf.init();
		crf.doo(pbuf, len);

		IWriteFile *wfile;
		wfile = D3DBase::GetDevice()->getFileSystem()->createAndWriteFile(name);
		if (!wfile)  
			return hr;

		if(4 != wfile->write(&version, 4))
		{
			wfile->drop();
			return hr;
		}
		if(len != wfile->write(pbuf, len))
		{
			wfile->drop();
			return hr;
		}
		wfile->drop();
	}
	return S_OK;
}

HRESULT D3DBase::LoadCourse(const char *coursename, int iscenery)
{
	HRESULT hr = S_OK;
	Enter();

	const fschar_t* scenename = "standard";
	if(iscenery < 0)
		iscenery = sceneidx;

	core::stringc scenepath, basepath, fname, fext;
	if(sceneryList && iscenery >= 0 && iscenery < scenecnt)
	{
		scenepath = sceneryList->getFullFileName(iscenery);
		splitFilename(scenepath, &basepath, &fname, &fext);
		scenename = fname.c_str();
	}

	m_fProgress = 0.0f;
	m_fBuild = 0.0f;

	if(!gpCourse)
		gpCourse = new CCourse(false,false,scenename);

	if(gpCourse) 
	{
		m_fProgress = 0.1f;

		gpCourse->ClearModels();
		courseNode = 0;

		if(skydome)
			skydome->remove();
		skydome=0;
		
		m_fProgress = 0.2f;

		const char *loadname = coursenames[courseidx < 0 ? 0 : courseidx];
		if(coursename)
			loadname = coursename;

		gpCourse->SetLandscape2(scenename);

		m_fProgress = 0.3f;

		if(gpCourse->LoadAny(loadname))
		{
			m_fProgress = 0.4f;

			m_fBuild = 0.0f;

			for(int i=0; i<MAX_RIDERS; i++)
			{
				rider[i]->ClearModel();
			}
			skydome = addSkyDomeSceneNode(mainNode);
			gpCourse->ReadyRace();
			gpCourse->ReadyModel();
			
#if 0 //def DOCOLLISION
			{ // create collison selectors
				// Create a meta triangle selector to hold several triangle selectors.
				/*
				Now we will find all the nodes in the scene and create triangle
				selectors for all suitable nodes.  Typically, you would want to make a
				more informed decision about which nodes to performs collision checks
				on; you could capture that information in the node name or Id.
				*/
				core::array<scene::ISceneNode *> nodes;
				smgr->getSceneNodesFromType(scene::ESNT_MESH, nodes, gpCourse->GetCourseNode()); // Find all nodes

				for (u32 i=0; i < nodes.size(); ++i)
				{
					scene::ISceneNode * node = nodes[i];
					scene::ITriangleSelector * selector = 0;

					if(node->getID() == 2000)
					{
						IMesh* imesh = ((scene::IMeshSceneNode*)node)->getMesh();
						if(imesh && 0 > imesh->getMeshBufferCount())
							selector = smgr->createTriangleSelector(imesh, node);
					}

					if(selector)
					{
						// Add it to the meta selector, which will take a reference to it
						meta->addTriangleSelector(selector);
						// And drop my reference to it, so that the meta selector owns it.
						selector->drop();
					}
				}
			}
#endif
			m_fBuild = 0.5f;

			m_fProgress = 0.45f;

			vector3df vpos, vvec, vrot;
			int hint = 0;
			float yaw, secrot,wind, roadgrade;

			gpCourse->RoadLoc(0.1f, vpos, vvec, roadgrade, hint, &yaw, &secrot, &wind);
			if(!pBannerStart)
			{
				pBannerStart = smgr->addMeshSceneNode(smgr->getMesh("gate.X"),mainNode);
				pBannerStart->setScale(core::vector3df(mult*1.8f,mult*1.5f,mult*1.5f));
				pBannerStart->setMaterialFlag(video::EMF_NORMALIZE_NORMALS, true);
				pBannerStart->setMaterialFlag(video::EMF_LIGHTING, true); // enable dynamic lighting
				pBannerStart->setMaterialFlag(video::EMF_FOG_ENABLE, fogOn);

				video::SMaterial *psmat;
				psmat = &pBannerStart->getMaterial(1);
				psmat->setTexture(0,driver->getTexture("Banner_Start.tga"));
			}
			vrot = pBannerStart->getRotation();
			vrot.y = radToDeg(yaw);
			pBannerStart->setRotation(vrot);
			pBannerStart->setPosition(vpos*mult);
			pBannerStart->updateAbsolutePosition();

			f32 endLine = gpCourse->EndAt() - gpCourse->StartAt();
			if(endLine <= 0)
				endLine = gpCourse->GetCourseLength();
			gpCourse->RoadLoc(endLine-0.1f, vpos, vvec, roadgrade, hint, &yaw, &secrot, &wind);
			if(!pBannerEnd)
			{
				pBannerEnd = smgr->addMeshSceneNode(smgr->getMesh("gate.X"),mainNode);
				pBannerEnd->setScale(core::vector3df(mult*1.8f,mult*1.5f,mult*1.5f));
				pBannerEnd->setMaterialFlag(video::EMF_NORMALIZE_NORMALS, true);
				pBannerEnd->setMaterialFlag(video::EMF_LIGHTING, true); // enable dynamic lighting
				pBannerEnd->setMaterialFlag(video::EMF_FOG_ENABLE, fogOn);
				//pBannerEnd->setMaterialTexture(0, driver->getTexture("finish.bmp"));

				video::SMaterial *psmat;
				psmat = &pBannerEnd->getMaterial(1);
				psmat->setTexture(0,driver->getTexture("Banner_Finish.tga"));
			}

			vrot = pBannerEnd->getRotation();
			vrot.y = radToDeg(yaw);
			pBannerEnd->setRotation(vrot);
			pBannerEnd->setPosition(vpos*mult);
			pBannerEnd->updateAbsolutePosition();

			bool uselight = false;
			gpCourse->RoadLoc(-2, vpos, vvec, roadgrade, hint, &yaw, &secrot, &wind);
			if(!pStartLine)
			{
				pStartLine = D3DBase::addCustomSceneNode(mainNode);
				pStartNode = new CDecalSceneNode(pStartLine, D3DBase::GetSceneManager(), 
					-1, vector3df(0,0,0), dimension2d<f32>(mult*4.0f,mult*4.0f), rect<f32>(0.01f,0.01f,0.99f,0.99f),
					SColor(255,153,153,153),SColor(255,153,153,153));
				pStartNode->drop();
				pStartNode->setRotation(vector3df(0,90,0));
				pStartNode->updateAbsolutePosition();
				//pStartNode->setMaterialFlag(video::EMF_LIGHTING, false); 
				pStartNode->setMaterialFlag(video::EMF_ANISOTROPIC_FILTER, true);
			}
			pStartLine->setPosition(vector3df(vpos.x*mult,(vpos.y*mult)+0.3f,vpos.z*mult));
			pStartLine->setRotation(vector3df(-roadgrade*60,radToDeg(yaw),0));
			pStartLine->updateAbsolutePosition();

			pStartNode->setMaterialTexture(0, D3DBase::GetDriver()->getTexture(gpCourse->getTex(CCourse::TEX_START)));
			/*
			pStartNode->setMaterialFlag(video::EMF_FOG_ENABLE, fogOn);
			pStartNode->setMaterialFlag(video::EMF_BACK_FACE_CULLING, true);
			pStartNode->setMaterialFlag(video::EMF_NORMALIZE_NORMALS,uselight);
			pStartNode->setMaterialFlag(video::EMF_LIGHTING,uselight);
			pStartNode->setMaterialType(video::EMT_SOLID);
			*/

			gpCourse->RoadLoc(endLine+2, vpos, vvec, roadgrade, hint, &yaw, &secrot, &wind);
			if(!pEndLine)
			{
				pEndLine = D3DBase::addCustomSceneNode(mainNode);
				pEndNode = new CDecalSceneNode(pEndLine, D3DBase::GetSceneManager(), 
					-1, vector3df(0,0,0), dimension2d<f32>(mult*4.0f,mult*4.0f), rect<f32>(0.01f,0.01f,0.99f,0.99f),
					SColor(255,153,153,153),SColor(255,153,153,153));
				pEndNode->drop();
				pEndNode->setRotation(vector3df(0,90,0));
				pEndNode->updateAbsolutePosition();
				//pEndNode->setMaterialFlag(video::EMF_LIGHTING, false); 
				pEndNode->setMaterialFlag(video::EMF_ANISOTROPIC_FILTER, true);
			}
			pEndLine->setPosition(vector3df(vpos.x*mult,(vpos.y*mult)+0.3f,vpos.z*mult));
			pEndLine->setRotation(vector3df(-roadgrade*60,radToDeg(yaw),0));
			pEndLine->updateAbsolutePosition();

			pEndNode->setMaterialTexture(0, D3DBase::GetDriver()->getTexture(gpCourse->getTex(CCourse::TEX_FINISH)));
			/*
			pEndNode->setMaterialFlag(video::EMF_FOG_ENABLE, fogOn);
			pEndNode->setMaterialFlag(video::EMF_BACK_FACE_CULLING, true);
			pEndNode->setMaterialFlag(video::EMF_NORMALIZE_NORMALS,uselight);
			pEndNode->setMaterialFlag(video::EMF_LIGHTING,uselight);
			pEndNode->setMaterialType(video::EMT_SOLID);
			*/
		}
	}

	m_fProgress = 0.5f;

	if(bDemoRider)
		ShowDemoRider(bDemoRider);
	else
	{
		switch(gamemode)
		{
		default:
		case GAME_LEAD:
		case GAME_NORMAL:
			ResetRace(0);
			break;
		}
	}

	if(gpCourse != NULL)
		gpCourse->HideAll();

	if(!gpCourse)
		hr = -1L;

	Leave();
	return hr;
}

HRESULT D3DBase::SetGameMode(int imode)
{
	HRESULT hr = S_OK;
	gamemode = max(min(GAME_MAX-1,imode),GAME_NORMAL);

	//ResetRace(0);
	return hr;
}

HRESULT D3DBase::ShowDemoRider(bool bShow)
{
	HRESULT hr = S_OK;

	bDemoRider = bShow;

	// background color
	
	if(bDemoRider)
		//#1E3966
		BGColor = SColor(0,0x1e, 0x39, 0x66);
		//BGColor = SColor(255,139, 149, 176);
	else
	{
		switch(gamemode)
		{
		default:
		case GAME_LEAD:
		case GAME_NORMAL:
			BGColor = SColor(255,0,0,0);
			//BGColor = SColor(255,110,155,30);
			break;
		}
	}
	

	// hide/show dome
	bShowDome = !bDemoRider;
	skydome->setVisible(bShowDome);

	// hide/show banners
	if(pBannerEnd)
		pBannerEnd->setVisible(bShowDome);
	if(pBannerStart)
		pBannerStart->setVisible(bShowDome);
	if(pStartLine)
		pStartLine->setVisible(bShowDome);
	if(pEndLine)
		pEndLine->setVisible(bShowDome);

	// hide/show course
	bShowCourse = !bDemoRider;
	if(gpCourse)
	{
		courseNode = gpCourse->GetCourseNode();
		if(courseNode)
			courseNode->setVisible(bShowCourse);
	}

	ResetRace(0);
	return hr;
}

HRESULT D3DBase::ParseGetSysCmds(const char *cmdstring, void **ptrresult)
{
	HRESULT hr = S_OK;
	Enter();

	if(0 == _strcmpi(cmdstring, "=riders"))
	{
		(*ptrresult) = (void *)numriders;
	}
	else if(0 == _strcmpi(cmdstring, "=views"))
	{
		(*ptrresult) = (void *)numViews;
	}
	else if(0 == _strcmpi(cmdstring, "=state"))
	{
		(*ptrresult) = (void *)(int)state;
	}
	else if(0 == _strcmpi(cmdstring, "=fps"))
	{
		(*ptrresult) = (void *)(int)gFTime.renderFPS;
	}
	else if(0 == _strnicmp(cmdstring, "_CreateView", 11))
	{
		int i;
		int x=0,y=0,w=1,h=1,show=0,irider=-1,icamera=-1;
		sscanf(&cmdstring[12],"%d,%d,%d,%d,%d,%d,%d",&x,&y,&w,&h,&show,&irider,&icamera);
		for(i=0; i<MAX_RIDERS; i++)
		{
			if((!view[i]->GetActive()) || (bDemoRider ? (view[i]->GetRiderIdx() < 0) : false) )
				break;
		}
		if(i<MAX_RIDERS)
		{
			numViews++;
			view[i]->MoveView(x,y,w,h);
			view[i]->SetVisible(show!=0);
			view[i]->SetCameraIdx(icamera);
			view[i]->SetRiderIdx(irider);
			view[i]->SetActive(true);
			(*ptrresult) = (void *)i;
		}
		else
			hr = -1L;
	}
	else if(0 == _strnicmp(cmdstring, "_MoveView", 9))
	{
		int i;
		int x=0,y=0,w=1,h=1;
		sscanf(&cmdstring[10],"%d,%d,%d,%d,%d",&i,&x,&y,&w,&h);
		if(i>=0 && i<MAX_RIDERS)
			view[i]->MoveView(x,y,w,h);
		else
			hr = -1L;
	}
	else if(0 == _strnicmp(cmdstring, "_DeleteView", 11))
	{
		int i;
		sscanf(&cmdstring[12],"%d",&i);
		if(i>=0 && i<MAX_RIDERS)
		{
			view[i]->SetActive(false);
			view[i]->SetVisible(false);
			view[i]->SetCameraIdx(-1);
			view[i]->SetRiderIdx(-1);
			numViews--;
		}
		else
			hr = -1L;
	}
	else if(0 == _strnicmp(cmdstring, "_ShowView", 9))
	{
		int i;
		int show=0;
		sscanf(&cmdstring[10],"%d,%d",&i,&show);
		if(i>=0 && i<MAX_RIDERS)
			view[i]->SetVisible(show!=0);
		else
			hr = -1L;
	}
	else if(0 == _strnicmp(cmdstring, "_GetCameraView", 14))
	{
		int i;
		int show=0;
		sscanf(&cmdstring[15],"%d",&i);
		if(i>=0 && i<MAX_RIDERS)
		{
			(*ptrresult) = (void *)(view[i]->GetCameraIdx());
		}
		else
			hr = -1L;
	}
	else if(0 == _strnicmp(cmdstring, "_SetCameraView", 14))
	{
		int i;
		int icamera=-1,irider=-1;
		sscanf(&cmdstring[15],"%d,%d",&i,&icamera);
		if(i>=0 && i<MAX_RIDERS)
		{
			view[i]->SetCameraIdx(icamera);
			irider = view[i]->GetRiderIdx();
			if(irider>=0 && irider<MAX_RIDERS)
			{
				D3DBase::rider[irider]->SetCameraMode(icamera);
			}
		}
		else
			hr = -1L;
	}
	else if(0 == _strnicmp(cmdstring, "_GetRiderView", 13))
	{
		int i;
		int show=0;
		sscanf(&cmdstring[14],"%d",&i);
		if(i>=0 && i<MAX_RIDERS)
		{
			(*ptrresult) = (void *)(view[i]->GetRiderIdx());
		}
		else
			hr = -1L;
	}
	else if(0 == _strnicmp(cmdstring, "_SetRiderView", 13))
	{
		int i;
		int icamera=-1,irider=-1;
		sscanf(&cmdstring[14],"%d,%d",&i,&irider);
		if(i>=0 && i<MAX_RIDERS)
		{
			view[i]->SetRiderIdx(irider);
			//if(irider>=0 && irider<MAX_RIDERS)
			//  D3DBase::rider[irider]->SetCameraMode(icamera);
		}
		else
			hr = -1L;
	}
	else if(0 == _strnicmp(cmdstring, "_NextCameraView", 15))
	{
		int i;
		int icamera=-1,irider=-1;
		sscanf(&cmdstring[16],"%d",&i);
		if(i>=0 && i<MAX_RIDERS)
		{
			irider = view[i]->GetRiderIdx();
			if(irider>=0 && irider<MAX_RIDERS)
			{
				icamera = D3DBase::rider[irider]->NextCameraMode();
				(*ptrresult) = (void *)(icamera);
			}
		}
		else
			hr = -1L;
	}
	else if(0 == _strnicmp(cmdstring, "_NextCameraRider", 16))
	{
		//int i;
		int icamera=-1,irider=-1;
		sscanf(&cmdstring[17],"%d",&irider);
		if(irider>=0 && irider<MAX_RIDERS)
		{
			D3DBase::rider[irider]->NextCameraMode();
		}
		else
			hr = -1L;
	}
	else if(0 == _strnicmp(cmdstring, "_GetCameraRider", 15))
	{
		//int i;
		int icamera=-1,irider=-1;
		sscanf(&cmdstring[16],"%d",&irider);
		if(irider>=0 && irider<MAX_RIDERS)
		{
			icamera = D3DBase::rider[irider]->getCameraMode();
			(*ptrresult) = (void *)icamera;
		}
		else
			hr = -1L;
	}
	else if(0 == _strnicmp(cmdstring, "_SetCameraRider", 15))
	{
		//int i;
		int irider=-1,icamera =-1;
		sscanf(&cmdstring[16],"%d %d",&irider,&icamera);
		if(irider>=0 && irider<MAX_RIDERS && icamera >= 0)
		{
			D3DBase::rider[irider]->SetCameraMode(icamera);
		}
		else
			hr = -1L;
	}
	else if(0 == _strnicmp(cmdstring, "_CreateRider", 12))
	{
		int i;
		int ridermodel, bikemodel, tiremodel, rideMode; 
		float bike_frame_size, tire_size, rider_height, rider_weight;
		unsigned int ucolor;
		//SColor color;

		sscanf(&cmdstring[13],"%d,%d,%d,%d,%f,%f,%f,%f,%ul",&rideMode, &ridermodel, &bikemodel, &tiremodel, &bike_frame_size, &tire_size, &rider_height, &rider_weight, &ucolor);
		for(i=0; i<MAX_RIDERS; i++)
		{
			if(!rider[i]->isVisible())
				break;
		}
		if(i<MAX_RIDERS)
		{
			numriders++;
			rider[i]->useRider(ridermodel);
			rider[i]->setVisible(true);
			rider[i]->SetRideMode(rideMode);
			rider[i]->SimpleReset();
			(*ptrresult) = (void *)i;
		}
		else
			hr = -1L;
	}
	else if(0 == _strnicmp(cmdstring, "_DeleteRider", 12))
	{
		int i;
		sscanf(&cmdstring[13],"%d",&i);
		if(i>=0 && i<MAX_RIDERS)
		{
			if(rider[i]->isVisible())
			{
				rider[i]->setVisible(false);
				numriders--;
			}
		}
		else
			hr = -1L;
	}
	else if(0 == _strnicmp(cmdstring, "_SetRiderSpeed", 14))
	{
		int i;
		float speed;
		sscanf(&cmdstring[15],"%d,%f",&i,&speed);
		if(i>=0 && i<MAX_RIDERS)
			rider[i]->SetSpeed(speed);
		else
			hr = -1L;
	}
	else if(0 == _strnicmp(cmdstring, "_SetRiderRpm", 12))
	{
		int i;
		float rpm;
		sscanf(&cmdstring[13],"%d,%f",&i,&rpm);
		if(i>=0 && i<MAX_RIDERS)
			rider[i]->setAnimationSpeed(rpm);
		else
			hr = -1L;
	}
	else if(0 == _strnicmp(cmdstring, "_SetRiderDistance", 17))
	{
		int i;
		float dist;
		sscanf(&cmdstring[18],"%d,%f",&i,&dist);
		if(i>=0 && i<MAX_RIDERS)
			rider[i]->SetRDist(dist);
		else
			hr = -1L;
	}
	else if(0 == _strnicmp(cmdstring, "_SetRiderModel", 14))
	{
		int i, iModelIdx;
		sscanf(&cmdstring[14],"%d,%d",&i,&iModelIdx);
		if(i>=0 && i<MAX_RIDERS && iModelIdx < MODEL_MAX)
			rider[i]->useRider(iModelIdx);
		else
			hr = -1L;
	}
	else if(0 == _strnicmp(cmdstring, "_SetRiderColor", 14))
	{
		int i, iColorIdx, iRed, iGreen, iBlue, iAlpha = 255;
		sscanf(&cmdstring[14],"%d,%d,%d,%d,%d,%d",&i,&iColorIdx,&iRed,&iGreen,&iBlue);
		if(i>=0 && i<MAX_RIDERS && iColorIdx < RIDER_COLORS_MAX && iRed < 256 && iGreen < 256 && iBlue < 256)
			rider[i]->SetColor((RIDER_COLORS)iColorIdx, SColor(iAlpha,iRed,iGreen,iBlue));
		else
			hr = -1L;
	}
	else if(0 == _strnicmp(cmdstring, "_SetRiderNumber", 15))
	{
		int i, inum;
		sscanf(&cmdstring[16],"%d,%d",&i,&inum);
		if(i>=0 && i<MAX_RIDERS)
			rider[i]->SetRiderNumber(inum);
		else
			hr = -1L;
	}
	else if(0 == _strnicmp(cmdstring, "_SetRiderReal", 13))
	{
		int i;
		int iReal;
		sscanf(&cmdstring[14],"%d,%d",&i,&iReal);
		if(i>=0 && i<MAX_RIDERS)
			rider[i]->SetReal(iReal > 0);
		else
			hr = -1L;
	}
	else if(0 == _strnicmp(cmdstring, "_SetRideMode", 12))
	{
		int i;
		int iRideMode;
		sscanf(&cmdstring[14],"%d,%d",&i,&iRideMode);
		if(i>=0 && i<MAX_RIDERS)
			rider[i]->SetRideMode(iRideMode);
		else
			hr = -1L;
	}
	else if(0 == _strnicmp(cmdstring, "_IsRiderDrafting", 16))
	{
		int i;
		sscanf(&cmdstring[17],"%d",&i);
		if(i>=0 && i<MAX_RIDERS)
			(*ptrresult) = (void *)rider[i]->IsRiderDrafting();
		else
			hr = -1L;
	}
	else if(0 == _strnicmp(cmdstring, "_GetSceneryCount", 16))
	{
		if(sceneryList)
			(*ptrresult) = (void *)scenecnt;
	}
	else if(0 == _strnicmp(cmdstring, "_GetSceneryName", 14))
	{
		IFileSystem *fs = device->getFileSystem();
		int iscenery;
		sscanf(&cmdstring[15],"%d",&iscenery);

		hr = -1L;
		core::stringc scenepath, basepath, fname, fext;
		if(sceneryList && iscenery >= 0 && iscenery < scenecnt)
		{
			scenepath = sceneryList->getFullFileName(iscenery);
			splitFilename(scenepath, &basepath, &fname, &fext);

			//sprintf(gstring,"./Media/%s.txt",fname.c_str());

			int i;
			char *p;
			s32 len; 
			IReadFile *file;
			sprintf(gstring, "%s/scene.txt", fname.c_str());
			file = D3DBase::GetDevice()->getFileSystem()->createAndOpenFile(gstring);

			len = file->getSize();
			p = new char[len+1];
			file->read(p, len);
			file->drop();

			for(i = 0; i<len; i++)
			{   
				if(p[i] == 0x0a)
					break;
			}
			if(i<len && i > 1)
			{
				strncpy(gstring,&p[1],i-1);
				gstring[i-1]='\0';
				stringc fname = gstring;
				fname.trim();
				sprintf(gstring,"%s",fname.c_str());
				(*ptrresult) = (void *)gstring;

				hr = S_OK;
			}
		}
	} 
	else if(0 == _strnicmp(cmdstring, "_GetSceneryThumbnailFilename", 28))
	{
		IFileSystem *fs = device->getFileSystem();
		int iscenery;
		sscanf(&cmdstring[29],"%d",&iscenery);

		core::stringc scenepath, basepath, fname, fext;
		if(sceneryList && iscenery >= 0 && iscenery < scenecnt)
		{
			scenepath = sceneryList->getFullFileName(iscenery);
			splitFilename(scenepath, &basepath, &fname, &fext);
			sprintf(gScratchBuf,"%s/Media/%s.bmp",gBasePath,fname.c_str());
			sprintf(gstring,"%s",fs->getAbsolutePath(gScratchBuf).c_str());
			(*ptrresult) = (void *)gstring;
		}
		else
			hr = -1L;
	}
	else if(0 == _strnicmp(cmdstring, "_GetSceneryID", 13))
	{
		IFileSystem *fs = device->getFileSystem();
		int iscenery;
		sscanf(&cmdstring[13],"%d",&iscenery);

		core::stringc scenepath, basepath, fname, fext;
		if(sceneryList && iscenery >= 0 && iscenery < scenecnt)
		{
			sprintf(gstring,"%s",sceneryList->getFileName(iscenery));
			(*ptrresult) = (void *)gstring;
		}
		else
			hr = -1L;
	}
	else if(0 == _strnicmp(cmdstring, "_ShowDemoRider", 14))
	{
		int i;
		sscanf(&cmdstring[15],"%d",&i);
		ShowDemoRider(i != 0);
	}
	else if(0 == _strnicmp(cmdstring, "_SetGameMode", 12))
	{
		int i;
		sscanf(&cmdstring[13],"%d",&i);
		SetGameMode(i);
	}
	/*
	else if(0 == _strnicmp(cmdstring, "_Convert", 14)) //Loads an old format .3dc into the engine.  
	{
		string<fschar_t> s = &cmdstring[15];
		core::array<string<fschar_t>> parm;
		s.split(parm,",");
		int i = atoi(parm[1].c_str());
		hr = LoadCourse(parm[0].c_str(),i);
	}
	else // Returns how much work is left to do from 1 to 0.  Zero meaning nothing left to load and things are running normally.  
	if(0 == _strnicmp(cmdstring, "_LoadCourseProgress", 19)) 
	{
		(*ptrresult) = (void *)(int)(100 * LoadCourseProgress());
	}
	else if(0 == _strnicmp(cmdstring, "_GetSceneryThumbnailFilename", 28))
	{
		int i;
		sscanf(&cmdstring[29],"%d",&i);
		if(sceneryList)
		{
			const io:path pstr = sceneryList->getFileName(i);
			pstr += "race.bmp";
			(*ptrresult) = (void *)pstr;
		}
	}
	*/

	Leave();
	return hr;
}

HRESULT D3DBase::ParseGetRiderCmds(int irider, const char *cmdstring, void **ptrresult)
{
	HRESULT hr = S_OK;
	Enter();

	if(0 == _strcmpi(cmdstring, "=model"))
	{
		(*ptrresult) = (void *)rider[irider]->GetModelType();
	}
	else if(0 == _strcmpi(cmdstring, "=dist"))
	{
		(*ptrresult) = (void *)(int)rider[irider]->m_dist; // float
	}
	else if(0 == _strcmpi(cmdstring, "=state"))
	{
		(*ptrresult) = (void *)(int)rider[irider]->m_state;
	}
	else if(0 == _strcmpi(cmdstring, "=show"))
	{
		(*ptrresult) = (void *)(rider[irider]->m_bShow ? 1 : 0);
	}
	Leave();
	return hr;
}

HRESULT D3DBase::ParseSysCmds(const char *cmdstring)
{
	HRESULT hr = S_OK;
	Enter();

	if(0 == _strcmpi(cmdstring, "start"))
		StartRace();
	else if(0 == _strcmpi(cmdstring, "prestart"))
		PreStartRace();
	else if(0 == _strcmpi(cmdstring, "pause"))
		PauseRace();
	else if(0 == _strcmpi(cmdstring, "run"))
		RunRace();
	else if(0 == _strcmpi(cmdstring, "reset"))
		ResetRace(300);
	else if(0 == _strnicmp(cmdstring, "demo", 4))
	{
		int iDemo = atoi(&cmdstring[5]);
		demomode = (iDemo > 0 ? true : false);
		ResetRace(300);
	}
	else if(0 == _strnicmp(cmdstring, "view", 4))
	{
		int inum = atoi(&cmdstring[5]);
		SetNumViews(inum);
	}
	else if(0 == _strnicmp(cmdstring, "toggleFOV", 9))
	{
		float tAngleFOV,tN,tfardistCam,tcamh;
		if(bToggleFOV)
		{
			bToggleFOV = false;
			tAngleFOV = 15.0f;
			tN = 22.0f;
			tfardistCam = 300;    
			tcamh = 0.6f;
			eyeAdjust = 12.0f;
			//tAngleFOV = 45.0f;
			//tN = 10.0f;
			//tfardistCam = 200;               
		}
		else
		{
			bToggleFOV = true;
			eyeAdjust = 5.19f;
			tAngleFOV = 45.0f;
			tN = 10.0f;
			tfardistCam = 300;               
			tcamh = 1.0f;
		}
		camh = tcamh * mult;
		distCam = lastn = tN;
		distCamM = distCam * mult;              
		fardistCam = lastfardistCam = tfardistCam;   
		fardist = fardistCam * mult;
		angleFOV = lastAngleFOV = tAngleFOV;               

		ctrlcamera[MAYA_CAM]->setFarValue(fardistCam * mult);
		ctrlcamera[MAYA_CAM]->setFOV(degToRad(angleFOV));
		ctrlcamera[FPS_CAM]->setFarValue(fardistCam * mult);
		ctrlcamera[FPS_CAM]->setFOV(degToRad(angleFOV));
		for(int i=0; i<MAX_RIDERS; i++)
		{
			rider[i]->UpdateCameraMode();
		}
		bChanged = true;
	}
	else if(0 == _strnicmp(cmdstring, "+riderviews", 11))
	{
		AddRiderViews(atoi(&cmdstring[12]));
	}
	else if(0 == _strnicmp(cmdstring, "+rider",6))
		AddRider(atoi(&cmdstring[7]));
	else if(0 == _strcmpi(cmdstring, "+model"))
		ChangeRiderModel();
	else if(0 == _strcmpi(cmdstring, "+view"))
		AddView();
	else if(0 == _strcmpi(cmdstring, "info"))
	{
		showinfo = !showinfo;
		text->setVisible(showinfo);
	}
	else if(0 == _strcmpi(cmdstring, "showtrees"))
	{
		bShowTrees = !bShowTrees;
		//treesNode->setVisible(bShowTrees);
	}
	else if(0 == _strcmpi(cmdstring, "showdome"))
	{
		bShowDome = !bShowDome;
		skydome->setVisible(bShowDome);
	}
	else if(0 == _strcmpi(cmdstring, "showcourse"))
	{
		if(gpCourse)
		{
			courseNode = gpCourse->GetCourseNode();
			if(courseNode)
				courseNode->setVisible(!courseNode->isVisible());
		}
	}
	else if(0 == _strcmpi(cmdstring, "showriders"))
	{
		bShowRiders = !bShowRiders;
		D3DBase::SetActive(-1,bShowRiders);
	}
	else if(0 == _strcmpi(cmdstring, "showscene"))
	{
		bShowScene = !bShowScene;
	}
	Leave();
	return hr;
}


HRESULT D3DBase::ParseRiderCmds(int irider, const char *cmdstring)
{
	HRESULT hr = S_OK;
	Enter();

	if(!demomode && (0 == _strnicmp(cmdstring, "speed", 5)))
	{
		float speed = (float)atof(&cmdstring[6]);
		rider[irider]->SetSpeed(speed);
	}
	else if(!demomode && (0 == _strnicmp(cmdstring, "+speed", 6)))
	{
		float speed = (float)atof(&cmdstring[7]);
		rider[irider]->AddSpeed(speed);
	}
	else if(!demomode && (0 == _strnicmp(cmdstring, "dist", 4)))
	{
		float dist = (float)atof(&cmdstring[5]);
		rider[irider]->SetRDist(dist);
	}
	else if(!demomode && (0 == _strnicmp(cmdstring, "+dist", 5)))
	{
		float dist = (float)atof(&cmdstring[6]);
		rider[irider]->AddDist(dist);
	}
	else if(!demomode && (0 == _strnicmp(cmdstring, "breal", 5)))
	{
		int iReal = atoi(&cmdstring[6]);
		rider[irider]->SetReal((iReal > 0 ? true : false));
	}
	else if(0 == _strnicmp(cmdstring, "model", 5))
	{
		int i = atoi(&cmdstring[6]);
		rider[irider]->useRider(i);
	}
	else if(0 == _strnicmp(cmdstring, "show", 4))
	{
		int i = atoi(&cmdstring[5]);
		rider[irider]->m_bShow = (i > 0 ? true : false);
	}
	else if(0 == _strcmpi(cmdstring, "camf"))
	{
		D3DBase::rider[irider]->NextCameraMode();
	}
	Leave();
	return hr;
}

void D3DBase::SetWindowSize(UINT &w, UINT &h)
{
	// Make sure no zero width or height;
	if(w < 1) w = 1;
	if(h < 1) h = 1;
	m_uWidth = w < m_uFullWidth ? w : m_uFullWidth; 
	w = m_uWidth;
	m_uHeight = h < m_uFullHeight ? h : m_uFullHeight;
	h = m_uHeight;
}
//! Adds an empty scene node.
CCustomSceneNode* D3DBase::addCustomSceneNode(ISceneNode* parent, s32 id)
{
	if (!parent)
		parent = smgr->getRootSceneNode();

	CCustomSceneNode* node = new CCustomSceneNode(parent, smgr, id);
	node->drop();

	return node;
}

//! Creates a simple ITriangleSelector, based on a mesh.
ITriangleSelector* D3DBase::createTriangleSelector(const IMesh* mesh, ISceneNode* node)
{
	if (!mesh)
		return 0;

	return new CCustomTriangleSelector(mesh, node);
}



//! Adds an empty scene node.
ISceneNode* D3DBase::addSkyDomeSceneNode(ISceneNode* parent, s32 id)
{
	if (!parent)
		parent = smgr->getRootSceneNode();

	CSkywallTiledSceneNode* node = 0;
	if(skyImg != 0)
	{
		skyImg->drop();
		skyImg = 0;
	}
	if(skyMapImg != 0)
	{
		skyMapImg->drop();
		skyMapImg = 0;
	}

	float tiled = gpCourse->getSkyDomeTiled();
	char *ptex = gpCourse->getTex(CCourse::TEX_DOME);
	if(ptex != NULL)
	{
		skyImg = driver->createImageFromFile(ptex);
		if(skyImg)
		{
			skyMapImg = driver->createImage(ECF_R8G8B8,core::dimension2d<u32>(512,256));
			skyImg->copyToScalingBoxFilter(skyMapImg);
		}
		driver->removeTexture(driver->getTexture("skydome"));
		driver->removeTexture(driver->getTexture("skydomemap"));
		driver->addTexture("skydomemap",skyMapImg);
		driver->addTexture("skydome",skyImg);
	}
	else
	{
		u32 texh = 256, texw = 256;
		//u32 texh = 512, texw = 256;
		s32 offseth = (512 - texh) + 4 ;


		skyMapImg = driver->createImage(ECF_R8G8B8,core::dimension2d<u32>(512,256));
		skyImg = driver->createImage(ECF_R8G8B8,core::dimension2d<u32>(1024,512));

		IImage *skyImg3 = driver->createImage(ECF_R8G8B8,core::dimension2d<u32>(128,256));
		IImage *skyImg2 = driver->createImage(ECF_R8G8B8,core::dimension2d<u32>(texw,texh));
		IImage *skyImg1 = 0;

		skyImg->fill(gpCourse->getSkyColor());
		skyImg3->fill(gpCourse->getSkyColor());
		skyImg1 = driver->createImageFromFile(gpCourse->getTex(CCourse::TEX_BACK_1));
		skyImg1->copyToScalingBoxFilter(skyImg2);
		skyImg2->copyTo(skyImg,core::position2d<s32>(0*texw,offseth));
		skyImg1->copyToScaling(skyImg3);
		skyImg3->copyTo(skyMapImg,core::position2d<s32>(0,0));
		skyImg1->drop();
		skyImg1 = driver->createImageFromFile(gpCourse->getTex(CCourse::TEX_BACK_2));
		skyImg1->copyToScalingBoxFilter(skyImg2);
		skyImg2->copyTo(skyImg,core::position2d<s32>(1*texw,offseth));
		skyImg1->copyToScaling(skyImg3);
		skyImg3->copyTo(skyMapImg,core::position2d<s32>(128,0));
		skyImg1->drop();
		skyImg1 = driver->createImageFromFile(gpCourse->getTex(CCourse::TEX_BACK_3));
		skyImg1->copyToScalingBoxFilter(skyImg2);
		skyImg2->copyTo(skyImg,core::position2d<s32>(2*texw,offseth));
		skyImg1->copyToScaling(skyImg3);
		skyImg3->copyTo(skyMapImg,core::position2d<s32>(256,0));
		skyImg1->drop();
		skyImg1 = driver->createImageFromFile(gpCourse->getTex(CCourse::TEX_BACK_4));
		skyImg1->copyToScalingBoxFilter(skyImg2);
		skyImg2->copyTo(skyImg,core::position2d<s32>(3*texw,offseth));
		skyImg1->copyToScaling(skyImg3);
		skyImg3->copyTo(skyMapImg,core::position2d<s32>(384,0));
		skyImg1->drop();
		skyImg2->drop();
		skyImg3->drop();

		//IImage *skyImg = driver->createImageFromFile("skydome5.jpg");
		driver->removeTexture(driver->getTexture("skydome"));
		driver->removeTexture(driver->getTexture("skydomemap"));

		driver->addTexture("skydomemap",skyMapImg);
		driver->addTexture("skydome",skyImg);

		skyImg->drop();
		skyImg = 0;
		skyMapImg->drop();
		skyMapImg = 0;
	}

	f32 ypos = 0;
	if(gpCourse)
		ypos = gpCourse->GetMinElev() * mult;

	node = new CSkywallTiledSceneNode(driver->getTexture("skydome"),driver->getTexture(gpCourse->getTex(CCourse::TEX_FAR_GROUND)),32,3,1.0f,1.05f,(10*200*mult)-mult,tiled*2,ypos,parent,smgr,id);
	node->drop();
	node->setScale(vector3df(1.0f,1.0f/fabs(tiled),1.0f));
	//node->setDebugDataVisible(scene::EDS_MESH_WIRE_OVERLAY);

	return node;
}
HRESULT D3DBase::CreateShadow()
{
//  int tw = 1024, th = 1024;
	HRESULT hr = S_OK;
	f32 shadowAngleFOV = 45.0f;


	scene::ISceneNode* shadowNode = 0;
	shadowNode = addCustomSceneNode(customNode);
	shadowNode->updateAbsolutePosition();

	scene::IAnimatedMeshSceneNode* riderModel = smgr->addAnimatedMeshSceneNode(smgr->getMesh(modelNames[1]),shadowNode);
	riderModel->setScale(core::vector3df(1.2f*mult,1.2f*mult,1.2f*mult));
	riderModel->setPosition(vector3df(0,0.4f,0.25f*mult));
	riderModel->setMaterialFlag(video::EMF_NORMALIZE_NORMALS, true);
	riderModel->setMaterialFlag(video::EMF_LIGHTING, true);
	riderModel->setMaterialType(video::EMT_TRANSPARENT_ALPHA_CHANNEL_REF);
	riderModel->updateAbsolutePosition();

//  shadowCam = smgr->addCameraSceneNode(customNode, core::vector3df(camh*1.5f,camh/1.8f,0),
//      core::vector3df(0,camh/1.8f,0));
	shadowCam = smgr->addCameraSceneNode(customNode, core::vector3df(camh*1.5f,camh/1.8f,0),
		core::vector3df(0,0,0));
	shadowCam->updateAbsolutePosition();
	shadowCam->setFarValue(200.0f * mult);
	shadowCam->setFOV(degToRad(shadowAngleFOV));
	shadowCam->setAspectRatio(1);

	if (!initShadow && shadowCam)
	{
		D3DBase::GetDevice()->run();
		driver->beginScene(true,true,0);

		// draw scene into render target
		shadowCam->setPosition(core::vector3df(12,40,-12));
		shadowCam->updateAbsolutePosition();
		shadowCam->setFOV(degToRad(shadowAngleFOV));

		smgr->setActiveCamera(shadowCam);

		//int inc_w = 128, inc_h = 128; 
		//float inc_rot = 5.625f;
		float rot = 0;

		for(int i = 0; i < sframes; i++)
		{
			riderShadows[i] = driver->addRenderTargetTexture(core::dimension2d<u32>(tw,th), "RTTShadow", ECF_A8R8G8B8);
			driver->setRenderTarget(riderShadows[i], true, true, video::SColor(0,255,255,255));
			riderModel->setCurrentFrame((f32)i * 24 / sframes);
			for(int h=0; h<th; h+=inc_h)
			{
				for(int w=0; w<tw; w+=inc_w)
				{
					shadowNode->setRotation(vector3df(0,rot,0));
					shadowNode->updateAbsolutePosition();
					driver->setViewPort(rect<s32>(w,h,w+inc_w,h+inc_h));
					// draw whole scene into render buffer
					smgr->drawAll();
					rot+=inc_rot;
				}
			}
		}

		driver->endScene();
		// set back old render target
		// The buffer might have been distorted, so clear it
		driver->setRenderTarget(0,true,true,BGColor);
		initShadow = true;
	}
	if(riderModel)
	{
		riderModel->remove();
		riderModel = 0;
	}
	if(shadowNode)
	{
		shadowNode->remove();
		shadowNode = 0;
	}
	return hr;
}

HRESULT D3DBase::CreateSpriteRider(int irider)
{
	HRESULT hr = S_OK;
	return hr;
}

char *obfuscatedPass()
{
	// ECT- obfuscating code to be added.
	return "007";
}

HRESULT D3DBase::Create(bool bWPF, HWND useHWND)
{
	HRESULT hr = S_OK;
	while(!bReady && bInit) 
		Sleep(1); // if here then this was called before so let the initialization complete before continuing

	if(!bReady)
	{
		if(bInit)
			return hr;
		bInit=true;
	}
    
	if(device )
		return hr;

	Sleep(200);

	bBusy = true;
	m_bWPF = bWPF;
/*
	if(FALSE)
	{
		LPDIRECTDRAW  pDD;
		DDSURFACEDESC tempddsd;
		GUID* pGUID=0;
		if( SUCCEEDED( DirectDrawCreate( pGUID, &pDD, NULL ) ) )
		{
			tempddsd.dwSize = sizeof(DDSURFACEDESC);
			pDD->GetDisplayMode( &tempddsd );
			m_uFullWidth = tempddsd.dwWidth;
			m_uFullHeight = tempddsd.dwHeight;
			m_uFullColorBit = tempddsd.ddpfPixelFormat.dwRGBBitCount;
			pDD->Release();
			pDD=NULL;
		}

	}
	else
*/

	{
		/*
		// create a NULL device to detect screen resolution
		IrrlichtDevice *nulldevice = createDevice(video::EDT_NULL);
		core::dimension2d<u32> deskres = nulldevice->getVideoModeList()->getDesktopResolution();
		nulldevice -> drop();

		m_uFullWidth = deskres.Width;
		m_uFullHeight = deskres.Height;
		*/
		m_uFullWidth = GetSystemMetrics(SM_CXFULLSCREEN);
		m_uFullHeight = GetSystemMetrics(SM_CYFULLSCREEN);
	}

	/*
	if(m_uFullColorBit==16)
	{
		m_ColorFormat = ECF_R5G6B5; //ECF_A1R5G5B5;
	}
	else // assume 32 bit for now
	{
		m_ColorFormat = ECF_R8G8B8; //ECF_A8R8G8B8;
	}
	*/

	m_uWidth = m_uFullWidth;
	m_uHeight = m_uFullHeight;

	//m_uFullWidth = 800; //m_uWidth;
	//m_uFullHeight = 600; //m_uHeight;

/*
	if(m_bWPF)
	{
	}
	else
	{
		m_uWidth = m_uFullWidth;
		m_uHeight = m_uFullHeight;
	}
	*/
	if(large)
		mult = 20;
	else
		mult = 1; 

	if(m_bWPF)
	{
		EnsureHWND();

		//video::SExposedVideoData videodata(0, 1);
		/*
		So now that we have some window, we can create an Irrlicht device
		inside of it. We use Irrlicht createEx() function for this. We only
		need the handle (HWND) to that window, set it as windowsID parameter
		and start up the engine as usual. That's it.
		*/
		// create irrlicht device in the button window
		irr::SIrrlichtCreationParameters param;
		param.DriverType = driverType;
		param.uflag = 1;
		param.WindowSize =core::dimension2d<u32>(m_uFullWidth, m_uFullHeight);
	//  if (key=='a')
		param.WindowId = reinterpret_cast<void*>(m_hwnd);
		//param.EventReceiver = &receiver;
		param.EventReceiver = NULL;
		param.Stencilbuffer = bRealShadows;
		param.Bits = m_uFullColorBit;
		device = irr::createDeviceEx(param);
		showinfo = false;

		videodata.D3D9.HWnd = reinterpret_cast<void*>(m_hwnd);
		videodata.D3D9.uflag = 1;
	}
	else if(useHWND)
	{
		m_hwnd = useHWND;
		//video::SExposedVideoData videodata(0, 0);
		/*
		So now that we have some window, we can create an Irrlicht device
		inside of it. We use Irrlicht createEx() function for this. We only
		need the handle (HWND) to that window, set it as windowsID parameter
		and start up the engine as usual. That's it.
		*/
		// create irrlicht device in the button window
		irr::SIrrlichtCreationParameters param;
		param.DriverType = driverType;
		param.uflag = 1;
		param.WindowSize =core::dimension2d<u32>(m_uFullWidth, m_uHeight);
	//  if (key=='a')
		param.WindowId = reinterpret_cast<void*>(m_hwnd);
		param.EventReceiver = &receiver;
		param.Stencilbuffer = bRealShadows;
		param.Bits = m_uFullColorBit;
		device = irr::createDeviceEx(param);

		videodata.D3D9.HWnd = reinterpret_cast<void*>(m_hwnd);
		videodata.D3D9.uflag = 0;
	}
	else
	{
		// create device and exit if creation failed
		device = createDevice(driverType, core::dimension2d<u32>(m_uFullWidth, m_uFullHeight),
			m_uFullColorBit, false, bRealShadows, false, &receiver);
		/*
		// create device and exit if creation failed
		device = createDevice(driverType, core::dimension2d<u32>(m_uWidth, m_uHeight),
			32, false, false);
			*/
	}

	//Msg("First Time");

	if (device == 0)
		return -1L; // could not create selected driver.

	driver = device->getVideoDriver();
	smgr = device->getSceneManager();
	env = device->getGUIEnvironment();


	//smgr->setShadowColor(video::SColor(150,0,0,0));

	//! add our private media directory to the file system
	bool bmedia;
	IFileSystem *fs = device->getFileSystem();
	
	sprintf(gScratchBuf,"%s/Media/Data/",gBasePath);
	bmedia = fs->addFileArchive(gScratchBuf,true,true,io::EFAT_FOLDER);
	if(!bmedia)
		return -1L; // could not find the media

	sprintf(gScratchBuf,"%s/Media/Scenes/",gBasePath);
	bmedia = fs->addFileArchive(gScratchBuf,true,true,io::EFAT_FOLDER);
	if(!bmedia)
		return -1L; // could not find the media
	else
	{
		int i;
		core::stringc pstr; // = fs->getAbsolutePath("./");

		sprintf(gScratchBuf,"%s/Media/Scenes/",gBasePath);
		core::stringc pstr2 = fs->getAbsolutePath(gScratchBuf);
		pstr2.make_lower();
		int cnt = fs->getFileArchiveCount();
		for ( i = 0; i != cnt; ++i )
		{
			pstr = fs->getFileArchive(i)->getFileList()->getPath();
			pstr.make_lower();
			if(pstr == pstr2)
				break;
		}

		core::stringc scenepath, basepath, fname, fext;
		sceneryList = fs->getFileArchive(i)->getFileList();
		if(sceneryList)
		{
			scenecnt = sceneryList->getFileCount();
			for ( i = 0; i != scenecnt; ++i )
			{
				scenepath = sceneryList->getFullFileName(i);
				splitFilename(scenepath, &basepath, &fname, &fext);

				bmedia = fs->addFileArchive(scenepath,true,false,io::EFAT_ZIP,obfuscatedPass());
			}
		}
	}


	// Main scene node
	mainNode = addCustomSceneNode();
	// Custom Node for effects
	customNode = addCustomSceneNode();


	if(!m_bWPF)
	{
		/*
		The next step is to read the configuration file. It is stored in the xml
		format and looks a little bit like this:

		@verbatim
		<?xml version="1.0"?>
		<config>
			<startUpModel file="some filename" />
			<messageText caption="Irrlicht Engine Mesh Viewer">
				Hello!
			</messageText>
		</config>
		@endverbatim

		We need the data stored in there to be written into the global variables
		StartUpModelFile, MessageText and Caption. This is now done using the
		Irrlicht Engine integrated XML parser:
		*/

		// read configuration from xml file

		io::IXMLReader* xml = device->getFileSystem()->createXMLReader( L"D3DConfig.xml");
		while(xml && xml->read())
		{
			switch(xml->getNodeType())
			{
			case io::EXN_TEXT:
				// in this xml file, the only text which occurs is the
				// messageText
				MessageText = xml->getNodeData();
				break;
			case io::EXN_ELEMENT:
				{
					if (core::stringw("messageText") == xml->getNodeName())
						Caption = xml->getAttributeValue(L"caption");
				}
				break;
			default:
				break;
			}
		}
		if (xml)
			xml->drop(); // don't forget to delete the xml reader

		device->setWindowCaption(L"Engine - Loading...");
	}

	//if(m_uFullColorBit==16)
	//{
	//  driver->setTextureCreationFlag(video::ETCF_ALWAYS_16_BIT, true);
	//}
	//else // assume 32 bit for now
	//{
		driver->setTextureCreationFlag(video::ETCF_ALWAYS_32_BIT, true);
	//}

	//driver->setTextureCreationFlag(video::ETCF_CREATE_MIP_MAPS, true);
	driver->setTextureCreationFlag(video::ETCF_ALLOW_NON_POWER_2, true);
	

	//driver->setTextureCreationFlag(video::ETCF_OPTIMIZED_FOR_SPEED, true);
	

	/*
Now we create our 8 cameras. One is looking at the model
from the front, one from the top and one from the side. In
addition there's a FPS-camera which can be controlled by the
user.
*/
/*
	distCamM = distCam * mult;
	camh *= mult;
	fardist *= mult;
*/
	mainrt = mainrt2 = 0;
	if (driver->queryFeature(video::EVDF_RENDER_TO_TARGET))
	{
		if(!bRealShadows)
		{
			CreateShadow();
		}

		if(m_bWPF)
		{
			mainrt2 = driver->addRenderTargetTexture(core::dimension2d<u32>(m_uFullWidth,m_uFullHeight), "RTTMain2",ECF_A8R8G8B8);
			if(mainrt2)
				driver->setRenderTarget(mainrt2,true,true,BGColor);
		}
		else
		{
			mainrt = driver->addRenderTargetTexture(core::dimension2d<u32>(m_uWidth,m_uHeight), "RTTMain",m_ColorFormat);
			if(mainrt)
			{
				driver->setRenderTarget(mainrt,true,true,BGColor);
				mainImg = env->addImage(mainrt,core::position2d<s32>(0,0));
				mainImg->setUseAlphaChannel(false);
				//mainImg->setScaleImage(true);
				driver->setRenderTarget(0,true,true,BGColor);
			}
		}
	}
	else
	{
		text2 = env->addStaticText(
			L"Your hardware or this renderer is not able to use the "\
			L"render to texture feature. RTT Disabled.",
			core::rect<s32>(150,100,470,60));
		text2->setOverrideColor(video::SColor(100,255,255,255));
	}

	//treesNode = smgr->addEmptySceneNode();
	//treesNode = addCustomSceneNode(mainNode);

	if(fogOn)
		driver->setFog(video::SColor(255,180, 180, 180), video::EFT_FOG_EXP, fardist/2, fardist, .001f, false, false);

	for (int i = 0; i < MAX_RIDERS; i++) 
	{
		view[i] = new D3DView();
		view[i]->SetActive(false);
	}
	for (int i = 0; i < MAX_RIDERS; i++) 
	{
		rider[i] = new Rider(0,mainNode,smgr,i);
		rider[i]->useRider(numtest);
		rider[i]->setVisible(false);
	}

	testSprite = new CSpriteSceneNode(
		mainNode, D3DBase::GetSceneManager(), -1, 
		vector3df(0,0,10), dimension2d<f32>(0.3f,0.3f)*mult,rect<f32>(0,0,1,1),
		SColor(128,128,128,128),SColor(128,128,128,128),true);
	testSprite->drop();
	testSprite->setMaterialFlag(video::EMF_LIGHTING, false);
	testSprite->setMaterialType(video::EMT_TRANSPARENT_ALPHA_CHANNEL);
	testSprite->setMaterialTexture(0, riderShadows[0]);
	//testSprite->setVisible(true);
	testSprite->setVisible(false);

	three21 = driver->getTexture("three21.tga");

	countDownSprite = new CSpriteSceneNode(
		mainNode, D3DBase::GetSceneManager(), -1, 
		vector3df(0,0,10), dimension2d<f32>(0.06f,0.06f)*mult,rect<f32>(0,0,1,1),
		SColor(128,128,128,128),SColor(128,128,128,128),true);
	countDownSprite->drop();
	countDownSprite->setMaterialFlag(video::EMF_LIGHTING, false);
	countDownSprite->setMaterialType(video::EMT_TRANSPARENT_ALPHA_CHANNEL);
	//countDownSprite->setMaterialTexture(0, riderShadow);
	countDownSprite->setVisible(false);

	ctrlcamera[RIDER_CAM] = rider[0]->getCamera();

	ctrlcamera[MAYA_CAM] = smgr->addCameraSceneNodeMaya(mainNode);
	ctrlcamera[MAYA_CAM]->setFOV(degToRad(angleFOV));
	
	ctrlcamera[FPS_CAM] = smgr->addCameraSceneNodeFPS(mainNode,100,1);
	ctrlcamera[FPS_CAM]->setFOV(degToRad(angleFOV));
	if(!testing)
	{
		ctrlcamera[MAYA_CAM]->setRotation(core::vector3df(-30,180,0));
		ctrlcamera[MAYA_CAM]->setTarget(core::vector3df(-10*mult,mult*60,-distCamM*8));
		ctrlcamera[FPS_CAM]->setPosition(core::vector3df(-10*mult,mult*60,-distCamM*8));
		ctrlcamera[FPS_CAM]->setTarget(vector3df(60*mult,0,25*mult));
	}
	else
	{
		ctrlcamera[MAYA_CAM]->setTarget(core::vector3df(0,camh*2,distCamM));
		ctrlcamera[FPS_CAM]->setPosition(core::vector3df(0,camh*2,0));
		ctrlcamera[FPS_CAM]->setTarget(vector3df(0,camh/2,distCamM));
	}

	for(int i = 1; i < MAX_CAM; i++)
	{
		ctrlcamera[i]->setFarValue(fardistCam * mult);
	}

	smgr->setActiveCamera(GetMainCamera());

#if 0
	lightNode = addCustomSceneNode(mainNode);
	scene::ILightSceneNode * sun = smgr->addLightSceneNode(lightNode, core::vector3df(0,100.0f*fardist,-100.0f*fardist/7),
		video::SColorf(1.0f,1.0f,1.0f),300.0f*fardist);
	smgr->setAmbientLight(video::SColorf(0.4f,0.4f,0.4f));
	lightNode->setRotation(vector3df(0,0,angleLight));
#else
	sunNode = smgr->addLightSceneNode(mainNode, core::vector3df(0,0,0),video::SColorf(1.0f,1.0f,1.0f),5000000.0f);
	sunNode->setLightType(video::ELT_DIRECTIONAL); 
	//sunNode->setRotation(vector3df((360.0f * matColor.getRed())/255,(360.0f * matColor.getGreen())/255,(360.0f * matColor.getBlue())/255));
	sunNode->setRotation(vector3df(70,0,angleLight));
	smgr->setAmbientLight(video::SColorf(0.4f,0.4f,0.4f));
#endif

#ifdef DOCOLLISION
	collMan = smgr->getSceneCollisionManager();
	//meta = smgr->createMetaTriangleSelector();
#endif

	/*
	To test out the render to texture feature, we need a render target
	texture. These are not like standard textures, but need to be created
	first. To create one, we call IVideoDriver::addRenderTargetTexture()
	and specify the size of the texture. Please don't use sizes bigger than
	the frame buffer for this, because the render target shares the zbuffer
	with the frame buffer.
	Because we want to render the scene not from the user camera into the
	texture, we add another fixed camera to the scene. But before we do all
	this, we check if the current running driver is able to render to
	textures. If it is not, we simply display a warning text.
	*/

	// create render target
	rt = 0;
	fixedCam = 0;

	// create problem text
	skin = env->getSkin();
	font = env->getFont("lucida.xml");
	if (font)
		skin->setFont(font);

	// change transparency of skin
	for (s32 i=0; i<gui::EGDC_COUNT ; ++i)
	{
		video::SColor col = env->getSkin()->getColor((gui::EGUI_DEFAULT_COLOR)i);
		col.setAlpha(222);
		env->getSkin()->setColor((gui::EGUI_DEFAULT_COLOR)i, col);
	}

	
	text = env->addStaticText(L"",core::rect<s32>(0,60,m_uWidth,100));
	text->setOverrideColor(video::SColor(222,0,0,0));


	if(!m_bWPF)
	{
		msgbox = showAboutText();
		msgbox->getCloseButton()->setVisible(false);
	}

	gIdleTime.reset(0,10);
	UpdateTime();
	demoStart = gFTime.simTime + 5000;

	bBusy = false;
	bReady = true;
	bInit = false;

	LoadCourse("");

	return hr;
}

HRESULT D3DBase::GetSurface(void **ppSurface)
{
	HRESULT hr = S_OK;
	if(mainrt2)
	{
		(*ppSurface) = mainrt2->_getRenderTargetSurface();
	}
	else if(mainrt)
	{
		(*ppSurface) = mainrt->_getRenderTargetSurface();
	}
	else
	{
		(*ppSurface)=NULL;
		//Render();
	}
	return hr;
}
	

bool D3DBase::GetHeightFromWorld(vector3df& vpos, CCustomSceneNode *pnode)
{
#ifndef DOCOLLISION
	// Disabled for now
	return vpos;
#else   // All intersections in this example are done with a ray cast out from the camera to
	// a distance of 1000.  You can easily modify this to check (e.g.) a bullet
	// trajectory or a sword's position, or create a ray from a mouse click position using
	// ISceneCollisionManager::getRayFromScreenCoordinates()
	core::line3d<f32> ray;

	//ray.start = camera->getPosition();
	//ray.end = ray.start + (camera->getTarget() - ray.start).normalize() * 1000.0f;
	ray.start = ray.end = vpos;
	ray.start.y += 1000.0f;
	ray.end.y -= 1000.0f;

	// Tracks the current intersection point with the level or a mesh
	core::vector3df intersection = vpos;
	// Used to show with triangle has been hit
	core::triangle3df hitTriangle;

	// This call is all you need to perform ray/triangle collision on every scene node
	// that has a triangle selector, including the Quake level mesh.  It finds the nearest
	// collision point/triangle, and returns the scene node containing that point.
	// Irrlicht provides other types of selection, including ray/triangle selector,
	// ray/box and ellipse/triangle selector, plus associated helpers.
	// See the methods of ISceneCollisionManager
	scene::ISceneNode * selectedSceneNode =
		collMan->getSceneNodeAndCollisionPointFromRay(
				ray,
				intersection, // This will be the position of the collision
				hitTriangle, // This will be the triangle hit in the collision
				0, // This ensures that only nodes that we have set up to be pickable are considered
				pnode); // Check the entire scene (this is actually the implicit default)
	if(selectedSceneNode)
	{
		vpos = intersection;
		return true;
	}
	else
	{
		return false;
	}
#endif
}

HRESULT D3DBase::RenderScene(void)
{
	HRESULT hr = S_OK;
	if(bShowScene)
	{
		// draw scene normally
		smgr->drawAll();
		bSceneNodeUpdated = true;
	}
	return hr;
}

HRESULT D3DBase::UpdateTime(void)
{
	HRESULT hr = S_OK;

	// Advances the 3D virtual system timer
	device->getTimer()->tick();

	gFTime.newTime = device->getTimer()->getRealTime();
	gFTime.idleTime = gFTime.newTime - gFTime.runTime;

	// if first time through or time has looped
	if((gFTime.oldTime==0) || (gFTime.oldTime > gFTime.newTime)) 
	{
		gFTime.oldTime = gFTime.newTime;
	}
	gFTime.elapsedtime = gFTime.newTime-gFTime.oldTime;
	gFTime.oldTime = gFTime.newTime;

	// Generate the gFTime
	gFTime.sysTime = gFTime.newTime;

	gFTime.advTime = (gFTime.bFastAdvance ? 1000/30 : gFTime.elapsedtime);
	gFTime.frameTime = (gFTime.advTime > gFTime.MaxAdvTime ? gFTime.MaxAdvTime : gFTime.advTime);
	gFTime.simTime += gFTime.frameTime;
	gFTime.frameSec = gFTime.frameTime * 0.001f;
	gFTime.simSec = gFTime.simTime * 0.001f;

	gFTime.elapse[gFTime.idx] = (gFTime.sysTime - gFTime.renderTime);
	gFTime.renderTime = gFTime.sysTime;
	DWORD elapseavg = 0;
	for(int i = 0; i <10; i++) {
		elapseavg += gFTime.elapse[i];
	}
	gFTime.renderFPS = 10000.0f / elapseavg; 
	gFTime.idx++; if(gFTime.idx >= 10) gFTime.idx = 0;

	gFTime.idleTime = (unsigned long)gIdleTime.calc(gFTime.idleTime,1);
	return hr;
}

HRESULT D3DBase::Update(void)
{
	HRESULT hr = S_OK;

	ctrlcamera[RIDER_CAM] = rider[0]->getCamera();

	if(demomode && device )
	{
		if(state == PRESTART && demoStart < gFTime.simTime)
		{   
			/*
			SEvent e;
			e.EventType            = EET_KEY_INPUT_EVENT;
			e.KeyInput.PressedDown = (in.Event.KeyEvent.bKeyDown == TRUE);
			e.KeyInput.Control     = (in.Event.KeyEvent.dwControlKeyState & (LEFT_CTRL_PRESSED | RIGHT_CTRL_PRESSED)) != 0;
			e.KeyInput.Shift       = (in.Event.KeyEvent.dwControlKeyState & SHIFT_PRESSED) != 0;
			e.KeyInput.Key         = EKEY_CODE(in.Event.KeyEvent.wVirtualKeyCode);
			e.KeyInput.Char        = in.Event.KeyEvent.uChar.UnicodeChar;
			*/
			SEvent e;
			e.EventType            = EET_KEY_INPUT_EVENT;
			e.KeyInput.PressedDown = true;
			e.KeyInput.Control     = false;
			e.KeyInput.Shift       = false;
			e.KeyInput.Key         = KEY_KEY_G;
			e.KeyInput.Char        = L'G';
			device->postEventFromUser(e);
			e.EventType            = EET_KEY_INPUT_EVENT;
			e.KeyInput.PressedDown = false;
			e.KeyInput.Control     = false;
			e.KeyInput.Shift       = false;
			e.KeyInput.Key         = KEY_KEY_G;
			e.KeyInput.Char        = L'G';
			device->postEventFromUser(e);
		}
		if(state == RACE)
		{
			bool bfinish = true;
			for (int i=0; i < MAX_RIDERS; i++)  
			{
				if (rider[i]->isVisible())  
				{
					if(rider[i]->m_state == RACE)
						bfinish = false;
					break;
				}
			}
			if(bfinish)
			{
				SEvent e;
				e.EventType            = EET_KEY_INPUT_EVENT;
				e.KeyInput.PressedDown = true;
				e.KeyInput.Control     = false;
				e.KeyInput.Shift       = false;
				e.KeyInput.Key         = KEY_KEY_R;
				e.KeyInput.Char        = L'R';
				device->postEventFromUser(e);
				e.EventType            = EET_KEY_INPUT_EVENT;
				e.KeyInput.PressedDown = false;
				e.KeyInput.Control     = false;
				e.KeyInput.Shift       = false;
				e.KeyInput.Key         = KEY_KEY_R;
				e.KeyInput.Char        = L'R';
				device->postEventFromUser(e);
			}
		}
	}

	switch(state)
	{
	case PRESTART:
		break;
	case START_3:
		{
			if(demoStart < gFTime.simTime+2000)
			{
				state = START_2;
				three21i = 1;
				countDownSprite->setPosition(vector3df(0,0,20.0f));
				for (int i = 0; i < MAX_RIDERS; i++) 
				{
					D3DBase::rider[i]->m_state = state;
					D3DBase::rider[i]->m_speed = 0;
				}
			}
		}
		break;
	case START_2:
		{
			if(demoStart < gFTime.simTime+1000)
			{
				state = START_1;
				three21i = 2;
				countDownSprite->setPosition(vector3df(0,0,20.0f));
				for (int i = 0; i < MAX_RIDERS; i++) 
				{
					D3DBase::rider[i]->m_state = state;
					D3DBase::rider[i]->m_speed = 0;
				}
			}
		}
		break;
	case START_1:
		{
			if(demoStart < gFTime.simTime)
			{
				state = START_0;
				three21i = 2;
				countDownSprite->setPosition(vector3df(0,0,20.0f));
				for (int i = 0; i < MAX_RIDERS; i++) 
				{
					D3DBase::rider[i]->m_state = state;
					D3DBase::rider[i]->m_speed = 0;
				}
			}
		}
		break;
	case START_0:
		{
			if(demoStart < gFTime.simTime)
			{
				demoStart = gFTime.simTime + 500;
				state = RACE;
				three21i = 3;
				countDownSprite->setPosition(vector3df(0,0,20.0f));
				//countDownSprite->setSize(dimension2d<f32>(0.075f,0.06f));
				for (int i = 0; i < MAX_RIDERS; i++) 
				{
					D3DBase::rider[i]->m_state = state;
					D3DBase::rider[i]->m_speed = 0;
				}
			}
		}
		break;
	case START_NOW:
		{
			state = RACE;
			demoStart = gFTime.simTime;
			countDownSprite->setVisible(false);
			for (int i = 0; i < MAX_RIDERS; i++) 
			{
				D3DBase::rider[i]->m_state = state;
				D3DBase::rider[i]->m_speed = 0;
			}
		}
		break;
	// Racing (Race Started)
	case RACE:
		{
			if(demoStart < gFTime.simTime)
			{
				three21i = 4;
				countDownSprite->setVisible(false);
			}
		}
		break;
	case PAUSE:
		break;
	case FINISH:
		break;
	case DEMORIDER:
		demoStart = 0;
		countDownSprite->setVisible(false);
		for (int i = 0; i < MAX_RIDERS; i++) 
		{
			D3DBase::rider[i]->m_state = state;
			//D3DBase::rider[i]->m_speed = 0;
		}
		break;
	default:
		break;
	}


	if (countDownSprite && three21i < 4 && three21)
	{
		int red = three21i < 3 ? 255 : 0;
		int green = three21i > 0 ? 255 : 0;
		vector3df vpos = countDownSprite->getPosition();
		vpos += countdownVec * gFTime.frameSec;
		countDownSprite->setPosition(vpos);
		countDownSprite->setUVRect(core::rect<f32>(three21x[2 + three21i * 2],0,three21x[3 + three21i * 2],1));
		countDownSprite->setColor(SColor(255,red,green,0));
	}

	if(angleFOV != lastAngleFOV)
	{
		ctrlcamera[MAYA_CAM]->setFOV(degToRad(angleFOV));
		ctrlcamera[FPS_CAM]->setFOV(degToRad(angleFOV));
		lastAngleFOV = angleFOV;
	}

	switch(scrollVal)
	{
	case 0:
		/*
		if(matColor != lastmatColor)
		{
			for (int i=0; i < MAX_RIDERS; i++)  
			{
				rider[i]->ChangeColor(matType,matColor);
			}
			lastmatColor = matColor;
		}
		*/
		if(matColor != lastmatColor)
		{
			sunNode->setRotation(vector3df((360.0f * matColor.getRed())/255,(360.0f * matColor.getGreen())/255,(360.0f * matColor.getBlue())/255));
			lastmatColor = matColor;
		}
		break;
	case 1:
		if(angleLight != lastAngleLight)
		{
			//lightNode->setRotation(vector3df(0,0,angleLight));
			sunNode->setRotation(vector3df(0,0,angleLight));
			lastAngleLight = angleLight;
		}
		break;
	case 2:
		if(fardistCam != lastfardistCam)
		{
			lastfardistCam = fardistCam;
			ctrlcamera[MAYA_CAM]->setFarValue(fardistCam * mult);
			ctrlcamera[FPS_CAM]->setFarValue(fardistCam * mult);
			for (int i=0; i < MAX_RIDERS; i++)  
			{
				rider[i]->getCamera()->setFarValue(fardistCam * mult);
			}
		}
		break;
	default:
		break;
	}

	if(distCam != lastn)
		lastn = distCam;

/*
	// Updates the riders
	for (int i=0; i < MAX_RIDERS; i++)  
	{
		rider[i]->Update();
	}
*/
	// Figure out the collision stuff
	if(!bDemoRider)
	{
		// get the orders of the riders
		// Check collision and drafting
		// figure out the lanes
		int lead=0;

		{
			double dist=0.0f, tdist;
			int iLast=0;

			// order riders by distance

			// assume the first rider is always active and has the lead
			orderDist[0] = 0;
			dist = D3DBase::rider[0]->GetDist();
			lead=0;

			// figure out the number of actual active riders and order them by distance
			for (int i=1; i < MAX_RIDERS; i++)  
			{
				orderDist[i-iLast] = i;

				if(D3DBase::rider[i]->isVisible())
				{
					tdist = D3DBase::rider[i]->GetDist();
					// active - insert rider to first if ahead
					if(tdist > dist)
					{
						lead = i;
						// new first distance to compare with
						dist = tdist;

						int j;
						for (j=(i-iLast); j > 0; j--)  
						{
							orderDist[j] = orderDist[j-1];
						}
						// i is first, put at the beginning of the list 
						orderDist[j] = i;
					}
					else // not first, so insert to correct position, assume the list is in order
					{
						int j;
						// compare from last to first
						for (j=(i-iLast); j > 0; j--)  
						{
							int k = orderDist[j-1];
							f64 tempDist = D3DBase::rider[k]->GetDist();
							// compare with the next rider in the list
							if(tdist >= tempDist)
							{
								// i is ahead, so move k down
								orderDist[j] = k;
								continue;
							}
							/*
							else // if same dist, use the lane to d
							if(tdist == tempDist)
							{

								if(i == k)
								{
									orderDist[j] = k;
									continue;
								}
							}
							*/
							// i found the position, insert here
							break;
						}
						// insert here
						orderDist[j] = i;
					}
				}
				else
				{
					// not active, put at the end of the list
					iLast++;
					orderDist[MAX_RIDERS-iLast] = i;
				}
			}

			int irider;
			// numRiders - actual number of active riders
			numRiders = MAX_RIDERS-iLast;
			// Order the riders by lanes
			for (int i=0; i < numRiders; i++)  
			{
				irider = orderDist[i];
				targetLanes[irider] = D3DBase::rider[irider]->m_targetLane;
			}
			int lane,tlane;

			// start with the one in front
			irider = orderDist[0];
			orderLane[0] = irider;
			lane = targetLanes[irider]; 


			for (int i=1; i < numRiders; i++)  
			{
				irider = orderDist[i];
				orderLane[i] = irider;
				tlane = targetLanes[irider]; 

				if(tlane < lane)
				{
					lane = tlane;
					int j;
					for (j=i; j > 0; j--)  
					{
						orderLane[j] = orderLane[j-1];
					}
					orderLane[j] = irider;
				}
				else
				{
					int j;
					for (j=i; j > 0; j--)  
					{
						int k = orderLane[j-1];
						int tempLane = targetLanes[k]; 
						if(tlane < tempLane)
						{
							orderLane[j] = k;
							continue;
						}
						if(tlane == tempLane)
						{
							f64 tdist = D3DBase::rider[irider]->GetDist();
							f64 tempDist = D3DBase::rider[k]->GetDist();
							if(tdist > tempDist)
							{
								orderLane[j] = k;
								continue;
							}
							else
							if(tdist == tempDist)
							{
								if(irider < k)
								{
									orderLane[j] = k;
									continue;
								}
							}
						}
						break;
					}
					orderLane[j] = irider;
				}
			}

			// TODO -  find a more realistic algorithm in chosing target lanes
			{
				// Figure out the target lanes for riders
				//
				bool bCollision = false;
				int targetLane; 
				int left = 0;
				int right = MAX_RIDERS-1;
				int irider;
				f64 riderDist; //= D3DBase::rider[k]->GetDist();
				f32 riderMovingDist;
				int riderDesiredLane = right;

				// go through only active riders, that wants to go to the right
				// start from right to left
				for (int i = numRiders-1; i >= 0; i--)  
				{
					irider = orderLane[i];
					riderDesiredLane = D3DBase::rider[irider]->m_desiredLane; // left lane or right lane
					// if desire is left, check later
					if(riderDesiredLane < D3DBase::rider[irider]->m_lane)
						continue;

					bCollision = false;
					D3DBase::rider[irider]->m_isColliding = bCollision;

					targetLane = min(D3DBase::rider[irider]->m_lane + 1, riderDesiredLane);
					riderDist = D3DBase::rider[irider]->GetDist();
					riderMovingDist = D3DBase::rider[irider]->m_velForw * gFTime.frameSec;

					// if right most lane rider and wants to go to right lane, then no obstacle
					if(targetLane >= D3DBase::rider[irider]->m_lane && i == numRiders-1)
					{
						targetLanes[irider] = targetLane;
						D3DBase::rider[irider]->m_desiredLane = targetLane;
						continue;
					}
					
					// here, riders are not in the right most lane
					// here the rider is in the left or same lane as someone
					int j;
					for (j = numRiders-1; j >= 0; j--)  
					{
						// I get what I want if I want the right and I am all the way to the right or at the right of rider I am comparing
						if(targetLane >= D3DBase::rider[irider]->m_lane && j <= i)
							break; 

						// ignore self
						if(j==i)
							continue;

						int n = orderLane[j];
						f64 aDist = D3DBase::rider[n]->GetDist();
						f32 aMovingDist = D3DBase::rider[n]->m_velForw * gFTime.frameSec; 
						f32 distColl = (bikeLength * collFactor);
						// if behind the bike is less then there is a collision
						if((aDist - distColl) > (riderDist + riderMovingDist) || (riderDist - distColl) > (aDist + aMovingDist))
							continue; // no collision

						bCollision = true;
						D3DBase::rider[irider]->m_isColliding = bCollision;
						D3DBase::rider[irider]->m_collisionHold = gFTime.simTime + 500; // Give a half second hold in looking for a normal target lane.
						D3DBase::rider[n]->m_isColliding = bCollision;
						D3DBase::rider[n]->m_collisionHold = gFTime.simTime + 500; // Give a half second hold in looking for a normal target lane.

						//int tLane = targetLanes[n];
						int tLane = min(D3DBase::rider[n]->m_desiredLane,targetLanes[n]);
						// if I am at left of rider and I want to go right or stay then I have to move left of rider
						if(targetLane >= tLane && i < j)
						{
							// Try to pass to the left
							targetLane = max(left,tLane - 1); //tLane - 1;
							/* TODO 
							// No room, try to pass to the right
							if(targetLane < left)
								targetLane = tLane + 1;
							*/
						}
					}
					targetLanes[irider] = targetLane;
					D3DBase::rider[irider]->m_desiredLane = targetLane;
					continue; // no collision
				}
			}
			{
				// Figure out the target lanes for riders
				//
				bool bCollision;    // = false;
				int targetLane; 
				int left = 0;
				int right = MAX_RIDERS-1;

				int irider;
				f64 riderDist; 
				f32 riderMovingDist; 
				int riderDesiredLane = left;

				for (int i=0; i < numRiders; i++)  
				{
					irider = orderLane[i];
					riderDesiredLane = D3DBase::rider[irider]->m_desiredLane; // left lane or right lane

					// if desire is right, already been processed
					if(riderDesiredLane > D3DBase::rider[irider]->m_lane)
						continue;

					bCollision = false;
					D3DBase::rider[irider]->m_isColliding = bCollision;

					targetLane = max(D3DBase::rider[irider]->m_lane - 1, riderDesiredLane);
					riderDist = D3DBase::rider[irider]->GetDist();
					riderMovingDist = D3DBase::rider[irider]->m_velForw * gFTime.frameSec;

					// if left most lane rider and wants to go to left lane, then no obstacle
					if(targetLane <= D3DBase::rider[irider]->m_lane && i == 0)
					{
						targetLanes[irider] = targetLane;
						D3DBase::rider[irider]->m_desiredLane = targetLane;
						continue;
					}
					
					// here, riders are not in the left most lane
					// here the rider is in the right or same lane as someone
					int j;
					for (j = 0; j < numRiders; j++)  
					{
						// I get what I want
						if(riderDesiredLane <= D3DBase::rider[irider]->m_lane && i <= j)
							break; 

						// ignore self
						if(j==i)
							continue;

						int n = orderLane[j];
						f64 aDist = D3DBase::rider[n]->GetDist();
						f32 aMovingDist = D3DBase::rider[n]->m_velForw * gFTime.frameSec; 
						f32 distColl = (bikeLength * collFactor);
						// if behind the bike is less then there is a collision
						if((aDist - distColl) > (riderDist + riderMovingDist) || (riderDist - distColl) > (aDist + aMovingDist))
							continue; // no collision

						bCollision = true;
						D3DBase::rider[irider]->m_isColliding = bCollision;
						D3DBase::rider[irider]->m_collisionHold = gFTime.simTime + 500; // Give a half second hold in looking for a normal target lane.
						D3DBase::rider[n]->m_isColliding = bCollision;
						D3DBase::rider[n]->m_collisionHold = gFTime.simTime + 500; // Give a half second hold in looking for a normal target lane.

						//int tLane = targetLanes[n];
						int tLane = max(D3DBase::rider[n]->m_desiredLane,targetLanes[n]);
						// if I am at right of rider and I want to go left or stay then I have to move right of rider
						if(targetLane <= tLane && j < i)
						{
							// Try to pass to the right
							targetLane = min(right,tLane + 1); //tLane + 1;
							/* TODO 
							// No room, try to pass to the left
							if(targetLane > right)
								targetLane = tLane - 1;
							*/
						}
					}
					targetLanes[irider] = targetLane;
					D3DBase::rider[irider]->m_desiredLane = targetLane;
					continue; // no collision
				}
			}

			// Figure out if in drafting position
			bool bDrafting = false, wasDrafting, isDrafting;
			for (int i=0; i < numRiders; i++)  
			{
				int n = orderLane[i];
				int riderLane = D3DBase::rider[n]->m_lane;
				int targetRiderLane = D3DBase::rider[n]->m_targetLane;

				// also set the desired lane here for the finish line
				wasDrafting = D3DBase::rider[n]->m_isDrafting;
				isDrafting = false;
				bDrafting = false;

				if(i < numRiders)
				{
					D3DBase::rider[n]->m_desiredLane = i;
					int j;
					for (j = 0; j < numRiders; j++)  
					{
						if(j==i)
							continue;
						int k = orderLane[j];

						int tLane = D3DBase::rider[k]->m_lane;
						int tprevLane = D3DBase::rider[k]->m_prevLane;

						f64 riderDist = D3DBase::rider[n]->GetDist();
						f64 tDist = D3DBase::rider[k]->GetDist();

						if((riderDist + bikeLength) < tDist &&  (tDist - riderDist) < (10.0f + bikeLength))
						{
							// ifin range and in same lane or was in same lane and target lane is same
							if(tLane == riderLane)
							{
								D3DBase::rider[n]->m_draftingHold = gFTime.simTime + 1000; // Give a second hold
								bDrafting = true;
								break;
							}

							if(D3DBase::rider[n]->m_draftingHold > gFTime.simTime) // Give a second hold
							{
								if(tLane == targetRiderLane)
									D3DBase::rider[n]->m_draftingHold = gFTime.simTime + 1000; // Give a second hold
								bDrafting = true;
								break;
							}
						}
					}
					if(!bDrafting)
					{
						D3DBase::rider[n]->m_draftingHold = 0;
						if(wasDrafting)
						{
							int bp = 1;
						}
					}
				}
				D3DBase::rider[n]->m_isDrafting = bDrafting;
			}
		}

		if(gamemode==GAME_LEAD)
		{
			int changeTime = 1500;
			int livelead = 0;
			for(int t = 0; t < numriders;t++)
			{
				int irider = orderDist[t];
				if(rider[irider]->GetRideMode() == 1) // live
				{
					livelead = irider;
					break;
				}
			}

			// Move view camera target to the lead rider
			if(D3DBase::leadRider != livelead && holdChange < gFTime.simTime)
			{   
				holdChange = gFTime.simTime + changeTime;
				D3DBase::prevLeadRider = D3DBase::leadRider;
				D3DBase::leadRider = livelead;
			}
			vector3df cur = leadTarget;
			vector3df newTarget;
			vector3df delta;
			if(holdChange > gFTime.simTime)
			{
				u32 dt = (holdChange - gFTime.simTime);
				f32 old = ((f32)(holdChange - gFTime.simTime)) / changeTime;
				newTarget = (D3DBase::rider[leadRider]->getPosition() * (1.0f - old)) + (D3DBase::rider[prevLeadRider]->getPosition() * old);
			}
			else
			{
				newTarget = D3DBase::rider[leadRider]->getPosition();
			}
			delta = (newTarget - cur);
			D3DBase::leadTarget += delta;
		}
	}

	// actual collision adjustments here
	for (int i=0; i < numRiders; i++)  
	{
		int iRider = orderDist[i];
		rider[iRider]->UpdatePosition();
	}

	// Updates the riders
	for (int i=0; i < MAX_RIDERS; i++)  
	{
		rider[i]->Update();
	}

	// Update riders cameras
	for (int i=0; i < MAX_RIDERS; i++)  
	{
		rider[i]->UpdateAfter();
	}

	/* For testing sprites 
	if(0)
	{
		vector3df vrot;
		//int tw = 1024, th = 1024, inc_w = 128, inc_h = 128;
		//float inc_rot = 5.625f;
		//if(m_state == RACE)
		{
			int texw = tw, texh = th;
			int w = texw/inc_w, h = texh/inc_h;
			vrot = rider[0]->getRotation();
			while(vrot.y < 0)
				vrot.y += 360.0f;
			while(vrot.y > 360.0f)
				vrot.y -= 360.0f;

			float idx = vrot.y / inc_rot;

			f32 tw = f32((int)idx % w)/w; 
			f32 th = f32((int)idx / w)/h;
			
			testSprite->setUVRect(rect<f32>(tw,th,tw+1.0f/w,th+1.0f/h));
		}
	}
	*/

	return hr;
}

HRESULT D3DBase::PreRender(void)
{
	HRESULT hr = S_OK;
	for (int i = 0; i < MAX_RIDERS; i++) 
	{
		rider[i]->preRender();
	}
	return hr;
}


bool D3DBase::Enter(bool bOut)
{
	if(bOut && bBusy)
		return false;
	while(bBusy) 
		Sleep(1);
	bBusy = true;
	return true;
}

void D3DBase::Leave()
{
	bBusy = false;
}

HRESULT D3DBase::Render2(void)
{
	HRESULT hr = S_OK;
	if(!Enter(true))
		return hr;

	UpdateTime();

	PreRender();

	bSceneNodeUpdated = false;

	Update();

	int ResX = m_uWidth,ResY = m_uHeight;
	int OffX = 0,OffY = 0;
	rect<s32> mainRect(0,0,m_uWidth,m_uHeight);
	if(m_bWPF)
		mainRect = rect<s32>(0,0,1,1); // nothing to present if in WPF mode

	//Set the viewpoint to the whole screen and begin scene
	//driver->setViewPort(mainRect);

	if(mainrt)
	{
		driver->beginScene(false,false,BGColor,videodata,&mainRect);
		driver->setRenderTarget(mainrt,true,true,BGColor);
	}
	else if(mainrt2)
	{
		driver->beginScene(false,false,BGColor,videodata,&mainRect);
		driver->setRenderTarget(mainrt2,true,true,BGColor);
	}
	else
	{
		driver->beginScene(true,true,BGColor,videodata,&mainRect);
	}

	int w,h,x,y,row,col;
	
	UINT uWidth = m_uWidth;
	UINT uHeight = m_uHeight;

	scene::ICameraSceneNode* pcam;
	if(bDemoRider)
	{
		rect<s32> r = view[0]->GetViewRect();
		pcam = rider[0]->getCamera();
		pcam->setFOV(degToRad(angleFOV/4));
		pcam->setAspectRatio((float)r.getWidth()/r.getHeight());
		smgr->setActiveCamera(pcam);
		
		if(gpCourse)
			gpCourse->ShowVisible(pcam, rider[0]->m_psec);

		//Set viewpoint to the first quarter (left top)
		driver->setViewPort(r);
		//Draw scene
		RenderScene();
	}
	else
	{
		switch(gamemode)
		{
		default:
		case GAME_LEAD:
		case GAME_NORMAL:
			switch(numViews)
			{
			case 1:
				row = 1; col = 1; 
				break;
			case 2:
				row = 1; col = 2; 
				break;
			case 3:
				row = 2; col = 2; 
				break;
			case 4:
				row = 2; col = 2; 
				break;
			case 5:
				row = 2; col = 3; 
				break;
			case 6:
				row = 2; col = 3; 
				break;
			case 7:
				row = 2; col = 4; 
				break;
			case 8:
				row = 2; col = 4; 
				break;
			}
			w = uWidth/col; h = uHeight/row;
			x = 0; y = 0; 
			for (int i = 0; i < MAX_RIDERS; i++) 
			{
				if(i < numViews && rider[i]->m_bShow)
				{
					view[i]->MoveView(x, y, w, h);
					//view[i]->SetCameraIdx(rider[i]->getCameraMode(),i);
					view[i]->SetCameraIdx(rider[i]->getCameraMode());
					view[i]->SetRiderIdx(i);
					view[i]->SetVisible(true);
					view[i]->SetActive(true);
				}
				else
				{
					view[i]->SetVisible(false);
					view[i]->SetActive(false);
				}
				x += w; 
				if((x + w) > (int)uWidth)
				{
					x = 0; y += h;
				}
			}


			for (int i = 0; i < MAX_RIDERS; i++) 
			{
				if(view[i]->GetVisible())
				{
					int iRider = view[i]->GetRiderIdx();
					if(rider[iRider]->isVisible())
					{
						rect<s32> r = view[i]->GetViewRect();
						rider[iRider]->setCameraMode(view[i]->GetCameraIdx());

						pcam = rider[iRider]->getCamera();
						if(iRider==0)
							pcam = GetMainCamera();

						pcam->setAspectRatio((float)r.getWidth()/r.getHeight());
						smgr->setActiveCamera(pcam);

						if(gpCourse)
							gpCourse->ShowVisible(pcam, rider[iRider]->m_psec);

						//Set viewpoint to the first quarter (left top)
						driver->setViewPort(r);
						//Draw scene
						RenderScene();
					}
				}
			}

			pcam = GetMainCamera();
			pcam->setAspectRatio((float)uWidth/uHeight);
			smgr->setActiveCamera(pcam);
			// The real full size window
			driver->setViewPort(rect<s32>(0,0,uWidth,uHeight));
		//  driver->setViewPort(rect<s32>(0,0,m_uFullWidth,m_uFullHeight));

			x = 0; y = 0; 
			w = m_uFullWidth/col; h = m_uFullHeight/row;
			for (int i = 1; i < row; i++) 
			{
				int sty = i * h;
				device->getVideoDriver()->draw2DLine(vector2d<s32>(0,sty),vector2d<s32>(m_uFullWidth,sty), BGColor);
			}
			for (int i = 1; i < col; i++) 
			{
				int stx = i * w;
				device->getVideoDriver()->draw2DLine(vector2d<s32>(stx,0),vector2d<s32>(stx,m_uFullHeight), BGColor);
			}
			break;
		}
	}

	if(mainrt)
	{
		if(mainrt2)
			driver->setRenderTarget(mainrt2,false,false,BGColor);
		else
			driver->setRenderTarget(0,false,false,BGColor);
	}
	// draw gui
	env->drawAll();

	driver->endScene();

	// display frames per second in window title
	//int fps = driver->getFPS();
	int fps = (int)gFTime.renderFPS;
	int idle = (int)(gFTime.idleTime);
	int busy = (int)(gFTime.elapsedtime) - idle;
	int fps2 = (int)driver->getFPS();
	if (showinfo && (lastFPS != fps || fps != fps2 || bChanged || (m_bWPF && demoStart >= gFTime.simTime)))
	{
		core::stringw str = L"Ver. ";
		str.append(core::stringw(gVersionNum));

		str += L" - FPS: ";
		//str.append(core::stringw(fps2));
		//str += L" / ";
		str.append(core::stringw(fps));
		str += L"-";
		str.append(core::stringw(idle));
		str += L"-";
		str.append(core::stringw(busy));
		//str += L" - ";
		//str.append(core::stringw((int)(gFTime.runPercent * 100) ));
		str += L" Tris: ";
		core::stringw tris = core::stringw(driver->getPrimitiveCountDrawn());
		core::stringw newtris = L"";
		addCommas(tris, newtris);
		str += newtris;
		str += L" Demo: ";
		if(demomode)
			str += L"On";
		else
			str += L"Off";
		str += L" Game: ";
		if(gamemode==GAME_LEAD)
			str += L"Lead";
		else
			str += L"Normal";
		str += L" Riders: ";
		str.append(core::stringw(numriders));
		str += L" Sun: ";
		str.append(core::stringw((int)angleLight));
		str += L" FOV: ";
		str.append(core::stringw((int)angleFOV));
		str += L" CamDist: ";
		str.append(core::stringw((int)distCam));
		str += L" Cull: ";
		str.append(core::stringw((int)fardistCam));
		str += L" Colors: ";
		sprintf(gScratchBuf,"%s.%c.%2d.%2d.%2d",matNames[matType],(matKey==0 ? 'R' : (matKey==1 ? 'G' : 'B')),matColor.getRed(),matColor.getGreen(),matColor.getBlue());
		str.append(gScratchBuf);

		if(!m_bWPF)
		{
			str += L" CamMode: ";
			if(camtype==RIDER_CAM)
				str.append(L"Fixed");
			else if(camtype==MAYA_CAM)
				str.append(L"Control from Mouse");
			else
				str.append(L"Fly");
		}

		u32 count = (demoStart - gFTime.simTime) / 1000;
		if(demomode && demoStart >= gFTime.simTime)
		{
			str += L"\n CountDown: ";
			str.append(core::stringw(count));
		}
		{
			str += L" Drafting:";
			for (int i = 0; i < MAX_RIDERS; i++) 
			{
				if(rider[i]->IsRiderDrafting())
				{
					str += L" ";
					str.append(core::stringw(i+1));
				}
			}
		}

		if(m_bWPF)
		{
			text->setText(str.c_str());
			text->setOverrideColor(video::SColor(255,255,255,255));
			text->setTextAlignment(gui::EGUIA_UPPERLEFT,gui::EGUIA_CENTER);
		}
		else
			device->setWindowCaption(str.c_str());

		lastFPS = fps;
		bChanged = false;
	}
//  busy = false;
	gFTime.runTime = device->getTimer()->getRealTime();

	if(gpCourse != NULL)
		gpCourse->ShowVisible(NULL, NULL, false);
	//if(gpCourse != NULL)
	//  gpCourse->HideAll();

	Leave();
	return hr;
}

HRESULT D3DBase::Render(void)
{
	HRESULT hr = S_OK;
	if(!Enter(true))
		return hr;

	UpdateTime();

	PreRender();

	bSceneNodeUpdated = false;

	Update();

	int ResX = m_uWidth,ResY = m_uHeight;
	int OffX = 0,OffY = 0;
	rect<s32> mainRect(0,0,m_uWidth,m_uHeight);
	if(m_bWPF)
		mainRect = rect<s32>(0,0,1,1); // nothing to present if in WPF mode

	//Set the viewpoint to the whole screen and begin scene
	//driver->setViewPort(mainRect);

	if(mainrt)
	{
		driver->beginScene(false,false,BGColor,videodata,&mainRect);
		driver->setRenderTarget(mainrt,true,true,BGColor);
	}
	else if(mainrt2)
	{
		driver->beginScene(false,false,BGColor,videodata,&mainRect);
		driver->setRenderTarget(mainrt2,true,true,BGColor);
	}
	else
	{
		driver->beginScene(true,true,BGColor,videodata,&mainRect);
	}

	
	UINT uWidth = m_uWidth;
	UINT uHeight = m_uHeight;

	scene::ICameraSceneNode* pcam;
	if(bDemoRider)
	{
		rect<s32> r = view[0]->GetViewRect();
		pcam = rider[0]->getCamera();
		pcam->setFOV(degToRad(angleFOV/4));
		pcam->setAspectRatio((float)r.getWidth()/r.getHeight());
		smgr->setActiveCamera(pcam);

		if(gpCourse)
			gpCourse->ShowVisible(pcam, rider[0]->m_psec);

		//Set viewpoint to the first quarter (left top)
		driver->setViewPort(r);
		//Draw scene
		RenderScene();
	}
	else
	{
		switch(gamemode)
		{
		default:
		case GAME_LEAD:
		case GAME_NORMAL:
			for (int i = 0; i < MAX_RIDERS; i++) 
			{
				if(view[i]->GetVisible())
				{
					int iRider = view[i]->GetRiderIdx();
					if(rider[iRider]->isVisible())
					{
						rect<s32> r = view[i]->GetViewRect();
						rider[iRider]->setCameraMode(view[i]->GetCameraIdx());

						pcam = rider[iRider]->getCamera();
						if(iRider==0)
							pcam = GetMainCamera();

						pcam->setAspectRatio((float)r.getWidth()/r.getHeight());
						smgr->setActiveCamera(pcam);

						if(gpCourse)
							gpCourse->ShowVisible(pcam, rider[iRider]->m_psec);

						//Set viewpoint to the first quarter (left top)
						driver->setViewPort(r);
						//Draw scene
						RenderScene();
					}
				}
			}
			break;
		}
	}

	pcam = GetMainCamera();
	pcam->setAspectRatio((float)uWidth/uHeight);
	smgr->setActiveCamera(pcam);
	// The real full size window
	driver->setViewPort(rect<s32>(0,0,m_uFullWidth,m_uFullHeight));


	if(mainrt)
	{
		if(mainrt2)
			driver->setRenderTarget(mainrt2,false,false,BGColor);
		else
			driver->setRenderTarget(0,false,false,BGColor);
	}
	// draw gui
	env->drawAll();

	driver->endScene();

	// display frames per second in window title
	int fps = (int)gFTime.renderFPS;
	int idle = (int)(gFTime.idleTime);
	int busy = (int)(gFTime.elapsedtime) - idle;
	int fps2 = (int)driver->getFPS();
	if (showinfo && (lastFPS != fps || fps != fps2 || bChanged || (m_bWPF && demoStart >= gFTime.simTime)))
	{
		core::stringw str = L"Ver. ";
		str.append(core::stringw(gVersionNum));

		str += L" - FPS: ";
		str.append(core::stringw(fps));
		str += L"-";
		str.append(core::stringw(idle));
		str += L"-";
		str.append(core::stringw(busy));
		str += L" Tris: ";
		core::stringw tris = core::stringw(driver->getPrimitiveCountDrawn());
		core::stringw newtris = L"";
		addCommas(tris, newtris);
		str += newtris;
		str += L" Demo: ";
		if(demomode)
			str += L"On";
		else
			str += L"Off";
		str += L" Game: ";
		if(gamemode==GAME_LEAD)
			str += L"Lead";
		else
			str += L"Normal";
		str += L" Riders: ";
		str.append(core::stringw(numriders));
		str += L" Sun: ";
		str.append(core::stringw((int)angleLight));
		str += L" FOV: ";
		str.append(core::stringw((int)angleFOV));
		str += L" CamDist: ";
		str.append(core::stringw((int)distCam));
		str += L" Cull: ";
		str.append(core::stringw((int)fardistCam));

		if(!m_bWPF)
		{
			str += L" CamMode: ";
			if(camtype==RIDER_CAM)
				str.append(L"Fixed");
			else if(camtype==MAYA_CAM)
				str.append(L"Control from Mouse");
			else
				str.append(L"Fly");
		}

		u32 count = (demoStart - gFTime.simTime) / 1000;
		if(demomode && demoStart >= gFTime.simTime)
		{
			str += L"\n CountDown: ";
			str.append(core::stringw(count));
		}
		{
			str += L" Drafting:";
			for (int i = 0; i < MAX_RIDERS; i++) 
			{
				if(rider[i]->IsRiderDrafting())
				{
					str += L" ";
					str.append(core::stringw(i+1));
				}
			}
		}
		if(m_bWPF)
		{
			text->setText(str.c_str());
			text->setOverrideColor(video::SColor(255,255,255,255));
			text->setTextAlignment(gui::EGUIA_UPPERLEFT,gui::EGUIA_CENTER);
		}
		else
			device->setWindowCaption(str.c_str());

		lastFPS = fps;
		bChanged = false;
	}
//  busy = false;
	gFTime.runTime = device->getTimer()->getRealTime();

	if(gpCourse != NULL)
		gpCourse->ShowVisible(NULL, NULL, false);
	//if(gpCourse != NULL)
	//  gpCourse->HideAll();

	Leave();
	return hr;
}
