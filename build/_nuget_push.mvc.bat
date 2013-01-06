echo "Click any key to push the NuGet package; or CTRL+C to abort"
pause
echo "ARE YOU SURE???"
pause
cd NuGetPackage
NuGet Push CsQuery.Mvc.1.3.0.nupkg
pause
