
/****************************************************************************

****************************************************************************/

#pragma once

#include <windows.h>

#include <config.h>
#include <decoder.h>

class perfDecoder : public Decoder  {

	public:
		perfDecoder(User *_user);
		virtual ~perfDecoder();
		virtual void update(void);					// added so perfSource's perfDecoder could do peaks, avgs, integration

	private:
		float wattseconds;

};
