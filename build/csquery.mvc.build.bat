call versions

mkdir NugetPackages

echo Starting build

.\ProcessNuspec\bin\release\ProcessNuspec.exe csquery.mvc.nuspec ..\source\CsQuery.Mvc\Csquery.Mvc.Signed.nuspec -id CsQuery.Mvc.Signed -version %csquery_mvc_version%-signed
.\ProcessNuspec\bin\release\ProcessNuspec.exe csquery.mvc.nuspec ..\source\CsQuery.Mvc\Csquery.Mvc.nuspec -id CsQuery.Mvc -version %csquery_mvc_version% 

%msbuild% ..\source\csquery.mvc.csproj /t:clean
%msbuild% ..\source\CsQuery.Mvc\csquery.mvc.csproj /p:Configuration=Release /p:SignAssembly=true /p:AssemblyOriginatorKeyFile=..\csquery\csquery.snk
nuget pack ..\source\CsQuery.Mvc\CsQuery.Mvc.Signed.nuspec -Prop Configuration=Release -Symbols -OutputDirectory "./NugetPackages" 
pause

%msbuild% ..\source\CsQuery.Mvc\csquery.mvc.csproj /p:Configuration=Release /t:Clean 
%msbuild% ..\source\CsQuery.Mvc\csquery.mvc.csproj /p:Configuration=Release /p:SignAssembly=false
nuget pack ..\source\CsQuery.Mvc\CsQuery.Mvc.nuspec -Prop Configuration=Release -Symbols -OutputDirectory "./NugetPackages"
echo Finished
pause