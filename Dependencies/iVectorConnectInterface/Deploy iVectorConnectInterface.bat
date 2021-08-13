cd "C:\Program Files (x86)\SourceGear\Vault Client"

set login=
set /p login=What is your source gear login?



vault checkout -host "athena.intuitivesystems.co.uk" -user "%login%" -password "" -exclusive -repository "Main Projects 2010" "$/Development/Lowcost/Lowcost Release 52/DLLRepository/iVectorConnectInterface/iVectorConnectInterface.dll"

xcopy "C:\projectsvault_core\development\Lowcost Release 52\iVectorConnectInterface\bin\iVectorConnectInterface.dll" "c:\projectsvault\Development\Lowcost\Lowcost Release 52\DLLRepository\iVectorConnectInterface\iVectorConnectInterface.dll" /Y

vault checkin -host "athena.intuitivesystems.co.uk" -user "%login%" -password "" -exclusive -repository "Main Projects 2010" "$/Development/Lowcost/Lowcost Release 52/DLLRepository/iVectorConnectInterface/iVectorConnectInterface.dll"



pause