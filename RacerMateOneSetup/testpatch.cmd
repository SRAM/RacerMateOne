cd Patch\Release\
copy ..\..\..\tools\sfxsettings.xml .
copy ..\..\..\RacerMateOne\RM1.ico .
copy ..\..\..\RacerMateOne\updatepatch.bat .\install.bat
copy ..\..\..\RacerMateOne\unpatch.bat .\uninstall.bat
..\..\..\tools\7z.exe a -r -tzip RacerMateOneSetup.zip RacerMateOneSetup.msp install.bat uninstall.bat
..\..\..\tools\ChilkatZipSE.exe -u QTASKCSFX_C4MdmBEH1RrV -cfg sfxsettings.xml -exe RacerMateOneSetup.exe RacerMateOneSetup.zip
