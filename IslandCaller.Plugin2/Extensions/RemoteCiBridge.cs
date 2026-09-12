using System.Reflection;
using ClassIsland.Shared;
using IslandCaller.Services.IslandCallerService;
using Microsoft.Extensions.Logging;

namespace IslandCaller.Extensions;

/// <summary>
/// RemoteCI 运行时桥：通过反射发现并注册远程扩展，避免 IslandCaller 静态引用 RemoteCI.Plugin.dll。
/// 这样即使 RemoteCI 未安装、或插件加载顺序不理想，IslandCaller 自身也能正常启动。
/// </summary>
public static class RemoteCiBridge
{
    /// <summary>远程扩展的全局唯一 Id，须保持稳定。</summary>
    internal const string ExtensionId = "IslandCaller.RandomCall";

    /// <summary>
    /// 手表端展示的 Material 图标名。
    /// 注意：RemoteCI 手表端只认编译期固化的内置图标名白名单（手表端 ExtensionIcons.kt），
    /// 白名单外的名字会被退化为纯文字，因此这里的取值必须来自白名单。可选值见 RemoteCI
    /// 文档站「接入扩展 - 扩展图标名」，casino 属于「随机与刷新」分类（shuffle/casino/autorenew/cached）。
    /// 若要使用 IslandCaller 自己的图标资源，需要 RemoteCI 侧扩展协议支持图片图标后再调整。
    /// </summary>
    internal const string WatchIconName = "casino";

    private const string RemoteCiPluginAssemblyName = "RemoteCI.Plugin";
    private const string RegistryTypeName = "RemoteCI.Plugin.Extensions.IRemoteCiExtensionRegistry";
    private const string ExtensionInterfaceName = "RemoteCI.Plugin.Extensions.IRemoteCiExtension";

    private static readonly object SyncRoot = new();
    private static object? _registry;
    private static Type? _registryType;

    /// <summary>
    /// 向 RemoteCI 注册“随机点名”远程扩展；RemoteCI 未加载或接口缺失时静默跳过。
    /// </summary>
    public static void RegisterRandomCallExtension(ILogger? logger)
    {
        lock (SyncRoot)
        {
            var assembly = FindRemoteCiAssembly();
            if (assembly is null)
            {
                logger?.LogDebug("未检测到 RemoteCI 插件，跳过远程扩展注册");
                return;
            }

            _registryType = assembly.GetType(RegistryTypeName);
            var extensionInterfaceType = assembly.GetType(ExtensionInterfaceName);
            if (_registryType is null || extensionInterfaceType is null)
            {
                logger?.LogWarning("RemoteCI 插件版本不包含扩展接口，跳过远程扩展注册");
                return;
            }

            _registry = GetRegistryService(_registryType);
            if (_registry is null)
            {
                logger?.LogWarning("未取得 RemoteCI 扩展注册表服务，跳过远程扩展注册");
                return;
            }

            var proxy = RemoteCiExtensionProxy.Create(extensionInterfaceType, logger);
            _registryType.GetMethod("Register")!.Invoke(_registry, new object[] { proxy });
            logger?.LogInformation("已注册 RemoteCI 远程扩展：随机点名");
        }
    }

    /// <summary>注销已注册的远程扩展，避免退出后残留无效入口。</summary>
    public static void UnregisterRandomCallExtension(ILogger? logger)
    {
        lock (SyncRoot)
        {
            if (_registry is null || _registryType is null) return;
            try
            {
                _registryType.GetMethod("Unregister")!.Invoke(_registry, new object[] { ExtensionId });
            }
            catch (Exception ex)
            {
                logger?.LogWarning($"注销 RemoteCI 扩展失败：{ex}");
            }
        }
    }

    /// <summary>在已加载程序集中查找 RemoteCI 插件程序集（插件加载完成后必然可见）。</summary>
    private static Assembly? FindRemoteCiAssembly() =>
        AppDomain.CurrentDomain.GetAssemblies()
            .FirstOrDefault(a => string.Equals(a.GetName().Name, RemoteCiPluginAssemblyName, StringComparison.Ordinal));

    /// <summary>反射调用 IAppHost.GetService&lt;IRemoteCiExtensionRegistry&gt;() 获取注册表服务。</summary>
    private static object? GetRegistryService(Type registryType)
    {
        var method = typeof(IAppHost)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .FirstOrDefault(m => m.Name == "GetService" && m.IsGenericMethodDefinition);
        if (method is null) return null;

        return method.MakeGenericMethod(registryType).Invoke(null, null);
    }
}

/// <summary>
/// 通过 DispatchProxy 动态实现 RemoteCI 的 IRemoteCiExtension 接口，
/// 使 IslandCaller 不需要在编译期引用 RemoteCI 的任何类型。
/// 注意：DispatchProxy 会在运行时生成该类的子类，因此此类不能声明为 sealed。
/// </summary>
internal class RemoteCiExtensionProxy : DispatchProxy
{
    private Type _commandResultType = null!;
    private ILogger? _logger;

    public RemoteCiExtensionProxy()
    {
    }

    public static object Create(Type extensionInterfaceType, ILogger? logger)
    {
        var proxy = (RemoteCiExtensionProxy)DispatchProxy.Create(extensionInterfaceType, typeof(RemoteCiExtensionProxy));
        proxy._logger = logger;

        // ExecuteAsync 返回 Task<CommandResult>，取出 CommandResult 类型用于反射构造结果。
        var executeMethod = extensionInterfaceType.GetMethod("ExecuteAsync")!;
        proxy._commandResultType = executeMethod.ReturnType.GetGenericArguments()[0];
        return proxy;
    }

    protected override object? Invoke(MethodInfo targetMethod, object?[]? args)
    {
        switch (targetMethod.Name)
        {
            case "get_Id":
                return RemoteCiBridge.ExtensionId;
            case "get_DisplayName":
                return "随机点名";
            case "get_RequiredPermission":
                // RemoteCI.Shared.UserPermissions 的 SendNotifications 位（通知类操作）。
                return Enum.Parse(targetMethod.ReturnType, "SendNotifications");
            case "get_Icon":
                return RemoteCiBridge.WatchIconName;
            case "get_Parameters":
                // 无参数扩展：返回空数组即可满足 IReadOnlyList<ExtensionParameter>。
                return Array.CreateInstance(targetMethod.ReturnType.GetGenericArguments()[0], 0);
            case "ExecuteAsync":
                return ExecuteCore();
            default:
                _logger?.LogWarning($"RemoteCI 扩展调用了未处理的成员：{targetMethod.Name}");
                return null;
        }
    }

    private object ExecuteCore()
    {
        try
        {
            // 与 IslandCallerService.ShowRandomStudent 的准入判断保持一致：课间禁用期间、
            // 或已有一次不可打断的点名正在进行时，点名不会真正执行，此时必须如实回执失败，
            // 否则手表端与 WebUI 会收到「已开始随机点名」的错误回执。
            var status = IAppHost.GetService<IslandCaller.Services.Status>();
            if (!status.IsPluginReady)
            {
                string code;
                string message;
                if (!status.IsTimeStatusAvailable)
                {
                    // 处于课间且启用了下课禁用，点名功能整体不可用。
                    code = "INVALID_REQUEST";
                    message = "当前处于课间，点名未执行";
                }
                else if (!status.OccupationDisable && !status.InterruptionEnable)
                {
                    // 上一次点名仍未结束且不允许打断，对应 RemoteCI 的 BUSY 语义。
                    code = "BUSY";
                    message = "上一次点名尚未结束，点名未执行";
                }
                else
                {
                    // 插件尚未完成初始化等其余未就绪状态。
                    code = "INVALID_REQUEST";
                    message = "IslandCaller 尚未就绪，点名未执行";
                }

                _logger?.LogInformation("RemoteCI 随机点名被拒绝（{Code}）：{Message}", code, message);
                return TaskFromResult(CreateResult(false, code, message));
            }

            // 触发一次单人随机抽选；展示与通知仍由 IslandCaller 自身完成。
            var islandCallerService = IAppHost.GetService<IslandCallerService>();
            islandCallerService.ShowRandomStudent(1);

            return TaskFromResult(CreateResult(true, "OK", "已开始随机点名"));
        }
        catch (Exception ex)
        {
            // 抛给 RemoteCI 执行端，由其统一转换为 INTERNAL_ERROR 回执。
            _logger?.LogError(ex, "RemoteCI 扩展执行失败（随机点名）");
            throw;
        }
    }

    /// <summary>
    /// 按 RemoteCI 的 CommandResult 结构反射构造执行结果。
    /// 失败码取自 RemoteCI.Shared 的 CommandResultCodes（如 BUSY、INVALID_REQUEST）。
    /// </summary>
    private object CreateResult(bool success, string code, string message)
    {
        var result = Activator.CreateInstance(_commandResultType)!;
        _commandResultType.GetProperty("Success")!.SetValue(result, success);
        _commandResultType.GetProperty("Code")!.SetValue(result, code);
        _commandResultType.GetProperty("Message")!.SetValue(result, message);
        return result;
    }

    /// <summary>反射构造 Task&lt;CommandResult&gt;，保证返回类型与接口签名一致。</summary>
    private object TaskFromResult(object result)
    {
        var fromResult = typeof(Task).GetMethod(nameof(Task.FromResult))!.MakeGenericMethod(_commandResultType);
        return fromResult.Invoke(null, new[] { result })!;
    }
}
