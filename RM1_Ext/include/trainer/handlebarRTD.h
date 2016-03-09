#ifndef _HANDLEBARRTD_H_
#define _HANDLEBARRTD_H_

#include <windows.h>

#include <config.h>

#ifndef VELOTRON

#include <rtd.h>
#include <handlebardecoder.h>

#ifdef COMPAT_3DP
#else
	#include <crf.h>
	#include <hue.h>
	#include <twopolefilter.h>
#endif

class handlebarRTD : public RTD {
	friend class scrollingChart;
	friend class Computrainer;
	friend class BACourse;
	friend class Mbox;

	protected:
		virtual void createOutputFile(void);

	private:
		#define	HB_WIND_RUN		0x2c
		#define	HB_WIND_PAUSE	0x28
		#define	HB_RFTEST		0x1c
		#define	HB_ERGO_RUN		0x14
		#define	HB_ERGO_PAUSE	0x10

		#ifdef MYDBG
		xxxxxxxxxxxxx
		FILE *dbgstream;
		char cdfFileName[256];
		#endif

		int lastGrade_i;
		int lastWind_i;
		unsigned char txbuf[6];
		unsigned char pkt_mask[6];
		unsigned char is_signed;

		handlebarDecoder *hbdecoder;
		float manualWattsStep;
		float manualWatts;
		bool done_finished;

#ifdef COMPAT_3DP
#else
		twoPoleFilter::STATE tpfs[24];									// save the current spinscan twopole bar filters
		PRE_CONDITION prec;
		POST_CONDITION postc;
		long postcOffset;
		long precOffset;
		long userOffset;
		long courseOffset;
		long filterOffset;

		TCRF pces;													// Post Condition Encryption State
		TCRF dses;													// data start encryption state
		TCRF user_estate;											// state of the encryptor at the user data offset
		HUE hue;
		CRF *sout;
#endif

		handlebarDecoder::RAW_COMPUTRAINER_LOGPACKET lp;
		long file_size;
		void destroy(void);
		void init(void);
		unsigned char packet[16+1];
		DWORD skips;
		unsigned char tx_select;
		unsigned char control_byte;
		DWORD lastWriteTime;
		int rr;
		void setControlByte(void);

	public:

		handlebarRTD(
				const char *_comPortString, 
				User *_user, 
				bool metric, 
				Course *_course=NULL, 
				int _appcode=-1, 
				float _manualWattsStep=0.0f, 
				const char *_dataFileName=NULL, 
				const char *_logfname="ds.log", 
				int _id=0,
				bool _show_errors=true);

		~handlebarRTD(void);
		virtual float getConstantWatts(void);
		int getNextRecord(DWORD _delay);
		inline float getManualWatts(void)  { return manualWatts; }
		virtual int updateHardware(bool _force=false);
		int getPreviousRecord(DWORD _delay);
		void gradeUp(void);
		void gradeDown(void);
		double getGrade(void);
		virtual void pause(void);
		void resume(void);
		void start(void);
		virtual void reset();
		void setConstantWatts(float _constantWatts);
		virtual void setwind(float _wind)  {
			decoder->meta.wind = _wind;
		}
		virtual void setdraftwind(float _draft_wind)  {
			decoder->draft_wind = _draft_wind;
			return; 
		}
		virtual float getdraftwind(void)  {
			return (float)decoder->draft_wind;
		}

		void dorr(void);

		virtual void finish(void);
#ifndef NO_PERFS
		virtual char * save(bool lastperf=false, bool _showDialog=true);
#endif

		virtual void flush(void);
		virtual unsigned char getControlByte(void)  {
			return control_byte;
		}

};


#endif		// ifndef VELOTRON
#endif		//#ifndef _HANDLEBARRTD_H_


