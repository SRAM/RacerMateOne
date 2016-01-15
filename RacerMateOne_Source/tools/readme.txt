"$(ProjectDir)..\tools\DisableUAC.exe" "$(BuiltOuputPath)" /UAC=Off
copy "$(ProjectDir)..\tools\sfxsettings.xml" .
copy "$(ProjectDir)..\tools\install.bat" .
copy "$(ProjectDir)..\tools\uninstall.bat" .

"$(ProjectDir)..\tools\7z.exe" a -r -tzip RacerMateOneSetup.zip RacerMateOneSetup.msi setup.exe install.bat uninstall.bat

"$(ProjectDir)..\tools\ChilkatZipSE.exe" -u QTASKCSFX_C4MdmBEH1RrV -cfg sfxsettings.xml -exe RacerMateOneSetup.exe RacerMateOneSetup.zip
