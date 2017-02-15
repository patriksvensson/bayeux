#addin "nuget:?package=Microsoft.VisualStudio.Setup.Configuration.Interop&version=1.7.13-rc&prerelease"

//////////////////////////////////////////////////////////////////////////////
// Adapted from script by Jonathon Marolf (@jmarolf)
// https://github.com/cake-build/cake/issues/1369#issuecomment-262108228
//////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Setup.Configuration;

public static class MSBuild15
{
    public static FilePath GetPath()
    {
        var instance = LocateVisualStudioInstance("15.0", new HashSet<string>(new[] { "Microsoft.Component.MSBuild" })) as ISetupInstance2;
        var installationPath = instance.GetInstallationPath();
        var path = System.IO.Path.Combine(installationPath, "MSBuild", "15.0", "Bin", "MSBuild.exe");
        return new FilePath(path);
    }

    private static ISetupConfiguration GetSetupConfiguration()
    {
        ISetupConfiguration setupConfiguration;

        try
        {
            setupConfiguration = new SetupConfiguration();
        }
        catch (COMException comException)
        {
            if(comException.HResult == unchecked((int)0x80040154))
            {
                Console.WriteLine("COM registration is missing, Visual Studio may not be installed correctly");
            }
            throw;
        }

        return setupConfiguration;
    }

    private static IEnumerable<ISetupInstance> EnumerateVisualStudioInstances()
    {
        var setupConfiguration = GetSetupConfiguration() as ISetupConfiguration2;

        var instanceEnumerator = setupConfiguration.EnumAllInstances();
        var instances = new ISetupInstance[3];

        var instancesFetched = 0;
        instanceEnumerator.Next(instances.Length, instances, out instancesFetched);

        if (instancesFetched == 0)
        {
            throw new Exception("There were no instances of Visual Studio 15.0 or later found.");
        }

        do
        {
            for (var index = 0; index < instancesFetched; index++)
            {
                yield return instances[index];
            }

            instanceEnumerator.Next(instances.Length, instances, out instancesFetched);
        }
        while (instancesFetched != 0);
    }

    private static ISetupInstance LocateVisualStudioInstance(string vsProductVersion, HashSet<string> requiredPackageIds)
    {
        var instances = EnumerateVisualStudioInstances().Where((instance) => instance.GetInstallationVersion().StartsWith(vsProductVersion));

        var instanceFoundWithInvalidState = false;

        foreach (ISetupInstance2 instance in instances.OrderByDescending(i => i.GetInstallationVersion()))
        {
            var packages = instance.GetPackages()
                                    .Where((package) => requiredPackageIds.Contains(package.GetId()));

            if (packages.Count() != requiredPackageIds.Count)
            {
                continue;
            }

            const InstanceState minimumRequiredState = InstanceState.Local | InstanceState.Registered;

            var state = instance.GetState();

            if ((state & minimumRequiredState) == minimumRequiredState)
            {
                return instance;
            }

            Console.WriteLine("An instance matching the specified requirements but had an invalid state. (State: {state})");
            instanceFoundWithInvalidState = true;
        }

        throw new Exception(instanceFoundWithInvalidState ?
                            "An instance matching the specified requirements was found but it was in an invalid state." :
                            "There were no instances of Visual Studio 15.0 or later found that match the specified requirements.");
    }
}