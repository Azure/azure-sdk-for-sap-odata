md C:\TestSDKBuild 
cd C:\Source
git clone https://github.com/Azure/azure-sdk-for-sap-odata.git
cd C:\Source\azure-sdk-for-sap-odata

REM BUILDING SDK Generator
call publishtobinariesfolder.cmd C:\TestSDKBuild
cd C:\TestSDKBuild

REM EXTRACTING SDK Generator for WinX64
call powershell Expand-Archive C:\TestSDKBuild\DataOperations.Generator.OData_win-x64.zip c:\Generator -Force
cd c:\Generator\publishout

REM GENERATING SDK Samples 
rmdir c:\sdk /S /Q
call .\DataOperations.Generator.OData.exe --inputfile C:\Generator\publishout\metadata.xml --outputfolder C:\SDK --templatefolder C:\Generator\publishout\Templates --samples true

cd c:\Source\azure-sdk-for-sap-odata

REM Now enter your SAP API connection details into the appsettings and local function config files to test the samples
call code c:\sdk\Samples\ -n

