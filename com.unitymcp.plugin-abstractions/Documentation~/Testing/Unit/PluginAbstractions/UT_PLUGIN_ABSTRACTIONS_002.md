---
testId: UT_PLUGIN_ABSTRACTIONS_002
module: PluginAbstractions
testType: EditMode
target: UnityMcp.Plugin.UnityMcpPluginContractValidator
testFile: Tests/Editor/UnityMcpPluginContextTests.cs
testMethod: ValidatePluginContext_AllowsLegacyContextWithoutPayloads
status: active
lastRun: "2026-08-29"
lastResult: passed
---

# UT_PLUGIN_ABSTRACTIONS_002

## Requirement
The additive payload contract remains compatible with existing providers.

## Risk
Older plugin contexts could fail after upgrading Plugin Abstractions.

## Steps
Validate a context containing only legacy project and assembly properties.

## Assertions
Validation does not throw.

## Expected Result
Legacy contexts remain valid.

## Latest Run Result
Passed in Unity 2022.3.53f1 EditMode (5-test fixture run).

## Notes
None.
