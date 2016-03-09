WPatch -- Optimized incremental patch system for NSIS (Nullsoft Install System)
Web site: http://wiz0u.free.fr/prog/WPatch
-----------------------------------------------------------------

This patch system consists of 3 tools:
- WGenPat
	Compares 2 versions ("old" and "new") of the same file and creates a chunk of binary data
	describing the changes needed to transform the "old" file into the "new" file
- WGenPatDir
	Compares 2 directories (using WGenPat eventually) and creates a NSIS script that can patch
	the first directory to transform it into the second directory (using WPatch plugin)
- WPatch
	NSIS plugin that use this binary data to convert effectively an "old" version of the file
	into the "new" version of the file.


Original Features
-----------------

In-place patching:
	WPatch is will patch files "in-place" in one pass. It does not create a temporary copy of
	the files to patch, and modifies the original file directly.
	This means the final user don't need extra free disk space, only the final file size is required.

Fast and Precise mode:
	When enabled, for each files to patch, instead of scanning the whole patch database for the file signature,
	it will locate immediately a specific patch information (by offset). File signature is still verified to
	prevent patching a wrong version of the file.
	This mode also allows to patch two identical source files into different target files.
	
Support for huge files:
	By default, a MD5 hash of the whole file is calculated as the file signature to verify that the file is
	really in the patch database. This can lead to a significant slowdown for huge files.
	For those, you can choose to only use the first & last 64K of the file as the signature. In this case,
	make sure that the beginning or end of the file contains data that are unique/specific for each patch
	(typically, I recommend placing a timestamp updated for each version of the huge file)
	
WPatch has been widely tested and performance improved in a real production environment.


Typical Usage
-------------

Just call: WGenPatDir.exe --precise --exclude *.tmp;.svn dir1 dir2
to establish the difference between dir1 and dir2
and, in your NSIS installer script:

Section
	InitPluginsDir
	;...
	; $INSTDIR points to the directory dir1 that is going to be transformed into dir2
SectionEnd
!include WGenPatDir.nsh
Section
	IfErrors 0 +2
		MessageBox MB_OK "There has been some errors !"
	;...
SectionEnd


WGenPat
-------

This program will take a <sourcefile> and a <targetfile> as input and create (or update) a <patchdatabase>.
This patchdatabase then contains the information required to convert a <sourcefile> into <targetfile>,
including the target file modification date/time.
Usage:
	 WGenPat [<options>] <sourcefile> <targetfile> <patchdatabase>

Specific options are: 
	-H	Huge file: Signature will use MD5 checksum over first and last 64K only.
		(otherwise signature will be the MD5 checksum over the whole file)
	-P	Use Fast and Precise update mode.
		In this mode, WGenPat exit code will be the offset to patch information inside the <patchdatabase>.
Run WGenPat without arguments to learn about all the available options & exit codes.


WGenPatDir
----------

The WGenPatDir utility recursively compares two directory structures looking for changes to the files and
subdirectories. It produces files containing instructions that will make your NSIS installer perform a patch 
upgrade from the original structure to the new.

WGenPatDir uses the WGenPat utility to generate the patch database. This binary file contain the delta between
the old and new version of an individual file. When applied to the old file, the file differences are applied
and the file is converted to its new contents.

Note: WGenPatDir is an original software, even if the command-line interface and the functionnalities have been
inspired by NsisPatchGen by Vibration Technology Ltd. WGenPatDir is supposedly faster and more efficient.

Usage: (Make sure WGenPat.exe is available in the current directory or the PATH environment variable)
	WGenPatDir [<options>] <directory1> <directory2>

Options are: 
	--forcediff             : force to check differences between files having same name & date (slower)
	--precise               : enable Fast and Precise mode
	--hidden                : include hidden files in comparison (ignored by default)
	--system                : include system files in comparison (ignored by default)
	--exclude wildcard-list : match filenames to exclude from comparison
	--last wildcard-list    : match filenames to be patched at the end
Run WGenPatDir without arguments to learn about all the available options.

WGenPatDir outputs 2 files:
- WGenPatDir.pat, the patch database
- WGenPatDir.nsh, the NSIS partial script to apply the patch (uses WPatch plugin)


WPatch
------

Call this NSIS plugin to effectively patch each file.
Variables $0 $1 $2 are required to be set before calling the plugin.

WPatch::PatchFile /NOUNLOAD
	Apply patch to source file $0 using options given in $1
	File signature is calculated and searched in the patch database $2.
	If found, the patch is applied and the source file is transformed in its new version (including date/time).
	Error code is returned in $0.

WPatch::PatchFile /UNLOAD
	Call this at the end of your patch script to unload the plugin from memory.
	Returns "0" in $0

Options in $1 can be: (separated by space)
	/CHECK
		In this mode, file signature will be searched/verified against the database but the
		patch will not be applied. Use this before starting to modify the user files.
	/PRECISE nnn
		Enable Fast and Precise mode.
		nnn should be the offset to patch information inside the database
	/HUGE
		Use this for huge files. Signature will use MD5 checksum over first and last 64K only.

Error codes returned in $0:
	-1 : File seems to be already updated. (Its signature matches the target version of the file)
	 0 : Success. File matched and was successfully patched (unless /CHECK mode)
	 1 : Some unspecified/unexpected error occured
	 2 : Patch database is corrupted
	 3 : File signature did not match / was not found in the patch database
	 4 : Out of memory while patching
	 5 : Invalid options/arguments


Changelog
---------

v4.02: More professionnal package & documentation for wide public release
v4.01: WGenPatDir generates a Section instead of Function, to take size of files in account for required disk space
v4.00: Added "Fast and Precise" mode
v3.9: Improved performance and reliability of WGenPat and WGenPatDir
v3.8: Fix bug on "Added directory" in WGenPatDir
v3.7: Added --last option to WGenPatDir. Unloading plugin at end of script
v3.6: Adapted to VC6 compilation so plugin has no specific VC dependencies
v3.5: Adapted to Unicode NSIS
v3.4: Various improvements, bug and crash fix
v3.3: Added /CHECK to check files before patching
v3.2: First version of WPatch system: In-place patching & support for huge files.
v3.1: Imported VPatch 3.1 sources (distributed 'as-is' in source form with NSIS 2)


License
-------
Copyright (c) 2007-2010 Olivier Marcoux

This software is provided 'as-is', without any express or implied warranty. In no event will the authors be held liable for any damages arising from the use of this software.

Permission is granted to anyone to use this software for any purpose, including commercial applications, and to redistribute it freely, subject to the following restriction:

The origin of this software must not be misrepresented; you must not claim that you wrote the original software. If you use this software in a product, an acknowledgment in the product documentation would be appreciated but is not required.

Parts of this software are derived from VPatch, an incremental patch system, distributed 'as-is' in source form with NSIS 2 (Nullsoft Install System) and copyright (C) 2001-2005 Koen van de Sande / Van de Sande Productions (http://www.tibed.net/vpatch)
