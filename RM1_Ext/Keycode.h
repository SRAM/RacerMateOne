/* ----------------------------------------------------------------------------
 * $Workfile: $
 * $Revision: 1.1 $
 *
 * $Author: EdgarCT-PC\Edgar $
 * $Date: 2008/06/27 07:01:14 $
 * ----------------------------------------------------------------------------
 * //Log: $
 * ----------------------------------------------------------------------------
 * This information is CONFIDENTIAL and PROPRIETARY
 * Copyright (C) 2001, Prolific Publishing, Inc.
 * All rights reserved.
 *
 * $Nokeywords: $
 */
#ifndef __KEYCODE_H
#define __KEYCODE_H

#include <stdlib.h>
#include <crtdbg.h>
#include <windows.h>
#include <time.h>     // sscanf()
#include "Common.h"
extern const char *gSSDate;

// NOTE: All these are inlines so that they will compile and merge with the rest of the
// code.
//
// Please make sure inline optimizations are on as this will help hide all this code.
//
#ifndef KEYCODE_NUM_CDKEYS
	#error KEYCODE_NUM_CDKEYS needs to be defined.
#endif

#ifndef KEYCODE_CD_FILESIZE
	#error KEYCODE_CD_FILESIZE needs to be defined.
#endif

#ifndef KEYCODE_CD_VOLUME_NAME
	#error KEYCODE_CD_VOLUME_NAME needs to be defined.
#endif

//=============================================================================
// inline unsigned short KeycodeCRC( const char *str, int cnt = 0 )
// 
// Calculates the CRC for the verious parts of the CRC - if cnt == 0 the 
// entire string is used.
//=============================================================================
inline unsigned short KeycodeCRC( const char *str, int cnt = 0 )
{
	unsigned short crc = 0;
	if (cnt == 0)
	{
		const char *t = str;
		while(*t++ != '\0')
			cnt++;
	}

	while(cnt-- > 0)
	{
		crc  = (unsigned char)(crc >> 8) | (crc << 8);
		crc ^= *str++;
		crc ^= (unsigned char)(crc & 0xff) >> 4;
		crc ^= (crc << 8) << 4;
		crc ^= ((crc & 0xff) << 4) << 1;
	}
	return crc;
}

//=============================================================================
// inline unsigned short CalcCRC( const char *str, int cnt = 0 )
// 
// Calculates the CRC of the entire string
//=============================================================================
inline unsigned short CalcCRC( const char *str, int cnt = 0 )
{
    unsigned short crc = 0;
	if (cnt == 0)
	{
		const char *t = str;
		while(*t++ != '\0')
			cnt++;
	}
	while(cnt-- > 0)
	{
		char v = toupper(*str++);
        crc = ((((int(crc / 256)) & 255) | (crc * 256)) & 65535);
        crc ^= int(v);
        crc ^= (int((crc & 255) / 16));
        crc ^= ((crc * 4096) & 65535);
        crc ^= (((crc & 255) * 32) & 65535);
	}
	return crc;
}

//=============================================================================
// inline unsigned short CalcCRC( const char *str, int cnt = 0 )
// 
// Calculates the CRC of the entire string
//=============================================================================
inline unsigned short CalcCRCAlphaNumeric( const char *str, int cnt = 0 )
{
    unsigned short crc = 0;
	if (cnt == 0)
	{
		const char *t = str;
		while(*t++ != '\0')
			cnt++;
	}
	while(cnt-- > 0)
	{
		char v = toupper(*str++);
		if((v >= 'A' && v <= 'Z') || (v >= '0' &&  v <= '9'))
		{
			crc = ((((int(crc / 256)) & 255) | (crc * 256)) & 65535);
			crc ^= int(v);
			crc ^= (int((crc & 255) / 16));
			crc ^= ((crc * 4096) & 65535);
			crc ^= (((crc & 255) * 32) & 65535);
		}
	}
	return crc;
}

inline void CalcCRCToString( char *pout, const char *str, int cnt = 0 )
{
	char tbuf[32];
	char digit[2];
	digit[1] = '\0';
	_ltoa( CalcCRCAlphaNumeric(str, cnt), tbuf, 10 );
		
	unsigned char b = 'A';
	char *t = tbuf;
	while(*t != '\0')
	{
		digit[0] = *t;
		int v = atoi(digit);
		v += b;
		while(v > 'Z')
			v -= 26;
		b = v;
		*t++ = b;
	}
	cnt = lstrlenA(tbuf);
	if(5 > cnt)
	{
		while(cnt < 5)
		{
			tbuf[cnt++] = 'A';
		}
		tbuf[cnt++] = '\0';
	}
	lstrcpyA(pout,tbuf);
}

//=============================================================================
// inline int KeycodeToNum( const char *str, int cnt )
// 
// Takes part of the "BINARY" string and converts it to a number.
//=============================================================================
inline int KeycodeToNum( const char *str, int cnt )
{
	int num = 0;
	while( cnt-- > 0)
	{
		num *= 2;
		if (*str++ == '1')
			num++;
	}
	return num;
}


//=============================================================================
// inline bool KeycodeConvert( const char *ps, char *pout )
// 
// Converts the Text version of the string to the "BINARY" version
// Text version must be 20 characters - no spaces no dash.  Can be upper and
// lower case.
//
// The "pout" must be at least 110 characters long.
//=============================================================================
inline bool KeycodeConvert( const char *ps, char *pout )
{
	const char *psave = ps;
	int i, corek;
	for(i=0, corek=0;i<20;i++,ps++)
	{
		unsigned char c = *(const unsigned char *)ps;
		unsigned char num;

        // Parses out "COREK" as prefix to Keycode, a letter at a time to hide it from code
        if(i==0 && (c=='c' || c=='C'))
            corek++;
        else
        if(i==2 && (c=='r' || c=='R'))
            corek++;
        else
        if(i==4 && (c=='k' || c=='K'))
            corek++;
        else
        if(i==1 && (c=='o' || c=='O'))
            corek++;
        else
        if(i==3 && (c=='e' || c=='E'))
            corek++;
        else
        if(corek==5)
        {
            // do nothing for now, not used here
//            return false;
        }

		if ((c >= 'a') && (c <= 'z'))
		{
			num = c - 'a' + 6;
		}
		else if ((c >= 'A') && (c <= 'Z'))
			num = c - 'A' + 6;
		else if ((c >= '2') && (c <= '7'))
			num = c - '2';
		else
			return false;

		unsigned char t = 0x10;
		for(int j=0;j<5;j++,t>>=1)
		{
			*pout++ = (t & num ? '1':'0');
		}
	}
	for(i=0;i<5;i++)
	{
		if ((*psave >= 'a') && (*psave <= 'z'))
			*pout++ = (*psave++ - 'a') + 'A';
		else
			*pout++ = *psave++;
	}
	*pout = '\0';
	return true;
}


//=============================================================================
// inline int KeycodeGetMin( const char *sbuf )
// 
// Gets the Minutes since created number of the "BINARY" string
//=============================================================================
inline int KeycodeGetMin( const char *sbuf )
{
	unsigned int m = KeycodeToNum(sbuf+33,30);
	unsigned int c = KeycodeCRC( sbuf+100,5 );

	c = ((c & 0x3fff) * 65536) + c;
	unsigned int r = m ^ c;
	return r;
}

//=============================================================================
// inline int KeycodeGetMinCRC( const char *sbuf )
// 
// Gets the Minute CRC out of the "BINARY" string
//=============================================================================
inline int KeycodeGetMinCRC( const char *sbuf )
{
	char tbuf[32];
	return KeycodeCRC( _ltoa( KeycodeGetMin(sbuf), tbuf, 10 ) );
}


//=============================================================================
// inline bool PackKeycode( char *ps )
// 
// 
//=============================================================================
inline bool PackKeycode( char *ps )
{
	char *pout = ps;
	int cnt = 0;
	for(;(*ps != '\0');ps++)
	{
		if (*ps == '0')
			*ps = 'o';
		if (*ps == '1')
			*ps = 'l';
		if (((*ps >= 'a') && (*ps <= 'z')) || ((*ps >= 'A') && (*ps <= 'Z')) || ((*ps >= '2') && (*ps <= '7')))
		{
			cnt++;
			if (pout != ps)
				*pout = *ps;
			pout++;
		}
	}
	*pout = '\0';
	return (cnt == 20);
}

//=============================================================================
// inline bool MungKeycode( char *sbuf, const char *rootdir )
// 
// 
//=============================================================================
inline bool MungKeycode( char *csbuf, bool fixed=false )
{
    BYTE *sbuf = (BYTE *)csbuf;
	char rootdir[MAX_PATH];
	GetWindowsDirectoryA( rootdir, MAX_PATH );
	rootdir[3] = '\0';
	DWORD sn,clen,flags;
    // use 1st 4 char of volume name as sn
    if(fixed)
    {
        sn = MAGICMUNGENUMBER;
    }
    else
    {
	    if (!GetVolumeInformationA( rootdir, NULL, 0, &sn, &clen, &flags, NULL, 0 ))
		    return false;
    }
	srand( sn );
	for(int i=0;i<20;i++)
	{
		BYTE t = (BYTE)(rand() & 0xff);
		*sbuf++ ^= t;
	}
	*sbuf = '\0';
	return true;
}


//=============================================================================
// inline void KeycodeReadySecCheck( int base )
// 
// 
//=============================================================================
inline void KeycodeReadySecCheck( int base )
{
	char buf[32];
	srand( (KeycodeCRC( _ltoa( base, buf, 20 ) ) << 16) | (base >> 8) );
}

//=============================================================================
// inline int KeycodeGetKeyLoc( int maxsize, int num = -1 )
// 
// 
//=============================================================================
inline int KeycodeGetKeyLoc( int num = -1, int numcdkeys=KEYCODE_NUM_CDKEYS, int cdfilesize=KEYCODE_CD_FILESIZE )
{
	if (num < 0)
	{
		srand( GetTickCount() );
		num = rand() % numcdkeys;
	}
	else if (num >= numcdkeys)
		return -1;

	srand(411);
	while(num-- > 0)
	{
		rand();
	}
	int first = rand();
	return (int)(((first << 16) | rand()) % ((cdfilesize-256)/256)) * 256;
}




#define KEYCODE_CRC_OK(sbuf)		(KeycodeCRC((sbuf),84) == KeycodeToNum((sbuf)+84,16))
#define KEYCODE_GETID(sbuf)		KeycodeToNum((sbuf)+25,8)

#define KEYCODE_MINOK_1(sbuf)		((KeycodeGetMin(sbuf) % 11) != 2)
#define KEYCODE_MINOK_2(sbuf)		((KeycodeGetMin(sbuf) %  9) != 5)
#define KEYCODE_MINOK_3(sbuf)		((KeycodeGetMinCRC(sbuf) %  5) != 3)
#define KEYCODE_MINOK_4(sbuf)		((KeycodeGetMinCRC(sbuf) %  7) != 5)
#define KEYCODE_MINOK_5(sbuf)		((KeycodeGetMinCRC(sbuf) % 13) != 11)

// added for upgrade path.
inline int Keycode_GetVer( char *sbuf )
{
	int num = KeycodeToNum((sbuf) + 63,7);
	return (num >= 100 ? num-100:num);
}

#define KEYCODE_GETVER(sbuf)		Keycode_GetVer( sbuf )			// SerialToNum((sbuf)+63,7)
#define KEYCODE_GETCOPIES(sbuf)	KeycodeToNum((sbuf)+70,8)

#define KEYCODE_GETLICENCE(sbuf)	KeycodeToNum((sbuf)+78,3)

// added for upgrade path.
inline int Keycode_IsUpgrade( char *sbuf )
{
	int num = KeycodeToNum((sbuf) + 63,7);
	return (num >= 100);
}
#define KEYCODE_ISUPGRADE(sbuf)		Keycode_IsUpgrade( sbuf )			// SerialToNum((sbuf)+63,7)

inline bool KeycodeCheckRandomPoint( HANDLE hfile,int cdfilesize=KEYCODE_CD_FILESIZE )
{
	DWORD loc = (rand() % (cdfilesize-256)) & 0xffffff00;
	int cnt = (rand() % 128) + 32;
	unsigned char buf[256];
	if (SetFilePointer( hfile,loc, NULL, FILE_BEGIN ) == loc)
	{
		DWORD r;
		if (ReadFile( hfile,buf, 256, &r, NULL ))
		{
			KeycodeReadySecCheck( loc );
			int i;
			for(i=0;i<256;i++)
			{
				buf[i] ^= (rand() & 0xff);
			}
			for(i=cnt;i<cnt+20;i++)
			{
				if (buf[i] != i)
					return false;
			}
			return true;
		}
	}
	return false;
}

bool KeycodeCDCheck( char *serial );
bool KeycodeCDCheck_V1();
//=============================================================================
// inline bool KeycodeCDCheck( char *serial )
// 
//=============================================================================
inline bool KeycodeCDCheckX( char *serial, 
                           char *cdvolname=KEYCODE_CD_VOLUME_NAME, 
                           char *cddatname="c:\\" CD_DAT_NAME,
                           DWORD numcdkeys=KEYCODE_NUM_CDKEYS,
                           DWORD cdfilesize=KEYCODE_CD_FILESIZE
                           )
{
// this is for development only, bypasses protection for now
// all CHEAT need to be removed later
#ifdef CHEAT
#ifndef CORP_LOGO
    lstrcpy(serial,"CD27225O2JKDC2267HPE");
#else
    lstrcpy(serial,"ACRYS242SVKOK3262H32");
#endif
    return true;
#endif

	// Go through and find all the CD Roms
	int cnt = 0;
	char c;
	char buf[256];
	strcpy( buf, "c:\\" );
	for(c = 'A';c <= 'Z';c++)
	{
		buf[0] = c;
		if ( GetDriveTypeA( buf ) ==	DRIVE_CDROM)
		{
			char vol[256];
			DWORD volserial,volmaxlen;
			if (GetVolumeInformationA( buf, vol, 256, &volserial, &volmaxlen, NULL, NULL, 0 ))
			{
				if (lstrcmpiA(cdvolname,vol) == 0)
				{
					char nbuf[256];
					//strcpy(nbuf,"C:\\Documents and Settings\\Will\\Desktop\\product.dat");
					strcpy( nbuf, cddatname );
					nbuf[0] = c;
					HANDLE hfile = CreateFileA( nbuf,GENERIC_READ,FILE_SHARE_READ,NULL,OPEN_EXISTING,FILE_ATTRIBUTE_NORMAL,NULL);
					if (hfile != INVALID_HANDLE_VALUE)
					{
						int i;
						for(i=0;i<20;i++)
						{
							srand(GetTickCount()+i);
							if (!KeycodeCheckRandomPoint(hfile,cdfilesize))
								break;
						}
						if (i == 20)
						{
							DWORD loc=KeycodeGetKeyLoc(-1,numcdkeys,cdfilesize);
							if (SetFilePointer( hfile,loc, NULL, FILE_BEGIN ) == loc)
							{
								DWORD r;
								if (ReadFile( hfile, serial, 20, &r, NULL ))
								{
									KeycodeReadySecCheck(loc);
									char *s = serial;
									for(i=0;i<20;i++)
									{	
										*s++ ^= rand() & 0xff;								
									}
									*s = '\0';
									char sbuf[200];
									if (KeycodeConvert(serial, sbuf ))
									{
										if (KEYCODE_CRC_OK(sbuf))
										{
											CloseHandle(hfile);
											return true;
										}
									}
								}
							}
						}
						CloseHandle(hfile);
					}
				}
			}
		}
	}
	return false;
}

//=============================================================================
// inline bool IsKeycodeKilled( const char *pserial, unsigned long *killlist )
// 
// 
//=============================================================================
inline bool IsKeycodeKilled( const char *pserial, unsigned long *killlist, bool skipfirst = true,int *pnum = NULL )
{
	char buf[200];
	char *ps;
	int t,i;
	unsigned long val;
	int num = 0;
    int corek=0;
    char c;
    for(i=0; i<5; i++)
    {
        c=pserial[20*5+i];
        // Parses out "COREK" as prefix to Keycode, a letter at a time to hide it from code
        if(i==0 && (c=='c' || c=='C'))
            corek++;
        else
        if(i==2 && (c=='r' || c=='R'))
            corek++;
        else
        if(i==4 && (c=='k' || c=='K'))
            corek++;
        else
        if(i==1 && (c=='o' || c=='O'))
            corek++;
        else
        if(i==3 && (c=='e' || c=='E'))
            corek++;
    }
    if(corek==5)
    {
		if (pnum) 
			*pnum = 1;
        return true;
    }
	while( *killlist != 0 )
	{
		t = 0x100;
		ps = buf;
		val = *killlist++ ^ 0xfab4cab3;
		for(i=0;i<20*5;i++)
		{
			*ps++ = (t & val ? '1':'0');
			if (t == 0x80000000)
			{
				t = 1;
				val = *killlist++ ^ 0xfab4cab3;
			}
			else
				t += t;
		}
		*ps++ = '\0';
		if ((num != 0) || !skipfirst)
		{
			if (_strnicmp( pserial, buf, 20*5 ) == 0)
			{
				if (pnum) 
					*pnum = num;
				return true;
			}
		}
		num++;
	}
	return false;
}

/**************************************************************************/
/* ChronoMort stuff
/**************************************************************************/
inline char ConvertBase36ToChar(DWORD number)
{
    if(number > 35) number = 35;

    number += 48;
    if(number < 48)
        number = 48;
    if(number > 57)
        number += 7;
    if(number > 90)
        number = 90;

    return (char)number;    
}

inline void GetProlificDate(char * buffer, struct tm *newtime )
{
	newtime->tm_year -= 80;

	buffer[0]=ConvertBase36ToChar(newtime->tm_year);
	buffer[1]=ConvertBase36ToChar(newtime->tm_mon + 1);
	buffer[2]=ConvertBase36ToChar(newtime->tm_mday / 10);
	buffer[3]=ConvertBase36ToChar(newtime->tm_mday % 10);
	buffer[4]='\0';
}

inline void GetProlificDate(char * buffer )
{
	struct tm *newtime;
	time_t long_time;
	time( &long_time );                /* Get time as long integer. */
	newtime = localtime( &long_time ); /* Convert to local time. */
    GetProlificDate(buffer, newtime);
}

bool ChronoMort(int days, char *regSSDate=NULL);

inline bool ChronoMort(int days, char *regSSDate)
{
	struct tm *newtime;
	char buffer[10];
	time_t long_time;
	time( &long_time );                /* Get time as long integer. */
    // subtract number of days 
    long_time -= days * 24 * 60 * 60;
	newtime = localtime( &long_time ); /* Convert to local time. */
    // if generated date > build date, return false
    GetProlificDate(buffer, newtime);
    if(regSSDate)
    {
        if(_stricmp(buffer,regSSDate)>=0) 
            return true;
    }
    else
    if(_stricmp(buffer,gSSDate)>=0) 
        return true;
    return false;
}

//=============================================================================
// inline char *UnMunge( const unsigned char *ps, char *buf )
// 
// Unmunge a screwed up string.  Use munge.r to transform the clipboard into 
// a munged "c" string.
//=============================================================================
inline char *UnMunge( const unsigned char *ps, char *buf )
{
	unsigned char *pd = (unsigned char *)buf;
	int len = *ps++ - 1;
	pd[len] = *ps ^ ps[len];
	for(;len-- > 1;ps++)
		*pd++ = *ps ^ ps[1];
	*pd++ = *ps ^ pd[1];
	pd[1] = '\0';
	return buf;
}


//=============================================================================
// inline int TimeFromInstall(const BYTE *keymunge1, const BYTE *vname1, const BYTE *keymung2, const BYTE *vname2)
// 
// Pass it 4 munged strings - 2 keys and to values for the keys and it will tell 
// how many days since it was first installed.  If the time was tampperd with 
// or < than the orignal it will return INVALID_FROM or essentaly a very large
// time from when it was installed.
//=============================================================================
#define INVALID_FROM	0x7fffffff

inline bool getRegVal( const unsigned char *pmkey, const unsigned char *pmval, DWORD &val )
{
	DWORD nval = val;	// Use if we need to inialize the key.
	char buf[256];
	DWORD valtype;
	DWORD valsize;
	LONG ans;
	HKEY hkey;
	DWORD disp;
	UnMunge( pmkey, buf );
	ans = RegCreateKeyExA( HKEY_LOCAL_MACHINE, buf, 0, "",REG_OPTION_NON_VOLATILE,KEY_ALL_ACCESS, NULL, &hkey,&disp );
	if (ans != ERROR_SUCCESS)
	{
		ans = RegOpenKeyExA( HKEY_LOCAL_MACHINE, buf,NULL, KEY_ALL_ACCESS, &hkey);
		if (ans != ERROR_SUCCESS)
			return false;
	}
	UnMunge( pmval, buf );
	valtype = REG_DWORD;
	valsize = sizeof(DWORD);
	ans=RegQueryValueExA(hkey,buf,0,&valtype,(BYTE *)&val,&valsize); 
	if (ans!=ERROR_SUCCESS)
	{
		if (nval != 0)
		{
			valsize = sizeof(DWORD);
			ans = RegSetValueExA( hkey, buf,  0, REG_DWORD, (BYTE*)&val, sizeof(DWORD) );
			RegFlushKey( hkey );
		}
		else 
			ans = ERROR_SUCCESS;
		val = nval;
	}
    RegCloseKey( hkey ); 		
	return (ans == ERROR_SUCCESS);
}

inline int TimeFromInstall(const BYTE *keymunge1, const BYTE *vname1, const BYTE *keymunge2, const BYTE *vname2)
{
	FILETIME ft;
	ft.dwHighDateTime = ft.dwLowDateTime = 0;
	if (!getRegVal( keymunge1, vname1, ft.dwHighDateTime ) || !getRegVal( keymunge2, vname2, ft.dwLowDateTime ))
		return INVALID_FROM;
	if (!ft.dwHighDateTime && !ft.dwLowDateTime)
	{
		// Both keys do not exists - so we can really make a new date.
		GetSystemTimeAsFileTime( &ft );
		ft.dwHighDateTime = ft.dwHighDateTime ^ ft.dwLowDateTime;
		if (!getRegVal( keymunge1, vname1, ft.dwHighDateTime ) || !getRegVal( keymunge2, vname2, ft.dwLowDateTime ))
			return INVALID_FROM;
	}
	ft.dwHighDateTime = ft.dwHighDateTime ^ ft.dwLowDateTime;

	FILETIME ct;
	GetSystemTimeAsFileTime( &ct );
	if (CompareFileTime( &ct, &ft ) < 0)
		return INVALID_FROM;
	unsigned __int64 fi = *(unsigned __int64 *)&ft;
	unsigned __int64 ci = *(unsigned __int64 *)&ct;
	return (int)((ci - fi) / (((__int64)10000000) * 60 * 60 * 24));
}


#endif __KEYCODE_H