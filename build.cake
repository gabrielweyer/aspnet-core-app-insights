var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

var assemblyVersion = "1.0.0";

var artifactsDir = MakeAbsolute(Directory("artifacts"));
var testsResultsDir = artifactsDir.Combine(Directory("tests-results"));
var publishDir = artifactsDir.Combine(Directory("publish"));

var solutionPath = "./AspNetCoreAppInsights.sln";

Task("Clean")
    .Does(() =>
    {
        CleanDirectory(artifactsDir);

        var settings = new DotNetCoreCleanSettings
        {
            Configuration = configuration
        };

        DotNetCoreClean(solutionPath, settings);
    });

Task("Restore")
    .IsDependentOn("Clean")
    .Does(() =>
    {
        DotNetCoreRestore();
    });

Task("SemVer")
    .IsDependentOn("Restore")
    .Does(() =>
    {
        var gitVersion = GitVersion();

        assemblyVersion = gitVersion.AssemblySemVer;

        Information($"AssemblySemVer: {assemblyVersion}");
    });

Task("Build")
    .IsDependentOn("SemVer")
    .Does(() =>
    {
        var settings = new DotNetCoreBuildSettings
        {
            Configuration = configuration,
            NoIncremental = true,
            NoRestore = true,
            MSBuildSettings = new DotNetCoreMSBuildSettings()
                .SetVersion(assemblyVersion)
                .WithProperty("FileVersion", assemblyVersion)
                .WithProperty("InformationalVersion", assemblyVersion)
                .WithProperty("nowarn", "7035")
        };

        DotNetCoreBuild(solutionPath, settings);
    });

Task("Publish")
    .IsDependentOn("Build")
    .WithCriteria(() => HasArgument("publish"))
    .Does(() =>
    {
        var settings = new DotNetCorePublishSettings
        {
            Configuration = configuration,
            NoDependencies = true,
            NoRestore = true,
            OutputDirectory = publishDir
        };

        GetFiles("./src/*/*.csproj")
            .ToList()
            .ForEach(f => DotNetCorePublish(f.FullPath, settings));
    });

Task("Default")
    .IsDependentOn("Publish");

RunTarget(target);
