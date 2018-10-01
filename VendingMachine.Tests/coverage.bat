@echo off

dotnet clean
dotnet build /p:DebugType=Full
dotnet minicover instrument --workdir ../ --assemblies vendingmachine.tests/**/bin/**/*.dll --sources vendingmachine/**/*.cs --exclude-sources vendingmachine/*.cs --exclude-sources vendingmachine/repositories/seeddata.cs

dotnet minicover reset --workdir ../

dotnet test --no-build
dotnet minicover uninstrument --workdir ../
dotnet minicover report --workdir ../ --threshold 70

pause
