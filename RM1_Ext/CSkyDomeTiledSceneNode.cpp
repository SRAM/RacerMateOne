// Copyright (C) 2002-2009 Nikolaus Gebhardt
// This file is part of the "Irrlicht Engine".
// For conditions of distribution and use, see copyright notice in irrlicht.h
// Code for this scene node has been contributed by Anders la Cour-Harbo (alc)

#include "CSkyDomeTiledSceneNode.h"
#include "IVideoDriver.h"
#include "ISceneManager.h"
#include "ICameraSceneNode.h"
#include "IAnimatedMesh.h"
//#include "os.h"

namespace irr
{
namespace scene
{

/* horiRes and vertRes:
	Controls the number of faces along the horizontal axis (30 is a good value)
	and the number of faces along the vertical axis (8 is a good value).

	texturePercentage:
	Only the top texturePercentage of the image is used, e.g. 0.8 uses the top 80% of the image,
	1.0 uses the entire image. This is useful as some landscape images have a small banner
	at the bottom that you don't want.

	spherePercentage:
	This controls how far around the sphere the sky dome goes. For value 1.0 you get exactly the upper
	hemisphere, for 1.1 you get slightly more, and for 2.0 you get a full sphere. It is sometimes useful
	to use a value slightly bigger than 1 to avoid a gap between some ground place and the sky. This
	parameters stretches the image to fit the chosen "sphere-size". */

CSkyDomeTiledSceneNode::CSkyDomeTiledSceneNode(video::ITexture* sky, video::ITexture* floor, u32 horiRes, u32 vertRes,
		f32 texturePercentage, f32 spherePercentage, f32 radius, f32 tiled, f32 ypos,
		ISceneNode* parent, ISceneManager* mgr, s32 id)
	: ISceneNode(parent, mgr, id), Buffer(0), 
	  HorizontalResolution(horiRes), VerticalResolution(vertRes),
	  TexturePercentage(texturePercentage),
	  SpherePercentage(spherePercentage), Radius(radius), Tiled(tiled), YPos(ypos)
{
	#ifdef _DEBUG
	setDebugName("CSkyDomeTiledSceneNode");
	#endif

	setAutomaticCulling(scene::EAC_OFF);

	Buffer = new SMeshBuffer();
	Buffer->Material.Lighting = false;
	Buffer->Material.ZBuffer = video::ECFN_NEVER;
	Buffer->Material.ZWriteEnable = false;
	Buffer->Material.AntiAliasing = video::EAAM_OFF;
	Buffer->Material.setTexture(0, sky);
	if(Tiled != 1.0f)
		Buffer->Material.setFlag(video::EMF_TEXTURE_WRAP,video::ETC_REPEAT);
	Buffer->BoundingBox.MaxEdge.set(0,0,0);
	Buffer->BoundingBox.MinEdge.set(0,0,0);

	Buffer->Vertices.clear();
	Buffer->Indices.clear();

	Material.Lighting = false;
	Material.ZBuffer = video::ECFN_NEVER;
	Material.ZWriteEnable = false;
	Material.AntiAliasing = video::EAAM_OFF;
	Material.setTexture(0, floor);
	Material.setFlag(video::EMF_TEXTURE_WRAP,video::ETC_REPEAT);

	// regenerate the mesh
	generateMesh();
}


CSkyDomeTiledSceneNode::~CSkyDomeTiledSceneNode()
{
	if (Buffer)
		Buffer->drop();
}


void CSkyDomeTiledSceneNode::generateMesh()
{
	f32 azimuth;
	u32 k;

	const f32 azimuth_step = (core::PI * 2.f) / HorizontalResolution;
	if (SpherePercentage < 0.f)
		SpherePercentage = -SpherePercentage;
	if (SpherePercentage > 2.f)
		SpherePercentage = 2.f;
	const f32 elevation_step = SpherePercentage * core::HALF_PI / (f32)VerticalResolution;

	Buffer->Vertices.reallocate( (HorizontalResolution + 1) * (VerticalResolution + 1) );
	Buffer->Indices.reallocate(3 * (2*VerticalResolution - 1) * HorizontalResolution);

	video::S3DVertex vtx;
	vtx.Color.set(255,255,255,255);
	vtx.Normal.set(0.0f,-1.f,0.0f);

	const f32 tcV = TexturePercentage / VerticalResolution;
	for (k = 0, azimuth = 0; k <= HorizontalResolution; ++k)
	{
		f32 elevation = core::HALF_PI;
		const f32 tcU = (f32)(k * Tiled)/ (f32)HorizontalResolution;
		const f32 sinA = sinf(azimuth);
		const f32 cosA = cosf(azimuth);
		for (u32 j = 0; j <= VerticalResolution; ++j)
		{
			const f32 cosEr = Radius * cosf(elevation);
			vtx.Pos.set(cosEr*sinA, Radius*sinf(elevation), cosEr*cosA);
			vtx.TCoords.set(tcU, j*tcV);

			vtx.Normal = -vtx.Pos;
			vtx.Normal.normalize();

			Buffer->Vertices.push_back(vtx);
			elevation -= elevation_step;
		}
		azimuth += azimuth_step;
	}

	for (k = 0; k < HorizontalResolution; ++k)
	{
		Buffer->Indices.push_back(VerticalResolution + 2 + (VerticalResolution + 1)*k);
		Buffer->Indices.push_back(1 + (VerticalResolution + 1)*k);
		Buffer->Indices.push_back(0 + (VerticalResolution + 1)*k);

		for (u32 j = 1; j < VerticalResolution; ++j)
		{
			Buffer->Indices.push_back(VerticalResolution + 2 + (VerticalResolution + 1)*k + j);
			Buffer->Indices.push_back(1 + (VerticalResolution + 1)*k + j);
			Buffer->Indices.push_back(0 + (VerticalResolution + 1)*k + j);

			Buffer->Indices.push_back(VerticalResolution + 1 + (VerticalResolution + 1)*k + j);
			Buffer->Indices.push_back(VerticalResolution + 2 + (VerticalResolution + 1)*k + j);
			Buffer->Indices.push_back(0 + (VerticalResolution + 1)*k + j);
		}
	}
	Buffer->setHardwareMappingHint(scene::EHM_STATIC);

	// Build the floor using floor texture.
	indices[0] = 2;
	indices[1] = 1;
	indices[2] = 0;
	indices[3] = 3;
	vertices[0].Color.set(255,255,255,255);;
	vertices[1].Color.set(255,255,255,255);;
	vertices[2].Color.set(255,255,255,255);;
	vertices[3].Color.set(255,255,255,255);;
	core::vector3df vposz(0,0,1),vposy(0,1,0),vposx(1,0,0);
	for (s32 i=0; i<4; ++i)
		vertices[i].Normal = vposy;
	core::vector3df vwidth = vposx * Radius;
	core::vector3df vlength = vposz * Radius;
	
	core::vector3df pos(0,vtx.Pos.y*0.98f,0);
	core::rect<f32> r(0,0,32,32);

	vertices[0].Pos = pos + vlength + vwidth*1;
	vertices[1].Pos = pos + vlength - vwidth*1;
	vertices[2].Pos = pos - vlength - vwidth*1;
	vertices[3].Pos = pos - vlength + vwidth*1;
	setUVRect(r);
}


//! renders the node.
void CSkyDomeTiledSceneNode::render()
{
	video::IVideoDriver* driver = SceneManager->getVideoDriver();
	scene::ICameraSceneNode* camera = SceneManager->getActiveCamera();
	
	if (!camera || !driver)
		return;

	float farvalue = camera->getFarValue();
	camera->setFarValue(11*farvalue);
	camera->render();

	if ( !camera->isOrthogonal() )
	{
		core::matrix4 mat(AbsoluteTransformation);
		core::vector3df pos = camera->getAbsolutePosition();
//		pos.y /= 100; // Leave the dome in the same height location
//		pos.y += YPos;
		mat.setTranslation(pos);

		driver->setTransform(video::ETS_WORLD, mat);

		driver->setMaterial(Buffer->Material);
		driver->drawMeshBuffer(Buffer);

		driver->setMaterial(Material);
		driver->drawIndexedTriangleFan(vertices, 4, indices, 2);
	}

	// for debug purposes only:
	if ( DebugDataVisible )
	{
		video::SMaterial m;
		m.Lighting = false;
		driver->setMaterial(m);

		if ( DebugDataVisible & scene::EDS_NORMALS )
		{
			IAnimatedMesh * arrow = SceneManager->addArrowMesh (
					"__debugnormal2", 0xFFECEC00,
					0xFF999900, 4, 8, 1.f * 40.f, 0.6f * 40.f, 0.05f * 40.f, 0.3f * 40.f);
			if ( 0 == arrow )
			{
				arrow = SceneManager->getMesh ( "__debugnormal2" );
			}
			IMesh *mesh = arrow->getMesh(0);

			// find a good scaling factor
			core::matrix4 m2;

			// draw normals
			const scene::IMeshBuffer* mb = Buffer;
			const u32 vSize = video::getVertexPitchFromType(mb->getVertexType());
			const video::S3DVertex* v = ( const video::S3DVertex*)mb->getVertices();
			for ( u32 i=0; i != mb->getVertexCount(); ++i )
			{
				// align to v->Normal
				core::quaternion quatRot(v->Normal.X, 0.f, -v->Normal.X, 1+v->Normal.Y);
				quatRot.normalize();
				quatRot.getMatrix(m2, v->Pos);

				m2 = AbsoluteTransformation * m2;

				driver->setTransform(video::ETS_WORLD, m2);
				for (u32 a = 0; a != mesh->getMeshBufferCount(); ++a)
					driver->drawMeshBuffer(mesh->getMeshBuffer(a));

				v = (const video::S3DVertex*) ( (u8*) v + vSize );
			}
			driver->setTransform(video::ETS_WORLD, AbsoluteTransformation);
		}

		// show mesh
		if ( DebugDataVisible & scene::EDS_MESH_WIRE_OVERLAY )
		{
			m.Wireframe = true;
			driver->setMaterial(m);

			driver->drawMeshBuffer(Buffer);
		}
	}
	camera->setFarValue(farvalue);
	camera->render();
}


//! returns the axis aligned bounding box of this node
const core::aabbox3d<f32>& CSkyDomeTiledSceneNode::getBoundingBox() const
{
	return Buffer->BoundingBox;
}


void CSkyDomeTiledSceneNode::OnRegisterSceneNode()
{
	if (IsVisible)
	{
		SceneManager->registerNodeForRendering(this, ESNRP_SKY_BOX );
	}

	ISceneNode::OnRegisterSceneNode();
}


//! returns the material based on the zero based index i. To get the amount
//! of materials used by this scene node, use getMaterialCount().
//! This function is needed for inserting the node into the scene hirachy on a
//! optimal position for minimizing renderstate changes, but can also be used
//! to directly modify the material of a scene node.
video::SMaterial& CSkyDomeTiledSceneNode::getMaterial(u32 i)
{
	return Buffer->Material;
}


//! returns amount of materials used by this scene node.
u32 CSkyDomeTiledSceneNode::getMaterialCount() const
{
	return 1;
}


//! Writes attributes of the scene node.
void CSkyDomeTiledSceneNode::serializeAttributes(io::IAttributes* out, io::SAttributeReadWriteOptions* options) const
{
	ISceneNode::serializeAttributes(out, options);

	out->addInt  ("HorizontalResolution", HorizontalResolution);
	out->addInt  ("VerticalResolution",   VerticalResolution);
	out->addFloat("TexturePercentage",    TexturePercentage);
	out->addFloat("SpherePercentage",     SpherePercentage);
	out->addFloat("Radius",               Radius);
	out->addFloat("Tiled",                Tiled);
}


//! Reads attributes of the scene node.
void CSkyDomeTiledSceneNode::deserializeAttributes(io::IAttributes* in, io::SAttributeReadWriteOptions* options)
{
	HorizontalResolution = in->getAttributeAsInt  ("HorizontalResolution");
	VerticalResolution   = in->getAttributeAsInt  ("VerticalResolution");
	TexturePercentage    = in->getAttributeAsFloat("TexturePercentage");
	SpherePercentage     = in->getAttributeAsFloat("SpherePercentage");
	Radius               = in->getAttributeAsFloat("Radius");
	Tiled                = in->getAttributeAsFloat("Tiled");

	ISceneNode::deserializeAttributes(in, options);
	
	// regenerate the mesh
	generateMesh();
}

//! Creates a clone of this scene node and its children.
ISceneNode* CSkyDomeTiledSceneNode::clone(ISceneNode* newParent, ISceneManager* newManager)
{
	if (!newParent) 
		newParent = Parent;
	if (!newManager) 
		newManager = SceneManager;

	CSkyDomeTiledSceneNode* nb = new CSkyDomeTiledSceneNode(Buffer->Material.TextureLayer[0].Texture, Material.TextureLayer[0].Texture, HorizontalResolution, VerticalResolution, TexturePercentage, 
		SpherePercentage, Radius, Tiled, YPos, newParent, newManager, ID);

	nb->cloneMembers(this, newManager);
	
	nb->drop();
	return nb;
}

//! Set the UVCoords of the vertices of the billboard
//! \param uvRect: the UV Coords
void CSkyDomeTiledSceneNode::setUVRect(const core::rect<f32>& uvRect)
{
	UVRect = uvRect;
	vertices[0].TCoords.set(UVRect.LowerRightCorner.x, UVRect.LowerRightCorner.y);
	vertices[1].TCoords.set(UVRect.LowerRightCorner.x, UVRect.UpperLeftCorner.y);
	vertices[2].TCoords.set(UVRect.UpperLeftCorner.x, UVRect.UpperLeftCorner.y);
	vertices[3].TCoords.set(UVRect.UpperLeftCorner.x, UVRect.LowerRightCorner.y);
}

//! Get the UVCoords of the vertices of the billboard
//! \param[out] uvRect: stores the color of the top vertices
void CSkyDomeTiledSceneNode::getUVRect(core::rect<f32>& uvRect) const
{
	uvRect = UVRect;
}

} // namespace scene
} // namespace irr
