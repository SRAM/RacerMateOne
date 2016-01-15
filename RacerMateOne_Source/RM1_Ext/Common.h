#ifndef _COMMON_H_

#undef PRODUCT_ID
#undef REG_DIALOG_ID
#undef TITLE_TEXT
#undef DEFGLOW
#undef KEYCODE_NUM_CDKEYS
#undef KEYCODE_CD_FILESIZE
#undef KEYCODE_CD_VOLUME_NAME
#undef KILL_KEYCODE_TEST
#undef KEYCODE_CD_KEYCODE_KEY
#undef CD_DAT_NAME
#undef LOOKUPLAST
#undef HARDWAREREGPATH
#undef SOFTWAREREGPATH
#undef TEMPHARDWAREREGPATH
#undef TEMPSOFTWAREREGPATH

#define _COMMON_H_

// ********************************************************
// ECT-todo Comment out next line when ready for release
//  #define CHEAT
// ********************************************************

// NOTE: MA type uses KEYCODE_ prefix, GA type uses KEYCODE_ prefix

#define TESTDAYSEXPIRED 20
// change DAYSEXPIRED from 7 dys to 14 per Reichart
#define DAYSEXPIRED 14
#define MAGICMUNGENUMBER    735914
//#define TRIALHARDWAREREGPATH "Software\\Classes\\PPIApplication\\PRODUCT_ID"

#if (defined _RM1_)

	#define PRODUCT_ID             3 //17
    #define REG_DIALOG_ID       IDD_REG

    #define KEYCODE_NUM_CDKEYS      100     // Numbers of copys of the CD key on the disk.
    #define KEYCODE_CD_FILESIZE     247283542
    #define KEYCODE_CD_VOLUME_NAME  "RM1"

    #define KILL_KEYCODE_TEST   "KILLA-47LX2-VS252-62IHR"
    #define KEYCODE_CD_KEYCODE_KEY  "CDRMA-46K54-FAG52-62VPV"   // Don't Use in program.

    #define CD_DAT_NAME         "rm1.dat"

    #define HARDWAREREGPATH     "Software\\RacerMate,Inc\\RacerMateOne"
    #define SOFTWAREREGPATH     "Software\\RacerMate,Inc\\RacerMateOne"
    #define TITLE_TEXT          "RacerMateOne"

#else

    #error ERROR: Need to define a program type.

#endif

#endif
