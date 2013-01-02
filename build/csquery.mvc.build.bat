set version="1.3.0.4"
mkdir NugetPackages

.\ProcessNuspec\bin\release\ProcessNuspec.exe csquery.mvc.nuspec ..\source\CsQuery.Mvc\Csquery.Mvc.Signed.nuspec -id CsQuery.Mvc.Signed -version %version%
.\ProcessNuspec\bin\release\ProcessNuspec.exe csquery.mvc.nuspec ..\source\CsQuery.Mvc\Csquery.Mvc.nuspec -id CsQuery.Mvc -version %version%

C:\Windows\Microsoft.NET\Framework\v4.0.30319\msbuild ..\source\CsQuery.Mvc\csquery.mvc.csproj /p:Configuration=Release /p:SignAssembly=true /p:AssemblyOriginatorKeyFile=..\csquery\csquery.snk
nuget pack ..\source\CsQuery.Mvc\CsQuery.Mvc.Signed.nuspec -Prop Configuration=Release -Symbols -OutputDirectory "./NugetPackages"

C:\Windows\Microsoft.NET\Framework\v4.0.30319\msbuild ..\source\csquery.mvc.sln /p:Configuration=Release
nuget pack ..\source\CsQuery.Mvc\CsQuery.Mvc.nuspec -Prop Configuration=Release -Symbols -OutputDirectory "./NugetPackages"

pause