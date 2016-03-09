#pragma once

enum RIDER_GENDER {
	RIDER_MALE,
	RIDER_FEMALE
};

enum RIDER_TYPE {
	RIDER_BOX,
	RIDER_TRIATHLON,
	RIDER_ROAD,
	RIDER_CHROME,
	RIDER_GOLD,
	RIDER_ONYX,
	RIDER_TYPE_MAX
};

typedef struct {
	RIDER_GENDER gender;
	RIDER_TYPE type;
	char *model;
} RModel;

enum RIDER_TEX {
	TEX_BOX			=0,
	TEX_CLOTHES		=TEX_BOX+1,
	TEX_BODY		=TEX_CLOTHES+4,
	TEX_ACCESS		=TEX_BODY+2,
	TEX_JERSEY		=TEX_ACCESS+1,
	TEX_FRAME		=TEX_JERSEY+3,
	TEX_MAX			=TEX_FRAME+3
};

/*
enum RIDER_MODEL {
	MODEL_BOX=0,
	MODEL_MALE,
	MODEL_FEMALE,
	MODEL_CHROME,
	MODEL_GOLD,
	MODEL_X,
	MODEL_MALE_ROAD,
	MODEL_FEMALE_ROAD,
	MODEL_CHROME2,
	MODEL_MAX
};
*/
enum RIDER_MODEL {
	MODEL_BOX=0,
	MODEL_MALE,
	MODEL_FEMALE,
	MODEL_MALE_ROAD,
	MODEL_FEMALE_ROAD,
	MODEL_MALE_CHROME,
	MODEL_FEMALE_CHROME,
	MODEL_MALE_GOLD,
	MODEL_FEMALE_GOLD,
	MODEL_X,
	MODEL_MAX
};

enum RIDER_CAMTYPE {
	CAMTYPE_NORMAL,
	CAMTYPE_FIXED,
	CAMTYPE_AI
};

enum RIDER_CAMMODE {
	CAM_FOLLOW=0,
	CAM_FIRST_PERSON,
	CAM_REAR_VIEW,
	CAM_RANDOM,
	CAM_RANDOM_NEAR,
	CAM_FIX,
	CAM_NORMAL_MAX,
	CAM_DEMORIDER=CAM_NORMAL_MAX,
	CAM_MAX
};

#define MAX_RIDER_TYPES MODEL_MAX
//#define MAX_RIDER_TYPES 4
class Rider : public scene::ISceneNode
{
	int m_rideMode;
	int maxtype;
	int m_type;
	bool m_bColorChanged;


	scene::ISceneNodeAnimator* m_cameraAnimNode;
	scene::ISceneNodeAnimator* m_cameraAnimNode2;
	scene::ISceneNode* m_cameraNode;
	scene::ISceneNode* m_cameraNode2;
	scene::ISceneNode* m_cameraTargetNode;
	scene::ISceneNode* m_modelNode;
	scene::ISceneNode* m_modelShadowNode;
	scene::CShadowSceneNode* m_modelShadow;
	scene::CSpriteSceneNode* m_pNumbers;
	scene::ICameraSceneNode* m_camera[2];
	scene::IAnimatedMeshSceneNode* m_model[MAX_RIDER_TYPES];
	scene::IAnimatedMeshSceneNode* m_modelShadowMesh;

	AvgFilter m_yawfilter;
	AvgFilter m_leanfilter;
	AvgFilter m_distfilter;
	AvgFilter m_camdistfilter;
	//! The bounding box of this mesh
	core::aabbox3d<f32> BoundingBox;

public:
	CCourse::Section *m_psec;
	f64 m_dist;
	f64 m_totaldist;
	f64 m_rdist;
	f64 m_lastdist;
	f32 m_deltaspeed;
	
	u32 m_laneNeedChange;
	int m_timeChange;
	int m_lane;
	int m_desiredLane;
	int m_targetLane;
	int m_prevLane;
	f32 m_xpos;
	f32 m_prevxpos;

	bool m_isColliding;
	u32 m_collisionHold;

	bool m_isDrafting;
	u32 m_draftingHold;

	u32 m_camNeedChange;
	int m_camMode;
	int m_camtype;
	f32 m_camArc;
	f32 m_camHeight;
	f32 m_camDist;

	f32 m_lean;
	f32 m_yaw;

	f32 m_newspeed;
	f32 m_speed;

	f32 m_newvelForw;
	f32 m_velForw;
	f32 m_newvelSide;
	f32 m_velSide;

	f32 m_newaccel;
	f32 m_accel;

	int m_state; 
	int m_oldstate; 
	bool m_bReal;
	bool m_initstart;
	bool m_bShow;

	f32 m_yadd;

	//vector3df m_dirvec;
	vector3df m_dirvecx;
	vector3df m_lastPos;

	u32 m_Colors[RIDER_COLORS_MAX];


	Rider(int  imode,ISceneNode* parent, ISceneManager* mgr, s32 id=-1,
				const core::vector3df& position = core::vector3df(0,0,0),
				const core::vector3df& rotation = core::vector3df(0,0,0),
				const core::vector3df& scale = core::vector3df(1.0f, 1.0f, 1.0f)
				) ;


	scene::ICameraSceneNode* getCamera() {return m_camera[m_camtype];}
	int getCameraMode() {return m_camMode;}
	void SetCameraMode(int camMode);
	void setCameraMode(int camMode) 
	{ 
		bDemoRider ? CAM_DEMORIDER : m_camMode = max(min(camMode,CAM_NORMAL_MAX-1),CAM_FOLLOW); 
	}
	scene::IAnimatedMeshSceneNode* getModel(int type = -1) 
	{
		if(type=-1) 
			type = m_type;
		return m_model[type];
	}
	scene::ISceneNode* getModelNode() {return m_modelNode;} 

	//! recalculates the bounding box
	void recalculateBoundingBox()
	{
		BoundingBox.reset(0.0f, 0.0f, 0.0f);

		ISceneNodeList::Iterator it = Children.begin();
		for (; it != Children.end(); ++it)
			BoundingBox.addInternalBox((*it)->getBoundingBox());
	}
	void Reset(int state = PRESTART);
	void SimpleReset();
	virtual const core::aabbox3d<f32>& getBoundingBox() const {return BoundingBox;}

	virtual void UpdatePosition();
	virtual void Update();
	virtual void UpdateAfter();
	virtual void render();

	f32 getFrameNr();
	void preRender();

	virtual void setVisible(bool bVis=true);
	void setAnimationSpeed(f32 framesPerSecond);
	void SetSpeed(float speed);
	void SetDist(float dist) {m_dist = dist;}
	void SetRDist(float dist) {m_rdist = dist;}
	void SetState(CState st) {m_state = st;}

	int UpdateCameraMode();
	int NextCameraMode();
	void AnimateRiderCamera();
	void useRider(int x);
	f64 GetDist() {return m_dist;}
	void SetReal(bool bReal) {m_bReal = bReal;}
	void SetRideMode(int iMode) {m_rideMode = iMode;}
	int GetRideMode() {return m_rideMode;}
	void AddSpeed(float speed);
	void AddDist(float dist);
	int GetModelType() {return m_type;}

	void ShowShadow(bool bVis=true){m_modelShadow->setVisible(bVis);}

	void ClearModel();
	void ChangeMatColor(int i, SColor color, int itex = -1 );
	void ChangeColor(RIDER_COLORS idx, SColor color, int itex = -1);
	void SetColor(RIDER_COLORS idx, SColor color);

	int SetRiderAttr( int ridermodel, int bikemodel, int tiremodel, float bike_frame_size, float tire_size, float rider_height, float rider_weight, SColor color) {}
	bool IsRiderDrafting() {return m_isDrafting;}
	void SetShadow();
	void SetRiderNumber(int inum);


};
