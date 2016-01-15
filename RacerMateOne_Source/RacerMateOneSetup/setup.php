<?php
	$config = "Release";
	$projdir = "d:/_fs/rm1/RacerMateOne_Source";

	date_default_timezone_set('America/New_York');
	$script_tz = date_default_timezone_get();

	$time = time();

	$date = date("Y-m-d H:i:s", $time);
	$hour = date("H", $time);											// starting hour = "00"
	$year = date("Y", $time);
	$month = date("m", $time);
	$day = date("d", $time);


	@unlink("out.txt");

	/******************************************************************************************************
	  PREBUILD
		run:
			d:/_fs/rm1/RacerMateOne_Source/tools/NSIS/bin/rebview.exe -w -s --script ../Images/RebolGenPatch.r Release
		creates:
			../Images/installDB.r
			../Images/installpatch.nsi
			../Images/install.nsi
	******************************************************************************************************/


	$cmd = "$projdir/tools/NSIS/bin/rebview.exe -w -s --script ../Images/RebolGenPatch.r $config";
	printf("cmd = %s\n", $cmd);
	$a = array();
	$status = 0;
	exec($cmd, $a, $status);
	if ($status != 0)  {
		printf("error running rebview, status = %d\n", $status);
		exit;
	}
	dump($cmd, $a);


	/******************************************************************************************************
	  PREBUILD
		run:
			d:/_fs/rm1/RacerMateOne_Source/tools/NSIS/makensis.exe ../Images/install.nsi
		creates:
			../Images/install.exe		5.8 megs
	******************************************************************************************************/

	$cmd = "$projdir/tools/NSIS/makensis.exe ../Images/install.nsi";
	printf("cmd = %s\n", $cmd);
	$a = array();
	$status = 0;
	exec($cmd, $a, $status);
	if ($status != 0)  {
		printf("error running makensis, status = %d\n", $status);
		exit;
	}
	dump($cmd, $a);


	/******************************************************************************************************
	  POSTBUILD
		run:
			copy ..\\tools\\sfxsettings.xml .\\Release
	******************************************************************************************************/

	$cmd = "copy ..\\tools\\sfxsettings.xml .\\$config";
	printf("cmd = %s\n", $cmd);
	$a = array();
	$status = 0;
	exec($cmd, $a, $status);
	if ($status != 0)  {
		printf("sfxsettings.xml copy error status = %d\n", $status);
		exit;
	}
	dump($cmd, $a);

	/******************************************************************************************************
	  POSTBUILD
		run:
			copy ..\\RacerMateOne\\RM1.ico .\Release
	******************************************************************************************************/

	$cmd = "copy ..\\RacerMateOne\\RM1.ico .\\$config";
	printf("cmd = %s\n", $cmd);
	$a = array();
	$status = 0;
	exec($cmd, $a, $status);
	if ($status != 0)  {
		printf("ico copy error status = %d\n", $status);
		exit;
	}
	dump($cmd, $a);


	/******************************************************************************************************
	  POSTBUILD
		run:
			copy ..\\Images\\install.exe .\Release
	******************************************************************************************************/

	$cmd = "copy ..\\Images\\install.exe .\\$config";
	printf("cmd = %s\n", $cmd);
	$a = array();
	$status = 0;
	exec($cmd, $a, $status);
	if ($status != 0)  {
		printf("install.exe copy error status = %d\n", $status);
		exit;
	}
	dump($cmd, $a);

	/******************************************************************************************************
	  POSTBUILD
		run:
			copy ..\Images\installDB.r .\Release

	******************************************************************************************************/

	$cmd = "copy ..\\Images\\installDB.r .\\$config";
	printf("cmd = %s\n", $cmd);
	$a = array();
	$status = 0;
	exec($cmd, $a, $status);
	if ($status != 0)  {
		printf("installDB.r copy error status = %d\n", $status);
		exit;
	}
	dump($cmd, $a);

	/******************************************************************************************************
	  POSTBUILD
		zips up necessary stuff into RacerMateOneSetup.zip in the .\Release directory
	******************************************************************************************************/

	$cmd = "$projdir/tools/7z.exe a -r -tzip .\\$config\\RacerMateOneSetup.zip RacerMateOneSetup.msi setup.exe install.exe installDB.r WindowsInstaller3_1 vcredist_x86 DotNetFX35SP1";
	printf("cmd = %s\n", $cmd);
	$a = array();
	$status = 0;
	exec($cmd, $a, $status);
	if ($status != 0)  {
		printf("7z error status = %d\n", $status);
		exit;
	}
	dump($cmd, $a);

	/******************************************************************************************************
	  POSTBUILD
	  		create .\Release\RacerMateOneSetup.exe (self-extracting zip file from .\Release\RacerMateOneSetup.zip
			RacerMateOneSetup.exe is about 133 megs

			http://www.chilkatsoft.com/ChilkatSfx.asp
		run:
			../tools/ChilkatZipSE.exe -u QTASKCSFX_C4MdmBEH1RrV -cfg sfxsettings.xml -exe .\Release\RacerMateOneSetup.exe .\Release\RacerMateOneSetup.zip
	******************************************************************************************************/

	/////////////////////////////////////////////////////////////////////////////

	// cwd = D:\_fs\rm1\RacerMateOne_Source\RacerMateOneSetup
	// cmd = d:/_fs/rm1/RacerMateOne_Source/tools/ChilkatZipSE.exe -u QTASKCSFX_C4MdmBEH1RrV -cfg .\Release\sfxsettings.xml -exe .\Release\RacerMateOneSetup.exe .\Release\RacerMateOneSetup.zip

	$cwd = getcwd();
	printf("cwd = %s\n", $cwd);

	$projdir = "d:\\_fs\\rm1\\RacerMateOne_Source";
	$unlock_code = "QTASKCSFX_C4MdmBEH1RrV";
	$cfg_file = ".\\$config\\sfxsettings.xml";

	$cmd = "$projdir\\tools\\ChilkatZipSE.exe -u $unlock_code -cfg $cfg_file -exe .\\$config\\RacerMateOneSetup.exe .\\$config\\RacerMateOneSetup.zip";
	printf("cmd = %s\n", $cmd);

	$a = array();
	$status = 0;
	exec($cmd, $a, $status);
	if ($status != 0)  {
		printf("ChilkatZipSE error status = %d\n", $status);
		exit;
	}
	dump($cmd, $a);

	// append ChilkatZipSE.log to out.txt

	$fp = fopen("out.txt", 'a+');
	fprintf($fp, "\n");
	$str = file_get_contents("ChilkatZipSE.log");
	$str = preg_replace("/\x0d/", "", $str);
	fwrite($fp, $str);
	fclose($fp);
	/////////////////////////////////////////////////////////////////////////////

exit;

/**********************************************************************************************************

**********************************************************************************************************/

function dump($cmd, $a)  {
	$fp = fopen("out.txt", "a");
	fprintf($fp, "\n\n******************************\n");
	fprintf($fp, "cmd = %s\n", $cmd);
	fprintf($fp, "******************************\n\n");

	$n = count($a);
	for($i=0; $i<$n; $i++)  {
		fprintf($fp, "%s\n", $a[$i]);
	}
	fclose($fp);
	return;
}


?>


