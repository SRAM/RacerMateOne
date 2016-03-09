#pragma once

#include <ddraw.h>		// needed to compile in win98 on laptop!!

typedef struct  {
	BOOL flag;
	char label[24];
} EXPORT_TYPE;

typedef struct  {
    RGBQUAD depth;
    RGBQUAD Amount;
    RGBQUAD Position;
} RGB16;


typedef struct  {
	float length;
	float grade;
	float wind;
	float length_so_far;
	float startel;
	float endel;
} LEG;


typedef struct  {
	float x;
	float y;
	float xval;
	float raw;
	float scaled;
} DATUM;

typedef struct {
	int left;
	int bottom;
	int right;
	int top;	
} MYRECT;


typedef struct tagCPUINFO  {
	DWORD CpuID;
	char ProcType, Family, Model, Stepping, MMX;
	char CpuName[13];
} CPUINFO, *LPCPUINFO;



typedef struct  {
	int Count;
	HWND hWindow;
} ENUMSTRUCT;

typedef struct {
	LPGUID lpGUID;
	GUID guid;
	char DriverName[64];
	char DriverDesc[64];
} DDDeviceList;


typedef struct {
	unsigned short power;
	unsigned short speed;
	unsigned short hr;
	unsigned short rpm;

	unsigned short lastpower;
	unsigned short lastspeed;
	unsigned short lasthr;
	unsigned short lastrpm;

	RECT powerRect;
	int powerx;
	int powery;

} PEAKS;

typedef struct {
	float power;
	float speed;
	float hr;
	float rpm;
} AVERAGES;

