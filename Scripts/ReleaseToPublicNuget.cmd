Echo Creates Release Packages

set packages="..\packages\release"

set program="..\src\ConcurrentEngine"
dotnet msbuild /p:Configuration=Release %program%
dotnet pack -o %packages% %program%
