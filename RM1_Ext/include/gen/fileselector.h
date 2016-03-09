#pragma once

#include <windows.h>
#include <stdlib.h>
#include <commdlg.h>


class fileSelector  {

	private:
		HWND hwnd;
		char title[256];
		char deftype[256];
		char filter[256];
		char curdir[256];
		char default_file[256];
		char fname[_MAX_PATH];
		char path[_MAX_PATH];
		char szFileTitle[_MAX_FNAME+_MAX_EXT];
		char datapath[_MAX_PATH];
		int idx;

		BOOL openflag;
		OPENFILENAME ofn;
		fileSelector(const fileSelector&);
		fileSelector &operator = (const fileSelector&);		// unimplemented

	public:

		fileSelector(
			HWND _hwnd, 
			char *_title, 
			char *_f, 
			const char *_path,			// default path
			char *_deftype,
			int _idx,				// 1-based filter index
			BOOL _openflag, 
			char *_default_file);

		virtual ~fileSelector();
		char * getSelectedFileName();
		char * getfname();
		char * getpath();
		bool cancelled;

};


