---
testId: UT_PLUGIN_ABSTRACTIONS_007
module: PluginAbstractions
testType: EditMode
target: UnityMcp.Plugin.UnityMcpPluginContractValidator
testFile: Tests/Editor/UnityMcpPluginContextTests.cs
testMethod: ValidateTool_RejectsInvalidProtocolMetadata
status: active
lastRun: "2026-09-01T23:27:00+08:00"
lastResult: passed
---

# UT_PLUGIN_ABSTRACTIONS_007

## Requirement
Declared MCP names must use canonical ASCII `unity_` lower snake case with no empty segments.

## Risk
Malformed or ambiguous protocol names could cross the plugin trust boundary and collide in MCP catalogs.

## Steps
Validate empty, wrong-prefix, mixed-case, double-underscore, trailing-underscore, punctuation, and non-ASCII names.

## Assertions
Each invalid declaration fails contract validation.

## Expected Result
Only canonical MCP names are accepted.

## Latest Run Result

Passed in Unity 2022.3.53f1 as part of the focused discovery-driven rc.6 EditMode run (`16/16`).
