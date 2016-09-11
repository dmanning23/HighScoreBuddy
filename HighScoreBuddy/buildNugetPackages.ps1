rm *.nupkg
nuget pack .\HighScoreBuddy.nuspec -IncludeReferencedProjects -Prop Configuration=Release
nuget push *.nupkg