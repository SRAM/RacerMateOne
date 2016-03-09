
#ifndef _INI_H_
#define _INI_H_

#include <windows.h>

/*********************************************************************
		
*********************************************************************/

class Ini  {

	private:
		char path[256];
		char buf[256];
		char def[256];
		DWORD dw;
		char curdir[128];
		char cursection[64];
		Ini(const Ini&);
		Ini &operator = (const Ini&);		// unimplemented
		int bp;

	protected:
	
	public:

		Ini(const char *_path);
		virtual ~Ini();

		void set_section(const char *_sec);
		bool getBool(const char *key, bool b);
		bool getBool(const char *section, const char *key, bool b);
		bool getBool(const char *section, const char *key, const char * def);
		bool getBool(const char *section, const char *key, const char * def, bool makeIfNotThere);

		float getFloat(const char *section, const char *key, float f);
		float getFloat(const char *section, const char *key, const char *def);
		float getFloat(const char *section, const char *key, const char *def, bool makeIfNotThere);
		float getFloat(const char *key, float f);

		int getInt(const char *section, const char *key, int i, bool _makeIfNotThere=true);
		int getInt(const char *section, const char *key, const char *def);
		int getInt(const char *section, const char *key, const char *def, bool);
		int getInt(const char *key, const char *def);
		int getInt(const char *key, int _i);

		unsigned long get_long_hex(const char *section, const char *key, unsigned long def, bool);
		unsigned long getul(const char *section, const char *key, unsigned long i, bool _makeIfNotThere=true);

		const char * getString(const char *section, const char *key, const char *def);
		const char * getString(const char *section, const char *key, const char *def, bool);
		const char * getString(const char *key, const char *def);

		void writeString(const char *section, const char *key, const char *str);
		void writeBool(const char *section, const char *key, BOOL b);
		void writeBool(const char *key, BOOL b);

		void writeInt(const char *section, const char *key, int i);
		void writeInt(const char *key, int i);

		void writeul(const char *section, const char *key, unsigned long i);
		void write_long_hex(const char *section, const char *key, DWORD _dw);
		void writeFloat(const char *section, const char *key, float f);
		void writeFloat(const char *key, float f);

		const char *getName(void);
		void clear_section(const char *_section);
};

#endif		//#ifndef _INI_H_



