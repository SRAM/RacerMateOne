
#pragma once

#include <windows.h>
#include <vector>

#include <modaldialog.h>
#include <button.h>

/***********************************************************************************

***********************************************************************************/

class colorSelector : public modalDialog  {

	public:
		typedef struct  {
			float x;
			float y;
			float w;
			float h;
			COLORREF color;
		} COLORBOX;

		colorSelector(std::vector<COLORBOX> &_boxes, int _default_selection, HWND _phwnd=NULL, const char *title="title", HICON _icon=0, int _x=100, int _y=100, int _w=400, int _h=400);
		virtual ~colorSelector();

		int get_selection(void)  { return selection; }

	private:
		virtual void wm_command(WPARAM wParam, LPARAM lParam);
		virtual void wm_keydown(WPARAM wParam, LPARAM lParam);
		virtual void wm_keyup(WPARAM wParam, LPARAM lParam);
		virtual void wm_user(WPARAM wParam, LPARAM lParam);
		virtual void wm_paint(WPARAM wParam, LPARAM lParam);

		enum  {
			BUTTON1,
			BUTTON2,
			LASTBUTTON
		};

		#define OK_BUTTON 2000
		#define CANCEL_BUTTON 2001

		int n;

		std::vector<COLORBOX> boxes;
		Button **button;
		RECT rb;							// rectangle occupied by the first radio button
		int selection;
		int original_selection;
		void seticon(void);

		Button *ok_button;
		Button *cancel_button;

		bool cancelled;


};


