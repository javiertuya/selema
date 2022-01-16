REM Instrumenta el codigo javascript para las pruebas
REM Solo se hace en local en la plataforma java, se desplegara en una url en CI
REM para .net se usara la misma url anterior
set from=%~dp0
set jscover=%from%\..\..\..\target\JSCover-all.jar
rmdir /s /q %from%\static\instrumented
java -jar %jscover% -fs --local-storage --log=INFO %from%\static\main %from%\static\instrumented
rmdir /s /q %from%\static\instrumented\original-src
copy %from%\..\..\main\resources\jscoverage-restore-local-storage.html %from%\static\instrumented\
pause


