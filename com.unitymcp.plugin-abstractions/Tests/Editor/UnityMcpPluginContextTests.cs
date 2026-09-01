using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace UnityMcp.Plugin.Tests
{
    public sealed class UnityMcpPluginContextTests
    {
        // TestRecord: Documentation~/Testing/Unit/PluginAbstractions/UT_PLUGIN_ABSTRACTIONS_001.md
        [Test]
        [Category("UT_PLUGIN_ABSTRACTIONS_001")]
        public void ValidatePluginContext_AcceptsContainedPayload()
        {
            var pluginRoot = Path.GetFullPath(Path.Combine(Path.GetTempPath(), "UnityMcpPluginContextTests", "package"));
            var context = new UnityMcpPluginContext
            {
                PluginId = "com.example.plugin",
                PluginVersion = "1.0.0",
                PluginRoot = pluginRoot,
                Payloads = new[]
                {
                    new UnityMcpPluginPayload
                    {
                        Id = "runtime",
                        Rid = "win-x64",
                        Path = Path.Combine(pluginRoot, "Tools~", "runtimes", "win-x64", "tool.exe"),
                        Sha256 = new string('a', 64)
                    }
                }
            };

            Assert.That(() => UnityMcpPluginContractValidator.ValidatePluginContext(context), Throws.Nothing);
        }

        // TestRecord: Documentation~/Testing/Unit/PluginAbstractions/UT_PLUGIN_ABSTRACTIONS_002.md
        [Test]
        [Category("UT_PLUGIN_ABSTRACTIONS_002")]
        public void ValidatePluginContext_AllowsLegacyContextWithoutPayloads()
        {
            var context = new UnityMcpPluginContext
            {
                ProjectRoot = "project",
                AssemblyName = "Example.Plugin"
            };

            Assert.That(() => UnityMcpPluginContractValidator.ValidatePluginContext(context), Throws.Nothing);
        }

        // TestRecord: Documentation~/Testing/Unit/PluginAbstractions/UT_PLUGIN_ABSTRACTIONS_003.md
        [Test]
        [Category("UT_PLUGIN_ABSTRACTIONS_003")]
        public void ValidatePluginContext_RejectsPayloadOutsidePluginRoot()
        {
            var parent = Path.GetFullPath(Path.Combine(Path.GetTempPath(), "UnityMcpPluginContextTests"));
            var context = CreateContext(Path.Combine(parent, "package"), Path.Combine(parent, "outside", "tool.exe"));

            Assert.That(
                () => UnityMcpPluginContractValidator.ValidatePluginContext(context),
                Throws.ArgumentException.With.Message.Contains("PluginRoot"));
        }

        // TestRecord: Documentation~/Testing/Unit/PluginAbstractions/UT_PLUGIN_ABSTRACTIONS_004.md
        [Test]
        [Category("UT_PLUGIN_ABSTRACTIONS_004")]
        public void ValidatePluginContext_RejectsDuplicatePayloadIdentity()
        {
            var pluginRoot = Path.GetFullPath(Path.Combine(Path.GetTempPath(), "UnityMcpPluginContextTests", "package"));
            var context = CreateContext(pluginRoot, Path.Combine(pluginRoot, "first.exe"));
            context.Payloads = new[]
            {
                context.Payloads[0],
                new UnityMcpPluginPayload
                {
                    Id = "runtime",
                    Rid = "win-x64",
                    Path = Path.Combine(pluginRoot, "second.exe")
                }
            };

            Assert.That(
                () => UnityMcpPluginContractValidator.ValidatePluginContext(context),
                Throws.ArgumentException.With.Message.Contains("duplicated"));
        }

        // TestRecord: Documentation~/Testing/Unit/PluginAbstractions/UT_PLUGIN_ABSTRACTIONS_005.md
        [Test]
        [Category("UT_PLUGIN_ABSTRACTIONS_005")]
        public void ValidatePayload_RejectsInvalidSha256()
        {
            var pluginRoot = Path.GetFullPath(Path.Combine(Path.GetTempPath(), "UnityMcpPluginContextTests", "package"));
            var payload = new UnityMcpPluginPayload
            {
                Id = "runtime",
                Rid = "win-x64",
                Path = Path.Combine(pluginRoot, "tool.exe"),
                Sha256 = "not-a-sha256"
            };

            Assert.That(
                () => UnityMcpPluginContractValidator.ValidatePayload(payload, pluginRoot),
                Throws.ArgumentException.With.Message.Contains("SHA-256"));
        }

        // TestRecord: Documentation~/Testing/Unit/PluginAbstractions/UT_PLUGIN_ABSTRACTIONS_006.md
        [Test]
        [Category("UT_PLUGIN_ABSTRACTIONS_006")]
        public void ValidateTool_AcceptsOptionalCanonicalProtocolMetadata()
        {
            Assert.That(() => UnityMcpPluginContractValidator.ValidateTool(new ProtocolTool("unity_android_debug_status")), Throws.Nothing);
            Assert.That(() => UnityMcpPluginContractValidator.ValidateTool(new LegacyTool()), Throws.Nothing);
        }

        // TestRecord: Documentation~/Testing/Unit/PluginAbstractions/UT_PLUGIN_ABSTRACTIONS_007.md
        [TestCase("")]
        [TestCase("android_debug_status")]
        [TestCase("unity_Android_debug_status")]
        [TestCase("unity_android__debug_status")]
        [TestCase("unity_android_debug_status_")]
        [TestCase("unity_android-debug-status")]
        [TestCase("unity_android_调试")]
        [Category("UT_PLUGIN_ABSTRACTIONS_007")]
        public void ValidateTool_RejectsInvalidProtocolMetadata(string mcpName)
        {
            Assert.That(
                () => UnityMcpPluginContractValidator.ValidateTool(new ProtocolTool(mcpName)),
                Throws.ArgumentException.With.Message.Contains("canonical"));
        }

        private static UnityMcpPluginContext CreateContext(string pluginRoot, string payloadPath)
        {
            return new UnityMcpPluginContext
            {
                PluginId = "com.example.plugin",
                PluginRoot = Path.GetFullPath(pluginRoot),
                Payloads = new[]
                {
                    new UnityMcpPluginPayload
                    {
                        Id = "runtime",
                        Rid = "win-x64",
                        Path = Path.GetFullPath(payloadPath)
                    }
                }
            };
        }

        private class LegacyTool : IUnityMcpTool
        {
            public UnityMcpToolDescriptor Descriptor { get; } = new UnityMcpToolDescriptor
            {
                Name = "unity.example.get",
                Title = "Example",
                Description = "Example tool.",
                DefaultTimeoutMs = 1000,
                AllowedRuntimeModes = UnityMcpToolRuntimeModes.Edit,
                SideEffect = UnityMcpToolSideEffect.None
            };

            public UnityMcpSchemaDeclaration InputSchema { get; } = new UnityMcpSchemaDeclaration
            {
                Kind = UnityMcpSchemaKind.InlineJson,
                Value = "{}"
            };

            public UnityMcpToolResult Execute(UnityMcpToolContext context, IUnityMcpCancellation cancellation)
            {
                return new UnityMcpToolResult { Success = true, Status = UnityMcpToolStatus.Success };
            }
        }

        private sealed class ProtocolTool : LegacyTool, IUnityMcpToolProtocolMetadata
        {
            public ProtocolTool(string mcpName)
            {
                McpName = mcpName;
            }

            public string McpName { get; }
        }
    }
}
