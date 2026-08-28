---
testId: UT_PLUGIN_ABSTRACTIONS_003
module: PluginAbstractions
testType: EditMode
target: UnityMcp.Plugin.UnityMcpPluginContractValidator
testFile: Tests/Editor/UnityMcpPluginContextTests.cs
testMethod: ValidatePluginContext_RejectsPayloadOutsidePluginRoot
status: active
lastRun: "2026-08-29"
lastResult: passed
---

# UT_PLUGIN_ABSTRACTIONS_003

## Requirement
Resolved payload paths must remain within the plugin root.

## Risk
A manifest could expose arbitrary host filesystem content to a provider.

## Steps
Validate a context whose payload resolves to a sibling directory.

## Assertions
Validation throws an argument error identifying the plugin-root boundary.

## Expected Result
The escaping payload is rejected.

## Latest Run Result
Passed in Unity 2022.3.53f1 EditMode (5-test fixture run).

## Notes
None.
