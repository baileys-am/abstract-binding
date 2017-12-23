setlocal

@rem enter this directory
cd /d %~dp0

set TOOLS_PATH=..\..\packages\Grpc.Tools.1.8.0\tools\windows_x64

%TOOLS_PATH%\protoc.exe --csharp_out . common.proto --grpc_out . --plugin=protoc-gen-grpc=%TOOLS_PATH%\grpc_csharp_plugin.exe
%TOOLS_PATH%\protoc.exe --csharp_out . recipientservice.proto --grpc_out . --plugin=protoc-gen-grpc=%TOOLS_PATH%\grpc_csharp_plugin.exe

endlocal