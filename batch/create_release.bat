@echo off

if exist Mo_Controls\ (
del Mo_Controls\
)
mkdir Mo_Controls
mkdir Mo_Controls\Mods
mkdir Mo_Controls\Mods\References
mkdir Mo_Controls\Mods\Assets

REM copy dll 
copy "..\Mo'Controls\bin\x64\Release\Mo_Controls.dll" "Mo_Controls\Mods\Mo_Controls.dll"

REM copy 3D assets
copy "..\Assets\asset_credit.txt" "Mo_Controls\Mods\Assets\asset_credit.txt"
copy "..\Assets\mo_controls.unity3d" "Mo_Controls\Mods\Assets\mo_controls.unity3d"

REM copy c sharp interface.
copy "..\Assets\XInputDotNetPure.dll" "Mo_Controls\Mods\References\XInputDotNetPure.dll"

REM copy c plus plus interface.
copy "..\Assets\XInputInterface.dll" "Mo_Controls\XInputInterface.dll"

REM copy readme
copy "..\readme.txt" "Mo_Controls\readme.txt"
copy "..\changelog.txt" "Mo_Controls\changelog.txt"
