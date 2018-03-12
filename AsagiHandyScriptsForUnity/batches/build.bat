SET UNITY_PATH=C:\Program Files\Unity2017-2-1p1
SET DLL_FILENAME=asagihandy.dll
cmd /C "%UNITY_PATH%\Editor\Data\Mono\bin\smcs.bat" ^
    -r:"%UNITY_PATH%\Editor\Data\Managed\UnityEngine.dll" ^
    -r:"%UNITY_PATH%\Editor\Data\Managed\UnityEditor.dll" ^
    -target:library ^
    -out:%DLL_FILENAME% ^
    *.cs
md "%UNITY_PATH%\Editor\Data\UnityExtensions\Unity\AsagiHandyScripts"
copy DLL_FILENAM /B "%UNITY_PATH%\Editor\Data\UnityExtensions\Unity\AsagiHandyScripts\" /B