del %2\publishout\*.* /s /q
REM First Build the windows output 
echo Building Generator from source for platform target %1 in folder %2

cd DataOperations.Generator.OData
call dotnet publish -r %1 --self-contained true -p:PublishTrimmed=true -p:PublishReadyToRun=true -c Release
cd ..

REM Publish the Sub-Libraries that contain the common code for the application SDK required for the SDK to actually run
call buildandpublish\publishsublibs.cmd DataOperations.Core %2
call buildandpublish\publishsublibs.cmd DataOperations.OData %2
call buildandpublish\publishsublibs.cmd DataOperations.WebJobs %2

REM call buildandpublish\publishsublibs.cmd FunctionsSample.GWSAMPLE_BASIC %2
REM call buildandpublish\publishsublibs.cmd TestClientSample.GWSAMPLE_BASIC %2
cd DataOperations.Generator.OData

xcopy /Y /S /E /I /V /B /R bin\release\net6.0\%1\publish %2\publishout
xcopy /Y /S /E /I /V /B /R Templates %2\publishout\Templates
echo -> Publishing Samples to %2\publishout\Samples
cd

md %2\publishout\Samples
xcopy /Y /S /E /I /V /B /R ..\Samples %2\publishout\Samples
xcopy /Y OData-Version.xsl %2\publishout
xcopy /Y V2-to-V4-CSDL.xsl %2\publishout
xcopy /Y metadata.xml %2\publishout
REM xcopy /Y /S /E /I /V /B /R ..\Dependencies\%1  %2\zipoutput\Dependencies\%1
powershell Compress-Archive %2\publishout %2\zipoutput\DataOperations.Generator.OData_%1.zip -Force
cd ..
