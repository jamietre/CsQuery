set version="1.3.0"
mkdir NugetPackages

.\ProcessNuspec\bin\release\ProcessNuspec.exe csquery.mvc.nuspec ..\source\CsQuery\Csquery.Mvc.Signed.nuspec -id CsQuery.Mvc.Signed -version %version%
.\ProcessNuspec\bin\release\ProcessNuspec.exe csquery.mvc.nuspec ..\source\CsQuery\Csquery.Mvc.Nuspec -id CsQuery.Mvc -version %version%

C:\Windows\Microsoft.NET\Framework\v4.0.30319\msbuild ..\source\csquery\source\csquery.csproj /p:SignAssembly=true /p:AssemblyOriginatorKeyFile=csquery.snk
nuget pack ..\source\CsQuery\CsQuery.Mvc.Signed.nuspec -Prop Configuration=Debug -Symbols -OutputDirectory "./NugetPackages"

C:\Windows\Microsoft.NET\Framework\v4.0.30319\msbuild ..\source\csquery.sln
nuget pack ..\source\CsQuery\CsQuery.Mvc.nuspec -Prop Configuration=Debug -Symbols -OutputDirectory "./NugetPackages"

pause