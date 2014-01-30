@echo off
@REM you need to set env variables : JAVAHOME, or modify the 2 values here

set SAMPLEDIR=.

if NOT DEFINED JAVAHOME (
   echo JAVAHOME not defined. Must be defined to run java apps.
   goto END
)

if NOT DEFINED VMKEYSTORE (
   echo VMKEYSTORE not defined. Must be defined to run java apps.
   goto END
)

setlocal

:SETENV
set PATH=%JAVAHOME%\bin;%PATH%
set LOCALCLASSPATH=%CD%\lib;%WBEMHOME%;
for %%i in ("lib\*.jar") do call lcp.bat %CD%\%%i
set LOCALCLASSPATH=%LOCALCLASSPATH%%CLASSPATH%

:next
if [%1]==[] goto argend
   set ARG=%ARG% %1
   shift
   goto next
:argend

:DORUN
"%JAVAHOME%"\bin\java -cp "%LOCALCLASSPATH%" -Djavax.net.ssl.trustStore="%VMKEYSTORE%" -Xmx1024M %ARG%

endlocal

:END
echo Done.