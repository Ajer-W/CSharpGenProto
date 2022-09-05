@echo off

set WORK_DIR=..\proto
set CS_OUT_PATH=..\Gen\Proto
set luaProtoIdDefine_path=E:\demo\Client\Hotfix\Script\Config\ProbufID.lua
set csProtoIdDefine_path=..\Gen\ProtoIdDefine.cs

if exist %luaProtoIdDefine_path% (del /f %luaProtoIdDefine_path%)
if exist %csProtoIdDefine_path% (del /f %csProtoIdDefine_path%)

for /f "delims=" %%i in ('dir /b  "../proto/*.proto"') do (
   protoc  --proto_path="%WORK_DIR%" --csharp_out="%CS_OUT_PATH%" "%WORK_DIR%\%%i"
)
GenProtoId.exe "%WORK_DIR%" "%csProtoIdDefine_path%" "%luaProtoIdDefine_path%" "%CS_OUT_PATH%"

echo "export ok!"
pause