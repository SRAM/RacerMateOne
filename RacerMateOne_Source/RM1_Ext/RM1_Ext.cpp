// RM1_Ext.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"

#define TESTING TRUE
#if (!TESTING)
#define DO_WIN 
#endif
extern bool testing;
//#define DO_WIN 

#ifdef D3DEXE
using namespace System;
static D3DBase one(false);


Tester Test[8];
extern int scenecnt;

#ifdef DO_WIN
INT WINAPI WinMain( HINSTANCE hInst, HINSTANCE, LPSTR strCmdLine, INT )
#else
int main(int argc, char* argv[])
#endif
{
	UINT w,h;
	testing = TESTING;

	camtype = 0; 
	numViews = 0;
	numriders = 0;
	large = true; 

	_unlink("BikeMsg.log");
    //Console::WriteLine(L"Hello World");
	// draw everything
	D3DBase::Create(false);
	D3DBase::GetFullWindowSize(w,h);
	Test[0].Init(7,0,0,w,h);

	//Test[0].Init(5,0,0,w/2,h);
	//Test[1].Init(6,w/2,0,w/2,h);
/*
	Test[0].Init(5,0,0,w/2,h/2);
	Test[1].Init(6,w/2,0,w/2,h/2);
	Test[2].Init(7,0,h/2,w/2,h);
	Test[3].Init(8,w/2,h/2,w/2,h);
*/
	numtest=0;
//	D3DBase::ParseSysCmds("Demo 1");

//	for(int i = 0; i< scenecnt; i++)
//	{
//		char *ptr = Test[0].GetSceneryName(i);
//		strcpy(gScratchBuf,ptr);
//	}
	/*
	static bool once = false;
	if(!once)
	{
		Test[0].ShowDemoRider(true);
		once = true;
	}
	*/


	while(D3DBase::GetDevice()->run() && D3DBase::GetDriver())
	{
		if (D3DBase::GetDevice()->isWindowActive())
		{
			//if(!gpCourse)
			//	D3DBase::LoadCourse();
			D3DBase::Render2();
		}
		else
			D3DBase::GetDevice()->yield();
	}
	D3DBase::Destroy();
	return 0;
}
#endif

#define LOGIT
void DebugLog(CHAR *szMsg)
{
	return;

    HANDLE hAppend;
    CHAR buff[MAX_PATH]="BikeMsg.log";       // directory buffer
    hAppend = CreateFileA(buff,   // open TWO.TXT 
        GENERIC_WRITE,                // open for writing 
        0,                            // do not share 
        NULL,                         // no security 
        OPEN_ALWAYS,                  // open or create 
        FILE_ATTRIBUTE_NORMAL,        // normal file 
        NULL);                        // no attr. template 

    if (hAppend != INVALID_HANDLE_VALUE) 
    { 
        DWORD  dwBytesRead, dwBytesWritten, dwPos; 
        //SetEndOfFile(hAppend);
        dwBytesRead = lstrlenA(szMsg);
        dwPos = SetFilePointer(hAppend, 0, NULL, FILE_END); 
        WriteFile(hAppend, szMsg, dwBytesRead, &dwBytesWritten, NULL); 
        CloseHandle(hAppend);
    }
}

//-----------------------------------------------------------------------------
// Name: Msg(char *szFormat, ...)
// Desc: Debug message output
//-----------------------------------------------------------------------------
void Msg(CHAR *szFormat, ...)
{
	return;

    CHAR szBuffer[1024];  // Large buffer for long filenames or URLs
    const size_t NUMCHARS = sizeof(szBuffer) / sizeof(szBuffer[0]);
    const int LASTCHAR = NUMCHARS - 1;

    // Format the input string
    va_list pArgs;
    va_start(pArgs, szFormat);

    // Use a bounded buffer size to prevent buffer overruns.  Limit count to
    // character size minus one to allow for a NULL terminating character.
    _vsnprintf(szBuffer, NUMCHARS - 1, szFormat, pArgs);
    va_end(pArgs);

    // Ensure that the formatted string is NULL-terminated
    szBuffer[LASTCHAR] = TEXT('\0');

#ifdef LOGIT
    lstrcatA(szBuffer,"\r\n");
    DebugLog(szBuffer);
#else
    OutputDebugString(szBuffer);
    OutputDebugString("\n");
#endif
}

/******************************************************************************

******************************************************************************/

char * Strip(char *buf)  {
	while (core::isspace(*buf))
		buf++;
	char *ps = buf + strlen(buf) - 1;
	while (ps > buf)  {
		if (core::isspace(*ps))
			*ps-- = '\0';
		else
			break;
	}
	return buf;
}

/******************************************************************************

******************************************************************************/

bool LinesIntersect(float x1,float y1,float x2,float y2,float x3,float y3,float x4,float y4)  {
	float Ax,Bx,Cx,Ay,By,Cy,d,e,f;
	float x1lo,x1hi,y1lo,y1hi;

	Ax = x2-x1;
	Bx = x3-x4;

	if(Ax<0) {						/* X bound box test*/
		x1lo= x2; x1hi= x1;
	}
	else {
		x1hi= x2; x1lo= x1;
	}
	if(Bx>0) {
		if(x1hi <  x4 ||  x3 < x1lo)
			return false;
	}
	else {
		if(x1hi <  x3 ||  x4 < x1lo)
			return false;
	}

	Ay = y2-y1;
	By = y3-y4;

	if(Ay<0) {						/* Y bound box test*/
		y1lo= y2; y1hi= y1;
	}
	else {
		y1hi= y2; y1lo= y1;
	}
	if(By>0) {
		if(y1hi <  y4 ||  y3 < y1lo)
			return false;
	}
	else {
		if(y1hi <  y3 ||  y4 < y1lo)
			return false;
	}

	Cx = x1-x3;
	Cy = y1-y3;
	d = By*Cx - Bx*Cy;					/* alpha numerator*/
	f = Ay*Bx - Ax*By;					/* both denominator*/
	if(f>0) {								/* alpha tests*/
		if(d<0 || d>f)
			return false;
	}
	else {
		if(d>0 || d<f)
			return false;
	}

	e = Ax*Cy - Ay*Cx;					/* beta numerator*/
	if(f>0) {								/* beta tests*/
		if(e<0 || e>f) return false;
	}
	else {
		if(e>0 || e<f) return false;
	}

	return true;
}

/******************************************************************************

******************************************************************************/

char * GetArg( char *&ps )  {
	char *str;
	char *arg = NULL;

	str = ps;

	while( core::isspace(*str) ) str++;

	if ((*str == '\0') || (*str == '\n') || (*str == '\r')) 
		return NULL;

	if (*str == '"') {
		*str++;
		arg = str;
		while(*str != '"')
			if (*str++ == '\0')  {
				ps = str - 1;
				return(arg);
			}
		ps = str+1;
	}
	else {
		arg = str;

		while( (!core::isspace(*str)) && (*str != '\n') && (*str != '\r'))  {
			if (*str++ == '\0') {
				ps = str - 1;
				return(arg);
			}
		}
		ps = str + 1;
	}
	*str++ = '\0';
	return(arg);
}

/******************************************************************************

******************************************************************************/

int ParseLine(char * wstring, int maxarg, char **argarr)  {
	int argc = 0;
	char **argv = argarr;

	if (maxarg > 0)
		*argv = NULL;

	while( maxarg-- > 1)  {
		*argv = NULL;

		while(core::isspace( *wstring ))
			wstring++;

		if (*wstring == '\0')
			break;

		*argv = GetArg( wstring );

		if (*argv == NULL)
			break;

		argv++;
		argc++;
	}

	return argc;
}

// ect-todo fix this function
short mapvalues(float x1,float x2,float y1,float y2,float *m,float *b) 
{
	float t1 = y2 - y1;
	float t2 = x2 - x1;

	*m = t1 / t2;
	*b = 0;
	return 0;
}

/*
void strip_path(char *fname){}
void strip_filename(char *fname){}
void strip_extension(char *fname){}
unsigned long filesize_from_name(const char *filename){return 0L;}
//float frand(float min, float max){return 0.0f;}
void convtime(DWORD ms, int &h, int &m, int &s, int &ts){}

int indexIgnoreCase(const char *str1, const char *str2, int startpos, bool reverse){
	int result = -1,i;
	if(startpos > -1)
	{
		const char *pstr = str1 + startpos;
		int len1 = strlen(pstr);
		int len2 = strlen(str2);
		if(len1 >= len2)
		{
			if(reverse)
			{
				for (i = (len1 - len2); i >= startpos ; i--)
				{
					pstr = str1 + i;
					if (0 == _strnicmp(pstr,str2,len2))
					{
						result = i;
						break;
					}
				}
			}
			else
			{
				for (i = startpos; i < (len1 - len2); i++)
				{
					pstr = str1 + i;
					if (0 == _strnicmp(pstr,str2,len2))
					{
						result = i;
						break;
					}
				}
			}
		}
	}
	return result;
}
*/

/************************************************************************************

************************************************************************************/
/*
float frand(float min, float max)  {
	return (max - min) * ((float) rand() / RAND_MAX) + min;
}
*/

/******************************************************************************

******************************************************************************/

float NormalizeRotation(float deg)  {

	f64 d = fmod((f64)deg, (f64)PI*2);

	if (d > PI)
		d -= PI*2;
	else if (d < -PI)
		d += PI*2;

	return (float)d;
}

//-----------------------------------------------------------------------------
// Name: _DbgOut()
// Desc: Outputs a message to the debug stream
//-----------------------------------------------------------------------------
HRESULT _DbgOut( char* strFile, DWORD dwLine, HRESULT hr, char* strMsg )
{
	return hr;

    CHAR buffer[1024];
    wsprintfA( buffer, "%s(%ld): (hr=%08lx) %s\n", strFile, dwLine, hr, strMsg );

#ifdef LOGIT
    DebugLog( buffer );
#else
    OutputDebugString( buffer );
#endif
    return hr;
}

float MoveTo(float org, float dest, float amount)  {
	if (org < dest)  {
		if ((org += amount) > dest)
			return dest;
	}
	else if (org > dest)  {
		if ((org -= amount) < dest)
			return dest;
	}
	return org;
}

float AngleDiff(float a1, float a2)  {
	#define PI360	(PI*2)
	#define PI90	(PI/2)

	a1 = (float) fmod((f64)a1, (f64)PI360);
	a2 = (float) fmod((f64)a2, (f64)PI360);
	return (float) fmod((f64)(a1 - a2), (f64)PI360);
/*
	a1 = (float) fmod((f64)a1, (f64)PI360);
	a2 = (float) fmod((f64)a2, (f64)PI360);
	if (a1 < -PI)
		a1 += (float)PI360;
	else if (a1 > PI)
		a1 -= (float)PI360;
	if (a2 < -PI)
		a2 += (float)PI360;
	else if (a2 > PI)
		a2 -= (float)PI360;

	if ((a1 < -PI90) && (a2 > PI90))
		return (float)(a1 + PI360 - a2);
	else if ((a1 > PI90) && (a2 < -PI90))
		return (float)(a1 - PI360 - a2);
	return (a1 - a2);
*/
}

// s is string passed in, ns is new string returned by ref. with commas inserted
void addCommas( const core::stringw& s, core::stringw& ns )
{
	core::stringw t;
    int j, k, topI = s.size()-1;
    do
    {
        for( j = topI, k = 0; j >= 0 && k < 3; --j, ++k )
        {
			t = ns; ns = s.subString(j,1); ns += t; // new char is added to front of ns
			if( j > 0 && k == 2)
			{
				t = ns; ns = L","; ns += t; // j > 0 means still more digits
			}
        }
        topI -= 3;
       
    }while( topI >= 0 );
}

