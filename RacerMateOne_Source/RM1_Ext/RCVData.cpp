#include "stdafx.h"
#include <stdio.h>
#include <crf.h>
#include <windows.h>
#include <time.h>
#include <stdlib.h>

#include <vector>
using namespace std;

//================================================================================================
	#pragma pack(push, 1)

typedef struct {
	unsigned long version;
	unsigned long laps;
	char fill[248];
} HEADER;


typedef struct {
	unsigned long frame;			// real frame number
	int real;						// 1 if real data, 0 if interpolated data
	int seconds;
	double lat;						// degrees
	double lon;						// degrees
	double unfiltered_elev;		// raw garmin elevation
	double filtered_elev;		// filtered garmin elevation
	double manelev;				// manually entered elevation

	double accum_meters1;		// from the garmin.txt file
	double accum_meters2;		// computed by me

	double section_meters1;		// garmin's version
	double section_meters2;		// computed by me

	double pg;						// percent grade
	double mps1;					// based on garmin distances
	double mph1;					// based on garmin distances
	double mps2;					// based on computed distances
	double mph2;					// based on computed distances
	double faz;						// forward azimuth
	double seconds_offset;
	double x;						// in meters, used to create the csv file
	double y;						// in meters, used to create the csv file
	double z;						// in meters, used to create the csv file same as manelev but in meters
} GPS_DATA;

	#pragma pack(pop)

typedef struct {
	unsigned long frame;			// real frame number
	int real;						// 1 if real data, 0 if interpolated data
	int seconds;
	double lat;						// degrees
	double lon;						// degrees
	double unfiltered_elev;		// raw garmin elevation
	double filtered_elev;		// filtered garmin elevation
	double manelev;				// manually entered elevation

	double accum_meters1;		// from the garmin.txt file
	double accum_meters2;		// computed by me

	double section_meters1;		// garmin's version
	double section_meters2;		// computed by me

	double pg;						// percent grade
	double mps1;					// based on garmin distances
	double mph1;					// based on garmin distances
	double mps2;					// based on computed distances
	double mph2;					// based on computed distances
	double faz;						// forward azimuth
	double seconds_offset;
	double x;						// in meters, used to create the csv file
	double y;						// in meters, used to create the csv file
	double z;						// in meters, used to create the csv file same as manelev but in meters

} U_GPS_DATA;

//================================================================================================
// Registration stuff
//================================================================================================
#pragma pack(push, 1)

// BIG FAT NOTE: IF YOU CHANGE THESE BE SURE TO ALSO CHANGE THE VIDSERV PROGRAM ON THE SERVER!!!

#define MAXFOLDERLEN 16

typedef struct {
	unsigned long version;
	unsigned long filesize;
	unsigned long folderlen;							// eg 5 for "imcda"
	char folder[MAXFOLDERLEN];								// eg "imcda"
	unsigned long hdsn;
	unsigned long dvdsn;
	char email[128];
	char lname[40];
	char fname[40];
} VIDREG;

typedef struct  {
	unsigned long dvdsn;
	char folder[MAXFOLDERLEN];
	unsigned long filesize;
	unsigned long hdsn;
} CDD;										// obfuscated 'code'

#pragma pack(pop)


class Ini  {

	private:
		char path[256];
		char buf[256];
		char def[256];
		DWORD dw;
		char curdir[128];
		char cursection[64];
		Ini(const Ini&);
		Ini &operator = (const Ini&);		// unimplemented
		int bp;

	protected:
	
	public:

		Ini(const char *_path);
		virtual ~Ini();

		void set_section(const char *_sec);
		bool getBool(const char *key, bool b);
		bool getBool(const char *section, const char *key, bool b);
		bool getBool(const char *section, const char *key, const char * def);
		bool getBool(const char *section, const char *key, const char * def, bool makeIfNotThere);

		float getFloat(const char *section, const char *key, float f);
		float getFloat(const char *section, const char *key, const char *def);
		float getFloat(const char *section, const char *key, const char *def, bool makeIfNotThere);
		float getFloat(const char *key, float f);

		int getInt(const char *section, const char *key, int i, bool _makeIfNotThere=true);
		int getInt(const char *section, const char *key, const char *def);
		int getInt(const char *section, const char *key, const char *def, bool);
		int getInt(const char *key, const char *def);
		int getInt(const char *key, int _i);

		unsigned long get_long_hex(const char *section, const char *key, unsigned long def, bool);
		unsigned long getul(const char *section, const char *key, unsigned long i, bool _makeIfNotThere=true);

		const char * getString(const char *section, const char *key, const char *def);
		const char * getString(const char *section, const char *key, const char *def, bool);
		const char * getString(const char *key, const char *def);

		void writeString(const char *section, const char *key, const char *str);
		void writeBool(const char *section, const char *key, BOOL b);
		void writeBool(const char *key, BOOL b);

		void writeInt(const char *section, const char *key, int i);
		void writeInt(const char *key, int i);

		void writeul(const char *section, const char *key, unsigned long i);
		void write_long_hex(const char *section, const char *key, DWORD _dw);
		void writeFloat(const char *section, const char *key, float f);
		void writeFloat(const char *key, float f);

		const char *getName(void);
		void clear_section(const char *_section);
};

WCHAR *to_wide( const char * strSource )
{
	static WCHAR wide[512];
	int len = (int)strlen(strSource)+1;

    MultiByteToWideChar( CP_ACP, 0, strSource, -1, 
                         wide, len-1 );

    wide[len-1] = 0;
	return wide;
}


//int hexbuf_to_binbuf(char *hexbuf, unsigned char *binbuf, int hexbufsize, int binbufsize);


int hexbuf_to_binbuf(char *sstr, unsigned char *enc, int len, int enclen)
{
	// Decodes something like this:
	// d403ab3ed8c85faadc1d16d41a8434b7cfc992e6d494d810f2f1d0b3
	bool fst = false;
	unsigned char val = 0;
	int cnt = 0;
	while(len > 0 && enclen > 0)
	{
		int n = -1;
		if (*sstr >= '0' && *sstr <= '9')
			n = *sstr - '0';
		else if (*sstr >= 'a' && *sstr <= 'f')
			n = (*sstr - 'a') + 10;
		else if (*sstr >= 'A' && *sstr <= 'F')
			n = (*sstr - 'A') + 10;
		sstr++;
		if (n >= 0)
		{
			len--;
			if (fst)
			{
				*enc++ = (val<<0) + n;
				enclen--;
				fst = false;
				cnt++;
			}
			else
			{
				fst = true;
				val = (unsigned char)(n<<4);
			}
		}
	}
	return cnt;
}

DWORD gds(const char *drive, DWORD &hdsn)  
{
	BOOL B;
	DWORD fsflags;				// this will return flags about the drive's file system
	DWORD maxpath;				// this is the max length in between each \ in a path
	hdsn=0;
	wchar_t fstype[10];			// this will contain the type of file system (ex: NTFS)
	wchar_t volname[12];
	char str[16];

	memset(volname, 0, sizeof(volname));
	memset(fstype, 0, sizeof(fstype));
	hdsn = 0L;
	maxpath = 0L;
	fsflags = 0L;

	strncpy(str, drive, sizeof(str)-1);
	strcat(str, "\\");

	B = GetVolumeInformation(
				//L"C:\\", 
				to_wide(str),
				volname,				// ""
				12, 
				&hdsn,			// 0x5cec7174
				&maxpath,
				&fsflags,
				fstype,				// "NTFS"
				10
			);

	if (B == FALSE)
		return false;
	return true;
}								// gds



bool chk(char *base, Ini *ini)
{
	char sstr[256];
	const char *cptr;
	CDD enc = {0}, dec = {0};
	CRF s;
	unsigned long _hdsn;
	int status;
	int nnn;


	memset(sstr, 0, sizeof(sstr));
	memset(sstr, '0', 2*sizeof(CDD));			// str now has all ascii 0's for 2 times the length of CDD

	cptr = ini->getString("registration", base, sstr);
	strncpy(sstr, cptr, sizeof(sstr)-1);

	/*
	logg->write("sizeof(CDD) = %d\n", sizeof(CDD));			// 28
	logg->write("sizeof(str) = %d\n", sizeof(sstr));			// 256
	logg->write("strlen(sstr) = %d\n", strlen(sstr));			// 56
	logg->write("chk1\n");
	*/

	if (strlen(sstr) != 2*sizeof(CDD) )  
	{
		return false;
	}
	//logg->write("chk2\n");

	nnn = sizeof(CDD);
	if (nnn != 28)  
	{								// 28
		return false;
	}

	/*
	logg->write("ok3\n");
	*/

	if (strlen(sstr) != 2*sizeof(CDD))  
	{
		return false;
	}

	//logg->write("ok4\n");

	status = hexbuf_to_binbuf(sstr, (unsigned char *)&enc, strlen(sstr), sizeof(CDD));
	if (status!=sizeof(CDD))  
	{
		return false;
	}

	//logg->write("ok5\n");
	memcpy(&dec, &enc, sizeof(CDD));
	s.init();
	s.doo((unsigned char *)&dec, sizeof(CDD));

	/*******************************************************************************************************************
		'dec' is the hd and dvd sn's at the time of registration, see if the hd sn now matches.
		This assumes that the dvdsn is not tampered with. We will now re-get the hdsn and the dvdsn (if available) and
		compare them. In the CDD structure dvdsn is first so that the encrypter is scrambled with the dvdsn. So if the
		first 8 bytes of the code are changed a different hdsn will be computed.
	*******************************************************************************************************************/

//#ifndef _DEBUG/
//	logg->write("hdsn = %08lx\n", dec.hdsn);
//	logg->write("dvdsn = %08lx\n", dec.dvdsn);
//	if (dec.dvdsn==dec.hdsn)  {
//		return;
//	}
//#endif

	if (!gds("c:",_hdsn))
		return false;
	if (dec.hdsn != _hdsn)  
	{
		return false;
	}
	return true;
}							// chk(void)


int IsRCVRegistered( const char *name, const char *inipath )
{
	Ini *ini = new Ini(inipath);
	bool ans = chk( (char *)name, ini );
	delete ini;
	return ans ? 1:0;
}



//================================================================================================


class CourseData
{
	vector<U_GPS_DATA> m_Arr;
public:
	CourseData();
	~CourseData();
	bool Load( const char *filename );

	int Segments() { return m_Arr.size(); }
	U_GPS_DATA &Segment(int num) { return m_Arr[num]; }
};

CourseData::CourseData()
{
}

CourseData::~CourseData()
{
}

bool CourseData::Load( const char *filename )
{
	CRF *crf = new CRF();
	crf->init();

	FILE *stream;
	if (fopen_s(&stream,filename, "rb") != 0)
		return false;
	HEADER header;
	size_t status;
	GPS_DATA gpsd;
	U_GPS_DATA ug;

	int cnt=0;
	
	fread(&header, sizeof(HEADER), 1, stream);
	while(1)  
	{
		status = fread(&gpsd, sizeof(GPS_DATA), 1, stream);
		if (status != 1) 
			break;
		crf->doo((unsigned char *) &gpsd, sizeof(GPS_DATA));

		ug.frame = gpsd.frame;
		ug.real = gpsd.real;
		ug.seconds = gpsd.seconds;
		ug.lat = gpsd.lat;
		ug.lon = gpsd.lon;
		ug.unfiltered_elev = gpsd.unfiltered_elev;
		ug.filtered_elev = gpsd.filtered_elev;
		ug.manelev = gpsd.manelev;

		ug.accum_meters1 = gpsd.accum_meters1;
		ug.accum_meters2 = gpsd.accum_meters2;

		ug.section_meters1 = gpsd.section_meters1;
		ug.section_meters2 = gpsd.section_meters2;

		ug.pg = gpsd.pg;
		ug.mps1 = gpsd.mps1;
		ug.mph1 = gpsd.mph1;
		ug.mps2 = gpsd.mps2;
		ug.mph2 = gpsd.mph2;
		ug.faz = gpsd.faz;
		ug.seconds_offset = gpsd.seconds_offset;
		ug.x = gpsd.x;
		ug.y = gpsd.y;
		ug.z = gpsd.z;


		m_Arr.push_back(ug);
	}
	fclose(stream);
	delete crf;
	return true;
}

//===========================================================
static long long ms_ChallengeID = 0; 
long long GetChallengeID()
{
	if (ms_ChallengeID == 0)
	{
		__time64_t t;
		_time64( (__time64_t *)&t );
		srand((int)(t & 0xffffffff));
		while(ms_ChallengeID == 0)
		{
			ms_ChallengeID = ((long long)rand()) << 32 | (long long)rand();
		}
	}
	return ms_ChallengeID;
}

static bool verifyKey(long long val )
{
	return val == ((4294967291 % ms_ChallengeID) + 17283884);
}


void *LoadCourseData( const char *strname, long long verify )
{
	if (!verifyKey( verify ))
		return NULL;

	CourseData *pc = new CourseData();
	if (pc->Load(strname))
		return (void *)pc;
	delete pc;
	return NULL;
}

void FreeCourseData( void *ptr )
{
	if (ptr != NULL)
		delete (CourseData *)ptr;
}

int GetCourseSegments( void *ptr )
{
	return (ptr != NULL ? ((CourseData *)ptr)->Segments():0);
}

U_GPS_DATA GetCourseSegment( void *ptr, int num )
{
	return ((CourseData *)ptr)->Segment( num );
}
	

//===========================================================

