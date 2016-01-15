
#ifndef _USER_H_
#define _USER_H_


#include <stdio.h>

#include <config.h>

#include <crf.h>
#include <ini.h>

typedef struct {
	char name[128];
	char sex[8];
	int age;
	float kgs;
	float lbs;
	float lower_hr;
	float upper_hr;
	BOOL metric;

	char date[16];
	char time[16];
	BOOL draft_enable;

	float drag_aerodynamic;
	float drag_tire;
	unsigned short rfdrag;
} USER_DATA;


	#pragma pack(push, 1)


typedef struct {
	char name[128];
	char sex[8];
	int age;
	float kgs;
	float lbs;
	float lower_hr;
	float upper_hr;
	BOOL metric;

	char date[16];
	char time[16];
	BOOL draft_enable;

	float drag_aerodynamic;
	float drag_tire;
	unsigned short rfdrag;
} OLD_USER_DATA;

	#pragma pack(pop)


class User  {

	private:

#define IDC_USER_NAME					300
#define IDC_USER_AGE						301
#define IDC_USER_WEIGHT					302
#define IDD_SEX_MALE						305
#define IDD_SEX_FEMALE					306
#define IDD_LBS							303
#define IDD_KGS							304
#define IDC_USER_LOWER_HR				307
#define IDC_USER_UPPER_HR				308

		int fsize;							// file size
		float watts_factor;
		void destroy(void);
		char caption[256];
		char fname[256];
		FILE *stream;
		int user_data_size;
		int file_size;
		char ueheader[23];
		OLD_USER_DATA olddata;
		HINSTANCE hinstance;
		HWND phwnd;
		HWND hwnd;

		#ifndef TOPO_APP
		#ifndef BIKE_APP
			static BOOL CALLBACK UserDataDialogProc(HWND hwndDlg, UINT iMessage, WPARAM wParam, LPARAM lParam);
		#endif
		#endif


		void init(void);
		int makeFromRDF(void);
		int makeFromCDF(void);
		void makeFromVDF(void);
		int sanityCheck(void);
		DWORD version;

	protected:
		CRF *s;

	public:
		User(void);
#ifndef TOPO_APP
		User(HINSTANCE, HWND);
#endif

		User(char *_fname);
		User (USER_DATA *);
		virtual ~User();
		USER_DATA data;						// structure compatible with old data files
		void writeEncrypted(FILE *outstream, CRF *s);
		bool canceled;
		void write(FILE *stream);

#ifndef TOPO_APP
		void edit(void);
#endif

		inline float getWeight(void);
		inline float get_watts_factor(void)  { 
			return watts_factor;
		}
		void set_watts_factor(float _f);
		inline int get_version(void)  { return version; }
		float ftp;																// functional threshold power
		inline float get_ftp(void)  { return ftp; }
};


#endif		// #ifndef _USER_H_

