setlocal enableDelayedExpansion

SET /A errno=0
SET /A ERROR_PUBLISH_FAILED=1
SET /A ERROR_COPY_FAILED=2

SET outdir=%1
SET addinName=%2.addin
SET thisDir=%3
SET publisherPath="D:\WS\T17_SIRIUS\binaries\Debug\x64\PublicAPI\V17.AddIn\Siemens.Engineering.AddIn.Publisher.exe"
SET addinsPath="D:\WS\T17_SIRIUS\binaries\Debug\x64\AddIns\"

ECHO outdir %outdir%
ECHO addinName %addinName%
ECHO thisDir %thisDir%

SET projectPath=%thisDir:"=%

SET configPath=%projectPath%Configuration.xml
ECHO configPath %configPath%

%publisherPath% --configuration "%configPath%" --logfile Log.txt --verbose

IF %ERRORLEVEL% NEQ 0 (
    SET /A errno^|=%ERROR_PUBLISH_FAILED%
	TYPE "%projectPath%%outdir%Log.txt"
) ELSE (
	copy "%projectPath%%outdir%%addinName%" %addinsPath% /Y
	IF %ERRORLEVEL% NEQ 0 (
		SET /A errno^|=%ERROR_COPY_FAILED%
	)
)
EXIT /B %errno%