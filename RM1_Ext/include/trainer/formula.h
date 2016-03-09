
#ifndef _FORMULA_H_
#define _FORMULA_H_

#include <windows.h>

#include <deque>

#include <defines.h>
#include <config.h>


typedef struct  {
	unsigned long ms;
	float watts;
} FORMULA_PAIR;

class Formula  {

	private:

		static int count;								// instances;
		char str[256];

		double fourth_total;
		unsigned long fourth_count;
		float fourth_avg;

		int bp;

		int id;											// instance number, 0, or 1
		float np;
		float IF;
		float tss;

		
		std::deque<FORMULA_PAIR> dq;

		double total_watts;
		float avg_watts;

		float ftp;


	public:
		Formula(float _ftp=0.0f);
		virtual ~Formula();
		void reset(void);
		void calc(DWORD time, float watts);

		void set_ftp(float _ftp)  { ftp = _ftp; }
		float get_ftp(void)  { return ftp; }

		float get_np(void)  { return np; }
		float get_tss(void)  { return tss; }
		float get_if(void)  { return IF; }
		float get_avg_watts(void)  { return avg_watts; }
};

#endif

