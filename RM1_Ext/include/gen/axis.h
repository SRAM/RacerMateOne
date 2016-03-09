#pragma once

#include <windows.h>

class Axis  {

	public:
		Axis(
			HDC _hdc, 
			RECT _waverect,								// the frame that the waveform is drawn in
			RECT _fullrect,
			float _fmin, 
			float _fmax,
			int _maxticks,
			COLORREF _labelColor,
			const char *_label,
			int _orientation,
			COLORREF _scaleColor
			);

		Axis(
			HDC _hdc, 
			RECT _waverect, 
			float _fmin, 
			float _fmax,
			int _maxticks,
			COLORREF _labelColor,
			const char *_label,
			int _orientation,
			COLORREF _scaleColor
			);

		virtual ~Axis();
		void draw(bool _printer, bool _grid, bool _integer_only, bool _leftyaxis, bool _force=false, int _precision=2);
		void draw(bool _printer);

		//void setscalemin(float _min);
		//void setscalemax(float _max);
		//double getMinScale(void)  {	return scalemin; }
		//double getMaxScale(void)  { return scalemax;}
		double getscalemin(void)  { return scalemin; }
		double getscalemax(void)  { return scalemax; }

		void setGridColor(COLORREF _gridcolor)  {
			gridcolor = _gridcolor;
			DeleteObject(gridpen);
			gridpen = CreatePen(PS_SOLID, 0, gridcolor );
			return;
		}

	private:
		//bool manual_scale;
		COLORREF gridcolor;
		HDC hdc;
		RECT waverect;							// waveform rectangle
		float fmin, fmax;
		//int imin, imax;
		float mx, bx;
		COLORREF scaleColor;
		COLORREF labelColor;
		int orientation;				// 0 = xaxis, 1 = yaxis
		void destroy(void);
		double scalemin;
		double scalemax;
		int actual;
		HPEN pen;
		HPEN gridpen;
		char gstr[256];
		const char *label;
		bool grid;
		int maxticks;
		int tickw, tickh;
		int screenw, screenh;
		RECT fullrect;							// same as clientrect
};



