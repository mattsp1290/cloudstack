@echo off

set SAMPLEJARDIR=%CD%\lib

cd samples

if exist "%SAMPLEJARDIR%\samples.jar" del /F "%SAMPLEJARDIR%\samples.jar"

if NOT "x%1" == "x-w" (
   if exist "%SAMPLEJARDIR%\vim25.jar" del /F "%SAMPLEJARDIR%\vim25.jar"
   if exist com\vmware\vim25 rmdir /q/s com\vmware\vim25 >nul 2>nul
   if exist com\vmware\vim25 rmdir /q/s com\vmware\vim25 >nul 2>nul
)

del /s /q com\vmware\samples\*.class >nul 2>nul

cd ..

:CLEANEND
@echo on