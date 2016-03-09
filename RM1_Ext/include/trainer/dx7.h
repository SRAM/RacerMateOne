
#pragma once

#include <windows.h>

#include <config.h>


#ifndef VIDEO_APP
#include <d3d.h>

#include <logger.h>

HRESULT DDSetColorKey(LPDIRECT3DDEVICE7 _pd3dDevice, LPDIRECTDRAWSURFACE7 surface, COLORREF rgb);
HRESULT DDSetColorKey(LPDIRECTDRAWSURFACE7 surface, COLORREF rgb, LPDIRECTDRAWSURFACE7 pddsPrimary);

DWORD colorMatch(LPDIRECT3DDEVICE7 pd3dDevice, COLORREF rgb);
HRESULT colorMatch(LPDIRECTDRAWSURFACE7 pddsPrimary, COLORREF rgb, DWORD *dw);
DWORD colorMatch(LPDIRECTDRAWSURFACE7 pddsPrimary, COLORREF rgb);

void TraceErrorDD(HRESULT hErr, char *sFile, int nLine);
void TraceErrorDD(HRESULT hErr, char *sFile, int nLine, Logger *log);
void TraceErrorD3D(HRESULT hErr, char *sFile, int nLine);

HRESULT fillRect(LPDIRECTDRAWSURFACE7 dest, RECT r, DWORD color);


HRESULT DDCopyBitmap(IDirectDrawSurface7 *pdds, HBITMAP hbm, int x, int y, int dx, int dy);

IDirectDrawSurface7 * DDLoadBitmap(
	IDirectDraw7 *pdd,
	LPCSTR szBitmap,
	int dx,
	int dy,
	int CAPFLAG);

FLOAT* CreatePlane( FLOAT* plane, D3DVECTOR& a, D3DVECTOR& b, D3DVECTOR& c );
BOOL WINAPI DDEnumCallback(GUID *pGUID, LPSTR pDescription, LPSTR pName, LPVOID context);
BOOL WINAPI DDEnumCallbackEx(GUID *pGUID, LPSTR pDescription, LPSTR pName, LPVOID pContext, HMONITOR hm);

void log_ddsd(char *str, DDSURFACEDESC2 ddsd, Logger *logg);
void log_ddpixelformat(DDPIXELFORMAT ddpf, Logger *logg);
void log_ddcolorkey(DDCOLORKEY ck, Logger *log);
void log_ddscaps(DDSCAPS2 ddsd, Logger *logg);
void getdeviceinfo(Logger *logg);
int getrefcount(LPDIRECTDRAW7 pDD);

#define MAX_DEVICES     16

#endif				// #ifndef VIDEO_APP
