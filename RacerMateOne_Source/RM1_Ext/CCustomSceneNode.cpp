// Copyright (C) 2002-2009 Nikolaus Gebhardt
// This file is part of the "Irrlicht Engine".
// For conditions of distribution and use, see copyright notice in irrlicht.h

#include "CCustomSceneNode.h"
#include "ISceneManager.h"

namespace irr
{
namespace scene
{

//! constructor
CCustomSceneNode::CCustomSceneNode(ISceneNode* parent, ISceneManager* mgr, s32 id)
: ISceneNode(parent, mgr, id)
{
	#ifdef _DEBUG
	setDebugName("CCustomSceneNode");
	#endif

	setAutomaticCulling(scene::EAC_OFF);
}


//! pre render event
void CCustomSceneNode::OnRegisterSceneNode()
{
	if (IsVisible)
		SceneManager->registerNodeForRendering(this);

	ISceneNode::OnRegisterSceneNode();
}


//! render
void CCustomSceneNode::render()
{
	// do nothing
}


//! returns the axis aligned bounding box of this node
const core::aabbox3d<f32>& CCustomSceneNode::getBoundingBox() const
{
	return Box;
}


//! Creates a clone of this scene node and its children.
ISceneNode* CCustomSceneNode::clone(ISceneNode* newParent, ISceneManager* newManager)
{
	if (!newParent)
		newParent = Parent;
	if (!newManager)
		newManager = SceneManager;

	CCustomSceneNode* nb = new CCustomSceneNode(newParent, 
		newManager, ID);

	nb->cloneMembers(this, newManager);
	nb->Box = Box;

	nb->drop();
	return nb;
}


} // end namespace scene
} // end namespace irr
