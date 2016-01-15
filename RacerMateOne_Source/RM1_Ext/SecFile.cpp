#pragma warning(disable:4996)					// for vista strncpy_s

#include "stdafx.h"
#include <config.h>
#include <config.h>
#include "SecFile.h"
#include <defines.h>
//#include <fatalerror.h>

/***********************************************************************************

***********************************************************************************/

SecFileWrite::SecFileWrite(FILE *file)  {
	m_File = file;
	m_Err = 0;

}

/***********************************************************************************

***********************************************************************************/

SecFileWrite::~SecFileWrite()  {

	m_File = NULL;
}

/***********************************************************************************

***********************************************************************************/

bool SecFileWrite::Write( const void *vdata, int size )  {

	if (m_Err > 0)  {
		return false;
	}

	const char *data = (char *)vdata;

	while(size > 0)  {
		int wsize = (size > 32*1024 ? 32*1024:size);

		if (fwrite(data, wsize, 1, m_File) != 1)  {
			m_Err++;
			return false;
		}

		data += wsize;
		size -= wsize;
	}

	return true;
}

/***********************************************************************************

***********************************************************************************/

long SecFileWrite::GetLoc()  {
	if (m_Err > 0)
		return 0;

	long ans = ftell( m_File );
	if (ans < 0)  {
		m_Err++;
		ans = 0;
	}
	return ans;
}


/***********************************************************************************

***********************************************************************************/

bool SecFileWrite::Push( char *secname )  {

	for(int i=0;i<4;i++)  {
		secstr[i] = (*secname ? *secname++:' ');
	}
	secstr[4] = 0;

	if (Write(secstr, 4))  {								// write the section name
		Write(0);										// dummy data for the size of the section (to be filled in later, in pop())
		long tloc = GetLoc();
		m_Stack.push(tloc);							// push location past size, for pop() later

	}


	return (m_Err == 0);
}

/***********************************************************************************

***********************************************************************************/

bool SecFileWrite::Pop()  {

	if (m_Err > 0)  {
		return false;
	}

	if (!m_Stack.empty())  {
		long tloc = GetLoc();

		while( (tloc % 4) && !m_Err)  {
			char c = 0;
			Write(&c, 1);
			tloc = GetLoc();
		}

		long wloc = m_Stack.top();

		if (fseek( m_File, wloc-4, SEEK_SET ) != 0)  {								// seek back to the size of the section offset
			m_Err++;
		}

		Write(tloc - wloc);

		m_Stack.pop();

		if (fseek( m_File, tloc, SEEK_SET ) != 0)  {									// now seek back past our curent section
			m_Err++;
		}

	}

	return (m_Err == 0);
}


/***********************************************************************************

***********************************************************************************/

bool SecFileWrite::WriteString( const char *str )
{
	int len = strlen(str)+1;
	unsigned char t;
	if (len > 255)
	{
		t = 255;
		Write( &t, 1 );
		Write( str, 254 );
		char t = 0;
		Write( &t, 1 );
	}
	else
	{
		t = (unsigned char)len;
		Write( &t,1 );
		Write( str, t );
	}
	return (m_Err == 0);
}


/***********************************************************************************

***********************************************************************************/

SecFile::SecFile( FILE *file)  {

	m_File = file;
	m_Err = 0;
	m_EOF = false;

	readSec(m_Sec);
}

/***********************************************************************************

***********************************************************************************/

SecFile::~SecFile()  {

	m_File = NULL;
}

/***********************************************************************************

***********************************************************************************/

bool SecFile::readSec( Sec &sec )  {

	sec.id[0] = '\0';

	if (m_Err)  {
		return false;
	}

	if (fread( sec.id, 4, 1, m_File) != 1)  {
		m_EOF = true;
	}

	if (fread( &sec.size, sizeof(long),1,m_File) != 1)  {
		m_EOF = true;
	}

	if (sec.size < 0)  {
		//throw (fatalError(__FILE__, __LINE__));
	}

	sec.id[4] = '\0';
	sec.rsize = 0;
	sec.loc = ftell(m_File);

	return (m_Err == 0);
}


/***********************************************************************************

***********************************************************************************/

bool SecFile::Read( void *vdata, int size )  {
	char *data;

	int status;
	data = (char *)vdata;
	status = fread(data, size, 1, m_File);
	//if (fread(data, size, 1, m_File) != 1)  {
	if (status != 1)  {
		//MessageBox(NULL, "SecFile::Read", "Info", MB_OK);
		m_Err++;
		return false;
	}
	return true;
}

/***********************************************************************************

***********************************************************************************/

bool SecFile::ReadString( char *buf )  {
	unsigned char c;
	Read( &c, 1 );
	return Read( buf, c );
}

/***********************************************************************************

***********************************************************************************/

bool SecFile::NextSection()  {

	if (m_Err || m_EOF)  {
		return false;
	}

	if (fseek( m_File, m_Sec.loc + m_Sec.size, SEEK_SET ) != 0)  {
		m_Err++;
	}

	readSec( m_Sec );

	return (!m_EOF);
}

/***********************************************************************************

***********************************************************************************/

bool SecFile::Push()  {
	m_Stack.push(m_Sec);
	return readSec( m_Sec );
}

/***********************************************************************************

***********************************************************************************/

bool SecFile::Pop()  {

	if (!m_Stack.empty()) {
		if (fseek( m_File, m_Sec.loc + m_Sec.size, SEEK_SET ) != 0)  {
			m_Err++;
		}

		m_Sec = m_Stack.top();
		return true;
	}

	return false;
}

/***********************************************************************************

***********************************************************************************/

bool SecFile::IsSection( const char *name ) const  {
	if (m_Err > 0)
		return false;
	char c[5];
	int i;
	for(i=0;i<4;i++)
	{
		c[i] = (*name ? *name++:' ');
	}
	c[4] = '\0';
	return (strcmp(c,m_Sec.id) == 0);
}


