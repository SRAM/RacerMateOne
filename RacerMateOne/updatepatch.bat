REM Add more entries of patches that was appplied before and will need to be removed

REM This will remove the patch, uncomment and replace MSIPATCHREMOVE with old GUID patches
Msiexec /qn /I {019B0015-35DB-4162-A1D1-C12321B997F8} MSIPATCHREMOVE={37C2B668-EC45-464C-AA30-3A0156E65689}
Msiexec /qn /I {019B0015-35DB-4162-A1D1-C12321B997F8} MSIPATCHREMOVE={607F2796-83DA-4FEB-A4E2-BC14C366AAA2}
Msiexec /qn /I {019B0015-35DB-4162-A1D1-C12321B997F8} MSIPATCHREMOVE={757E6D3E-E29B-493F-AB85-4D71B5B7A222}
rem Msiexec /qn /I {019B0015-35DB-4162-A1D1-C12321B997F8} MSIPATCHREMOVE={6847829D-6380-4B0B-9C54-A46D3ACDF436}

REM This will install the patch
Msiexec /qb /p RacerMateOneSetup.msp /n {019B0015-35DB-4162-A1D1-C12321B997F8}
echo "done"