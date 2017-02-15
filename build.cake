#load "scripts/msbuild15.cake"
#load "scripts/version.cake"

var target = Argument<string>("target", "Default");
var version = GetVersion(Context);

Task("Clean")
    .Does(() =>
{
    CleanDirectory("./.artifacts");
});

Task("Restore")
    .Does(() =>
{
    DotNetCoreRestore("./src/Bayeux.sln");
});

Task("Build")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .Does(() =>
{
    var settings = new MSBuildSettings
    {
        ToolPath = MSBuild15.GetPath(),
        Verbosity = Verbosity.Minimal,
        Configuration = "Release",
        PlatformTarget = PlatformTarget.MSIL
    };

    MSBuild("./src/Bayeux.sln", settings.WithProperty("Version", version.Version));
});

Task("Pack")
    .IsDependentOn("Build")
    .Does(() =>
{
    DotNetCorePack("./src/Bayeux/Bayeux.csproj", new DotNetCorePackSettings {
        Configuration = "Release",
        OutputDirectory = "./.artifacts",
        NoBuild = true,
        ArgumentCustomization = args => args.Append("/p:Version=" + version.GetSemanticVersion())
    });
});

Task("Default")
    .IsDependentOn("Pack");

RunTarget(target);