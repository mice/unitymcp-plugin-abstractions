---
testId: UT_PLUGIN_ABSTRACTIONS_006
module: PluginAbstractions
testType: EditMode
target: UnityMcp.Plugin.UnityMcpPluginContractValidator
testFile: Tests/Editor/UnityMcpPluginContextTests.cs
testMethod: ValidateTool_AcceptsOptionalCanonicalProtocolMetadata
status: active
lastRun: "2026-09-01T23:27:00+08:00"
lastResult: passed
---

# UT_PLUGIN_ABSTRACTIONS_006

## Requirement
Tools may optionally provide a canonical MCP name through `IUnityMcpToolProtocolMetadata` without changing the base tool contract.

## Risk
Making protocol metadata mandatory would break plugins compiled against Plugin Abstractions 0.1.2.

## Steps
Validate one tool with canonical protocol metadata and one legacy tool without the optional interface.

## Assertions
Both tools pass contract validation.

## Expected Result
The new protocol metadata capability is additive.

## Latest Run Result

Passed in Unity 2022.3.53f1 as part of the focused discovery-driven rc.6 EditMode run (`16/16`).
