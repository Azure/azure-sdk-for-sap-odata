@echo off

md %1
del %1\*.* /Q 

REM Clear and ensure the correct files and folders are present
if exist "zipoutput" (
    echo Cleaning up the output folders
    del %1\zipoutput\*.* /s /q
    del %1\publishout\Dependencies\*.* /s /q
)
if not exist "zipoutput" (
    echo Creating the output folders
    md %1\zipoutput
    md %1\publishout
    md %1\publishout\Dependencies
)

REM Publish the Application SDK Generator itself in different target formats
for /F "tokens=*" %%A in (build.targets) do (call buildandpublish\publishforplatform.cmd %%A %1)

REM Copy the completed output to the target
xcopy /Y /S /E /I /V /B /R %1\zipoutput\*.* %1

REM We're all done - clean up the worker folders
echo Cleaning up the output folders

rmdir %1\zipoutput /s /q
rmdir %1\publishout /s /q
