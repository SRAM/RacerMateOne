
#pragma once

#include <vector>

using namespace std;

#include <stdio.h>

#include "course.h"


typedef struct  {
	float minutes;
	float watts;
} ERGOPOINT;

/**************************************************************************

**************************************************************************/

typedef struct  {
	float startMinutes;
	float startWatts;
	float endMinutes;
	float endWatts;
	float m;						// the slope between ergopoints
	float b;
} ERGOLEG;


class ergoCourse : public Course {


	private:
		void computeLegs(void);
		float totalMinutes;
		float minload;
		float maxload;
		vector<ERGOPOINT> points;
		vector<ERGOLEG> legs;

		float minScaleMinutes;
		float maxScaleMinutes;

		float minScaleLoad;
		float maxScaleLoad;

	public:
		ergoCourse(RECT *_courserect, OLD_COURSE_HEADER *courseheader, vector<ERGOPOINT> _ergopoints);
		ergoCourse(char *_cdfname, RECT *_courserect);

		virtual ~ergoCourse();
		float getMinutes(void);
		int readAsciiHeader(void);
		int readAsciiData(void);
		int draw(HDC, HFONT, bool, double, bool _drawProfile, bool _floodfill, int _style, float _minxxscale, float _maxxscale, float _minyscale, float _maxyscale);
		void resize(RECT courserect);
		virtual float getGrade(double dist, int *_lastGrade_i=NULL);
		float getWind(double);

		float getTotalLen(void) { return totalMinutes; }

		float getLoad(int i);
		double getLoad(double _minutes);
		int gety(double);
		void writeEncrypted(FILE *outstream, CRF*, FILE *_dbgstream=NULL);
		float getGrade(int i);									// virtual
		void test(void);
		void exportLegs(FILE *stream);
		virtual void reset(void)  {
			return;
		}
		virtual float getMinY(void)  {
			return minload;
		}
		virtual float getMaxY(void)  {
			return maxload;
		}

		virtual float getMinX(void)  {
			return 0.0f;
		}
		virtual float getMaxX(void)  {
			return totalMinutes;
		}
};

