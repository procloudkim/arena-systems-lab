# ADR 0007: Measured Day 3 Tooling

- Status: Accepted
- Date: 2026-09-04

## Context

Day 3에는 자료구조 실험, 측정 가능한 performance baseline, 반복 설정 오류를 찾는 Editor Tool이 필요하다. 현재 game은 enemy 최대 40개를 생성하지만 neighbor query가 없고 spawn/despawn가 실제 병목이라는 분리 측정도 없다. 따라서 측정 전에 pooling이나 runtime 최적화를 추가하면 효과를 입증할 수 없다.

## Decision

- `SpatialHash2D<T>`는 caller가 제공한 결과 list를 재사용하는 cell 기반 query index로 구현한다.
- deterministic dataset에서 brute-force scan과 결과 수가 같은지 검사하고 query elapsed time을 기록한다.
- 실제 gameplay에는 spatial hash를 연결하지 않는다. neighbor query와 병목이 생길 때만 adoption을 재검토한다.
- object pooling은 추가하지 않는다. spawn/despawn 병목이 별도 측정될 때만 `UnityEngine.Pool`을 검토한다.
- PlayMode test에서 `ProfilerRecorder`로 Main Thread, GC Allocated In Frame, Game Object Count를 수집한다.
- `ArenaProjectValidator`는 exact Editor version, enabled build Scene, Input Actions asset과 `Player/Move`, `Player/Attack`을 검사한다.
- validator는 Editor menu, command-line entry point, 정상·실패 rule test를 제공한다.
- Scene, Prefab, package, tracked ProjectSettings는 변경하지 않는다.

## Consequences

자료구조 correctness와 측정 결과를 남기면서 사용되지 않는 runtime 복잡성은 만들지 않는다. Editor/Test Runner 측정에는 instrumentation overhead가 있으므로 [performance baseline](../PERFORMANCE_BASELINE.md)을 비교 기준으로만 사용한다. Validator는 현재 runtime bootstrap 구조에 필요한 project-level 설정만 검사하며 Scene object graph까지 검사하지 않는다.

## Validation

- Exact Unity Editor: 6000.5.1f1
- EditMode tests: PASS, 16 passed / 0 failed / 0 skipped
- Spatial hash correctness: PASS, 6,882 matches for both query methods
- Spatial hash measured elapsed: 1.381 ms; brute-force: 73.824 ms
- Project validator command line: PASS
- Final PlayMode profile test: PASS, 1 passed / 0 failed / 0 skipped
- Profile condition: target 120 FPS, warm-up 1.0 s, sample 5.0 s, 601 samples
- Profile result: Main Thread mean 8.317 ms/max 13.756 ms; GC mean 211,689 B/max 273,180 B; GameObject mean 11.3/max 14
- Failed attempts retained: missing recorder option, then missing `Start()` after `Reset()`
- Compiler error marker in final logs: 0
- Scene, Prefab, Package, tracked ProjectSettings changes: none
- Human Editor menu and gameplay verification: NOT RUN
- Remote branch SHA: first push 후 기록
