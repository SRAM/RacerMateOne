// Copyright (C) 2002-2009 Nikolaus Gebhardt
// This file is part of the "Irrlicht Engine".
// For conditions of distribution and use, see copyright notice in irrlicht.h

#include "CDecalSceneNode.h"
#include "IVideoDriver.h"
#include "ISceneManager.h"
#include "ICameraSceneNode.h"
//#include "os.h"

namespace irr
{
namespace scene
{

//! constructor
CDecalSceneNode::CDecalSceneNode(ISceneNode* parent, ISceneManager* smgr, s32 id,
			const core::vector3df& position, const core::dimension2d<f32>& size,
			const core::rect<f32>& uvRect,
			video::SColor colorTop, video::SColor colorBottom )
	: IBillboardSceneNode(parent, smgr, id, position), UVRect(uvRect)
{
	#ifdef _DEBUG
	setDebugName("CDecalSceneNode");
	#endif

	setSize(size);

	indices[0] = 2;
	indices[1] = 1;
	indices[2] = 0;
	indices[3] = 3;
	/*
	indices[0] = 0;
	indices[1] = 2;
	indices[2] = 1;
	indices[3] = 0;
	indices[4] = 3;
	indices[5] = 2;
	*/
	vertices[0].Color = colorBottom;
	vertices[1].Color = colorTop;
	vertices[2].Color = colorTop;
	vertices[3].Color = colorBottom;

	core::vector3df vposz(0,0,1),vposy(0,1,0),vposx(1,0,0);
	for (s32 i=0; i<4; ++i)
		vertices[i].Normal = vposy;

	core::vector3df vwidth = vposx * 0.5f * Size.Width;
	core::vector3df vlength = vposz * 0.5f * Size.Height;

	core::vector3df pos(0,0,0);
	/*
	vertices[0].Pos = pos + vwidth + vlength;
	vertices[1].Pos = pos + vwidth - vlength;
	vertices[2].Pos = pos - vwidth - vlength;
	vertices[3].Pos = pos - vwidth + vlength;
	*/
	
	vertices[0].Pos = pos + vlength + vwidth;
	vertices[1].Pos = pos + vlength - vwidth;
	vertices[2].Pos = pos - vlength - vwidth;
	vertices[3].Pos = pos - vlength + vwidth;
	/*
	vertices[0].Pos = pos + vwidth + vlength*0 ;
	vertices[1].Pos = pos - vwidth + vlength*0 ;
	vertices[2].Pos = pos - vwidth - vlength*2 ;
	vertices[3].Pos = pos + vwidth - vlength*2 ;
	*/
/*
	vertices[0].Pos = pos + vlength + vwidth*0;
	vertices[1].Pos = pos + vlength - vwidth*2;
	vertices[2].Pos = pos - vlength - vwidth*2;
	vertices[3].Pos = pos - vlength + vwidth*0;
*/
	setUVRect(uvRect);
}


//! pre render event
void CDecalSceneNode::OnRegisterSceneNode()
{
	if (IsVisible)
		SceneManager->registerNodeForRendering(this,ESNRP_SOLID);

	ISceneNode::OnRegisterSceneNode();
}


//! render
void CDecalSceneNode::render()
{
	video::IVideoDriver* driver = SceneManager->getVideoDriver();
	if (!driver)
		return;
/*
	core::vector3df vposz(0,0,-1),vposy(0,1,0),vposx(1,0,0);
	core::vector3df vwidth = vposx * 0.5f * Size.Width;
	core::vector3df vlength = vposz * 0.5f * Size.Height;

	core::vector3df pos = getAbsolutePosition();
	vertices[0].Pos = pos + vwidth + vlength;
	vertices[1].Pos = pos + vwidth - vlength;
	vertices[2].Pos = pos - vwidth - vlength;
	vertices[3].Pos = pos - vwidth + vlength;
	// draw
*/
	if ( DebugDataVisible & scene::EDS_BBOX )
	{
		driver->setTransform(video::ETS_WORLD, AbsoluteTransformation);
		video::SMaterial m;
		m.Lighting = false;
		driver->setMaterial(m);
		driver->draw3DBox(BBox, video::SColor(0,208,195,152));
	}

	driver->setTransform(video::ETS_WORLD, AbsoluteTransformation);
	//driver->setTransform(video::ETS_WORLD, core::IdentityMatrix);

	driver->setMaterial(Material);

	//driver->drawIndexedTriangleList(vertices, 4, indices, 2);
	driver->drawIndexedTriangleFan(vertices, 4, indices, 2);
}


//! returns the axis aligned bounding box of this node
const core::aabbox3d<f32>& CDecalSceneNode::getBoundingBox() const
{
	return BBox;
}


//! sets the size of the billboard
void CDecalSceneNode::setSize(const core::dimension2d<f32>& size)
{
	Size = size;

	if (Size.Width == 0.0f)
		Size.Width = 1.0f;

	if (Size.Height == 0.0f )
		Size.Height = 1.0f;

	f32 avg = (size.Width + size.Height)/6;
	BBox.MinEdge.set(-avg,-avg,-avg);
	BBox.MaxEdge.set(avg,avg,avg);
}


video::SMaterial& CDecalSceneNode::getMaterial(u32 i)
{
	return Material;
}


//! returns amount of materials used by this scene node.
u32 CDecalSceneNode::getMaterialCount() const
{
	return 1;
}


//! gets the size of the billboard
const core::dimension2d<f32>& CDecalSceneNode::getSize() const
{
	return Size;
}


//! Writes attributes of the scene node.
void CDecalSceneNode::serializeAttributes(io::IAttributes* out, io::SAttributeReadWriteOptions* options) const
{
	IBillboardSceneNode::serializeAttributes(out, options);

	out->addFloat("Width", Size.Width);
	out->addFloat("Height", Size.Height);
	out->addColor ("Shade_Top", vertices[1].Color );
	out->addColor ("Shade_Down", vertices[0].Color );
	out->addFloat ("Tex_Offx1", UVRect.LowerRightCorner.x );
	out->addFloat ("Tex_Offy1", UVRect.LowerRightCorner.y );
	out->addFloat ("Tex_Offx0", UVRect.UpperLeftCorner.x );
	out->addFloat ("Tex_Offy0", UVRect.UpperLeftCorner.y );
}


//! Reads attributes of the scene node.
void CDecalSceneNode::deserializeAttributes(io::IAttributes* in, io::SAttributeReadWriteOptions* options)
{
	IBillboardSceneNode::deserializeAttributes(in, options);

	Size.Width = in->getAttributeAsFloat("Width");
	Size.Height = in->getAttributeAsFloat("Height");
	vertices[1].Color = in->getAttributeAsColor ( "Shade_Top" );
	vertices[0].Color = in->getAttributeAsColor ( "Shade_Down" );
	UVRect.UpperLeftCorner.x = in->getAttributeAsFloat ( "Tex_Offx0" );
	UVRect.UpperLeftCorner.y = in->getAttributeAsFloat ( "Tex_Offy0" );
	UVRect.LowerRightCorner.x = in->getAttributeAsFloat ( "Tex_Offx1" );
	UVRect.LowerRightCorner.y = in->getAttributeAsFloat ( "Tex_Offy1" );

	setSize(Size);
	setUVRect(UVRect);
}


//! Set the color of all vertices of the billboard
//! \param overallColor: the color to set
void CDecalSceneNode::setColor(const video::SColor & overallColor)
{
	for(u32 vertex = 0; vertex < 4; ++vertex)
		vertices[vertex].Color = overallColor;
}


//! Set the color of the top and bottom vertices of the billboard
//! \param topColor: the color to set the top vertices
//! \param bottomColor: the color to set the bottom vertices
void CDecalSceneNode::setColor(const video::SColor & topColor, const video::SColor & bottomColor)
{
	vertices[0].Color = bottomColor;
	vertices[1].Color = topColor;
	vertices[2].Color = topColor;
	vertices[3].Color = bottomColor;
}


//! Gets the color of the top and bottom vertices of the billboard
//! \param[out] topColor: stores the color of the top vertices
//! \param[out] bottomColor: stores the color of the bottom vertices
void CDecalSceneNode::getColor(video::SColor & topColor, video::SColor & bottomColor) const
{
	bottomColor = vertices[0].Color;
	topColor = vertices[1].Color;
}

//! Set the UVCoords of the vertices of the billboard
//! \param uvRect: the UV Coords
void CDecalSceneNode::setUVRect(const core::rect<f32>& uvRect)
{
	UVRect = uvRect;
	vertices[0].TCoords.set(UVRect.LowerRightCorner.x, UVRect.LowerRightCorner.y);
	vertices[1].TCoords.set(UVRect.LowerRightCorner.x, UVRect.UpperLeftCorner.y);
	vertices[2].TCoords.set(UVRect.UpperLeftCorner.x, UVRect.UpperLeftCorner.y);
	vertices[3].TCoords.set(UVRect.UpperLeftCorner.x, UVRect.LowerRightCorner.y);
}

//! Get the UVCoords of the vertices of the billboard
//! \param[out] uvRect: stores the color of the top vertices
void CDecalSceneNode::getUVRect(core::rect<f32>& uvRect) const
{
	uvRect = UVRect;
}

//! Creates a clone of this scene node and its children.
ISceneNode* CDecalSceneNode::clone(ISceneNode* newParent, ISceneManager* newManager)
{
	if (!newParent)
		newParent = Parent;
	if (!newManager)
		newManager = SceneManager;


	CDecalSceneNode* nb = new CDecalSceneNode(newParent, 
		newManager, ID, RelativeTranslation, Size, UVRect);

	nb->cloneMembers(this, newManager);
	nb->Material = Material;

	nb->drop();
	return nb;
}


} // end namespace scene
} // end namespace irr

