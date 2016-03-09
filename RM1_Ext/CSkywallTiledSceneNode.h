// Copyright (C) 2002-2009 Nikolaus Gebhardt
// This file is part of the "Irrlicht Engine".
// For conditions of distribution and use, see copyright notice in irrlicht.h
// Code for this scene node has been contributed by Anders la Cour-Harbo (alc)

#ifndef __C_SKY_WALL_TILED_SCENE_NODE_H_INCLUDED__
#define __C_SKY_WALL_TILED_SCENE_NODE_H_INCLUDED__

#include "ISceneNode.h"
#include "SMeshBuffer.h"

namespace irr
{
namespace scene
{

class CSkywallTiledSceneNode : public ISceneNode
{
	public:
		CSkywallTiledSceneNode(video::ITexture* texture, video::ITexture* floor, u32 horiRes, u32 vertRes,
			f32 texturePercentage, f32 spherePercentage, f32 radius, f32 tiled, f32 ypos,
			ISceneNode* parent, ISceneManager* smgr, s32 id);
		virtual ~CSkywallTiledSceneNode();
		virtual void OnRegisterSceneNode();
		virtual void render();
		virtual const core::aabbox3d<f32>& getBoundingBox() const;
		virtual video::SMaterial& getMaterial(u32 i);
		virtual u32 getMaterialCount() const;
		virtual ESCENE_NODE_TYPE getType() const { return ESNT_SKY_DOME; }

		virtual void serializeAttributes(io::IAttributes* out, io::SAttributeReadWriteOptions* options) const;
		virtual void deserializeAttributes(io::IAttributes* in, io::SAttributeReadWriteOptions* options);
		virtual ISceneNode* clone(ISceneNode* newParent=0, ISceneManager* newManager=0);

		SMeshBuffer* GetBuffer() {return Buffer;}

		//! Set the UVCoords of the vertices of the billboard
		//! \param uvRect: the UV Coords
		virtual void setUVRect(const core::rect<f32>& uvRect);

		//! Get the UVCoords of the vertices of the billboard
		//! \param[out] uvRect: stores the color of the top vertices
		virtual void getUVRect(core::rect<f32>& uvRect) const;

	private:

		void generateMesh();

		SMeshBuffer* Buffer;

		u32 HorizontalResolution, VerticalResolution;
		f32 TexturePercentage, SpherePercentage, Radius, Tiled, YPos;

		video::SMaterial Material;
		video::S3DVertex vertices[4];
		u16 indices[6];
		core::rect<f32> UVRect;
};


}
}

#endif

