
#pragma warning(disable:4996)

/*************************************************************************************

 *****************************************************************************************/

#include <assert.h>
#include <time.h>

#include <shlobj.h>
#include <conio.h>
#include <psapi.h>
#include <tlhelp32.h>

#include <vector>
#include <string>

#define _CRTDBG_MAP_ALLOC
#include <stdlib.h>
#include <crtdbg.h>

#include <new.h>

//#include <utils.h>
//#include <ergvid.h>
//#include <ev.h>
//#include <computrainer.h>

//#include <vutils.h>
#include <errors.h>
//#include <fatalerror.h>
//#include <err.h>
//#include <globals.h>
//#include <seh.h>
//#include <ini.h>


void cleanup(void);
void trans_func( unsigned int u, EXCEPTION_POINTERS* pExp);
int handle_program_memory_depletion(size_t);


void test_drag_factor(void);

std::vector<std::string> strs;


/*********************************************************************************************************

 *********************************************************************************************************/

int main(int argc, char *argv[])  {
	int rc = 0;
	DWORD start_time;

	start_time = timeGetTime();

	_CrtMemState memstart, memstop, memdiff;
	int flags =0;
	flags = _CRTDBG_REPORT_FLAG;
	flags = _CrtSetDbgFlag(_CRTDBG_REPORT_FLAG);
	flags |= _CRTDBG_CHECK_ALWAYS_DF;
	flags |= _CRTDBG_ALLOC_MEM_DF;
	flags |= _CRTDBG_LEAK_CHECK_DF;
	_CrtSetDbgFlag(flags);
	_CrtSetReportMode(_CRT_ERROR, _CRTDBG_MODE_DEBUG);
	_CrtMemCheckpoint(&memstart);
	//_crtBreakAlloc = 128;

	OutputDebugString("\nBEGINNING PROGRAM...\n\n");


	try  {
		_set_se_translator(trans_func);
		_set_new_handler(handle_program_memory_depletion);
		_set_new_mode(1);


		rc = 0;
		start_time = timeGetTime();

		strs.push_back("x");			// causes memory leak
		strs.clear();

		test_drag_factor();

	}						// try
	catch (const char *str) {
		rc = 1;
		printf("\nexception: %s\n\n", str);
	}
	catch (fatalError & e)  {
		rc = 1;
		printf("\nfatal error\n\n");
	}
	catch (int &i)  {
		rc = 1;
		printf("\nInteger Exception (%d)\n\n", i);
	}
	catch (...)  {
		rc = 1;
		printf("\nUnhandled Exception\n\n");
	}

	cleanup();

	sprintf(gstring, "total time = %.2f seconds", (timeGetTime()-start_time)/1000.0f);
	OutputDebugString("\ncalling _CrtCheckMemory:\n");
	_CrtCheckMemory( );
	OutputDebugString("\ncalling _CrtDumpMemoryLeaks:\n");
	_CrtDumpMemoryLeaks();
	_CrtMemCheckpoint(&memstop);
	OutputDebugString("\ncalling _CrtMemDifference:\n");
	int kk = _CrtMemDifference(&memdiff, &memstart, &memstop);
	if(kk)  {
		printf("kk = %d\n", kk);
		_CrtMemDumpStatistics(&memdiff);
	}
	OutputDebugString("\ndone checking memory corruption\n\n");

	printf("\nhit a key...");

	getch();
	return rc;

}



/**************************************************************************

 **************************************************************************/

void cleanup(void)  {
	int status;

	status = ResetALLtoIdle();
	if (status != ALL_OK)  {
		throw(fatalError(__FILE__, __LINE__));
	}

	return;
}

/**********************************************************************

 **********************************************************************/

void trans_func( unsigned int u, EXCEPTION_POINTERS* pExp)  {
	throw SE_Exception(u, (DWORD)pExp->ExceptionRecord->ExceptionAddress);
}

/**********************************************************************

 **********************************************************************/

int handle_program_memory_depletion(size_t)  {
	throw (fatalError(__FILE__, __LINE__, "Out Of Memory"));
	return 0;
}


/**************************************************************************

 **************************************************************************/

void test_drag_factor(void)  {
	char c;
	int drag_factor = 100;
	int port, ix, fw, cal;
	EnumDeviceType what;
	const char *cptr;
	int status;
	int keys = 0x40;
	int lastkeys=0;
	float watts, mwatts;
	float rpm, hr;
	float mph;
	unsigned long display_count = 0L;
	float *bars;
	float *average_bars;
	float seconds;
	unsigned long start_time;
	unsigned long sleep_ms = 5L;
	float slope;
	float np, iff, tss;
	bool rising_edge = false;
	DWORD now, last_display_time = 0;
	int i;
	float bike_kgs = (float)(TOKGS*20.0f);
	float person_kgs = (float)(TOKGS*150.0f);
	TrainerData td;
	Ini ini("test.ini");


	status = Setlogfilepath(".");
	if (status != ALL_OK)  {
		throw(fatalError(__FILE__, __LINE__));
	}

	bool _bikelog = true;
	bool _courselog = true;
	bool _decoderlog = true;
	bool _dslog = true;
	bool _gearslog = true;
	bool _physicslog = true;


	status = Enablelogs(_bikelog, _courselog, _decoderlog, _dslog, _gearslog, _physicslog);
	if (status != ALL_OK)  {
		throw(fatalError(__FILE__, __LINE__));
	}

	port = ini.getInt("options", "port", 6, true);
	ix = port - 1;

	printf("getting device id\n");

	what = GetRacerMateDeviceID(ix);
	if (what != DEVICE_COMPUTRAINER && what != DEVICE_VELOTRON )  {
		throw(fatalError(__FILE__, __LINE__));
	}

	if (what==DEVICE_COMPUTRAINER)  {
		printf("found computrainer\n");
	}
	else  {
		printf("found velotron\n");
	}

	fw = GetFirmWareVersion(ix);
	if (FAILED(fw))  {
		cptr = get_errstr(fw);
		sprintf(gstring, "%s", cptr);
		throw(fatalError(__FILE__, __LINE__, gstring));
	}
	printf("firmware = %d\n", fw);

	cal = GetCalibration(port-1);
	if (port==COMPUTRAINER_PORT1)  {
		if (cal != 200)  {
			throw(fatalError(__FILE__, __LINE__));
		}
	}

	status = set_ftp(ix, fw, 1.0f);
	if (status!=DEVICE_NOT_RUNNING)  {
		cptr = get_errstr(status);
		throw(fatalError(__FILE__, __LINE__, cptr));
	}

	if (what==DEVICE_VELOTRON)  {
		int Chainrings[MAX_FRONT_GEARS] = {	28, 39, 56, 0, 0 };
		int cogset[MAX_REAR_GEARS] = { 23, 21, 19, 17, 16, 15, 14, 13, 12, 11, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
		float wheeldiameter = 700.0f;				// (float) (INCHES_TO_MM*27.0);
		int ActualChainring = 62;
		int Actualcog = 14;
		float bike_kgs = (float)(TOKGS*20.0f);
		SetVelotronParameters(
				ix,
				fw,
				3,
				10,
				Chainrings, 
				cogset, 
				wheeldiameter,			// mm
				ActualChainring, 
				Actualcog,
				bike_kgs,
				//person_kgs,
				2,						// 2, start off in gear 56/11
				9						// 9, start off in gear 56/11
				);
	}


	status = startTrainer(ix);										// start computrainer
	if (status!=ALL_OK)  {
		cptr = get_errstr(status);
		throw(fatalError(__FILE__, __LINE__, cptr));
	}

	status = set_ftp(ix, fw, 1.0f);
	if (status!=ALL_OK)  {
		cptr = get_errstr(status);
		throw(fatalError(__FILE__, __LINE__, cptr));
	}

	printf("computrainer started\n");

	slope = 0.0f;

	status = SetSlope(ix, fw, cal, bike_kgs, person_kgs, drag_factor, slope);
	if (status!=ALL_OK)  {
		cptr = get_errstr(status);
		throw(fatalError(__FILE__, __LINE__, cptr));
	}


	start_trainer(ix, true);										// set started

	Sleep(1000);											// wait a little for threads to prime the data

	keys = lastkeys = GetHandleBarButtons(ix, fw);			// keys should be accumulated keys pressed since last call


	ResetAverages(ix, fw);
	setPause(ix, false);

	start_time = timeGetTime();

	while(1)  {
		if (kbhit())  {
			c = getch();
			if (c==0)  {
				c = getch();
				continue;
			}
			if (c==VK_ESCAPE)  {
				bp = 1;
				break;
			}
			else if (c==VK_UP)  {
				bp = 2;
			}
			else if (c==VK_RIGHT)  {
				bp = 2;
			}
			else  {
				bp = 3;
			}
		}

		now = timeGetTime();

		td = GetTrainerData(ix, fw);
		watts = td.Power;
		mph = (float)(TOMPH*td.kph);
		rpm = td.cadence;
		hr = td.HR;
		keys = GetHandleBarButtons(ix, fw);					// keys should be accumulated keys pressed since last call
		bars = get_bars(ix, fw);
		average_bars = get_average_bars(ix, fw);

		// it takes 30 seconds for non-zero values to show up

		np = get_np(ix, fw);
		iff = get_if(ix, fw);
		tss = get_tss(ix, fw);

		int bits = get_status_bits(ix, fw);

		if (keys && (keys ^ lastkeys))  {
			rising_edge = true;
		}

		int ddrag = 100;

		if (rising_edge)  {
			rising_edge = false;
			switch(keys)  {
				case 0x00:
					bp =  3;
					break;
				case 0x40:						// not connected
					break;

				case 0x01:						// reset
					bp =  0;
					break;
				case 0x04:						// f2
					bp =  1;
					break;
				case 0x10:						// +
					if (what==DEVICE_COMPUTRAINER)  {
						drag_factor += ddrag;
						status = SetSlope(ix, fw, cal, bike_kgs, person_kgs, drag_factor, slope);
						if (status!=ALL_OK)  {
							cptr = get_errstr(status);
							throw(fatalError(__FILE__, __LINE__, cptr));
						}
					}
					break;
				case 0x02:						// f1
					if (what==DEVICE_VELOTRON)  {
						drag_factor += ddrag;
						status = SetSlope(ix, fw, cal, bike_kgs, person_kgs, drag_factor, slope);
						if (status!=ALL_OK)  {
							cptr = get_errstr(status);
							throw(fatalError(__FILE__, __LINE__, cptr));
						}
					}
					bp =  1;
					break;
				case 0x08:						// f3
					bp =  1;
					break;
				case 0x20:						// -
					if (what==DEVICE_COMPUTRAINER)  {
						drag_factor -= ddrag;
						if (drag_factor<0)  {
							drag_factor = 0;
						}
						status = SetSlope(ix, fw, cal, bike_kgs, person_kgs, drag_factor, slope);
						if (status!=ALL_OK)  {
							cptr = get_errstr(status);
							throw(fatalError(__FILE__, __LINE__, cptr));
						}
					}
					break;
				default:
					bp =  99;
					break;
			}
		}

		lastkeys = keys;


		if ( (now - last_display_time) >= 500)  {
			last_display_time = now;
			seconds = (float) ((now - start_time)/1000.0f) ;
			printf("%6.1f    %04X     %12d  %.2f   %.2f   %.2f\n",seconds, bits & 0x0000ffff, drag_factor, tss, iff, np);
		}


		Sleep(sleep_ms);
	}


	status = stopTrainer(ix);
	if (status!=ALL_OK)  {
		cptr = get_errstr(status);
		throw(fatalError(__FILE__, __LINE__, cptr));
	}

	printf("computrainer stopped\n");


	return;
}							// test_drag_factor()


