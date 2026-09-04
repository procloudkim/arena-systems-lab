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

`2026-09-04T02:09:17+09:00`에 사용자의 명시적 요청으로 public `procloudkim/arena-systems-lab` repository를 생성하고 로컬 `origin`으로 연결했다. 공개 repository 목록의 일반 프로젝트 naming pattern인 소문자 kebab-case와 기능 중심 명칭을 적용했다. 원격 repository는 비어 있으며 stage, commit, push는 수행하지 않았다.

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

AI는 ADR 0006에서 하나의 연결된 최소 구조를 선택했다. 현재 Unity arena가 score를 C#/.NET TCP server에 제출하고 MySQL에 저장하며 Unreal C++ `ArenaObserver`가 같은 leaderboard를 읽는다. SVN은 Git source-of-truth와 섞지 않는 isolated workflow lab으로 제한한다.

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

baseline 문서 구현 commit `01cd869`를 `origin/work/portfolio-technology-baseline`에 push하고 local/remote SHA 일치를 확인했다. Markdown link, ADR 0001~0006 sequence와 section, glossary 0.3.0의 43개 entry shape, 변경 경계 검사는 PASS다. runtime test는 documentation-only checkpoint이므로 실행하지 않았다.

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

첫 test invocation은 감사되지 않은 `C:` 기본 Editor 경로를 사용해 executable을 찾지 못했고 test가 실행되지 않았다. 기존 Day 3 log와 제한된 일반 설치 경로 탐색으로 실제 `D:` 설치를 확인한 뒤 exact Editor를 재사용했다.

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

- 현재 보안 문서의 threat model과 local-only 한계가 portfolio 설명에 맞는지 검토한다.
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

첫 `dotnet restore`에서 Windows .NET CLI가 ASP.NET Core HTTPS development certificate를 자동 생성했다고 보고했다. 이 인증서는 server에서 사용하지 않으며 repository 밖 certificate store를 승인 없이 수정하지 않았다. 이후 .NET 명령은 process 범위 telemetry opt-out과 first-time-experience skip을 적용했다.

실제 server CLI smoke에서 WSL Python client의 `127.0.0.1` 연결은 `ConnectionRefused`로 실패했다. Windows process와 같은 host namespace의 PowerShell `TcpClient`로 재검사해 health response를 확인했으며, 첫 결과는 환경 차이에 의한 실패 시도로 그대로 기록했다.
