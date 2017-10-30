@SETLOCAL
@SET PATHEXT=%PATHEXT:;.JS;=;%
node  "%~dp0\..\node_modules\typescript\bin\tsc" %*