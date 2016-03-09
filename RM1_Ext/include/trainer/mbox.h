
#pragma once

#include <windows.h>

#include <config.h>
#include <datasource.h>								// <<<<<<<<<<<<<<<< culprit
#include <vdefines.h>
#include <button.h>

class Mbox  {
	public:
		enum  {
			YES_BUTTON,
			NO_BUTTON,
			CANCEL_BUTTON
		};
		Mbox(HWND _phwnd, char *_title, char *_txt, dataSource *_ds, unsigned long _style, bool _cebtered=true);
		~Mbox();
		int getResult(void);
		int run(void);

	private:
		#define MBOXCLASS "mbox_class"
		bool aborting;
		bool wait_for_all_keys_released;
		bool done;

#ifdef VELOTRON
		bool physics_was_paused;
#endif


		bool was_paused;
		HWND phwnd;
		HWND hwnd;
		char title[32];
		static LRESULT CALLBACK proc(HWND hwnd, UINT uMsg, WPARAM wParam, LPARAM lParam);
		HINSTANCE hinstance;
		int button;
		dataSource *ds;
		bool shiftkey;
		HFONT hfont, hfontold;
		HDC hdc;
		char str[256];									// the text string

		void *saved_object;
		void (*saved_keydownfunc)(void *object, int index, int _key);	// function pointer to application level function
		void (*saved_keyupfunc)(void *object, int index, int _key);		// function pointer to application level function

		static void keydownfunc(void *object, int index, int _key);
		static void keyupfunc(void *object, int index, int _key);

#ifndef VELOTRON
		void computrainerKeys(void);
#else
		void velotronKeys(void);
	#ifndef DOUPKEY
		bool downclear;
		bool upclear;
	#endif
#endif

		Button *yesbutton;			// also used as ok button
		Button *nobutton;
		Button *cancelbutton;
		unsigned long style;

};


