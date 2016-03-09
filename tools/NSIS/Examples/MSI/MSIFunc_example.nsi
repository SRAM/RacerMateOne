;!define DEBUG ## Enable definition this to see additional details in the ListView
 
!include MSIFunc.nsh
 
OutFile MSIFunc_Example.exe
ShowInstDetails Show
 
Section TypicalUse
    DetailPrint "### Typical Use ###"
    ## Get the Product Info for the "Microsoft .NET Framework 3.5 SP1" Package
        ${MsiGetProductInfo} "{CE2CDD62-0124-36CA-84D3-9F4DCF5C5BD9}" "ProductName" $0
        ${If} ${Errors}
            Abort $0
        ${EndIf}
        DetailPrint "ProductName = $0"
 
        ${MsiGetProductInfo} "{CE2CDD62-0124-36CA-84D3-9F4DCF5C5BD9}" "InstallDate" $1
        ${If} ${Errors}
            Abort "$1" 
        ${EndIf}
        DetailPrint "InstallDate = $1"
 
        ${MsiGetProductInfo} "{019B0015-35DB-4162-A1D1-C12321B997F8}" "ProductName" $1
        ${If} ${Errors}
            Abort "$1" 
        ${EndIf}
        DetailPrint "ProductName = $1"
        
        ${MsiGetProductInfo} "{019B0015-35DB-4162-A1D1-C12321B997F8}" "InstallDate" $1
        ${If} ${Errors}
            Abort "$1" 
        ${EndIf}
        DetailPrint "InstallDate = $1"
        
        ${MsiGetProductInfo} "{019B0015-35DB-4162-A1D1-C12321B997F8}" "InstallDir" $1
        ${If} ${Errors}
            Abort "$1" 
        ${EndIf}
        DetailPrint "InstallLocation = $1"
        
    DetailPrint ""
SectionEnd
 
Section UsingTheStack
    DetailPrint "### Using the Stack ###"
    ## Get the Product Info for the "Microsoft .NET Framework 3.5 SP1" Package
        ${MsiGetProductInfo} "{CE2CDD62-0124-36CA-84D3-9F4DCF5C5BD9}" "ProductName" s
        ${If} ${Errors}
            Pop $0
            Abort $0
        ${EndIf}
 
        ${MsiGetProductInfo} "{CE2CDD62-0124-36CA-84D3-9F4DCF5C5BD9}" "InstallDate" s
        ${If} ${Errors}
            Pop $0
            Abort $0
        ${EndIf}
 
        Pop $1
        Pop $0
 
        DetailPrint "ProductName = $0"
        DetailPrint "InstallDate = $1"
 
SectionEnd
