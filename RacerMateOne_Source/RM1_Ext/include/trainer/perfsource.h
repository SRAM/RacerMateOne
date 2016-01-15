#pragma once

#include <config.h>
#include <datasource.h>
#include <perfdecoder.h>

/**********************************************************************************************

**********************************************************************************************/

class perfSource : public dataSource  {

	private:
		User *user;

	public:
		perfSource(User *_user);
		virtual ~perfSource(void);

		void start(void);
		virtual void reset(void);


};



