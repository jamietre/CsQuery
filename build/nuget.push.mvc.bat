call versions.bat
echo Click any key to push the NuGet for CsQuery %csquery_mvc_version%; or CTRL+C to abort
pause
echo ARE YOU SURE???
pause

cd NuGetPackage
NuGet Push NugetPackages\CsQuery.Mvc.%csquery_mvc_version%.nupkg
rem NuGet Push NugetPackages\CsQuery.Mvc.Signed.%csquery_mvc_version%-signed.nupkg

