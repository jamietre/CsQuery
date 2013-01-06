echo "Click any key to push the NuGet package to D&R nuget server"
pause
cd NuGetPackage
nuget push CsQuery.1.3.0.nupkg -s http://10.0.17.6/ drintl

pause