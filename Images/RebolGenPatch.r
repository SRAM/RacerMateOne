REBOL [
    Title: "RebolGenPatch.r"
    File: %RebolGenPatch.r
    Date: 06-May-2011
    Author: "Edgar Tolentino"
    Purpose: {Example of a Rebol patch generator.}
    Category: [view]
    History: [
        ]
]
print clean-path %./

; where to bind config variables
bindhere: none 

; set defaults overriden by racermateone-cfg.r
version: 1.0.0    
baseversion: 1.0.0    
configuration: "release"
buildtype: "full"

;; baseMainFolder: %BaseEmpty/
targetMainFolder: %Target/
patchScript: %installfull.nsi
realPatchScript: %install.nsi

;; ; to user's document folder
;; usersPersonalFiles: [
    ;; %..\RacerMateOne\UserPersonal\RacerMate\
    ;; ]
    
;; ; to rcv's folder
rcvFiles: reduce [
    to-rebol-file %"..\RacerMateOne\Real Course Video\"
    ]
; to program files    
srcFiles:  [
    %..\RacerMateOne\bin\Release\ErrorReport.exe
    %..\RacerMateOne\bin\Release\RacerMateOne.exe
    %..\RacerMateOne\bin\Release\DirectShowLib-2005.dll
    %..\RacerMateOne\bin\Release\Irrlichtf.dll
    %..\RacerMateOne\bin\Release\RM1.dll
    %..\RacerMateOne\bin\Release\RM1_Ext.dll
    %..\RacerMateOne\bin\Release\WPFMediaKit.dll
    %..\RacerMateOne\bin\Release\RM1_DefaultSettings.xml
    %..\RacerMateOne\Resources\ZRes_Icon.ico
    %..\RacerMateOne\Resources\RM1.ico
    %..\RacerMateOne\Art\
    %..\RacerMateOne\Courses\
    %..\RacerMateOne\Help\
    %..\RacerMateOne\Media\
    %..\RacerMateOne\Resources\
    ]
    
try [

if all [
    system/script/args
    not empty? system/script/args 
    ][
    args: parse system/script/args none 
    if 0 < length? args [
        configuration: pick args 1 
        ]
    if 1 < length? args [
        buildtype: pick args 2
        ]
    ]

; override for default configuration
if exists? %racermateone-cfg.r [ do bind load %racermateone-cfg.r 'bindhere ]
if exists? realPatchScript [attempt [delete realPatchScript]]

; format the versions as an integer
hexversion: 0.0.0.0
hexbaseversion: 0.0.0.0
hexversion/1: version/1
hexversion/2: version/2
hexversion/3: 0
hexversion/4: version/3
hexbaseversion/1: baseversion/1
hexbaseversion/2: baseversion/2
hexbaseversion/3: 0
hexbaseversion/4: baseversion/3

probe reduce ["args" system/script/args "targetMainFolder" targetMainFolder "patchScript" patchScript "configuration" configuration "buildtype" buildtype]
probe reduce ["version" version "baseversion" baseversion "hexversion" hexversion "hexbaseversion" hexbaseversion ]

; tools
context [

delim1: "/**" ; use as delimters for template procesing of nsis script
delim2: "**/"
delimlen: length? delim1

SET 'rnsis func [ "create a transformed NSIS script from a NSIS script template"
    text /local p out pos s p1 p2 t
    ][
    p: [ (out: copy "" pos: 1) s:
        any [
            thru delim1 p1: copy code to delim2
            p2: (
            repend out [(copy/part at s pos ((index? p1) - delimlen - pos) )(do code)]
            pos: delimlen + index? p2
            )
            ]
        to end (append out at s pos)
        ]
    return either not t: attempt [ parse text p ][ text ][ out ]
    ]
    
SET 'showblk func ["prints blocks contents"
    blk
    ][
    repeat b blk [print b]
    ]
    
SET 'sel func ["Selects a data value from a block of string data pairs"
    series [series!]
    value
    ][
    pick any [select/skip series value 2 []] 1
    ]
    
SET 'safe-make-dir func ["Creates a folder and makes sure missing parent folders are created - recursive"
    fpath [file!]
    /local f
    ][
    probe fpath
    if not exists? fpath [
        f: split-path fpath
        if not exists? f/1 [
            safe-make-dir f/1
            ]
        make-dir fpath
        ]
    ]
    
SET 'copy-file func [ "copy a file and keep mod date"
    srcfpath [file!]
    dstfpath [file!]
    /local modtime
    ][
    attempt [
        write/binary dstfpath read/binary srcfpath 
        modtime: get-modes srcfpath 'modification-date
        set-modes dstfpath [modification-date: modtime]
        ]
    ]
]   

; file checksum tools
context [ "File checksum"
    blocksize: 16384
    blockcnt: 0
    methodval: 'SHA1
    SET 'port-checksum func [{Checksum secure of a file using port 'checksum}
        f [file!]
        /method usemethod [word!]
        /local sum dat sport fport t cnt
        ][
        if method [methodval: usemethod]
		if any [(dir? f) (not exists? f)] [probe f return checksum/method #{} methodval ]
        dat: make string! blocksize
		fport: open/read/direct/binary f
		sport: open [scheme: 'checksum algorithm: methodval]
        either blockcnt > 0 [
            cnt: blockcnt
    		while [ all [
                cnt > 0
                not equal? 0 (read-io fport dat blocksize)
                ]][
                cnt: cnt - 1
                insert sport dat 
                clear dat
                ]
            ][
    		while [
                not equal? 0 (read-io fport dat blocksize)
                ][
                insert sport dat 
                clear dat
                ]
            ]
		update sport
		sum: copy sport
		close sport
		close fport
		return sum
        ]
    SET 'checksum-file FUNC ["checksum a file"
        file [file!]
        ][
        return port-checksum file
        ]
    SET 'checksum-binary FUNC ["checksum binary data in memory"
        indata [binary!]
        ][
        return checksum/method indata methodval
        ]
    SET 'checksum-config FUNC ["initialize object's local variables"
        /init blksize [integer!] blkcnt [integer!] method [word!]
        ][
        either init [
            blocksize: blksize
            blockcnt: blkcnt
            methodval: method
            ][
            blocksize: 16384
            blockcnt: 0
            methodval: 'SHA1
            ]
        ]
    ]

; object that walks folder heirarchy
dirInfo: context [
    files: copy [] ; [[#{checksum} relativepath][#{checksum} relativepath] ]
    folders: copy [] ; [ relativepath relativepath ]
    root: %./
    
    process-init: FUNC ["init object's local variables"
        rootfolder
        ][
        files: copy []
        folders: copy []
        root: copy rootfolder
        ]

    process-dir: func ["walk the folder contents and create checksum" 
        folder
        /local file flist curfolder f newroot
        ][
        insert tail folders folder
        while [not empty? folders][  
            curfolder: clean-path join root t2: first folders
            if exists? curfolder [
                t1: load curfolder
                repeat f t1 [
                    curf: join t2 f 
                    either #"/" = last to-string f [
                        insert tail folders curf
                        ][
                        insert tail files reduce[reduce [checksum-file clean-path join root curf curf]]
                        ]
                    ]
                ]
            folders: next folders
            ]
        folders: head folders    
        ]
    ]
    
; 
filesInfo: context [
    dstfiles: copy [] ; [[#{checksum}+relativepath absolutepath][#{checksum}+relativepath absolutepath] ]
    files: copy [] ; [[#{checksum}+relativepath absolutepath][#{checksum}+relativepath absolutepath] ]
    dstfolders: copy [] ; [ relativepath relativepath ]
    folders: copy [] ; [ relativepath relativepath ]
    root: %./
    
    process-init: FUNC [
        rootfolder
        ][
        files: copy []
        folders: copy []
        root: copy rootfolder
        ]

    process-files: func [
        filesblk [block!] 
        fdst [file!]
        /local curf fpath fname f
        ][
        repeat f filesblk [
            curf: to-file f 
            set [fpath fname] split-path curf
            ;; probe reduce [fpath fname]
            curdstf: to-file join fdst fname
            either #"/" = last to-string f [
                insert tail folders curf
                insert tail dstfolders fname
                ][
                if exists? srcfile: clean-path join root curf [
                    insert tail files reduce [rejoin [checksum-file srcfile fname] srcfile]
                    ]
                if exists? dstfile: clean-path join root curdstf [
                    insert tail dstfiles reduce [rejoin [checksum-file dstfile fname] dstfile]
                    ]
                ]
                
            ]
        ]
    ]
    
; Create the patch script to transform base into target     
syncFolders: func [
    base [file!]
    target [file!]
    /local dir1 dir2
    ][
    checksum-config
    probe reduce ["Start target " target now/precise]
    dir1: make dirInfo []
    dir1/process-init target
    dir1/process-dir ""
    probe reduce ["End target Start base " base now/precise]
    dir2: make dirInfo []
    dir2/process-init base
    dir2/process-dir ""
    probe reduce ["Done base " now/precise]
    reduce [dir1 dir2]
    ]
    
; Create the patch script to transform base into target     
processFolder: func [
    folder [file!]
    /local dirI
    ][
    checksum-config
    dirI: make dirInfo []
    dirI/process-init folder
    dirI/process-dir ""
    dirI
    ]
    
syncSourceFolders: func [
    srcFolders [block!]
    dstRoot [file!]
    /local a folder name dstFolder src dst 
    taddFiles tdelFiles taddFolders tdelFolders
    delfile delfolder makefolder fname fullsrcfilename fulldstfilename
    ][
    repeat a srcFolders [
        srcFolder: to-file a
        set [folder name] split-path srcFolder
        dstFolder: join dstRoot name 
        if not exists? dstFolder [
            print rejoin ["make-dir " to-local-file clean-path dstFolder]
            attempt [safe-make-dir dstFolder]
            ;; quit
            ]
            
        print rejoin ["syncFolders srcFolder:" to-local-file clean-path srcFolder " dstFolder:" to-local-file clean-path dstFolder]
        set [src dst] syncFolders dstFolder srcFolder
        
        
        taddFiles: difference src/files intersect src/files dst/files
        tdelFiles: difference dst/files intersect src/files dst/files
        taddFolders: difference src/folders intersect src/folders dst/folders
        tdelFolders: difference dst/folders intersect src/folders dst/folders
        
        repeat a tdelFiles [
            delfile: clean-path join dstFolder to-file pick a 2
            insert tail delFiles delfile
            ]
        repeat a tdelFolders [
            delfolder: clean-path join dstFolder to-file a
            insert tail delFolders delfolder
            ]
        repeat a taddFolders [
            makefolder: clean-path join dstFolder to-file a
            insert tail addFolders makefolder
            ]
        repeat a taddFiles [
            fname: to-file pick a 2
            fullsrcfilename: clean-path to-file join srcFolder fname
            fulldstfilename: clean-path to-file join dstFolder fname
            insert tail addFiles reduce [reduce [fullsrcfilename fulldstfilename]]
            ]
        ]
    ]
    

; Create the target folder
;Program Files folder    
safe-make-dir targetAppFolder: to-file join targetMainFolder "Application/"

;; ;user document folder    
;; safe-make-dir targetUserFolder: to-file join targetMainFolder "User/"

;rcv folder    
safe-make-dir targetRCVFolder: to-file join targetMainFolder "RCV/"

addFiles: copy [] ; [ [srcabsolutepath dstabsolutepath][srcabsolutepath dstabsolutepath] ... ]
delFiles: copy [] ; [ absolutepath absolutepath ... ]
addFolders: copy []
delFolders: copy []

; App main files and folders
fInfo: make filesInfo []
fInfo/process-init %./
fInfo/process-files srcFiles targetAppFolder

tsrcfiles: extract fInfo/files 2 
tdstfiles: extract fInfo/dstfiles 2 
tcommon: intersect tsrcfiles tdstfiles
tdelFiles: difference tdstfiles tcommon
taddFiles: difference tsrcfiles tcommon

repeat a tdelFiles [
    delfile: clean-path to-file sel fInfo/dstfiles a
    insert tail delFiles delfile
    ]
repeat a taddFiles [
    fullsrcfilename: clean-path to-file sel fInfo/files a
    set [folder name] split-path fullsrcfilename
    fulldstfilename: clean-path clean-path to-file join targetAppFolder name
    insert tail addFiles reduce [reduce [fullsrcfilename fulldstfilename]]
    ]

srcAppFolders: copy fInfo/folders
syncSourceFolders srcAppFolders targetAppFolder

;; ; User personal folders
;; fUInfo: make filesInfo []
;; fUInfo/process-init %./
;; fUInfo/process-files usersPersonalFiles targetUserFolder

;; srcUserFolders: copy fUInfo/folders
;; syncSourceFolders srcUserFolders targetUserFolder

; RCV folders
fRCVInfo: make filesInfo []
fRCVInfo/process-init %./
fRCVInfo/process-files rcvFiles targetRCVFolder

tsrcfiles: extract fRCVInfo/files 2 
tdstfiles: extract fRCVInfo/dstfiles 2 
tcommon: intersect tsrcfiles tdstfiles
tdelFiles: difference tdstfiles tcommon
taddFiles: difference tsrcfiles tcommon

repeat a tdelFiles [
    delfile: clean-path to-file sel fRCVInfo/dstfiles a
    insert tail delFiles delfile
    ]
repeat a taddFiles [
    fullsrcfilename: clean-path to-file sel fRCVInfo/files a
    set [folder name] split-path fullsrcfilename
    fulldstfilename: clean-path clean-path to-file join targetAppFolder name
    insert tail addFiles reduce [reduce [fullsrcfilename fulldstfilename]]
    ]

srcRCVFolders: copy fRCVInfo/folders
syncSourceFolders srcRCVFolders targetRCVFolder

;Process user and app files
repeat delfile delFiles [
    print rejoin ["delete " to-local-file delfile]
    attempt [delete delfile]
    ]
delFolders: sort/reverse delFolders    
repeat delfolder delFolders [
    print rejoin ["delete-dir " to-local-file delfolder]
    attempt [delete-dir delfolder]
    ]
addFolders: sort addFolders    
repeat makefolder addFolders [
    print rejoin ["make-dir " to-local-file makefolder]
    attempt [safe-make-dir makefolder]
    ]
repeat a addFiles [
    set [fullsrcfilename fulldstfilename] a
    print rejoin ["copy  " to-local-file fullsrcfilename " to " to-local-file fulldstfilename]
    copy-file fullsrcfilename fulldstfilename
    ]

; here we have the contents of targetMainFolder sync from orginal source.
; This contains what we want the final destination will be
d1: processFolder targetMainFolder 
targetfiles:   d1/files
targetfolders: d1/folders    

; not needed
;; d2: processFolder baseMainFolder
;; basefiles:   d2/files
;; basefolders: d2/folders  

installDB: reduce [copy [] copy [] copy [] copy []]
set [baseFiles baseFolders allFiles allFolders] installDB

if buildtype <> "full" [
    if exists? %installDB.r  [
        attempt [
            installDB:  load %installDB.r
            set [baseFiles baseFolders allFiles allFolders] installDB
            ]
        ]
    ]
    
commonfiles:   intersect basefiles targetfiles
commonfolders: intersect basefolders targetfolders

; remove any files ever installed that is not in the target 
; todelete = all - common
delFiles: sort difference allFiles commonfiles
delFolders: sort/reverse difference allFolders commonfolders

; add any files missing from the base (d1) 
; toadd = target - common
addFiles: sort difference targetfiles commonfiles
addFolders: sort difference targetfolders commonfolders

either buildtype <> "full" [
    basefiles:   commonfiles
    basefolders: commonfolders   
    ][
    basefiles:   targetfiles
    basefolders: targetfolders   
    ]

allFiles:   sort union allFiles targetfiles
allFolders: sort/reverse union allFolders targetfolders

; save all files and folders ever installed since base
; save all files and folders that has not changed since full build
installDB: reduce [baseFiles baseFolders allFiles allFolders]
write %installDB.r mold/all installDB

; remove duplicates
tdelFiles: copy []
repeat b delFiles [
    a: pick b 2
    insert tail tdelFiles a
    ]
tdelFiles: union tdelFiles copy []

taddFiles: copy []
repeat b addFiles [
    a: pick b 2
    insert tail taddFiles a
    ]
taddFiles: union taddFiles copy []

tallFiles: copy []
repeat b allFiles [
    a: pick b 2
    insert tail tallFiles a
    ]
tallFiles: union tallFiles copy []


; start setting the values needed by the NSIS script template
FilesRemove: copy ""
FoldersRemove: copy ""
FoldersAdd: copy ""
FilesAdd: copy ""
UserFilesRemove: copy ""
UserFoldersRemove: copy ""
UserFoldersAdd: copy ""
UserFilesAdd: copy ""
RCVFilesRemove: copy ""
RCVFoldersRemove: copy ""
RCVFoldersAdd: copy ""
RCVFilesAdd: copy ""
RemovePrevious: copy ""
either buildtype <> "full" [
    CheckVersion: "!define PATCHBUILD"
    ][
    CheckVersion: "; !define PATCHBUILD"
    ]

;FoldersRemove
repeat b delFolders [
    probe b
    if all [a: find/tail b "Application/" not empty? a][
        append FoldersRemove rejoin [""  tab  {RMDir "$INSTDIR\} to-local-file to-file a {"} newline]
        ]
    ]
; FoldersAdd    
repeat b addFolders [
    probe b
    if all [a: find/tail b "Application/" not empty? a][
        append FoldersAdd rejoin ["" tab {CreateDirectory "$INSTDIR\} to-local-file to-file a {"} newline]
        ]
    ]
;FilesRemove
repeat a tdelFiles [
    if all [fname: find/tail a "Application/" not empty? fname][
        append FilesRemove rejoin ["" tab {Delete "$INSTDIR\} to-local-file to-file fname {"} newline]
        ]
    ]
;FilesAdd
repeat a taddFiles [
    if all [fname: find/tail a "Application/" not empty? fname][
        fullsrcfilename: to-file join targetAppFolder fname
        set [dstpath filename] split-path to-file fname
        
        append FilesAdd rejoin ["" tab {SetOutPath "$INSTDIR\} to-local-file dstpath {"} newline]
        append FilesAdd rejoin ["" tab {File "} to-local-file fullsrcfilename {"} newline]
        ]
    ]
;; ;UserFoldersRemove
;; repeat b delFolders [
    ;; probe b
    ;; if all [a: find/tail b "User/" not empty? a][
        ;; append UserFoldersRemove rejoin [""  tab  {RMDir "$DOCUMENTS\} to-local-file to-file a {"} newline]
        ;; ]
    ;; ]
;; ;UserFoldersAdd
;; repeat b addFolders [
    ;; probe b
    ;; if all [a: find/tail b "User/" not empty? a][
        ;; append UserFoldersAdd rejoin ["" tab {CreateDirectory "$DOCUMENTS\} to-local-file to-file a {"} newline]
        ;; ]
    ;; ]
;; ;UserFilesRemove
;; repeat a tdelFiles [
    ;; if all [fname: find/tail a "User/" not empty? fname][
        ;; append UserFilesRemove rejoin ["" tab {Delete "$DOCUMENTS\} to-local-file to-file fname {"} newline]
        ;; ]
    ;; ]
;; ;UserFilesAdd
;; repeat a taddFiles [
    ;; if all [fname: find/tail a "User/" not empty? fname][
        ;; fullsrcfilename: to-file join targetUserFolder fname
        ;; set [dstpath filename] split-path to-file fname
        
        ;; append UserFilesAdd rejoin ["" tab {SetOutPath "$DOCUMENTS\} to-local-file dstpath {"} newline]
        ;; append UserFilesAdd rejoin ["" tab {File "} to-local-file fullsrcfilename {"} newline]
        ;; ]
    ;; ]
    
;; ;RCVFoldersRemove
;; repeat b delFolders [
    ;; probe b
    ;; if all [a: find/tail b "RCV/" not empty? a][
        ;; append RCVFoldersRemove rejoin [""  tab  {RMDir "$CROOT\} to-local-file to-file a {"} newline]
        ;; ]
    ;; ]
; Only add if tit doesn't exist    
;RCVFoldersAdd
repeat b addFolders [
    probe b
    if all [a: find/tail b "RCV/" not empty? a][
        tfile: rejoin [ {"$CROOT\} to-local-file to-file a {"} ]
        append RCVFoldersAdd rejoin ["" tab {IfFileExists } tfile { +2 0} newline]
        append RCVFoldersAdd rejoin ["" tab tab {CreateDirectory } tfile newline]
        ]
    ]
;; ;RCVFilesRemove
;; repeat a tdelFiles [
    ;; if all [fname: find/tail a "RCV/" not empty? fname][
        ;; append RCVFilesRemove rejoin ["" tab {Delete "$CROOT\} to-local-file to-file fname {"} newline]
        ;; ]
    ;; ]
    
; Only add if tit doesn't exist    
;RCVFilesAdd
repeat a taddFiles [
    if all [fname: find/tail a "RCV/" not empty? fname][
        fullsrcfilename: to-file join targetRCVFolder fname
        set [dstpath filename] split-path to-file fname
        
        tfile: rejoin [ {"$CROOT\} to-local-file to-file fname {"} ]
        append RCVFilesAdd rejoin ["" tab {IfFileExists } tfile { +3 0} newline]
        append RCVFilesAdd rejoin ["" tab tab {SetOutPath "$CROOT\} to-local-file dstpath {"} newline]
        append RCVFilesAdd rejoin ["" tab tab {File "} to-local-file fullsrcfilename {"} newline]
        ]
    ]
    
    
;RemovePrevious
repeat a tallFiles [
    if all [fname: find/tail a "Application/" not empty? fname][
        append RemovePrevious rejoin ["" tab {Delete  "$0\} to-local-file to-file fname {"} newline]
        ]
    ]
repeat b allFolders [
    if all [fname: find/tail b "Application/" not empty? fname][
        append RemovePrevious rejoin ["" tab {RMDir "$0\} to-local-file to-file fname {"} newline]
        ]
    ]
    
;; repeat a tallFiles [
    ;; if all [fname: find/tail a "User/" not empty? fname][
        ;; append RemovePrevious rejoin ["" tab {Delete  "$DOCUMENTS\} to-local-file to-file fname {"} newline]
        ;; ]
    ;; ]
;; repeat b allFolders [
    ;; if all [fname: find/tail b "User/" not empty? fname][
        ;; append RemovePrevious rejoin ["" tab {RMDir "$DOCUMENTS\} to-local-file to-file fname {"} newline]
        ;; ]
    ;; ]

; Don't remove RCV stuff
;; repeat a tallFiles [
    ;; if all [fname: find/tail a "RCV/" not empty? fname][
        ;; append RemovePrevious rejoin ["" tab {Delete  "$CROOT\} to-local-file to-file fname {"} newline]
        ;; ]
    ;; ]
;; repeat b allFolders [
    ;; if all [fname: find/tail b "RCV/" not empty? fname][
        ;; append RemovePrevious rejoin ["" tab {RMDir "$CROOT\} to-local-file to-file fname {"} newline]
        ;; ]
    ;; ]

; process NSIS template
main: rnsis read %RacerMateOneTemplate.nsi

; Keep a copy
write patchScript main
; write what is used
write realPatchScript main

;; halt
quit/return 0
]
;; halt

quit/return -1

