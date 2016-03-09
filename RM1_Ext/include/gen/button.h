
#ifndef _BUTTON_H_
#define _BUTTON_H_

	#include <windows.h>

	class Button  {

		private:
			HDC hdc;
			HFONT font;
			int pw;
			int type;
			int bp;
			RECT pclientRect;

			void init(
					HWND _phwnd, 
					float _xOffsetPercentage,
					float _yOffsetPercentage,
					float _xPercentage,
					float _yPercentage,
					int _id,
					int _type);

			char title[64];
			Button(const Button&);
			WNDPROC oldproc;
			static LRESULT CALLBACK proc(HWND hwnd, UINT msg, WPARAM wParam, LPARAM lParam);
			bool keydown;
			bool ownerdraw;
			void compute_colors(void);

		protected:
			HWND phwnd;
			HINSTANCE hinstance;
			float xOffsetPercentage;
			float yOffsetPercentage;
			float xPercentage;
			float yPercentage;
			int id;
			int x;
			int y;
			int w;
			int h;

			void computeSize(void);
			COLORREF bgcolor;
			COLORREF framecolor;
			COLORREF textcolor;
			HBRUSH bgbrush;
			HBRUSH framebrush;
			HPEN pen1;
			HPEN pen2;
			HPEN pen3;
			HPEN pen4;
			COLORREF color1;
			COLORREF color2;
			COLORREF color3;
			COLORREF color4;

		public:

			enum  {
				BUTTON_REGULAR,
				BUTTON_RADIO,
				BUTTON_CHECKBOX
			};


			Button(
			   HWND _phwnd, 
				char *_title,
				float _xOffsetPercentage,
				float _yOffsetPercentage,
				float _xPercentage,			// width in percentage of client area
				float _yPercentage,			// heigth in percentage of client area
				int _id,
				int _type=BUTTON_REGULAR,
				bool _ownerdraw=false);


			HWND hwnd;
			void OnCreate(void);
			void OnPaint(void);
			virtual ~Button();
			void drawBitmap(LPARAM lParam);
			void resize(void);
			void onMove(void);
			bool getchecked(void);
			void setchecked(bool _b);
			void setLabel(const char *_label);
			void set_bg_color(COLORREF _color);
			void set_fg_color(COLORREF _color);
			void set_text_color(COLORREF _color);
			LRESULT drawitem(WPARAM wparam, LPARAM lparam);
			inline HWND gethwnd(void)  { return hwnd; }

			float get_xp(void)  { return xOffsetPercentage;}
			float get_yp(void)  { return yOffsetPercentage;}
			float get_wp(void)  { return xPercentage;}
			float get_hp(void)  { return yPercentage; }

			HWND get_hwnd(void)  { return hwnd; }
			LRESULT wmctlcolorbtn(WPARAM wparam, LPARAM lparam);

	};

#endif

