var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

var assemblyVersion = "1.0.0.0";
var buildVersion = assemblyVersion;

var artifactsDir = MakeAbsolute(Directory("artifacts"));
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

Task("Version")
    .IsDependentOn("Restore")
    .Does(() =>
    {
        var buildNumber = EnvironmentVariable("BUILD_BUILDNUMBER");

        if (buildNumber != null)
        {
            buildVersion = buildNumber;
        }

        Information($"Assembly Version: {assemblyVersion}");
        Information($"Build Version: {buildVersion}");
    });

Task("Build")
    .IsDependentOn("Version")
    .Does(() =>
    {
        var settings = new DotNetCoreBuildSettings
        {
            Configuration = configuration,
            NoIncremental = true,
            NoRestore = true,
            MSBuildSettings = new DotNetCoreMSBuildSettings()
                .SetVersion(assemblyVersion)
                .WithProperty("FileVersion", buildVersion)
                .WithProperty("InformationalVersion", buildVersion)
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
            OutputDirectory = publishDir,
            // Workaround for https://github.com/dotnet/cli/issues/5331
            // We need to rebuild to be able to version the assemblies :(
            MSBuildSettings = new DotNetCoreMSBuildSettings()
                .SetVersion(assemblyVersion)
                .WithProperty("FileVersion", buildVersion)
                .WithProperty("InformationalVersion", buildVersion)
                .WithProperty("nowarn", "7035")
        };

        GetFiles("./samples/SampleApi/*.csproj")
            .ToList()
            .ForEach(f =>
            {
                Information($"Publishing '{f.FullPath}' to '{settings.OutputDirectory}'");
                DotNetCorePublish(f.FullPath, settings);
            });
    });

Task("Default")
    .IsDependentOn("Publish");

RunTarget(target);
