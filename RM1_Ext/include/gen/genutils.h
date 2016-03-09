
#pragma once

#include <windows.h>
#include <sql.h>
#include <logger.h>

bool SQLSuccess(SQLRETURN ret);
bool SQLFailed(SQLRETURN ret);
SQLRETURN errprint(SQLSMALLINT htype, SQLHANDLE hndl /*, SQLRETURN rc*/);

void getDisplayModes(Logger *logg);
int dec(char *fname);
void negets(char *, int);

int memfind(char *buf1, char *buf2, int len1, int len2);
char *browseForFolder(HWND hwnd, char *caption);
void storeStr(char *_str, FILE *_stream);
void getStr(char *_str, int _maxlen, FILE *_stream);


