
#ifndef _MYUTILS_H_
#define _MYUTILS_H_


/********************************************************************
	BEWARE: THERE IS A UTILS.H DEEP IN THE MICROSOFT SDKS!!!
	POSITION THE PATH THAT THIS ONE IS IN BEFORE THE MICROSOFT ONE
	OR RENAME THIS WHOLE FILE.
********************************************************************/

#pragma warning(disable:4101 4786 4065)

#include <windows.h>
#include <stdio.h>
#include <vector>

//------------------------------------------------
// these things work in both windows and unix
//------------------------------------------------

int indexIgnoreCase(const char *str1, const char *str2, int startpos=0, bool reverse=false);
bool exists(const char *fname);
void trimcrlf(char *string);
void ltrim(char *string, int trim_tabs=1, char *ends=NULL);
short map(float x1,float x2,float y1,float y2,float *m,float *b);
unsigned long filesize_from_name(const char *filename);
void strip_path(char *fname);

int pv(RECT, float);
int ph(RECT, float);
void center_window(HWND hwnd);
void box(HDC hdc, RECT r, COLORREF=RGB(128, 128, 128), bool fill=false, HPEN pen=0);
char *MessageName(UINT msg);
void strip_filename(char *fname);
void strip_extension(char *fname);
bool rm(const char *fname);
bool is_open(const char *fname);
bool direxists(const char *path);
void explode(char *str, const char *sep, std::vector<std::string> &a);
void strtolower(char *str);
void SetRValue( COLORREF* pColor, BYTE value );
void SetGValue( COLORREF* pColor, BYTE value );
void SetBValue( COLORREF* pColor, BYTE value );
float frand(float min, float max);
void delay(DWORD ms);
void convtime(DWORD ms, int &h, int &m, int &s, int &ts);
void time_format(DWORD tms, char *str);
short dmap(double x1,double x2,double y1,double y2,double *m,double *b);
unsigned short getRandomNumber(void);
int replace(char *string, const char *oldstr, const char *newstr, int flag);
int myindex(char *str1, const char *str2, int startpos );

#endif		// #ifndef _UTILS_H_

