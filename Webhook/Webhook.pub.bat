:: nuget spec
set "project=Webhook"
set "localSourcePath=\\Origin-10\NuGetPackages"
set /p version=Version number: 

:: create package
nuget pack %project%.csproj -Prop Configuration=Debug -Symbols -IncludeReferencedProjects -Version %version%

:: publish the package to a local source
move %project%.%version%.nupkg %localSourcePath%
move %project%.%version%.symbols.nupkg %localSourcePath%

:: publish the package to nuget
:: nuget push %project%.%version%.nupkg
pause;