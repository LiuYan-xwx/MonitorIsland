using Microsoft.Extensions.DependencyInjection;
using MonitorIsland.Abstractions.Interfaces;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Loader;

namespace MonitorIsland.Services;

public static class PlatformBackendLoader
{
    private const string WindowsBackendAssemblyName = "MonitorIsland.Windows";

    public static Exception? Initialize(IServiceCollection services)
    {
        var backendAssemblyName = GetBackendAssemblyName();
        if (backendAssemblyName is null)
            return null;

        try
        {
            var backendAssembly = LoadBackendAssembly(backendAssemblyName);
            var backendTypes = backendAssembly.GetExportedTypes()
                .Where(type =>
                    type is { IsClass: true, IsAbstract: false } &&
                    typeof(IMonitorIslandBackend).IsAssignableFrom(type))
                .ToList();

            if (backendTypes.Count != 1)
            {
                throw new InvalidOperationException(
                    $"平台后端 {backendAssemblyName} 应包含一个 " +
                    $"{nameof(IMonitorIslandBackend)} 实现，实际找到 {backendTypes.Count} 个");
            }

            var backend = Activator.CreateInstance(backendTypes[0])
                as IMonitorIslandBackend
                ?? throw new InvalidOperationException(
                    $"无法创建平台后端入口 {backendTypes[0].FullName}");

            backend.Initialize(services);
            return null;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    private static Assembly LoadBackendAssembly(string backendAssemblyName)
    {
        var mainAssembly = typeof(Plugin).Assembly;
        var resolver = new AssemblyDependencyResolver(mainAssembly.Location);
        var backendPath = resolver.ResolveAssemblyToPath(
            new AssemblyName(backendAssemblyName));

        if (backendPath is null)
            throw new FileNotFoundException($"未找到平台后端 {backendAssemblyName}");

        var loadContext = AssemblyLoadContext.GetLoadContext(mainAssembly)
            ?? throw new InvalidOperationException("无法获取插件程序集加载上下文");

        loadContext.Resolving += (_, assemblyName) =>
        {
            var dependencyPath = resolver.ResolveAssemblyToPath(assemblyName);
            return dependencyPath is null
                ? null
                : loadContext.LoadFromAssemblyPath(dependencyPath);
        };
        loadContext.ResolvingUnmanagedDll += (_, libraryName) =>
        {
            var dependencyPath = resolver.ResolveUnmanagedDllToPath(libraryName);
            return dependencyPath is null
                ? IntPtr.Zero
                : NativeLibrary.Load(dependencyPath);
        };

        return loadContext.LoadFromAssemblyPath(backendPath);
    }

    private static string? GetBackendAssemblyName()
    {
        if (OperatingSystem.IsWindows())
            return WindowsBackendAssemblyName;

        return null;
    }
}
