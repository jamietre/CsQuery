param($installPath, $toolsPath, $package, $project) 
$path = [System.IO.Path] 
$readmefile = $path::Combine($installPath, "path\to\your.file") 
$DTE.ItemOperations.OpenFile($readmefile) 
