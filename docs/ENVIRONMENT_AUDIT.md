# Environment Audit

> 이 문서는 감사 시점의 역사적 snapshot이다. 현재 진행 상태와 다음 작업은 [PROCESS.md](../PROCESS.md)를 따른다.

## 감사 실행 시점

`2026-09-03T23:43:04+09:00`

## 조사 범위

현재 작업 폴더와 최대 2단계 하위에서 Unity 프로젝트를 탐색했다. 확인된 프로젝트 루트에서 Unity version, 설치 Editor와 build module, package, input, test, project settings, local development tool, source-control 준비 상태를 읽기 전용으로 조사했다. 사용자 홈이나 drive 전체 검색, token 검색, package 설치, Unity 설정 변경은 수행하지 않았다.

## 환경 감사표

| 항목 | 감지 결과 | 버전/설정 | 상태 | 근거 | 필요한 조치 |
|---|---|---|---|---|---|
| Unity 프로젝트 | 필수 구조 확인 | 생성된 프로젝트 root | READY | `Assets/`, manifest, ProjectVersion | 현재 root 사용 |
| Unity 프로젝트 버전 | 확인됨 | 6000.5.1f1 / `0d9463e84828` | READY | `ProjectVersion.txt` | 업그레이드 금지 |
| 일치하는 Unity Editor | 실행 파일 확인 | 6000.5.1f1 | READY | registry와 executable metadata | 열린 instance 재사용 |
| Unity Hub | 확인됨 | 3.21.0 | READY | registry와 executable | 없음 |
| Windows Build Support | 설치됨 | Standalone / Win64 / Mono | READY | PlaybackEngine variations | 없음 |
| Windows IL2CPP | 없음 | variation 없음 | OPTIONAL | module directory | Day 1 불필요 |
| WebGL | 설치됨 | WebGLSupport | OPTIONAL | module directory | Day 1 불필요 |
| Android | 없음 | AndroidPlayer 없음 | OPTIONAL | module directory | 설치하지 않음 |
| Dedicated Server | 없음 | server variation 없음 | OPTIONAL | module directory | 설치하지 않음 |
| IDE | VS Code, Visual Studio | VS Code 1.136.0 / VS 2026 18.9.1 | READY | registry와 `vswhere` | 없음 |
| Visual Studio Unity workload | 확인되지 않음 | workload/component 0건 | MISSING | `vswhere -requires` | VS Code로 대체 가능 |
| Unity IDE 연동 package | 직접 선언 | Visual Studio 2.0.26 / Rider 3.0.38 | READY | manifest depth 0 | 변경 없음 |
| Input System | 직접 선언 | 1.19.0 | READY | manifest/lock depth 0 | 기존 API 사용 |
| Active Input Handling | New Input System | 값 1 | READY | `ProjectSettings.asset` | Legacy API 미사용 |
| Legacy Input | asset만 존재 | active하지 않음 | OPTIONAL | InputManager와 active setting | 사용하지 않음 |
| Input Actions | 존재 | Player/UI, Move/Attack 포함 | READY | JSON parse | 의미 재사용 |
| Unity Test Framework | 직접 선언 | 1.7.0 builtin | READY | manifest/lock depth 0 | EditMode 테스트 작성 |
| Performance Testing API | 간접 의존성 | 3.5.0 / depth 2 | OPTIONAL | packages-lock | 직접 package로 간주하지 않음 |
| Package 일관성 | 문제 없음 | direct/lock issue 0 | READY | JSON parse | 변경 없음 |
| Git | 설치됨 | 2.43.0 | READY | version command | 없음 |
| Git 저장소 | 초기화 및 기준선 기록 | `main`, initial checkpoint | READY | local/remote ref probe | 작업별 branch 사용 |
| GitHub 원격 | 생성 및 연결됨 | public `procloudkim/arena-systems-lab` | READY | GitHub API와 `git remote -v` | 없음 |
| Git LFS | 설치됨 | 3.7.1 | OPTIONAL | version command | 현재 대상 없음 |
| Codex CLI | 설치됨 | 0.152.1 | READY | version command | 없음 |
| `.gitignore` | 있음 | Unity/IDE 생성물 제외 | READY | `git check-ignore` | source와 `.meta` 유지 |
| Visible Meta Files | 설정됨 | Visible Meta Files | READY | `VersionControlSettings.asset` | 없음 |
| Force Text Serialization | 설정됨 | mode 2 | READY | `EditorSettings.asset` | 없음 |
| Build Target | 확인됨 | Standalone / Win64 | READY | Library setting marker | 없음 |
| Build Scene | SampleScene 1개 활성 | `Assets/Scenes/SampleScene.unity` | READY | EditorBuildSettings | 직접 YAML 수정 금지 |
| Render Pipeline | URP 2D | URP 17.5.0 | READY | package와 asset GUID | 기존 설정 재사용 |
| 기존 compile error | 표식 없음 | C# 0 / import 0 | READY | 현재 Editor log | 변경 후 재검사 |
| 미커밋 변경사항 | 최초 기준선에 포함 | handoff 시 clean 확인 | READY | staged diff와 최종 `git status` | 변경마다 재확인 |
| 대형 source binary | 없음 | 10 MB 초과 0건 | READY | Assets/Packages/ProjectSettings scan | LFS 불필요 |
| 기존 코드 | 없음 | C# 0, asmdef 0, prefab 0 | READY | asset inventory | 별도 namespace/folder 사용 |
| Product Name | 목표 이름으로 정리 | `Arena Systems Lab` | READY | `ProjectSettings.asset`와 exact Editor 재검증 | 없음 |
| 프로젝트 폴더 이름 | 목표 이름으로 정리 | leaf `Arena Systems Lab` | READY | filesystem/Git root probe | Hub에서 새 경로 확인 |
| VS Code Server | 감사 중 자동 갱신 | 새 server version 생성 | CONFLICT | `code --version` 부작용 | 추가 CLI 호출·자동 rollback 금지 |

## Gate 판정

**READY_WITH_GAPS**

정확한 Editor, Windows Mono build support, Input System, Test Framework가 있고 compile 및 Day 1 검증을 통과했다. Git repository, `.gitignore`, 목표 프로젝트 이름도 준비됐지만 Visual Studio Unity workload와 Windows player build는 미완료다. 이 항목들은 현재 구현과 검증을 차단하지 않으므로 판정은 `READY_WITH_GAPS`를 유지한다.

## 필수 조치

- Day 1 필수 검증은 완료됐다.

## 선택 조치

- Visual Studio를 주 IDE로 사용할 때만 Unity workload 설치를 검토한다.
- Unity Hub에서 이름이 변경된 project path를 다시 열어 목록을 갱신한다.

## 확인하지 못한 사항

- 실제 Windows player build 성공 여부
- Rider 실행 파일 존재 여부
- VS Code extension 구성

## 초기 감사에서 실행한 명령과 결과

| 명령 종류 | 목적 | 결과 |
|---|---|---|
| `pwd`, bounded `find`, marker probe | 프로젝트 root 판별 | 최대 2단계에서 root 확인 |
| `git rev-parse`, `git branch`, `git status` | Git 상태 | repository 아님, exit 128 |
| `ProjectVersion.txt` 조회 | 프로젝트 version | 6000.5.1f1 확인 |
| registry/executable metadata 조회 | Hub와 Editor | Hub 3.21.0, exact Editor 확인 |
| PlaybackEngine directory probe | build modules | Windows Mono와 WebGL 확인 |
| Python standard-library JSON parse | manifest/lock/input actions | JSON 유효, package depth와 actions 확인 |
| `rg`, `find`, `strings` | 설정과 asset inventory | Win64, URP 2D, 기존 code 없음 확인 |
| shared-read Editor log 집계 | 초기 compile 상태 | compiler/import error 표식 0건 |
| tool version commands | local tools | Git, LFS, Codex, Node, Python, PowerShell 확인 |
| `vswhere` | Visual Studio와 Unity workload | VS 2026 확인, workload/component 0건 |
| `code --version` | VS Code version 확인 시도 | WSL Server 자동 갱신 발생; version 출력 실패 |

`jq`가 없어 최초 JSON 검사 문구가 잘못 출력됐으나, Python 표준 JSON parser로 재검사해 두 package 파일 모두 유효함을 확인했다. Editor log 일반 읽기는 파일 lock으로 실패했으며 FileShare read-only 방식으로 재검사했다. 실패한 중간 결과는 성공 근거로 사용하지 않았다.

## Day 1 자동 검증 결과

`2026-09-04`에 exact Editor 6000.5.1f1로 EditMode batch test를 실행했다.

- Runtime assembly compile: PASS
- Test assembly compile: PASS
- EditMode tests: 5 passed, 0 failed, 0 skipped
- Test log compiler error: 0
- 결과 파일: `Logs/EditModeResults.xml`
- 성공 로그: `Logs/EditModeTest-2.log`
- PlayMode manual verification: PASS, 사용자 확인
- Unity Console: PASS, 사용자 확인

첫 실행은 `-quit` 때문에 Test Runner가 시작되지 않아 결과 XML이 생성되지 않았다. 설치된 Test Framework 1.7 source의 warning을 확인하고 `-quit`을 제거해 재실행했다. 시작 중 license channel handshake error가 한 차례 기록됐지만 새 channel 연결과 license update가 성공했고 test run은 exit code 0으로 완료됐다.

## Git 준비 후속 감사

실행 시점: `2026-09-04T02:03:33+09:00`

| 명령 | 목적 | 결과 |
|---|---|---|
| `git init -b main` | repository 초기화 | 성공, 빈 `main` branch |
| `git rev-parse --show-toplevel` | 실제 repository root 확인 | Unity 프로젝트 root와 일치 |
| `git branch --show-current` | branch 확인 | `main` |
| `git status --short --branch --untracked-files=all` | 초기 상태 기록 | commit 없음, untracked 75개 |
| `git check-ignore -v` | 생성물 제외 확인 | `Library/`, `Logs/`, `UserSettings/` 제외 |
| source path `git check-ignore` | source/`.meta` 보존 확인 | `Assets`, `Packages`, `ProjectSettings`, `.meta`는 제외되지 않음 |

Git stage, commit, remote 설정, Git LFS 초기화는 실행하지 않았다.

## GitHub 원격 준비

실행 시점: `2026-09-04T02:09:17+09:00`

공개 repository의 일반 프로젝트 이름이 소문자 kebab-case와 기능 중심 명칭을 사용하는 점을 확인하고 `arena-systems-lab`을 선택했다.

| 명령 | 목적 | 결과 |
|---|---|---|
| GitHub public repository API 조회 | 기존 naming pattern 확인 | `webmcp-guardrail-labs`, `pubg-telemetry-watch`, `agri-weather-pipeline` 등 확인 |
| `gh repo view procloudkim/arena-systems-lab` | 이름 충돌 확인 | 기존 repository 없음 |
| `gh repo create procloudkim/arena-systems-lab --public ...` | public 원격 생성과 `origin` 연결 | 성공 |
| `gh repo view ... --json ...` | 원격 설정 검증 | public, 설명 일치 |
| `git ls-remote --heads origin` | 원격 branch 확인 | 출력 없음, 아직 push되지 않은 빈 repository |

원격 URL은 `https://github.com/procloudkim/arena-systems-lab`이다. stage, commit, push는 실행하지 않았다.

## 최초 원격 기준선

실행 시점: `2026-09-04T02:12:12+09:00`

사용자의 최신 Git 운영 지침에 따라 Day 1 source, test, 환경 문서, `.gitignore`, ADR을 `main`의 최초 기준선으로 commit하고 `origin/main`에 push한다. 이 기준선의 검증 근거는 ADR 0001과 commit message에 함께 기록한다.

## 프로젝트 이름 정리

실행일: `2026-09-04`

사용자 승인에 따라 활성 Unity project folder leaf와 PlayerSettings의 `productName`을 `Arena Systems Lab`으로 통일했다. UWP에 남아 있던 template 이름도 `metroPackageName: ArenaSystemsLab`, `metroApplicationDescription: Arena Systems Lab`로 정리했다. 같은 Location 아래의 `My`와 `My project` 잔여 폴더는 범위 밖이므로 이동하거나 삭제하지 않았다.

- Exact Editor 6000.5.1f1 compilation: PASS
- EditMode tests: 5 passed, 0 failed, 0 skipped
- Compiler/batch failure marker: 0
- 승인 범위 밖 tracked file 변경: 0
- Windows player build: NOT RUN

## 필수 기술 baseline 후속 감사

실행 시점: `2026-09-04T22:20:56+09:00`

사용자가 최종 portfolio 하한으로 지정한 Unity, Unreal Engine, Git, SVN, MySQL, network programming, socket programming, multithreading, OOP를 대상으로 기존 설치와 repository 구현 상태를 읽기 전용으로 다시 확인했다. program 탐색은 command lookup, Windows registry, 일반 설치 경로로 제한했고 drive나 home 전체를 검색하지 않았다.

| 항목 | 감지 결과 | 버전/설정 | 상태 | 근거 | 필요한 조치 |
|---|---|---|---|---|---|
| Unity | 현재 playable project | Editor 6000.5.1f1 exact match | READY | 기존 compile/test/manual evidence | Day 3~4 계속 진행 |
| Unreal Editor | 일반 설치 경로에서 executable 확인 | 5.8.0 / CL 55116800 | READY | `Build.version`, executable metadata | exact version으로 새 project 검증 |
| Unreal project | repository에 없음 | `.uproject` 0개, depth 4 | MISSING | bounded repository search | Milestone 8에서 최소 C++ observer 생성 |
| Visual Studio Native Game | workload와 x64 C++ component 확인 | VS Community 18.9.1 | READY | `vswhere -requires` 2종 | 기존 toolchain 재사용 |
| Git | local/remote 동기화 | 2.43.0, public origin | READY | version, status, remote ref | 기존 workflow 유지 |
| SVN | command·일반 설치·registry 없음 | Ubuntu candidate 1.14.3-1build4 | MISSING | `command -v`, `where.exe`, path/registry, `apt-cache policy` | 설치 승인 필요 |
| MySQL | client·service·일반 설치·registry 없음 | native server version 없음 | MISSING | command, service, path/registry probe | runtime 준비 승인 필요 |
| Docker Desktop client | Windows executable 확인 | client 29.7.2 | READY | `docker.exe version` client result | 기존 설치 재사용 |
| Docker daemon/MySQL image | daemon 응답 없음 | image list 조회 불가 | UNKNOWN | server version·named pipe 없음 | 사용자가 Desktop 시작 후 재감사 |
| .NET SDK | Windows executable 확인 | 10.0.400 | READY | `dotnet.exe --version`, SDK list | BCL 기반 server에 재사용 |
| Network programming | source 없음 | 구현 0건 | MISSING | repository code search | Milestone 5·7·8 구현 |
| Socket programming | source 없음 | 구현 0건 | MISSING | repository code search | 공통 TCP protocol 구현 |
| Multithreading | source 없음 | 구현 0건 | MISSING | repository code search | concurrent server와 test 구현 |
| OOP | Unity runtime 책임 분리 | Health/controller/FSM/spawner | READY | source와 EditMode test | server/client에서도 유지 |

### 후속 Gate 판정

**READY_WITH_GAPS**

Unity, Unreal, Git, C++ toolchain과 .NET SDK는 기존 설치를 재사용할 수 있어 승인 없는 영역부터 진행 가능하다. SVN과 MySQL runtime은 충분한 근거로 `MISSING`이고 Docker image는 `UNKNOWN`이므로 해당 milestone은 승인 전 실행하지 않는다. 현재 repository source와 package에는 충돌이 없다.

### 승인 전에 실행하지 않은 제안

| 제안 | 필요성 | 영향 | 대안 | rollback 방법 |
|---|---|---|---|---|
| WSL Ubuntu `subversion` package 설치 | SVN lab의 `svn`, `svnadmin` 필요 | 관리자 권한, package download, WSL system 변경 | Windows용 공식 binary provider를 사용자가 선택 | 설치 방식에 맞춰 package 제거; lab data는 별도 확인 후 삭제 |
| Docker Desktop 시작 후 `mysql:8.4` image download | MySQL 8.4 LTS integration test 필요 | daemon 실행, image download, local container와 data volume 생성 | native MySQL server 설치 요청 | container/image 제거; data volume 삭제는 별도 명시 승인 |
| `.NET` project에 `MySqlConnector` 2.6.2 추가 | BCL에는 MySQL provider가 없음 | third-party NuGet dependency와 lock/asset 변경 | Oracle `MySql.Data` 9.7.0 | `dotnet remove package MySqlConnector`와 code rollback |

승인된 설치·download·package 추가는 아직 하나도 실행하지 않았다.

### 실행한 명령과 결과

| 명령 종류 | 목적 | 결과 |
|---|---|---|
| `git status`, branch/ref probe | 변경 전 Git 기준선과 branch 충돌 확인 | `main...origin/main` clean, 대상 branch 없음 |
| bounded `.uproject` search | 기존 Unreal project 확인 | 없음 |
| Unreal `Build.version`와 executable metadata 조회 | 설치 Engine exact version 확인 | 5.8.0 / CL 55116800 |
| `vswhere -requires` C++/NativeGame | Unreal C++ build toolchain 확인 | VS Community 18.9.1 반환 |
| WSL/Windows command lookup, 일반 경로, registry probe | SVN 설치 확인 | 발견되지 않음 |
| `apt-cache policy subversion` | 설치 상태와 repository candidate 확인 | installed 없음, candidate 1.14.3-1build4 |
| command/service/일반 경로/registry probe | MySQL 설치·service 확인 | 발견되지 않음 |
| Windows Docker client/version probe | 기존 Docker 재사용 가능성 확인 | client 29.7.2, daemon 응답 없음 |
| Windows `dotnet.exe --version`, `--list-sdks` | C# server SDK 확인 | 10.0.400 |
| repository `rg` source search | network/socket/thread 구현 여부 확인 | 구현 없음 |

### 외부 기준 확인

- MySQL 8.4는 공식 LTS series다: [MySQL Release Model](https://dev.mysql.com/doc/refman/8.4/en/mysql-releases.html)
- Docker Official Image에는 rolling `mysql:8.4` tag가 제공된다: [Docker Hub MySQL Official Image](https://hub.docker.com/_/mysql)
- Ubuntu의 Subversion package에는 command-line client와 `svnserve`가 포함된다: [Apache Subversion Binary Packages](https://subversion.apache.org/packages.html)
- 권장 connector 후보와 version은 감사 시점 NuGet listing으로 확인했다: [MySqlConnector 2.6.2](https://www.nuget.org/packages/MySqlConnector)
- 공식 provider 대안은 Oracle Connector/NET이다: [MySQL Connector/NET 9.7](https://dev.mysql.com/downloads/connector/net/9.7.html)

## Day 3 측정·Editor Tool 검증

실행 시점: `2026-09-04T22:58:37+09:00`

exact Unity Editor 6000.5.1f1가 닫혀 있고 `Temp/UnityLockfile`이 없는 상태에서 새 runtime, Editor, EditMode, PlayMode assembly를 import하고 검증했다.

| 검증 | 결과 | 근거 |
|---|---|---|
| Runtime/Editor/Test compilation | PASS | final batch log compiler error marker 0 |
| EditMode tests | PASS | 16 passed / 0 failed / 0 skipped |
| Spatial query correctness | PASS | spatial hash와 brute-force 모두 6,882 matches |
| Spatial query measurement | RECORDED | 1.381 ms 대 73.824 ms, 단일 Editor run |
| Project validator CLI | PASS | `Arena Systems Lab validation passed.` |
| PlayMode profile test | PASS | 1 passed / 0 failed / 0 skipped |
| Gameplay profile baseline | RECORDED | target 120 FPS, 1 s warm-up, 5 s sampling, 601 samples |
| Scene/Prefab/Package/tracked ProjectSettings | UNCHANGED | Git diff 확인 |
| Human Day 3 verification | PASS | 사용자 확인, Editor menu·기존 gameplay·Console checklist 완료 |

PlayMode 첫 시도는 marker recorder option 누락으로 `NotSupportedException`이 발생했다. `SumAllSamplesInFrame`을 공용 recorder 생성 지점에 추가해 해결했다. 이후 sampling 초기화를 위해 호출한 `Reset()`이 수집도 중지한다는 local Unity API 문서를 확인했고 `Start()`를 추가한 뒤 final test가 통과했다.

Unity가 test 실행 중 미추적 `ProjectSettings/SceneTemplateSettings.json`을 생성했다. 승인 범위 밖 파일이므로 내용을 확인하고 test 완료 후 제거했으며 기존 tracked ProjectSettings 변경은 0건이다.

## Day 4 Windows build·demo 검증

실행 시점: `2026-09-04T23:19:12+09:00`

exact Unity Editor가 닫혀 있고 project lock이 없는 상태에서 전체 regression, project validation, Windows Mono Development build와 player launch smoke를 순서대로 실행했다.

| 검증 | 결과 | 근거 |
|---|---|---|
| Runtime/Editor/Test compilation | PASS | Day 4 EditMode import와 compile, compiler error marker 0 |
| EditMode regression | PASS | 16 passed / 0 failed / 0 skipped |
| PlayMode regression/profile | PASS | 1 passed / 0 failed / 0 skipped |
| Project validator CLI | PASS | validation success log |
| Windows Mono Development build | PASS | build result Success, 64.697초 |
| Player artifact | PASS | PE32+ GUI x86-64, local output 약 166 MB |
| Player launch smoke | PASS | 8초 process 생존 후 해당 process 종료 |
| Player log | PASS_WITH_WARNINGS | managed exception·crash 없음; D3D12 info queue와 shutdown cleanup 진단 존재 |
| Windows player full gameplay | PASS | 사용자 확인, standalone 8단계와 오류 없음 |
| Package/Scene/Prefab final diff | UNCHANGED | Git diff 확인 |
| Tracked ProjectSettings final diff | UNCHANGED | build 자동 직렬화분 복원 후 Git diff 확인 |

첫 test 명령은 설치 위치를 `C:` 기본 경로로 추정해 executable을 찾지 못했으므로 테스트가 실행되지 않았다. 이전 log와 일반 설치 경로에서 감사된 실제 `D:` Editor를 확인해 이후 명령에 재사용했다.

Unity/URP는 build 중 `DefaultVolumeProfile.asset`, `UniversalRP.asset`, `UniversalRenderPipelineGlobalSettings.asset`, `ProjectSettings.asset`과 미추적 `SceneTemplateSettings.json`을 자동 직렬화했다. 작업 전 clean 상태와 diff를 대조해 이번 실행이 만든 변경만 복원했으며 build output과 log는 기존 `.gitignore` 규칙으로 제외했다.

### 실행한 명령과 결과

| 명령 종류 | 목적 | 결과 |
|---|---|---|
| exact Editor EditMode `-runTests` | 전체 logic·Editor regression | PASS, 16/16 |
| exact Editor PlayMode `-runTests` | gameplay profile regression | PASS, 1/1 |
| `ArenaProjectValidator.ValidateFromCommandLine` | project preflight | PASS |
| `ArenaWindowsBuilder.BuildWindowsFromCommandLine` | Windows Development player 생성 | PASS |
| Windows `Start-Process`, targeted close | 생성 player 시작 확인 | PASS, 8초 생존 |
| `file`, `sha256sum`, output inventory | artifact 형식과 존재 확인 | x86-64 PE, SHA-256 기록 |
| `git diff`, `git status` | Unity 자동 변경과 최종 source 경계 확인 | 승인 범위 밖 최종 diff 0 |

설치, package 추가, download, Unity Editor upgrade는 수행하지 않았다.
