@ECHO OFF
REM If you have just cloned the code for the first time, run me!
set foundOpenwrap = where o
if foundOpenwrap == 0 ( 
echo Could not find openwrap executable in path. 
) else (
o add-remote -name beta -href http://wraps.openwrap.org/beta/
o update-wrap openwrap -system
o update-wrap openwrap -usesystem
)
pause