# Arena Systems Lab

> 적의 압박을 피하고 직접 조준해 반격하는 2D arena survival을 통해 Unity/C# 게임 시스템의 설계·측정·검증 과정을 보여 주는 포트폴리오다.

`Arena Systems Lab`은 뱀파이어 서바이버류의 생존 압박을 작은 vertical slice로 재해석했다. 자동 공격 대신 직접 조준·발사를 사용해 이동 경로와 공격 방향을 동시에 판단하게 만드는 것이 현재 핵심 재미다.

## 현재 플레이 범위

- WASD, 방향키 또는 gamepad left stick 이동
- mouse, right stick 또는 마지막 이동 방향 조준
- 기본 projectile 공격과 처치 score
- 지속적으로 생성되어 player를 추적·접촉 공격하는 적
- `Idle`, `Chase`, `Attack`, `Dead` enemy FSM과 상태별 색상 표시
- player death, Game Over, `R` restart loop

Scene과 Prefab을 직접 바꾸지 않고 `ArenaGame` runtime bootstrap이 기존 `SampleScene` 위에 arena를 구성한다. 외부 art, sound, networking package는 사용하지 않는다.

## 핵심 재미

적이 계속 좁혀 오는 상황에서 이동으로 공간을 만들고, 직접 조준한 공격으로 위험을 처치 score로 바꾸는 짧은 **압박 → 회피 → 반격 → 재도전** loop다.

## 실행 방법

필수 환경은 프로젝트와 정확히 일치하는 Unity Editor `6000.5.1f1`이다.

1. Unity Hub에서 이 폴더를 project로 연다.
2. `Assets/Scenes/SampleScene.unity`를 연다.
3. Play 버튼을 누른다.

| 동작 | Keyboard / Mouse | Gamepad |
|---|---|---|
| 이동 | `WASD` 또는 방향키 | left stick |
| 조준 | mouse pointer 또는 마지막 이동 방향 | right stick 또는 마지막 이동 방향 |
| 공격 | left click 또는 `Enter` | west button |
| 재시작 | Game Over에서 `R` | 아직 지원하지 않음 |

## 구조

| 영역 | 책임 |
|---|---|
| `Health` | damage, health, 단 한 번의 death event |
| `PlayerController` | Input System 입력, 이동, projectile 발사 |
| `EnemyController` | 추적, 접촉 damage, 상태 debug 표시 |
| `EnemyStateMachine` | 적 상태와 전이 우선순위 |
| `EnemySpawner` | arena 경계 spawn과 최대 적 수 제어 |
| `ArenaGame` | bootstrap, round lifecycle, score, Game Over/restart |
| `SpatialHash2D` | gameplay와 분리된 spatial query correctness·비용 실험 |
| `ArenaProjectValidator` | Editor version, Build Scene, Input Actions 검사 |
| `ArenaWindowsBuilder` | 검증 후 Windows x86-64 Mono Development build 생성 |
| `WireProtocol` | 길이 기반 TCP frame과 엄격한 JSON request 검증 |
| `LeaderboardServer` | loopback socket 수명, timeout, 동시 client 제한 |
| `LeaderboardStore` | 최고 score 규칙과 thread-safe bounded state |

## 자동 검증

아래 명령의 `<UnityEditor>`와 `<project-root>`를 실제 경로로 바꾼다. Test Framework 1.7에서는 test command에 `-quit`을 넣지 않는다.

```powershell
& "<UnityEditor>\Unity.exe" -batchmode -nographics -projectPath "<project-root>" -runTests -testPlatform EditMode -testFilter "ArenaSystemsLab.Tests.EditMode" -testResults "<project-root>\Logs\EditModeResults.xml" -logFile "<project-root>\Logs\EditModeTest.log"

& "<UnityEditor>\Unity.exe" -batchmode -nographics -projectPath "<project-root>" -runTests -testPlatform PlayMode -testFilter "ArenaSystemsLab.Tests.PlayMode" -testResults "<project-root>\Logs\PlayModeResults.xml" -logFile "<project-root>\Logs\PlayModeTest.log"

& "<UnityEditor>\Unity.exe" -batchmode -nographics -projectPath "<project-root>" -executeMethod ArenaSystemsLab.Editor.ArenaProjectValidator.ValidateFromCommandLine -logFile "<project-root>\Logs\ProjectValidation.log"
```

2026-09-04 기준 exact Editor에서 EditMode `16/16`, PlayMode `1/1`, command-line project validation이 통과했다.

## Windows build

Editor에서는 `Tools > Arena Systems Lab > Build Windows Development`를 사용한다. Command line에서는 다음 entry point를 실행한다.

```powershell
& "<UnityEditor>\Unity.exe" -batchmode -nographics -projectPath "<project-root>" -executeMethod ArenaSystemsLab.Editor.ArenaWindowsBuilder.BuildWindowsFromCommandLine -logFile "<project-root>\Logs\WindowsBuild.log"
```

빌드는 기존 활성 Scene과 Standalone Mono 설정만 사용한다. 설정이 다르면 자동 전환하지 않고 실패한다. 결과는 `Builds/Windows/ArenaSystemsLab.exe`이며 `Builds/`와 `Logs/`는 Git 추적 대상이 아니다.

Day 4 자동 검증에서 Windows x86-64 Development build와 8초 player launch smoke test가 통과했다. 이어서 사용자가 [demo guide](docs/DEMO_GUIDE.md)의 독립 실행 파일 gameplay checklist PASS와 오류 없음을 확인했다.

## Loopback leaderboard server

Milestone 5의 C#/.NET server foundation은 Unity project와 분리된 `Server/`에 있다. 현재 Unity game에는 아직 연결되지 않았으며 local protocol 검증만 허용한다. project root의 PowerShell에서 `<dotnet>`을 감사된 Windows .NET 10 executable로 바꿔 실행한다.

```powershell
& "<dotnet>" restore Server/ArenaSystemsLab.Server.Verification/ArenaSystemsLab.Server.Verification.csproj --configfile Server/NuGet.Config
& "<dotnet>" build Server/ArenaSystemsLab.Server.Verification/ArenaSystemsLab.Server.Verification.csproj --configuration Release --no-restore
& "<dotnet>" run --project Server/ArenaSystemsLab.Server.Verification/ArenaSystemsLab.Server.Verification.csproj --configuration Release --no-build --no-restore
& "<dotnet>" run --project Server/ArenaSystemsLab.Server/ArenaSystemsLab.Server.csproj --configuration Release --no-build --no-restore -- --port 7777
```

server는 `127.0.0.1`에만 bind하며 4-byte big-endian 길이와 UTF-8 JSON을 사용한다. 2026-09-05 Release build는 경고 0·오류 0, verification은 8/8 PASS다. protocol 한계와 원격 공개 금지 조건은 [network security baseline](docs/NETWORK_SECURITY.md)에 기록했다.

## 설계·검증 근거

- 현재 상태와 다음 작업: [PROCESS.md](PROCESS.md)
- 4일 구현 및 portfolio 확장 계획: [docs/IMPLEMENTATION_PLAN.md](docs/IMPLEMENTATION_PLAN.md)
- 측정 조건과 최적화 채택 판단: [docs/PERFORMANCE_BASELINE.md](docs/PERFORMANCE_BASELINE.md)
- 3~5분 demo와 수동 검증: [docs/DEMO_GUIDE.md](docs/DEMO_GUIDE.md)
- 게임 개발 용어 백과사전: [docs/GAME_DEV_GLOSSARY.md](docs/GAME_DEV_GLOSSARY.md)
- 네트워크 위협 모델과 protocol: [docs/NETWORK_SECURITY.md](docs/NETWORK_SECURITY.md)
- AI 작성·사람 검증 기록: [docs/AI_USAGE.md](docs/AI_USAGE.md)
- Architecture Decision Records: [docs/adr/](docs/adr/)

## 현재 한계와 다음 단계

- 도형과 IMGUI만 사용한 system prototype이며 외부 art, animation, sound가 없다.
- object pooling은 병목 근거가 없어 적용하지 않았고 `SpatialHash2D`도 실제 neighbor query가 생기기 전까지 gameplay에 연결하지 않는다.
- Day 4까지 Unity MVP와 loopback TCP server foundation을 완료했다. Unity client 연결, MySQL persistence, Unreal C++ observer, isolated SVN workflow는 후속 milestone이다.
- 현재 TCP server는 인증·TLS·server-authoritative score가 없는 local lab이다. LAN이나 public interface에 노출하지 않는다.
- 실제 협업·live-service 경험을 수행했다고 주장하지 않는다.
