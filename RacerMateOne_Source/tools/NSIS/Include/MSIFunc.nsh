!ifndef ___MSIFUNC___
!define ___MSIFUNC___ 0.1
!warning "This is a only a Preview Release, I plan on adding ALOT more functions and perhaps even change the API for existing functions: YOU HAVE BEEN WARNED!"
 
!include LogicLib.nsh
 
!verbose push
!verbose 2
 
!ifndef NSIS_VERSION_MAJOR & NSIS_VERSION_MINOR
    !searchparse ${NSIS_VERSION} "v" NSIS_VERSION_MAJOR `.` NSIS_VERSION_MINOR
!endif
 
!ifmacrondef _SetDetailsPrint
    !macro _SetDetailsPrint _VALUE
        ## FIXME [BUG] SetDetailsPrint lastused
        ## There is a bug that prevents lastused from working on the 2.46 version of NSIS
        ##   I'm hoping this will be fixed in the next version
        ##   http://sourceforge.net/tracker/?func=detail&aid=2969695&group_id=22049&atid=373085
        !if ${_VALUE} == lastused
            !if ${NSIS_VERSION_MAJOR}.${NSIS_VERSION_MINOR} > 2.46
                !warning `[BUG WORKAROUND SKIPPED] - If any following "DetailPrint" operations Fail try setting "SetDetailsPrint" manually.`
                SetDetailsPrint lastused
            !else
                !warning `[BUG WORKAROUND APPLIED] - "SetDetailsPrint" set to "both" and not "lastused"`
                SetDetailsPrint both
            !endif
        !else
            SetDetailsPrint ${_VALUE}
        !endif
    !macroend
    !define SetDetailsPrint `!insertmacro _SetDetailsPrint`
!endif
 
!ifmacrondef _MsiGetProductInfo
    !macro _MsiGetProductInfo _PRODUCTCODE _PROPERTY _RetVar
        Push `${_PRODUCTCODE}`
        Push `${_PROPERTY}`
        Call MsiGetProductInfo
        !if '${_RetVar}' == 's'
        !else if '${_RetVar}' == ''
        !else
            Pop ${_RetVar} 
        !endif
    !macroend
    !define MsiGetProductInfo `!insertmacro _MsiGetProductInfo`
!endif
 
!macro FUNC_MsiGetProductInfo _UN
    Function ${_UN}MsiGetProductInfo
 
        ## If DEBUG is not defined then hide all the DetailPrint Events
            !ifndef DEBUG
                SetDetailsPrint none
            !endif
 
        ## Stack Protection
            ;Stack: _PROPERTY _PRODUCTCODE
            Exch $0 ; $0 _PRODUCTCODE
            Exch    ; _PRODUCTCODE $0
            Exch $1 ; $1 $0
 
            ; $0 = _PROPERTY
            ; $1 = _PRODUCTCODE
 
            Push $2 ; $2 $1 $0
            Push $3 ; $3 $2 $1 $0
            Push $4 ; $4 $3 $2 $1 $0
            Push $5 ; $5 $4 $3 $2 $1 $0
 
            ; $2 = Return Codes
            ; $3 = Pointer To Buffer Size
            ; $4 = Pointer to Buffer
            ; $5 = Buffer Size
 
        ## Create/Allocate a Pointer to a DWORD for the Buffer Size
            System::Alloc 2
            Pop $3
 
        ## DEBUG: 
            !ifdef DEBUG
                DetailPrint "DEBUG: What's in the registers..."
                DetailPrint "   $$0 = $0"
                DetailPrint "   $$1 = $1"
                DetailPrint "   $$2 = $2"
                DetailPrint "   $$3 = $3"
                DetailPrint "   $$4 = $4"
                DetailPrint "   $$5 = $5"
                ;MessageBox MB_OK "PAUSED"
            !endif
 
        ## Determine out big the buffer need to be
            System::Call `msi::MsiGetProductInfo(t r1,t r0,n,i $3)i .r2`
 
        ## Error Checking
            ${Select} $2
                ${Case}    0  ## ERROR_SUCCESS
                    DetailPrint "ERROR_SUCCESS - The operation completed successfully."
                    ClearErrors
                ${Case} 1610  ## ERROR_BAD_CONFIGURATION
                    StrCpy $0 "ERROR_BAD_CONFIGURATION"
                    DetailPrint "$0 - The configuration data for product $1 is corrupt. Contact your support personnel."
                    SetErrors
                    Goto CleanUp
                ${Case}   87  ## ERROR_INVALID_PARAMETER
                    StrCpy $0 "ERROR_INVALID_PARAMETER"
                    DetailPrint "$0 - The parameter is incorrect."
                    SetErrors
                    Goto CleanUp
                ${Case}  234  ## ERROR_MORE_DATA
                    StrCpy $0 "ERROR_MORE_DATA"
                    DetailPrint "$0 - More data is available."
                    SetErrors
                    Goto CleanUp
                ${Case} 1605  ## ERROR_UNKNOWN_PRODUCT
                    StrCpy $0 "ERROR_UNKNOWN_PRODUCT"
                    DetailPrint "$0 - This action is valid only for products that are currently installed."
                    SetErrors
                    Goto CleanUp
                ${Case} 1608  ## ERROR_UNKNOWN_PROPERTY
                    StrCpy $0 "ERROR_UNKNOWN_PROPERTY"
                    DetailPrint "$0 - Unknown property."
                    SetErrors
                    Goto CleanUp
                ${CaseElse}
                    StrCpy $0 "ERROR_UNANTICIPATED"
                    DetailPrint "$0 - AKA: I have no idea what happened!"
                    SetErrors
                    Goto CleanUp
            ${EndSelect}
 
        ## Get the requested buffer size
            System::Call `*$3(&i2 .r5)`
            DetailPrint "Requested Buffer Size = $5"
 
        ## Increase bufer size to account for required Null Terminator
            intop $5 $5 + 1
            System::Call `*$3(&i2 r5)`
 
        ## DEBUG: Display Actual buffer size
            !ifdef DEBUG
                System::Call `*$3(&i2 .r5)`
                DetailPrint "Actual Buffer Size = $5"
                ;MessageBox MB_OK "PAUSED"
            !endif
 
        ## Allocate Buffer
            System::Alloc $5
            Pop $4
 
        ## Get the Parameter Value
            System::Call `msi::MsiGetProductInfo(t r1,t r0,i $4,i $3)i .r2`
 
        ## Error Checking
            ${Select} $2
                ${Case}    0  ## ERROR_SUCCESS
                    DetailPrint "ERROR_SUCCESS - The operation completed successfully."
                    ClearErrors
                ${Case} 1610  ## ERROR_BAD_CONFIGURATION
                    StrCpy $0 "ERROR_BAD_CONFIGURATION"
                    DetailPrint "$0 - The configuration data for product $1 is corrupt. Contact your support personnel."
                    SetErrors
                    Goto CleanUp
                ${Case}   87  ## ERROR_INVALID_PARAMETER
                    StrCpy $0 "ERROR_INVALID_PARAMETER"
                    DetailPrint "$0 - The parameter is incorrect."
                    SetErrors
                    Goto CleanUp
                ${Case}  234  ## ERROR_MORE_DATA
                    StrCpy $0 "ERROR_MORE_DATA"
                    DetailPrint "$0 - More data is available."
                    SetErrors
                    Goto CleanUp
                ${Case} 1605  ## ERROR_UNKNOWN_PRODUCT
                    StrCpy $0 "ERROR_UNKNOWN_PRODUCT"
                    DetailPrint "$0 - This action is valid only for products that are currently installed."
                    SetErrors
                    Goto CleanUp
                ${Case} 1608  ## ERROR_UNKNOWN_PROPERTY
                    StrCpy $0 "ERROR_UNKNOWN_PROPERTY"
                    DetailPrint "$0 - Unknown property."
                    SetErrors
                    Goto CleanUp
                ${CaseElse}
                    StrCpy $0 "ERROR_UNANTICIPATED"
                    DetailPrint "$0 - AKA: I have no idea what happened!"
                    SetErrors
                    Goto CleanUp
            ${EndSelect}
 
        ## Get Value Size
            System::Call `*$3(&i2 .r5)`
 
        ## Read Property
            System::Call `*$4(&t$5 .r0)`
            DetailPrint "Property = $0"    
 
        ## Free Memory
            CleanUp:
            System::Free $3
            System::Free $4
 
        ## Restore Stack
                    ; $5 $4 $3 $2 $1 $0
            Pop $5  ; $4 $3 $2 $1 $0
            Pop $4  ; $3 $2 $1 $0
            Pop $3  ; $2 $1 $0
            Pop $2  ; $1 $0
            Pop $1  ; $0
            Exch $0 ; RETVALUE
 
        ## Restore DetailPrint Events
            !ifndef DEBUG
                ## FIXME [BUG] ${SetDetailsPrint}
                ${SetDetailsPrint} lastused
            !endif 
    FunctionEnd
!macroend
!insertmacro FUNC_MsiGetProductInfo ""
!insertmacro FUNC_MsiGetProductInfo "un."
 
!verbose pop
!endif
