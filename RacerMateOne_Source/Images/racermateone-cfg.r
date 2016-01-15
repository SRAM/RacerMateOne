REBOL [
    Title: "racermateone-cfg.r"
    Date: 18-May-2011
    Revision: "$Revision$"
    License: "(C) 2011 Prolific Publishing, Inc. All rights reserved."
    Description: {Configuration for building installer}
    ]
    
; ECT - Touched to save a comment 
; change buildtype if full or patch 
;;buildtype: "full"    
buildtype: "patch"    


; change base version and current version 
version: 4.0.6
; NOTE: Remember to change baseversion to match version when doing a full build
baseversion: 4.0.2

; if full version, base version is same as current version
if buildtype = "full" [
    baseversion: version  
    ]

; users folder
switch/default configuration [
    "debug" [
        either buildtype = "full" [
            ;; baseMainFolder: %BaseEmpty/
            ;; targetMainFolder: %Target/
            patchScript: %installfull.nsi
            ][
            ;; baseMainFolder: %Base/
            ;; targetMainFolder: %Target/
            patchScript: %installpatch.nsi
            ]
            
        srcFiles: [
            %..\Images\ReadMe.txt
            %..\Images\License.txt
            %..\RacerMateOne\bin\Debug\CleanSettings.exe
            %..\RacerMateOne\bin\Debug\ErrorReport.exe
            %..\RacerMateOne\bin\Debug\RacerMateOne.exe
            %..\RacerMateOne\bin\Debug\DirectShowLib-2005.dll
            %..\RacerMateOne\bin\Debug\Irrlichtf.dll
            %..\RacerMateOne\bin\Debug\RM1.dll
            %..\RacerMateOne\bin\Debug\RM1_Ext.dll
            %..\RacerMateOne\bin\Debug\WPFMediaKit.dll
            %..\RacerMateOne\bin\Debug\RM1_DefaultSettings.xml
            %..\RacerMateOne\Resources\ZRes_Icon.ico
            %..\RacerMateOne\Resources\RM1.ico
            %..\RacerMateOne\Art\
            %..\RacerMateOne\Courses\
            %..\RacerMateOne\Help\
            %..\RacerMateOne\Media\
            %..\RacerMateOne\Resources\
            ]
        ]
    ][ ; default is release full build
    either buildtype = "full" [
        ;; baseMainFolder: %BaseEmpty/
        ;; targetMainFolder: %Base/
        patchScript: %installfull.nsi
        ][
        ;; baseMainFolder: %Base/
        ;; targetMainFolder: %Target/
        patchScript: %installpatch.nsi
        ]
    srcFiles: [
        %..\Images\ReadMe.txt
        %..\Images\License.txt
        %..\RacerMateOne\bin\Release\CleanSettings.exe
        %..\RacerMateOne\bin\Release\ErrorReport.exe
        %..\RacerMateOne\bin\Release\Dotfuscated\RacerMateOne.exe
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
    ]
    
;; usersPersonalFiles: [
;; %..\RacerMateOne\UserPersonal\RacerMate\
;; ]
    
rcvFiles: reduce [
    to-rebol-file %"..\RacerMateOne\Real Course Video\"
    ]
    
