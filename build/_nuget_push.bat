echo "Click any key to push the NuGet package; or CTRL+C to abort"
pause
echo "ARE YOU SURE???"
pause
cd NuGetPackage
NuGet Push CsQuery.1.3.2.nupkg

