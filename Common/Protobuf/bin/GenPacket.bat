Rem Path Setting
pushd %~dp0

Rem Compile
protoc -I=./ --csharp_out=./ ./Enum.proto
protoc -I=./ --csharp_out=./ ./Struct.proto
protoc -I=./ --csharp_out=./ ./Protocol.proto

Rem Error -> Pause cmd
IF ERRORLEVEL 1 PAUSE

Rem Excute bat -> Make file -> Copy with folder
XCOPY /Y Enum.cs "../../../Assets/Scripts/Protobuf\"
XCOPY /Y Struct.cs "../../../Assets/Scripts/Protobuf\"
XCOPY /Y Protocol.cs "../../../Assets/Scripts/Protobuf\"

DEL /Q /F *.cs

PAUSE


