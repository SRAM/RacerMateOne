
#pragma once

#include <stdio.h>
#include <d3d.h>

#include <config.h>
#include <course.h>
#include <secfile.h>

#include <_course.h>


class willCourse : public Course {
	friend class Delorme;
	friend void plot(void);
	friend class velotronMFD;
	friend class velotronSFD;
	friend class PreVelotronData;
	friend class jimData;

	private:
		typedef struct  {
			float length;				// meters
			float grade;				// % grade
			float angle;				// degrees
			float wind;					// kph
			float accumMeters;		// accumulated meters at the end of this leg
			float elevation;			// meters
		} WILL_LEG;

		float second_start_meters;
		void init1(void);				// before we know the number of legs
		void init2(void);				// after we know the number of legs
		
		_Course *_course;
		bool hasperf;
		int perfgradever;				// if there is a performance in the _Course::
		int lastGrade_i;
		int lastWind_i;
		float *y;
		int *x;
		float *my, *by;

		double minel;
		double maxel;
		WILL_LEG *legs;
		void destroy2(void);
		long version;
		void editVersion56(FILE *instream, FILE *outstream);
		void dumpVersion56(void);
		void readVersion345(FILE *istream);

#ifdef TOPO_APP
		void loadEditLeg(int i);
		void save_leg_to_memory(int i);
#endif

		void save(char *_fname);

	public:
		willCourse(bool _metric, HWND _phwnd, char *_fname, RECT *_courserect);
		willCourse(RECT *_courserect, Course::OLD_COURSE_HEADER *_cheader , WILL_LEG *_legs, char *_fname=NULL);
		virtual ~willCourse();

		virtual int draw(HDC, HFONT, bool, double, bool _drawProfile=true, bool floodfill=true, int style=0, float _minxxscale=-FLT_MAX, float _maxxscale=-FLT_MAX, float _minyscale=-FLT_MAX, float _maxyscale=-FLT_MAX);

		virtual void resize(RECT waverect);
		virtual float getGrade(double dist, int *_lastGrade_i=NULL);
		virtual float getGrade(int i);
		virtual float getWind(double dist, int *_lastWind_i=NULL);

		virtual float getTotalLen(void) {return (float)totalFeet;}
		virtual float getLoad(int i);
		double getLoad(double _minutes);
		virtual int gety(double);
		void test(void);
		void exportLegs(FILE *stream);
		int edit(HICON);
		int showInfo(void);

		virtual void writeEncrypted(FILE *outstream, CRF*, FILE *_dbgstream=NULL);
		void write(char *_fname);

		double getSummedMeters(void)  {
			return (float)totalMeters;
		}
		double getTotalMeters(void)  {
			return totalMeters;
		}
		virtual void computeStats(void);

};


