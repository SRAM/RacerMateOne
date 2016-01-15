
#pragma once

#include <vector>

using namespace std;

#include <windows.h>
#include <stdio.h>

#include <config.h>
#include <crf.h>
#include <MFD.h>
#include <ergocourse.h>
#include <regularcourse.h>
#include <hue.h>
#include <oldss.h>
#include <user.h>

#define NEWPREVELDATA

class PreVelotronData : public MFD  {

	private:
		HUE hue;
		DWORD inzonecount;
		float wattseconds;
		User *user;
		char lastname[32];
		char firstname[32];
		#ifdef MYDBG
		char pvdfilename[256];
		#endif

	public:
#pragma pack(push, 1)						// for old borland compatibility
		typedef struct {
			unsigned long ssticks;
			unsigned short rpmf;
			unsigned short rpm;
			unsigned short power;
			unsigned short speed;
			unsigned short grade;
			unsigned short hrf;
			unsigned short hr;
			unsigned long raw_distance;
			unsigned short ssraw[24];
		} SS_RECORD;
#pragma pack(pop)

	private:
		int get_next_ss_time(void);
		SS_RECORD ssrec;
		SS_RECORD next_ssrec;
		OldSS *oss;
		unsigned long next_ss_time;
		handlebarDecoder *hbdecoder;

		PRE_CONDITION prec;
		POST_CONDITION postc;

		void cleanup(void);
		vector<ERGOPOINT> ergopoints;
		REGULARLEG *legs;
		int readErgoPoints(int nlegs);
		int readLegs(int nlegs);
		FILE *outstream;
		CRF s;
		int course_type;
		int header_size;
		int user_data_size;
		int course_header_size;
		int course_data_size;
		int file_data_size;
		int ss_record_size;
		void process_ss(void);
		int version;
#ifdef NEWPREVELDATA
	static void keydownfunc(void *object, int index, int _key);
	static void keyupfunc(void *object, int index, int _key);
#endif

	public:
		PreVelotronData(
			char *_fname, 
			RECT *_courserect=NULL,
			bool _enc=true
			);


		virtual ~PreVelotronData(void);

		void testwind(void);

		Course *getCourse(void) { return course;}
		unsigned char getversion(void)  { return version; }
		virtual double getGrade(void);
		virtual char * getFirstName(void);
		virtual char * getLastName(void);

		bool bad;
		void write_course(char *_fname);

		virtual void pause(void) {
			decoder->paused = true;
		}
		virtual void start(void)  {
			MFD::start();
			decoder->paused = false;
			return;
		}

};

