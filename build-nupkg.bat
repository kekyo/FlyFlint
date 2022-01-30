@echo off

rem Lightweight static O/R mapping builder at compile time.
rem Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo2)
rem
rem Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0

echo.
echo "==========================================================="
echo "Build FlyFlint"
echo.

rem git clean -xfd

dotnet restore
dotnet build -c Release -p:Platform="Any CPU" FlyFlint.sln
dotnet pack -p:Configuration=Release -p:Platform=AnyCPU -o artifacts FlyFlint.Core\FlyFlint.Core.csproj
dotnet pack -p:Configuration=Release -p:Platform=AnyCPU -o artifacts FlyFlint.Dynamic.Core\FlyFlint.Dynamic.Core.csproj
dotnet pack -p:Configuration=Release -p:Platform=AnyCPU -o artifacts FlyFlint.Build\FlyFlint.Build.csproj
