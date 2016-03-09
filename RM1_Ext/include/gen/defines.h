
#ifndef _DEFINES_H_
#define _DEFINES_H_

#define WXWID

#define null NULL
#define DISABLE_TOOLBAR_BUTTON(button) { SendMessage(toolbar, TB_SETSTATE, button, MAKELONG(TBSTATE_INDETERMINATE, 0)); }
#define ENABLE_TOOLBAR_BUTTON(button) { SendMessage(toolbar, TB_SETSTATE, button, MAKELONG(TBSTATE_ENABLED, 0)); }


#define DLLExport   __declspec( dllexport ) 

//Detect if there is a zero byte in a 32bit word:
#define has_null_byte(x) (((x) - 0x01010101) & ~(x) & 0x80808080)


// determines if a number is a power of 2:
#define ISPOW2_A(x)  (x && !(x & (x-1)))
#define ISPOW2_B(n)  (!( n & (n-1))
#define ISPOW2_C(x)  ((~(~0U >> 1) | x) & x - 1)



#define DEL(p)  { if(p) { delete (p);     (p)=NULL; } }
#define FCLOSE(stream)  { if((stream)!=NULL) { fclose(stream); (stream)=NULL; } }
#define DELARR(p)  { if(p) { delete [] (p); (p)=NULL;} }
#define TOGGLE(b)  { if(b)  {b = false;} else {b = true; } }

#define SAFE_RELEASE(p) { if(p) { (p)->Release(); (p)=NULL; } }

#ifndef PI
	#define PI 3.1415926535897932384626433832795028841971693993751058209749445923078164
	#define TWOPI (2.0*PI)
#endif

#define ROUND(f) (int)((f) + ((f) >= 0 ? 0.5 : -0.5))


// returns the linear interpolated value b. All values are floats.
// a1, a, a2 is the interval we are interpolating onto the [b1,b2] interval.
// first used in the video project (movie7).
#define FINTERP(a1, a, a2, b1, b2)  ( b1 + ((a - a1) * (b2-b1)) / (a2-a1) )

//-------------
// length:
//-------------

#define FEETTOMETERS		.3048										// exact definition
#define INCHESTOMETERS	(FEETTOMETERS/12.0)					// .3048/12 = .0254

#define METERSTOFEET		(1.0 / FEETTOMETERS)					// as "exact" as it can be
#define METERSTOINCHES		(12.0*(1.0 / FEETTOMETERS))	// as "exact" as it can be

#define CMTOINCHES		(12.0 * METERSTOFEET / 100.0)		// .393701 (= 1/2.54)
#define INCHES_TO_CM		(1.0 / CMTOINCHES)
#define KMTOMILES			(1000.0 / (.3048 * 5280.0))		// approx .62137
#define TOMILES			KMTOMILES

#define MILESTOKM			((.3048 * 5280.0) / 1000.0)		// approx 1.60935
#define TOKM				MILESTOKM
#define MILESTOMETERS	(1000.0 * MILESTOKM)
#define MILESTOFEET		5280.0

#define KMTOFEET			(KMTOMILES*5280.0)					// 3280.8336
#define METERSTOMILES	(KMTOMILES / 1000.0)					// .0006214


//----------
// mass
//----------

#define KGTOLBS 2.2046
#define TOPOUNDS 2.2046
#define KGSTOLBS 2.2046
#define KGSTOPOUNDS 2.2046
#define TOKGS (1.0 / KGTOLBS)
#define POUNDSTOKGS (1.0 / KGTOLBS)
#define POUNDSTOSLUGS 0.03108			// 1 lb / 32.2 fps/s


//-----------
// force
//-----------

#define POUNDSTONEWTONS		4.44824
#define NEWTONSTOPOUNDS		(1.0/POUNDSTONEWTONS)

//-----------
// pressure
//-----------

#define MBAR_TO_PSI .014504					// millibar to psi
#define MBAR_TO_INHG 0.02953				// millibar to inches of mercury

//--------------------------------------------------------------
// acceleration due to gravity conversion of Kgs to Newtons
//--------------------------------------------------------------

#define KGSTONEWTONS		9.80665	// http://www.chemie.fu-berlin.de/chemistry/general/constants_en.html
#define  GMETRIC			9.80665	// http://www.chemie.fu-berlin.de/chemistry/general/constants_en.html

//----------
// torque
//----------

#define NMTOFP (NEWTONSTOPOUNDS*METERSTOFEET)							// NEWTON-METERS TO FOOT-POUNDS .737559
#define FPTONM (1.0/NMTOFP)

//---------------------
// speed conversions
//---------------------

#define KPH_TO_METERS_PER_SEC	(1000.0 / 3600.0)			// kilometers per hour --> meters per second
#define KPHTOMPS					(1000.0 / 3600.0)			// kilometers per hour --> meters per second
#define MPH_TO_METERS_PER_SEC (KPH_TO_METERS_PER_SEC * MPHTOKPH)
#define MPH_TO_FPS (5280.0 / 3600.0)
#define FPS_TO_MPH (3600.0 / 5280.0)
#define MPSTOKPH (3600.0 / 1000.0)							// meters per second --> kilometers per hour
#define MPS_TO_KPH (3600.0 / 1000.0)						// meters per second --> kilometers per hour
#define KPHTOMPH			KMTOMILES
#define TOMPH				KMTOMILES
#define TOKPH				MILESTOKM
#define MPHTOKPH			MILESTOKM
#define MPHTOMPS			( MPHTOKPH * 1000.0 / 3600.0 )
#define MPSTOMPH (3600 / (.3048 * 5280.0) )

//------------
// angle
//------------

#define DEGTORAD (PI / 180.0)
#define RADTODEG (180.0 / PI)

//------------
// power
//------------

#define HP_TO_WATTS	745.699
#define WATTS_TO_HP	(1.0 / 745.699)

//------------
// volume
//------------

#define LITERS_TO_GALLONS .264172051
#define GALLONS_TO_LITERS 3.7854118

//------------
// temperatre
//------------
	
#define C_TO_DEG (9.0/5.0 + 32.0)
#define DEG_TO_C (1.0 / C_TO_DEG)


//-------------------------
// from 3d riderdata.h:
//-------------------------

#define INCHES_TO_METERS(i)	((i) * 0.0254f)
#define FEET_TO_METERS(i)	(((i)*12.0f) * 0.0254f)
#define METERS_TO_FEET(m)	((m) * 3.281f)
#define METERS_TO_INCHES(m)	((m) * (3.281f * 12.0f)) 
#define FEET_TO_MILES(f)	((f) / 5280.0f)
#define METERS_TO_MILES(m)	(((m) * 3.281f) / 5280.0f)					// .000621401
#define MILES_TO_METERS(m)	((m) * 1609.34f)
#define KILOGRAMS_TO_POUNDS(k)	((k) * 2.204586f)
#define POUNDS_TO_KILOGRAMS(p)	((p) * 0.4536f)
#define MILES_TO_KM(m)		((m) * 1.609344f)
#define KM_TO_MILES(m)		((m) * 0.621371192237334f)




#define STATUS_WINDOW	2229

template <class Type> inline Type MIN(Type a, Type b) {
  if(a < b)
    return a;
  return b;
}

template <class Type> inline Type MAX(Type a, Type b) {
  if(a > b)
    return a;
  return b;
}

#define DIALOG_GRAY RGB(212, 208, 200)

#define ACK 0x06
#define NAK 0x15


#endif	// #ifndef _DEFINES_H_

