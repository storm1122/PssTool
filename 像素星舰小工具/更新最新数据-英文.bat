@echo off

set "exeFile=bin\Release\net6.0\PssTool.exe"
rem 调用可执行文件并传递参数数组
"%exeFile%" CheckConfig en
endlocal
pause