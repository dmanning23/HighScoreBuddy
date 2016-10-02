rm *.nupkg
nuget pack .\HighScoreBuddy.nuspec -IncludeReferencedProjects -Prop Configuration=Release
nuget push -source https://www.nuget.org -NonInteractive *.nupkg