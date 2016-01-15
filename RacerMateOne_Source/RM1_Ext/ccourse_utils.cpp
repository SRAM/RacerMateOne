
//#include <config.h>
//#include "_Course.h"
//#include <stdio.h>
//#include <string.h>
//#include <math.h>

//#include <windows.h>

/*
#ifdef BIKE_APP
	#include <utils.h>
	#include <mesh.h>
	#include <Sprite.h>
	#include <Mesh.h>
	#include <GLModel.h>
	//#include <dprintf.h>
	#include <string.h>
	#include "computrainerbase.h"
	#include "globals.h"
	#include "bikeApp.h"
	#include "tools.h"
	#include "Landscape.h"
	#include "Terrain.h"
#else
//	#include <tools.h>
	//#include <mesh.h>
#endif
*/
#include "stdafx.h"

#include "ccourse_utils.h"


extern STRIP_INFO stripArrBase[MAX_STRIPS];
extern STRIP_INFO *stripArr;
extern int lSTRIPS;					// 10
extern int lSTRIPS_H;				// 5
extern int lSTRIP_START;			// 0


/************************************************************************************

************************************************************************************/

void initStr(char *&pstr)  {
	pstr = new char[2];
	pstr[0] = '\0';
}

/************************************************************************************

************************************************************************************/

void setString(char *&pstr, const char *str)  {

	if (!str)  {
		str = "";
	}

	if (pstr)  {
		delete[] pstr;
	}

	pstr = new char[strlen(str) + 1];
	strcpy(pstr, str);
}

/************************************************************************************

************************************************************************************/

char * makeName(const char *name, char *buf)  {
	char *slash = (char *)strrchr(name, '\\');
	if (!slash)  {
		char *c = (char *)strrchr(name, '.');
		if (c)  {
			if (gPerformanceDirectory[0] != 0 && gCourseDirectory[0] != 0)  {
				sprintf(buf, "%s\\%s", (_stricmp(c + 1, "rmp") == 0 ? gPerformanceDirectory : gCourseDirectory), name);
				name = buf;
			}
			else  {
				sprintf(buf, "%s", name);
				name = buf;
			}
		}
	}
	else
		strcpy(buf, name);
	return buf;
}

/************************************************************************************

************************************************************************************/

void extRect2(core::vector3df &minv, core::vector3df &maxv, CONST core::vector3df &v)  {
	if (v.x < minv.x)
		minv.x = v.x;
	if (v.x > maxv.x)
		maxv.x = v.x;
	if (v.z < minv.z)
		minv.z = v.z;
	if (v.z > maxv.z)
		maxv.z = v.z;
}

/************************************************************************************

************************************************************************************/

void extRect3(core::vector3df &minv, core::vector3df &maxv, CONST core::vector3df &v)  {
	if (v.y < minv.y)
		minv.y = v.y;
	if (v.y > maxv.y)
		maxv.y = v.y;
	extRect2(minv, maxv, v);
}

/************************************************************************************

************************************************************************************/

float ffmod(float v, float d)  {
	register double a = fmod(v, d);
	if (a < 0.0)
		a += d;
	return (float) a;
}


/************************************************************************************

************************************************************************************/

void setStrips(bool ledge, bool redge)  {

	if (ledge && redge)  {
		stripArr = stripArrBase;
		lSTRIPS = 10;
		lSTRIPS_H = 5;
		lSTRIP_START = 0;
	}
	else if (ledge)  {
		stripArr = stripArrBase;
		lSTRIPS = 7;
		lSTRIPS_H = 5;
		lSTRIP_START = 0;
	}
	else if (redge)  {
		stripArr = stripArrBase + 3;
		lSTRIPS = 7;
		lSTRIPS_H = 2;
		lSTRIP_START = 3;
	}
	else  {
		stripArr = stripArrBase + 3;
		lSTRIPS = 4;
		lSTRIPS_H = 2;
		lSTRIP_START = 3;
	}
	return;
}


#if defined(BIKE_APP) || defined(MULTI_APP) || defined(MULTIVID_APP) || defined(TOPO_APP)

/************************************************************************************

************************************************************************************/

bool splitLine(char *buf, char *&t, char *&d)  {
	char *ps = strchr(buf, '=');
	if (!ps)
		return false;
	*ps++ = '\0';
	t = Strip(buf);
	d = Strip(ps);
	return true;
}

#endif




#ifdef BIKE_APP

/************************************************************************************

************************************************************************************/

int cmpStr(const void *elem1, const void *elem2)  {
	return _stricmp(*((const char * *) elem1), *((const char * *) elem2));
}


/************************************************************************************

************************************************************************************/

void buildStrip(WORD *&idx, int &minv, int &maxv, int sleft, int sright, int cnt)  {
	if (sleft < sright)  {
		minv = sleft;
		maxv = sright + cnt;
	}
	else  {
		minv = sright;
		maxv = sleft + cnt;
	}
	int i;
	for (i=0; i<cnt; i++)  {
		*idx++ = i + sright;
		*idx++ = i + sleft;
	}
}

/************************************************************************************

************************************************************************************/

void buildSide(WORD *&pidx, int &minv, int &maxv, int l, int lcnt, int s, int scnt, bool ccw)  {
	int li = 0, si = 0;
	float lstep = 1000.0f / (lcnt - 1);
	float sstep = 1000.0f / (scnt - 1);

	float ld = lstep / 2.0f;
	float sd = 0.0f;

	minv = (l < s ? l : s);
	maxv = (l + lcnt > s + scnt ? l + lcnt : s + scnt);

	scnt--;

	while (1)  {
		*pidx++ = l + li;
		if (ccw)  {
			*pidx++ = s + si + 1;
			*pidx++ = s + si;
		}
		else  {
			*pidx++ = s + si;
			*pidx++ = s + si + 1;
		}

		sd += sstep;
		si++;
		if (sd >= ld)  {
			*pidx++ = s + si;
			if (ccw)  {
				*pidx++ = l + li;
				*pidx++ = l + li + 1;
			}
			else  {
				*pidx++ = l + li + 1;
				*pidx++ = l + li;
			}
			li++;
			ld += lstep;
		}

		if (si >= scnt)
			break;
	}
}

/************************************************************************************

************************************************************************************/

void smooth(COURSEVERTEX *pcv, int sdiv, int div)  {
	if (sdiv < 2)
		sdiv = 2;
	if (sdiv >= div)  {
		for (sdiv = 0;
			  sdiv < div;
			  sdiv++)  {
			pcv->tu1 = 1.0f;
			pcv++;
		}
		return;
	}

	float acc;
	float ad = (float) div / sdiv;

	COURSEVERTEX *ps = pcv;
	COURSEVERTEX *pd = pcv + 1;
	COURSEVERTEX *pn;
	for (acc = 0;
		  acc < div;
		  ps = pn)  {
		acc += ad;
		pn = (acc >= div ? pcv + (div - 1) : pcv + (int) acc);
		pn->tu1 = 0.0f;
		int num = pn - ps;
		int i;
		core::vector3df v = pn->v - ps->v;
		for (i = 1;
			  pd < pn;
			  pd++,i++)  {
			pd->v = ps->v + v * (float) i / (float) num;
			pd->tu1 = 0.0f;
		}
	}
	pcv->tu1 = 0.0f;
	pcv[div - 1].tu1 = 0.0f;
}

/************************************************************************************

************************************************************************************/

int line_dxf(FILE *dxf_file, core::vector3df &v1, core::vector3df &v2, int color)  {
	fprintf(dxf_file, "0\r\n");
	fprintf(dxf_file, "LINE\r\n");

	//fprintf(dxf_file, "8\r\n");
	// fprintf(dxf_file, "%s\r\n", layer);

	fprintf(dxf_file, "62\r\n");

	// fprintf(dxf_file, "5\r\n");//"%d\r\n", color);

	fprintf(dxf_file, "%d\r\n", color);
	fprintf(dxf_file, "10\r\n");
	fprintf(dxf_file, "%lf\r\n", v1.x);
	fprintf(dxf_file, "20\r\n");
	fprintf(dxf_file, "%lf\r\n", -v1.y);
	fprintf(dxf_file, "30\r\n");
	fprintf(dxf_file, "%lf\r\n", v1.z);
	fprintf(dxf_file, "11\r\n");
	fprintf(dxf_file, "%lf\r\n", v2.x);
	fprintf(dxf_file, "21\r\n");
	fprintf(dxf_file, "%lf\r\n", -v2.y);
	fprintf(dxf_file, "31\r\n");
	fprintf(dxf_file, "%lf\r\n", v2.z);

	return 0;
}

/************************************************************************************

************************************************************************************/

SColor windColor(float wind)  {
#ifndef IGNOREFORNOW

	static SColor base(152, 157, 12, 128);
	static SColor neg(12, 12, 157, 128);
	static SColor pos(200, 12, 12, 128);
	static bool setup = false;

	/*
		if (!setup)
		{
			neg = base - neg;
			pos = base - pos;
			setup = true;
		}
		*/
	//SColor(152,157,12,128 );

	wind /= SECTION_WIND_MAX;
	return base + ((wind < 0.0f ? neg : pos) - base) * (wind < 0.0f ? -wind : wind);
#endif
	return SColor(152,157,12,128 );
}

/************************************************************************************

************************************************************************************/

void addBB(core::vector3df &v)  {
	/*
	if (v.x < bbminx)
		bbminx = v.x;
	else if (v.x > bbmaxx)
		bbmaxx = v.x;

	if (v.y < bbminy)
		bbminy = v.y;
	else if (v.y > bbmaxy)
		bbmaxy = v.y;

	if (v.z < bbminz)
		bbminz = v.z;
	else if (v.z > bbmaxz)
		bbmaxz = v.z;*/
}

/******************************************************************************

******************************************************************************/
/*
void addEdge_F(edgepoints *&pe, std::vector<COURSEVERTEX> &va, int type)  {
	vector<COURSEVERTEX>::iterator i = va.begin();
	i++;
	for (;
		  i != va.end();
		  i++,pe++)  {
		COURSEVERTEX &v = (*i);
		pe->vertex = v.v;
		pe->color = v.color;
		pe->type = type;

		addBB(v.v);
	}
}
*/

/******************************************************************************

******************************************************************************/

/*
void addEdge_R(edgepoints *&pe, std::vector<COURSEVERTEX> &va, int type)  {
	vector<COURSEVERTEX>::reverse_iterator i = va.rbegin();
	i++;
	for (;
		  i != va.rend();
		  i++,pe++)  {
		COURSEVERTEX &v = (*i);
		pe->vertex = v.v;
		pe->color = v.color;
		pe->type = type;

		addBB(v.v);
	}
}
*/

/******************************************************************************

******************************************************************************/

/*
void addEdgePoint(edgepoints *&pe, COURSEVERTEX &v, int type)  {
	pe->vertex = v.v;
	pe->color = v.color;
	pe->type = type;
	addBB(v.v);
	pe++;
}
*/

#endif		// #ifdef BIKE_APP
