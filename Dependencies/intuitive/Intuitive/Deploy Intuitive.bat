cd "C:\Program Files (x86)\SourceGear\Vault Client"

set login=
set /p login=What is your source gear login?



vault checkout -host "athena.intuitivesystems.co.uk" -user "%login%" -password "" -exclusive -repository "Main Projects 2010" "$/Development/Core/DLLRepository/Intuitive/Intuitive.dll"
vault checkout -host "athena.intuitivesystems.co.uk" -user "%login%" -password "" -exclusive -repository "Main Projects 2010" "$/Development/Core/DLLRepository/Intuitive/Intuitive.xml"

xcopy "C:\ProjectsStash\Core\Intuitive\bin\intuitive.dll" "c:\projectsvault\development\core\dllrepository\intuitive\Intuitive.dll" /Y
xcopy "C:\ProjectsStash\Core\Intuitive\bin\intuitive.dll" "c:\projectsvault\development\core\dllrepository\intuitive\Intuitive.xml" /Y

vault checkin -host "athena.intuitivesystems.co.uk" -user "%login%" -password "" -exclusive -repository "Main Projects 2010" "$/Development/Core/DLLRepository/Intuitive/Intuitive.dll"
vault checkin -host "athena.intuitivesystems.co.uk" -user "%login%" -password "" -exclusive -repository "Main Projects 2010" "$/Development/Core/DLLRepository/Intuitive/Intuitive.xml"


pause