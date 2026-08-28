---
testId: UT_PLUGIN_ABSTRACTIONS_001
module: PluginAbstractions
testType: EditMode
target: UnityMcp.Plugin.UnityMcpPluginContractValidator
testFile: Tests/Editor/UnityMcpPluginContextTests.cs
testMethod: ValidatePluginContext_AcceptsContainedPayload
status: active
lastRun: "2026-08-29"
lastResult: passed
---

# UT_PLUGIN_ABSTRACTIONS_001

## Requirement
Resolved plugin payloads inside an absolute plugin root are accepted.

## Risk
Valid package content could be rejected and prevent external plugins from loading.

## Steps
Validate one Windows payload below its plugin root.

## Assertions
Validation does not throw.

## Expected Result
The contained payload context is valid.

## Latest Run Result
Passed in Unity 2022.3.53f1 EditMode (5-test fixture run).

## Notes
None.
