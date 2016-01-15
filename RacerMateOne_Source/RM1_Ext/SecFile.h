
#pragma once

#include <stdio.h>
#pragma warning(disable:4786)
#include <stack>

#include <config.h>



#ifdef DO_ENCRYPTION
	#include <crf.h>
#endif

/*********************************************************************************
	constructor
*********************************************************************************/

class SecFileWrite  {

	private:
		FILE *m_File;
		int m_Err;
		std::stack<long> m_Stack;
		int bp;
		char secstr[5];
		SecFileWrite(const SecFileWrite&);
		SecFileWrite &operator = (const SecFileWrite&);		// unimplemented

	public:
		SecFileWrite(FILE *file);
		virtual ~SecFileWrite();

		bool Write( const void *data, int size );
		bool Write( long data )						{ return Write( &data, sizeof(long) ); }
		bool WriteFloat( float data )				{ return Write( &data, sizeof(float) ); }
		bool WriteString( const char *str );
		bool Push( char *secname );
		bool Pop();
		long GetLoc();

		bool IsOK()		{ return (m_Err == 0); }
};


/*********************************************************************************

*********************************************************************************/

class SecFile  {
#ifdef _DEBUG
	friend class CCourse;
	friend class Section;
#endif

	public:
		FILE *m_File;

	private:
		int m_Err;
		bool m_EOF;

		struct Sec  {
			char id[5];
			long size;
			long rsize;
			long loc;
		};
		Sec m_Sec;

		std::stack<Sec> m_Stack;
		bool readSec(Sec &s);

#ifdef DO_ENCRYPTION2
		bool ok;									// obfuscated "encrypted"
		CRF *crf;
#endif
		SecFile(const SecFile&);
		SecFile &operator = (const SecFile&);		// unimplemented

	public:
#ifdef DO_ENCRYPTION2
		SecFile( FILE *file, bool _ok=false );
#else
		SecFile( FILE *file);
#endif
		virtual ~SecFile();
		bool NextSection();
		bool IsSection( const char *name ) const;
		bool Read(void *data, int size);
		bool ReadString( char *buf );
		bool Push();
		bool Pop();

		char *GetSectionName()  {
			return m_Sec.id; 
		}

		bool IsOK()	const  { 
			return (m_Err == 0); 
		}

		bool eof() const  {
			return m_EOF; 
		}

		bool Read(long& data)  { 
			return Read(&data, sizeof(long)); 
		}

		bool Read(float& data)  { 
			return Read( &data, sizeof(float)); 
		}

		long ReadLong()  {
			long i = 0; 
			Read(&i, sizeof(long)); return i; 
		}

		float ReadFloat()	{ 
			float f = 0.0f; 
			Read(&f, sizeof(float)); return f; 
		}

		long GetLoc() {
			return ftell(m_File); 
		}
};

