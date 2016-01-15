#pragma once

#include <config.h>
#include <datasource.h>
#include <demodecoder.h>
#include <course.h>

/**********************************************************************************************

**********************************************************************************************/

class demoSource : public dataSource  {

	private:
		User *user;
		DWORD now;
		DWORD lastnow;
		unsigned long dms;
		unsigned long start_time;
		float savemph;

	public:
		demoSource(User *_user, Course *_course);
		~demoSource(void);
		virtual int getNextRecord(DWORD _delay);
		virtual void keydown(WPARAM wparam);
		virtual void start(void);
		virtual void reset(void);
		virtual void pause(void);
		virtual void resume(void);
};



