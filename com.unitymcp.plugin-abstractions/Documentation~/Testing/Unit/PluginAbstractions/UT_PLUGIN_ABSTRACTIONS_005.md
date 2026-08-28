---
testId: UT_PLUGIN_ABSTRACTIONS_005
module: PluginAbstractions
testType: EditMode
target: UnityMcp.Plugin.UnityMcpPluginContractValidator
testFile: Tests/Editor/UnityMcpPluginContextTests.cs
testMethod: ValidatePayload_RejectsInvalidSha256
status: active
lastRun: "2026-08-29"
lastResult: passed
---

# UT_PLUGIN_ABSTRACTIONS_005

## Requirement
Optional SHA-256 metadata must use the canonical hexadecimal shape.

## Risk
Malformed integrity metadata could be treated as valid evidence.

## Steps
Validate a payload with an invalid SHA-256 value.

## Assertions
Validation throws an argument error identifying SHA-256.

## Expected Result
Malformed hash metadata is rejected.

## Latest Run Result
Passed in Unity 2022.3.53f1 EditMode (5-test fixture run).

## Notes
None.
