@REM This batchfile generates JAXWS client stubs (proxy code) from and vim25
@REM versions of the WSDL and then compiles the client sample applications.
@REM 
@REM To compile the client applications without re-generating the client stubs,
@REM pass the -w as an argument to the script. For example:
@REM build -w
@REM 
@REM Note that this batchfile requires you to set three environment 
@REM variables:
@REM   JAVAHOME
@REM 
@REM See the Developer's Setup Guide for more information about
@REM JAVAHOME.
@REM Alternatively, you can modify the settings of
@REM these three variables in the batchfile. Be careful if you do so.
@REM


cls

@echo off

setlocal
set SAMPLEDIR=.\samples
set SAMPLEJARDIR=%CD%\lib

if NOT DEFINED JAVAHOME (
   @echo JAVAHOME not defined. Must be defined to build java apps.
   goto END
)

if NOT DEFINED VIMWSDLPATHPREFIX set VIMWSDLPATHPREFIX=..\..

if NOT "x%1" == "x-w" (
   set WSDLLOCATION25=vimService.wsdl
   set WSDLFILE25=%VIMWSDLPATHPREFIX%\..\wsdl\vim25\vimService.wsdl
)

:SETENV
set PATH=%JAVAHOME%\bin;%PATH%

set LOCALCLASSPATH=%CD%\lib;

for %%i in ("lib\*.jar") do call lcp.bat %CD%\%%i

:DOBUILD
call clean.bat %1

cd samples

if NOT "x%1" == "x-w" (
   IF EXIST com\vmware\vim25 (
      rmdir/s/q com\vmware\vim25
   )
   mkdir com\vmware\vim25

   xcopy /q/i/s %VIMWSDLPATHPREFIX%\..\wsdl\vim25\*.* com\vmware\vim25\

   echo Generating vim25 stubs from wsdl

   %JAVAHOME%\bin\wsimport -wsdllocation %WSDLLOCATION25% -p com.vmware.vim25 -s . %WSDLFILE25%

   @rem fix VimService class to get the wsdl from the vim25.jar
   %JAVAHOME%\bin\java -classpath %LOCALCLASSPATH% FixJaxWsWsdlResource "%CD%\com\vmware\vim25\VimService.java"
   del /q/f com\vmware\vim25\VimService.class
   %JAVAHOME%\bin\javac -classpath "%LOCALCLASSPATH%" com\vmware\vim25\VimService.java

   %JAVAHOME%\bin\jar cf "%SAMPLEJARDIR%\vim25.jar" com\vmware\vim25\*.class com\vmware\vim25\*.wsdl com\vmware\vim25\*.xsd

   del /q/f com\vmware\vim25\*.wsdl
   del /q/f com\vmware\vim25\*.xsd
   del /q/f com\vmware\vim25\*.class

   echo Done generating vim25 stubs from wsdl
)

@rem allow for only compiling stub code, without regenerating java stub files
if "x%2" == "x-c" (
   echo Compiling vim25 stubs

   xcopy /q/i/y %VIMWSDLPATHPREFIX%\..\wsdl\vim25\*.* com\vmware\vim25\

   %JAVAHOME%\bin\javac -classpath "%LOCALCLASSPATH%" com\vmware\vim25\*.java
   %JAVAHOME%\bin\jar cf "%SAMPLEJARDIR%\vim25.jar" com\vmware\vim25\*.class com\vmware\vim25\*.wsdl com\vmware\vim25\*.xsd

   del /q/f com\vmware\vim25\*.wsdl
   del /q/f com\vmware\vim25\*.xsd
   del /q/f com\vmware\vim25\*.class

   echo Done compiling vim25 stubs
)

@echo Compiling samples

%JAVAHOME%\bin\javac -classpath "%LOCALCLASSPATH%  %SAMPLEJARDIR%\vim25.jar;" com\vmware\vm\*.java
%JAVAHOME%\bin\javac -classpath "%LOCALCLASSPATH%  %SAMPLEJARDIR%\vim25.jar;" com\vmware\performance\widgets\*.java
%JAVAHOME%\bin\javac -classpath "%LOCALCLASSPATH%  %SAMPLEJARDIR%\vim25.jar;" com\vmware\performance\*.java
%JAVAHOME%\bin\javac -classpath "%LOCALCLASSPATH%  %SAMPLEJARDIR%\vim25.jar;" com\vmware\host\*.java
%JAVAHOME%\bin\javac -classpath "%LOCALCLASSPATH%  %SAMPLEJARDIR%\vim25.jar;" com\vmware\general\*.java
%JAVAHOME%\bin\javac -classpath "%LOCALCLASSPATH%  %SAMPLEJARDIR%\vim25.jar;" com\vmware\alarms\*.java
%JAVAHOME%\bin\javac -classpath "%LOCALCLASSPATH%  %SAMPLEJARDIR%\vim25.jar;" com\vmware\events\*.java
%JAVAHOME%\bin\javac -classpath "%LOCALCLASSPATH%  %SAMPLEJARDIR%\vim25.jar;" com\vmware\httpfileaccess\*.java
%JAVAHOME%\bin\javac -classpath "%LOCALCLASSPATH%  %SAMPLEJARDIR%\vim25.jar;" com\vmware\scsilun\*.java
%JAVAHOME%\bin\javac -classpath "%LOCALCLASSPATH%  %SAMPLEJARDIR%\vim25.jar;" com\vmware\scheduling\*.java
%JAVAHOME%\bin\javac -classpath "%LOCALCLASSPATH%  %SAMPLEJARDIR%\vim25.jar;" com\vmware\vapp\*.java
%JAVAHOME%\bin\javac -classpath "%LOCALCLASSPATH%  %SAMPLEJARDIR%\vim25.jar;" com\vmware\simpleagent\*.java
%JAVAHOME%\bin\javac -classpath "%LOCALCLASSPATH%  %SAMPLEJARDIR%\vim25.jar;" com\vmware\security\credstore\*.java
%JAVAHOME%\bin\javac -classpath "%LOCALCLASSPATH%  %SAMPLEJARDIR%\vim25.jar;" com\vmware\guest\*.java
%JAVAHOME%\bin\javac -classpath "%LOCALCLASSPATH%  %SAMPLEJARDIR%\vim25.jar;" com\vmware\storage\*.java

@echo Jarring samples
%JAVAHOME%\bin\jar cf "%SAMPLEJARDIR%\samples.jar" com\vmware\general\*.class com\vmware\vm\*.class com\vmware\performance\widgets\*.class com\vmware\performance\*.class com\vmware\host\*.class com\vmware\alarms\*.class com\vmware\scsilun\*.class com\vmware\scheduling\*.class com\vmware\events\*.class com\vmware\httpfileaccess\*.class com\vmware\vapp\*.class com\vmware\simpleagent\*.class com\vmware\security\credstore\*.class com\vmware\guest\*.class com\vmware\storage\*.class

cd ..

:END
@echo Done.
@echo on
