set version="1.3.3"
mkdir NugetPackages

rem create signed version first so unsigned spec is left behind

.\ProcessNuspec\bin\release\ProcessNuspec.exe csquery.nuspec ..\source\CsQuery\Csquery.Signed.nuspec -id CsQuery.Signed -version %version%
.\ProcessNuspec\bin\release\ProcessNuspec.exe csquery.nuspec ..\source\CsQuery\Csquery.Nuspec -id CsQuery -version %version%

C:\Windows\Microsoft.NET\Framework\v4.0.30319\msbuild ..\source\csquery\source\csquery.csproj /p:SignAssembly=true /p:AssemblyOriginatorKeyFile=csquery.snk
nuget pack ..\source\CsQuery\CsQuery.Signed.nuspec -Prop Configuration=Debug -Symbols -OutputDirectory "./NugetPackages"

C:\Windows\Microsoft.NET\Framework\v4.0.30319\msbuild ..\source\csquery.sln
nuget pack ..\source\CsQuery\CsQuery.nuspec -Prop Configuration=Debug -Symbols -OutputDirectory "./NugetPackages"

pause