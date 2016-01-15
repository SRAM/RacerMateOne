#include "StdAfx.h"

f32 leanFactor = 1.2f; //20.0f; //1000.0f;
f32 passFactor = 0.035f; //0.125f; // 0.1f;
float lookAhead = 15.0f;

const char *modelTex[] = {
"UV.tga",                       // 0 TEX_BOX

"RacerMate_ShirtGray.tga",      // 0 TEX_CLOTHES
"RacerMate_ShortsGray.tga",     // 1
"RacerMate_ShirtGray.tga",             // 2 changed from racerX
"RacerMate_ShortsGray.tga",            // 3 changed from racerx

"RacerMale_Body.tga",           // 0 TEX_BODY
"RacerFemale_Body.tga",         // 1

"RacerMale_Access.tga",         // 0 TEX_ACCESS

"Racer_Chrome.tga",             // 0 TEX_JERSEY
"Racer_Gold.tga",               // 1
"Racer_Chrome.tga",                  // 2  //changed from Racer_X

"BIKE_Frame_Nuetral.tga",       // 0 TEX_FRAME
"BIKE_Frame_Gold.tga",          // 1
"BIKE_Frame_Nuetral.tga",             // 2  //changed from Racer_X
};

//const char *maleModels[] = {
//"box_test.X",               // 0
//"RacerMale_WhiteGray.X",    // 1    Tria
//"Racer_OldBikeMale.X",      // 2    Road
//"Racer_Chrome.X",           // 3    Chrome
//"Racer_Chrome.X",           // 4    Gold
//"Racer_X.X",                // 5    Onyx
//};

//const char *femaleModels[] = {
//"box_test.X",               // 0
//"RacerFemale_WhiteGray.X",  // 1    Tria
//"Racer_OldBikeFemale.X",    // 2    Road
//"Racer_ChromeFemale.X",     // 3    Chrome
//"Racer_ChromeFemale.X",     // 4    Gold
//"Racer_X.X",                // 5    Onyx
//};

const char *modelNames[] = {
//"box_test.X",               // 0
//"RacerMale_WhiteGray.X",    // 1    Male
//"RacerFemale_WhiteGray.X",  // 2    Female
//"Racer_OldBikeMale.X",      // 6    OldBike Male
//"Racer_OldBikeFemale.X",    // 7    OldBike Female
//"Racer_Chrome.X",           // 3    Chrome Male
//"Racer_ChromeFemale.X",     // 4    Chrome Female
//"Racer_Chrome.X",           // 3    Gold Male
//"Racer_ChromeFemale.X",     // 4    Gold Female
//"Racer_X.X",                // 5    Female Onyx
//};
"box_test.X",               // 0
"RacerMale_WhiteGray.X",    // 1    Male
"RacerFemale_WhiteGray.X",  // 2    Female
"Racer_OldBikeMale.X",      // 3    OldBike Male
"Racer_OldBikeFemale.X",    // 4    OldBike Female
"Racer_Chrome.X",           // 5    Chrome Male
"Racer_ChromeFemale.X",     // 6    Chrome Female
"Racer_Chrome.X",           // 7    Gold Male
"Racer_ChromeFemale.X",     // 8    Gold Female
"Racer_Male_WhiteGray.X",                // 9    chnaged from Racer_X Female Onyx
};


Rider::Rider(int imode, ISceneNode* parent, ISceneManager* smgr, s32 id,
				const core::vector3df& position,
				const core::vector3df& rotation,
				const core::vector3df& scale) 
: ISceneNode(parent, smgr, id, position, rotation, scale)
{
	BoundingBox.reset(1.0f, 1.0f, 1.0f);
	m_bShow = true;
	m_rideMode = imode; // pacer, realrider
	m_bReal = (imode != 0); // controlled externally
	m_type = 1;
	m_psec = NULL;
	m_bColorChanged = false;

	m_Colors[RIDER_BIKE] = SColor(255, 60, 160, 240).color;
	m_Colors[RIDER_TIRES] = SColor(255, 255, 255, 255).color;
	m_Colors[RIDER_SKIN] = SColor(255, 245, 217, 190).color; //0xffFF8080;
	m_Colors[RIDER_HAIR] = 0xff804010;
	m_Colors[RIDER_HELMET] = 0xffF08000;
	m_Colors[RIDER_JERSEY_TOP] = 0xffFF0000; // red
	m_Colors[RIDER_JERSEY_BOTTOM] = 0xff0000FF; // blue
	m_Colors[RIDER_SHOES] = 0xff202020;


	for(int i=0; i<MAX_RIDER_TYPES; i++)
	{
		m_model[i] = 0;
	}

	if(id==0)
	{
		for(int i=0; i<MODEL_MAX; i++)
		{
			D3DBase::GetSceneManager()->getMesh(modelNames[i]);
		}
	}

	m_modelNode = D3DBase::addCustomSceneNode(this);

	if(!bRealShadows)
	{
		m_modelShadow = new CShadowSceneNode(
			this, D3DBase::GetSceneManager(), -1, 
			//vector3df(1.0f,0.4f+id*0.01f,-0.1f), 
			vector3df(0,0.6f+id*0.02f,0), 
			dimension2d<f32>(mult*1.4f,mult*1.4f),rect<f32>(0,0,0.5f,0.5f),
			SColor(60,0,0,0),SColor(224,0,0,0));
		m_modelShadow->drop();
		m_modelShadow->setScale(core::vector3df(1.2f*mult,1.2f*mult,1.2f*mult));
		m_modelShadow->setMaterialFlag(video::EMF_LIGHTING, false);
		m_modelShadow->setMaterialFlag(video::EMF_COLOR_MATERIAL, false);
		m_modelShadow->setMaterialType(video::EMT_ONETEXTURE_BLEND);
		m_modelShadow->getMaterial(0).MaterialTypeParam = pack_texureBlendFunc(EBF_SRC_ALPHA, EBF_ONE_MINUS_SRC_ALPHA,EMFN_MODULATE_1X,(EAS_VERTEX_COLOR|EAS_TEXTURE));

		SetShadow();
		
	}

	maxtype = MAX_RIDER_TYPES;

	m_lane = id;
	m_speed = float(rand() % maxspeed);
	if(m_speed < 10.0f)
		m_speed = 10.0f;

	m_cameraTargetNode = D3DBase::addCustomSceneNode(mainNode);

	m_cameraNode = D3DBase::addCustomSceneNode(m_cameraTargetNode);
	m_cameraNode->setPosition(vector3df(0,camh+camh/3,0));
	m_cameraNode->setRotation(vector3df(0,0,0));

	m_cameraNode2 = m_cameraNode;
	//m_cameraNode2 = D3DBase::addCustomSceneNode(m_cameraNode);
	//m_cameraNode2->setPosition(vector3df(0,0,0));
	//m_cameraNode2->setRotation(vector3df(0,0,0));

	m_camera[CAMTYPE_NORMAL] = smgr->addCameraSceneNode(m_cameraNode2, vector3df(0,0,-distCamM), vector3df(0,camh/2,0),-1,false);
	if(m_camera[CAMTYPE_NORMAL])
	{
		m_camera[CAMTYPE_NORMAL]->setFarValue(fardistCam*mult);
		m_camera[CAMTYPE_NORMAL]->setFOV(degToRad(angleFOV));
	}
	m_camera[CAMTYPE_FIXED] = smgr->addCameraSceneNode(mainNode, vector3df(0,2*camh+camh/2,distCamM));
	if(m_camera[CAMTYPE_FIXED])
	{
		m_camera[CAMTYPE_FIXED]->setFarValue(fardistCam * mult);
		m_camera[CAMTYPE_FIXED]->setFOV(degToRad(angleFOV));
	}

	m_pNumbers = new CSpriteSceneNode(
		m_modelNode, D3DBase::GetSceneManager(), -1, 
		vector3df(0,camh*1.125f,0), dimension2d<f32>(0.25f,0.25f)*mult,rect<f32>((0.0f+id)/8,0,(1.00048828125f+id)/8,1.00390625f),
		SColor(96,255,255,255),SColor(96,255,255,255));
	m_pNumbers->drop();
	m_pNumbers->setMaterialFlag(video::EMF_LIGHTING, false);
	m_pNumbers->setMaterialTexture(0, D3DBase::GetDriver()->getTexture("Numbers.png"));
	m_pNumbers->setMaterialType(video::EMT_ONETEXTURE_BLEND);
	m_pNumbers->getMaterial(0).MaterialTypeParam = pack_texureBlendFunc(EBF_SRC_ALPHA,EBF_ONE_MINUS_SRC_ALPHA,EMFN_MODULATE_1X,(EAS_VERTEX_COLOR|EAS_TEXTURE));

	recalculateBoundingBox();
	m_cameraAnimNode = m_cameraAnimNode2 = 0;

	Reset();
}

void Rider::Reset(int state)
{
	vector3df roadpos,roadvec, vpos, vvec,vtarget, vrot, vrot2, cvpos, cvrot;
	f32 local_grade_d_100, roadgrade, secrot, wind, yaw, xpos;
	int hint=0;

	m_oldstate = -1;
	m_dist=0;
	m_rdist=0;
	m_newvelForw=0;
	m_velForw=0;
	m_lean=0;
	m_yaw=0;
	m_camtype = CAMTYPE_FIXED;      
	m_camDist=0;
	m_isDrafting = false;
	m_draftingHold = 0;
	m_collisionHold = 0;
	//m_camMode = CAM_FOLLOW;

	m_distfilter.reset(0,15);

	if(bDemoRider)
	{
		m_speed = float(rand() % maxspeed);
		if(m_speed < 10.0f)
			m_speed = 10.0f;
		if(getID() == 0)
		{
			m_camtype = CAMTYPE_NORMAL;          
			m_camMode = CAM_DEMORIDER; // animatecam
			useRider(m_type);
			setVisible(true);
		}
		else
		{
			setVisible(false);
		}
	}
	else
	{
		if(getID() == 0)
			ShowShadow(!bDemoRider);

		SetCameraMode(m_camMode);
	}

	m_laneNeedChange = gFTime.simTime + 1000;
	if(gpCourse)
	{
		m_psec = gpCourse->RoadLoc((float)(m_dist - bikeLength), vpos, vvec, roadgrade, hint, &m_yaw, &secrot, &wind, m_psec);
		m_lastPos = vpos;
		gpCourse->RoadLoc((f32)m_dist, roadpos, roadvec, local_grade_d_100, BIKE_HINTS, &yaw, &secrot, &wind, m_psec);

		vrot = getRotation();
		vrot.y = radToDeg(m_yaw);
		setRotation(vrot);

		m_desiredLane = 0;
		m_targetLane = m_prevLane = m_lane = startLane[getID()];
		m_prevxpos = m_xpos = riderLanes[m_lane]; //riderStart[getID()];
		xpos = m_xpos;
		vpos += vvec * xpos;
		vpos *= mult;

		setPosition(vpos);
		updateAbsolutePosition();
	}

	// Camera target
	m_cameraTargetNode->setPosition(vpos);
	m_cameraTargetNode->setRotation(core::vector3df(0,m_yaw,0));
	m_cameraTargetNode->updateAbsolutePosition();

	m_state = state;
	m_initstart = false;

	m_speed = m_newspeed = float(rand() % (maxspeed - 10)) + 10;
	setAnimationSpeed(0);
	m_leanfilter.reset(radToDeg(m_lean), 12);
	m_yawfilter.reset(0, 12);
	m_camdistfilter.reset(0, 10);


	getModelNode()->setRotation(core::vector3df(0,0,(f32)m_leanfilter.getval()));
	getModelNode()->updateAbsolutePosition();

	m_cameraNode->setPosition(vector3df(0,camh+camh/3,0));
	m_cameraNode->setRotation(core::vector3df(0,0,0));
	m_cameraNode->updateAbsolutePosition();

	m_camera[CAMTYPE_NORMAL]->setPosition(vector3df(0,0,-distCamM/2));

	//vector3df 
	vtarget = getAbsolutePosition();

	if(getID() == 0)
	{
		D3DBase::prevLeadRider = D3DBase::leadRider = 0;
		D3DBase::prevLeadTarget = D3DBase::leadTarget = getPosition();
		holdChange = gFTime.simTime + 1500;
	}

	vtarget = m_cameraTargetNode->getAbsolutePosition();

	if(bDemoRider)
	{
		m_camera[CAMTYPE_NORMAL]->setTarget(vector3df(vtarget.x,vtarget.y+camh/3+camh/3,vtarget.z));
	}
	else
	{
		switch(gamemode)
		{
		default:
		case GAME_NORMAL:
			m_camera[CAMTYPE_NORMAL]->setTarget(vector3df(vtarget.x,vtarget.y+camh/2,vtarget.z));
			break;
		case GAME_LEAD:
			m_camera[CAMTYPE_NORMAL]->setTarget(vector3df(vtarget.x,vtarget.y+camh/2,vtarget.z));
			break;
		}
	}

	m_camNeedChange = gFTime.simTime + (rand() % 15000) + 6000;

	AnimateRiderCamera();
}

void Rider::SimpleReset()
{
	return;
	/*
	vector3df roadpos,roadvec, vpos, vvec,vtarget, vrot, vrot2, cvpos, cvrot;
	f32 local_grade_d_100, roadgrade, secrot, wind, yaw, xpos;
	int hint=0;

	m_oldstate = -1;
	m_dist=0;
	m_rdist=0;
	m_newvelForw=0;
	m_velForw=0;
	m_lean=0;
	m_yaw=0;
	m_camDist=0;
	m_isDrafting = false;
	m_draftingHold = 0;
	m_collisionHold = 0;
	m_camMode = CAM_FOLLOW;
	m_camtype = CAMTYPE_NORMAL;          

	m_distfilter.reset(0,15);

	if(bDemoRider)
	{
		m_speed = float(rand() % maxspeed);
		if(m_speed < 10.0f)
			m_speed = 10.0f;
		if(getID() == 0)
		{
			m_camtype = CAMTYPE_NORMAL;          
			m_camMode = CAM_DEMORIDER; // animatecam
			useRider(m_type);
			setVisible(true);
		}
		else
		{
			setVisible(false);
		}
	}
	else
	{
		if(getID() == 0)
			ShowShadow(!bDemoRider);

		SetCameraMode(m_camMode);
	}

	m_laneNeedChange = gFTime.simTime + 1000;
	if(gpCourse)
	{
		m_psec = gpCourse->RoadLoc((float)(m_dist - bikeLength), vpos, vvec, roadgrade, hint, &m_yaw, &secrot, &wind, m_psec);
		m_lastPos = vpos;
		gpCourse->RoadLoc((f32)m_dist, roadpos, roadvec, local_grade_d_100, BIKE_HINTS, &yaw, &secrot, &wind, m_psec);

		vrot = getRotation();
		vrot.y = radToDeg(m_yaw);
		setRotation(vrot);

		m_desiredLane = 0;
		m_targetLane = m_prevLane = m_lane = startLane[getID()];
		m_prevxpos = m_xpos = riderLanes[m_lane]; //riderStart[getID()];
		xpos = m_xpos;
		vpos += vvec * xpos;
		vpos *= mult;

		setPosition(vpos);
		updateAbsolutePosition();
	}
	else
	{
		vpos = core::vector3df(0,0,0);
		vrot = core::vector3df(0,0,0);
	}

	// Camera target
	m_cameraTargetNode->setPosition(vpos);
	m_cameraTargetNode->setRotation(vrot);
	m_cameraTargetNode->updateAbsolutePosition();

	m_state = state;
	m_initstart = false;

	m_speed = m_newspeed = float(rand() % (maxspeed - 10)) + 10;
	setAnimationSpeed(0);
	m_leanfilter.reset(radToDeg(m_lean), 12);
	m_yawfilter.reset(0, 12);
	m_camdistfilter.reset(0, 10);


	getModelNode()->setRotation(core::vector3df(0,0,(f32)m_leanfilter.getval()));
	getModelNode()->updateAbsolutePosition();

	m_cameraNode->setPosition(vector3df(0,camh+camh/3,0));
	m_cameraNode->setRotation(core::vector3df(0,0,0));
	m_cameraNode->updateAbsolutePosition();

	m_camera[CAMTYPE_NORMAL]->setPosition(vector3df(0,0,-distCamM/2));

	//vector3df 
	//vtarget = getAbsolutePosition();

	if(getID() == 0)
	{
		D3DBase::prevLeadRider = D3DBase::leadRider = 0;
		D3DBase::prevLeadTarget = D3DBase::leadTarget = getPosition();
		holdChange = gFTime.simTime + 1500;
	}

	m_cameraTargetNode->setPosition(vpos);
	m_cameraTargetNode->updateAbsolutePosition();
	vtarget = m_cameraTargetNode->getAbsolutePosition();

	if(bDemoRider)
	{
		m_camera[CAMTYPE_NORMAL]->setTarget(vector3df(vtarget.x,vtarget.y+camh/3+camh/3,vtarget.z));
	}
	else
	{
		switch(gamemode)
		{
		default:
		case GAME_NORMAL:
			m_camera[CAMTYPE_NORMAL]->setTarget(vector3df(vtarget.x,vtarget.y+camh/2,vtarget.z));
			break;
		case GAME_LEAD:
			m_camera[CAMTYPE_NORMAL]->setTarget(vector3df(vtarget.x,vtarget.y+camh/2,vtarget.z));
			break;
		}
	}

	m_camNeedChange = gFTime.simTime + (rand() % 15000) + 6000;

	AnimateRiderCamera();
	*/
}

void Rider::SetRiderNumber(int inum)
{
	if(inum >= 0 && inum < MAX_RIDERS)
		m_pNumbers->setUVRect(rect<f32>((0.0f+inum)/8,0,(1.00048828125f+inum)/8,1.00390625f));
}

void Rider::SetCameraMode(int camMode)
{
	if(bDemoRider)
	{
		m_camMode = CAM_DEMORIDER;
		m_camera[CAMTYPE_NORMAL]->setFOV(degToRad(demoAngleFOV/4));
	}
	else
	{
		// validate to valid value
		m_camMode = max(min(camMode,CAM_NORMAL_MAX-1),CAM_FOLLOW);

		float factor = float(rand() % 10) / 10;
		float fov = angleFOV;
		if(m_camMode == CAM_RANDOM_NEAR)
			fov = float( (angleFOV/2 - angleFOV/4) * factor) + angleFOV/4;
		else if(m_camMode == CAM_RANDOM || m_camMode == CAM_FIX)
			fov = float( (angleFOV - angleFOV/2) * factor) + angleFOV/2;

		m_camera[CAMTYPE_NORMAL]->setFOV(degToRad(fov));
	}
	// assign the mode to the view
	int i;
	for(i=0; i<MAX_RIDERS; i++)
	{
		int irider = getID();
		if(irider == D3DBase::view[i]->GetRiderIdx())
		{
			D3DBase::view[i]->SetCameraIdx(m_camMode);
		}
	}
}

int Rider::UpdateCameraMode()
{
	m_camdistfilter.reset(0, 10);
	UpdateAfter();

	m_cameraNode->setPosition(vector3df(0,camh+camh/3,0));
	m_cameraNode->setRotation(core::vector3df(0,0,0));
	m_cameraNode->updateAbsolutePosition();

	m_camera[CAMTYPE_NORMAL]->setFarValue(fardistCam * mult);
	m_camera[CAMTYPE_NORMAL]->setFOV(degToRad(angleFOV));
	m_camera[CAMTYPE_NORMAL]->setPosition(vector3df(0,0,-distCamM/2));
	m_camera[CAMTYPE_NORMAL]->updateAbsolutePosition();

	m_camera[CAMTYPE_FIXED]->setFarValue(fardistCam * mult);
	m_camera[CAMTYPE_FIXED]->setFOV(degToRad(angleFOV));
	m_camera[CAMTYPE_FIXED]->setPosition(vector3df(0,2*camh+camh/2,distCamM));
	m_camera[CAMTYPE_FIXED]->updateAbsolutePosition();

	SetCameraMode(m_camMode);
	m_camNeedChange = gFTime.simTime + (rand() % 15000) + 6000;
	AnimateRiderCamera();

	if (state < RACE)
		m_initstart = false;

	return m_camMode;
}

int Rider::NextCameraMode()
{
	int camMode = m_camMode + 1;
	
	if(camMode > CAM_RANDOM)
		camMode = CAM_FOLLOW;
	else if(camMode == CAM_RANDOM)
		camMode = ((rand() % 100) > 50 ? CAM_RANDOM : CAM_RANDOM_NEAR);

	SetCameraMode(camMode);
	AnimateRiderCamera();
	return m_camMode;
}

void Rider::AnimateRiderCamera()
{
	if(m_cameraAnimNode)
	{
		m_cameraNode->removeAnimator(m_cameraAnimNode);
		m_cameraAnimNode->drop();
		m_cameraAnimNode = 0;
	}
	if(m_cameraAnimNode2)
	{
		m_cameraNode2->removeAnimator(m_cameraAnimNode2);
		m_cameraAnimNode2->drop();
		m_cameraAnimNode2 = 0;
	}

	switch(m_camMode)
	{
	case CAM_RANDOM:
	case CAM_RANDOM_NEAR:
		{
			float rot = (1.0f + float(rand() % 60)/30.0f) * ((rand() % 100) > 50 ? -0.05f : 0.05f);
			m_cameraAnimNode =  getSceneManager()->createRotationAnimator(core::vector3df(0, rot, 0));
			if(m_cameraAnimNode)
			{
				m_cameraNode->addAnimator(m_cameraAnimNode);
				m_cameraAnimNode->animateNode(m_cameraNode, D3DBase::GetDevice()->getTimer()->getTime()+(rand() % 1000)+500);
			}
			m_cameraAnimNode2 = getSceneManager()->createFlyStraightAnimator(
				core::vector3df(0, camh/2, distCamM/4),
				core::vector3df(0, 2*camh, -distCamM/4),
				(rand() % 8000)+12000,
				true,
				true
				);
			if(m_cameraAnimNode2)
			{
				m_cameraNode2->addAnimator(m_cameraAnimNode2);
				m_cameraAnimNode2->animateNode(m_cameraNode2,(rand() % 8000)+12000);
			}
		}
		break;
	case CAM_FOLLOW:
		{
			m_cameraNode->setPosition(vector3df(0,camh+camh/3,0));
			m_cameraNode->setRotation(core::vector3df(0,0,0));
			m_cameraNode->updateAbsolutePosition();
		}
		break;
	case CAM_DEMORIDER:
		{
			float rot = (3.5f + float(rand() % 90)/30) * ((rand() % 100) > 50 ? -0.05f : 0.05f);
			m_cameraAnimNode =  getSceneManager()->createRotationAnimator(core::vector3df(0, rot, 0));
			if(m_cameraAnimNode)
			{
				m_cameraNode->addAnimator(m_cameraAnimNode);
				//m_cameraAnimNode->animateNode(m_cameraNode, D3DBase::GetDevice()->getTimer()->getTime()+(rand() % 1000)+500);
			}
			m_cameraAnimNode2 = getSceneManager()->createFlyStraightAnimator(
				core::vector3df(0, -(camh/3), distCamM/4),
				core::vector3df(0, camh, -distCamM/4),
				(rand() % 8000)+12000,
				true,
				true
				);
			if(m_cameraAnimNode2)
			{
				m_cameraNode2->addAnimator(m_cameraAnimNode2);
				m_cameraAnimNode2->animateNode(m_cameraNode2,(rand() % 8000)+12000);
			}
		}
		break;
	default:
		break;
	}
}

void Rider::SetSpeed(float speed)
{
	m_speed = speed;
}

void Rider::AddSpeed(float speed)
{
	m_speed += speed;
}

void Rider::AddDist(float dist)
{
	if(m_state == RACE)
		m_dist += dist;
}


void Rider::UpdateAfter()
{
	vector3df vpos, vrot, vtarget, vcampos, vcamrpos, vcoll;
	vpos = getPosition();

	if(bDemoRider)
	{
		// Camera target
		m_cameraTargetNode->setPosition(vpos);
		m_cameraTargetNode->updateAbsolutePosition();

		vtarget = m_cameraTargetNode->getAbsolutePosition();
		m_camera[m_camtype]->setTarget(vector3df(vtarget.x,vtarget.y+camh/3+camh/3,vtarget.z));     
	}
	else 
	{
		if(m_camtype == CAMTYPE_FIXED)
		{
			if(gamemode == GAME_LEAD)
				vpos = D3DBase::leadTarget;
			m_cameraTargetNode->setPosition(vpos);
		}
		else
		{
			switch(gamemode)
			{
			default:
			case GAME_NORMAL:
				if(m_camMode == CAM_RANDOM || m_camMode == CAM_RANDOM_NEAR || m_camMode == CAM_FIX)
				{
					m_cameraTargetNode->setPosition(vpos);
				}
				else if(m_camMode == CAM_FIRST_PERSON)
				{
					vrot = getRotation();
					vrot.x = 0;
					vector3df vdir = vrot.rotationToDirection();
					vector3df vposeye = vpos + vdir*distCamM*eyeAdjust/distCam;
					vposeye.y -= 1 + camh/3;
					m_cameraTargetNode->setPosition(vposeye);
					m_cameraTargetNode->setRotation(vrot);
				}
				else if(m_camMode == CAM_REAR_VIEW)
				{
					//vpos = m_lastPos * mult;
					m_cameraTargetNode->setPosition(vpos);
					vrot = getRotation();
					vrot.y += 180;
					vrot.x = 0;
					m_cameraTargetNode->setRotation(vrot);
				}
				else if(m_camMode == CAM_FOLLOW)
				{
					vrot = getRotation();
					vrot.x = 0;
					vector3df vdir = vrot.rotationToDirection();
					vpos.y = vpos.y + camh/8;
					float addy = (m_camDist * camh * 8);
					if(m_camDist < 0)
					{
						vpos -= vdir * (m_camDist * distCamM * 0.75f);
						addy = -m_camDist * camh * 0.8f;
					}
					vpos.y += addy;
					m_cameraTargetNode->setPosition(vpos);
					m_cameraTargetNode->setRotation(vrot);
				}
				else
				{
					//vpos = m_lastPos * mult;
					m_cameraTargetNode->setPosition(vpos);
					vrot = getRotation();
					vrot.x = 0;
					m_cameraTargetNode->setRotation(vrot);
				}
				break;
			case GAME_LEAD:
				vpos = D3DBase::leadTarget;
				if(m_camMode == CAM_RANDOM || m_camMode == CAM_RANDOM_NEAR || m_camMode == CAM_FIX)
				{
					m_cameraTargetNode->setPosition(vpos);
				}
				else if(m_camMode == CAM_FIRST_PERSON)
				{
					vrot = D3DBase::rider[D3DBase::leadRider]->getRotation();
					vrot.x = 0;
					vector3df vdir = vrot.rotationToDirection();
					vector3df vposeye = vpos + vdir*distCamM*eyeAdjust/distCam;
					vposeye.y -= 1 + camh/3;
					m_cameraTargetNode->setPosition(vposeye);
					m_cameraTargetNode->setRotation(vrot);
				}
				else if(m_camMode == CAM_REAR_VIEW)
				{
					m_cameraTargetNode->setPosition(vpos);
					vrot = D3DBase::rider[D3DBase::leadRider]->getRotation();
					vrot.y += 180;
					vrot.x = 0;
					m_cameraTargetNode->setRotation(vrot);
				}
				else if(m_camMode == CAM_FOLLOW)
				{
					vrot = D3DBase::rider[D3DBase::leadRider]->getRotation();
					vrot.x = 0;
					vector3df vdir = vrot.rotationToDirection();
					vpos.y = vpos.y + camh/8;
					float addy = (m_camDist * camh * 8);
					if(m_camDist < 0)
					{
						vpos -= vdir * (m_camDist * distCamM * 0.75f);
						addy = -m_camDist * camh * 0.8f;
					}
					vpos.y += addy;
					m_cameraTargetNode->setPosition(vpos);
					m_cameraTargetNode->setRotation(vrot);
				}
				else
				{
					//vpos = D3DBase::rider[D3DBase::leadRider]->m_lastPos * mult;
					m_cameraTargetNode->setPosition(vpos);
					vrot = D3DBase::rider[D3DBase::leadRider]->getRotation();
					vrot.x = 0;
					m_cameraTargetNode->setRotation(vrot);
				}
				break;
			}
		}

		m_cameraTargetNode->updateAbsolutePosition();
		vtarget = m_cameraTargetNode->getAbsolutePosition();
		m_camera[m_camtype]->setTarget(vector3df(vtarget.x,vtarget.y+camh/2,vtarget.z));

		//m_camera[m_camtype]->setTarget(vector3df(vtarget.x,vtarget.y,vtarget.z));

		// * TODO -  fix the ground collision routine so camera does not go under ground

		if(m_camtype == CAMTYPE_NORMAL && (m_camMode == CAM_RANDOM || m_camMode == CAM_RANDOM_NEAR))
		{
			vcamrpos = vector3df(0,0,-distCamM/2);
			m_camera[m_camtype]->setPosition(vcamrpos);
			m_camera[m_camtype]->updateAbsolutePosition();
			vcampos = m_camera[m_camtype]->getAbsolutePosition();
			vcampos.y -= 2.0f;
			vcoll = vcampos;
			CCourse::Section *psec;
			bool bhit = false;
			for(int n = 0; n < 3; n++)
			{
				switch(n)
				{
				case 0:
					psec = m_psec;
					break;
				case 1:
					psec = m_psec->Prev();
					break;
				case 2:
					psec = m_psec->Next();
					break;
				}
				if(!psec)
					continue;

				bhit = D3DBase::GetHeightFromWorld(vcoll, psec->GetSectionNode());
				if(bhit)
				{
					if(vcoll.y > vcampos.y)
					{
						vcamrpos.y += vcoll.y - vcampos.y;
						m_camera[m_camtype]->setPosition(vcamrpos);
						m_camera[m_camtype]->updateAbsolutePosition();
					}
					break;
				}
			}
			if(!bhit)
			{
				int bp = 1;
			}
		}
		// * /
		m_camera[m_camtype]->updateAbsolutePosition();
	}

	if(m_camNeedChange < gFTime.simTime)
	{
		m_camNeedChange = gFTime.simTime + (rand() % 15000) + 6000;
		if(m_cameraAnimNode)
		{
			m_cameraNode->removeAnimator(m_cameraAnimNode);
			m_cameraAnimNode->drop();
			m_cameraAnimNode = 0;
		}
		if(m_camMode == CAM_RANDOM || m_camMode == CAM_RANDOM_NEAR || m_camMode == CAM_DEMORIDER)
		{
			float rot = (1.0f + float(rand() % 60)/30) * ((rand() % 100) > 50 ? -0.05f : 0.05f);
			if( m_camMode == CAM_DEMORIDER)
				rot = (3.5f + float(rand() % 90)/30) * ((rand() % 100) > 50 ? -0.05f : 0.05f);
			m_cameraAnimNode =  getSceneManager()->createRotationAnimator(core::vector3df(0, rot, 0));
			if(m_cameraAnimNode)
			{
				m_cameraNode->addAnimator(m_cameraAnimNode);
				if( m_camMode != CAM_DEMORIDER)
					m_cameraAnimNode->animateNode(m_cameraNode, D3DBase::GetDevice()->getTimer()->getTime()+(rand() % 1000)+500);

			}
			{
				float factor = float(rand() % 10) / 10;
				float fov = angleFOV;
				if(m_camMode == CAM_RANDOM_NEAR)
					fov = float( (angleFOV/2 - angleFOV/4) * factor) + angleFOV/4;
				else
					fov = float( (angleFOV - angleFOV/2) * factor) + angleFOV/2;

				m_camera[CAMTYPE_NORMAL]->setFOV(degToRad(fov));
			}
		}
		else
		{
			float fov = angleFOV;
			m_camera[CAMTYPE_NORMAL]->setFOV(degToRad(fov));
		}
	}
	m_cameraNode2->updateAbsolutePosition();
	m_cameraNode->updateAbsolutePosition();
	m_camera[m_camtype]->updateAbsolutePosition();

	
	
}

void Rider::SetShadow()
{
	f32 fnum = getFrameNr() * sframes / 24;
	vector3df vrot;
	int w = tw/inc_w, h = th/inc_h;
	vrot = getRotation();
	while(vrot.y < 0)
		vrot.y += 360.0f;
	while(vrot.y > 360.0f)
		vrot.y -= 360.0f;

	float idx = vrot.y / inc_rot;
	float roty = fmod(vrot.y, inc_rot);

	f32 ftw = f32((int)idx % w)/w; 
	f32 fth = f32((int)idx / w)/h;
	
	m_modelShadow->setScale(sscale);
	m_modelShadow->setRotation(vector3df(0,-vrot.y + roty + sroty,0));
	m_modelShadow->updateAbsolutePosition();
	m_modelShadow->setMaterialTexture(0, riderShadows[(int)fnum]);
	m_modelShadow->setUVRect(rect<f32>(ftw,fth,ftw+1.0f/w,fth+1.0f/h));
}

void Rider::UpdatePosition()
{
	if (!isVisible())  
		return;

	bool bProcess = true;
	if(!bDemoRider)
	{
		switch(gamemode)
		{
		default:
		case GAME_LEAD:
		case GAME_NORMAL:
			switch(state)
			{
			case START_3:
			case START_2:
			case START_1:
			default:
				bProcess = false;
				break;

			case PRESTART:
			case START_0: // Racing (Race Started)
			case RACE:
				break;
			}
			break;
		}
	}
	if(bProcess)
	{
		if (!(m_bReal && !demomode))
		{
			if(0 == (gFTime.simTime/(((rand() % numriders*2) + 2) * 10) % 3))
				m_newspeed = float(rand() % (maxspeed - 10)) + 10;
			m_speed = MoveTo(m_speed, m_newspeed, gFTime.frameSec * 5);
			
			float speed = 0;
			if(m_state == RACE)
				speed = m_speed;

			m_deltaspeed = (speed * speedFactor * gFTime.frameSec);
			if (m_deltaspeed > 1.0f)  
			{
				m_deltaspeed = 1.0f;
			}
			else if (m_deltaspeed < 0.0f)  
			{
				m_deltaspeed = 0.0f;
			}
			m_rdist += m_deltaspeed;
		}
		m_dist = (f32)m_distfilter.calc(m_rdist, gFTime.frameTime);
		m_velForw = (f32)(m_dist - m_distfilter.getoldval()) / gFTime.frameSec;
		m_velSide = 0;

		int thisRider = getID();
		// handles moving from lane to lane
		if(m_dist > 2.5f * distCam)
		{
			int left = 0;
			int right = MAX_RIDERS-1;
			// handles moving from lane to lane
			if(m_dist > 2.5f * distCam)
			{
				// decide when to disable passing 
				int targetLane = targetLanes[getID()]; 
				// Move view camera target to the lead rider
				//if(targetLane != m_targetLane)
				{   
					m_prevLane = m_lane;
					m_targetLane = targetLane;
					m_prevxpos = m_xpos;
				}
				f32 cur = m_xpos;
				f32 curTarget = riderLanes[m_targetLane];
				f32 newTarget;
				f32 delta;

				// checking if rider is in the target xpos
				f32 diffX = curTarget - cur;
/*
				// if not, try to move it there
				if(diffX != 0)
				{
*/
					// assuming no collision, move the rider normally to target xpos
					float passVal = (passFactor * m_velForw) * gFTime.frameSec;
					if(fabs(diffX) < passVal)
						passVal = fabs(diffX);

					newTarget = cur + (diffX < 0 ? -passVal : passVal);

					f32 distColl = (bikeLength * collFactor);
					f32 distCollx = 0.3f;
					bool canPush = false;
					// actual collision adjustments here
					for (int i=0; i < numRiders; i++)  
					{
						int iRider = orderDist[i];
						// same rider
						if(iRider == thisRider)
						{
							canPush = true;
							continue;
						}

						Rider *pRider = D3DBase::rider[iRider];
						f64 zDist = pRider->GetDist();
						float deltaz = float(zDist - m_dist);
						bool bBehind = deltaz > 0 ? true : false;
						deltaz = fabs(deltaz);
						// if in possible collision
						if(deltaz < distColl)
						{
							distCollx = min(0.9f,distColl-deltaz)/3;
							// compare x locations
							float deltax = fabs(pRider->m_xpos - newTarget);
							// if in actual collision
							if(deltax < distCollx)
							{
								deltax = distCollx - deltax;
								if(canPush)
								{
									if(pRider->m_xpos < newTarget)
										pRider->m_xpos -= deltax;
									else
										pRider->m_xpos += deltax;
								}
								else
								{
									if(pRider->m_xpos < newTarget)
										newTarget += deltax;
									else
										newTarget -= deltax;
								}
							}
						}
					}
/*
				}
				else // if yes, then set the variables to match
				{
					if(m_targetLane != m_lane)
					{
						m_prevLane = m_lane = m_targetLane;
					}
					newTarget = riderLanes[m_lane];
				}
*/
				// adjust actual rider position
				delta = (newTarget - cur);
				m_xpos += delta;

				// calculate rider's actual side velocity
				m_velSide = delta / gFTime.frameSec;

				// Figure out what lane this rider is in, based on new xpos
				if(curTarget != m_xpos)
				{
					if(curTarget < m_xpos)
					{
						for(int n = 0; n < MAX_RIDERS; n++)
						{
							if(m_xpos <= riderLanes[n] + 0.2f)
							{
								m_lane = n;
								break;
							}
						}
					}
					else
					{
						for(int n = MAX_RIDERS - 1; n >= 0; n--)
						{
							if(m_xpos >= (riderLanes[n] - 0.2f))
							{
								m_lane = n;
								break;
							}
						}
					}
				}
			}
			else
			{
				m_desiredLane = m_targetLane = m_prevLane = m_lane = startLane[getID()];
			}
		}
	}
}

void Rider::Update()
{
	vector3df roadpos,roadvec, vpos, vvec,vtarget, vrot, vrot2, cvpos, cvrot;
	f32 local_grade_d_100, grade_ahead_100, speed, roadgrade, secrot, wind, yaw, xpos, yawAhead;
	int hint=0;
	bool bProcess = true;

	if (!isVisible())  
		return;

	int thisRider = getID();

	if(angleFOV != lastAngleFOV)
	{
		m_camera[CAMTYPE_NORMAL]->setFOV(degToRad(angleFOV));
		m_camera[CAMTYPE_FIXED]->setFOV(degToRad(angleFOV));
	}
	if(distCam != lastn)
	{
		m_camera[CAMTYPE_NORMAL]->setPosition(vector3df(0,0,-distCamM/2));
		m_initstart = false;
	}

	if(bDemoRider)
	{
		m_state = DEMORIDER;
		if((getID() == 0) && !m_initstart)
		{
			m_cameraNode->setRotation(core::vector3df(0,0,0));
			m_cameraNode->setPosition(vector3df(0,camh+camh/3,0));
			m_cameraNode->updateAbsolutePosition();

			vector3df vpos, vvec, vtarget;
			int hint = 0;
			float yaw, secrot,wind, roadgrade;
			m_psec = NULL;
			m_psec = gpCourse->RoadLoc(1.25f*distCam, vpos, vvec, roadgrade, hint, &yaw, &secrot, &wind, m_psec);
			m_lastPos = vpos;
			vpos = (vpos + vvec * 1.6f) * mult;
			vpos.y += camh * 2.5f;
			m_camtype = CAMTYPE_FIXED;
			m_camera[m_camtype]->setPosition(vpos);
			
			vtarget = getAbsolutePosition();
			m_camera[m_camtype]->setTarget(vector3df(vtarget.x,vtarget.y+camh/3,vtarget.z));

			m_initstart = true;
		}

		ShowShadow(!bDemoRider);
		if(getID() != 0)
		{
			setVisible(false);
			return;
		}
	}
	else
	{
		switch(gamemode)
		{
		default:
		case GAME_LEAD:
		case GAME_NORMAL:
			switch(state)
			{
			case PRESTART:
				if(((m_bReal && !demomode) || (getID() == 0)) && !m_initstart)
				{
					m_cameraNode->setRotation(core::vector3df(0,0,0));
					m_cameraNode->setPosition(vector3df(0,camh+camh/3,0));
					m_cameraNode->updateAbsolutePosition();

					vector3df vpos, vvec, vtarget;
					int hint = 0;
					float yaw, secrot,wind, roadgrade;
					m_psec = NULL;
					m_psec = gpCourse->RoadLoc(1.25f*distCam, vpos, vvec, roadgrade, hint, &yaw, &secrot, &wind, m_psec);
					m_lastPos = vpos;
					vpos = (vpos + vvec * 1.6f) * mult;
					vpos.y += camh * 2.5f;
					m_camtype = CAMTYPE_FIXED;
					m_camera[m_camtype]->setPosition(vpos);

					//vtarget = getAbsolutePosition();
					//m_camera[m_camtype]->setTarget(vector3df(vtarget.x,vtarget.y+camh/3,vtarget.z));

					m_initstart = true;
				}
				break;
			case START_3:
			case START_2:
			case START_1:
				bProcess = false;
				break;
			// Racing (Race Started)
			case START_0:
				break;
			case RACE:
				break;
			default:
				bProcess = false;
				break;
			}
			break;
		}
	}
	bool bStateChanged = false;
	vector3df prevvpos = getPosition();
	vector3df prevvrot = getRotation();
	if(m_oldstate != m_state && getID() == 0)
	{
		m_oldstate = m_state;
		bStateChanged = true;
		D3DBase::prevLeadRider = D3DBase::leadRider = 0;
		D3DBase::prevLeadTarget = D3DBase::leadTarget = prevvpos;
		holdChange = gFTime.simTime + 1500;
	}

	//float coursedist = gpCourse->CourseDistance();
	double coursedist = gpCourse->GetRaceLength();
	//double lapdist = gpCourse->GetCourseLength();
//  float endoftrack = gpCourse->GetEndOfTrack();
	if(bProcess)
	{
		//if(m_dist > coursedist + 90.0f)
		//  m_state = FINISH;
		if(bDemoRider)
		{
			m_camtype = CAMTYPE_NORMAL;
			m_speed = 24.0f;

			setAnimationSpeed(m_speed);
			if(getModel())
			{
				getModel()->animateJoints();
			}


			m_psec = gpCourse->RoadLoc((f32)(m_dist - bikeLength), vpos, vvec, roadgrade, hint, &m_yaw, &secrot, &wind, m_psec);
			m_lastPos = vpos;
			gpCourse->RoadLoc((f32)m_dist, roadpos, roadvec, local_grade_d_100, hint, &yaw, &secrot, &wind, m_psec);

			
			/*
			vector3df nv = roadpos - vpos;
			if(0.0f == nv.getLengthSQ())
			{
				nv = vector3df(0.0f,0.0f,1.0f);
			}
			nv.normalize();
			m_dirvec = nv;
			*/
			

			m_dirvecx = vvec;

			vrot = getRotation();
			vrot.y = radToDeg(m_yaw);
			setRotation(vrot);

			m_desiredLane = 0;
			m_targetLane = m_prevLane = m_lane = startLane[getID()];
			m_xpos = riderLanes[m_lane]; //riderStart[getID()];
			xpos = m_xpos;
			vpos += vvec * xpos;
			vpos *= mult;

			setPosition(vpos);
			updateAbsolutePosition();

			vrot2 = getModelNode()->getRotation();
			vrot2.z = (f32)m_leanfilter.calc(radToDeg(m_lean),gFTime.frameTime);
			getModelNode()->setRotation(vrot2);
			getModelNode()->updateAbsolutePosition();
		}
		else
		{
			switch(gamemode)
			{
			default:
			case GAME_LEAD:
			case GAME_NORMAL:

				f64 leadDist = m_dist;
				if(gamemode == GAME_LEAD)
				{
					leadDist = D3DBase::rider[D3DBase::leadRider]->GetDist();
				}

				int oldCamType = m_camtype;
				if(((m_bReal && !demomode) || (getID() == 0)) && leadDist < 2.5f * distCam)
					m_camtype = CAMTYPE_FIXED;
				else
					m_camtype = CAMTYPE_NORMAL;
				if(oldCamType != m_camtype && m_camtype == CAMTYPE_NORMAL)
					AnimateRiderCamera();

				/*
				if (!(m_bReal && !demomode))
				{
					if(0 == (gFTime.simTime/(((rand() % numriders*2) + 2) * 10) % 3))
						m_newspeed = float(rand() % (maxspeed - 10)) + 10;
					m_speed = MoveTo(m_speed, m_newspeed, gFTime.frameSec * 5);
				}
				*/

				//if(true || m_state == RACE)
				{
					if(!m_bReal)
						setAnimationSpeed(m_speed);
					
					if(getModel())
					{
						getModel()->animateJoints();

						core::vector3df v(radToDeg((float)m_dist),0,0);
						IBoneSceneNode *t = 0;
						switch(m_type)
						{
						case MODEL_MALE_ROAD:
						case MODEL_FEMALE_ROAD:
							t = getModel()->getJointNode("BIKE_Tire_RearO");
							if(t != 0)
								t->setRotation(v);
							t = getModel()->getJointNode("BIKE_Tire_FrontO");
							if(t != 0)
								t->setRotation(v);
							break;
						default:
							t = getModel()->getJointNode("BIKE_Tire_Rear");
							if(t != 0)
								t->setRotation(v);
							t = getModel()->getJointNode("BIKE_Tire_Front");
							if(t != 0)
								t->setRotation(v);
							break;
						}
					}
					//speed = m_speed;
				}
				/*
				else 
				{
					if(!m_bReal)
						setAnimationSpeed(0);
					speed = 0;
				}
				*/
				if(m_state == RACE)
					speed = m_speed;
				else
					speed = 0;

		
				/*
				f64 oldDist = m_dist;
				if(!(m_bReal && !demomode))
				{
					m_deltaspeed = (speed * speedFactor * gFTime.frameSec);
					if (m_deltaspeed > 1.0f)  
					{
						m_deltaspeed = 1.0f;
					}
					else if (m_deltaspeed < 0.0f)  
					{
						m_deltaspeed = 0.0f;
					}
					m_rdist += m_deltaspeed;
				}
				m_dist = (f32)m_distfilter.calc(m_rdist, gFTime.frameTime);
				*/

				f32 moreYaw = 0;
				int left = 0;
				int right = MAX_RIDERS-1;
				f64 oldDist = m_distfilter.getoldval();

				/*
				m_velForw = (f32)(m_dist - oldDist) / gFTime.frameSec;
				m_velSide = 0;
				*/
#if 0
				// handles moving from lane to lane
				if(m_dist > 2.5f * distCam)
				{
					int timeChange = 1500;
					vector3df roadpos, roadvec;
					int hint=0;

					// decide when to disable passing 
					int targetLane = targetLanes[getID()]; 
					/*
					int targetLane = m_targetLane;
					if(!gpCourse->IsLoopClosed() && !gpCourse->IsLooped() && (m_dist > endoftrack))
						targetLane = m_desiredLane; 
					else
						targetLane = targetLanes[getID()];
					*/

					// Move view camera target to the lead rider
					//if(targetLane != m_targetLane)
					{   
						m_prevLane = m_lane;
						m_targetLane = targetLane;
						m_prevxpos = m_xpos;
					}
					f32 cur = m_xpos;
					f32 curTarget = riderLanes[m_targetLane];
					f32 newTarget;
					f32 delta;

					// checking it rider is in the target xpos
					f32 diffX = curTarget - cur;
					// if not, try to move it there
					if(diffX != 0)
					{
						// assuming no collision, move the rider normally to target xpos
						float passVal = (passFactor * m_velForw) * gFTime.frameSec;
						if(fabs(diffX) < passVal)
							passVal = fabs(diffX);

						newTarget = cur + (diffX < 0 ? -passVal : passVal);

						f32 distColl = (bikeLength * collFactor);
						f32 distCollx = 0.4f;
						// actual collision adjustments here
						// Order the riders by lanes
						for (int i=0; i < numRiders; i++)  
						{
							int iRider = orderDist[i];
							// same rider
							if(iRider == thisRider)
								continue;
							Rider *pRider = D3DBase::rider[iRider];
							f64 zDist = pRider->GetDist();
							float deltaz = fabs(float(zDist - m_dist));
							// if in possible collision
							if(deltaz < distColl)
							{
								// compare x locations
								float deltax = fabs(pRider->m_xpos - newTarget);
								// if in actual collision
								if(deltax < distCollx)
								{
									if(pRider->m_xpos < newTarget)
										newTarget += deltax;
									else
										newTarget -= deltax;
								}
							}
						}

					}
					else // if yes, then set the variables to match
					{
						if(m_targetLane != m_lane)
						{
							m_prevLane = m_lane = m_targetLane;
						}
						newTarget = riderLanes[m_lane];
					}

					// adjust actual rider position
					delta = (newTarget - cur);
					m_xpos += delta;

					// calculate rider's actual side velocity
					m_velSide = delta / gFTime.frameSec;

					// Figure out what lane this rider is in, based on new xpos
					if(curTarget != m_xpos)
					{
						if(curTarget < m_xpos)
						{
							for(int n = 0; n < MAX_RIDERS; n++)
							{
								if(m_xpos <= riderLanes[n] + 0.2f)
								{
									m_lane = n;
									break;
								}
							}
						}
						else
						{
							for(int n = MAX_RIDERS - 1; n >= 0; n--)
							{
								if(m_xpos >= (riderLanes[n] - 0.2f))
								{
									m_lane = n;
									break;
								}
							}
						}
					}
				}
				else
				{
					m_desiredLane = m_targetLane = m_prevLane = m_lane = startLane[getID()];
				}
#endif

				m_psec = gpCourse->RoadLoc((f32)(m_dist - bikeLength), vpos, vvec, roadgrade, BIKE_HINTS, &m_yaw, &secrot, &wind, m_psec);
				m_lastPos = vpos;
				gpCourse->RoadLoc((f32)(m_dist + lookAhead), roadpos, roadvec, grade_ahead_100, BIKE_HINTS, &yawAhead, &secrot, &wind, m_psec);
				gpCourse->RoadLoc((f32)(m_dist), roadpos, roadvec, local_grade_d_100, BIKE_HINTS, &yaw, &secrot, &wind, m_psec);
				
				m_camDist = (f32)m_camdistfilter.calc(grade_ahead_100,gFTime.frameTime);

				if(m_velForw > 0 && m_velSide != 0)
				{
					moreYaw = atan2( m_velSide, m_velForw) / 2;
				}
				moreYaw = (f32)m_yawfilter.calc(radToDeg(moreYaw),gFTime.frameTime);


				f32 lean = 0.0f;
				f32 rdelta = AngleDiff(yawAhead, m_yaw);
				//if(m_collisionHold < gFTime.simTime)
				{
					if(D3DBase::bDraftingEnabled)
					{
						if(-rdelta >= 0)
							m_desiredLane = max(left,m_lane - 1); //left;
						else //if(-rdelta < 0)
							m_desiredLane = min(right,m_lane + 1); //right;
						//else 
						//  m_desiredLane = m_lane;
					}
					else
					{
						if(-rdelta > 0)
							m_desiredLane = max(left,m_lane - 1); //left;
						else if(-rdelta < 0)
							m_desiredLane = min(right,m_lane + 1); //right;
						else 
							m_desiredLane = m_lane;
					}
				}

				//rdelta = AngleDiff(m_yaw, yaw);
				rdelta = AngleDiff(yaw, m_yaw);
				f32 leanspeed = m_velForw - 3.0f;
				if (leanspeed < 0)
					leanspeed = 0;
				lean = -rdelta * leanspeed * leanFactor; //leanFactor * gFTime.frameSec;

				if (lean < -m_MaxLeanAngle)  
				{
					lean = -m_MaxLeanAngle;
				}
				else if (lean > m_MaxLeanAngle)  
				{
					lean = m_MaxLeanAngle;
				}
				m_lean = (f32)m_leanfilter.calc(radToDeg(lean),gFTime.frameTime);

				m_dirvecx = vvec;

				vrot = getRotation();
				// set rotate for turns
				vrot.y = radToDeg(m_yaw);
				vrot.x = -roadgrade * 45.0f;

				/*
				else
				{
					vrot.y += (360.0f * gFTime.frameTime) / 8000;
					if(vrot.y > 360)
						vrot.y -= 360;
				}
				*/

				setRotation(vrot);

				xpos = m_xpos; //riderLanes[startLane[getID()]];//riderStart[getID()];

				vpos += vvec * xpos;
				vpos *= mult;

				setPosition(vpos);
				updateAbsolutePosition();

				vrot2 = getModelNode()->getRotation();
				vrot2.y = moreYaw;
				vrot2.z = m_lean;
				getModelNode()->setRotation(vrot2);
				getModelNode()->updateAbsolutePosition();

				break;
			}
		}
	}
	else 
	{
		vpos = getPosition();
		vrot = getRotation();
		updateAbsolutePosition();
	}

	//m_psec->Show(true);

	if(!bDemoRider && m_modelShadow && m_modelShadow->isVisible() && m_state == RACE)
	{
		SetShadow();
	}

	/* disabled for now - not working correctly, probably not needed
	if(m_type == MODEL_MALE_CHROME || m_type == MODEL_FEMALE_CHROME || m_type == MODEL_MALE_GOLD || m_type == MODEL_FEMALE_GOLD)
	{
		if(m_model[m_type])
		{
			vector3df vrott;//, vrot = getRotation();
			float degyaw = radToDeg(m_yaw);
			while(degyaw < 0)
				degyaw += 360.0f;
			while(degyaw > 360.0f)
				degyaw -= 360.0f;
			for(int m = 0; m < (int)m_model[m_type]->getMaterialCount(); m ++)
			{
				vrott = m_model[m_type]->getMaterial(m).getTextureMatrix(1).getRotationDegrees();
				vrott.y = degyaw;
				m_model[m_type]->getMaterial(m).getTextureMatrix(1).setRotationDegrees( vrott );
				//m_model[m_type]->getMaterial(m).Shininess = 100.0f;
			}
		}
	}
	*/

	if(bDemoRider)
		m_pNumbers->setVisible(false);
	else
	{
		switch(gamemode)
		{
		default:
		case GAME_LEAD:
		case GAME_NORMAL:
			if(numriders < 3)
				m_pNumbers->setVisible(false);
			else
				m_pNumbers->setVisible(true);
			break;
		}
	}
}
void Rider::render()
{
	int i=0;
	if(IsVisible)
		i=1;
}

f32 Rider::getFrameNr()
{
	if (m_model[m_type])
	{
		return m_model[m_type]->getFrameNr();
	}
	return 0.0f;
}

void Rider::setAnimationSpeed(f32 framesPerSecond)
{
	if (m_model[m_type])
	{
		m_model[m_type]->setAnimationSpeed(framesPerSecond);
	}
}

void Rider::SetColor(RIDER_COLORS idx, SColor color)
{
	m_Colors[idx] = color.color;
	m_bColorChanged = true;
}

void Rider::setVisible(bool bVis)
{
	if (m_model[m_type])
	{
		m_model[m_type]->setVisible(bVis);
	}
	ISceneNode::setVisible(bVis);
}

void Rider::useRider(int x)
{
	if(x < 0) 
		x = 0;
	else if(x >= MAX_RIDER_TYPES)
		x = (MAX_RIDER_TYPES - 1);
	if (x != m_type && m_model[m_type])
	{
		m_model[m_type]->setVisible(false);
	}

	m_type = x;

	if(m_model[m_type])
	{
		m_model[m_type]->setVisible(isVisible());
	}

	if(bDemoRider)
	{
		SetCameraMode(CAM_DEMORIDER);
	}
	else
	{
#ifdef D3DEXE
		if(m_type == 0 || m_type >= MODEL_MALE_CHROME)
			m_rideMode = 2; // pacer
		else
			m_rideMode = 1; // live
#endif
	}



}

void Rider::ClearModel()
{   
	int i;
	for(i = 0; i < MAX_RIDER_TYPES; i++)
	{
		if(m_model[i])
		{
			m_model[i]->remove();
			m_model[i] = 0;
		}
	}
}
void Rider::preRender()
{   
	int i;
	for(i = 0; i < MAX_RIDER_TYPES; i++)
	{
		if(i != m_type && m_model[i])
		{
			m_model[i]->remove();
			m_model[i] = 0;
		}
	}

	if(!isVisible())
		return;

	if(m_model[m_type])
	{
		if(m_bColorChanged)
		{
			switch(m_type)
			{
			default:
				break;
			case MODEL_FEMALE: // female
			case MODEL_MALE: // male
			case MODEL_FEMALE_ROAD: // female
			case MODEL_MALE_ROAD: // male
				for(int j = 0; j < RIDER_COLORS_MAX; j++)
				{
					ChangeColor((RIDER_COLORS)j,m_Colors[j]);
				}
				break;
			}
			m_bColorChanged = false;
		}
		return;
	}

	i = m_type;
	m_model[i] = D3DBase::GetSceneManager()->addAnimatedMeshSceneNode(D3DBase::GetSceneManager()->getMesh(modelNames[i]),m_modelNode);
	if (m_model[i] != 0)
	{
		m_model[i]->setScale(core::vector3df(1.2f*mult,1.2f*mult,1.2f*mult));
		m_model[i]->setPosition(vector3df(0,0.6f+getID()*0.02f,0.25f*mult));
		m_model[i]->setAnimationSpeed(0);
		m_model[i]->setMaterialFlag(video::EMF_NORMALIZE_NORMALS, true);
		m_model[i]->setMaterialFlag(video::EMF_LIGHTING, true); // enable dynamic lighting
		m_model[i]->setMaterialFlag(video::EMF_FOG_ENABLE, fogOn);
		m_model[i]->setVisible(isVisible());
		// manually run the animation
		m_model[i]->setJointMode(irr::scene::EJUOR_CONTROL);
	}

	switch(i)
	{
	default:
		break;
	case MODEL_MALE_ROAD:
	case MODEL_FEMALE_ROAD:
	case MODEL_MALE: // male
	case MODEL_FEMALE: // female
		for(int j = 0; j < RIDER_COLORS_MAX; j++)
		{
			ChangeColor((RIDER_COLORS)j,m_Colors[j]);
		}
		break;
	case MODEL_MALE_GOLD: // gold
	case MODEL_FEMALE_GOLD: // gold
		ChangeColor(RIDER_SKIN,m_Colors[RIDER_SKIN],TEX_JERSEY+1);
		break;
	case MODEL_MALE_CHROME: // chrome
	case MODEL_FEMALE_CHROME: // chrome
		ChangeColor(RIDER_SKIN,m_Colors[RIDER_SKIN],TEX_JERSEY+0);
		break;
	case MODEL_X: // x
		break;
	}

}

void Rider::ChangeMatColor(int i, SColor clr, int itex)
{
	video::SMaterial *psmat = &m_model[m_type]->getMaterial(i);
	if(itex >= 0)
	{
		switch(m_type)
		{
		default:
			return;
		case MODEL_MALE:
		case MODEL_FEMALE:
		case MODEL_MALE_ROAD:
		case MODEL_FEMALE_ROAD:
			psmat->setTexture(0,D3DBase::GetDriver()->getTexture(modelTex[itex]));
			break;
		}
	}
	psmat->setFlag(video::EMF_COLOR_MATERIAL,false);
	psmat->DiffuseColor = clr.color;
	psmat->AmbientColor = clr.color;
}

void Rider::ChangeColor(RIDER_COLORS idx, SColor color, int itex)
{
	if(m_model[m_type] == 0)
	{
		return;
	}

	//video::SMaterial *psmat;
	switch(m_type)
	{
	default:
		return;
	case MODEL_MALE_CHROME: // chrome
	case MODEL_FEMALE_CHROME: // chrome
	case MODEL_MALE_GOLD: // gold
	case MODEL_FEMALE_GOLD: // gold
		if(idx == RIDER_SKIN)
		{
			m_model[m_type]->setMaterialTexture(0,D3DBase::GetDriver()->getTexture(modelTex[itex]));
			m_model[m_type]->setMaterialTexture(1,D3DBase::GetDriver()->getTexture("skydomemap"));
			
			m_model[m_type]->setMaterialType(video::EMT_REFLECTION_2_LAYER);

			m_model[m_type]->setMaterialFlag(video::EMF_NORMALIZE_NORMALS,true);
			m_model[m_type]->setMaterialFlag(video::EMF_LIGHTING,false);
			for(int m = 0; m < (int)m_model[m_type]->getMaterialCount(); m ++)
			{
				// < 1 is zoom, > 1 is unzoom
				m_model[m_type]->getMaterial(m).getTextureMatrix(1).setTextureScale(0.1f,0.3f);
				m_model[m_type]->getMaterial(m).getTextureMatrix(1).setRotationDegrees( irr::core::vector3d<f32>(0,0,180) );
				m_model[m_type]->getMaterial(m).Shininess = 40.0f;
			}
		}
		return;

	case MODEL_FEMALE_ROAD: // female
	case MODEL_FEMALE: // female
		break;
	case MODEL_MALE_ROAD: // male
	case MODEL_MALE: // male
		if(idx == RIDER_HAIR)
			return;
		break;
	}

	switch(idx)
	{
	default:
		break;
	case RIDER_BIKE:
		switch(m_type)
		{
		case MODEL_FEMALE:
			ChangeMatColor(0, color, itex);
			ChangeMatColor(6, color, itex);
			break;
		case MODEL_MALE:
			ChangeMatColor(0, color, itex);
			ChangeMatColor(6, color, itex);
			break;
		case MODEL_FEMALE_ROAD:
			ChangeMatColor(7, color, itex);
			ChangeMatColor(13, color, itex);
			break;
		case MODEL_MALE_ROAD:
			ChangeMatColor(5, color, itex);
			ChangeMatColor(11, color, itex);
			break;
		default:
			break;
		}
		break;
	case RIDER_TIRES:
		switch(m_type)
		{
		case MODEL_FEMALE:
			ChangeMatColor(5, color, itex);
			ChangeMatColor(7, color, itex);
			break;
		case MODEL_MALE:
			ChangeMatColor(5, color, itex);
			ChangeMatColor(7, color, itex);
			break;
		case MODEL_FEMALE_ROAD:
			ChangeMatColor(10, color, itex);
			ChangeMatColor(12, color, itex);
			break;
		case MODEL_MALE_ROAD:
			ChangeMatColor(12, color, itex);
			ChangeMatColor(14, color, itex);
			break;
		default:
			break;
		}
		break;
	case RIDER_JERSEY_TOP:
		switch(m_type)
		{
		case MODEL_FEMALE:
			ChangeMatColor(10, color, itex);
			break;
		case MODEL_MALE:
			ChangeMatColor(10, color, itex);
			break;
		case MODEL_FEMALE_ROAD:
			ChangeMatColor(2, color, itex);
			break;
		case MODEL_MALE_ROAD:
			ChangeMatColor(2, color, itex);
			break;
		default:
			break;
		}
		break;
	case RIDER_JERSEY_BOTTOM:
		switch(m_type)
		{
		case MODEL_FEMALE:
			ChangeMatColor(11, color, itex);
			break;
		case MODEL_MALE:
			ChangeMatColor(12, color, itex);
			break;
		case MODEL_FEMALE_ROAD:
			ChangeMatColor(3, color, itex);
			break;
		case MODEL_MALE_ROAD:
			ChangeMatColor(4, color, itex);
			break;
		default:
			break;
		}
		break;
	case RIDER_HAIR:
		switch(m_type)
		{
		case MODEL_FEMALE:
			ChangeMatColor(12, color, itex);
			break;
		case MODEL_MALE:
			//ChangeMatColor(7, color, itex);
			break;
		case MODEL_FEMALE_ROAD:
			ChangeMatColor(4, color, itex);
			break;
		case MODEL_MALE_ROAD:
			//ChangeMatColor(10, color, itex);
			break;
		default:
			break;
		}
		break;
	case RIDER_SHOES:
		switch(m_type)
		{
		case MODEL_FEMALE:
			ChangeMatColor(8, color, itex);
			break;
		case MODEL_MALE:
			ChangeMatColor(9, color, itex);
			break;
		case MODEL_FEMALE_ROAD:
			ChangeMatColor(0, color, itex);
			break;
		case MODEL_MALE_ROAD:
			ChangeMatColor(1, color, itex);
			break;
		default:
			break;
		}
		break;
	case RIDER_HELMET:
		switch(m_type)
		{
		case MODEL_FEMALE:
			ChangeMatColor(13, color, itex);
			break;
		case MODEL_MALE:
			ChangeMatColor(8, color, itex);
			break;
		case MODEL_FEMALE_ROAD:
			ChangeMatColor(5, color, itex);
			break;
		case MODEL_MALE_ROAD:
			ChangeMatColor(0, color, itex);
			break;
		default:
			break;
		}
		break;
	case RIDER_SKIN:
		switch(m_type)
		{
		case MODEL_FEMALE:
			ChangeMatColor(9, color, itex);
			break;
		case MODEL_MALE:
			ChangeMatColor(11, color, itex);
			break;
		case MODEL_FEMALE_ROAD:
			ChangeMatColor(1, color, itex);
			break;
		case MODEL_MALE_ROAD:
			ChangeMatColor(3, color, itex);
			break;
		default:
			break;
		}
		break;
	}
	return;
}

/*
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
*/

// MODEL_BOX,         2, TEX_JERSEY+0, TEX_JERSEY+0 
					
// MODEL_MALE,   13 materials, // 
//  TEX_FRAME+0,         // 0  Frame
//  TEX_FRAME+0,         // 1  Chain gear
//  TEX_FRAME+0,         // 2  Sprocket
//  TEX_FRAME+0,         // 3  Pedal left
//  TEX_FRAME+0,         // 4  Pedal right
//  TEX_FRAME+0,         // 5  Rear tire
//  TEX_FRAME+0,         // 6  Front fork
//  TEX_FRAME+0,         // 7  Front tire
//  TEX_ACCESS,          // 8  Helmet Glasses
//  TEX_ACCESS,          // 9  Shoes
//  TEX_CLOTHES+0,       // 10 Shirt
//  TEX_BODY+0,          // 11 Skin
//  TEX_CLOTHES+1,       // 12 Pants

// MODEL_FEMALE, 16 materials, // 
//  TEX_FRAME+0,         // 0  Frame
//  TEX_FRAME+0,         // 1  Chain gear
//  TEX_FRAME+0,         // 2  Sprocket
//  TEX_FRAME+0,         // 3  Pedal left
//  TEX_FRAME+0,         // 4  Pedal right
//  TEX_FRAME+0,         // 5  Rear tire
//  TEX_FRAME+0,         // 6  Front fork
//  TEX_FRAME+0,         // 7  Front tire
//  TEX_BODY+1,          // 8  Shoes
//  TEX_BODY+1,          // 9  Skin
//  TEX_CLOTHES+0,       // 10 Shirt
//  TEX_BODY+1,          // 11 Pants
//  TEX_BODY+1,          // 12 Hair
//  TEX_ACCESS,          // 13 Helmet
//  TEX_ACCESS,          // 14 Glasses

// MODEL_CHROME, 13 materials, // 
// MODEL_GOLD, 13 materials, // 
//  TEX_FRAME+0,         // 0  Frame
//  TEX_FRAME+0,         // 1  Sprocket
//  TEX_FRAME+0,         // 2  Pedal left
//  TEX_FRAME+0,         // 3  Pedal right
//  TEX_FRAME+0,         // 4  Rear tire
//  TEX_FRAME+0,         // 5  Front fork
//  TEX_FRAME+0,         // 6  Front tire
//  TEX_BODY+1,          // 7  Shoes
//  TEX_CLOTHES+0,       // 8  Shirt
//  TEX_BODY+1,          // 9  Skin
//  TEX_BODY+1,          // 10 Pants
//  TEX_FRAME+0,         // 11 Chain gear
//  TEX_FRAME+0          // 12 Chain

// MODEL_MALE_ROAD,      13 materials, // 
//  TEX_ACCESS,          // 0   Helmet Glasses
//  TEX_ACCESS,          // 1   Shoes
//  TEX_CLOTHES+0,       // 2   Shirt
//  TEX_BODY+0,          // 3   Skin
//  TEX_CLOTHES+1,       // 4   Pants
//  TEX_FRAME+0,         // 5   Frame
//  TEX_FRAME+0,         // 6   Chain gear
//  TEX_FRAME+0,         // 7   Sprocket
//  TEX_FRAME+0,         // 8   Pedal left
//  TEX_FRAME+0,         // 9   Pedal right
//  TEX_FRAME+0,         // 10  Rear tire
//  TEX_FRAME+0,         // 11  Front fork
//  TEX_FRAME+0,         // 12  Front tire

// MODEL_FEMALE_ROAD, 15 materials, // 
//  TEX_BODY+1,          // 0   Shoes
//  TEX_BODY+1,          // 1   Skin
//  TEX_CLOTHES+0,       // 2   Shirt
//  TEX_BODY+1,          // 3   Pants
//  TEX_BODY+1,          // 4   Hair
//  TEX_ACCESS,          // 5   Helmet
//  TEX_ACCESS,          // 6   Glasses
//  TEX_FRAME+0,         // 7   Frame
//  TEX_FRAME+0,         // 8   Chain gear
//  TEX_FRAME+0,         // 9   Sprocket
//  TEX_FRAME+0,         // 10  Pedal left
//  TEX_FRAME+0,         // 11  Pedal right
//  TEX_FRAME+0,         // 12  Rear tire
//  TEX_FRAME+0,         // 13  Front fork
//  TEX_FRAME+0,         // 14  Front tire
