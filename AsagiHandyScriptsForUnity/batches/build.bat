
FOR /F "eol=# tokens=1-2 delims=," %%i IN (%1) DO SET %%i=%%j

md "../output/"

call "%UNITY_PATH%\Editor\Data\Mono\bin\smcs.bat" ^
    -r:"%UNITY_PATH%\Editor\Data\Managed\UnityEngine.dll" ^
    -r:"%UNITY_PATH%\Editor\Data\Managed\UnityEditor.dll" ^
	-r:"%UNITY_PATH%\Editor\Data\PlaybackEngines\VuforiaSupport\Managed\Runtime\Vuforia.UnityExtensions.dll" ^
    -target:library ^
    -out:"../output/%OUTPUT%.dll" ^
    "..\*.cs"

md "%UNITY_PATH%\Editor\Data\UnityExtensions\Unity\%OUTPUT%"
copy "../output/%OUTPUT%.dll" /B "%UNITY_PATH%\Editor\Data\UnityExtensions\Unity\%OUTPUT%\" /B