// Terrain.h: interface for the Terrain class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_TERRAIN_H__25DDFA3B_33B1_439B_9EA9_3B2E55C4F4C0__INCLUDED_)
#define AFX_TERRAIN_H__25DDFA3B_33B1_439B_9EA9_3B2E55C4F4C0__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include <TList.h>
#include <CCourse.h>
#include <gl\glu.h>

#pragma warning(disable:4786)
#include <vector>
using namespace std;


class Terrain : public TList<Terrain>::Node  {

	public:
		static Terrain *ms_RenderList;
	protected:
		static TList<Terrain> ms_List;
		static TList<Terrain> ms_ReadyList;
		//static float ms_usize;
		//static float ms_vsize;
		static unsigned long ms_RID;


	public:

		float ms_usize;
		float ms_vsize;
		static HRESULT RenderAll();

		struct EdgeNode{
				struct	EdgeNode *next;	// Next Edge node in this list.
				struct	EdgeNode *pair;	// The other pair of this edge node.

				float start;	// Starting location.
				float dist;	// distance - (neg or pos)

				short idx;
				short count;	// negitive for an end node.
				int dir;
				int uid;
		};

		static bool DumpAll(const char *name);
		static void ClearAll();
		static bool FinishCutAll();
		static int CountAll();

		static void CloseAllModels();

		static Terrain * GetFirstTerrain()  {
			return ms_List.GetFirst();
		}

		static void StartRender()  {
			ms_RID++;
		}
		static void EndRender();

		void Show(bool bShow);
		ISceneNode *GetNode() {return terrainNode;}

		vector3df GetCenter() {return m_Center;}
		float GetRadius() {return m_Radius;}

		Terrain *m_pNextRender;
		bool m_bRender;

	protected:
		CCourse &m_Course;

		vector3df m_Min, m_Max;
		vector3df m_Center;
		float m_Radius;
		vector<COURSEVERTEX> m_V;
		vector<WORD> m_I;
		int m_Errors;

		unsigned long m_RID;

		GLUtesselator *m_ptess;

		COURSEVERTEX m_cv;

		enum {
			 MODE_NONE,
			 MODE_ENCLOSED,
			 MODE_CUT
		}											m_Mode;

		// The cutter.
		EdgeNode *m_pEdgeArr[4];
		EdgeNode *m_pStartEdge;
		int m_Cuts;
		vector3df m_LastV;

		int pushV(const COURSEVERTEX &cv);
		GLUtesselator *createTess();
		EdgeNode *addEdgeNode(const COURSEVERTEX &cv, int dir, bool left);

		void finishV();

		// Render information
		// ==================
		//LPDIRECT3DVERTEXBUFFER7 m_pVBuf;

		SMesh* m_pMesh;
		ISceneNode *terrainNode;
		bool bMeshCreated;

		DWORD m_VertCount;
		//LPWORD					m_pIBuf;
		DWORD m_IndexCount;


		// tlm20050506. microsoft's opengl header files need stdcall!

		static void __stdcall _beginCB(GLenum type, Terrain *caller);
		static void __stdcall _edgeFlagCB(GLboolean flag, Terrain *caller);
		static void __stdcall _vertexCB(unsigned int vertexIndex, Terrain *caller);
		static void __stdcall _endCB(Terrain *caller);
		static void __stdcall _combineCB(GLdouble coords[3], unsigned int vertexData[4], GLfloat weight[4], unsigned int *outData, Terrain *caller);
		static void __stdcall _errorCB(GLenum errno, Terrain *caller);


		/*
		static void _beginCB(GLenum type, Terrain *caller);
		static void _edgeFlagCB(GLboolean flag, Terrain *caller);
		static void _vertexCB(unsigned int vertexIndex, Terrain *caller);
		static void _endCB(Terrain *caller);
		static void _combineCB(GLdouble coords[3], unsigned int vertexData[4], GLfloat weight[4], unsigned int *outData, Terrain *caller);
		static void _errorCB(GLenum errno, Terrain *caller);
		*/

	public:
		Terrain::Terrain(CCourse &course, const vector3df &min, const vector3df &max);
		~Terrain();


		void AddRender()  {
			//if (!m_bRender)  {
				m_pNextRender = ms_RenderList;
				ms_RenderList = this;
				m_bRender = true;
			//}
		}
		static void ClearRenderList()  {
			ms_RenderList = NULL;
		}
		static void ClearRendered()  {
			Terrain *ptNext;
			for(Terrain *pt = ms_RenderList;pt;pt = ptNext)
			{
				ptNext = pt->m_pNextRender;
				pt->Show(false);
				pt->m_pNextRender = NULL;
				pt->m_bRender = false;

			}
			ms_RenderList = NULL;
		}

		bool InBox(const COURSEVERTEX &cv);

		// Used for standalone
		bool StartEnclosed();
		bool EndEnclosed();
		void BeginContour();
		void EndContour();
		void AddV(const COURSEVERTEX &cv);
		void AddBox();



		void InitCut();
		bool FinishCut();
		void StartCut(const COURSEVERTEX &cv, int dir, bool left);
		void AddCutV(const COURSEVERTEX &cv);
		void EndCut(const COURSEVERTEX &cv, int dir, bool left);

		COURSEVERTEX * GetCornerPoints()  {
			return &(m_V[0]);
		}

		bool ReadyModel(ISceneNode *groupNode);
		void CloseModel();
		HRESULT Render();

		bool InFrustum(ICameraSceneNode* cam);

		bool Dump(const char *name);
};

#endif // !defined(AFX_TERRAIN_H__25DDFA3B_33B1_439B_9EA9_3B2E55C4F4C0__INCLUDED_)
