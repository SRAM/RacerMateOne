
#pragma once

#include <windows.h>
#include <stdio.h>

#include <config.h>

#include <defines.h>
#include <course.h>
#include <coursefile.h>
#include <MFD.h>
#include <bike.h>


class jimData : public MFD  {

	private:
		PerformanceInfo pinfo;
		int perfpointcount;
		int version;
		double totalMiles;
		void cleanup(void);
		User *user;
		int laps;

	public:
		jimData(HWND _phwnd, bool _metric, RECT *_courserect, char *_fname);
		virtual ~jimData(void);
		Course *getCourse(void) { return course;}
		double getTotalMiles(void)  {
			return totalMiles; 
		}		// hack for .3dp files

#ifdef DO_HISTOGRAMS
		virtual float *get_array(int _index);
#endif

};

