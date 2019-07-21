@echo off

set config=%1
if "%config%" == "" (
   set config=Debug
)

"%ProgramFiles(x86)%\Microsoft Visual Studio\2019\Enterprise\MSBuild\Current\Bin\MSBuild.exe" /t:restore
"%ProgramFiles(x86)%\Microsoft Visual Studio\2019\Enterprise\MSBuild\Current\Bin\MSBuild.exe" Language.Gd.sln /p:Configuration="%config%" /maxcpucount:4 /v:n

REM Pause