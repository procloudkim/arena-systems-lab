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

## Optimization Decision

- Object pooling: 적용하지 않음. 현재 profile만으로 spawn/despawn가 병목이라고 분리해 입증하지 못했다.
- Spatial hash runtime adoption: 적용하지 않음. correctness와 실험 결과는 확보했지만 현재 gameplay에 query consumer가 없다.
- Editor validation: 적용. project version, enabled build Scene, Input Actions asset, `Player/Move`, `Player/Attack`을 한 번에 검사한다.

## Reproduction

Unity Editor가 닫혀 있고 `Temp/UnityLockfile`이 없을 때 exact Editor로 실행한다. Test Framework 1.7 규칙에 따라 test command에 `-quit`을 넣지 않는다.

```text
<UnityEditor>/Unity.exe -batchmode -nographics -projectPath <project-root> -runTests -testPlatform EditMode -testFilter ArenaSystemsLab.Tests.EditMode -testResults <project-root>/Logs/Day3EditModeResults.xml -logFile <project-root>/Logs/Day3EditMode.log

<UnityEditor>/Unity.exe -batchmode -nographics -projectPath <project-root> -runTests -testPlatform PlayMode -testFilter ArenaSystemsLab.Tests.PlayMode -testResults <project-root>/Logs/Day3PlayModeResults.xml -logFile <project-root>/Logs/Day3PlayMode.log

<UnityEditor>/Unity.exe -batchmode -nographics -projectPath <project-root> -executeMethod ArenaSystemsLab.Editor.ArenaProjectValidator.ValidateFromCommandLine -logFile <project-root>/Logs/Day3ProjectValidation.log
```

Editor menu에서는 `Tools > Arena Systems Lab > Validate Project`를 사용한다.

## Validation History

- EditMode: PASS, 16 passed / 0 failed / 0 skipped
- Project validator command line: PASS
- First PlayMode attempt: FAIL, marker recorder에 `SumAllSamplesInFrame` option 누락
- Short sampling attempt after option fix: PASS, gameplay 시간이 0.055초라 기준선에서 제외
- Reset sampling attempt: FAIL, `ProfilerRecorder.Reset()` 후 `Start()` 누락
- Final PlayMode baseline: PASS, 1 passed / 0 failed / 0 skipped
- Compiler error marker in final logs: 0
- Human menu and gameplay verification: NOT RUN

Unity가 test 실행 중 생성한 미추적 `ProjectSettings/SceneTemplateSettings.json`은 승인 범위 밖이므로 검사 후 제거해 작업 전 상태로 복원했다.
