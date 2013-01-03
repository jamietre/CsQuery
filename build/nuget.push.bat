call versions.bat
echo Click any key to push the NuGet for CsQuery %csquery_version%; or CTRL+C to abort
pause
echo ARE YOU SURE???
pause

cd NuGetPackage
rem NuGet Push NugetPackages\CsQuery.%csquery_version%.nupkg
NuGet Push NugetPackages\CsQuery.Signed.%csquery_version%-signed.nupkg

