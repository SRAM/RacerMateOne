// Copyright (C) 2002-2009 Nikolaus Gebhardt
// This file is part of the "Irrlicht Engine".
// For conditions of distribution and use, see copyright notice in irrlicht.h

#ifndef __C_CUSTOM_SCENE_NODE_H_INCLUDED__
#define __C_CUSTOM_SCENE_NODE_H_INCLUDED__

extern bool bSceneNodeUpdated;

#include "ISceneNode.h"

namespace irr
{
namespace scene
{

	class CCustomSceneNode : public ISceneNode
	{
	public:

		//! constructor
		CCustomSceneNode(ISceneNode* parent, ISceneManager* mgr, s32 id=-1);

		//! recalculates the bounding box
		void recalculateBoundingBox()
		{
			Box.reset(0.0f, 0.0f, 0.0f);

			ISceneNodeList::Iterator it = Children.begin();
			for (; it != Children.end(); ++it)
				Box.addInternalBox((*it)->getBoundingBox());
		}

		//! returns the axis aligned bounding box of this node
		virtual const core::aabbox3d<f32>& getBoundingBox() const;

		//! This method is called just before the rendering process of the whole scene.
		virtual void OnRegisterSceneNode();

		virtual void OnAnimate(u32 timeMs)
		{
			if (IsVisible && !bSceneNodeUpdated)
			{
				// animate this node with all animators

				ISceneNodeAnimatorList::Iterator ait = Animators.begin();
				while (ait != Animators.end())
					{
					// continue to the next node before calling animateNode()
					// so that the animator may remove itself from the scene
					// node without the iterator becoming invalid
					ISceneNodeAnimator* anim = *ait;
					++ait;
					anim->animateNode(this, timeMs);
				}

				// update absolute position
				updateAbsolutePosition();

				// perform the post render process on all children

				ISceneNodeList::Iterator it = Children.begin();
				for (; it != Children.end(); ++it)
					(*it)->OnAnimate(timeMs);
			}
		}

		//! does nothing.
		virtual void render();

		//! Returns type of the scene node
		virtual ESCENE_NODE_TYPE getType() const { return ESNT_EMPTY; }

		//! Creates a clone of this scene node and its children.
		virtual ISceneNode* clone(ISceneNode* newParent=0, ISceneManager* newManager=0);

	private:

		core::aabbox3d<f32> Box;
	};

} // end namespace scene
} // end namespace irr

#endif

