#pragma once

#include <windows.h>
#include <stdio.h>
#include <datasource.h>


class MFD : public dataSource {

	friend class scrollingChart;

	private:
		void destruct(void);
		long nextrec;
		int lastdir;

	protected:
		int dir;			// 0 = forward, 1 = reverse, -1 = not selected
		DWORD lastnow;
		FILE *stream;
		char tmpfilename[256];		//tlm20040817
		char fname[256];
		unsigned long fileStartTime;
		unsigned long fileEndTime;
		float *array;
		bool enc;						// encryption flag

	public:
		MFD(char *stub, HWND _phwnd=NULL, bool _enc=true);
		virtual ~MFD(void);
		int get_nRecords(void);

		int seek(long rec);
		int getPreviousRecord(DWORD _delay);
		int read(int k);
		int getNextRecord(DWORD _delay);
		int getRecord(int k);
		long getOffset(void);
		unsigned long getFileEndTime(void)  {
			return fileEndTime;
		}

		virtual void reset(void);
		virtual int myexport(char *_fname);
		virtual void pause(void) {	return;}
		virtual void start(void)  {
			return; 
		}

};
