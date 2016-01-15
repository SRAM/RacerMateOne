
#pragma once

#include <config.h>
#include <serial.h>


#include <vector>

#define NEW_HLIST

enum  {
	COMTEST_EXISTS,
	COMTEST_DOES_NOT_EXIST,
	COMTEST_ACCESS_DENIED,
	COMTEST_ERROR,
	COMTEST_MODEM,
	COMTEST_COMPUTRAINER,
	COMTEST_VELOTRON
};

#define NEW_STATUS_WINDOW

#ifdef NEW_STATUS_WINDOW
	#include <statusdlg.h>
#endif

#define N_COMM_PORTS 20


	class CommTest  {

		private:

			HINSTANCE hInstance;
			HWND phwnd;


			#ifdef NEW_STATUS_WINDOW
				statusDlg *statusdlg;
			#else
				HWND hwndStatus;
			#endif


			char gstring[256];
			void flush(DWORD timeout, BOOL echo);
			
			static BOOL CALLBACK StatusDlgProc( HWND hDlg, UINT Message, WPARAM wParam, LPARAM lParam);
			char baudstr[16];
			char comstr[16];
			int baudrate,comport;
			int k;
			Serial *port;

#ifdef NEW_HLIST
#else
			HWND hlist;
#endif
			char testgstring[2][16];
			bool verbose;

		public:
			CommTest(HINSTANCE hInstance=0, HWND _phwnd=0, bool _verbose=true, unsigned long _delay=500);
			void getList(std::vector<int>&list);

			~CommTest();
			void run(void);
			int id[N_COMM_PORTS];
	};
