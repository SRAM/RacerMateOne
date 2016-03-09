

#ifndef _VELOTRONDECODER_H_
#define _VELOTRONDECODER_H_

#ifdef WIN32
	#include <windows.h>
#endif

#include <config.h>

#ifdef VELOTRON

#include <bike.h>

#include <user.h>
#include <heartrate.h>

#include <ini.h>
#include <decoder.h>
#include <physics.h>
#include <course.h>

class velotronDecoder : public Decoder {
	friend class velotronRTD;
	friend class velotronData;
	friend class Calibrator;

	private:
		unsigned long period_timeout;
		unsigned long max_period_timeout;

		#define PAUSE_KEYS	(SHIFT_KEY | START_KEY)			// PAUSE/RESUME
		#define RESET_KEYS	(SHIFT_KEY | GRADE_UP_KEY)
		#define BRAKE_KEYS	(SHIFT_KEY | GEAR_UP_KEY)

		typedef struct  {
			unsigned long version;
			unsigned long sernum;
			unsigned long calibration;
			unsigned long prog_checksum;
			unsigned char fill[238];
			unsigned short nv_checksum;
		} NVRAM;

		bool debug;
		bool course_is_3dc;

		void runkeys(void);
		Physics *physics;

		LPFilter *voltsFilter;
		LPFilter *polarFilter;
		float polarsig;
		double polarStrength;
		double elStrength;
		signalStrengthMeter *earLobeSSM;
		signalStrengthMeter *polarSSM;

		Heartrate *hr;
		bool dbg;
		unsigned short ramaddr;
		unsigned char ramdata;

		unsigned char nvram[256];

		Ini *ini;
		unsigned long d0;
		unsigned long d1;
		unsigned long d2;
		DWORD lastTdcPacket;
		Bike *bike;
		double Vdd;

	public:
		velotronDecoder(Bike *_bike, Physics *_physics=NULL, Course *_course=NULL, User *_user=NULL, int _id=0);
		virtual ~velotronDecoder();
		virtual void set_watts_factor(float _f);
		void decode(unsigned char *packet, DWORD _ms=0);
		virtual void set_grade(float _grade);
		virtual void setDistance(float _meters)  {				// for compatibility with 3d
			Decoder::setDistance(_meters);
			physics->setDistance(_meters);
			return;
		}
		virtual double inline get_measured_ergo_watts(void)  { return physics->get_measured_ergo_watts(); }
		virtual void set_weight_lbs(double _d);


#ifdef VELOTRON
		virtual bool get_physics_pause(void)  { return physics->paused; }
		virtual void set_physics_pause(bool _b)  {
			if (_b)  {
				physics->pause();
			}
			else  {
				physics->resume();
			}
		}
#endif


};

#endif		// ifdef VELOTRON

#endif		// #ifndef _X_H_


