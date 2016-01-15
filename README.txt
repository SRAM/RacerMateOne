Source from the 3.2.153 Patch.  
- Set the installer to do a full build
- Set the version number to be 3.2.154,   
- Added DirectX includs and lib into compile path so that the sdk does not need to be installed.

Steps to compile
 - Compile using Visual Studio 8.   
 - Any path to the directory should work.
 - Open Solution RacerMateOne_Source/RacerMateOne.sln.  This has all the projects needed to build the current version.

For some reason when building the debug or the release version for the first time you might get a IO error when you attempt to run it.  If this happens:
  - Right click the solution and select rebuild.
  - After it builds again it seems to run.

===============================================
To Build a full version installer
===============================================
- Open up the "RacerMateOne/Properties/AssemblyInfo.cs" file
- Edit these two lines to be the new version number
  [assembly: AssemblyVersion("3.2.153")]
  [assembly: AssemblyFileVersion("3.2.153")]
- Click on RacerMateOneSetup Project
- In the RacermateOneSetup Properties window edit the "Version" entry to the new version number.
	- When you enter a new number for the version field this dialog will show up.
		"It is recommended that the ProductCode be changed if you change the version. 
		Do you want to do this?"
	- Select [NO].  If you select yes thing will have problem installing... never select yes.
- Edit the "Images\racermateone-cfg.r" file. In this file you will find:
	; change buildtype if full or patch 
	buildtype: "full"    
	;; buildtype: "patch"    

	; change base version and current version 
	version: 3.2.153
	; NOTE: Remember to change baseversion to match version when doing a full build
	baseversion: 3.2.153
	
	- If you are making a full version
		- comment out the "patch" line and uncomment the "full" line (use ;;)
		- change the "baseversion" and "version" to the same number as you set your version to above.
	- If you are making a patch.
		- comment out the "full" line and uncomment the "patch" line (use ;;)
		- change only the "version" to the new version.
- Right click on the "RacerMateOneSetup" program and click "ReBuild".
- RacerMateOneSetup\Release\RacerMateOneSetup.exe
   - For a release it will be around 124 megs.
   - For a patch usually 5 to 20 megs depending on the changes.
   
-- NOTE: The build process updates an installDB.r file.   Keep this around to
correctly build the susequent patches.

-- Also note the first time you run this it may ask you to register rebol.
Just answer the questions then rebuild the patch again.

