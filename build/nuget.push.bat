call versions.bat
echo Click any key to push the NuGet for CsQuery %csquery_version%; or CTRL+C to abort
pause
echo ARE YOU SURE???
pause

cd NuGetPackage
NuGet Push CsQuery.%csquery_version%.nupkg
rem NuGet Push CsQuery.Signed.%csquery_version%-signed.nupkg

