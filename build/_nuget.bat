cd NuGetPackage
NuGet.exe SetApiKey 56c3b553-79a0-42e7-90e6-b7d37df267ed
nuget pack ..\..\source\CsQuery\CsQuery.nuspec -Prop Configuration=Debug -Symbols
pause