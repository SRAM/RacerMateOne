// Copyright (C) 2002-2009 Nikolaus Gebhardt
// This file is part of the "Irrlicht Engine".
// For conditions of distribution and use, see copyright notice in irrlicht.h

#ifndef __C_CUSTOM_TRIANGLE_SELECTOR_H_INCLUDED__
#define __C_CUSTOM_TRIANGLE_SELECTOR_H_INCLUDED__

#include "ITriangleSelector.h"
#include "IMesh.h"
#include "irrArray.h"

namespace irr
{
namespace scene
{

class ISceneNode;
class IAnimatedMeshSceneNode;

//! Stupid triangle selector without optimization
class CCustomTriangleSelector : public ITriangleSelector
{
public:

	//! Constructs a selector based on a mesh
	CCustomTriangleSelector(ISceneNode* node);

	//! Constructs a selector based on a mesh
	CCustomTriangleSelector(const IMesh* mesh, ISceneNode* node);

	//! Constructs a selector based on an animated mesh scene node
	//!\param node An animated mesh scene node, which must have a valid mesh
	CCustomTriangleSelector(IAnimatedMeshSceneNode* node);

	//! Constructs a selector based on a bounding box
	CCustomTriangleSelector(const core::aabbox3d<f32>& box, ISceneNode* node);

	//! Gets all triangles.
	void getTriangles(core::triangle3df* triangles, s32 arraySize, s32& outTriangleCount, 
		const core::matrix4* transform=0) const;

	//! Gets all triangles which lie within a specific bounding box.
	void getTriangles(core::triangle3df* triangles, s32 arraySize, s32& outTriangleCount, 
		const core::aabbox3d<f32>& box, const core::matrix4* transform=0) const;

	//! Gets all triangles which have or may have contact with a 3d line.
	virtual void getTriangles(core::triangle3df* triangles, s32 arraySize,
		s32& outTriangleCount, const core::line3d<f32>& line, 
		const core::matrix4* transform=0) const;

	//! Returns amount of all available triangles in this selector
	virtual s32 getTriangleCount() const;

	virtual u32 getSelectorCount() const;

	virtual ITriangleSelector* getSelector(u32 index);

	//! Get TriangleSelector based on index based on getSelectorCount
	/** Only useful for MetaTriangleSelector, others return 'this' or 0
	*/
	virtual const ITriangleSelector* getSelector(u32 index) const;

	//! Return the scene node associated with a given triangle.
	virtual ISceneNode* getSceneNodeForTriangle(u32 triangleIndex) const { return SceneNode; }

protected:
	//! Create from a mesh
	virtual void createFromMesh(const IMesh* mesh); 

	//! Update when the mesh has changed
	virtual void updateFromMesh(const IMesh* mesh) const; 

	//! Update the triangle selector, which will only have an effect if it
	//! was built from an animated mesh and that mesh's frame has changed 
	//! since the last time it was updated.
	virtual void update(void) const;

	ISceneNode* SceneNode;
	mutable core::array<core::triangle3df> Triangles;

	IAnimatedMeshSceneNode* AnimatedNode;
	mutable s32 LastMeshFrame;
};

} // end namespace scene
} // end namespace irr


#endif

