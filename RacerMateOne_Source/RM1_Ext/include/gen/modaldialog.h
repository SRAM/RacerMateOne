
#pragma once

#include <windows.h>

#include <defines.h>

/**********************************************************************************************************
	parent class for general-purpose modal dialogs
**********************************************************************************************************/

 class modalDialog  {

	public:
		modalDialog(HWND _phwnd, const char *_title, HICON _icon=0, int _x=0, int _y=0, int _w=0, int _h=0, bool _realtime=false);
		virtual ~modalDialog();
		bool getChanged(void)  { return changed; }
		virtual void mainloop(void);

	protected:
		HWND phwnd;
		HWND hwnd;
		HDC hdc;
		int bp;
		RECT clientrect;
		bool keydown;
		HINSTANCE hInstance;
		COLORREF bgcolor;
		HICON icon;
		bool realtime;
		int cx, cy;

		// necessary interface for subclasses:

		virtual void wm_command(WPARAM wParam, LPARAM lParam) {
			bp = 1;
			return;
		}

		virtual int wm_close(WPARAM wp, LPARAM lp) {
			bp = 1;
			return 1;
		}

		virtual void wm_user(WPARAM wParam, LPARAM lParam) {
			bp = 1;
			return;
		}

		virtual void wm_keydown(WPARAM wParam, LPARAM lParam) {
			bp = 1;
			return;
		}

		virtual void wm_keyup(WPARAM wParam, LPARAM lParam) {
			bp = 1;
			return;
		}

		virtual void wm_char(WPARAM wparam, LPARAM lparam) {
			bp = 1;
			return;
		}

		virtual void wm_paint(WPARAM wParam, LPARAM lParam) {
			bp = 1;
			return;
		}

		bool changed;

	private:
		#define MY_DIALOG_CLASS "dlg_cls"

		static LRESULT CALLBACK proc(HWND hwnd, UINT uMsg, WPARAM wParam, LPARAM lParam);
		void cleanup(void);
		HBRUSH bgbrush;
		char title[32];
		int x, y, w, h;
};

