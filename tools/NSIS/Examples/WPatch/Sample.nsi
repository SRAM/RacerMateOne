; WPatch - Sample script

!system "rmdir /s /Q dir_to_patch"
!system "xcopy /E /Q /H dir1\* dir_to_patch\"

!ifdef TARGETDIR
!addplugindir "${TARGETDIR}"
!else
!addplugindir "..\..\bin"
!endif

Name "Sample WPatch"
OutFile Sample.exe
ShowInstDetails show
InstallDir $EXEDIR\dir_to_patch
Page directory
Page instfiles

Section
	InitPluginsDir
SectionEnd

!include WGenPatDir.nsh

Section
	IfErrors 0 +2
		MessageBox MB_OK "There has been some errors !"
SectionEnd
