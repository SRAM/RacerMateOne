
/****************************************************************************

****************************************************************************/

#ifndef _HANDLEBARDECODER_H_
#define _HANDLEBARDECODER_H_

#include <windows.h>

#include <config.h>

#include <logger.h>
#include <decoder.h>
#include <user.h>
#include <twopolefilter.h>

class handlebarDecoder : public Decoder  {
	friend class PreVelotronData;
	friend class handlebarRTD;
	friend class SFD;
	friend class BACourse;

	public:
		// do not pack this structure so that we're compatible with the old charts .cdf files
		//#pragma pack(push, 1)
		typedef struct {
			unsigned long time;
			unsigned char buf[6];
		} RAW_COMPUTRAINER_LOGPACKET;
		//#pragma pack(pop)

	private:

		void runkeys(void);

		#define DT .030295900					// determined empirically from avgtimer->update()

		#define RPM_VALID	0x08
		#define XORVAL 0xff

		DWORD lastms;

		twoPoleFilter **tpf;

		unsigned char newmask[6];
		virtual void peaks_and_averages(void);
		DWORD lastFilterTime;
		int sscount;
		unsigned char ssbuf[24];
		unsigned char ssraw[24];

		unsigned long lastmphtime;
		float unpack(unsigned short p);
		unsigned char last_keys;
		void do_ss_filter(void);
		double wattseconds;
		void gradeUp(void);
		void gradeDown(void);

		DWORD startTime;
		DWORD pausedTime;				// accumulates paused time
		DWORD pauseStartTime;

	public:
		handlebarDecoder(Course *_course=NULL, User *_user=NULL, int _id=0);
		~handlebarDecoder();

		virtual void set_watts_factor(float _f);
		void decode(unsigned char *packet, DWORD _ms=0);
		virtual void integrate(void);					// added for 3d software to stop jerking

		unsigned char ssRaw[24];
		unsigned char ssRawDisplay[24];
		unsigned char hb_DLE;
		bool ssFound;
		unsigned char HB_ERGO_RUN;
		unsigned char HB_ERGO_PAUSE;
		unsigned short heartRateFlags;
		unsigned short minHeartRate;
		unsigned short maxHeartRate;
		unsigned short hbStatus;
		virtual void reset(void);

};

#endif		//#ifndef _HANDLEBARDECODER_H_



