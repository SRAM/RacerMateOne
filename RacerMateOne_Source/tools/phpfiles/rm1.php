<?php

	if ($argc!=2)  {
		printf("syntax is 'php rm1.php enc|dec\n");
		exit;
	}
	$op = $argv[1];					// 'enc' or 'dec'
	//$op = 'dec';
	//$op = 'enc';
	if ($op != 'enc' && $op != 'dec')  {
		printf("%s is illegal\n", $op);
		exit;
	}
	require_once("arrays.php");
	require("utils.php");
	require("config.php");

	$files_to_obfuscate = array (
		"RM1.cs",
		 //"Statistics.cs",
		 //"Unit.cs",
		 //"HardwareLine.xaml.cs",
	);

	$files = array();
	$recurse = true;

	get_files($rm1srcdir, "/\.*/", $recurse);

	print_r($files);

	$n = count($files);

	for($k=0; $k<$n; $k++)  {
		$fname = $files[$k]['fullpath'];
		$basename = $files[$k]['basename'];
		$path = $files[$k]['path'];
		$extension = $files[$k]['extension'];

		$s = file_get_contents($fname);

		$changed = false;

		if ($op=='enc')  {
			foreach($encarr as $key => $value)  {
				if (strpos($s, $key))  {
					$s = str_replace($key, $value, $s);
					$changed = true;
				}
			}

			if ($changed)  {											// save a copy of the unencrypted file
				$backup_name = $backupdir . $basename;
				copy($fname, $backup_name);
				file_put_contents($fname, $s);			// write the altered file
			}
		}
		else  if ($op=='dec')  {
			foreach($decarr as $key => $value)  {
				if (strpos($s, $key))  {
					$s = str_replace($key, $value, $s);
					$changed = true;
				}
			}
			if ($changed)  {
				file_put_contents($fname, $s);			// write the altered file
			}
		}
	}

?>

