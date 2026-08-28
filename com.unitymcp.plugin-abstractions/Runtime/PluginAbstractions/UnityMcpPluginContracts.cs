using System;
using System.Collections.Generic;
using System.IO;

namespace UnityMcp.Plugin
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class UnityMcpPluginAttribute : Attribute
    {
        public UnityMcpPluginAttribute(string pluginId, string pluginVersion)
        {
            PluginId = pluginId;
            PluginVersion = pluginVersion;
        }

        public string PluginId { get; }

        public string PluginVersion { get; }
    }

    [Flags]
    public enum UnityMcpToolRuntimeModes
    {
        None = 0,
        Edit = 1 << 0,
        Play = 1 << 1,
        EditAndPlay = Edit | Play
    }

    public enum UnityMcpToolSideEffect
    {
        None = 0,
        ReadsProject = 1,
        MutatesProject = 2,
        RunsUserCode = 3
    }

    public enum UnityMcpSchemaKind
    {
        InlineJson = 0,
        AssetPath = 1,
        PackagePath = 2,
        EmbeddedResource = 3
    }

    public sealed class UnityMcpRoslynCompilerPayload
    {
        public string CompilerPath { get; set; }

        public string RuntimeMode { get; set; }

        public string RuntimeVersion { get; set; }
    }

    public sealed class UnityMcpPluginContext
    {
        public string ProjectRoot { get; set; }

        public string AssemblyName { get; set; }

        public string PluginId { get; set; }

        public string PluginVersion { get; set; }

        public string PluginRoot { get; set; }

        public IReadOnlyList<UnityMcpPluginPayload> Payloads { get; set; } = Array.Empty<UnityMcpPluginPayload>();

        public UnityMcpRoslynCompilerPayload RoslynCompilerPayload { get; set; }

        public object HostServices { get; set; }
    }

    public sealed class UnityMcpPluginPayload
    {
        public string Id { get; set; }

        public string Rid { get; set; }

        public string Path { get; set; }

        public string Sha256 { get; set; }
    }

    public sealed class UnityMcpToolContext
    {
        public string CommandId { get; set; }

        public string ToolName { get; set; }

        public int TimeoutMs { get; set; }

        public string RawArgsJson { get; set; }

        public string ProjectRoot { get; set; }

        public string TempRoot { get; set; }
    }

    public interface IUnityMcpCancellation
    {
        bool IsCancellationRequested { get; }

        void ThrowIfCancellationRequested();
    }

    public interface IUnityMcpToolProvider
    {
        IEnumerable<IUnityMcpTool> GetTools(UnityMcpPluginContext context);
    }

    public interface IUnityMcpTool
    {
        UnityMcpToolDescriptor Descriptor { get; }

        UnityMcpSchemaDeclaration InputSchema { get; }

        UnityMcpToolResult Execute(UnityMcpToolContext context, IUnityMcpCancellation cancellation);
    }

    public static class UnityMcpToolStatus
    {
        public const string Success = "success";
        public const string InvalidArgs = "invalid_args";
        public const string Failed = "failed";
        public const string Timeout = "timeout";
        public const string Cancelled = "cancelled";
        public const string Exception = "exception";
    }

    [Serializable]
    public sealed class UnityMcpToolResult
    {
        public bool Success;
        public string Status;
        public string Summary;
        public List<UnityMcpToolError> Errors = new List<UnityMcpToolError>();
        public List<UnityMcpToolWarning> Warnings = new List<UnityMcpToolWarning>();
        public List<UnityMcpToolLog> Logs = new List<UnityMcpToolLog>();
        public string MetricsObjectJson = "{}";
        public List<string> ChangedFiles = new List<string>();
        public string ReportPath;
    }

    [Serializable]
    public sealed class UnityMcpToolError
    {
        public string Code;
        public string Message;
        public string File;
        public int Line;
        public int Column;
    }

    [Serializable]
    public sealed class UnityMcpToolWarning
    {
        public string Code;
        public string Message;
    }

    [Serializable]
    public sealed class UnityMcpToolLog
    {
        public string Level;
        public string Message;
        public string Timestamp;
    }

    public sealed class UnityMcpToolDescriptor
    {
        public string Name { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public int DefaultTimeoutMs { get; set; }

        public UnityMcpToolRuntimeModes AllowedRuntimeModes { get; set; }

        public UnityMcpToolSideEffect SideEffect { get; set; }

        public bool MayTriggerDomainReload { get; set; }
    }

    public sealed class UnityMcpSchemaDeclaration
    {
        public UnityMcpSchemaKind Kind { get; set; }

        public string Value { get; set; }

        public string ResourceName { get; set; }
    }

    public static class UnityMcpPluginContractValidator
    {
        public static void ValidatePluginContext(UnityMcpPluginContext context, string paramName = "context")
        {
            if (context == null)
            {
                throw new ArgumentNullException(paramName);
            }

            if (context.Payloads == null || context.Payloads.Count == 0)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(context.PluginId))
            {
                throw new ArgumentException("PluginId is required when payloads are provided.", paramName);
            }

            if (string.IsNullOrWhiteSpace(context.PluginRoot) || !Path.IsPathRooted(context.PluginRoot))
            {
                throw new ArgumentException("PluginRoot must be an absolute path when payloads are provided.", paramName);
            }

            var pluginRoot = Path.GetFullPath(context.PluginRoot);
            var payloadIds = new HashSet<string>(StringComparer.Ordinal);
            foreach (var payload in context.Payloads)
            {
                ValidatePayload(payload, pluginRoot, payloadIds, paramName);
            }
        }

        public static void ValidatePayload(UnityMcpPluginPayload payload, string pluginRoot, string paramName = "payload")
        {
            if (string.IsNullOrWhiteSpace(pluginRoot) || !Path.IsPathRooted(pluginRoot))
            {
                throw new ArgumentException("Plugin root must be an absolute path.", nameof(pluginRoot));
            }

            ValidatePayload(payload, Path.GetFullPath(pluginRoot), new HashSet<string>(StringComparer.Ordinal), paramName);
        }

        public static void ValidateTool(IUnityMcpTool tool, string paramName = "tool")
        {
            if (tool == null)
            {
                throw new ArgumentNullException(paramName);
            }

            ValidateDescriptor(tool.Descriptor, $"{paramName}.Descriptor");
            ValidateSchema(tool.InputSchema, $"{paramName}.InputSchema");
        }

        public static void ValidateDescriptor(UnityMcpToolDescriptor descriptor, string paramName = "descriptor")
        {
            if (descriptor == null)
            {
                throw new ArgumentNullException(paramName);
            }

            if (string.IsNullOrWhiteSpace(descriptor.Name))
            {
                throw new ArgumentException("Tool name is required.", paramName);
            }

            if (string.IsNullOrWhiteSpace(descriptor.Title))
            {
                throw new ArgumentException($"Tool '{descriptor.Name}' must declare a title.", paramName);
            }

            if (string.IsNullOrWhiteSpace(descriptor.Description))
            {
                throw new ArgumentException($"Tool '{descriptor.Name}' must declare a description.", paramName);
            }

            if (descriptor.DefaultTimeoutMs <= 0)
            {
                throw new ArgumentException($"Tool '{descriptor.Name}' must declare a positive default timeout.", paramName);
            }

            if (descriptor.AllowedRuntimeModes == UnityMcpToolRuntimeModes.None)
            {
                throw new ArgumentException($"Tool '{descriptor.Name}' must declare at least one allowed runtime mode.", paramName);
            }
        }

        public static void ValidateSchema(UnityMcpSchemaDeclaration schema, string paramName = "schema")
        {
            if (schema == null)
            {
                throw new ArgumentNullException(paramName);
            }

            switch (schema.Kind)
            {
                case UnityMcpSchemaKind.InlineJson:
                case UnityMcpSchemaKind.AssetPath:
                case UnityMcpSchemaKind.PackagePath:
                    if (string.IsNullOrWhiteSpace(schema.Value))
                    {
                        throw new ArgumentException("Schema value is required.", paramName);
                    }

                    break;
                case UnityMcpSchemaKind.EmbeddedResource:
                    if (string.IsNullOrWhiteSpace(schema.ResourceName))
                    {
                        throw new ArgumentException("Embedded resource schemas require ResourceName.", paramName);
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException(paramName, schema.Kind, "Unsupported schema kind.");
            }
        }

        private static void ValidatePayload(
            UnityMcpPluginPayload payload,
            string pluginRoot,
            ISet<string> payloadIds,
            string paramName)
        {
            if (payload == null)
            {
                throw new ArgumentNullException(paramName);
            }

            if (string.IsNullOrWhiteSpace(payload.Id))
            {
                throw new ArgumentException("Payload Id is required.", paramName);
            }

            if (!payloadIds.Add(payload.Id))
            {
                throw new ArgumentException($"Payload Id '{payload.Id}' is duplicated.", paramName);
            }

            if (string.IsNullOrWhiteSpace(payload.Rid))
            {
                throw new ArgumentException($"Payload '{payload.Id}' must declare a RID.", paramName);
            }

            if (string.IsNullOrWhiteSpace(payload.Path) || !Path.IsPathRooted(payload.Path))
            {
                throw new ArgumentException($"Payload '{payload.Id}' path must be absolute.", paramName);
            }

            var payloadPath = Path.GetFullPath(payload.Path);
            var relativePath = Path.GetRelativePath(pluginRoot, payloadPath);
            if (relativePath == ".." ||
                relativePath.StartsWith(".." + Path.DirectorySeparatorChar, StringComparison.Ordinal) ||
                Path.IsPathRooted(relativePath))
            {
                throw new ArgumentException($"Payload '{payload.Id}' path must remain under PluginRoot.", paramName);
            }

            if (!string.IsNullOrWhiteSpace(payload.Sha256) && !IsSha256(payload.Sha256))
            {
                throw new ArgumentException($"Payload '{payload.Id}' SHA-256 must contain 64 hexadecimal characters.", paramName);
            }
        }

        private static bool IsSha256(string value)
        {
            if (value.Length != 64)
            {
                return false;
            }

            for (var index = 0; index < value.Length; index++)
            {
                var character = value[index];
                if (!((character >= '0' && character <= '9') ||
                      (character >= 'a' && character <= 'f') ||
                      (character >= 'A' && character <= 'F')))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
