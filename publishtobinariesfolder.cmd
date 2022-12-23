IF [%1]==[] GOTO :FAIL
:DOBUILD 
buildandpublish\publishme.cmd %1
IF ERRORLEVEL 0 GOTO :END
:Errored
Echo Error. Please check the console log for more details.
GOTO :END
:FAIL
ECHO ERROR: No build folder specified, please specify a full folder path to the build folder as a parameter.
ECHO USAGE: publishtobinariesfolder.cmd <full path to build folder>
:END
Echo Finished.

