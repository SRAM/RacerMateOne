
//#pragma once

#ifndef _C_COURSE_UTILS_H_
#define _C_COURSE_UTILS_H_

#include "StdAfx.h"

#define MAX_STRIPS	10


int cmpStr(const void *elem1, const void *elem2);
void buildStrip(WORD *&idx, int &minv, int &maxv, int sleft, int sright, int cnt);
void buildSide(WORD *&pidx, int &minv, int &maxv, int l, int lcnt, int s, int scnt, bool ccw);
void smooth(COURSEVERTEX *pcv, int sdiv, int div);
int line_dxf(FILE *dxf_file, core::vector3df &v1, core::vector3df &v2, int color);
video::SColor windColor(float wind);
void addBB(core::vector3df &v);

#define SECTION_WIND_MAX		50.0f

struct STRIP_INFO {
		float offset;		// Offset from center of the road.
		int div_divide;	// divisions divide.
		float rndlow;		// Range of the hills on the side of the road.
		float rndhigh;
		float texrepeat;	// How much the texture should repeat in the z dimension (tv)
		float x_repeat;	// how much the texture should repeat in the x dimension (tu)
		float coloradd;	// How much color you should add.
		bool treeok;		// Trees ok in this strip.
};

void initStr(char *&pstr);
//float frand(float min, float max);
void setString(char *&pstr, const char *str);
char * makeName(const char *name, char *buf);
float ffmod(float v, float d);
void setStrips(bool ledge, bool redge);
void extRect2(core::vector3df &minv, core::vector3df &maxv, CONST core::vector3df &v);
void extRect3(core::vector3df &minv, core::vector3df &maxv, CONST core::vector3df &v);


#endif
