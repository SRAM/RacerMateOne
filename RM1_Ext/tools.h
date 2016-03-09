#pragma once

#define NUMAVG 100
class AvgFilter  {
	public:
		DWORD scnt;
		double oldavg;
		double avg;
		double n[NUMAVG];
		DWORD s[NUMAVG];
		int idx;
		int numcnt;
		AvgFilter(void) {reset();}
		virtual ~AvgFilter(void){};
		
		double calc(double in, DWORD frames=1)
		{
			oldavg = avg;
			avg = 0;
			scnt = 0;
			idx++;
			if(idx >= numcnt) idx=0;
			n[idx] = in;
			s[idx] = frames;
			for(int i = 0; i < numcnt; i++) 
			{
				avg += n[i]*s[i];
				scnt += s[i];
			}
			avg /= scnt;
			return avg;
		}
		double getval(void) {return avg;}
		double getoldval(void) {return oldavg;}
		double getlastval(void) {return n[idx];}
		void reset(double val=0,int cnt = 3){numcnt = cnt; for(int i=0; i< numcnt; i++){n[i]=val;s[i]=1;oldavg=avg=val;} idx=0;}
};

class AvgFilterDW  {
	public:
		DWORD scnt;
		DWORD oldavg;
		DWORD avg;
		DWORD n[NUMAVG];
		DWORD s[NUMAVG];
		int idx;
		int numcnt;
		AvgFilterDW(void) {reset();}
		virtual ~AvgFilterDW(void){};
		
		DWORD calc(DWORD in, DWORD frames=1)
		{
			oldavg=avg;
			avg = 0;
			scnt = 0;
			idx++;
			if(idx >= numcnt) idx=0;
			n[idx] = in;
			s[idx] = frames;
			for(int i = 0; i < numcnt; i++) 
			{
				avg += n[i]*s[i];
				scnt += s[i];
			}
			avg /= scnt;
			return avg;
		}
		DWORD getval(void) {return avg;}
		DWORD getoldval(void) {return avg;}
		DWORD getlastval(void) {return n[idx];}
		void reset(DWORD val=0,int cnt = 3){numcnt = cnt; for(int i=0; i< numcnt; i++){n[i]=val;s[i]=1;oldavg=avg=val;} idx=0;}
};
