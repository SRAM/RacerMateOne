
#pragma once

#include <statictext.h>

/********************************************************************

********************************************************************/

class statusDlg  {

	public:
		statusDlg(char *_title, float _xp, float _yp);
		virtual ~statusDlg();
		void setText(char * _txt);

	private:
		#define MY_STATUS_DIALOG_CLASS "st_dlg_cls"
		staticText *text;
		HWND hwnd;
		HINSTANCE hInstance;
		HBRUSH bgbrush;
		static LRESULT CALLBACK proc(HWND hwnd, UINT uMsg, WPARAM wParam, LPARAM lParam);
		void cleanup(void);
		RECT clientrect;
		char *title;
		float xp;
		float yp;
		int bp;

};

