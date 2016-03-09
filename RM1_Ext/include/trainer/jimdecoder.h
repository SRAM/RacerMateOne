
#pragma once

#include <windows.h>

#include <config.h>

#include <bike.h>
#include <decoder.h>

#define START_KEY			0x02
#define GRADE_UP_KEY		0x04
#define GEAR_UP_KEY		0x08
#define SHIFT_KEY			0x10
#define GRADE_DOWN_KEY	0x20
#define GEAR_DOWN_KEY	0x40

#define PAUSE_KEYS	(SHIFT_KEY | START_KEY)			// PAUSE/RESUME
#define RESET_KEYS	(SHIFT_KEY | GRADE_UP_KEY)
#define BRAKE_KEYS	(SHIFT_KEY | GEAR_UP_KEY)



class jimDecoder : public Decoder {


	public:
		jimDecoder(int _segments, Course *_course, User *_user);
		virtual ~jimDecoder();
		void decode(unsigned char *packet);

};



