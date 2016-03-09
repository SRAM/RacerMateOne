
#ifndef _REGULARCOURSE_H_
#define _REGULARCOURSE_H_

#include <config.h>

#if defined VIDEO_APP
#elif defined MULTIVID_APP
#else
	#include "..\\3d\\include\\d3d.h"
#endif

#include <vector>

using namespace std;

#include <stdio.h>

#include <course.h>

typedef struct  {
	float length;
	float grade;
	float wind;
	float accumMiles;
	float startel;
	float endel;
	float minutes;
	float dm;
	float load;
} REGULARLEG;

class regularCourse : public Course {


	protected:
		virtual void computeStats(void);

	private:
		#define NEW_ELEVATION_CALCULATION
		POINT *p;
		HICON icon;
		
		int lastGrade_i;
		int lastWind_i;
		int lasti;
		float *y;

		int *x;
		float *my, *by;
		void computeElevations(void);
		void readCourseFile(void);
		void cleanup(void);

#ifdef TOPO_APP
		void loadEditLeg(int i);
		void save_leg_to_memory(int i);
		bool metric;
#endif

		bool regular_course_metric_header;				// this is the metric flag from the course header
										// Course::gMetric is the overall user metric flag


	public:

		int *get_x_array(void);
		virtual void reset_wind_lookup(void);

		regularCourse(bool _metric, HWND _phwnd, char *_fname, RECT *_courserect=NULL);
		regularCourse(RECT *_courserect, OLD_COURSE_HEADER *_cheader , REGULARLEG *_legs);
		regularCourse(char *roadfname, char *_crsname, RECT *_courserect);
		regularCourse(void);

		virtual ~regularCourse();

		virtual float *get_y_array(void);
		virtual int getnpoints(void);
		virtual int getnlegs(void);

		virtual void save(char *_fname);
		void dump(void);
		void dump2(char *_fname, int _units);

		int readLegs(void);
		int draw(HDC, HFONT, bool, double, bool _drawProfile, bool _floodfill, int _style, float _minxxscale, float _maxxscale, float _minyscale, float _maxyscale);
	
		float minel;				// in miles
		float maxel;				// in miles
		float minfeet;
		float maxfeet;

		float elevation;

		void resize(RECT waverect);
		int read_data(void);
		int read_header(void);

		float getTotalLen(void) {return (float)totalFeet;}

		float getLoad(int i);
		virtual float getGrade(double dist, int *_lastGrade_i=NULL);
		virtual float getGrade(int i);
		virtual float getWind(double dist, int *_lastWind_i=NULL);
		int gety(double);
		void writeEncrypted(FILE *outstream, CRF*, FILE *_dbgstream=NULL);
		double getLoad(double _minutes);
		void test(void);
		void exportLegs(FILE *stream);
		virtual POINT *getGradeChangeCoordinates(int *n);
		vector<REGULARLEG> legs;

#ifdef TOPO_APP
		int showInfo(void);
		int edit(HICON _icon);
#endif

		virtual void reset(void)  {
			lastGrade_i = 0;
			lastWind_i = 0;
			warned = false;
		}

};
#endif		// #ifndef _REGULARCOURSE_H_

