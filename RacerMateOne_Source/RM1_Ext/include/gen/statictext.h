
#pragma once

#include <windows.h>


class staticText  {

	private:
		staticText(const staticText&);
		HWND hwnd;
		HWND phwnd;
		HINSTANCE hinstance;
		float px;
		float py;
		float pw;
		float ph;
		char text[32];
		int x;
		int y;
		int w;
		int h;
		RECT pclientRect;
		HFONT hfont;
		COLORREF bgcolor;
		COLORREF textcolor;
		int id;
		HBRUSH hbrush;
		void cleanup(void);
		bool autoscale;
		LRESULT wmctlcolorstatic(LPARAM lParam, WPARAM wParam);
		HDC hdc;

	public:
		staticText(HWND _phwnd, char *_text, float _xOffsetPercentage, float _yOffsetPercentage, float _xPercentage, float _yPercentage, int _id, HFONT _hfont=0, COLORREF _bgcolor=RGB(0,0,0), COLORREF _textcolor=RGB(0,255,0));
		virtual ~staticText();
		void setText(char * _txt);
};


