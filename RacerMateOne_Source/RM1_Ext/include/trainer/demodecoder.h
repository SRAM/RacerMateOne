
/****************************************************************************


****************************************************************************/

#pragma once

#include <windows.h>

#include <config.h>
#include <decoder.h>

class demoDecoder : public Decoder  {

	public:
		demoDecoder(User *_user);
		virtual ~demoDecoder();
		virtual void update(void);					// added so demoSource's demoDecoder could do peaks, avgs, integration
		virtual void update(float _watts);		// used by compupacer

	protected:

	private:
		float wattseconds;

};
