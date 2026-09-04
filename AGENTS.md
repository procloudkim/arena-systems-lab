# Arena Systems Lab 작업 지침

## 1. Project objective

Unity와 C#으로 플레이 가능한 시스템을 만들고, 객체지향 설계, 알고리즘 적용, 디버깅, 성능 측정, Editor Tool, 테스트, AI 생성 코드의 사람 검증 기록을 포트폴리오로 남긴다. 최종 완료 하한은 Unity, Unreal Engine, Git, Apache Subversion(SVN), MySQL, network programming, socket programming, multithreading, OOP의 실제 구현·검증 근거다.

## 2. Current milestone

현재 마일스톤, 검증 상태, 다음 재개 지점은 [PROCESS.md](PROCESS.md)에서만 관리한다. 모든 session은 구현 전에 이 문서를 읽는다.

## 3. Repository structure

- `Assets/Scenes/`: 기존 Unity Scene
- `Assets/Settings/`: 기존 URP 2D 설정
- `Assets/InputSystem_Actions.inputactions`: 기존 Input System 액션 정의
- `Assets/ArenaSystemsLab/Runtime/`: gameplay runtime, 최소 enemy FSM, 실험용 `SpatialHash2D`
- `Assets/ArenaSystemsLab/Editor/`: project validation과 Windows Development build Editor Tool
- `Assets/ArenaSystemsLab/Tests/EditMode/`: runtime logic, spatial query, Editor validation 테스트
- `Assets/ArenaSystemsLab/Tests/PlayMode/`: 자동 gameplay profiling 기준선 테스트
- `Server/ArenaSystemsLab.Server/`: BCL-only loopback TCP leaderboard server
- `Server/ArenaSystemsLab.Server.Verification/`: package 없는 protocol·security·threading 검증 executable
- `Server/NuGet.Config`: server restore에서 외부 package source를 사용하지 않는 기준
- `Packages/`: Unity package 선언과 lock 파일
- `ProjectSettings/`: Unity 프로젝트 설정
- `README.md`: 프로젝트 소개, 실행, 검증, build entry point
- `PROCESS.md`: 현재 상태와 다음 재개 지점의 단일 기준
- `docs/`: 감사, 구현 계획, AI 사용 기록
- `docs/GAME_DEV_GLOSSARY.md`: 게임·Unity·물리·검증 용어 백과사전
- `docs/PERFORMANCE_BASELINE.md`: 측정 조건, 수치, 최적화 채택 여부
- `docs/DEMO_GUIDE.md`: 3~5분 demo flow와 Windows player 수동 checklist
- `docs/NETWORK_SECURITY.md`: network threat model, protocol limits, remote exposure gate
- `docs/adr/`: 사람과 LLM이 함께 읽는 의사결정 기록
- `.gitignore`: Unity/IDE 생성물 제외 규칙

다음 디렉터리는 [ADR 0006](docs/adr/0006-portfolio-technology-baseline.md)에 따라 필요한 milestone에서만 만든다. 아직 없으면 생성됐다고 가정하지 않는다.

- `Database/`: planned MySQL schema와 migration
- `Unreal/`: planned Unreal Engine C++ `ArenaObserver`
- `docs/evidence/`: planned 재현 가능한 검증 evidence

`Library/`, `Temp/`, `Logs/`, `UserSettings/`는 생성 결과이며 소스의 기준이 아니다.

## 4. Verified development environment

- Unity Editor: `6000.5.1f1` (`0d9463e84828`), 프로젝트와 정확히 일치
- Render Pipeline: Universal Render Pipeline `17.5.0`, 2D Renderer
- Input: Input System `1.19.0`, Active Input Handling은 Input System only
- Test Framework: `1.7.0`, manifest 직접 선언
- Performance Testing API: `3.5.0`, lock의 간접 의존성만 확인
- Build Target: Standalone / Win64
- Build Support: Windows Mono 사용 가능, IL2CPP 없음, WebGL 있음
- Product Name: `Arena Systems Lab`; project folder leaf: `Arena Systems Lab`
- Version Control Mode: Visible Meta Files
- Asset Serialization: Force Text
- Git 저장소의 최초 검증 기준선은 `main`에 commit/push한다. Public `origin`은 `https://github.com/procloudkim/arena-systems-lab.git`이다. Git LFS도 설치돼 있지만 현재 필요한 대형 source asset은 없다.
- Visual Studio는 설치돼 있으나 Unity workload/component는 확인되지 않았다.
- Unreal Engine: `5.8.0`, Editor executable 확인
- Visual Studio Native Game workload와 C++ x64 tool component: 확인됨
- Windows .NET SDK: `10.0.400`, 전체 Windows executable 경로로 사용 가능
- Apache Subversion client/admin: 확인된 command·표준 설치·registry가 없어 `MISSING`
- MySQL client/server/service: 확인된 command·표준 설치·registry가 없어 `MISSING`
- Docker Desktop client: `29.7.2`; daemon은 실행되지 않아 기존 MySQL image는 `UNKNOWN`
- VS Code는 설치돼 있다. 감사 중 WSL Server 자동 갱신이 발생했으므로 추가 `code` CLI 호출을 피한다.

## 5. Source-of-truth files

- Unity 버전: `ProjectSettings/ProjectVersion.txt`
- 직접 package 의존성: `Packages/manifest.json`
- 해석된 package 의존성과 depth: `Packages/packages-lock.json`
- 입력 활성화: `ProjectSettings/ProjectSettings.asset`
- serialization: `ProjectSettings/EditorSettings.asset`
- meta file mode: `ProjectSettings/VersionControlSettings.asset`
- build scenes: `ProjectSettings/EditorBuildSettings.asset`
- 입력 액션: `Assets/InputSystem_Actions.inputactions`
- 현재 진행 상태와 checkpoint: `PROCESS.md`
- 프로젝트 진입점과 실행 방법: `README.md`
- 게임 개발 용어 정의: `docs/GAME_DEV_GLOSSARY.md`
- 감사 상태: `docs/ENVIRONMENT_AUDIT.md`
- AI 작업 기록: `docs/AI_USAGE.md`
- Day 3 측정 기준선: `docs/PERFORMANCE_BASELINE.md`
- demo와 standalone 수동 검증: `docs/DEMO_GUIDE.md`
- network protocol과 보안 경계: `docs/NETWORK_SECURITY.md`
- standalone server build 기준: `Server/ArenaSystemsLab.Server/ArenaSystemsLab.Server.csproj`
- standalone server verification 기준: `Server/ArenaSystemsLab.Server.Verification/ArenaSystemsLab.Server.Verification.csproj`
- 작업·구조 의사결정: `docs/adr/`
- 최종 필수 기술 baseline: `docs/adr/0006-portfolio-technology-baseline.md`
- milestone과 완료 근거 matrix: `docs/IMPLEMENTATION_PLAN.md`
- 생성물 제외 정책: `.gitignore`

## 6. Dependency policy

- 제3자 package 추가에는 사용자의 명시적 승인이 필요하다.
- 기존 Unity package와 built-in API를 먼저 재사용한다.
- 승인 없이 `Packages/manifest.json` 또는 `Packages/packages-lock.json`을 수정하지 않는다.
- Unity Editor를 자동 업그레이드하지 않는다.
- Day 1~4 Unity MVP 동안 networking, database, ECS, DI, tweening, async helper, behavior tree, asset framework package를 설치하지 않는다.
- ADR 0006의 post-MVP server/database 확장은 승인된 dependency만 사용한다. Unity package는 추가하지 않는다.
- MySQL wire protocol을 직접 구현하지 않는다. connector가 필요하면 승인 요청에 package, exact version, license, 영향, 대안, rollback을 포함한다.
- 의존성 추가를 요청하기 전에 이유, 버전, 영향, rollback 방법을 기록한다.

## 7. Scope restrictions

Day 1에는 Spatial Hash, Object Pool, 정식 Enemy FSM, multiplayer, network, database, login, cloud service, Addressables, ECS/DOTS, 복잡한 animation, 외부 art, sound, save, DI, tween, async helper를 구현하지 않는다.

Day 2 enemy FSM은 `enum`과 작은 상태 결정 class로 유지한다. 상태별 interface/class 계층은 실제로 서로 다른 행동 구현이 필요해질 때만 검토한다.

최종 portfolio 확장은 ADR 0006 범위로 제한한다. Unity arena client, C#/.NET TCP leaderboard server, MySQL persistence, Unreal C++ read-only observer, isolated SVN workflow evidence만 만든다. multiplayer simulation, public deployment, authentication, cloud service, 별도 Unreal game은 만들지 않는다.

실제 다직군 협업과 live-service 대응 경험은 사용자의 현재 결정에 따라 완료 기준과 포트폴리오 주장에 포함하지 않는다.

명시적 사용자 승인 없이 기존 `.unity`, `.prefab`, package 파일과 Project Settings를 직접 수정하지 않는다. 승인된 naming 작업은 `productName`, `metroPackageName`, `metroApplicationDescription`에 한정한다. Scene 구성이 필요하면 기존 Scene을 보존하고 runtime 구성 또는 안전한 Editor API를 사용한다.

## 8. Coding conventions

- namespace는 `ArenaSystemsLab`로 시작한다.
- 클래스는 한 가지 명확한 책임만 갖는다.
- Inspector 노출은 `[SerializeField] private`를 우선한다.
- 필요한 component는 `[RequireComponent]`로 표현하고 참조 오류는 이해 가능한 메시지로 남긴다.
- 입력은 현재 활성화된 `UnityEngine.InputSystem` API만 사용한다.
- 매 frame LINQ, boxing, 새 collection 생성 등 불필요한 allocation을 만들지 않는다.
- 측정하지 않은 성능 향상을 주장하지 않는다.
- 한 구현뿐인 interface, factory, service locator, DI container를 만들지 않는다.
- network payload는 크기와 값 범위를 검증하고 최초 통합은 loopback으로 제한한다.
- loopback을 authentication으로 간주하지 않는다. server는 frame 16 KiB, JSON depth 8, client 16개, request 5초, player 10,000개 상한을 유지한다.
- TLS, authentication, abuse control과 server-authoritative score 검증 전에는 server를 LAN/public interface에 bind하지 않는다.
- raw network payload, credential, player-controlled 문자열과 stack trace를 운영 log에 남기지 않는다.
- asynchronous I/O와 multithreading을 같은 것으로 기록하지 않는다. concurrent test와 shared-state 안전성 근거가 있어야 multithreading 완료로 표시한다.
- credential, connection string, local database volume, generated engine cache를 commit하지 않는다.

## 9. Testing and validation commands

- 초기 import 검증: 현재 Editor 로그에서 C# compiler/import error 표식 0건을 확인했다.
- EditMode Test Runner UI: **Not yet verified**. `Window > General > Test Runner`에서 EditMode 테스트를 실행한다.
- 자동 EditMode 명령: **Verified on 2026-09-04**. 정확히 일치하는 Editor가 닫히고 Unity process와 `Temp/UnityLockfile`이 없을 때 다음 형태로 실행한다.
  `"<UnityEditor>/Unity.exe" -batchmode -nographics -projectPath "<project-root>" -runTests -testPlatform EditMode -testFilter "ArenaSystemsLab.Tests.EditMode" -testResults "<project-root>/Logs/EditModeResults.xml" -logFile "<project-root>/Logs/EditModeTest.log"`
- Test Framework 1.7에서는 command-line test에 `-quit`을 함께 지정하지 않는다. 설치 package source가 이 조합은 동작하지 않는다고 명시한다.
- EditMode 결과: **Verified on 2026-09-04**, 16 passed / 0 failed / 0 skipped.
- 자동 PlayMode profiling 명령: **Verified on 2026-09-04**. 위 명령에서 `-testPlatform PlayMode -testFilter "ArenaSystemsLab.Tests.PlayMode"`를 사용하며 결과는 1 passed / 0 failed / 0 skipped.
- Project validation command-line: **Verified on 2026-09-04**. Batchmode에서 `-executeMethod ArenaSystemsLab.Editor.ArenaProjectValidator.ValidateFromCommandLine`을 사용한다.
- Project validation Editor menu: **Verified on 2026-09-04**. 사용자가 `Tools > Arena Systems Lab > Validate Project`를 포함한 Day 3 수동 검증 완료를 확인했다.
- PlayMode 수동 검증: Day 1 흐름, Day 2 상태 색상·전이, Day 3 변경 후 기존 gameplay 회귀를 **Verified on 2026-09-04**.
- Unity Console: Day 1~3 변경 후 **Verified on 2026-09-04**, 사용자 확인 오류 없음.
- Windows Mono Development build command-line: **Verified on 2026-09-04**. Batchmode에서 `-executeMethod ArenaSystemsLab.Editor.ArenaWindowsBuilder.BuildWindowsFromCommandLine`을 사용하며 output은 `Builds/Windows/ArenaSystemsLab.exe`다.
- Windows build Editor menu: **Not yet human verified**. `Tools > Arena Systems Lab > Build Windows Development`를 사용한다.
- Windows player launch smoke: **Verified on 2026-09-04**. 생성된 x86-64 player가 8초 동안 생존했고 managed exception·crash 없이 종료됐다.
- Windows player 전체 gameplay 수동 검증: **Verified on 2026-09-04**. 사용자가 `docs/DEMO_GUIDE.md`의 이동·공격·FSM·Game Over·restart checklist PASS와 오류 없음을 확인했다.
- Windows .NET SDK version 명령: **Verified on 2026-09-04**. Windows `dotnet.exe --version` 결과 `10.0.400`.
- .NET server restore/build/verification: **Verified on 2026-09-05** with Windows .NET SDK 10.0.400. `<dotnet>`을 감사된 executable로 바꿔 다음 argument를 사용한다.
  - `"<dotnet>" restore Server/ArenaSystemsLab.Server.Verification/ArenaSystemsLab.Server.Verification.csproj --configfile Server/NuGet.Config`
  - `"<dotnet>" build Server/ArenaSystemsLab.Server.Verification/ArenaSystemsLab.Server.Verification.csproj --configuration Release --no-restore`
  - `"<dotnet>" run --project Server/ArenaSystemsLab.Server.Verification/ArenaSystemsLab.Server.Verification.csproj --configuration Release --no-build --no-restore`
- .NET Release build: **PASS**, warnings 0 / errors 0. Verification executable: **PASS**, 8 passed / 0 failed.
- .NET server CLI smoke: **PASS on 2026-09-05**, port 7777에서 Windows PowerShell client `health` response와 `Ctrl+C` 정상 종료 확인.
- Unreal C++ build/run: **Not yet verified**. `ArenaObserver` project가 생긴 뒤 exact Engine 5.8로 확인한다.
- SVN workflow와 MySQL integration: **Not yet verified**. 필요한 도구와 dependency가 승인·준비된 뒤 실행한다.

검증하지 않은 명령을 성공한 명령처럼 기록하지 않는다.

## 10. Git safety rules

- 작업 전후 `git status`를 기록한다. 저장소가 아니면 그 사실을 기록한다.
- 최초 기준선은 `main`에 남긴다. 이후 주요 작업은 `work/<short-kebab-topic>` branch에서 시작하고 해당 branch를 push한다.
- 의미 있는 변경은 `PROCESS.md`와 관련 ADR을 갱신하고, 실제 검증 결과를 commit message에 기록한 뒤 같은 작업 turn에서 commit과 push까지 완료한다.
- 실행하지 않은 test를 통과했다고 기록하지 않는다. 실패나 미검증 항목도 commit과 ADR에 명시한다.
- 생성물, credential, token을 stage하지 않는다.
- Unity build 뒤 tracked asset이나 ProjectSettings가 자동 직렬화되면 diff로 이번 build의 부작용인지 확인하고 승인 없이 commit하지 않는다.
- `git reset`, `git clean`, `git checkout -- .`, `git restore .`, `git stash`, `git pull`, `git rebase`, force push를 실행하지 않는다.
- 사용자 변경을 되돌리거나 덮어쓰지 않는다.
- commit 전 staged diff와 전체 상태를 검토하고, push 후 remote ref를 확인한다.

## 11. AI-assisted development policy

- AI가 조사한 근거, 생성·수정 파일, 실행한 검증과 미검증 항목을 `docs/AI_USAGE.md`에 기록한다.
- AI 코드는 사람이 PlayMode 동작, Console 오류, Inspector 상태를 확인하기 전까지 완료로 간주하지 않는다.
- AI 제안은 기존 코드, Unity 문서, compiler/test 결과와 대조한다.
- 실패와 환경 부작용을 숨기지 않고 원인과 영향을 기록한다.
- AI는 session 시작 시 `PROCESS.md`를 읽고 종료 전 현재 상태와 checkpoint를 갱신한다.
- 새 게임·Unity·물리·검증 용어가 처음 등장하면 같은 checkpoint에서 `docs/GAME_DEV_GLOSSARY.md`와 version history를 갱신한다.

## 12. Definition of done

- 요청 범위가 실제 플레이 흐름으로 연결된다.
- 변경된 C# 코드가 정확한 Unity Editor에서 compile된다.
- 전체 EditMode 테스트가 통과한다.
- Console에 이번 변경으로 생긴 error가 없다.
- 사람이 변경된 PlayMode flow를 확인한다.
- package와 Project Settings에 승인되지 않은 변경이 없다.
- 문서와 AI 사용 기록이 실제 결과와 일치한다.
- `PROCESS.md`가 현재 상태와 다음 작업을 단독으로 설명한다.
- 새로 사용한 전문 용어가 glossary에 정의되거나 기존 항목을 참조한다.
- 관련 ADR과 검증 근거가 commit에 포함되고 해당 branch가 remote에 push된다.
- Unity, Unreal Engine, Git, SVN, MySQL, network programming, socket programming, multithreading, OOP 각각에 source와 재현 가능한 검증 근거가 있다.
- Git만 canonical VCS로 사용하고 SVN metadata는 active Git working tree에 섞지 않는다.
- 실제 협업·live-service 대응을 수행했다고 주장하지 않는다.
