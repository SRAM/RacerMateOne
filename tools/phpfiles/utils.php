<?php
	require_once("arrays.php");

/*****************************************************************************************
	get directory listing through recursion
*****************************************************************************************/

function get_files($dir, $pat, $recurse)  {
	global $srcdir;
	global $files;
	global $files_to_obfuscate;


	if(!is_dir($dir))  {
		return false;
	}


	$d = dir($dir);

	while($fname = $d->read())  {
		if($fname == "." || $fname == "..")  {
			continue;
		}

		if ($recurse)  {
			if(is_dir($dir."/".$fname))  {
				get_files($dir."/".$fname, $pat, $recurse);
				continue;
			}
		}


		$n = preg_match($pat, $fname);
		if ($n===false)  {
			printf("\n\ncontinue\n\n");
			continue;
		}

		if ($n==0)  {
			continue;
		}


		$fullpath = $dir . "/" . $fname ;
		$basename = basename($fullpath);

		if (in_array($basename, $files_to_obfuscate))  {

			$a = array();

			$a['basename'] = $basename;
			$a['fullpath'] = $fullpath;

			$ctime = date("Y-m-d H:i:s", filectime($fullpath));
			$mtime = date("Y-m-d H:i:s", filemtime($fullpath) );
			$atime = date("Y-m-d H:i:s", fileatime($fullpath) );

			$a['mtime'] = $mtime;
			$a['filesize'] = filesize($fullpath);
			//$a['md5'] = md5($fullpath);

			$path_parts = pathinfo($fullpath);
			$path = $path_parts['dirname'];
			$a['path'] = $path;
			$a['extension'] = $path_parts['extension'];

			array_push($files, $a);
		}

	}

	$d->close();

	return;
}						// get_files

?>


