
#ifndef _BEEP2_H_
#define _BEEP2_H_

#include <mmsystem.h>
#pragma warning(disable:4996)					// for vista strncpy_s


#include <utils.h>
#include <fatalerror.h>

class Beep2  {

	private:
		FILE *stream;
		int size;
		char *beepSound;
		int status;

	public:

		/**********************************************************************

		**********************************************************************/

		Beep2(void)  {
			int i;

			stream = fopen("beep.wav","rb");
			if (stream==NULL)  {
				//throw("Can't find beep.wav");
				throw(fatalError(__FILE__, __LINE__, "Can't find beep.wav"));
			}

			fclose(stream);
			size = (int) filesize_from_name("beep.wav");
			beepSound = new char[size];					// beepSound is char *

		   if (beepSound==NULL)  {
				//throw("Can't allocate beep memory");
				throw(fatalError(__FILE__, __LINE__, "Can't allocate beep memory"));
			}

			stream = fopen("beep.wav","rb");

			for(i=0;i<size;i++)  {
  				status = fgetc(stream);
     			if (status==EOF)  {
     				break;
		      }
		      beepSound[i] = (char) status;
			}

        	fclose(stream);
		}

		/**********************************************************************

		**********************************************************************/

		~Beep2()  {
			delete[] beepSound;
			beepSound = NULL;
		}

		/**********************************************************************

		**********************************************************************/

		bool play(void)  {

			if (beepSound)  {
	#ifdef UNICODE
		//		status = PlaySound(beepSound,NULL,SND_ASYNC | SND_MEMORY);
	#else
				status = PlaySound(beepSound,NULL,SND_ASYNC | SND_MEMORY);
	#endif
				if (status==FALSE)  {
   				return false;
				}
			}

			return true;
		}

};


#endif		// #ifndef _BEEP2_H_

