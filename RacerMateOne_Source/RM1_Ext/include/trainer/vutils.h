
#ifndef _VUTILS_H_
#define _VUTILS_H_


#include <stdio.h>

#include <config.h>

typedef struct  {
	unsigned long version;
	char coursename[256];
	int course_section_count;
	float course_length_in_meters;
	int number_of_perfpoints;
} COURSE_SUMMARY;

void loadnew(char *fname);

unsigned long make_section_code(const char *str);
COURSE_SUMMARY *get_course_summary(const char *fname);


#include <serial.h>

bool isVelotron(Serial *port, unsigned long _delay);

void dump_tmpfile(char *fname);

#ifdef VELOTRON
	bool wueh(FILE *outstream, bool oldformat=false);
	bool ruh(FILE *stream);
#else
#endif		// #ifdef VELOTRON

#endif		// #ifndef _VUTILS_H_
