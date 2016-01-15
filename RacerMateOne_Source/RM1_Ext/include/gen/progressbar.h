#pragma once

#include <windows.h>

#define COLOUR_BAR    RGB(0,0,255)
#define COLOUR_TEXT    RGB(255,255,255)


class progressBar  {

	private:
		HWND hwnd;
		HWND phwnd;
		HINSTANCE hInstance;
		char classname[64];
		char windowname[64];
		void init(void);
		COLORREF bg;
		COLORREF fg;
		int min;
		int max;
		//float m, b;
		progressBar(const progressBar&);

	public:
		class progressBar(char *_classname, char *_windowname, COLORREF bg, COLORREF fg);
		class progressBar(HWND _phwnd, COLORREF bg, COLORREF fg, int _min=0, int _max=100);

		~progressBar(void);
		void draw(int);
};



