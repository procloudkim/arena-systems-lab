# Day 3 Performance Baseline

> 이 문서는 측정 시점의 기준선이다. 수치는 해당 조건에서만 유효하며 최적화 효과나 player build 성능으로 일반화하지 않는다.

## Measurement Environment

- Date: 2026-09-04 KST
- Unity Editor: 6000.5.1f1 exact project version
- Mode: Editor PlayMode, batchmode, nographics, Unity Test Framework
- Scenario: runtime bootstrap, player input 없음, enemy 자동 spawn과 chase
- Target frame rate: 120
- Warm-up: 1.0 seconds
- Sampling: 5.0 seconds, 601 samples
- Recorder source: Unity `ProfilerRecorder`

## Gameplay Baseline

| Metric | Mean | Maximum |
|---|---:|---:|
| Main Thread frame time | 8.317 ms | 13.756 ms |
| GC Allocated In Frame | 211,689 bytes | 273,180 bytes |
| Game Object Count | 11.3 | 14 |

이 결과에는 Editor, Test Runner, coroutine과 profiler instrumentation 비용이 포함된다. `Application.targetFrameRate = 120`의 frame 대기 시간도 Main Thread 값에 포함될 수 있다. 따라서 Day 4 Windows player build에서 같은 조건을 다시 측정하기 전에는 release 성능이나 병목을 주장하지 않는다.

## Spatial Query Experiment

고정 seed `20260904`로 200 × 200 범위에 20,000 point를 만들고 radius 3인 query 500회를 실행했다. Spatial hash cell size는 2이며, 두 방식 모두 총 6,882개 match를 반환했다.

| Query | Total elapsed |
|---|---:|
| `SpatialHash2D` | 1.381 ms |
| Brute-force scan | 73.824 ms |

측정값은 Editor의 단일 microbenchmark 결과이며 합격 threshold로 사용하지 않는다. 현재 gameplay에는 neighbor query가 없으므로 `SpatialHash2D`를 runtime에 연결하지 않았다.

당시 검사 범위의 한계: 2026-09-04 비교 테스트는 500개 query의 전체 match 수 합계만 비교했다. 2026-09-09 [ADR 0014](adr/0014-v1-contract-hardening.md)에서 측정 구간 밖에 query별 결과 ID·개수 비교를 추가하고 반지름 0·정확한 경계 검사를 보강했다. [SpatialHash2DTests](../Assets/ArenaSystemsLab/Tests/EditMode/SpatialHash2DTests.cs)의 exact Editor 회귀 검사 PASS이며 위 과거 측정값을 새 성능 수치로 대체하지 않는다.

## Optimization Decision

- Object pooling: 적용하지 않음. 현재 profile만으로 spawn/despawn가 병목이라고 분리해 입증하지 못했다.
- Spatial hash runtime adoption: 적용하지 않음. correctness와 실험 결과는 확보했지만 현재 gameplay에 query consumer가 없다.
- Editor validation: 적용. project version, enabled build Scene, Input Actions asset, `Player/Move`, `Player/Attack`을 한 번에 검사한다.

## Reproduction

안전 조건과 정확한 PowerShell 명령 인수 패턴은 [실행 및 검증 가이드](DEMO_GUIDE.md)를 따른다. EditMode에서 spatial query 실험, PlayMode에서 자동 gameplay sampling을 실행한다. 결과 경로는 이전 기록을 덮지 않도록 선택한다.

새 실행은 현재 소스에 대한 측정이다. 위 수치는 2026-09-04 당시 결과이며 이후 network client 추가 등 변경이 포함된 새 측정값으로 대체하거나 동일하다고 가정하지 않는다.

## Validation History

- EditMode: PASS, 16 passed / 0 failed / 0 skipped
- Project validator command line: PASS
- First PlayMode attempt: FAIL, marker recorder에 `SumAllSamplesInFrame` option 누락
- Short sampling attempt after option fix: PASS, gameplay 시간이 0.055초라 기준선에서 제외
- Reset sampling attempt: FAIL, `ProfilerRecorder.Reset()` 후 `Start()` 누락
- Final PlayMode baseline: PASS, 1 passed / 0 failed / 0 skipped
- Compiler error marker in final logs: 0
- Human menu, existing gameplay flow, and Console checklist: PASS, 사용자 확인

Unity가 test 실행 중 생성한 미추적 `ProjectSettings/SceneTemplateSettings.json`은 승인 범위 밖이므로 검사 후 제거해 작업 전 상태로 복원했다.
