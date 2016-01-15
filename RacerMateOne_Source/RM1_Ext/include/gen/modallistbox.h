
#pragma once

#include <vector>
#include <string>

#include <modaldialog.h>

/********************************************************************

********************************************************************/

class modalListBox : public modalDialog  {

	public:
		modalListBox(HWND _phwnd, char *_title, const std::vector<std::string> &str, int _x=0, int _y=0, int _w=0, int _h=0);
		virtual ~modalListBox();

	private:
		#define IDC_LISTBOX 22
		HWND hlist;
		HDC listhdc;

		virtual void wm_command(WPARAM wParam, LPARAM lParam);
		virtual void wm_keydown(WPARAM wParam, LPARAM lParam);
		virtual void wm_keyup(WPARAM wParam, LPARAM lParam);
		virtual void wm_user(WPARAM wParam, LPARAM lParam);

};

