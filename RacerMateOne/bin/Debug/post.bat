@echo ************************** RM2 debug post.bat ********************************
::@dir
::@cd
::@copy RacerMateOne.exe "C:\Program Files (x86)\RacerMate Inc\RacerMateOne"
::@copy ..\..\..\..\..\racermate\release\racermate.dll "C:\Program Files (x86)\RacerMate Inc\RacerMateOne"

::nca+++ get this from the local stash
::@copy ..\..\..\..\racermate\release\racermate.dll
::@copy ..\..\..\..\racermate\release\racermate.lib
@copy ..\..\racermate.dll
@copy ..\..\racermate.lib
::nca---

::@copy ..\..\..\..\..\racermate\debug\racermate.dll
::@copy ..\..\..\..\..\racermate\debug\racermate.lib
::@copy ..\..\..\..\..\racermate\debug\racermate.pdb
@echo ********************************************************************************

