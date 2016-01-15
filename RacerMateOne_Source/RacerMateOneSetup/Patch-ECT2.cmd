if "%1"=="" %0 Debug Release Done 

@SETLOCAL 
rem @set path=%path%;"C:\Program Files\Microsoft Platform SDK\Samples\SysMgmt\Msi\Patching"
rem @set path=%path%;"C:\Program Files\Microsoft SDKs\Windows\v6.1\Bin";"C:\Program Files\Microsoft SDKs\Windows\v6.1\Bin\msitools\Schemas\MSI"
@set path=%path%;"D:\home\racermate\RacermateOne_2\tools\bin";"D:\home\racermate\RacermateOne_2\tools\bin\msitools\Schemas\MSI"
@set PatchTmp=D:\VSTemp2
@set PatchTmpLog=D:\VSTemplog2


:loop 
if "%1"=="Done" goto end 

if not exist %1\*.msi goto nopatch 
if not exist TargetImage\%1\*.msi goto nopatch 

:ok 
rmdir /s /q %PatchTmp% 
mkdir %PatchTmp% 
mkdir %PatchTmp%\TargetImage 
mkdir %PatchTmp%\UpgradedImage 
mkdir %PatchTmp%\Patch 

for %%a in (TargetImage\%1\*.msi) do copy %%a %PatchTmp%\RacerMateOneSetup.msi 
msiexec /qb /a %PatchTmp%\RacerMateOneSetup.msi TARGETDIR=%PatchTmp%\TargetImage /L*v %PatchTmp%\TargetImage\RacerMateOneSetup.log 
del %PatchTmp%\RacerMateOneSetup.msi 

for %%a in (%1\*.msi) do copy %%a %PatchTmp%\RacerMateOneSetup.msi 
msiexec /qb /a %PatchTmp%\RacerMateOneSetup.msi TARGETDIR=%PatchTmp%\UpgradedImage /L*v %PatchTmp%\UpgradedImage\RacerMateOneSetup.log 
del %PatchTmp%\RacerMateOneSetup.msi 

copy patch.pcp %PatchTmp% 
set PatchDir=%CD% 
chdir %PatchTmp% 
msimsp -s patch.pcp -p Patch\RacerMateOneSetup.msp -l Patch\RacerMateOneSetup.log -f %PatchTmp%\Tmp -d 

rmdir /s /q %PatchTmp%\TargetImage 
rmdir /s /q %PatchTmp%\UpgradedImage 
rmdir /s /q %PatchTmp%\Tmp 
chdir %PatchDir% 

mkdir Patch 
mkdir Patch\%1 
copy %PatchTmp%\Patch\*.* Patch\%1\*.* 
rmdir /s /q %PatchTmp% 

cd Patch\%1
copy ..\..\..\tools\sfxsettings.xml .
copy ..\..\..\RacerMateOne\RM1.ico .
copy ..\..\..\RacerMateOne\updatepatch.bat .\install.bat
copy ..\..\..\RacerMateOne\unpatch.bat .\uninstall.bat
..\..\..\tools\7z.exe a -r -tzip RacerMateOneSetup.zip RacerMateOneSetup.msp install.bat uninstall.bat
..\..\..\tools\ChilkatZipSE.exe -u QTASKCSFX_C4MdmBEH1RrV -cfg sfxsettings.xml -exe RacerMateOneSetup.exe RacerMateOneSetup.zip


:nopatch 
shift 
goto loop 

:end 
pause