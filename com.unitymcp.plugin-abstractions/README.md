# Unity MCP Plugin Abstractions

This Unity package provides the `UnityMcp.Plugin.Abstractions` assembly used by Unity Agent Bridge plugins.

It defines the plugin-facing provider, tool, schema, descriptor, context, cancellation, and result contracts in the `UnityMcp.Plugin` namespace.

External package plugins receive immutable package content through `UnityMcpPluginContext.PluginRoot` and `Payloads`. Hosts resolve payload paths, validate them against the package root, and pass absolute paths to providers; providers should not infer package locations from their assembly path.

`UnityMcpPluginContractValidator.ValidatePluginContext` validates payload identity, RID, absolute normalized path containment, and optional SHA-256 metadata without depending on Unity or Agent Bridge types.

Tools may additionally implement `IUnityMcpToolProtocolMetadata` to declare a canonical MCP-facing name from inside the provider DLL. Hosts validate declared names as `unity_` lower snake case. Tools compiled against earlier package versions remain valid and may omit this optional interface.

Package id: `com.unitymcp.plugin-abstractions`

Assembly name: `UnityMcp.Plugin.Abstractions`
