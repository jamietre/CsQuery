call versions

mkdir NugetPackages

rem create signed version first so unsigned spec is left behind

.\ProcessNuspec\bin\release\ProcessNuspec.exe csquery.nuspec ..\source\CsQuery\Csquery.Signed.nuspec -description "CsQuery - Signed/Strong Named Edition. It's strongly advised that you use use the package 'CsQuery' unless you know why you want the signed edition." -summary "A complete CSS selector engine and jQuery port for .NET 4 and C# (STRONG NAMED)." -title "CsQuery.Signed" -id CsQuery.Signed -version %csquery_version%-signed
.\ProcessNuspec\bin\release\ProcessNuspec.exe csquery.nuspec ..\source\CsQuery\Csquery.Nuspec -id CsQuery -version %csquery_version%

%msbuild% ..\source\CsQuery\csquery.csproj /p:Configuration=Release /t:Clean 
%msbuild% ..\source\csquery\csquery.csproj /p:Configuration=Release /p:SignAssembly=true /p:AssemblyOriginatorKeyFile=csquery.snk
nuget pack ..\source\CsQuery\CsQuery.Signed.nuspec -Prop Configuration=Release -Symbols -OutputDirectory "./NugetPackages"
pause 

%msbuild% ..\source\CsQuery\csquery.csproj /p:Configuration=Release /t:Clean
%msbuild% ..\source\csquery\csquery.csproj /p:Configuration=Release /p:SignAssembly=false
nuget pack ..\source\CsQuery\CsQuery.nuspec -Prop Configuration=Release -Symbols -OutputDirectory "./NugetPackages"

pause