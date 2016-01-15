// Copyright (C) 2002-2009 Nikolaus Gebhardt
// This file is part of the "Irrlicht Engine".
// For conditions of distribution and use, see copyright notice in irrlicht.h

#ifndef __C_SPRITE_SCENE_NODE_H_INCLUDED__
#define __C_SPRITE_SCENE_NODE_H_INCLUDED__

#include "IBillboardSceneNode.h"
#include "S3DVertex.h"

namespace irr
{
namespace scene
{

//! Scene node which is a billboard. A billboard is like a 3d sprite: A 2d element,
//! which always looks to the camera. 
class CSpriteSceneNode : virtual public IBillboardSceneNode
{
public:

	//! constructor
	CSpriteSceneNode(ISceneNode* parent=0, ISceneManager* mgr=NULL, s32 id=-1,
			const core::vector3df& position = const core::vector3df(0,0,0), 
			const core::dimension2d<f32>& size = core::dimension2d<f32>(1.0f,1.0f),
			const core::rect<f32>& uvRect = core::rect<f32>(0,0,1.0f,1.0f),
			video::SColor colorTop=video::SColor(0xFFFFFFFF), video::SColor colorBottom=video::SColor(0xFFFFFFFF),
			bool mode2D = false
			);

	//! pre render event
	virtual void OnRegisterSceneNode();

	//! render
	virtual void render();

	//! returns the axis aligned bounding box of this node
	virtual const core::aabbox3d<f32>& getBoundingBox() const;

	//! sets the size of the billboard
	virtual void setSize(const core::dimension2d<f32>& size);

	//! gets the size of the billboard
	virtual const core::dimension2d<f32>& getSize() const;

	virtual video::SMaterial& getMaterial(u32 i);
	
	//! returns amount of materials used by this scene node.
	virtual u32 getMaterialCount() const;
	
	//! Set the color of all vertices of the billboard
	//! \param overallColor: the color to set
	virtual void setColor(const video::SColor & overallColor);

	//! Set the color of the top and bottom vertices of the billboard
	//! \param topColor: the color to set the top vertices
	//! \param bottomColor: the color to set the bottom vertices
	virtual void setColor(const video::SColor & topColor, const video::SColor & bottomColor);

	//! Gets the color of the top and bottom vertices of the billboard
	//! \param[out] topColor: stores the color of the top vertices
	//! \param[out] bottomColor: stores the color of the bottom vertices
	virtual void getColor(video::SColor& topColor, video::SColor& bottomColor) const;

	//! Set the UVCoords of the vertices of the billboard
	//! \param uvRect: the UV Coords
	virtual void setUVRect(const core::rect<f32>& uvRect);

	//! Get the UVCoords of the vertices of the billboard
	//! \param[out] uvRect: stores the color of the top vertices
	virtual void getUVRect(core::rect<f32>& uvRect) const;

	//! Writes attributes of the scene node.
	virtual void serializeAttributes(io::IAttributes* out, io::SAttributeReadWriteOptions* options=0) const;

	//! Reads attributes of the scene node.
	virtual void deserializeAttributes(io::IAttributes* in, io::SAttributeReadWriteOptions* options=0);

	//! Returns type of the scene node
	virtual ESCENE_NODE_TYPE getType() const { return ESNT_BILLBOARD; }

	//! Creates a clone of this scene node and its children.
	virtual ISceneNode* clone(ISceneNode* newParent=0, ISceneManager* newManager=0);
	
private:

	core::dimension2d<f32> Size;
	core::aabbox3d<f32> BBox;
	video::SMaterial Material;

	video::S3DVertex vertices[4];
	u16 indices[6];

	core::rect<f32> UVRect;
	bool Mode2D;
};


} // end namespace scene
} // end namespace irr

#endif

