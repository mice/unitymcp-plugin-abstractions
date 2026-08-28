---
testId: UT_PLUGIN_ABSTRACTIONS_004
module: PluginAbstractions
testType: EditMode
target: UnityMcp.Plugin.UnityMcpPluginContractValidator
testFile: Tests/Editor/UnityMcpPluginContextTests.cs
testMethod: ValidatePluginContext_RejectsDuplicatePayloadIdentity
status: active
lastRun: "2026-08-29"
lastResult: passed
---

# UT_PLUGIN_ABSTRACTIONS_004

## Requirement
Payload IDs must be unique within one plugin context.

## Risk
Providers could select ambiguous executable content.

## Steps
Validate two payloads with the same ID.

## Assertions
Validation throws an argument error identifying the duplicate.

## Expected Result
Duplicate payload identity is rejected.

## Latest Run Result
Passed in Unity 2022.3.53f1 EditMode (5-test fixture run).

## Notes
None.
