!ifndef ADDEDSOURCE
!define ADDEDSOURCE 'dir2\'
!endif

Function Patch
	DetailPrint 'Patch: $0'
	StrCpy $0 '$INSTDIR\$0'
retry:
	WPatch::PatchFile /NOUNLOAD	; expects $0:file path, $1:options, $2:patch path
	IntCmp $1 0 continue can_skip 0
		SetErrors
can_skip:
		SetDetailsPrint listonly
		DetailPrint '=> Error $1'
		SetDetailsPrint both
		IntCmp $1 1 0 continue continue
			MessageBox MB_RETRYCANCEL|MB_ICONEXCLAMATION $(^FileError_NoIgnore) /SD IDCANCEL IDRETRY retry
			Abort
continue:
FunctionEnd

Section 'ApplyPatch'
ClearErrors
SetOutPath '$PLUGINSDIR'
File WGenPatDir.pat
StrCpy $2 '$PLUGINSDIR\WGenPatDir.pat'
DetailPrint 'Checking before patch...'

StrCpy $0   'changedDir\changedFiles\changedFile.txt' 	; Check modified file
StrCpy $1 '/CHECK /PRECISE 12'
Call Patch
StrCpy $0   'changedFile.bin' 	; Check modified file
StrCpy $1 '/CHECK /PRECISE 117'
Call Patch
StrCpy $0   'changedFile.txt' 	; Check modified file
StrCpy $1 '/CHECK /PRECISE 207'
Call Patch
StrCpy $0   'changedFile_hadSameContent.txt' 	; Check modified file
StrCpy $1 '/CHECK /PRECISE 313'
Call Patch

IfErrors 0 +3
	SetErrors
	Goto end_of_patch
DetailPrint 'Beginning real patch...'

StrCpy $0   'changedDir\changedFiles\changedFile.txt' 	; Modified file
StrCpy $1 '/PRECISE 12'
Call Patch
File '/oname=$INSTDIR\changedDir\newFiles\newFile.txt' 	'${ADDEDSOURCE}changedDir\newFiles\newFile.txt' 	; Added file
Delete      '$INSTDIR\changedDir\removedFiles\removedFile.txt' 	; Removed file
StrCpy $0   'changedFile.bin' 	; Modified file
StrCpy $1 '/PRECISE 117'
Call Patch
StrCpy $0   'changedFile.txt' 	; Modified file
StrCpy $1 '/PRECISE 207'
Call Patch
StrCpy $0   'changedFile_hadSameContent.txt' 	; Modified file
StrCpy $1 '/PRECISE 313'
Call Patch
SetOutPath  '$INSTDIR\newDir' 	; Added directory
File /r /x *.tmp /x .svn 	'${ADDEDSOURCE}newDir\*'
CreateDirectory '$INSTDIR\newDirEmpty' 	; Added empty directory
File '/oname=$INSTDIR\newFile.txt' 	'${ADDEDSOURCE}newFile.txt' 	; Added file
RMDir /r    '$INSTDIR\removedDir' 	; Removed directory
RMDir /r    '$INSTDIR\removedDirEmpty' 	; Removed directory
Delete      '$INSTDIR\removedFile.txt' 	; Removed file
Delete      '$INSTDIR\smiley-face.GIF' 	; Removed file

end_of_patch:
WPatch::PatchFile /UNLOAD
Delete $2
; Now you should check for IfErrors ...
SectionEnd

