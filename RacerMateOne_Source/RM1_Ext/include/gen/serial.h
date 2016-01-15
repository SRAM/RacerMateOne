
#ifndef _SERIAL_H_
#define _SERIAL_H_


#include <windows.h>
#include <logger.h>

enum  {
	SERIAL_ERROR_DOES_NOT_EXIST=1,
	SERIAL_ERROR_ACCESS_DENIED,
	SERIAL_ERROR_BUILDCOMMDCB,
	SERIAL_ERROR_SETCOMMSTATE,
	SERIAL_ERROR_CREATEEVENT1,
	SERIAL_ERROR_CREATEEVENT2,
	SERIAL_ERROR_OTHER
};


static const char *serialErrorStrings[SERIAL_ERROR_OTHER] = {
	"SERIAL_ERROR_DOES_NOT_EXIST",
	"SERIAL_ERROR_ACCESS_DENIED",
	"SERIAL_ERROR_BUILDCOMMDCB",
	"SERIAL_ERROR_SETCOMMSTATE",
	"SERIAL_ERROR_CREATEEVENT1",
	"SERIAL_ERROR_CREATEEVENT2",
	"SERIAL_ERROR_OTHER"
};


#define XON		0x11
#define XOFF	0x13
#define TXQLEN 1024
#define RXQLEN 1024


class Serial  {

	private:
		Serial(const Serial&);
		Serial &operator = (const Serial&);		// unimplemented
		char parity[4];								// eg, "N", "E", "O"
		int databits;									// eg, 8
		int stopbits;									// eg, 1
		char portstr[32];
		DWORD bytes_in;
		DWORD bytes_sent;

		Logger *logg;
		BOOL displayErrors;

		char str[256];
		HANDLE writeEvent;
		HANDLE rxThreadExitEvent;
		int comnum;						// 1, 2, 3, 4
		char szPort[16];
		void DisplayOpenError(int iStatus);
		void process_events(DWORD flags);
		BOOL fThreadDone;
		BOOL fWaitingOnRead;
		BOOL fWaitingOnStat;
		DWORD   eventflags;
		BOOL txcontin;
		BOOL txpending;				// indicates whether transmitter thread is
											// awake but waiting for current character to
											// finish being transmitted
	
		unsigned short maxtxdiff;
		unsigned short maxrxdiff;
		DWORD maxin;
		unsigned long writefile_error_count;
		HANDLE hCommPort;
		HANDLE txthrd;
		HANDLE rxthrd;
		HANDLE hresult;
		DWORD id;
		unsigned char txq[TXQLEN];
		unsigned char rxq[RXQLEN];
		static void txThread(void *);
		static void rxThread(void *);
		void GetPort(char *pszPort, int iMaxLen);
		BOOL IsWaitingForXon();
		unsigned int GetCommError();
		void GetCommErrorStr(unsigned int dwCommStatus, char *pszErrStr, int iMaxStr);
		int Open(const char * pszPort, const char *BaudRate, BOOL bDisplayErrors = TRUE);
		void Close(int code);
		unsigned int OutQueueLength();
		void FlushBuffers();
		void Clear();
		unsigned long shutDownDelay;


	public:
		Serial(void);		// constructor 1
		Serial(const char *pszPort, const char *pszBaudRate, const char *_parity, int _databits, int _stopbits, BOOL _displayErrors=TRUE, Logger *_logg=NULL);		// constructor 2
		Serial(const char *pszPort, const char *pszBaudRate, BOOL _displayErrors, Logger *_logg);				// constructor 3
		Serial(const char *pszPort, const char *pszBaudRate, Logger *_logg);		//, bool _displayErrors=true, char *_dummy=NULL);		// constructor 4

		virtual ~Serial();
		void hyperterm(void);

		int rx(char *buf, int buflen);
		void flush(void);
		BOOL txi(unsigned char c);
		BOOL tx(unsigned char c);
		void txx(unsigned char *b, int n);
		BOOL txasleep;
		HANDLE txControlEvent;
		void flushtx(void);
		void flushrx(DWORD ms=0);
		BOOL comopen;
		unsigned short txdiff;
		unsigned short rxdiff;
		unsigned short rxinptr;
		unsigned short rxoutptr;
		unsigned short txinptr;
		unsigned short txoutptr;
		void forcetx(void);
		BOOL IsOpen();
		void setShutDownDelay(unsigned long _ms)  { shutDownDelay = _ms; }

		int getPortNumber(void)  { return comnum; }
		int expect(char *str, DWORD timeout);
		void send(char *str, int flush_flag);
		void txwake(void);
};

#endif			// #ifndef _SERIAL_H_



