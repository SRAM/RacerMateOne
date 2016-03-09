
#include "stdafx.h"
#include "CCourse.h"
#include "Terrain.h"
#include <gl\glu.h>

class DVECTOR3{
	public:
		double x, y, z;
		DVECTOR3()  {
		}

		DVECTOR3(float _x, float _y, float _z) :
			x(_x),
			y(_y),
			z(_z)
		{
		}

		DVECTOR3(double _x, double _y, double _z) :
			x(_x),
			y(_y),
			z(_z)
		{
		}

		DVECTOR3(const vector3df &v) :
			x(v.x),
			y(v.y),
			z(v.z)
		{
		}

		operator double*()  {
			return &x;
		}
		operator const double*() const  {
			return &x;
		}
};

typedef void (__stdcall *GluTessCallbackType)();
//typedef void ( *GluTessCallbackType)();


Terrain *Terrain::ms_RenderList;
TList<Terrain> Terrain::ms_List;
TList<Terrain> Terrain::ms_ReadyList;

// xxx

//float Terrain::ms_usize = 10.0f;							// 20.0f, smaller = more repeats
//float Terrain::ms_vsize = 10.0f;							// 20.0f

unsigned long Terrain::ms_RID;

/****************************************************************************************

****************************************************************************************/

Terrain::Terrain(CCourse &course, const vector3df &min, const vector3df &max) :
	m_Course(course),
	m_Min(min),
	m_Max(max)
{
	SetData(this);
	m_bRender = true;

	//ms_usize = 10.0f;							// 20.0f, smaller = more repeats
	//ms_vsize = 10.0f;							// 20.0f, smaller = more repeats

	ms_usize = course.ms_usize;				// 20.0f, smaller = more repeats
	ms_vsize = course.ms_vsize;				// 20.0f, smaller = more repeats

	m_Course.m_TerrainList.AddTail(*this);

	m_Errors = 0;
	m_Mode = MODE_NONE;
	m_bRender = false;
	ms_List.AddHead(*this);
	m_pEdgeArr[0] = NULL;
	m_ptess = NULL;

#ifndef IGNOREFORNOW
	m_pVBuf = NULL;
	//m_pIBuf = NULL;
#endif
//	m_pMesh = NULL;
	terrainNode = 0;
	bMeshCreated = false;

	m_Center = (m_Max + m_Min) / 2.0f;
	//m_Radius = D3DXVec3Length(&(m_Max - m_Min));
	vector3df v = (m_Max - m_Min);
	m_Radius = v.getLengthSQ();
	m_RID = 0;
}

/****************************************************************************************

****************************************************************************************/

Terrain::~Terrain()  {
	if (m_ptess)
		gluDeleteTess(m_ptess);
	EndEnclosed();
	FinishCut();
	CloseModel();
}

/****************************************************************************************

****************************************************************************************/

void Terrain::ClearAll()  {
	Terrain *pt;
	while ((pt = ms_ReadyList.GetFirst()))
		delete pt;
	while ((pt = ms_List.GetFirst()))
		delete pt;
}


/****************************************************************************************

****************************************************************************************/

bool Terrain::FinishCutAll()  {
	Terrain *pt = ms_List.GetFirst();
	int cnt = 0;
	if (!pt)
		return false;

	for (; pt; pt = pt->Next())  {
		pt->m_Course.barAdd();
		pt->FinishCut();
	}

	return true;
}

/****************************************************************************************

****************************************************************************************/

int Terrain::CountAll()  {
	Terrain *pt = ms_List.GetFirst();
	int cnt = 0;
	for (;
		  pt;
		  pt = pt->Next())
		cnt++;
	return cnt;
}

/****************************************************************************************

****************************************************************************************/

void Terrain::CloseAllModels()  {
	Terrain *pt;
	while ((pt = ms_ReadyList.GetFirst()))
		pt->CloseModel();
}

/****************************************************************************************

****************************************************************************************/

HRESULT Terrain::RenderAll()  {
	HRESULT hr = 0;
	Terrain *pt;
	for (pt = ms_List.GetFirst();
		  pt;
		  pt = pt->Next())  {
		pt->Render();
	}

	/*
	for(pt = ms_RenderList;pt;pt = pt->m_pNextRender)
	{
		pt->m_bRender = false;
		hh = pt->Render();
		if (!hr && hh)
			hr = hh;
	}
	ms_RenderList = NULL;
	*/
	return hr;
}


/****************************************************************************************

****************************************************************************************/

void Terrain::EndRender()  {
	Terrain *pt;
	Terrain *ptnext;
	unsigned long rid = ms_RID - 10;
	for (pt = ms_ReadyList.GetFirst();
		  pt;
		  pt = pt->Next())  {
		if (pt->m_RID < rid)
			break;
	}
	while (pt)  {
		ptnext = pt->Next();
		pt->CloseModel();
		pt = ptnext;
	}
}

/****************************************************************************************

****************************************************************************************/

GLUtesselator * Terrain::createTess()  {
	GLUtesselator *ptess = gluNewTess();
	gluTessCallback(ptess, GLU_TESS_BEGIN_DATA, reinterpret_cast<GluTessCallbackType>(_beginCB));
	gluTessCallback(ptess, GLU_TESS_EDGE_FLAG_DATA,
						 reinterpret_cast<GluTessCallbackType>(_edgeFlagCB));
	gluTessCallback(ptess, GLU_TESS_VERTEX_DATA, reinterpret_cast<GluTessCallbackType>(_vertexCB));
	gluTessCallback(ptess, GLU_TESS_END_DATA, reinterpret_cast<GluTessCallbackType>(_endCB));
	gluTessCallback(ptess, GLU_TESS_COMBINE_DATA, reinterpret_cast<GluTessCallbackType>(_combineCB));
	gluTessCallback(ptess, GLU_TESS_ERROR_DATA, reinterpret_cast<GluTessCallbackType>(_errorCB));
	return ptess;
}

/****************************************************************************************

****************************************************************************************/

bool Terrain::StartEnclosed()  {
	EndEnclosed();
	FinishCut();
	CloseModel();
	m_ptess = createTess();
	if (!m_ptess)  {
		m_Errors = 1;
		return false;
	}

	m_cv.v.y = 0.0f;
	m_cv.color = 1.0f;
	m_cv.pdata = 0;


	m_Errors = 0;

	// begin polygon
	gluTessBeginPolygon(m_ptess, reinterpret_cast<void*>(this));

	m_V.clear();
	m_I.clear();

	m_Mode = MODE_ENCLOSED;
	return true;
}


/****************************************************************************************

****************************************************************************************/

bool Terrain::EndEnclosed()  {

	//tlm20060314+++
	/*
	if (m_ptess)  {
		gluDeleteTess(m_ptess);
		m_ptess = NULL;
	}

	if (m_Mode == MODE_ENCLOSED)  {
		m_Mode = MODE_NONE;
		gluTessEndPolygon(m_ptess);
	}
	*/

	if (m_Mode == MODE_ENCLOSED)  {
		m_Mode = MODE_NONE;
		gluTessEndPolygon(m_ptess);
	}

	if (m_ptess)  {
		gluDeleteTess(m_ptess);
		m_ptess = NULL;
	}


	//tlm20060314---


	finishV();
	return (m_Errors == 0);
}

/****************************************************************************************

****************************************************************************************/

void Terrain::AddBox()  {
	COURSEVERTEX cv;
	cv.v.y = 0.0f;
	cv.color = 1.0f;
	cv.pdata = 0;

	if (m_ptess)  {
		gluTessBeginContour(m_ptess);
		cv.v.x = m_Min.x;
		cv.v.z = m_Min.z;
		AddV(cv);
		cv.v.x = m_Max.x;
		AddV(cv);
		cv.v.z = m_Max.z;
		AddV(cv);
		cv.v.x = m_Min.x;
		AddV(cv);
		gluTessEndContour(m_ptess);
	}
}

/****************************************************************************************

****************************************************************************************/

void Terrain::BeginContour()  {
	if (m_ptess)
		gluTessBeginContour(m_ptess);
}

/****************************************************************************************

****************************************************************************************/

void Terrain::EndContour()  {
	if (m_ptess)
		gluTessEndContour(m_ptess);
}

/****************************************************************************************

****************************************************************************************/

void Terrain::AddV(const COURSEVERTEX &cv)  {
	if (m_ptess)  {
		int num = pushV(cv);
		gluTessVertex(m_ptess, DVECTOR3(cv.v.x, 0.0f, cv.v.z), (void *) (num));
	}
}

/****************************************************************************************

****************************************************************************************/

bool Terrain::InBox(const COURSEVERTEX &cv)  {
	return (((cv.v.x > m_Min.x) && (cv.v.x < m_Min.x)) && ((cv.v.x > m_Min.z) && (cv.v.x < m_Min.z)));
}



/****************************************************************************************

****************************************************************************************/

bool Terrain::Dump(const char *name)  {

	FILE *file = fopen(name, "w");

	fprintf(file,
			  "#\n"
			  "# Wavefront OBJ file\n"
			  "# Converted by the DEEP Exploration \n"
			  "# Right Hemisphere, LTD\n"
			  "# http://www.righthemisphere.com/\n"
			  "# \n"
			  "# object Object #3\n"
			  "g Terrain\n");
	int i;

	for (i = 0; i < (int)m_V.size(); i++)  {
		fprintf(file, "v %f %f %f\n", m_V[i].v.x, m_V[i].v.y, m_V[i].v.z);
	}

	fprintf(file, "# %d verticies\n", m_V.size());

	for (i = 0; i < (int)m_I.size(); i += 3)  {
		fprintf(file, "f %d %d %d\n", m_I[i + 0] + 1, m_I[i + 1] + 1, m_I[i + 2] + 1);
	}

	fprintf(file, "# %d verticies\n", m_V.size());

	fclose(file);

	return true;

}


/****************************************************************************************

****************************************************************************************/

bool Terrain::DumpAll(const char *name)  {
	FILE *file = fopen(name, "w");
	fprintf(file,
			  "#\n"
			  "# Wavefront OBJ file\n"
			  "# Converted by the DEEP Exploration \n"
			  "# Right Hemisphere, LTD\n"
			  "# http://www.righthemisphere.com/\n"
			  "# \n"
			  "# object Object #3\n");
	fprintf(file, "g Terrain\n");
	Terrain *pt;
	int add = 0;
	int i;
	for (pt = ms_List.GetFirst();
		  pt;
		  pt = pt->Next())  {
		for (i = 0;
			  i < (int)pt->m_V.size();
			  i++,add++)  {
			fprintf(file, "v %f %f %f\n", pt->m_V[i].v.x, pt->m_V[i].v.y, pt->m_V[i].v.z);
		}
	}
	fprintf(file, "# %d verticies\n", add);
	add = 0;
	for (pt = ms_List.GetFirst();
		  pt;
		  pt = pt->Next())  {
		for (i = 0;
			  i < (int)pt->m_I.size();
			  i += 3)  {
			fprintf(file, "f %d %d %d\n", pt->m_I[i + 0] + 1 + add, pt->m_I[i + 1] + 1 + add,
					  pt->m_I[i + 2] + 1 + add);
		}
		add += pt->m_V.size();
		fprintf(file, "# %d verticies\n", pt->m_V.size());
	}
	fclose(file);
	return true;
}

/****************************************************************************************

****************************************************************************************/

void Terrain::finishV()  {
	int i;
	m_VertCount = m_V.size();

	//tlm20070416+++
	if (m_VertCount==0)  {
		//int bp = 1;
		return;
	}
	//tlm20070416---

	// Figure out the color and the 

	COURSEVERTEX *pcv = &(m_V[0]);
	float u, v;
	u = m_Min.x / ms_usize;
	v = m_Min.z / ms_vsize;
	u = u - ((int) u);
	v = v - ((int) v);

	for (i = 0; i < (int)m_VertCount; i++,pcv++)  {
		if (pcv->color > 1.0f)  {
			pcv->color = 1.0f;
		}
		u32 diffuse = SColorf(pcv->color, pcv->color, pcv->color,1.0f).toSColor().color;
		pcv->diffuse = diffuse;
		pcv->tu1 = ((pcv->v.x - m_Min.x) / ms_usize) + u;
		pcv->tv1 = ((pcv->v.z - m_Min.z) / ms_vsize) + v;
	}

	m_IndexCount = m_I.size();

	if (m_IndexCount > 0)  {				// tlm20070420
		WORD *pi = &(m_I[0]);
		for (i = 0; i < (int)m_IndexCount; i += 3)  {
			WORD t = pi[i];
			pi[i] = pi[i + 1];
			pi[i + 1] = t;
		}
	}

	return;
}



/****************************************************************************************

****************************************************************************************/

void Terrain::CloseModel()  {
#ifndef IGNOREFORNOW
	//if (m_pIBuf)
	//{
	//	m_pIBuf->Release();
	//	m_pIBuf = NULL;
	//}
	if (m_pVBuf)  {
		m_pVBuf->Release();
		m_pVBuf = NULL;
	}
#endif
//	if(m_pMesh)
//		m_pMesh->drop();
//	m_pMesh = NULL;

	//if(terrainNode != 0)
	//	terrainNode->drop();
	terrainNode = 0;

	ms_List.AddHead(*this);
}


/****************************************************************************************

****************************************************************************************/

void __stdcall Terrain::_beginCB(GLenum type, Terrain *pt)  {
//void Terrain::_beginCB(GLenum type, Terrain *pt)  {
	if (type != GL_TRIANGLES)  {
	}
}


/****************************************************************************************

****************************************************************************************/

void __stdcall Terrain::_edgeFlagCB(GLboolean flag, Terrain *pt)  {
//void Terrain::_edgeFlagCB(GLboolean flag, Terrain *pt)  {
}

/****************************************************************************************

****************************************************************************************/

void __stdcall Terrain::_vertexCB(unsigned int vertexIndex, Terrain *pt)  {
//void Terrain::_vertexCB(unsigned int vertexIndex, Terrain *pt)  {
	if (vertexIndex < 0x8000)
		pt->m_I.push_back((short) (vertexIndex));
	else
		pt->m_Errors++;
	int aa = 0;
}


/****************************************************************************************

****************************************************************************************/

void __stdcall Terrain::_endCB(Terrain *pt)  {
//void Terrain::_endCB(Terrain *pt)  {
	int b = 0;
}


/****************************************************************************************

****************************************************************************************/

void __stdcall Terrain::_combineCB(GLdouble coords[3], unsigned int vertexData[4], GLfloat weight[4], unsigned int *outData, Terrain *pt)  {
//void Terrain::_combineCB(GLdouble coords[3], unsigned int vertexData[4], GLfloat weight[4], unsigned int *outData, Terrain *pt)  {
	pt->m_cv.v.x = (float) coords[0];
	pt->m_cv.v.y = (float)
						(pt->m_V[vertexData[0]].v.y * weight[0] +
						 pt->m_V[vertexData[1]].v.y * weight[1] +
						 pt->m_V[vertexData[2]].v.y * weight[2] +
						 pt->m_V[vertexData[3]].v.y * weight[3]);
	pt->m_cv.v.z = (float) coords[2];
	pt->m_V.push_back(pt->m_cv);
	/*
		// create a new vertex with the given coordinates
		Vertex newVertex;
		newVertex.x = coords[0];
		newVertex.y = coords[1];
		newVertex.z = coords[2];
		const Vertex& vert0 = pt->Vertices[vertexData[0]];
		const Vertex& vert1 = pt->Vertices[vertexData[1]];
		const Vertex& vert2 = pt->Vertices[vertexData[2]];
		const Vertex& vert3 = pt->Vertices[vertexData[3]];
		newVertex.u =
			 vert0.u * weight[0] +
			 vert1.u * weight[1] +
			 vert2.u * weight[2] +
			 vert3.u * weight[3];
		newVertex.v =
			 vert0.v * weight[0] +
			 vert1.v * weight[1] +
			 vert2.v * weight[2] +
			 vert3.v * weight[3];
		// add the vertex to the calling object's vertex vector
		pt->Vertices.push_back(newVertex);
		// pass back the index of the new vertex; it will be passed
		// as the vertexIndex parameter to VertexCB in turn
		*outData = pt->Vertices.size() - 1;
	*/
}


/****************************************************************************************

****************************************************************************************/

void __stdcall Terrain::_errorCB(GLenum errno, Terrain *pt)  {
//void Terrain::_errorCB(GLenum errno, Terrain *pt)  {
	pt->m_Errors++;
}


/****************************************************************************************

****************************************************************************************/

int Terrain::pushV(const COURSEVERTEX &cv)  {
	//float len = D3DXVec3Length(&(m_LastV - cv.v));
	float len = (m_LastV - cv.v).getLengthSQ();
	float idx;
	if (len > 0.0000001f)  {
		m_V.push_back(cv);
		m_LastV = cv.v;
	}
	else
		idx = 1.0f;
	return m_V.size() - 1;
}

/****************************************************************************************

****************************************************************************************/

void Terrain::InitCut()  {
	EndEnclosed();
	FinishCut();
	CloseModel();

	m_cv.v.y = 0.0f;
	m_cv.color = 1.0f;
	m_cv.pdata = 0;

	m_V.clear();
	m_I.clear();

	// Now build the initial nodes.
	m_cv.v.x = m_Min.x;
	m_cv.v.z = m_Min.z;

	EdgeNode *pn = new EdgeNode;
	m_pEdgeArr[0] = pn;
	pn->dir = 0;
	pn->pair = NULL;
	pn->start = m_Min.x;
	pn->dist = m_Max.x - m_Min.x;
	pn->count = 1;
	pn->idx = 0;
	pn->uid = 0;
	m_V.push_back(m_cv);

	m_cv.v.x = m_Max.x;
	pn = new EdgeNode;
	m_pEdgeArr[1] = pn;
	pn->dir = 1;
	pn->pair = NULL;
	pn->start = m_Min.z;
	pn->dist = m_Max.z - m_Min.z;
	pn->count = 1;
	pn->idx = 1;
	pn->uid = 0;
	m_V.push_back(m_cv);


	m_cv.v.z = m_Max.z;
	pn = new EdgeNode;
	m_pEdgeArr[2] = pn;
	pn->dir = 2;
	pn->pair = NULL;
	pn->start = m_Max.x;
	pn->dist = m_Max.x - m_Min.x;
	pn->count = 1;
	pn->idx = 2;
	pn->uid = 0;
	m_V.push_back(m_cv);

	m_cv.v.x = m_Min.x;
	pn = new EdgeNode;
	m_pEdgeArr[3] = pn;
	pn->dir = 3;
	pn->pair = NULL;
	pn->start = m_Max.z;
	pn->dist = m_Max.z - m_Min.z;
	pn->count = 1;
	pn->idx = 3;
	pn->uid = 0;
	m_V.push_back(m_cv);
	m_LastV = m_cv.v;

	m_pEdgeArr[0]->next = m_pEdgeArr[1];
	m_pEdgeArr[1]->next = m_pEdgeArr[2];
	m_pEdgeArr[2]->next = m_pEdgeArr[3];
	m_pEdgeArr[3]->next = m_pEdgeArr[0];

	m_pStartEdge = NULL;
	m_Cuts = 0;
}

/****************************************************************************************

****************************************************************************************/

bool Terrain::FinishCut()  {
	if (!m_pEdgeArr[0])  {
		return false;
	}

	EdgeNode *pn = m_pEdgeArr[0];
	EdgeNode *pstart;
	EdgeNode *pnext;
	GLUtesselator *ptess = createTess();
	if (ptess)  {
		if (m_Cuts == 0)  {
			// Just a square.
			gluTessBeginPolygon(ptess, reinterpret_cast<void*>(this));
			gluTessBeginContour(ptess);
			gluTessVertex(ptess, DVECTOR3(m_V[0].v.x, 0.0f, m_V[0].v.z), (void *) (0));
			gluTessVertex(ptess, DVECTOR3(m_V[1].v.x, 0.0f, m_V[1].v.z), (void *) (1));
			gluTessVertex(ptess, DVECTOR3(m_V[2].v.x, 0.0f, m_V[2].v.z), (void *) (2));
			gluTessVertex(ptess, DVECTOR3(m_V[3].v.x, 0.0f, m_V[3].v.z), (void *) (3));
			gluTessEndContour(ptess);
			gluTessEndPolygon(ptess);
		}
		else  {
			static int uid = 2;
			pstart = m_pEdgeArr[0];
			pn = m_pEdgeArr[0];
			pnext = pn;
			int best = 0;
			do  {
				if ((pn->pair) && (pn->idx > best) && (pn->count > 0))  {
					best = pn->idx;
					pnext = pn;
				}
				pn = pn->next;
			}
			while (pn != pstart);
			pstart = pnext;
			pn = pstart;

			do  {
				if (pn->count < 0)  {
					uid++;
					pn->count = 0;
					EdgeNode *ps = pn->next;
					EdgeNode *preset = ps;
					gluTessBeginPolygon(ptess, reinterpret_cast<void*>(this));
					gluTessBeginContour(ptess);
					do  {
						if (ps->uid == uid)
							break;
						ps->uid = uid;
						if (ps->pair && (ps->count != 0))  {
							pnext = ps->pair;
							if (ps->idx < pnext->idx)  {
								for (int i = ps->idx;
									  i < pnext->idx;
									  i++)  {
									gluTessVertex(ptess, DVECTOR3(m_V[i].v.x, 0.0f, m_V[i].v.z), (void *) (i));
								}
							}
							else  {
								for (int i = ps->idx;
									  i > pnext->idx;
									  i--)  {
									gluTessVertex(ptess, DVECTOR3(m_V[i].v.x, 0.0f, m_V[i].v.z), (void *) (i));
								}
							}

							ps->count = 0;
							pnext->count = 0;
						}
						else  {
							gluTessVertex(ptess, DVECTOR3(m_V[ps->idx].v.x, 0.0f, m_V[ps->idx].v.z),
											  (void *) (ps->idx));
							pnext = ps->next;
						}
						ps = pnext;
					}
					while (preset != ps);
					gluTessEndContour(ptess);
					gluTessEndPolygon(ptess);
				}
				pn = pn->next;
			}
			while (pn != pstart);
		}
		gluDeleteTess(ptess);
	}

	int i, j;
	int max = m_V.size();

	//tlm20070516+++
	if (max<5)  {
		goto skip;
	}
	//tlm20070516---

{
	float d[4];

	COURSEVERTEX *pcorner = &m_V[0];
	COURSEVERTEX *pcv = &(m_V[4]);

	d[0] = d[1] = d[2] = d[3] = FLT_MAX;

	for (i=4; i<max; i++,pcv++)  {
		for (j=0; j<4; j++)  {
			float x = pcv->v.x - pcorner[j].v.x;
			float z = pcv->v.z - pcorner[j].v.z;
			float c = x*x + z*z;

			if (c < d[j])  {
				pcorner[j].v.y = pcv->v.y;
				d[j] = c;
			}
		}
	}
}

skip:

	// Delete the edge nodes.

	pn = m_pEdgeArr[0];
	pstart = pn;
	pnext = pn->next;

	do  {
		pnext = pn->next;
		delete pn;
		pn = pnext;
	}
	while (pn != pstart);

	m_pEdgeArr[0] = NULL;
	finishV();

	return true;
}


/****************************************************************************************

****************************************************************************************/

Terrain::EdgeNode * Terrain::addEdgeNode(const COURSEVERTEX &cv, int dir, bool left)  {
	EdgeNode *plast;
	EdgeNode *ps = m_pEdgeArr[dir];
	while (ps->dir == dir)  {
		float base = (dir & 1 ? cv.v.z : cv.v.x);
		base -= ps->start;
		if (dir >= 2)  {
			if (base > 0.0f)  {
				m_Errors++;
				return NULL;
			}
			base = -base;
		}
		else if (base < 0.0f)  {
			m_Errors++;
			return NULL;
		}
		if (base == 0.0f)	// VERY SPECIAL CASE - We need to turn this corner into a linked node.
			return ps;

		if (base < ps->dist)  {
			// Break this node into to.
			EdgeNode *pnew = new EdgeNode;
			pnew->next = ps->next;
			ps->next = pnew;
			pnew->dist = ps->dist - base;
			pnew->start = ps->start + (dir >= 2 ? -base : base);
			ps->dist = base;
			pnew->dir = dir;
			pnew->pair = 0;
			pnew->count = 1;
			pnew->idx = pushV(cv);
			pnew->uid = 0;
			return pnew;
		}
		plast = ps;
		ps = ps->next;
	}
	m_Errors++;
	return NULL;
}


/****************************************************************************************

****************************************************************************************/

void Terrain::StartCut(const COURSEVERTEX &cv, int dir, bool left)  {
	if (!m_pEdgeArr[0] || m_pStartEdge)  {
		m_Errors++;
		return;
	}
	EdgeNode *pn = addEdgeNode(cv, dir, left);
	if (!pn || pn->pair)  {
		m_Errors++;
		return;
	}
	m_pStartEdge = pn;
}

/****************************************************************************************

****************************************************************************************/

//static int sm;
void Terrain::AddCutV(const COURSEVERTEX &cv)  {
	if (!m_pEdgeArr[0] || !m_pStartEdge)  {
		m_Errors++;
		return;
	}
	pushV(cv);
}

/****************************************************************************************

****************************************************************************************/

void Terrain::EndCut(const COURSEVERTEX &cv, int dir, bool left)  {
	if (!m_pEdgeArr[0] || !m_pStartEdge)  {
		m_Errors++;
		return;
	}
	EdgeNode *pn = addEdgeNode(cv, dir, left);
	if (!pn || pn->pair)  {
		m_Errors++;
		return;
	}
	pn->pair = m_pStartEdge;
	m_pStartEdge->pair = pn;
	m_pStartEdge->count = (pn->idx - m_pStartEdge->idx) + 1;
	pn->count = -m_pStartEdge->count;
	m_Cuts++;
	m_pStartEdge = NULL;
}


/****************************************************************************************

****************************************************************************************/

HRESULT Terrain::Render()  {
#ifndef IGNOREFORNOW

	if (!ReadyModel())  {
		return E_FAIL;
	}

	// tlm20070711+++
	if (m_I.size()==0)  {
		return E_FAIL;
	}
	// tlm20070711---
#ifndef IGNOREFORNOW

	m_Course.ms_pDevice->DrawIndexedPrimitiveVB(
			D3DPT_TRIANGLELIST, 
			m_pVBuf, 
			0, 
			m_VertCount, 
			&(m_I[0]), 
			m_IndexCount, 
			0
		);
#endif
	m_RID = ms_RID;
	ms_ReadyList.AddHead(*this);
#endif

	return 0;
}


/****************************************************************************************

****************************************************************************************/
void Terrain::Show(bool bShow)
{
	if(gpCourse != NULL && terrainNode != 0)
	{
		if(bShow)
		{
			if(terrainNode->getParent() != gpCourse->m_pNodeVisible)
			{
				gpCourse->m_pNodeVisible->addChild(terrainNode);
				AddRender();
			}
		}
		else
		{
			if (terrainNode->getParent() != gpCourse->m_pNodeHidden)
				gpCourse->m_pNodeHidden->addChild(terrainNode);
		}
	}
}


bool Terrain::ReadyModel(ISceneNode *groupNode)  {

#ifdef IGNOREFORNOW

	if(bMeshCreated)
		return true;

	m_VertCount = m_V.size();
	m_IndexCount = m_I.size();

	if ((m_VertCount <= 0) || (m_IndexCount <= 0))  {
		return false;
	}

	int i;

	terrainNode = 0;
	SMesh* pMesh = new SMesh();
	if(pMesh)
	{
		SMeshBuffer *pMeshBuf = new SMeshBuffer();
		pMesh->addMeshBuffer(pMeshBuf);
		// to simplify things we drop here but continue using buf
		pMeshBuf->drop();

		m_Course.Material.TextureLayer[0].Texture = D3DBase::GetDriver()->getTexture(m_Course.m_Tex[CCourse::TEX_FAR_GROUND]);
		//m_Course.Material.TextureLayer[1].Texture = D3DBase::GetDriver()->getTexture("detailmap3.jpg");
		pMeshBuf->Material = m_Course.Material;

		pMeshBuf->Vertices.set_used(m_VertCount);
		COURSEVERTEX *pv2 = &(m_V[0]);
		for (i = 0; i < (int)m_VertCount; i++)  {
			u32 diffuse = pv2->diffuse; //SColor(255,153,153,153).color;

			S3DVertex& v = pMeshBuf->Vertices[i];
			v.Pos.set(pv2->v * mult);
			v.Normal.set(0,1,0);
			v.Color = diffuse;
			v.TCoords.set(pv2->tu1, pv2->tv1);
			//Msg("i=%d,x=%f,y=%f,z=%f",i,pv2->v.x,pv2->v.y,pv2->v.z);
			pv2++;
		}
		WORD *idx = &(m_I[0]);
		pMeshBuf->Indices.set_used(m_IndexCount);
		for (i = 0; i < (int)m_IndexCount; i++)  {
			pMeshBuf->Indices[i]=idx[i];
			//Msg("i=%d,idx=%d",i,pMeshBuf->Indices[i]);
		}
		pMeshBuf->recalculateBoundingBox();
		pMesh->recalculateBoundingBox();

		// / *

//		terrainNode = D3DBase::addCustomSceneNode(groupNode);
		IMeshSceneNode* meshnode = D3DBase::GetSceneManager()->addMeshSceneNode(pMesh, groupNode);
		pMesh->drop();
		//meshnode->setScale(core::vector3df(mult,mult,mult));
		meshnode->setPosition(core::vector3df(0,0,0));
		meshnode->setMaterialFlag(video::EMF_FOG_ENABLE, fogOn);
		meshnode->setMaterialFlag(video::EMF_BACK_FACE_CULLING, true);
		meshnode->setMaterialFlag(EMF_TEXTURE_WRAP, ETC_REPEAT);
		bool uselight = false;
		meshnode->setMaterialFlag(EMF_NORMALIZE_NORMALS,uselight);
		meshnode->setMaterialFlag(EMF_LIGHTING,uselight);
		meshnode->setMaterialFlag(video::EMF_ANISOTROPIC_FILTER, true);

		terrainNode = meshnode;
		// * /
	}
	bMeshCreated = true;
	if(terrainNode == 0)
	{
		int bp = 1;
	}

#else
	if (m_pVBuf)  {
		return true;
	}

	m_VertCount = m_V.size();
	m_IndexCount = m_I.size();

	if ((m_VertCount <= 0) || (m_IndexCount <= 0))  {
		return false;
	}
	D3DVERTEXBUFFERDESC desc;
	desc.dwSize = sizeof(desc);
	desc.dwCaps = D3DVBCAPS_WRITEONLY | gVBufFlags;
	desc.dwFVF = FVF_COURSEVERTEX;
	desc.dwNumVertices = m_VertCount;

	HRESULT hr = gpD3D->CreateVertexBuffer(&desc, &m_pVBuf, 0);

	if (hr != 0)  {
		CloseModel();
		return false;
	}

	COURSEVERTEX *vert_dest;
	hr = m_pVBuf->Lock(DDLOCK_WRITEONLY, (LPVOID *) &vert_dest, 0);
	if (hr != 0)  {
		CloseModel();
		return false;
	}

	COURSEVERTEX *pcv = &(m_V[0]);

	for (int i=0; i<(int)m_VertCount; i++)  {
		*vert_dest++ = *pcv++;
	}

	m_pVBuf->Unlock();
#endif

	ms_ReadyList.AddHead(*this);

	return true;
}

bool Terrain::InFrustum(ICameraSceneNode* cam)
{
	f32 culldist = fardistCam+30;
	f32 culldistM = culldist * mult;
	core::aabbox3d<f32> box = terrainNode->getBoundingBox(); 
	terrainNode->getAbsoluteTransformation().transformBox(box); 
	vector3df vrot,vdir,vobj,vcam = cam->getAbsolutePosition(), v,minv,maxv;
	f32 dist, dot, r, d = culldist * mult;
	vobj = box.getCenter(); 
	r = (float)vobj.getDistanceFrom(box.MinEdge); 
	v = vobj - vcam;
	dist = v.getLength();
	if(dist - r < d)
	{
		if(r > dist)
			return true;
		else
		{
			vrot = cam->getRotation();
			vdir = vrot.rotationToDirection();
			dot = vdir.dotProduct(vobj);
			if(dot > 0)
			{
				return true;
			}
		}
	}
	return false;

	/*
   float d=0; 
   for(u16 i=0;i<6;i++) 
   { 
		d = -pvf->planes[i].getDistanceTo(sphere_pos);
      //d = pvf->planes[i].Normal.dotProduct(sphere_pos)+pvf->planes[i].D; 
      if(d >= sphere_r) 
         return false; 
   } 
   return true; 
   */
}
