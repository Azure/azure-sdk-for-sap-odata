REM Publish the Sub-Libraries that contain the common code for the application SDK required for the SDK to actually run
REM Copy the sub-libraries to the zipoutput folder
echo Copying source for %1 to Work Folder %2
del %2\publishout\Dependencies\%1\*.* /s /q
xcopy /Y /S /E /I /V /B /R Dependencies\%1 %2\publishout\Dependencies\%1
