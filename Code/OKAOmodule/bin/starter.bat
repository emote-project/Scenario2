@Echo off
cd %~dp0
:Start
rem Use the switches here
echo Close this Window first if you don't want to restart automatically

OKAOmodule_Release.exe
echo Program terminated at %Date% %Time% with Error %ErrorLevel% >> crachlog.log 

timeout 3

goto Start