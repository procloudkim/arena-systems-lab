# AI Usage Record

> 이 문서는 AI 작업의 시간순 기록이다. 현재 진행 상태와 다음 작업은 [PROCESS.md](../PROCESS.md)를 따른다.

## AI가 수행한 조사

- Unity 프로젝트 root와 Git 상태 확인
- 프로젝트 Unity version과 설치 Editor의 exact-match 확인
- Unity Hub와 Windows/Mono, IL2CPP, WebGL, Android, Dedicated Server module 확인
- manifest와 package lock의 직접/간접 dependency 및 일관성 확인
- Input System, Active Input Handling, Input Actions 구조 확인
- Test Framework, test directory, asmdef 상태 확인
- Version Control Mode, serialization, Build Target, Build Scene, URP 설정 확인
- 기존 script, Scene, Prefab, documentation, large binary 확인
- Git, Git LFS, Codex, Visual Studio, VS Code와 선택 도구 확인
- 현재 Editor log의 초기 compiler/import error 표식 확인
- Git 저장소 초기화 후 Unity 생성물 제외와 source/`.meta` 추적 가능 여부 확인
- 공개 GitHub repository의 naming pattern과 대상 이름 충돌 여부 확인

## AI가 생성하거나 수정한 파일

- `AGENTS.md`: 프로젝트 작업 지침 생성
- `PROCESS.md`: 현재 상태와 session 재개 지점의 단일 기준 생성
- `docs/GAME_DEV_GLOSSARY.md`: 개발 중 등장한 게임·Unity·물리·검증 용어 백과사전 생성
- `docs/ENVIRONMENT_AUDIT.md`: 환경 감사 기록 생성
- `docs/IMPLEMENTATION_PLAN.md`: 4일 구현 계획 생성
- `docs/AI_USAGE.md`: 이 기록 생성
- `.gitignore`: Unity/IDE/OS 생성물 제외 규칙 생성
- `docs/adr/0001-commit-push-and-adr-workflow.md`: branch, ADR, commit, push 운영 결정 기록
- `docs/adr/0002-project-naming.md`: project naming 범위와 보존 결정 기록
- `docs/adr/0003-process-and-adr-governance.md`: PROCESS SSOT와 ADR naming governance 기록
- `docs/adr/0004-game-development-glossary-governance.md`: glossary 구성과 versioning governance 기록
- `docs/adr/0005-minimal-enemy-fsm.md`: 최소 enemy FSM 구조와 전이 우선순위 기록
- `Assets/ArenaSystemsLab/Runtime/ArenaSystemsLab.Runtime.asmdef`
- `Assets/ArenaSystemsLab/Runtime/Health.cs`
- `Assets/ArenaSystemsLab/Runtime/ArenaGame.cs`
- `Assets/ArenaSystemsLab/Runtime/PlayerController.cs`
- `Assets/ArenaSystemsLab/Runtime/Projectile.cs`
- `Assets/ArenaSystemsLab/Runtime/EnemyController.cs`
- `Assets/ArenaSystemsLab/Runtime/EnemySpawner.cs`
- `Assets/ArenaSystemsLab/Runtime/EnemyStateMachine.cs`
- `Assets/ArenaSystemsLab/Tests/EditMode/ArenaSystemsLab.Tests.EditMode.asmdef`
- `Assets/ArenaSystemsLab/Tests/EditMode/HealthTests.cs`
- `Assets/ArenaSystemsLab/Tests/EditMode/EnemyStateMachineTests.cs`

## 사람이 확인한 항목

- PlayMode에서 이동, 공격, spawn, chase, death, Game Over, restart: PASS
- Unity Console error 확인: 오류 없음
- Day 2 enemy 상태 색상과 FSM 적용 후 전체 수동 흐름: PASS
- Day 2 변경 후 Unity Console error 확인: 오류 없음
- Day 3 Editor menu, 기존 gameplay 회귀, Unity Console checklist: PASS

## 사람이 추후 결정하거나 확인해야 할 항목

- Windows build Editor menu 동작

## 실행된 테스트

- 초기 Editor log compiler error 표식 검사: 0건
- 초기 Editor log asset import error 표식 검사: 0건
- manifest/lock JSON parse와 direct dependency 일관성 검사: issue 0건
- Runtime/Test assembly compilation: PASS
- EditMode test: 5 passed, 0 failed, 0 skipped
- 첫 batch test 시도: `-quit` 때문에 Test Runner가 시작되지 않아 NOT RUN
- 두 번째 batch test: `-quit` 제거 후 exit code 0, result Passed
- PlayMode manual verification: PASS, 사용자 확인
- Unity Console: PASS, 사용자 확인
- 이름 변경 후 exact Editor compilation 및 EditMode test: PASS, 5 passed / 0 failed / 0 skipped
- Day 2 exact Editor compilation 및 EditMode test: PASS, 9 passed / 0 failed / 0 skipped
- Day 2 PlayMode FSM 흐름과 Unity Console: PASS, 사용자 확인

## 미검증 항목

- Windows build Editor menu 수동 실행

## AI 제안을 그대로 채택하지 않은 부분

- Input System과 Test Framework가 이미 있어 package 추가를 제안하지 않았다.
- IL2CPP, Android, Dedicated Server는 Day 1에 필요 없어 설치하지 않았다.
- Performance Testing API는 lock의 간접 dependency일 뿐이며 직접 dependency로 추가하지 않았다.
- 기존 Scene과 package 파일은 수정하지 않았다. Project Settings는 사용자 승인 범위의 naming field 3개만 수정했다.
- 최초 감사에서는 Git repository와 `.gitignore`를 자동 생성하지 않았고, 사용자 승인 후 별도 단계에서 준비했다.
- 외부 art, networking, database, DI, tween, async helper package를 사용하지 않는다.

## 감사 중 발생한 부작용

`code --version`이 WSL용 VS Code Server를 자동 갱신하고 이전 설치 제거를 시도했다. 새 server directory가 생성된 것은 확인했으며, 이는 의도하지 않은 IDE update다. 프로젝트 파일에는 영향을 주지 않았고 AI는 추가 `code` CLI 호출이나 rollback을 실행하지 않았다.

## 구현 및 검증 메모

기존 Scene과 Prefab을 수정하지 않고 runtime bootstrap을 사용했다. Health는 invalid damage를 무시하고 death event를 한 번만 발생시키며, 이 동작을 EditMode test로 검증했다. test 실행 중 최초 license channel handshake가 실패했지만 새 channel 연결과 license update가 성공했고 최종 test 결과에는 영향을 주지 않았다. 사용자가 Day 1 PlayMode 흐름과 Console error 없음도 확인했다.

## Git 준비 기록

`2026-09-04T02:03:33+09:00`에 사용자 진행 지시를 받아 실제 Unity 프로젝트 root에서 `main` 브랜치의 빈 Git repository를 초기화하고 `.gitignore`를 생성했다. `Library/`, `Temp/`, `Obj/`, `Build/`, `Builds/`, `Logs/`, `UserSettings/`, IDE 생성물을 제외하며 `Assets/`, `Packages/`, `ProjectSettings/`, `.meta`는 제외하지 않는다. stage, commit, LFS 초기화는 수행하지 않았다.

## GitHub 원격 생성 기록

`2026-09-04T02:09:17+09:00`에 사용자의 명시적 요청으로 public `arena-systems-lab` repository를 생성하고 로컬 `origin`으로 연결했다. 공개 repository 목록의 일반 프로젝트 naming pattern인 소문자 kebab-case와 기능 중심 명칭을 적용했다. 원격 repository는 비어 있으며 stage, commit, push는 수행하지 않았다.

## Git 운영 정책 변경

사용자의 최신 지침에 따라 기존 commit/push 금지 규칙을 교체했다. 최초 기준선은 `main`에 남기고, 이후 주요 작업은 `work/<short-kebab-topic>` branch에서 진행한다. 의미 있는 변경은 ADR과 실제 검증 결과를 함께 기록하고 같은 작업 turn에서 commit과 push까지 완료한다.

Day 1 source, test, 환경 문서, `.gitignore`, ADR을 최초 검증 기준선 `51bd3a4`로 commit하고 `origin/main`에 push했다. 저장소 로컬 commit 작성자는 GitHub noreply identity를 사용하며 전역 Git 설정은 변경하지 않았다.

## 프로젝트 이름 정리 기록

`work/project-naming` branch에서 활성 project folder leaf와 PlayerSettings 이름을 `Arena Systems Lab`으로 통일했다. `metroPackageName`은 공백 없는 `ArenaSystemsLab`을 사용했다. Location container의 불완전한 `My`, `My project` 폴더는 사용자 데이터 보호를 위해 그대로 두었다.

이름 변경 후 exact Editor 6000.5.1f1로 새 경로를 열어 compile failure 표식 0건과 EditMode test 5건 통과를 확인했다. Unity는 승인 범위 밖의 tracked file을 변경하지 않았고 Windows player build는 실행하지 않았다.

구현 commit `20157ea`를 `origin/work/project-naming`에 push하고 local/remote SHA 일치를 확인했다.

## Process SSOT 정리 기록

상태 정보가 여러 문서에 흩어진 위치를 두 가지 검색으로 감사하고 `PROCESS.md`를 현재 상태의 단일 기준으로 선택했다. `AGENTS.md`와 구현 계획의 현재 상태 복사본은 참조로 교체하고, 환경 감사와 AI 기록은 역사적 문서임을 명시했다. ADR 0003에서 ADR filename, status, 필수 section, supersede, checkpoint 규칙을 확정했다.

구현 commit `c6a7fb0`를 `origin/work/process-governance`에 push하고 local/remote SHA 일치를 확인했다.

## 게임 개발 용어 백과사전 기록

사용자의 용어 학습 요청에 따라 `docs/GAME_DEV_GLOSSARY.md` `0.1.0`을 생성했다. 현재 코드와 확정된 Day 2~4 계획에 등장한 용어만 정의하고, 각 항목에 일반 정의·프로젝트 예시·주의점을 연결했다. ADR 0004에서 semantic versioning과 지속 갱신 규칙을 확정했다.

구현 commit `7ac3c5d`를 `origin/work/game-dev-glossary`에 push하고 local/remote SHA 일치를 확인했다.

## Day 2 최소 Enemy FSM 기록

기존 `EnemyController`의 추적·접촉 공격 흐름을 보존하면서 `Idle`, `Chase`, `Attack`, `Dead`를 `enum` 기반 `EnemyStateMachine`으로 명시했다. 상태별 interface나 class 계층은 만들지 않았으며, 사망은 되돌릴 수 없는 종료 상태로 처리한다. 적 색상과 Hierarchy 이름은 상태가 실제로 바뀔 때만 갱신한다.

exact Editor 6000.5.1f1에서 runtime/test assembly compilation과 EditMode 테스트 9건이 통과했다. 성공 종료 뒤 batch log에 Mono thread 정리와 debugger-agent 종료 진단이 남았지만 compiler error, exception, test failure 표식은 없었다. PlayMode 상태 표시와 변경 후 Console은 사람이 확인하기 전까지 미검증으로 둔다.

구현 commit `8539a8b`를 `origin/work/enemy-fsm`에 push하고 local/remote SHA 일치를 확인했다.

사용자가 Day 2 상태 표시를 포함한 수동 게임 흐름 PASS와 Unity Console 오류 없음을 확인했다. 확인 시점에 Unity Editor가 실행 중이므로 사용자 작업 보호를 위해 branch 전환과 `main` 통합은 Editor 종료 뒤로 보류했다.

수동 검증 근거를 commit `b294fbb`로 `origin/work/enemy-fsm`에 push했다. 이후 사용자가 Editor 종료를 알렸고, `Temp/UnityLockfile` 부재와 실행 중인 `Unity.exe` 없음도 확인했다.

## 필수 기술 baseline 확장 기록

사용자가 실제 협업·live-service 대응을 제외하고 Unity, Unreal Engine, Git, SVN, MySQL, network programming, socket programming, multithreading, OOP를 최종 project 하한으로 지정했다.

AI는 source를 수정하기 전에 기존 Git 상태, exact Unity 환경, Unreal Engine과 Visual Studio C++ toolchain, Windows .NET SDK, SVN, MySQL, Docker, repository 구현 여부를 읽기 전용으로 감사했다. Unity 6000.5.1f1, Unreal 5.8.0, Visual Studio Native Game/C++ component, .NET SDK 10.0.400, Git은 재사용 가능하다. SVN과 native MySQL은 발견되지 않았으며 Docker client 29.7.2는 있지만 daemon이 꺼져 기존 MySQL image는 확인하지 못했다.

AI는 ADR 0006에서 하나의 연결된 최소 구조를 선택했다. 계획된 구조는 Unity arena가 score를 C#/.NET TCP server에 제출하고 MySQL에 저장하며 Unreal C++ `ArenaObserver`가 같은 leaderboard를 읽는 것이다. 이 시점의 확장 설계이며 당시 구현 완료를 뜻하지 않는다. SVN은 Git source-of-truth와 섞지 않는 isolated workflow lab으로 제한한다.

이번 checkpoint에서 변경한 대상은 `AGENTS.md`, `PROCESS.md`, 구현 계획, 환경 감사, AI 기록, glossary, ADR 0006이다. runtime source, Unity Scene/Prefab, package manifest/lock, Project Settings는 변경하지 않았다.

사람이 승인하거나 확인해야 하는 항목:

- WSL `subversion` package 설치
- Docker Desktop 시작 후 기존 image 재감사와 필요 시 `mysql:8.4` download
- .NET MySQL connector 선택과 package 추가
- 이후 구현될 .NET server, MySQL, Unity network client, Unreal observer, SVN lab의 실행 결과

이번 checkpoint에서는 Unity, .NET, Unreal runtime test와 build를 다시 실행하지 않았다. 문서와 environment 사실만 갱신했으며 승인 없는 install, package 추가, image download, external script 실행은 하지 않았다.

AI 제안 중 채택하지 않은 부분:

- 필수 기술마다 무관한 demo project를 별도로 만들지 않았다.
- MySQL wire protocol을 직접 구현하지 않았다.
- Unreal에서 두 번째 arena game을 만들지 않고 read-only observer로 범위를 제한했다.
- asynchronous I/O만으로 multithreading 경험을 주장하지 않도록 별도 concurrent test gate를 두었다.

baseline 문서 구현 commit `01cd869`를 당시 원격 작업 branch에 push하고 local/remote SHA 일치를 확인했다. Markdown link, ADR 0001~0006 sequence와 section, glossary 0.3.0의 43개 entry shape, 변경 경계 검사는 PASS다. runtime test는 documentation-only checkpoint이므로 실행하지 않았다.

## Day 3 측정·Editor Tool 기록

AI는 기존 gameplay에 neighbor query가 없음을 확인하고 `SpatialHash2D<T>`를 correctness와 비용 비교용 독립 자료구조로 구현했다. runtime enemy flow에는 연결하지 않았고 object pooling도 추가하지 않았다.

생성·수정 대상:

- `Assets/ArenaSystemsLab/Runtime/SpatialHash2D.cs`
- `Assets/ArenaSystemsLab/Editor/ArenaProjectValidator.cs`와 Editor assembly
- EditMode spatial hash·validator tests와 PlayMode profile test
- `docs/PERFORMANCE_BASELINE.md`
- ADR 0007, glossary 0.4.0, 환경·AI·process 기록

exact Editor에서 EditMode 16건과 final PlayMode 1건이 통과했고 validator command-line entry도 PASS였다. 알고리즘 실험은 20,000 point, 500 query에서 두 방식 모두 6,882 matches를 반환했으며 spatial hash 1.381 ms, brute-force 73.824 ms를 기록했다.

PlayMode profile은 target 120 FPS, 1초 warm-up, 5초 sampling으로 601 sample을 수집했다. Main Thread 평균/최대는 8.317/13.756 ms, frame GC allocation 평균/최대는 211,689/273,180 bytes, GameObject 평균/최대는 11.3/14였다. Editor와 Test Runner overhead가 포함되므로 최적화 성과나 player build 성능으로 표현하지 않는다.

실패와 수정:

- 최초 PlayMode test는 marker recorder option 누락으로 FAIL했다. `SumAllSamplesInFrame`을 추가했다.
- 짧은 frame-count sampling은 0.055초라 gameplay 기준선에서 제외했다.
- `Reset()` 후 recorder를 다시 시작하지 않은 시도는 sample 0으로 FAIL했다. local Unity API 문서를 확인하고 `Start()`를 추가했다.
- Unity가 자동 생성한 미추적 `ProjectSettings/SceneTemplateSettings.json`은 승인 범위 밖이라 검사 후 제거했다.

Day 3 Editor menu와 기존 gameplay 회귀, Unity Console checklist는 사용자가 완료했고 PASS로 기록했다. package 설치, Scene/Prefab 수정, tracked ProjectSettings 변경은 수행하지 않았다.

Day 3 구현 commit `86e90f4`를 `origin/work/day3-profiling-validation`에 push하고 local/remote SHA 일치를 확인했다. 전체 `git diff --check`는 Unity가 생성한 새 `.meta`의 기존 형식과 같은 빈 YAML field 후행 공백을 보고했으며, `.meta`를 제외한 staged diff check는 PASS였다. 생성된 metadata를 임의로 재작성하지 않았다.

사용자가 Day 3 Editor menu, 기존 gameplay 회귀, Unity Console checklist 완료를 확인했다. 이 사람 검증 근거를 별도 commit으로 remote branch에 push한 뒤 Unity process와 lock 부재를 확인하고 Day 3 branch를 `main`에 통합했다.

## Day 4 Windows build·README·demo 기록

AI가 생성하거나 수정한 대상:

- `Assets/ArenaSystemsLab/Editor/ArenaWindowsBuilder.cs`
- `README.md`
- `docs/DEMO_GUIDE.md`
- `docs/adr/0008-reproducible-windows-build-and-demo.md`
- `AGENTS.md`, `PROCESS.md`, 환경 감사, AI 기록, glossary, 구현 계획

`ArenaWindowsBuilder`는 기존 validator, Build Settings Scene과 Standalone Mono backend를 재사용한다. project 설정을 자동 전환하지 않고 Windows x86-64 Development player를 ignored `Builds/Windows` 아래에 생성한다. release/IL2CPP build, installer, archive upload는 추가하지 않았다.

실행 결과:

- EditMode regression: PASS, 16/16
- PlayMode regression/profile: PASS, 1/1
- Project validator CLI: PASS
- Windows Mono Development build: PASS, 64.697초
- Player artifact: PASS, PE32+ GUI x86-64, local output 약 166 MB
- Player launch smoke: PASS, 8초 생존 후 target process 종료
- Player full gameplay: PASS, 사용자 확인 및 오류 없음

첫 test invocation은 확인하지 않은 기본 Editor 경로를 사용해 executable을 찾지 못했고 test가 실행되지 않았다. 기존 Day 3 log와 제한된 일반 설치 경로 탐색으로 실제 설치를 확인한 뒤 exact Editor를 재사용했다.

build 중 Unity/URP가 4개 tracked asset/settings와 미추적 `SceneTemplateSettings.json`을 자동 직렬화했다. 직전 clean Git 상태와 diff로 이번 실행의 부작용임을 확인하고 해당 값만 원래 내용으로 복원했다. 최종 Scene, Prefab, Package, tracked ProjectSettings 변경은 없다.

Player log에는 D3D12 info queue 경고와 종료 시 Unity cleanup 진단이 있었지만 managed exception, crash, 조기 종료는 없었다. 이 smoke 결과를 전체 gameplay PASS로 확대하지 않고 standalone 수동 checklist를 `docs/DEMO_GUIDE.md`에 남겼다.

Day 4 구현 commit `fca8a38`을 `origin/work/day4-build-demo`에 push하고 local/remote SHA 일치를 확인했다. 당시에는 Windows player 전체 gameplay와 build Editor menu가 사람 검증 전이므로 branch를 `main`에 통합하지 않았다.

이후 사용자가 Windows player standalone 8단계 checklist PASS와 오류 없음을 확인했다. 확인 시점에 player process가 아직 실행 중이므로 사람 검증 기록은 남기되 강제 종료나 branch 통합은 수행하지 않는다.

사람 검증 commit `f87a0d5`를 remote branch와 대조한 뒤 generated player 창에 정상 종료 신호를 보냈고 process가 즉시 종료됐다. Unity Editor, player, crash handler와 project lock이 모두 없는 상태에서 Day 4 통합을 재개한다.

## Milestone 5 네트워크 보안 foundation 기록

실행일: `2026-09-05`

### AI가 수행한 조사

- Day 4 사람 검증 근거와 `main` 통합 상태, Unity/player process 종료 상태를 확인했다.
- TCP message boundary, UTF-8 JSON과 parser limit, loopback bind, `ReadExactlyAsync`, input validation, resource exhaustion, 안전한 logging, async I/O와 multithreading 차이, 향후 TLS 설정을 공식 RFC·Microsoft·OWASP·CWE 자료와 대조했다.
- “한 send는 한 read다”, “loopback은 인증이다”, “공식 client input은 신뢰 가능하다”, “async I/O 자체가 multithreading 증거다”라는 가정을 모두 기각했다.
- 기존 repository에 network/socket/thread source가 없고 Windows .NET SDK 10.0.400을 재사용할 수 있음을 확인했다.

### AI가 생성하거나 수정한 파일

- `Server/ArenaSystemsLab.Server/`: BCL-only protocol, loopback server, bounded thread-safe in-memory store
- `Server/ArenaSystemsLab.Server.Verification/`: framework 없는 실행형 검증 8건
- `Server/NuGet.Config`: restore package source clear
- `docs/NETWORK_SECURITY.md`
- `docs/adr/0009-loopback-first-bounded-tcp-protocol.md`
- `.gitignore`, `AGENTS.md`, `README.md`, 구현 계획, 환경 감사, AI 기록, glossary

### 사람이 확인해야 할 항목

- 보안 문서의 위협 모델과 로컬 전용 한계가 실제 구현과 일치하는지 검토한다.
- Unity client 연결 이후 실제 Game Over score 제출·조회와 Console을 수동 검증한다.
- LAN/public bind가 필요해지면 TLS, authentication, abuse control과 server-authoritative score 설계를 먼저 승인한다.

### 실행된 테스트

- Offline restore with cleared NuGet sources: PASS
- Release build: PASS, warnings 0 / errors 0
- Fragmented frame: PASS
- Zero/oversized frame length: PASS
- Strict request validation: PASS
- Bounded player store: PASS
- Actual 8-thread shared store: PASS
- Loopback health: PASS
- Slow-client timeout: PASS
- 24 concurrent score clients and leaderboard: PASS
- 최종 verification: 8 passed / 0 failed
- Actual server CLI + Windows PowerShell health client: PASS, graceful shutdown

### 미검증 항목

- Unity와 Unreal client end-to-end flow
- MySQL persistence와 restart recovery
- TLS, authentication, authorization, replay/rate limiting
- client 제출 score의 gameplay 정당성
- 부하 한계, 장시간 soak, 외부 penetration test
- 첫 .NET restore의 실제 telemetry 전송 여부

### AI 제안을 그대로 채택하지 않은 부분

- loopback foundation에 TLS·account system·token을 미리 만들지 않았다. remote exposure 전까지 bind 자체를 금지하는 gate로 남겼다.
- xUnit/NUnit 등 test package를 추가하지 않고 작은 verification executable을 사용했다.
- asynchronous socket I/O를 multithreading 증거로 포장하지 않고 실제 `Thread` 8개 검사를 분리했다.
- 측정 전 cache, worker pool, per-player lock, database abstraction을 추가하지 않았다.
- client가 제출한 score를 안전하다고 주장하지 않았다.

첫 `dotnet restore`에서 Windows .NET CLI가 ASP.NET Core HTTPS development certificate를 자동 생성했다고 보고했다. 이 인증서는 server에서 사용하지 않으며 repository 밖 certificate store를 승인 없이 수정하지 않았다. 이후 .NET 명령에는 process 범위 `DOTNET_CLI_TELEMETRY_OPTOUT=1`을 적용했다. `DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1`도 설정했지만 .NET Core 3.0 이후 지원되지 않으므로 부작용 방지 근거로 사용하지 않는다. 첫 실행 전에 인증서 생성을 막아야 한다면 `DOTNET_GENERATE_ASPNET_CERTIFICATE=false`를 사용해야 한다.

실제 server CLI smoke에서 WSL Python client의 `127.0.0.1` 연결은 `ConnectionRefused`로 실패했다. 이후 확인한 WSL networking mode는 `nat`이고 Microsoft 문서의 기본 NAT 동작과 일치하지만 당시 mode가 변경되지 않았다는 기록은 없어 정확한 역사적 원인은 추론으로 남긴다. Windows process와 같은 host의 PowerShell `TcpClient`로 재검사해 health response를 확인했으며 첫 결과는 실패 시도로 그대로 기록했다.

초기 verification 7/7 뒤에도 보안 경계를 다시 검토했고, 서로 다른 player를 계속 추가하면 dictionary가 무한히 커지는 누적 자원 위험을 발견했다. player 10,000개 상한과 검사를 추가해 final 8/8로 다시 검증했다.

첫 PowerShell inline client는 shell 사이 JSON quoting 오류로 parser 단계에서 실패해 request를 보내지 못했다. payload를 PowerShell `ConvertTo-Json`으로 생성하도록 고쳐 같은 Windows host에서 재실행했다.

구현·문서 commit `3909f6b`를 `origin/work/network-security-foundation`에 push하고 local/remote SHA 일치를 확인했다. 다음 통합 여부와 재개 지점은 `PROCESS.md`의 `CP-20260905-01`에서 관리한다.

자동 검증과 실제 CLI smoke, 변경 경계 검사를 근거로 branch를 merge commit `bb3a24e`에서 `main`에 통합하고 `origin/main`에 push했다. Unity runtime, Scene, Prefab, Package, ProjectSettings 변경은 없다.

## Milestone 7 Unity network client 기록

실행일: `2026-09-05`

### AI가 생성하거나 수정한 파일

- `Assets/ArenaSystemsLab/Runtime/LeaderboardClient.cs`: loopback TCP client, bounded frame·response validation, cancellation과 1회 retry
- `Assets/ArenaSystemsLab/Runtime/ArenaGame.cs`: Game Over submit/query, leaderboard IMGUI와 사망 이후 score 고정
- `Assets/ArenaSystemsLab/Tests/EditMode/LeaderboardClientTests.cs`: 정상 protocol, retry, oversized response, server-unavailable 검사
- `docs/adr/0010-unity-loopback-leaderboard-client.md`
- README, demo guide, 환경 감사, AI 기록, glossary, AGENTS 작업 지침

### 실행된 검증

- Exact Editor runtime/test compilation: PASS, final compiler error 0
- EditMode regression: PASS, 20 passed / 0 failed / 0 skipped
- PlayMode regression/profile: PASS, 1 passed / 0 failed / 0 skipped
- Project validator command line: PASS
- Package·Scene·Prefab·tracked ProjectSettings: UNCHANGED
- 실제 .NET server와 Game Over 사람 검증: 자동 checkpoint에서는 NOT RUN, 이후 사람 검증 결과는 아래에 기록

### 실패와 수정

- 첫 compile은 Unity API profile의 `TcpListener`가 `IDisposable`이 아니어서 FAIL했다. test listener를 `finally`의 `Stop()`으로 종료하도록 수정했다.
- 첫 network test는 Unity main-thread synchronization context를 동기 대기해 정지했다. AI가 시작한 batch Editor와 worker PID만 종료하고 test body를 `Task.Run`에서 시작하도록 수정했다.
- 다음 async test 형태도 같은 실행기에서 정지해 동일하게 시작한 PID만 정리했다. 두 중단 시도는 PASS 근거에서 제외했다.
- server-unavailable 첫 검사는 `connection_failed`만 기대했지만 Windows Unity에서는 두 번의 bounded connect가 `request_timeout`으로 끝나 19/20 FAIL했다. 두 결과 모두 같은 unavailable 계약이므로 환경에 따른 두 고정 code를 검증하도록 수정했고 final 20/20이 통과했다.
- PlayMode 실행이 미추적 `ProjectSettings/SceneTemplateSettings.json`을 생성했다. 해당 실행이 만든 단일 부작용임을 확인하고 제거했으며 tracked 설정은 변경되지 않았다.

### 사람이 확인해야 할 항목

- server가 없을 때 Game Over에 unavailable이 표시되고 `R` restart가 유지되는지 확인한다.
- Windows .NET server를 실행한 뒤 Game Over score와 `UnityPlayer` leaderboard가 일치하는지 확인한다.
- Unity Console과 server log에 Error/Exception 또는 외부 payload 노출이 없는지 확인한다.

### 채택하지 않은 범위

- networking package, interface 하나뿐인 service layer, Scene·Prefab 설정을 추가하지 않았다.
- TLS, authentication, remote bind, account ID와 server-authoritative score를 local demo에 미리 구현하지 않았다.
- 매 frame polling을 만들지 않고 Game Over에서만 두 개의 bounded request를 보낸다.

구현·ADR·자동 검증 문서 commit `ac1f24a`를 `origin/work/unity-network-client`에 push하고 local/remote SHA 일치를 확인했다. 사람 검증 전이므로 `main` 통합은 수행하지 않았으며 현재 checkpoint는 `PROCESS.md`의 `CP-20260905-02`에서 관리한다.

사용자가 server 미실행 Game Over에서 unavailable 표시, `R` restart와 Console 오류 없음을 확인했다. 이후 AI가 기존 Release server executable을 loopback port 7777에서 시작했고 사용자가 실제 Game Over score submit/query와 Console을 검증했다. 첫 run 3점, 두 번째 run 11점 뒤 leaderboard에는 `UnityPlayer 11` 한 항목이 표시됐다.

이 결과는 `LeaderboardStore`가 player별 최고 score를 보존하는 현재 계약과 일치한다. 사용자가 과거 3점 run이 보이지 않는 이유를 질문해 leaderboard와 run history를 구분했고, MySQL milestone에서 모든 run을 별도로 저장하되 retry 중복을 막는 idempotency key를 먼저 결정하도록 구현 계획과 glossary를 보완했다. 현재 protocol과 in-memory store는 변경하지 않았다.

AI가 시작한 server PID `44368`은 검증 뒤 해당 process만 종료했고 port 7777이 비어 있음을 확인했다. stdout은 loopback listening 한 줄, stderr는 0 byte였으며 이 종료는 `Ctrl+C` graceful shutdown 검사가 아니라 target process 정리다. 서버 시작 전 첫 PowerShell wrapper는 Bash가 `$` 변수를 먼저 확장해 parser error로 실패했고 Unity나 server process를 생성하지 않았다. 변수 확장을 차단해 재실행한 뒤 exact Editor와 server를 정상 시작했다.

사람 검증과 leaderboard/run-history 구분을 commit `02e472a`로 `origin/work/unity-network-client`에 push했다. 사용자가 Unity Editor 종료를 확인했고 AI도 Unity process, `Temp/UnityLockfile`, port 7777 부재와 clean remote branch를 대조했다. branch를 merge commit `b0a3f8e`에서 `main`에 통합하고 같은 SHA를 `origin/main`에 push했다.

## Milestone 8 Unreal observer 기록

실행일: `2026-09-05`

### AI가 수행한 조사

- Epic Launcher 설치 기록과 `Build.version`에서 exact Unreal Engine 5.8.0 / CL 55116800을 재확인했다.
- `vswhere`로 Visual Studio Native Game/C++, MSVC x64와 Windows SDK component를 확인했다.
- 설치 Engine의 `FSocket`, `ISocketSubsystem`, `FJsonSerializer`, automation command와 blank C++ template source를 직접 읽어 5.8 API를 확인했다.
- Git clean main handoff, Unreal/Unity process 부재와 repository 내 기존 `.uproject` 부재를 확인한 뒤 work branch를 만들었다.

### AI가 생성하거나 수정한 파일

- `Unreal/ArenaObserver`: C++ project descriptor, 최소 config, module/target, native leaderboard client, Game Mode, HUD와 automation test
- `.gitignore`: Unreal `Binaries`, `DerivedDataCache`, `Intermediate`, `Saved` 제외
- `docs/adr/0011-unreal-read-only-leaderboard-observer.md`
- README, demo guide, 구현 계획, 환경 감사, AI 기록과 glossary

### 실행된 검증

- Development Editor C++ build: PASS
- Unreal Automation protocol test: PASS, 1 passed / 0 failed / 0 warnings
- Native client server-unavailable: PASS, port 7777 부재에서 3초 bounded failure
- Native client actual .NET server query: PASS, test 1 passed / 0 failed / 0 warnings
- 기존 server `Ctrl+C` 종료와 port 해제: PASS
- request frame, 정상 response, 정렬·정수·ID·중복·크기 경계: PASS
- generated directory ignore와 Unity source/settings 비변경 검사: PASS
- server-unavailable HUD: PASS, 사용자가 `Connection I/O error` unavailable 상태를 확인
- actual-server HUD: PASS, 사용자가 `ObserverFixture 42`와 연결 상태를 확인
- Unreal project error 확인: PASS, 실행 log에 `ArenaObserver` Error/Fatal/ensure 없음

### 실패와 수정

- WSL에서 `Build.bat`를 직접 실행한 시도와 잘못 quoting한 `cmd.exe` 시도는 build가 시작되지 않아 FAIL로 분류했다. Windows PowerShell에서 batch를 호출해 해결했다.
- 첫 수동 GUI 실행은 공백이 있는 `.uproject` 인자가 보존되지 않아 project browser만 열렸다. process command line과 새 log 부재로 발견해 해당 창만 정상 종료하고, 전체 project 경로를 인용해 exact Editor에서 다시 열었다.
- 첫 compile은 중첩 test source가 module root header를 찾지 못해 FAIL했다. test source를 module root로 옮겼다.
- source 이동 직후 incremental build는 이전 경로를 가진 UBT makefile cache 때문에 FAIL했다. `-NoUBTMakefiles`로 source를 재수집한 뒤 final build가 통과했다.
- 첫 Editor automation 실행이 Android File Server 설정과 token, 91줄 input config를 자동 생성했다. token section을 제거하고 해당 plugin 및 Fab/Bridge를 project에서 비활성화했으며, input config는 자동 rewrite를 막는 3줄만 유지했다. 재실행에서 token 재생성 부재와 Fab/Bridge 비활성화를 확인했다. EOS 전체 비활성화까지 확인한 것으로 해석하지 않는다.
- automation 시작 시 Engine 5.8 자체 `UnifiedErrorTests.cpp`가 의도적으로 생성하는 `LogAutomationTest: Error: Condition failed` 15줄이 있다. project test report는 errors 0이고 해당 source 위치를 local Engine에서 대조했으므로 project 오류로 재분류하지 않았다.

### 사람이 확인한 항목

- server가 없을 때 `Connection I/O error`가 unavailable 상태로 표시되고 Editor가 응답함을 확인했다.
- local server에 `ObserverFixture 42`가 있는 상태에서 연결 문구와 Top 5 row가 표시됨을 확인했다.
- AI가 실행 log에서 `ArenaObserver` Error/Fatal/ensure가 없음을 재확인했고 사용자가 화면 검증을 PASS로 판정했다.

### AI 제안을 그대로 채택하지 않은 부분

- 두 번째 gameplay, UMG asset, Blueprint, 외부 plugin과 marketplace asset을 만들지 않았다.
- endpoint configuration, periodic refresh, submit, retry, interface/factory를 미래 확장용으로 추가하지 않았다.
- Engine thread pool 사용을 별도의 multithreading 성능 증거로 주장하지 않았다.

구현·ADR·자동 검증 commit `20b0d55`를 `origin/work/unreal-arena-observer`에 push하고 local/remote SHA 일치를 확인했다. 이후 exact Unreal 5.8 Editor에서 사람이 두 HUD 경로를 검증했다. actual-server 화면의 data는 Unity가 새 session에서 제출한 것으로 가장하지 않고, 고정 protocol로 주입한 ephemeral `ObserverFixture 42`로 기록한다. Unity actual-server submit/query 사람 검증은 Milestone 7에 별도 근거가 있다.

사람 검증 근거 commit `724a094`를 remote work branch와 대조한 뒤 merge commit `b57db33`으로 `main`에 통합하고 push했다. Editor와 server는 정상 종료됐고 port 7777 listener와 관련 process가 남지 않았으며, 다음 MySQL·SVN 단계는 설치·download 승인을 받기 전 시작하지 않는다.

## 2026-09-07 기술 문서 5종 정리

### 조사와 사실 확인

`work/technical-documentation`에서 `fa834cf`의 Unity 게임·테스트·Editor 도구, .NET 프로토콜·저장소, Unreal 클라이언트·HUD·설정을 대조했다. 시작 Git 상태는 `main...origin/main`, 미커밋 변경 없음이었다.

`factchk`에 따라 TCP·비동기 I/O·JSON 숫자 변환·누락 필드·논리 ERD·Markdown 다이어그램의 의미를 RFC, Microsoft, Unity, Mermaid, GitHub 공식 자료와 대조했다. [ADR 0012](adr/0012-technical-documentation-governance.md)의 판정표에 수정한 설명과 보류한 코드 문제를 분리했다.

### 생성·변경 범위

- 새 기술 명세: `docs/REQUIREMENTS.md`, `docs/ARCHITECTURE.md`, `docs/DATA_MODEL.md`
- 재사용한 기술 명세: `docs/NETWORK_SECURITY.md`, `docs/DEMO_GUIDE.md`
- 진입점·운영 규칙: README, AGENTS, PROCESS, 구현 계획
- 기존 기록 정정: 환경 감사, AI 기록, 성능 기준선, ADR 0003·0006·0008·0011
- 새 결정: ADR 0012. ADR 0006은 결정 번호·기술 근거를 유지하며 파일명과 링크만 정리했다.
- 용어 문서: `0.10.0`, ERD·schema·idempotency 추가, 78개. FSM 예시를 실제 물리 접촉 조건으로 정정했다.

현재 문서의 기술과 무관한 외부 목적, 개인 계정·식별 URL·장치 경로를 제거하거나 명시적 자리표시자로 치환했다. Git 과거 이력·작성자·remote 주소는 건드리지 않았다. 문서 정리는 저장소 이력 전체의 정보 제거를 뜻하지 않는다.

### 실행한 검증과 실패

- Markdown 내부 파일 링크·JSON 예제·fence 짝·ADR 연속 번호/필수 section·glossary 구조·문서 버전 검사: PASS.
- 현재 Markdown의 비기술적 목적·개인 식별 패턴 검사: 일치 0건. 패턴 검사와 내용 검토 범위이며 이력 전체의 무정보성을 보증하지 않는다.
- 기존 Unity XML: EditMode 20/20, PlayMode 1/1 PASS를 읽기 전용 재확인했다. 이번에 테스트를 다시 실행한 것은 아니다.
- 기존 Unreal automation JSON 3개: 각각 1 passed / 0 failed / 0 warnings를 재확인했다.
- 기존 Day 4 Windows 빌드 로그: 성공 표식 재확인. 네트워크 기능을 포함한 새 빌드 결과가 아니다.
- 첫 Unreal JSON 집계는 BOM 처리 누락으로 실패했다. 읽은 문자열에서 BOM을 제외해 재검사했고 파일 자체는 변경하지 않았다.
- 한 문서 patch는 예상 문맥이 맞지 않아 적용되지 않았다. 파일 상태를 재확인하고 실제 문맥으로 후속 편집했다.
- Unity/.NET/Unreal compile·자동/수동 runtime 검사: NOT RUN, 문서 전용 변경.
- Mermaid 최종 웹 렌더링: NOT RUN. 새 렌더러나 패키지를 설치하지 않았다.

### 사람이 확인할 항목과 채택하지 않은 변경

README에서 5대 문서·소스 링크·도식을 읽고 설명과 실제 조작이 일치하는지 확인한다. 같은 서버 세션의 Unity 제출 → Unreal 조회 시연과 현재 Windows 재빌드는 별도 검증이다.

MySQL 구현을 가정한 물리 ERD, 공간 해시의 gameplay 적용, 전역 3초 네트워크 보장, 완전한 JSON 검증·부하 안전성 주장은 채택하지 않았다. 숫자 ValueKind 오류 분류와 Unity 응답 누락 필드·중복 ID 문제는 기록했지만 코드 수정으로 범위를 넓히지 않았다. 성능 비교 테스트가 총 match 수만 비교하는 한계도 명시했다.

구현 commit `8cabddd`를 `origin/work/technical-documentation`에 push하고 SHA 일치를 확인했다. 문서 25개, 내부 링크 누락 0, JSON 예제 7개, ADR 12개, glossary 78개와 정보 경계 검사 PASS이며 runtime·엔진 설정 변경은 없다.

Checkpoint commit `15a01b3`을 원격 작업 branch에 기록했다. GitHub 소개에 남은 비기술적 목적 설명도 현재 Unity·.NET·Unreal 구성의 기술 요약으로 변경하고 재조회했다. 이어 merge `77435fc`으로 main에 통합·push하고 원격 SHA 일치를 확인했다. 공개 범위·homepage·계정은 변경하지 않았다.

## 2026-09-09 기술 완결성 설계

### 조사와 선택

f896940의 clean main에서 시작해 AGENTS·PROCESS·관련 ADR·명세와 코드·테스트를 읽었다. factchk로 JSON 숫자 예외·Unity 필드 초기값·MySQL transaction/DDL·connector 사용 규칙·배포 명세를 확인했다. 출처는 [설계서](TECHNICAL_COMPLETION_DESIGN.md)에 기록했다. 사용자는 MySQL 단일 모드와 전체 v2 전환을 선택했다. 설치 승인은 별도로 남겼다.

### 변경 파일과 확인할 항목

설계서·ADR 0013을 생성하고 README·AGENTS·PROCESS·구현 계획·glossary·감사·AI 기록을 연결했다. glossary는 0.11.0, 84개다. 사람이 확인할 항목은 저장/재시도 계약·승인 범위·최종 수동 시연이며 v2·DB·SVN 구현을 완료로 기록하지 않는다.

### 실행한 검사와 부작용

- 변경 전 exact Unity 6000.5.1f1 EditMode: PASS, 20/20. 결과는 ignored Logs/CompletionBaseline-20260909.xml이다.
- 변경 전 .NET 10.0.401 Release build: PASS, 경고·오류 0. 기존 server verification: PASS, 8/8. restore·package 설치 명령은 실행하지 않았다.
- Hub 목록에는 다른 Editor만 있었으나 기존 로그가 가리킨 설치에서 exact Editor를 확인했다. Hub 목록만으로 미설치라고 단정하지 않았다. Editor process와 project lock은 없었다.
- 첫 process 조회의 PowerShell 인용이 잘못되어 CIM query가 실패했다. Get-Process와 명시적 경로 조회로 재확인했다.
- SDK 첫 build가 ASP.NET Core HTTPS 개발 인증서를 자동 생성했다고 출력했다. AI가 별도로 요청한 설치가 아니며 인증서 저장소 접근·신뢰·제거는 하지 않았다. 이후 명령에서는 해당 첫 실행 생성을 비활성화한다.
- 첫 문서 patch는 AGENTS 문맥 불일치로 전체 미적용이었다. Git·파일 부재를 확인한 뒤 분리 적용했다. 중단 뒤 설계서 한 파일만 저장된 상태를 확인하고 이어서 작업했다.
- 새로운 dependency, engine upgrade, Scene/Prefab/ProjectSettings 변경, v2 코드와 SQL 생성은 이 설계 checkpoint에 포함하지 않는다.
- 문서 검사와 commit·remote 근거는 PROCESS checkpoint에서 관리한다.

### 채택하지 않은 AI 제안

콘텐츠 확장·새 프레임워크·memory fallback·v1 호환 계층·offline queue를 추가하지 않았다. 일반 JSON schema 완전 검증, 벽시계 timeout 보장, 측정하지 않은 성능 개선을 주장하지 않았다.

## 2026-09-09 v1 경계와 회귀 검사 보강

### 조사·변경 범위

설계 branch의 commit 3afa91e와 checkpoint 5b29e49를 push하고 remote SHA를 대조한 뒤 `work/network-contract-hardening`으로 분기했다. 공통 정수 파서의 세 호출자, server dispatch·store, Unity submit/query·ArenaGame 수신 경로와 기존 mock, SpatialHash2D query를 읽었다.

- runtime 수정: `WireProtocol.cs`의 Number guard, `LeaderboardClient.cs`의 누락 점수 초기값과 Ordinal HashSet. protocol v1·메모리 저장·gameplay·Unreal은 유지했다.
- 회귀 검사: 기존 server verification, LeaderboardClientTests, SpatialHash2DTests만 확장했다. 새 test framework·추상화·package는 없다.
- 문서: ADR 0014 생성, 통신·실행·요구사항의 현재 한계 정정, 성능 기준선의 과거 검사 범위와 새 검사 분리, PROCESS·감사·AI 기록 갱신. glossary 0.12.0에 Hash Set을 추가했다.

### 실행·실패·통과

- 테스트를 먼저 추가한 수정 전 실행: .NET build PASS, server 8 PASS / 2 FAIL. 문자열 숫자에서 일반 예외와 internal_error를 관측했다.
- 수정 전 exact Unity EditMode: 26 PASS / 4 FAIL. 누락 bestScore가 즉시 invalid_response로 거부되지 않고 후속 요청 timeout으로 이어졌고, 누락 entry score와 두 중복 ID 응답은 수용됐다.
- 최소 수정 후: .NET Release warnings/errors 0, server 10/10 PASS. exact Unity EditMode 30/30, PlayMode 1/1, validator PASS. API 설명만 믿지 않고 Unity의 실제 초기값 처리와 정상 0 수용을 확인했다.
- Unity 배치 로그에 미변경 ArenaGame의 CS0618 경고가 출력됐다. 라이선스 갱신 진단은 변경 전 baseline에도 있으며 validator의 외부 설정 요청도 실패했다. 컴파일 실패나 테스트 실패로 확대 해석하지 않았고 로그 전체 무오류로 숨기지도 않았다.
- PlayMode 검사 중 새로 생성된 기본 `ProjectSettings/SceneTemplateSettings.json`을 읽고 모든 userAdded 값이 false임을 확인했다. Editor와 lock이 없는 상태에서 이 생성 부작용만 제거했다. 같은 검사에서 재생성 가능한 기본 파일이며 기존 사용자 파일 삭제가 아니다.
- XML 조회 뒤 process 없음으로 PowerShell 마지막 명령이 exit 1을 반환한 진단 호출이 있었다. XML은 정상 집계됐으며 후속 명시적 process/lock 검사에서 없음으로 확인했다.
- .NET 명령에는 인증서 최초 실행 생성을 억제하는 process 환경변수를 사용했다. restore·설치·신뢰·인증서 저장소 접근은 실행하지 않았다.

### 사람 확인·미검증·채택하지 않은 변경

정상 0점 제출·높은 점수 갱신·server 없음·R 재시작·Unity Console을 이번 branch에서 사람이 확인해야 한다. Windows 최신 build·Unreal 재빌드·동일 server 연속 시연·MySQL·v2·SVN은 미실행이다. 과거 사람 PASS를 새 결과로 옮기지 않는다.

새 JSON library, 모든 schema 검사를 수행하는 parser, object pooling·gameplay spatial hash, 임의의 새 성능 기준선을 추가하지 않았다. MySQL·SVN 설치와 외부 작업 공간은 별도 승인이며 main 통합도 보류했다. commit·원격 일치 근거는 PROCESS checkpoint에 둔다.

구현 commit c6fec8c를 작업 branch에 push하고 local/remote SHA 일치를 확인했다. main 원격 ref는 f896940으로 유지됐다. 후속 CP-20260909-02는 이 구현 SHA와 자동 검사·사람 검증 대기를 기록한다.

## 2026-09-09 sip 자체 점검

### 범위와 독립 검토

직전 기술 완결성 설계와 v1 보강 코드·검사·기록을 `sip`으로 검토했다. Git 호출·commit·push는 하지 않았고 이전 SHA의 현재 일치를 주장하지 않는다. `shower` 지침에 따라 별도 검토자에게 설계서 본문 전체만 제공했으며, 저장소·대화 배경·외부 탐색 없이 처음부터 끝까지 읽은 판정은 `minor gaps`였다.

`factchk`는 외부 명세를 대조하고 `mandela`는 검사 독립성을 읽기 전용으로 감사했다. `ssotize`는 중복 위치와 통합안만 보고했다. `re0`는 기존 문구를 정리했다. 도구별 실행 안내·설계·작업 기록은 도구 중립 이식성을 약속하지 않으므로 `detool` 변환은 생략했다.

### 사실 확인

| 주장 | 판정과 독립 근거 |
|---|---|
| TryGetInt32의 비숫자 타입 처리 | API 예외는 사실. Number guard로 invalid_request를 반환하는 서버 동작과 구분해 설명 수정. [Microsoft](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonelement.trygetint32?view=net-10.0) |
| Unity 누락 필드의 초기값 유지 | 명세와 일치. exact Editor의 기존 실패/통과 XML도 대조했으며 재실행은 아님. [Unity](https://docs.unity3d.com/6000.0/Documentation/ScriptReference/JsonUtility.FromJson.html) |
| MySQL transaction 잠금·DDL 복구 | 명시적 transaction의 관리 행 잠금과 일반 ROLLBACK으로 DDL을 되돌릴 수 없다는 경계 확인. [잠금](https://dev.mysql.com/doc/refman/8.4/en/innodb-locking-reads.html), [암묵적 commit](https://dev.mysql.com/doc/refman/8.4/en/implicit-commit.html) |
| Connector 연결 공유·취소 | 동시 연결 사용 금지와 CommandTimeout이 전체 벽시계 상한이 아니라는 설명 확인. 프로젝트의 2초 적용 범위는 외부 명세로 대신 결정할 수 없음. [연결](https://mysqlconnector.net/troubleshooting/connection-reuse/), [취소](https://mysqlconnector.net/overview/command-cancellation/) |
| MySqlConnector 2.6.2 후보 | MIT·net10.0 의존성 두 개의 이름·최소 버전과 일치. 로컬 설치·해석 결과 검증은 아님. [NuGet 배포 명세](https://www.nuget.org/packages/MySqlConnector/2.6.2) |
| MySQL image·SVN binary 후보 | mysql:8.4.11 태그 존재 확인. Apache가 연결된 binary를 유지·보증한다는 뜻은 아님. 다운로드·실행·보안 인증은 하지 않음. [공식 image 목록](https://raw.githubusercontent.com/docker-library/official-images/master/library/mysql), [Apache 안내](https://subversion.apache.org/packages.html) |
| SDK 환경변수 두 개의 역할 | 인증서 생성 억제와 사용 정보 전송 중단은 서로 다른 역할. 실행 안내에서 분리 설명. [Microsoft](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-environment-variables) |

### 검사 독립성 감사

대상은 서버 parser·Unity client·SpatialHash2D이며, 판정자는 verification executable과 NUnit assertion, 설계자는 구현 AI다. 데이터는 공개된 literal JSON, 고정 seed 좌표와 수기로 기대 ID를 정한 경계 사례다. 외부 API 명세·실제 엔진 XML·bucket 검색을 사용하지 않는 brute-force 경로를 근거로 8개 누출 유형을 점검했다.

`Tautology`의 국소적 중복을 확인했다. [Query](../Assets/ArenaSystemsLab/Runtime/SpatialHash2D.cs)는 results.Count를 반환하고 [테스트](../Assets/ArenaSystemsLab/Tests/EditMode/SpatialHash2DTests.cs)는 그 반환값을 다시 results.Count와 비교한다. 이 단언만으로 검색 정답을 증명할 수는 없다. 독립 기대값인 expectedResults.Count와 비교하도록 바꾸거나 중복 단언을 제거하는 것이 개선안이다. 다만 뒤의 query별 예상 ID·중복 개수 비교와 수기 경계 사례가 별도로 있으므로 기존 테스트 전체가 순환 검증이라는 뜻은 아니다.

감사 자체에도 shared hallucination·tautology·verifier=designer 여부를 재적용했다. 두 코드 위치와 공개 fixture·기존 XML에서 독자가 판정을 재현할 수 있게 했으며 새 독립 실험이나 보안 인증으로 표현하지 않았다. `mandela`는 읽기 전용이므로 테스트 변경과 엔진 재실행은 보류했다.

### SSOT 감사와 승인 대기 통합안

README·AGENTS·PROCESS·docs의 Markdown 28개를 정수 결과·version·protocol·dependency 문자열로 검색하고, 두 번째로 동의어를 포함한 문서별 열거와 원문 읽기로 범위를 대조했다. 현재 값과 날짜가 고정된 과거 결과 사이에 모순은 확인하지 못했다.

| 위치 | 분류·기준 문서 | 승인 후 제안 |
|---|---|---|
| PROCESS 검증표, IMPLEMENTATION_PLAN의 현재 상태·Matrix, DEMO의 최신 결과 수 | 부분 중복. 실제 작업 종료 때 갱신하는 PROCESS가 현재 상태 기준 | 구현 계획의 현재 상태 문장·열과 시연표의 최신 결과 수를 PROCESS 참조로 교체. 고유 완료 기준·실행 기대 형식은 유지 |
| README·DEMO의 날짜별 결과, ENVIRONMENT_AUDIT·AI_USAGE·ADR | 시점별 정확/부분 복사, 원본 실행의 역사적 근거 | 과거 실패·결과·도구 버전 보존. 최신 결과로 일괄 치환하지 않음 |
| 설계 §6, 구현 계획 Milestone 6 의존성, ADR 0013 | 승인 범위 요약·부분 중복. 정확한 package·영향·rollback은 설계 기준 | 계획의 반복 승인 설명을 설계 참조로 교체. ADR의 당시 결정은 보존 |
| NETWORK_SECURITY·DATA_MODEL, 기술 완결성 설계 | 현재 v1·미래 v2의 서로 다른 범위 | 모순이 아니므로 합치지 않음. 현재 source와 미래 설계 경계 유지 |

위 표와 변경 대상을 편집 전에 보고했으며 통합 승인은 아직 없다. 중복 제거·상태표 재구성은 실행하지 않았다. 이번 문구 정정과 링크 보완은 통합 작업이 아니다.

### 적용·보류와 검증

- 수정: 설계 0.1.1의 API 설명·구체적인 코드/Matrix 링크·간접 package 이름, 실행 가이드 1.1.1의 중복 제목·명령별 종료 확인·환경변수 설명, PROCESS와 이 기록. 새 파일·ADR·의존성·runtime 변경은 없다.
- 미결: v2 제출 성공 후 조회 실패의 반환·UI·retry, version 거부와 필수 속성 검사 우선순위, DB 2초 예산의 시작점·공유 범위. 새 계약을 임의 선택하지 않고 구현 전에 확정하도록 표시했다.
- 검토자에게 전달한 본문의 공백 지적 1건은 실제 파일에서는 이미 올바르므로 채택하지 않았다.
- 첫 메모리 snapshot 집계는 긴 출력이 잘려 JSON 해석에 실패했다. 파일 쓰기 없이 SHA-256 전용 집계로 바꿨다.
- compile·EditMode·PlayMode·수동 화면·Console·Windows/Unreal build: NOT RUN. 기존 XML의 수정 전 26/30, 수정 후 30/30·1/1과 validator 성공 표식만 읽기 전용 재확인했다.
- 문서 정적 검사: PASS. Markdown 28개·내부 링크 208개(앵커 4개 포함)·JSON 예제 9개·ADR 14개 연속 번호/필수 section·glossary 85개 구조·수정 문서 버전/이력·fence 짝·정보 경계 패턴을 검사했다. Mermaid 최종 웹 렌더링은 NOT RUN이다.
- 범위 대조: 수정 전후 111개 파일의 SHA-256을 비교해 위 문서 4개만 변경됐고 추가·삭제·비Markdown 변경은 없었다. 범위는 루트 문서·docs의 Markdown, Assets/ArenaSystemsLab, Server의 bin/obj 제외 파일, Packages, ProjectSettings다. 저장소 전체나 Git 상태를 검사한 것으로 확대하지 않는다.

## 2026-09-09 승인된 문서 SSOT 통합

### 승인·보존 범위

사용자가 앞선 ssotize 통합안에 적용을 승인했다. 시작 상태는 `work/network-contract-hardening`, local/remote 4194a4f 일치, 직전 sip 문서 4개만 미커밋이며 staged 변경은 없었다. 해당 diff를 대조한 뒤 `work/documentation-ssot`으로 분기해 기존 변경을 보존했다. main 통합·설치·v2 미결 설계 결정은 승인 범위에 포함하지 않는다.

### 적용 내용

- 구현 계획: 현재 상태 열을 PROCESS 참조로 바꾸고 9개 기술의 구현 위치·완료 근거를 유지했다. Git/OOP 완료 범위와 실제 8-thread 검증 정보는 PROCESS에서 보존했다. M5는 날짜별 구현 기록임을 제목에 표시했다.
- 의존성: M6 후보 버전·승인 설명은 기술 완결성 설계 §6, Docker·SVN 확인 상태는 PROCESS로 연결했다. 승인·rollback 내용 자체는 바꾸지 않았다.
- 실행 가이드: 1.1.2. 최신 테스트 개수는 PROCESS 참조로 바꾸고 명령·합격 조건·수동 절차·날짜별 결과는 유지했다.
- 운영 기록: PROCESS와 ADR 0003에 이번 적용을 기록했다. 기존 결정의 변경이 아니므로 새 ADR은 만들지 않았다. 직전 sip의 설계 0.1.1 변경도 보존해 함께 기록한다.

### 검증과 미검증

`rg` 검색과 동의어 기반 문서별 열거로 대상 위치를 재확인했다. 첫 patch는 긴 문장의 일부만 일치시켜 문맥 검사에서 거부됐고 파일은 변경되지 않았다. 실제 문장을 다시 읽은 뒤 정확한 문맥으로 적용했다.

엔진·runtime·자동/수동 게임 검사는 NOT RUN이다. 이번 변경은 문서 참조 통합이며 과거 PASS를 새 실행 결과로 옮기지 않는다.

- 문서 정적 검사: Markdown 28개·JSON 예제 9개·ADR 14개·glossary 85개 구조와 내부 링크·앵커·문서 버전 검사 PASS.
- 보존 검사 12항목 PASS: Matrix 9행의 구현 위치·완료 근거, M5와 DEMO의 날짜별 결과, 이전 AI·ADR 기록, 설계 승인 표를 대조했다. 제거한 최신 상태·후보 버전은 참조로 연결되고 Git/OOP·8-thread 범위가 PROCESS에 남아 있다.
- sip: 별도 문맥 없는 검토자가 계획·실행 가이드·PROCESS의 변경 관련 발췌 전체를 읽어 minor gaps로 판정했다. 전체 원문이나 링크를 검토한 것으로 확대하지 않는다. re0로 문서 작업의 마무리 순서와 v1 보강 변경의 사람 검증을 구분하고 일반 텍스트 참조를 링크로 정리했다.
- 이번 통합에는 새 외부 기술 주장·검사 설계가 없어 factchk·mandela 재감사는 생략했다. 기존 출처·검사 내용은 유지했다. detool은 도구별 실행·운영 기록이어서 변환하지 않았다. sip 중에는 Git을 호출하지 않았으며 완료 뒤 일반 commit·push workflow로 복귀한다.

구현 commit 3f71d69를 `origin/work/documentation-ssot`에 push하고 local/remote SHA 일치를 확인했다. 변경은 직전 sip 보완을 포함한 문서 6개이며 코드·package·설정·Scene·Prefab 변경은 없다. main은 f896940, 기존 v1 작업 branch는 4194a4f를 유지했다. 구현 시 내부 링크 220개·앵커 17개, staged diff 검사도 PASS였으며 후속 CP-20260909-03에 구현 SHA와 인계 상태를 기록한다.

## 2026-09-10 사실 확인과 문서 마감

### 조사와 변경 범위

사용자가 미구현 사유·최종 완료 조건·사람 검증 항목을 확인한 뒤 문서 정리와 당일 작업 종료를 요청했다. `factchk`로 외부 명세와 저장소 관측을 구분해 대조했다. 이번 요청을 신규 수동 PASS, 설치 승인, 미결 설계 선택 또는 main 통합 승인으로 해석하지 않았다.

분기 전 work/documentation-ssot 3ce607a와 원격 SHA 일치·clean을 확인하고 `work/session-closeout`으로 분기했다. 조사 근거는 [환경 감사](ENVIRONMENT_AUDIT.md#2026-09-10-미완료-사유와-환경-재확인)에 기록했다.

- `PROCESS.md`: Docker 현재 상태 정정, 미구현·미실행·미검증 구분, 최종 완료까지의 work queue와 다음 사람 검증 참조 정리.
- `docs/ENVIRONMENT_AUDIT.md`: 제한된 설치 탐색·로컬 daemon/image·Git 관측, 공식 출처와 판단 한계 추가. 과거 관측은 보존.
- `docs/adr/0003-process-and-adr-governance.md`: 기존 운영 결정의 적용 기록 추가. 새 구조 결정이 없어 새 ADR은 생성하지 않음.
- `docs/AI_USAGE.md`: 이번 조사·수정 범위·실패·미검증 기록.

기존 수동 체크리스트를 복제하지 않고 [Unity 절차](DEMO_GUIDE.md#unity-leaderboard-수동-체크리스트)를 재사용했다. 현재 기본 플레이·정상 0점·최고 점수·server 없음·R 재시작·Console 확인과 향후 Windows·Unreal·MySQL 통합 검증을 구분했다. 새 기술 용어를 도입하지 않아 glossary 버전은 유지한다.

### 검증과 한계

첫 patch는 환경 감사의 마지막 문장과 문맥이 달라 거부됐다. 직후 git diff가 비어 있음을 확인하고 실제 원문으로 다시 적용했다. 사용자 파일을 되돌리거나 기존 기록을 삭제하지 않았다.

compile·EditMode·PlayMode·server verification·Windows/Unreal build·MySQL·SVN·사람 화면 검사는 NOT RUN이다. 새로운 Console 무오류 확인도 없으며 과거 사람 PASS를 승계하지 않았다. 문서 정적 검사는 PASS이며 Markdown 28개·내부 링크 234개·앵커 29개·JSON 예제 9개·ADR 14개, 과거 기록 3개 보존·문서 4개 변경 경계·6단계 인계·NOT RUN 유지·추가 문구의 정보 경계를 확인했다. commit·원격 인계 결과는 [PROCESS checkpoint](../PROCESS.md#checkpoints)에 기록한다.

### 채택하지 않은 제안과 다음 확인

설치 대기를 전체 작업 중단 사유로 확대하지 않았다. v2 일괄 전환은 프로젝트의 설계 선택이며 MySQL 자체가 강제하는 조건으로 설명하지 않았다. Object Pool·gameplay Spatial Hash·추가 콘텐츠는 새 필수 작업으로 만들지 않았다. 공개 서버·실제 협업·미검증 완료 주장도 범위에 넣지 않았다.

다음 session은 현재 소스의 exact Unity에서 사람이 checklist를 확인하는 지점부터 재개한다. MySQL·connector·SVN 설치와 저장소 외부 쓰기는 기존 별도 승인 경계를 유지한다.

문서 commit a1bfc39를 work/session-closeout에 push하고 원격 SHA 일치를 확인했다. main은 f896940, 작업 트리는 clean이었다. 후속 CP-20260910-01에 구현 SHA·검증·사람 확인 대기를 기록하며 이번 종료를 runtime 완료나 main 통합으로 표시하지 않는다.

## 2026-09-11 v1 수동 검증

### 실행과 사람 확인

시작 기준선은 clean인 `work/session-closeout`의 `5242a5c`다. 프로젝트 버전과 일치하는 Unity `6000.5.1f1` (`0d9463e84828`), 동일 프로젝트 Editor·lock 부재와 port 7777 부재를 확인한 뒤 승인된 GUI 실행으로 수동 검증을 준비했다. 기존 [체크리스트](DEMO_GUIDE.md#unity-leaderboard-수동-체크리스트)를 재사용하고 소스·설정·package를 수정하지 않았다.

- 오프라인: 사용자가 기본 플레이 PASS, 서버 없음 PASS, 재시작 PASS, Console 오류 없음을 확인했다.
- 서버 준비: 기존 Windows .NET SDK `10.0.401`과 복원된 의존성으로 `dotnet build Server/ArenaSystemsLab.Server/ArenaSystemsLab.Server.csproj --configuration Release --no-restore` 실행, exit 0·경고 0·오류 0. 최초 인증서 생성·telemetry 억제 환경변수는 해당 process에만 적용했다. restore·다운로드·설치는 없다.
- 온라인: 별도 PowerShell 창에서 Release 서버 DLL을 `--port 7777`로 실행하고 실제 해당 서버의 `127.0.0.1:7777` 수신을 확인했다. 사용자가 0점 PASS, 최고 점수 갱신 PASS, 중복 없음, Console 오류 없음을 확인했다. 보고되지 않은 비영점 점수는 추정하지 않았다.
- Unity 실행 로그 `Logs/ManualValidation-20260911-001506.log`에서 exact version·Asset Pipeline Refresh 완료를 확인했다. 이 시작 로그 조회를 전체 Console 무오류의 자동 증명으로 취급하지 않으며 Console PASS의 근거는 사용자 보고다.
- 정상 종료는 사용자에게 Unity Play 종료·서버 Ctrl+C·Editor 종료를 안내한 상태다. 마지막 진단에서는 exact Editor와 해당 loopback 서버가 실행 중이고 lock이 있어 종료를 PASS로 기록하지 않았다.

첫 CIM 조회는 shell 인용 문제로 실패했고 읽기 전용 filter를 수정해 다시 확인했다. 서버 실행 직후 최초 port 조회는 0건이었으나 후속 조회에서 정상 수신을 확인했으며 중복 서버를 띄우지 않았다. 문서 조회 2건은 파일명 불일치로 실패해 실제 링크의 파일명으로 다시 읽었다. 문서 하위 AGENTS 조회의 exit 1은 일치 파일 없음이며 검증 실패가 아니다.

### 변경 범위와 미검증

`work/v1-manual-validation`을 기준선에서 분기해 `PROCESS.md`, `docs/adr/0014-v1-contract-hardening.md`, 이 기록만 갱신한다. PROCESS는 현재 사람 PASS와 다음 종료 확인을 구분하고 ADR·AI의 과거 결과는 보존한다. 기존 결정의 evidence 추가여서 새 ADR·기술 문서 버전·glossary 항목은 만들지 않는다.

기본 플레이·오프라인·온라인·Console은 사용자 확인 PASS다. 새 EditMode·PlayMode·server verification·validator·Windows/Unreal build·MySQL·v2·SVN은 NOT RUN이며 기존 날짜별 자동 PASS를 새 실행으로 옮기지 않는다. 정상 종료·종료 후 파일 변경 검사는 아직 대기다. 서버 실행 후와 이번 문서 편집 전 git status는 clean이며 package 2개·ProjectVersion·ProjectSettings·EditorSettings·SampleScene의 SHA-256 6개가 Unity 실행 전과 일치했다.

추가 기능·dependency·엔진 업그레이드·main 통합은 실행하지 않는다. 문서 검사와 commit·원격 SHA 확인은 [PROCESS checkpoint](../PROCESS.md#checkpoints)에 기록한다.

문서 정적 검사 PASS: Markdown 28개·내부 링크 239개·앵커 33개·JSON 예제 9개·ADR 14개, 과거 기록 2개 보존·문서 3개 변경 경계·미실행 상태·정보 경계·diff 검사. 첫 검사 호출은 JavaScript 문자열 인용 오류로 실행 전에 거부됐고, 수정 후 sandbox의 Git 하위 process EPERM으로 중단됐다. 승인된 재실행에서 exit 0으로 확인했으며 문서 오류나 runtime 실패로 분류하지 않았다.

사용자의 resume 요청 후 미커밋 문서 3개와 기준선 SHA가 유지됨을 확인했다. Unity·해당 서버·loopback port·lock은 여전히 존재해 정상 종료는 대기로 유지했다. 원격 main은 f896940, 이전 마감 branch는 5242a5c였으며 새 검증 branch의 원격 ref는 아직 없었다. 중단된 검증 기록을 보존하고 commit·push부터 이어간다.

검증 기록 commit 68d8da7을 work/v1-manual-validation에 push하고 원격 SHA 일치·clean을 확인했다. main f896940은 유지했다. 후속 CP-20260911-01에 해당 SHA·사람 PASS·자동 테스트 재실행과 정상 종료 대기를 기록했다. 코드·package·설정·Scene은 변경하지 않았고 정상 종료를 기다리는 동안 강제 종료·lock 삭제·추가 Editor 실행은 하지 않았다.

### 2026-09-11 정상 종료와 인계

기준선은 work/v1-manual-validation의 991f40e, Git clean이다. 첫 종료 보고에서는 해당 서버와 port 7777이 사라졌지만 Unity Editor 창과 lock이 남아 있어 재확인을 요청했다. 사용자가 다시 종료 완료를 보고한 뒤 `Get-CimInstance`·`Get-NetTCPConnection`·`Test-Path`로 해당 Unity·서버 process 0개, port 7777 listener 0개, `Temp/UnityLockfile` 없음을 확인했다. 종료 확인 명령 exit 0이며 사용자 보고와 관측을 함께 근거로 정상 종료 PASS를 기록한다.

`git status --short --branch`는 clean, `sha256sum`으로 대조한 package 2개·ProjectVersion·ProjectSettings·EditorSettings·SampleScene의 해시 6개는 실행 전과 일치했다. 이번 변경은 PROCESS의 종료 상태·다음 통합 판단과 기존 ADR 0014·AI 기록의 근거 추가뿐이다. 기존 종료 대기 이력은 보존하며 새 branch·ADR·glossary 항목은 만들지 않았다.

엔진·서버 재실행, compile·EditMode·PlayMode·server verification·Windows/Unreal build는 이번 종료 확인에서 NOT RUN이다. 강제 종료·lock 삭제·package·설정·Scene 수정과 main 통합은 수행하지 않았다. v1 수동 검증 마감은 MySQL·v2·SVN·최신 Windows build의 최종 완료를 뜻하지 않는다.

기존 읽기 전용 문서 검사의 비교 기준을 991f40e, 종료 상태를 PASS로 갱신해 재사용했다. Markdown 28개·내부 링크 240개·앵커 33개·JSON 예제 9개·ADR 14개·과거 기록 2개 보존·문서 3개 변경 경계·미실행 상태·정보 경계·diff 검사 PASS, exit 0이다.

종료 기록 commit d8634a9를 같은 작업 branch에 push하고 원격 SHA 일치·clean을 확인했다. main은 f896940을 유지했다. 후속 CP-20260911-02는 해당 SHA와 종료 PASS를 연결하며 다음 재개 지점을 별도 승인된 main 통합 판단으로 바꾼다. 전체 기술 범위의 최종 DONE 선언은 아니다.
