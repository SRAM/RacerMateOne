
#include "stdafx.h"
//#include <stdio.h>
//#include <crf.h>
//#include <windows.h>
//#include <time.h>
//#include <stdlib.h>

//#include "CCourse.h"
//#include <vector>
using namespace std;


struct SegmentData3DC
{
	double	Length;
	double	StartGrade;
	double	EndGrade;
	double	Wind;
	double	Rot;
	double	StartY;
	double	EndY;
	int		Divisions;
};

struct ExtSegmentData3DC
{
	double	StartDist;
	int special;
	DWORD	flags;
	//flags & SECTION_F_ABS_STARTLOC
	double	startlocX;
	double	startlocY;
	double	startlocZ;
	//flags & SECTION_F_ABS_Y
	double	divisions;
	double	actuallength;
	double **yArr;
	double **gArr;
};

class CourseData3DC
{
public:
	CCourse							m_Course;
	vector<SegmentData3DC>			m_Arr;
	vector<ExtSegmentData3DC>		m_ExtArr;

	CourseData3DC():m_Course(false,true) {}
	
	bool Load(const char *strname)
	{
		if (!m_Course.Load( strname ))
			return false;
		CCourse::Section *psec = m_Course.GetFirstRealSection();
		int cnt = 0;
		SegmentData3DC sdata;
		ExtSegmentData3DC esdata;
		double prevgrade = 0.0;
		for(;psec;psec = psec->Next())
		{
			sdata.Wind = psec->GetWind();
			sdata.Length = psec->GetLength();
			core::vector3df vstart = psec->GetStartLoc();
			core::vector3df vend = psec->GetEndLoc();
			sdata.StartGrade = prevgrade;
			prevgrade = sdata.EndGrade = psec->get_grade_d_100();
			sdata.Divisions = psec->GetDivisions();
			sdata.StartY = vstart.y;
			sdata.EndY = vend.y;
			/*
			if(sdata.Length != 0)
				sdata.Grade = (float)(((vend.y - vstart.y) / sdata.Length));
			else
				sdata.Grade = 0;
			*/
			sdata.Rot = psec->GetRotation();
			m_Arr.push_back(sdata);

			esdata.actuallength = psec->GetActualLength();
			//esdata.divisions = psec->m_Divisions;
			//esdata.flags = psec->m_Flags;
			//esdata.special = psec->m_SpecialTexture;
			esdata.StartDist = psec->GetStartDistance();
			esdata.startlocX = vstart.x;
			esdata.startlocY = vstart.y;
			esdata.startlocZ = vstart.z;
			//float divisionlength = psec->m_DivisionLength;
			//esdata.yArr = psec->m_YArr;
			//esdata.gArr = psec->m_GArr;

			if (psec->IsFinishSection())
				break;
		}
		return true;
	}
};

void *Load3DCCourse( const char *strname )
{
	CourseData3DC *pc = new CourseData3DC();

	if (pc->Load( strname ))
	{
		return (void *)pc;
	}
	delete pc;
	return NULL;
}

void Free3DCCourse( void *ptr )
{
	if (ptr != NULL)
		delete (CourseData3DC *)ptr;
}


int Get3DCCourseSegments( void *ptr )
{
	CourseData3DC *pc = (CourseData3DC *)ptr;
	return (pc != NULL ? pc->m_Arr.size():0);
}

SegmentData3DC Get3DCCourseSegment( void *ptr, int num )
{
	CourseData3DC *pc = (CourseData3DC *)ptr;
	return pc->m_Arr[num];
}

int IsCourseClosedLoop( void *ptr )
{
	CourseData3DC *pc = (CourseData3DC *)ptr;
	return (pc != NULL ? (pc->m_Course.IsLoopClosed() ? 1 : 0) : 0);
}

int GetLaps( void *ptr )
{
	CourseData3DC *pc = (CourseData3DC *)ptr;
	return (pc != NULL ? pc->m_Course.getLaps() : 0);
}


int GetExt3DCCourseSegments( void *ptr )
{
	CourseData3DC *pc = (CourseData3DC *)ptr;
	return (pc != NULL ? pc->m_ExtArr.size():0);
}

ExtSegmentData3DC GetExt3DCCourseSegment( void *ptr, int num )
{
	CourseData3DC *pc = (CourseData3DC *)ptr;
	return pc->m_ExtArr[num];
}
