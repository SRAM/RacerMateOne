/* ----------------------------------------------------------------------------
 * $Workfile: $
 * $Revision: 1.1 $
 *
 * $Author: EdgarCT-PC\Edgar $
 * $Date: 2008/06/27 07:00:40 $
 * ----------------------------------------------------------------------------
 * //Log: $
 * ----------------------------------------------------------------------------
 * This information is CONFIDENTIAL and PROPRIETARY
 * Copyright (C) 2003, Prolific Publishing, Inc.
 * All rights reserved.
 *
 * $Nokeywords: $
 */
#include "stdafx.h"

//#include "Keycode.h"

//=============================================================================
// static void writeRegCode(HKEY userhkey )
// 
// 
//=============================================================================
// VERSION 
#define PIRATED         -1
#define DEMO            0
#define TEMPREG         1
#define DELUXE          2

char gRegVersion[32];
extern unsigned long gKillKeycodes[];

int registered = DEMO;// Is this copy registered?
int lastregistered;// Is this copy registered?
CHAR tRegCodeBuff[32];
CHAR regCodeBuff[32];
CHAR ShrinkedHardwareID[256];
CHAR ExpandedHardwareID[256];
CHAR regCDKEY[32];
CHAR regEmail[256];
//CHAR regCodeBuffVal[256];
#ifdef _CDONLY
bool gIsFromCD = true;
#else
bool gIsFromCD = false;
#endif
bool bUpgradeKeycode = false;
bool bOldKeyCode = false;

bool gVersionError = false; // Version has changed - put in the CD again.
CHAR gOverrideURL[256] = "";
CHAR gOverrideURLLink[256] = "";
CHAR gOverrideBuyURLLink[256] = "";
//bool gNovabuild = false;
// Settings - place the hardware and software registry settings here.
//#ifdef BETATEST
//TCHAR *hardwarePath = TEMPHARDWAREREGPATH;
//TCHAR *softwarePath = TEMPSOFTWAREREGPATH;
//#else
//CHAR *trialhardwarePath = TRIALHARDWAREREGPATH;
CHAR *hardwarePath = HARDWAREREGPATH;
CHAR *softwarePath = SOFTWAREREGPATH;
//#endif
bool oldReg;    // An old registration code has been entered... 

static char productVersion[] = "VersionStr";
char tPirateDate[32];
//bool connected = false;
CHAR trialdate[32];
bool gbTrialOver=false;

HKEY hwkey=NULL,swkey=NULL;
//=============================================================================
// bool OpenKeys()
// 
// 
//=============================================================================
bool OpenKeys(const char *subkey)
{
    if (!swkey)
    {
        char *tbuf;
        tbuf = new char[MAX_PATH];
        
        
        if (subkey)
        {
            sprintf(tbuf,"%s\\%s",softwarePath,subkey);// temp
        }
        else
        {
            sprintf(tbuf,"%s",softwarePath);// temp
        }

        DWORD dis;
        LONG ans = RegCreateKeyExA( HKEY_CURRENT_USER, tbuf, 0, "", REG_OPTION_NON_VOLATILE, KEY_ALL_ACCESS, NULL, &swkey, &dis );

        if(ans != ERROR_SUCCESS)
        {
            ans = RegOpenKeyExA( HKEY_CURRENT_USER, tbuf ,NULL, KEY_ALL_ACCESS, &swkey);
            if (ans != ERROR_SUCCESS)
                swkey = NULL;
        }

        //if (subkey)
            delete[] tbuf;
    }
    if (!hwkey)
    {
        DWORD dis;
        LONG ans = RegCreateKeyExA( HKEY_LOCAL_MACHINE, hardwarePath, 0, "", REG_OPTION_NON_VOLATILE, KEY_ALL_ACCESS, NULL, &hwkey, &dis );
        if(ans != ERROR_SUCCESS)
        {
            ans = RegOpenKeyExA( HKEY_LOCAL_MACHINE, hardwarePath ,NULL, KEY_ALL_ACCESS, &hwkey);
            if (ans != ERROR_SUCCESS)
            {
                ans = RegOpenKeyExA( HKEY_LOCAL_MACHINE, hardwarePath ,NULL, KEY_READ, &hwkey);
                if (ans != ERROR_SUCCESS)
                    hwkey = NULL;
            }
        }
    }
    return ((swkey != NULL) || (hwkey != NULL));
}
//=============================================================================
// void CloseKeys()
// 
// 
//=============================================================================
void CloseKeys()
{
    if (swkey)
    {
        RegFlushKey( swkey );
        RegCloseKey( swkey ); 
        swkey = NULL;
    }
    if (hwkey)
    {
        RegFlushKey( hwkey );
        RegCloseKey( hwkey ); 
        hwkey = NULL;
    }
}

//=============================================================================
// bool Unmunge( void *pd, DWORD size )
// 
// 
//=============================================================================
char *Unmunge( const unsigned char *ps, char *buf, DWORD maxsize )
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
// void InitAffEx( const char *regpath, const char *regkey )
// 
// 
//=============================================================================
#define AFF_KEY_MAX     16
static char affExData[ 256 ];
static char *affKeyPairs[2 * AFF_KEY_MAX];
static int affKeys;

//-----------------------------------------------------------------------------
// Name: InitAffEx()
// Desc: 
//-----------------------------------------------------------------------------
void InitAffEx( const char *regpath, const char *regname )
{
    affKeys = 0;
    BYTE buf[256];

    HKEY hwkey;
    DWORD dis;
    LONG ans = RegCreateKeyExA( HKEY_LOCAL_MACHINE, regpath, 0, "", REG_OPTION_NON_VOLATILE, KEY_ALL_ACCESS, NULL, &hwkey, &dis );
    if(ans != ERROR_SUCCESS)
    {
        ans = RegOpenKeyExA( HKEY_LOCAL_MACHINE, regpath ,NULL, KEY_ALL_ACCESS, &hwkey);
        if (ans != ERROR_SUCCESS)
            return;
    }
    DWORD size = 256;
    DWORD type = REG_BINARY;
    ans = RegQueryValueExA( hwkey, regname,  0, &type, (BYTE*)buf, &size );
    if ((ans == ERROR_SUCCESS) && (type == REG_BINARY) && (size >= 4) && (size - 4 == (DWORD)buf[2]))
    {
        BYTE *v = ((BYTE *)buf) + 2;
        DWORD crc = 0;
        DWORD i;
        for(i=2;i<size;i++)
        {
            crc  = (unsigned char)(crc >> 8) | (crc << 8);
            crc ^= *v++;
            crc ^= (unsigned char)(crc & 0xff) >> 4; 
            crc ^= (crc << 8) << 4;
            crc ^= ((crc & 0xff) << 4) << 1;
        }
        if (*(WORD *)buf == (WORD)crc)
        {
            char *ps = Unmunge( buf + 2, affExData, size - 2 );
            if ((ps[0] == '@') && (ps[1] == '#'))
            {
                ps+=2;
                // OK Everything is all right... get the stuff.
                char **pps = affKeyPairs;
                while(*ps && (affKeys < AFF_KEY_MAX))
                {
                    *pps++ = ps;
                    while(*ps++)
                        ;
                    *pps++ = ps;
                    while(*ps++)
                        ;
                    affKeys++;
                }
            }

        }
    }
    RegCloseKey( hwkey );

}
//-----------------------------------------------------------------------------
// Name: GetAffExValue()
// Desc: 
//-----------------------------------------------------------------------------
const char *GetAffExValue( const char *pk )
{
    char **pps = affKeyPairs;
    for(int i=0;i<affKeys;i++,pps+=2)
    {
        if (_stricmp( *pps, pk ) == 0)
            return pps[1];
    }
    return NULL;
}

void clearRegCode ()
{
    if(!OpenKeys(NULL)) 
        return;
	for(int i=0; i < 32; i++)
	{
		regCodeBuff[i] = '\0';
		regCDKEY[i] = '\0';
	}
	regEmail[0] = '\0';
    if(swkey)
    {
        RegSetValueExA( swkey, "RegCode",  0, REG_BINARY, (BYTE*)&regCodeBuff, 32 );
        RegSetValueExA( swkey, "CDKey",  0, REG_BINARY, (BYTE*)&regCDKEY, 32 );
        RegSetValueExA( swkey, "Email",  0, REG_SZ, (BYTE*)&regEmail, 1 );
    }
    if(hwkey)
    {
        RegSetValueExA( hwkey, "RegCode",  0, REG_BINARY, (BYTE*)&regCodeBuff, 32 );
        RegSetValueExA( hwkey, "CDKey",  0, REG_BINARY, (BYTE*)&regCDKEY, 32 );
        RegSetValueExA( hwkey, "Email",  0, REG_SZ, (BYTE*)&regEmail, 1 );
    }
    CloseKeys();
}

void writeRegCode( HKEY userhkey )
{
    if(!userhkey)
        if(!OpenKeys(NULL)) 
            return;

    // save prev registered value
    int lastreg=lastregistered;
    // only write this if it is pirated keycode, and previous is not PIRATED
    if(registered==PIRATED)
    {
        if(lastreg!=PIRATED)
        {
            GetProlificDate(tPirateDate);
            lastregistered=registered;
        }
    }
    else
    {   
        // else delete the piratedate, it is clean so far
        lastregistered=registered;
        tPirateDate[0]=0;
        tPirateDate[1]=0;
        tPirateDate[2]=0;
        tPirateDate[3]=0;
    }

    regCodeBuff[31-4]=tPirateDate[0];
    regCodeBuff[31-3]=tPirateDate[1];
    regCodeBuff[31-2]=tPirateDate[2];
    regCodeBuff[31-1]=tPirateDate[3];
    regCodeBuff[31]=lastregistered;

    lstrcpyA(gRegVersion,gProlificVersion);

    if(swkey)
    {
        RegSetValueExA( swkey, "RegCode",  0, REG_BINARY, (BYTE*)&regCodeBuff, 32 );
        RegSetValueExA( swkey, productVersion,  0, REG_SZ, (BYTE*)gRegVersion, 32 );
        RegSetValueExA( swkey, "CDKey",  0, REG_BINARY, (BYTE*)&regCDKEY, 32 );
        RegSetValueExA( swkey, "Email",  0, REG_SZ, (BYTE*)&regEmail, lstrlenA(regEmail)+1 );
    }
    if(hwkey)
    {
        RegSetValueExA( hwkey, "RegCode",  0, REG_BINARY, (BYTE*)&regCodeBuff, 32 );
        RegSetValueExA( hwkey, productVersion,  0, REG_SZ, (BYTE*)gRegVersion, 32 );
        RegSetValueExA( hwkey, "CDKey",  0, REG_BINARY, (BYTE*)&regCDKEY, 32 );
        RegSetValueExA( hwkey, "Email",  0, REG_SZ, (BYTE*)&regEmail, lstrlenA(regEmail)+1 );
    }
    if(!userhkey)
        CloseKeys();
}

bool readRegCode( HKEY userhkey )
{
    if(!userhkey)
        if(!OpenKeys(NULL)) 
            return false;

    int ans;
    DWORD dwType = REG_BINARY;

    if(hwkey)
    {
        DWORD dwSize2 = 32;
        ans=RegQueryValueExA( hwkey, "RegCode",  0, &dwType, (BYTE*)&regCodeBuff, &dwSize2);

        lastregistered = regCodeBuff[31];
        tPirateDate[0] = regCodeBuff[31-4];
        tPirateDate[1] = regCodeBuff[31-3];
        tPirateDate[2] = regCodeBuff[31-2];
        tPirateDate[3] = regCodeBuff[31-1];
        tPirateDate[4] = 0;

        ans=RegQueryValueExA( hwkey, "CDKey",  0, &dwType, (BYTE*)&regCDKEY, &dwSize2);

        regEmail[0] = '\0';
        {
            regEmail[255] = '\0';
            DWORD type = REG_SZ;
            DWORD size = 255;
            if (RegQueryValueExA( hwkey, "Email",  0, &type, (BYTE*)regEmail, &size ) != ERROR_SUCCESS)
                regEmail[0] = '\0';
        }

        gRegVersion[0] = '\0';
        {
            DWORD type = REG_SZ;
            DWORD size = 32;
            if (RegQueryValueExA( hwkey, productVersion,  0, &type, (BYTE*)gRegVersion, &size ) != ERROR_SUCCESS)
                gRegVersion[0] = '\0';
        }

        gOverrideURL[0] = '\0';
        {
            gOverrideURL[255] = '\0';
            DWORD type = REG_SZ;
            DWORD size = 255;
            if (RegQueryValueExA( hwkey, "AffiliateWebsite",  0, &type, (BYTE*)gOverrideURL, &size ) != ERROR_SUCCESS)
                gOverrideURL[0] = '\0';
        }
        gOverrideURLLink[0] = '\0';
        {
            gOverrideURLLink[255] = '\0';
            DWORD type = REG_SZ;
            DWORD size = 255;
            if (RegQueryValueExA( hwkey, "Affiliate",  0, &type, (BYTE*)gOverrideURLLink, &size ) != ERROR_SUCCESS)
                gOverrideURLLink[0] = '\0';
        }
        gOverrideBuyURLLink[0] = '\0';
        {
            gOverrideBuyURLLink[255] = '\0';
            DWORD type = REG_SZ;
            DWORD size = 255;
            if (RegQueryValueExA( hwkey, "AffiliateBuy",  0, &type, (BYTE*)gOverrideBuyURLLink, &size ) != ERROR_SUCCESS)
                gOverrideBuyURLLink[0] = '\0';
        }


#if defined(TRIALMODE)

        {
            LONG ans;
            HKEY trialhwkey=NULL;
            TCHAR tempsz[32];
            DWORD type = REG_SZ;
            DWORD size = 32;
            wsprintf(tempsz, TEXT("%d"), PRODUCT_ID);
            GetProlificDate(trialdate); // use today's date as the default
            if (!trialhwkey)
            {
                DWORD dis;
                ans = RegOpenKeyEx( HKEY_LOCAL_MACHINE, trialhardwarePath ,NULL, KEY_ALL_ACCESS, &trialhwkey);
                if (ans != ERROR_SUCCESS)
                {
                    ans = RegOpenKeyEx( HKEY_LOCAL_MACHINE, trialhardwarePath ,NULL, KEY_READ, &trialhwkey);
                    if (ans != ERROR_SUCCESS)
                    {
                        ans = RegCreateKeyEx( HKEY_LOCAL_MACHINE, trialhardwarePath, 0, "", REG_OPTION_NON_VOLATILE, KEY_ALL_ACCESS, NULL, &trialhwkey, &dis );
                        if(ans == ERROR_SUCCESS)
                        {
                            RegSetValueEx( trialhwkey, tempsz,  0, type, (BYTE*)&trialdate, size );
                        }
                    }
                }
            }
            if (trialhwkey)
            {
                if (RegQueryValueEx( trialhwkey, tempsz,  0, &type, (BYTE*)trialdate, &size ) != ERROR_SUCCESS)
                {
                    RegSetValueEx( trialhwkey, tempsz,  0, type, (BYTE*)&trialdate, size );
                }
                RegFlushKey( trialhwkey );
                RegCloseKey( trialhwkey ); 
                trialhwkey = NULL;
            }
            else
            {
                gbTrialOver=true;
            }
            if(ChronoMort(DAYSEXPIRED,trialdate))
            {
                gbTrialOver=true;
            }
        }
#endif

    }

    if(ans != ERROR_SUCCESS && swkey )
    {
        DWORD dwSize2 = 32;
        ans=RegQueryValueExA( swkey, "RegCode",  0, &dwType, (BYTE*)&regCodeBuff, &dwSize2);

        tPirateDate[0] = regCodeBuff[31-4];
        tPirateDate[1] = regCodeBuff[31-3];
        tPirateDate[2] = regCodeBuff[31-2];
        tPirateDate[3] = regCodeBuff[31-1];
        tPirateDate[4] = 0;
        lastregistered = regCodeBuff[31];

        ans=RegQueryValueExA( swkey, "CDKey",  0, &dwType, (BYTE*)&regCDKEY, &dwSize2);

        regEmail[0] = '\0';
        {
            regEmail[255] = '\0';
            DWORD type = REG_SZ;
            DWORD size = 255;
            if (RegQueryValueExA( swkey, "Email",  0, &type, (BYTE*)regEmail, &size ) != ERROR_SUCCESS)
                regEmail[0] = '\0';
        }

        if (gRegVersion[0] == '\0')
        {
            DWORD type = REG_SZ;
            DWORD size = 32;
            if (RegQueryValueExA( swkey, productVersion,  0, &type, (BYTE*)gRegVersion, &size ) != ERROR_SUCCESS)
                gRegVersion[0] = '\0';
        }

    }

    if(!userhkey)
        CloseKeys();

    // Handle Affiliates
    {
        InitAffEx( hardwarePath, "AffEx" );
        const char *ps;
        if ((ps = GetAffExValue( "website" )))
        {
            lstrcpyA(gOverrideURL, ps);
        }
        if ((ps = GetAffExValue( "url" )))
        {
            lstrcpyA(gOverrideURLLink, ps);
        }
        if ((ps = GetAffExValue( "buyurl" )))
        {
            lstrcpyA(gOverrideBuyURLLink, ps);
        }
    }

//    if(strstr(gOverrideURL,"nova") || strstr(gOverrideURL,"avanquest") )
//        gNovabuild = true;

    return (ans == ERROR_SUCCESS);
}

char hexval[16] = {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F'};
const char *CalcShrink( const char *str )
{
	char tbuf[256];
	char tout[256];
	int c;
	int cnt = 0;
	char *p = tout;
	const char *t = str;
	while(*t != '\0')
	{
		c = int(*t++);
		if(c > 32)
		{
			c -= 33;
			if (c > 58)
			{
				tbuf[cnt++] = 59;
				c -= 59;
			}
			tbuf[cnt++] = c;
		}
	}
	tbuf[cnt++] = 59;
	tbuf[cnt++] = 35;
	tbuf[cnt] = 0;


	unsigned char num;
	unsigned char ch = 0;
	for(int i=0; i<cnt; i++)
	{
		num = tbuf[i];
		ch = 0x20;
		for(int j=0;j<6;j++,ch>>=1)
		{
			*p++ = (ch & num ? '1':'0');
		}
	}
	*p = '\0';

	// add padding to keep the ame length and divisible by 8
	int rlen = lstrlenA(tout);
	int xlen = 86 - rlen;
	int len = (rlen + xlen) & -8;
	for(int j=0;j<xlen;j++)
	{
		int k = j+j*8;
		if (k >= rlen)
			k -= rlen; 
		*p++ = tout[k];
	}
	//lstrcatA(tout, "00000000");
	tout[len] = '\0';

	p = ShrinkedHardwareID;
	cnt = 0;
	for(int i=0,j=0; i<len; i++)
	{
		ch = (ch << 1) | ((tout[i] == '1') ? 1 : 0);
		j++;
		if(j >= 4)
		{
			*p++ = hexval[ch];
			cnt++;
			ch = 0;
			j = 0;
			if((len > i+8) && ((cnt==6)||(cnt==12)||(cnt==18)))
				*p++ = '-';
		}
	}
	*p = '\0';
	return ShrinkedHardwareID;
}

const char *CalcExpand( const char *str )
{
	char tbuf[10];
	char tout[256];
	char digit[2];
	unsigned char ch;
	digit[1]='\0';

	char *p = tout;
	const char *t = str;
	while(*t != '\0')
	{
		ch = (*t++);
		if(ch != '-')
		{
			if(ch > '9')
				ch = (ch - 'A') + 10;
			else
				ch -= '0';
			for(int j=0;j<4;j++)
			{
				*p++ = ((ch & 0x08) ? '1':'0');
				ch = ch << 1;  
			}
		}
	}
	*p = '\0';

	int len = lstrlenA(tout);
	int cnt = len / 6;
	lstrcatA(tout,"00000000");

	p = ExpandedHardwareID;
	tbuf[6] = '\0';
	unsigned char inc = 33;
	for(int i=0; i<cnt; i++)
	{
//		*t = (const char *)&tout[i*6];
		lstrcpynA(tbuf, &tout[i*6], 7);

		ch = (unsigned char)strtol(tbuf,NULL,2);
		if(ch == 59)
		{
			inc += 59;
		}
		else
		{
			ch += inc;
			
			if(ch == 127)
				break;

			*p++ = ch; 
			inc = 33;
		}
	}
	*p = '\0';
	return ExpandedHardwareID;
}
//=============================================================================
// bool Settings::IsRegistered( bool cdonly )
// 
// 
//=============================================================================
bool IsRegistered( bool bRead, bool cdonly )
{
    registered = DEMO;
    char *serialbuf = new char[200];
    char serial[32];
    if(!bRead) 
        MungKeycode(regCodeBuff);
    memcpy( serial, regCodeBuff, 32 );
    if (MungKeycode(serial) && KeycodeConvert(serial,serialbuf))
    {
        int num;
        bool killed = IsKeycodeKilled( serialbuf, gKillKeycodes,false,&num);
        if (killed && (num == 0))
        {
            // OK this is a CD Key -- see if the versions match.
            if (lstrcmpiA( gRegVersion, gProlificVersion ) == 0)
            {
                killed = false;
            }
            else
                gVersionError = true;
        }
        else if (cdonly)    // Corp logo - only accepts the CDKEY
            killed = true;

        unsigned short a = KEYCODE_CRC_OK(serialbuf);
        unsigned short b = KEYCODE_GETID(serialbuf);
        unsigned short c = KEYCODE_MINOK_4(serialbuf);
/*
#ifdef BETATEST // temporary for beta testing - ECT-Todo BETATEST

        // entered keycode is a valid Keycode, give a DELUXE registration
        if (KEYCODE_CRC_OK(serialbuf) && 
            ((KEYCODE_GETID(serialbuf) == PRODUCT_ID) || (KEYCODE_GETID(serialbuf) == 14)) && 
            KEYCODE_MINOK_4(serialbuf) && !killed) 
        {
            // version 2, or 3 succeeds - ECT-Todo - change back to only version 3
            if((KEYCODE_GETVER(serialbuf) == gVersion) || (gVersion == 3 && KEYCODE_GETVER(serialbuf) == 2))
            //if(KEYCODE_GETVER(serialbuf) == (gVersion ? gVersion:1))
            {
                // if (UpgradeKeycode and OldCD) || notUpgradeKeycode, accept it
                if( (!KEYCODE_ISUPGRADE(serialbuf)) ||
                    (KEYCODE_ISUPGRADE(serialbuf) && (bRead || KeycodeCDCheck_V1())) 
                  )
                {
                    registered = DELUXE;
                    //if (MungKeycode(regCodeBuff))
                        writeRegCode(NULL);
                }
                else  // must be upgradecode
                {
                    bUpgradeKeycode = true;
                }
            }
            else
            if(KEYCODE_GETVER(serialbuf) < gVersion)
            {
                bOldKeyCode=true;
            }
        }
#else // final 
*/
        // entered keycode is a valid Keycode, give a DELUXE registration
        if (KEYCODE_CRC_OK(serialbuf) && 
            (KEYCODE_GETID(serialbuf) == PRODUCT_ID) && 
            KEYCODE_MINOK_4(serialbuf) && !killed) 
        {
            if(KEYCODE_GETVER(serialbuf) == (gVersion ? gVersion:1))
            {
                // if (UpgradeKeycode and OldCD) || notUpgradeKeycode, accept it
                if( (!KEYCODE_ISUPGRADE(serialbuf)) ||
                    (KEYCODE_ISUPGRADE(serialbuf) && (bRead || KeycodeCDCheck_V1())) 
                  )
                {
                    registered = DELUXE;
                    writeRegCode(NULL);
                }
                else  // must be upgradecode
                {
                    bUpgradeKeycode = true;
                }
            }
            else
            if(KEYCODE_GETVER(serialbuf) < gVersion)
            {
                bOldKeyCode=true;
            }
        }
//#endif
/*
        if (KEYCODE_CRC_OK(serialbuf) && KEYCODE_MINOK_1(serialbuf) && !killed)
        {
            registered=DELUXE;
        }
*/
    }
    delete[] serialbuf;
    return (registered == DELUXE);
}

//=============================================================================
// int int CheckRegisteredType()
// 
// 
//=============================================================================
int CheckRegisteredType(const char *hardwareid)
{
	char tbuf[256];
	lstrcpyA(ShrinkedHardwareID,hardwareid);

	readRegCode(NULL);
    MungKeycode(regCodeBuff);

	lstrcpyA(tbuf, CalcExpand(ShrinkedHardwareID));
	lstrcatA(tbuf, regCDKEY);
	lstrcatA(tbuf, TITLE_TEXT);
	CalcCRCToString(tRegCodeBuff,tbuf);

	if(0 == _strnicmp(tRegCodeBuff,regCodeBuff,5))
	{
		IsRegistered(false, gIsFromCD);
	}
    return registered;
}

//=============================================================================
// bool IsRegisteredValid()
// 
// 
//=============================================================================
bool IsRegisteredValid()
{
    return (registered == DELUXE) ;
}


//=============================================================================
// bool Settings::CDCheck( bool cdonly )
// 
// 
//=============================================================================
bool CDCheck( bool cdonly )
{
    char serial[32];
    if (registered == DELUXE)
        return true;

    if (KeycodeCDCheck( serial ))
    {
        memcpy( regCodeBuff, serial, 32 );
        if (MungKeycode(regCodeBuff))
            writeRegCode(NULL); 
        registered = DELUXE;
        gVersionError = false;
    }
    return (registered == DELUXE);
}
#if 0
bool TryRegister(HWND hwndDlg, bool bForce)
{
    bUpgradeKeycode = false;
    bOldKeyCode = false;
    CHAR szText[500];

    // if not registered
    if(((registered<DELUXE)&&(registered>PIRATED))||gVersionError)
    {
        //int num;
        // get the entered Keycode
        GetDlgItemText(hwndDlg, IDD_REGCODE, (LPSTR)regCodeBuff, 32);
        lstrcpyA(regCodeBuffVal,regCodeBuff); // save the entered code for the duration of the config

        // pack it, remove extra chars
        if (PackKeycode( regCodeBuff ))
        {
            // convert to binary keycode
            if(IsRegistered(false, gIsFromCD))
                goto exitdlg;
        }
        // if test Keycode is entered, then give TEMPREG registration
        if (!gbTrialOver && stricmp(regCodeBuff, "TESTBIKE" ) == 0)
        {
            registered=TEMPREG;
            // clear keycode buf
            regCodeBuff[0]=0;
        }
        else    // else check CD for Keycode
        {
            if(bForce)
            {
                if(!bUpgradeKeycode) // check CD if not upgradecode 
                {
                    // clear keycode buf, so keycode will come from CD
                    regCodeBuff[0]=0;
                    if (CDCheck(gIsFromCD))
                        goto exitdlg;
                }

                GetDlgItemText(hwndDlg, IDD_REGCODE, (LPSTR)regCodeBuff, 32);

                // trying to save,  
    //          if(((!connected && isChanged())||bUpgradeKeycode||bOldKeyCode)
                //if(bUpgradeKeycode||bOldKeyCode)
                //{
                    //if(giState==2) // if state 2
                    //  giState++;  // goto state 3

                    if(bUpgradeKeycode)
                    {
                        LoadString( NULL, IDS_ERROR_UPGRADESTR, szText, 500 );
//                        MessageBox(hwndDlg, szText, _T(TITLE_TEXT), MB_OK);
                        MessageBox(hwndDlg, szText, g_strWindowTitle, MB_OK);
//                      MessageBox( hwndDlg, "Please insert a previous version of Marine Aquarium CD to upgrade to 3.0.",
//                                  TITLE_TEXT, MB_OK );
                    }
                    else
                    if(bOldKeyCode)
                    {
                        LoadString( NULL, IDS_ERROR_INVALIDKEYCODE, szText, 500 );
//                        MessageBox(hwndDlg, szText, _T(TITLE_TEXT), MB_OK);
                        MessageBox(hwndDlg, szText, g_strWindowTitle, MB_OK);
//                      MessageBox( hwndDlg, 
//                                  "The Keycode you entered appears to be for an older version. To determine why your Keycode might not be valid, please check the F.A.Q regarding \"Keycodes\" on our website first.  Or to purchase the newest version of the product please visit our site from the button below.",
//                                  TITLE_TEXT, MB_OK );
                    }
                    else
                    {
                        if(gbTrialOver)
                            LoadString( NULL, IDS_ERROR_REENTERKEYCODE_TRIAL, szText, 500 );
                        else
                            LoadString( NULL, IDS_ERROR_REENTERKEYCODE, szText, 500 );
//                        MessageBox(hwndDlg, szText, _T(TITLE_TEXT), MB_OK);
                        MessageBox(hwndDlg, szText, g_strWindowTitle, MB_OK);
//                      MessageBox( hwndDlg, "Please enter \"TESTBIKE\" or a valid keycode or place a Marine Aquarium 3 CD into the drive to continue.",
//                                  TITLE_TEXT, MB_OK );
                    }
                //}
                //else
                //{
                //  goto exitdlg;
                //}
            }
            return false;
        }
    }
exitdlg:
    return true;
}
#else

//=============================================================================
// int GetRegistrationInfo(void **ptrRegCode, void **ptrCDKey, void **ptrEmail)
// Returns KeyCode, CDKey and Email registered to
//=============================================================================
int GetRegistrationInfo(void **ptrRegCode, void **ptrCDKey, void **ptrEmail)
{
	readRegCode(NULL);
    MungKeycode(regCodeBuff);
	(*ptrRegCode) = (void *)regCodeBuff;
	(*ptrCDKey) = (void *)regCDKEY;
	(*ptrEmail) = (void *)regEmail;
    return registered;
}

int TryRegister(CHAR *regCodeBuffVal, CHAR *HardwareSerialNum, CHAR *CDKey, CHAR *email, bool bForce)
{
    bUpgradeKeycode = false;
    bOldKeyCode = false;
    //CHAR szText[500];

    // if not registered
    if(((registered<DELUXE)&&(registered>PIRATED))||gVersionError)
    {
        // get the entered Keycode
        //GetDlgItemText(hwndDlg, IDD_REGCODE, (LPSTR)regCodeBuff, 32);
        //lstrcpyA(regCodeBuffVal,regCodeBuff); // save the entered code for the duration of the config
        lstrcpyA(regCodeBuff,regCodeBuffVal); // save the entered code for the duration of the config
        lstrcpyA(regEmail,email); // save the entered code for the duration of the config

        // pack it, remove extra chars
        if (PackKeycode( regCodeBuff ))
        {
			char tbuf[256];
			lstrcpyA(ShrinkedHardwareID,HardwareSerialNum);
			lstrcpyA(regCDKEY,CDKey);

			lstrcpyA(tbuf, CalcExpand(HardwareSerialNum));
			lstrcatA(tbuf, regCDKEY);
			lstrcatA(tbuf, TITLE_TEXT);
			CalcCRCToString(tRegCodeBuff,tbuf);

			if(0 == _strnicmp(tRegCodeBuff,regCodeBuff,5))
			{
				// convert to binary keycode
				if(IsRegistered(false, gIsFromCD))
					goto exitdlg;
			}
        }
        // if test Keycode is entered, then give TEMPREG registration
        if (!gbTrialOver && lstrcmpiA(regCodeBuff, "TESTBIKE" ) == 0)
        {
            registered=TEMPREG;
            // clear keycode buf
            regCodeBuff[0]=0;
        }
        else    // else check CD for Keycode
        {
            if(bForce)
            {
                if(!bUpgradeKeycode) // check CD if not upgradecode 
                {
                    // clear keycode buf, so keycode will come from CD
                    regCodeBuff[0]=0;
                    if (CDCheck(gIsFromCD))
                        goto exitdlg;
                }
				/*
                GetDlgItemText(hwndDlg, IDD_REGCODE, (LPSTR)regCodeBuff, 32);

                // trying to save,  

                if(bUpgradeKeycode)
                {
                    LoadString( NULL, IDS_ERROR_UPGRADESTR, szText, 500 );
                    MessageBox(hwndDlg, szText, g_strWindowTitle, MB_OK);

                }
                else
                if(bOldKeyCode)
                {
                    LoadString( NULL, IDS_ERROR_INVALIDKEYCODE, szText, 500 );
                    MessageBox(hwndDlg, szText, g_strWindowTitle, MB_OK);
                }
                else
                {
                    if(gbTrialOver)
                        LoadString( NULL, IDS_ERROR_REENTERKEYCODE_TRIAL, szText, 500 );
                    else
                        LoadString( NULL, IDS_ERROR_REENTERKEYCODE, szText, 500 );
                    MessageBox(hwndDlg, szText, g_strWindowTitle, MB_OK);
				}
				*/
            }
            //return registered;
        }
    }
exitdlg:
    return registered;
}

#endif
int CheckRegistered(CHAR* strCmdLine)
{
    CDCheck(gIsFromCD);
/*
    while((*strCmdLine == ' ') || (*strCmdLine == '\t'))
        strCmdLine++;
    TCHAR *ec = strCmdLine;
    while(*ec)
        ec++;
    while( ec-- > strCmdLine )
    {
        if ((*ec == ' ') || (*ec == '\t'))
            *ec = '\0';
        else
            break;
    }
*/
    while((*strCmdLine == ' ') || (*strCmdLine == '\t'))
        strCmdLine++;
    CHAR *ec = strCmdLine;
    while(*ec)
    {
        if ((*ec == ' ') || (*ec == '\t'))
            *ec = '\0';
        ec++;
    }
    
// uncomment next line if we need to enable registering through commandline
// syntax:  screensaver.scr -register actualkeycode
#define REGCMDLINE 1
#if (defined REGCMDLINE)
    LPSTR pkeystr = strCmdLine + lstrlenA(strCmdLine);
    while(pkeystr < ec)
    {
        if(*pkeystr)
            break;
        pkeystr++;
    }
#endif

    if (lstrcmpiA("-register", strCmdLine) == 0)
    {
#if (defined REGCMDLINE)
        if(*pkeystr)
        {
            lstrcpyA((LPSTR)regCodeBuff,pkeystr);
            // pack it, remove extra chars
            if (PackKeycode( regCodeBuff ))
            {
                if (MungKeycode(regCodeBuff))
                {
                    registered = DELUXE;
                    writeRegCode(NULL);
                }
            }
        }
#endif
        return 0;
    }
    
    if(gVersionError)
        return -1;

    // here it is killed code
    if(registered==PIRATED)
    {
        if(ChronoMort(DAYSEXPIRED,tPirateDate))
        {
            registered=TEMPREG;
            regCodeBuff[0]=0;
            writeRegCode( NULL );
        }
    }
    return 1;
}

char buf[32];


bool KeycodeCDCheck_RM1(char *serial);

#if defined(_RM1_)
    bool KeycodeCDCheck( char *serial )
    {
        return (KeycodeCDCheck_RM1(serial)); 
    }
    bool KeycodeCDCheck_V1()
    {
        return false;
    }
#else

    bool KeycodeCDCheck(char *serial)
    {
        return (KeycodeCDCheckX(serial));
    }
    bool KeycodeCDCheck_V1()
    {
        return false;
    }
#endif


#undef _RM1_

#define _RM1_
#undef _COMMON_H_
#include "Common.h"
inline bool KeycodeCDCheck_RM1(char *buf)
{
    //char buf[32];
    char sbuf[200];
    buf[0]=0;
    char cdvolname[]=KEYCODE_CD_VOLUME_NAME;
    char cddatname[]="c:\\" CD_DAT_NAME;
    int numcdkeys=KEYCODE_NUM_CDKEYS;
    int cdfilesize=KEYCODE_CD_FILESIZE;
    //char buf1[256];
    //wsprintf(buf1,"%s,%s,%d,%d",cdvolname,cddatname,numcdkeys,cdfilesize);
    //MessageBox(NULL,buf1,TITLE_TEXT,MB_OK);
    if (KeycodeCDCheckX( buf,cdvolname,cddatname,numcdkeys,cdfilesize ))
    {
        if (KeycodeConvert(buf,sbuf))
        {
            // Other checks - 
           if (KEYCODE_CRC_OK(sbuf) && KEYCODE_MINOK_1(sbuf) && (KEYCODE_GETID(sbuf) == PRODUCT_ID))
                return true;
        }
    }
    return false;
}

