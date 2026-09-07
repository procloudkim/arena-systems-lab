# 실행 및 검증 가이드

- 문서 버전: `1.0.0`
- 코드·기록 확인일: 2026-09-07 KST
- 적용 소스: `fa834cf`. 명령 인수 패턴의 기존 검증일은 2026-09-04~05이며 이번 문서 작업에서 엔진·서버를 재실행하지 않았다.
- 최신 결과: [PROCESS](../PROCESS.md). 테스트 범위는 [요구사항](REQUIREMENTS.md), 통신 오류 의미는 [프로토콜](NETWORK_SECURITY.md)을 따른다.

## 읽기 순서와 환경

게임만 실행하려면 [README의 빠른 실행](../README.md)을 따른다. 네트워크 시연은 서버를 준비한 뒤 Unity와 Unreal을 차례로 확인한다. 모든 네트워크 프로세스는 같은 Windows 호스트에서 실행한다. WSL과 Windows의 loopback이 같은 서버를 가리킨다고 가정하지 않는다.

| 대상 | 소스상 요구 / 기존 검증 환경 |
|---|---|
| Unity | ProjectVersion `6000.5.1f1`, Input System `1.19.0`, URP `17.5.0`, Test Framework `1.7.0` |
| Windows 빌드 | Windows x86-64, 기존 Standalone Mono, Development |
| .NET 서버 | `net10.0`, 검증 SDK `10.0.400`, 외부 NuGet 의존성 없음 |
| Unreal observer | association `5.8`, 검증 Engine `5.8.0`, VS Native Game/C++·MSVC `14.50`·Windows SDK `10.0.26100` |

환경 차이와 설치 승인은 [환경 감사](ENVIRONMENT_AUDIT.md) 및 [AGENTS](../AGENTS.md)를 따른다. 기존 로그는 해당 시점의 근거일 뿐 제3자 환경에서의 PASS를 보장하지 않는다.

## 명령 실행 전 확인

아래는 Windows PowerShell 명령 템플릿이다. `<project-root>`는 저장소 루트, `<UnityEditor>`는 정확한 Unity.exe의 상위 폴더, `<UnrealEngine>`는 엔진 루트, `<dotnet>`은 확인한 Windows dotnet.exe로 바꾼다. 꺾쇠 자리표시자를 그대로 실행하지 않는다.

- 프로젝트 루트에서 실행한다. 먼저 `git status --short --branch`를 기록한다.
- 같은 Unity 프로젝트 또는 Unreal 프로젝트를 다른 Editor가 열고 있다면 두 번째 인스턴스·batch 검사를 시작하지 않는다. 저장 후 정상 종료하고 관련 프로세스·잠금 파일을 확인한다. 잠금 파일을 임의 삭제하지 않는다.
- 기존 미커밋 변경과 충돌하면 멈춘다. 엔진 변환·패키지 설치·설정 자동 전환을 승인 없이 실행하지 않는다.
- 명령은 캐시·로그·빌드 생성물을 쓴다. 기존 결과를 보존해야 한다면 먼저 다른 결과 이름을 정한다.
- 종료 코드와 결과 XML/JSON·로그를 함께 확인한다. 실패한 명령 이후 단계를 성공으로 간주하지 않는다.

## 서버 준비·검증·실행

첫 restore는 서버 검증 프로젝트의 project reference를 통해 서버도 준비한다. `Server/NuGet.Config`는 외부 package source를 비운다. 필요한 SDK가 없으면 설치를 요청하고 중단한다.

```powershell
& "<dotnet>" --version
& "<dotnet>" restore Server/ArenaSystemsLab.Server.Verification/ArenaSystemsLab.Server.Verification.csproj --configfile Server/NuGet.Config
& "<dotnet>" build Server/ArenaSystemsLab.Server.Verification/ArenaSystemsLab.Server.Verification.csproj --configuration Release --no-restore
& "<dotnet>" run --project Server/ArenaSystemsLab.Server.Verification/ArenaSystemsLab.Server.Verification.csproj --configuration Release --no-build --no-restore
& "<dotnet>" run --project Server/ArenaSystemsLab.Server/ArenaSystemsLab.Server.csproj --configuration Release --no-build --no-restore -- --port 7777
```

기대 결과는 Release 빌드 오류·경고 0, 검증 8 passed / 0 failed, 서버의 `127.0.0.1:7777` listening 표시다. 마지막 명령은 서버를 계속 실행하므로 별도 창을 사용한다. 종료는 Ctrl+C다. 포트가 사용 중이면 다른 프로세스를 강제 종료하지 말고 소유자를 확인한다.

## Unity 자동 검사와 Windows 빌드

```powershell
& "<UnityEditor>\Unity.exe" -batchmode -nographics -projectPath "<project-root>" -runTests -testPlatform EditMode -testFilter "ArenaSystemsLab.Tests.EditMode" -testResults "<project-root>\Logs\EditModeResults.xml" -logFile "<project-root>\Logs\EditModeTest.log"
& "<UnityEditor>\Unity.exe" -batchmode -nographics -projectPath "<project-root>" -runTests -testPlatform PlayMode -testFilter "ArenaSystemsLab.Tests.PlayMode" -testResults "<project-root>\Logs\PlayModeResults.xml" -logFile "<project-root>\Logs\PlayModeTest.log"
& "<UnityEditor>\Unity.exe" -batchmode -nographics -projectPath "<project-root>" -executeMethod ArenaSystemsLab.Editor.ArenaProjectValidator.ValidateFromCommandLine -logFile "<project-root>\Logs\ProjectValidation.log"
& "<UnityEditor>\Unity.exe" -batchmode -nographics -projectPath "<project-root>" -executeMethod ArenaSystemsLab.Editor.ArenaWindowsBuilder.BuildWindowsFromCommandLine -logFile "<project-root>\Logs\WindowsBuild.log"
```

Test Framework 1.7의 테스트 명령에는 `-quit`을 함께 넣지 않는다. 설치된 package의 `UnityEditor.TestRunner/CommandLineTest/SettingsBuilder.cs` 경고와 기존 실행으로 확인한 인수 조합이다. validator와 builder는 batch 완료 시 스스로 종료 코드를 반환한다.

Editor 메뉴는 `Tools > Arena Systems Lab > Validate Project`, `Build Windows Development`다. 결과는 `Builds/Windows/ArenaSystemsLab.exe`와 같은 폴더의 동반 파일들이다. exe만 떼어 실행하지 않는다. 빌드 도구는 Development·Mono 전용이며 IL2CPP·Release·배포 archive를 만들지 않는다. 빌드 후 `git status`와 설정·에셋 diff를 확인한다.

## Unreal 빌드와 자동 검사

```powershell
& "<UnrealEngine>\Engine\Build\BatchFiles\Build.bat" ArenaObserverEditor Win64 Development "<project-root>\Unreal\ArenaObserver\ArenaObserver.uproject" -WaitMutex -NoHotReloadFromIDE
& "<UnrealEngine>\Engine\Binaries\Win64\UnrealEditor-Cmd.exe" "<project-root>\Unreal\ArenaObserver\ArenaObserver.uproject" -unattended -nop4 -NullRHI -NoSplash -NoSound -ExecCmds="Automation RunTests ArenaSystemsLab.ArenaObserver; Quit" -ReportExportPath="<project-root>\Unreal\ArenaObserver\Saved\Automation"
```

두 번째 명령은 기본적으로 protocol fixture 검사다. 실제 소켓 검사는 port 7777의 상태를 먼저 확인한 뒤 같은 명령에 아래 플래그 중 하나를 추가한다. 둘을 동시에 넣지 않는다.

| 플래그 | 준비 | 기대 결과 |
|---|---|---|
| `-ArenaObserverExpectNoServer` | 7777에 서버 없음 | Fetch 실패가 예상 결과로 처리되어 검사 PASS |
| `-ArenaObserverExpectServer` | 위 .NET 서버 실행 | Fetch 성공, 유효한 응답으로 검사 PASS |

각 실행의 report 경로를 구분해 이전 결과가 덮이지 않게 한다. 검사 이름은 `ArenaSystemsLab.ArenaObserver`, 기존 결과는 실행마다 1 passed / 0 failed / 0 warnings다. 이 검사는 Unreal 프로젝트 검사 결과이며 엔진 전체 로그에 오류 문자열이 전혀 없었다는 뜻은 아니다. HUD 화면은 아래 별도 수동 검사로 확인한다.

## 테스트 범위

| 검사 소스 | 다루는 요구사항 / 경계 | 다루지 않는 것 |
|---|---|---|
| [HealthTests](../Assets/ArenaSystemsLab/Tests/EditMode/HealthTests.cs) | FR-06, 피해·사망·중복 이벤트·잘못된 피해 | 실제 화면과 물리 접촉 |
| [EnemyStateMachineTests](../Assets/ArenaSystemsLab/Tests/EditMode/EnemyStateMachineTests.cs) | FR-05, 상태 전이·우선순위·종료 상태 | 이동·충돌 callback 전체 |
| [LeaderboardClientTests](../Assets/ArenaSystemsLab/Tests/EditMode/LeaderboardClientTests.cs) | FR-09, 프레임·재시도·과대 응답·서버 없음 | 실제 .NET 서버와 화면, 누락 필드·중복 ID 전체 |
| [SpatialHash2DTests](../Assets/ArenaSystemsLab/Tests/EditMode/SpatialHash2DTests.cs) | 공간 질의 정확성·비용 실험 | 게임 전체 최적화 효과 |
| [ArenaProjectValidatorTests](../Assets/ArenaSystemsLab/Tests/EditMode/ArenaProjectValidatorTests.cs) | FR-12, 설정 snapshot 검사 | 실제 빌드 전체 |
| [ArenaProfileBaselineTests](../Assets/ArenaSystemsLab/Tests/PlayMode/ArenaProfileBaselineTests.cs) | 5초 자동 gameplay 측정 | 장시간 안정성·사람 조작 전체 |
| [서버 검증 실행 파일](../Server/ArenaSystemsLab.Server.Verification/Program.cs) | FR-10, 프레임·경계·동시성·timeout | 모든 공격·운영 부하·숫자 ValueKind 오류 분류 |
| [Unreal automation](../Unreal/ArenaObserver/Source/ArenaObserver/ArenaLeaderboardProtocolTests.cpp) | FR-11, wire fixture·선택적 native socket | HUD 가독성·연속 cross-engine 시연 |

수동 검사의 공통 실패 판정은 crash·freeze·반복 Error/Exception·기대 데이터 불일치다. 기존 오류인지 이번 변경인지 판별할 수 없으면 UNKNOWN으로 기록하고 PASS를 보류한다.

## 시연 준비 조건

## 준비 조건

- Unity Editor `6000.5.1f1` exact match
- `Assets/Scenes/SampleScene.unity`가 Build Settings에서 활성화
- 실행 전 `git status --short --branch`로 예상하지 않은 변경이 없는지 확인
- Unity가 다른 process에서 이 project를 열고 있지 않음
- leaderboard 시연 시 Windows PowerShell에서 아래 local server를 먼저 실행
- Unreal observer 시연 시 exact Unreal Engine `5.8.0`과 Development Editor build 준비

```powershell
& "<dotnet>" run --project Server/ArenaSystemsLab.Server/ArenaSystemsLab.Server.csproj --configuration Release --no-build --no-restore -- --port 7777
```

## 3~5분 시연 흐름

| 시간 | 시연 | 확인할 근거 |
|---:|---|---|
| 0:00~0:30 | `README.md`에서 한 줄 정의와 핵심 재미 설명 | 작은 game loop와 engineering evidence를 함께 보여 주는 목적 |
| 0:30~1:00 | `Tools > Arena Systems Lab > Validate Project` 실행 | exact Editor, enabled Scene, `Player/Move`, `Player/Attack` validation PASS |
| 1:00~2:30 | `SampleScene` Play | 이동·조준·공격, spawn·chase, HP·score 증가 |
| 2:30~3:00 | enemy와 접촉 후 Hierarchy/색상 확인 | Gray `Idle`, Red `Chase`, Orange `Attack`; death는 terminal state |
| 3:00~3:40 | player 사망 후 leaderboard와 `R` 확인 | score 제출·상위 5개 조회 후 새 round 시작 |
| 3:40~4:30 | Test Runner와 `PERFORMANCE_BASELINE.md` | EditMode 20/20, PlayMode 1/1, 측정 전 최적화를 채택하지 않은 판단 |
| 4:30~5:00 | Windows build 근거 확인 | 독립 player build와 launch 기록 |

## Unity leaderboard 수동 체크리스트

기존 Windows build는 Unity client 구현 전 생성물이므로 이 검증은 우선 exact Editor PlayMode에서 수행한다.

| 단계 | 실행 | 기대 결과 | 실패 판정 |
|---:|---|---|---|
| 1 | port 7777 server가 없는 상태로 Play 후 사망 | Game Over와 final score가 표시되고 잠시 뒤 leaderboard가 `unavailable`로 바뀜 | freeze, crash, 반복 Console error |
| 2 | `R` 입력 | server 실패와 무관하게 HP 100·score 0인 새 round 시작 | 요청 취소나 restart 실패 |
| 3 | 위 PowerShell 명령으로 server 실행 | `127.0.0.1:7777`, protocol v1 listening message | 다른 interface bind, 즉시 종료 |
| 4 | 다시 Play하여 적을 처치한 뒤 사망 | `Leaderboard: connected`, 최고 점수 순위 표시. 새 서버에서 이 ID만 제출했다면 `UnityPlayer` 한 줄 표시 | score 불일치, 응답 미표시 |
| 5 | 한 번 더 플레이해 더 높은 score 제출 | 같은 `UnityPlayer` entry가 최고 score로 갱신 | duplicate entry 또는 낮은 score로 감소 |
| 6 | Unity Console과 server log 확인 | Unity Error/Exception 없음, server는 고정 상태 log만 출력 | payload/player ID/stack trace 노출 또는 error |
| 7 | Play 종료 후 server에서 `Ctrl+C` | server가 정상 종료되고 Unity도 PlayMode를 빠져나옴 | 종료 불가 또는 남은 process |

## Unreal observer 수동 체크리스트

준비: Unity와 Unreal Editor가 이 저장소 project를 열고 있지 않은 상태에서 위 `ArenaObserverEditor` build가 PASS했는지 확인한다. observer는 시작 시 한 번만 조회하므로 상태를 새로 읽으려면 Play를 재시작한다.

| 단계 | 실행 | 기대 결과 | 실패 판정 |
|---:|---|---|---|
| 1 | port 7777 server가 없는지 확인하고 exact Unreal 5.8로 `Unreal/ArenaObserver/ArenaObserver.uproject`를 연다 | Engine 기본 map이 열리고 project module load 오류가 없음 | version 변환 요구, module load 실패 |
| 2 | Play를 누르고 연결 결과를 기다린다 | `ARENA OBSERVER`, `Leaderboard unavailable`이 표시되고 Editor가 멈추지 않음 | freeze, crash, 화면 미표시 |
| 3 | Play를 종료하고 local leaderboard server를 실행한다 | server가 `127.0.0.1:7777` protocol v1로 대기 | 다른 interface bind, 즉시 종료 |
| 4 | 필요하면 Unity에서 score를 한 번 제출한 뒤 Unreal에서 Play를 다시 누른다 | `Connected to 127.0.0.1:7777`과 score 내림차순 Top 5 표시 | 연결 실패, 순서·score 불일치 |
| 5 | Unreal Output Log와 Message Log를 확인한다 | `ArenaObserver` C++ Error/Fatal/ensure 없음 | project code의 Error, crash, ensure |
| 6 | Play와 Editor를 종료하고 server에서 `Ctrl+C`를 누른다 | 관련 process가 모두 종료되고 port 7777이 해제됨 | 남은 Editor/server process |

## Windows player 수동 체크리스트

준비: `Tools > Arena Systems Lab > Build Windows Development`를 실행하거나 위 command-line build를 사용한다. 생성물은 `Builds/Windows/ArenaSystemsLab.exe`다.

| 단계 | 실행 | 기대 결과 | 실패 판정 |
|---:|---|---|---|
| 1 | `ArenaSystemsLab.exe` 실행 | 창이 열리고 HUD, blue player, arena가 보임 | 조기 종료, 검은 화면, missing file dialog |
| 2 | WASD/방향키로 이동 | player가 arena 경계 안에서 움직임 | 입력 없음, 경계 밖 이동 |
| 3 | mouse 조준 후 left click | yellow projectile가 pointer 방향으로 발사됨 | 발사 없음, 반복 error |
| 4 | enemy 처치 | enemy가 사라지고 score가 1 증가 | damage 또는 score 미반영 |
| 5 | enemy와 접촉 | enemy가 orange `Attack` 상태가 되고 HP가 주기적으로 감소 | 매 frame damage, 상태 전이 없음 |
| 6 | player HP를 0으로 만듦 | Game Over가 표시되고 spawn·입력이 중지됨 | round가 계속 진행되거나 exception 발생 |
| 7 | `R` 입력 | HP 100, score 0인 새 round 시작 | 이전 round object가 남거나 restart 실패 |
| 8 | player 종료 후 log/Console 확인 | 이번 변경에서 발생한 Error/Exception 없음 | compiler error, managed exception, crash |

## 기존 검증 기록 — 2026-09-04~05

| 항목 | 상태 | 근거 |
|---|---|---|
| EditMode regression | PASS | exact Editor, 20 passed / 0 failed / 0 skipped |
| PlayMode regression/profile | PASS | 1 passed / 0 failed / 0 skipped |
| Project validator CLI | PASS | validation success log |
| Windows Mono Development build | PASS | Windows x86-64 player 생성, build result Success |
| Player launch smoke | PASS | 8초간 process 생존, 조기 종료·managed exception·crash 없음 |
| Windows player 전체 gameplay | PASS | 사용자 확인, 위 8단계와 오류 없음 |
| Unity leaderboard 자동 protocol 검사 | PASS | 정상 framing·single retry·oversized response·server unavailable 4건 |
| Server-unavailable Game Over flow | PASS | 사용자 확인, unavailable·restart·Console 오류 없음 |
| Actual server Game Over flow | PASS | 사용자 확인, `UnityPlayer` 3점→11점 최고 score 갱신·중복 없음 |
| Milestone 7 Windows player rebuild | NOT RUN | 사람 PlayMode 검증 후 최종 build 단계에서 실행 |
| Unreal Development Editor build | PASS | exact Engine 5.8.0, MSVC 14.50, Windows SDK 10.0.26100 |
| Unreal protocol automation | PASS | 1 passed / 0 failed / 0 warnings |
| Unreal native socket server-unavailable | PASS | port 7777 부재에서 실패 상태 검사 PASS, 코드의 deadline 3초 |
| Unreal native socket actual server | PASS | 기존 .NET server query, test 1/1, graceful shutdown 후 port 해제 |
| Unreal server-unavailable 화면 | PASS | 사용자 확인, `Connection I/O error`가 unavailable 상태로 표시되고 Editor 응답 유지 |
| Unreal actual-server Top 5 화면 | PASS | 사용자 확인, `ObserverFixture 42` 표시·project 오류 없음 |

Player smoke log에는 GPU 환경의 D3D12 info queue 경고와 강제 종료 시점의 Unity resource cleanup 진단이 남았다. build 실패, managed exception, crash 근거는 아니며 이후 사람 검증에서도 새 Error/Exception이 없음을 확인했다.

현재 `Top 5`는 run 5개가 아니라 서로 다른 player의 최고 score 5개다. 같은 `UnityPlayer`의 이전 시도는 표시하지 않으며 과거 run 목록은 MySQL persistence 단계에서 별도 `Recent Runs`로 검증한다.

Unreal 화면 검증의 `ObserverFixture 42`는 새 in-memory server session에 protocol v1로 넣은 일회성 검증 data다. Unity의 실제 submit/query는 Milestone 7에서 별도로 사람 검증했으며, 두 경로를 같은 Unity play session에서 연속 시연하는 것은 최종 demo capture 때 수행한다.

## 시연 녹화 절차

1. 제목 화면 대신 실행 중 HUD와 몰려오는 적을 먼저 보여 준다.
2. 이동하면서 mouse로 반대 방향을 조준해 수동 공격의 차별점을 보여 준다.
3. enemy 색상과 Hierarchy 이름으로 FSM을 짧게 설명한다.
4. 일부러 사망하고 `R`로 즉시 재시작한다.
5. 마지막 화면에 test 결과, performance baseline, Windows executable을 차례로 보여 준다.

영상·GIF와 같은 서버 세션의 Unity → Unreal 연속 시연 기록은 아직 없다. 녹화 시 같은 서버를 유지한 채 Unity에서 새 점수를 제출하고 Unreal Play를 재시작해 동일 값을 확인한다.
