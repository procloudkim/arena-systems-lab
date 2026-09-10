# Environment Audit

> 이 문서는 감사 시점의 역사적 snapshot이다. 현재 진행 상태와 다음 작업은 [PROCESS.md](../PROCESS.md)를 따른다. 아래 명령의 개인 식별 인수는 `<owner>`·`<repository-origin>`으로 치환했으며 실제 인수 그대로의 재실행 기록이 아니다.

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
| Input Actions | 존재 | Player/UI, Move/Attack 포함 | READY | JSON parse | 기존 에셋 보존, runtime은 장치 API 직접 사용 |
| Unity Test Framework | 직접 선언 | 1.7.0 builtin | READY | manifest/lock depth 0 | EditMode 테스트 작성 |
| Performance Testing API | 간접 의존성 | 3.5.0 / depth 2 | OPTIONAL | packages-lock | 직접 package로 간주하지 않음 |
| Package 일관성 | 문제 없음 | direct/lock issue 0 | READY | JSON parse | 변경 없음 |
| Git | 설치됨 | 2.43.0 | READY | version command | 없음 |
| Git 저장소 | 초기화 및 기준선 기록 | `main`, initial checkpoint | READY | local/remote ref probe | 작업별 branch 사용 |
| GitHub 원격 | 생성 및 연결됨 | public `arena-systems-lab` | READY | GitHub API와 `git remote -v` | 없음 |
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
| GitHub public repository API 조회 | 기존 naming pattern 확인 | 소문자 kebab-case의 기능 중심 명명 규칙 확인 |
| `gh repo view <owner>/arena-systems-lab` | 이름 충돌 확인 | 기존 repository 없음 |
| `gh repo create <owner>/arena-systems-lab --public ...` | public 원격 생성과 `origin` 연결 | 성공 |
| `gh repo view ... --json ...` | 원격 설정 검증 | public, 설명 일치 |
| `git ls-remote --heads origin` | 원격 branch 확인 | 출력 없음, 아직 push되지 않은 빈 repository |

원격은 로컬 Git의 `origin`으로 연결했다. stage, commit, push는 실행하지 않았다.

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

사용자가 최종 기술 하한으로 지정한 Unity, Unreal Engine, Git, SVN, MySQL, network programming, socket programming, multithreading, OOP를 대상으로 기존 설치와 repository 구현 상태를 읽기 전용으로 다시 확인했다. program 탐색은 command lookup, Windows registry, 일반 설치 경로로 제한했고 drive나 home 전체를 검색하지 않았다.

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

## Milestone 5 네트워크 보안·.NET 검증

실행 시점: `2026-09-05T00:05:22+09:00`

Day 4 통합 후 `work/network-security-foundation` branch에서 외부 package 없이 C#/.NET TCP server를 구현하기 전에 공식 자료로 신뢰 경계와 자원 제한을 조사했다. TCP stream framing, JSON 제한, server-side input validation, resource exhaustion, logging, async I/O와 multithreading 구분, 향후 TLS 기준을 [Network Security Baseline](NETWORK_SECURITY.md)에 근거와 함께 기록했다.

| 항목 | 감지 결과 | 버전/설정 | 상태 | 근거 | 필요한 조치 |
|---|---|---|---|---|---|
| Windows .NET SDK | 기존 설치 재사용 | 10.0.400 | READY | 이전 version 감사와 실제 restore/build/run | 없음 |
| Server dependency | BCL only | NuGet package 0개, source clear | READY | 두 csproj와 `Server/NuGet.Config` | 승인 없이 package 추가 금지 |
| TCP bind | IPv4 loopback only | `127.0.0.1`, default port 7777 | READY | `TcpListener(IPAddress.Loopback, port)`와 integration check | remote bind 금지 |
| Framing | length-prefixed UTF-8 JSON | 4-byte big-endian, 최대 16 KiB | READY | source와 fragmented/oversized check | engine client에서 동일 규격 사용 |
| Input validation | exact schema와 bounds | JSON depth 8, ID/score/query 제한 | READY | parser check | client validation으로 대체 금지 |
| Resource limits | 명시적 상한 | clients 16, timeout 5초, players 10,000 | READY | source와 timeout/capacity check | 실제 부하 한계는 미측정 |
| Shared state | `lock`으로 보호 | actual worker thread 8개 | READY | multithreaded store check | 측정 전 lock 구조 변경 금지 |
| TLS/authentication | 구현 없음 | plaintext, identity 없음 | MISSING | source와 threat model | remote exposure 전 필수 |
| Score authority | client 제출값 범위만 검사 | gameplay 정당성 검증 없음 | MISSING | protocol semantics | public leaderboard 전 설계 |

### 실행한 명령과 결과

| 명령 종류 | 목적 | 결과 |
|---|---|---|
| Git branch/status와 bounded source inventory | Day 4 통합 상태와 기존 server source 충돌 확인 | `main`=`origin/main` at `75151b9`, server source 없음, 새 branch 시작 |
| 공식 RFC·Microsoft·OWASP·CWE 자료 조사 | protocol·input·DoS·log·async/TLS 사실 검증 | claim 6개 판정과 remote gate 기록 |
| `dotnet restore ... --configfile Server/NuGet.Config` | 외부 source 없는 project assets 생성 | PASS, 두 project restore |
| `dotnet build ... --configuration Release --no-restore` | server와 verification compile | PASS, warnings 0 / errors 0 |
| `dotnet run ... --no-build --no-restore` | protocol·security·threading integration 검사 | PASS, 8 passed / 0 failed |
| actual server CLI + Windows PowerShell socket client | process entry point와 cross-process frame 확인 | PASS, port 7777 health response·graceful shutdown |
| WSL Python client → Windows loopback | cross-OS namespace 동작 확인 시도 | FAIL, `ConnectionRefused`; server 판정 근거에서 제외 |
| `git check-ignore -v` | .NET generated artifact와 csproj 경계 확인 | `obj/bin` ignored, `Server/**/*.csproj` source로 보존 |

첫 `dotnet restore`는 package source를 비운 상태라 외부 NuGet package를 받지 않았다. 다만 Windows .NET CLI 최초 실행 메시지가 ASP.NET Core HTTPS development certificate 생성 사실을 보고했다. 이번 TCP server는 그 인증서를 참조하거나 사용하지 않으며, repository 밖 certificate store를 승인 없이 조사·삭제하지 않았다.

첫 restore에는 telemetry 안내가 표시됐고 network capture를 수행하지 않았으므로 실제 telemetry 전송 여부는 `UNKNOWN`이다. 이후 build/run 명령에는 process 범위 `DOTNET_CLI_TELEMETRY_OPTOUT=1`을 사용했다. `DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1`도 설정했지만 .NET Core 3.0 이후 지원되지 않으므로 first-run 방지 근거가 아니다. 첫 CLI 실행 전에 개발 인증서 생성을 막아야 할 때 사용할 변수는 `DOTNET_GENERATE_ASPNET_CERTIFICATE=false`다. 근거: [.NET SDK 환경 변수](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-environment-variables), [ASP.NET Core HTTPS 개발 인증서](https://learn.microsoft.com/en-us/aspnet/core/security/enforcing-ssl?view=aspnetcore-10.0).

Unity Editor, Scene, Prefab, Unity package, ProjectSettings는 이 checkpoint에서 열거나 변경하지 않았다. TLS/authentication과 score authority가 없으므로 현재 server는 loopback protocol lab으로만 분류하며 LAN/public service 검증은 `NOT RUN`이다.

WSL의 `127.0.0.1`에서 Windows `dotnet.exe` process로 접속한 첫 cross-process 시도는 연결되지 않았다. 이후 `wslinfo --networking-mode`에서 `nat`를 확인했고 이 결과는 [Microsoft WSL networking](https://learn.microsoft.com/en-us/windows/wsl/networking)의 기본 NAT 설명과 일치한다. 다만 당시 mode가 변경되지 않았다는 별도 기록은 없으므로 정확한 역사적 원인은 추론으로 남긴다. 같은 Windows host의 PowerShell `TcpClient`로 다시 실행해 정상 response를 확인했으며 실패 시도를 server PASS로 재해석하지 않았다.

## Milestone 7 Unity network client 자동 검증

실행 시점: `2026-09-05`

| 항목 | 결과 | 근거 |
|---|---|---|
| Runtime/Test compilation | PASS | exact Editor final compiler error 0 |
| EditMode regression | PASS | 20 passed / 0 failed / 0 skipped |
| Client framed protocol | PASS | submit/query JSON과 big-endian frame 확인 |
| Retry boundary | PASS | incomplete response에 250 ms 뒤 정확히 한 번 재시도 |
| Oversized response | PASS | 16 KiB 초과 길이를 payload allocation 전에 거부 |
| Server unavailable | PASS | bounded `connection_failed` 또는 `request_timeout` 반환 |
| PlayMode regression/profile | PASS | 1 passed / 0 failed / 0 skipped |
| Project validator CLI | PASS | `Arena Systems Lab validation passed.` |
| Package·Scene·Prefab·tracked ProjectSettings | UNCHANGED | hash와 Git diff 대조 |
| Actual server Game Over·Console human verification | NOT RUN at automated checkpoint | 이후 사람 검증 결과는 아래에 기록 |

첫 compile은 Unity API profile의 `TcpListener`가 `IDisposable`이 아니어서 실패했다. listener 종료를 `finally`의 `Stop()`으로 바꿨다. 이어진 두 test run은 Unity main-thread synchronization context를 동기 대기해 정지했으며 AI가 시작한 batch Editor와 worker PID만 종료했다. network test를 worker thread에서 시작하도록 수정한 뒤 최종 실행이 정상 종료됐다.

server-unavailable 첫 assertion은 즉시 `connection_failed`만 예상했지만 실제 Windows Unity에서는 bounded `request_timeout`이 반환되어 19/20으로 실패했다. 두 결과 모두 server 미실행을 나타내는 고정 code이므로 계약을 수정했고 final 20/20을 확인했다. PlayMode가 생성한 미추적 `ProjectSettings/SceneTemplateSettings.json`은 해당 실행의 부작용임을 확인한 뒤 제거했다.

### 사람 end-to-end 검증

| 검증 | 결과 | 근거 |
|---|---|---|
| Server-unavailable Game Over | PASS | 사용자 확인, unavailable 표시 후 restart 정상 |
| Actual server submit/query | PASS | `UnityPlayer` 3점 뒤 11점 최고 score 갱신 |
| Duplicate player entry | PASS | 같은 player는 한 항목, 최고 11점 유지 |
| Unity Console | PASS | 사용자 확인 Error/Exception 없음 |
| Server stderr | PASS | 검증 session 0 byte |
| Historical run persistence | NOT IMPLEMENTED | protocol v1은 player별 최고 score만 보존; MySQL milestone 예정 |

AI가 시작한 Release server는 `127.0.0.1:7777` listening을 확인한 뒤 사람 검증에 사용했다. 검증 후 target PID만 종료하고 port가 비어 있음을 확인했다. 이 session의 종료는 graceful shutdown 검사가 아니며 graceful `Ctrl+C`는 Milestone 5 CLI smoke에서 별도로 검증됐다.

최초 verification 7건은 모두 통과했지만 보안 재검토에서 서로 다른 `playerId`를 무한히 누적할 수 있는 전체 state 상한 누락을 발견했다. stored player 10,000개 제한과 capacity 검사를 추가한 뒤 final verification 8/8을 다시 실행했다.

첫 Windows PowerShell inline client 명령은 Bash와 PowerShell 사이 JSON quote가 손실되어 parser error로 종료됐고 network request는 전송되지 않았다. `ConvertTo-Json`으로 quote 경계를 제거한 다음 동일 Windows host에서 재실행해 health response를 확인했다.

## Milestone 8 Unreal C++ 후속 감사와 자동 검증

실행 시점: `2026-09-05`

Milestone 7 통합 뒤 clean `main`에서 Unreal project 유무, exact Engine, C++ build toolchain과 실행 중 Editor를 다시 읽기 전용으로 확인했다. 기존 `.uproject`는 없었고 Unreal/Unity process도 실행 중이 아니어서 `work/unreal-arena-observer` branch에서 새 경로만 사용했다.

| 항목 | 감지 결과 | 버전/설정 | 상태 | 근거 | 필요한 조치 |
|---|---|---|---|---|---|
| Unreal Engine | 기존 설치 재사용 | 5.8.0 / CL 55116800 | READY | Launcher 설치 기록, `Build.version`, executable | 자동 upgrade 금지 |
| Unreal C++ toolchain | 기존 Visual Studio 재사용 | Community 18.9.1, MSVC 14.50 | READY | `vswhere`, 실제 UBT compile | 없음 |
| Windows SDK | 실제 compile에 선택됨 | 10.0.26100.0 | READY | UBT toolchain output | 없음 |
| Engine APIs | local header 확인 | `Sockets`, `Json`, `AHUD`, Automation | READY | 설치 Engine source와 실제 link | 외부 plugin 불필요 |
| Unreal project | 새 최소 C++ observer | `Unreal/ArenaObserver` | READY | Development Editor build·사람 HUD 검증 | 없음 |
| Wire protocol | server v1 재사용 | big-endian 4 byte, UTF-8, 최대 16 KiB | READY | C++ fixture 1/1·actual server HUD PASS | 없음 |
| Network boundary | loopback query only | `127.0.0.1:7777`, 3초 deadline | READY | server 없음·actual server native test | remote 공개 금지 |
| Generated artifacts | Git 제외 확인 | Binaries/Intermediate/Saved/DDC | READY | `git check-ignore` | source/config만 추적 |
| Android File Server token | 첫 Editor 실행에서 자동 생성 후 제거 | project plugin disabled | READY | config 재실행 검사 | token commit 금지 |
| Runtime HUD | exact Editor 사람 검증 | server 없음/정상 두 경로 | READY | unavailable·`ObserverFixture 42` 표시 PASS | 없음 |
| Unreal project log | project 오류 없음 | Error/Fatal/ensure 0 | READY | 실행 log와 사용자 PASS | Engine self-test 진단과 구분 |

### Gate 판정

**READY_WITH_GAPS**

정확한 Engine과 native toolchain으로 C++ build, protocol automation, server-unavailable 및 actual-server HUD 사람 검증이 통과했다. 외부 dependency나 project upgrade는 필요하지 않다. 전체 기술 gate는 승인 대기 중인 MySQL·SVN 때문에 `READY_WITH_GAPS`를 유지한다.

### 실행한 명령과 결과

| 명령 종류 | 목적 | 결과 |
|---|---|---|
| Git status/log와 bounded `.uproject` search | clean handoff와 기존 Unreal 충돌 확인 | `main...origin/main` clean, 기존 project 없음 |
| Launcher record·`Build.version`·Engine executable probe | exact Engine 확인 | 5.8.0 / CL 55116800 READY |
| `vswhere -requires` | Native Game/C++, x64 compiler, SDK 확인 | VS Community 18.9.1 READY |
| local Engine header/template 조회 | 5.8 socket, JSON, target/build API 확인 | 필요한 built-in API 존재 |
| `Build.bat ArenaObserverEditor Win64 Development` | C++ compile·link | final PASS |
| `UnrealEditor-Cmd` Automation | protocol fixture | 1 passed / 0 failed / 0 warnings |
| port 확인 후 `-ArenaObserverExpectNoServer` automation | native failure 경로 | PASS, 3초 bounded unavailable |
| 기존 .NET server와 `-ArenaObserverExpectServer` automation | native socket end-to-end | PASS, 1/1 |
| exact Unreal Editor와 server 미실행 Play | unavailable HUD | 사용자 PASS, Editor 응답 유지 |
| ephemeral `ObserverFixture 42` 제출 후 Play 재시작 | actual-server HUD | 사용자 PASS, Top 5 row 표시 |
| server console `Ctrl+C`와 port probe | 검증 process 정리 | graceful stop, port 7777 free |
| automation report·log·Git diff 검사 | test 결과, generated 설정과 source 경계 확인 | report errors 0, Unity 변경 0 |

첫 두 build command는 WSL이 batch를 Bash로 해석하거나 `cmd.exe` quote를 잘못 전달해 build를 시작하지 못했다. PowerShell이 batch를 명시 호출한 final command만 compile 근거로 사용했다.

첫 actual compile은 test source include 경로로 실패했고 source 이동 직후에는 UBT makefile cache가 이전 경로를 참조했다. test를 module root로 옮기고 `-NoUBTMakefiles`로 한 번 재수집한 뒤 일반 incremental build도 통과했다.

Automation startup의 Engine `UnifiedErrorTests.cpp`가 의도적으로 출력하는 error test 15줄은 project test 시작 전 발생했다. exported report의 `ArenaSystemsLab.ArenaObserver.Protocol`은 success, warnings 0, errors 0이며 project fatal/assert/ensure는 없었다. 이 Engine 진단과 사람 HUD 검증을 구분했고, actual-server 화면과 실행 log의 project 오류 없음도 확인했다.

## 2026-09-07 기술 문서 소스 대조

이번 작업은 기존 환경 재설치나 엔진 실행이 아닌 문서 전용 감사다. `fa834cf`에서 시작했으며 runtime 요구 버전과 구현 소스를 읽고 기술 문서 5종의 명세를 확인했다. 상세 판정은 [ADR 0012](adr/0012-technical-documentation-governance.md)에 있다.

| 확인 | 결과 | 근거 / 한계 |
|---|---|---|
| 시작 상태 | main, 미커밋 변경 없음 | Git status·ref |
| Unity 요구 버전 | 6000.5.1f1 | ProjectVersion.txt, 이번 설치 재감사·실행 아님 |
| 직접 패키지 선언 | Input System 1.19.0, URP 17.5.0, Test Framework 1.7.0 | manifest 읽기, 변경 없음 |
| .NET target / dependency | net10.0, 외부 NuGet 참조 없음 | csproj와 NuGet.Config |
| Unreal association | 5.8 | uproject, 이번 Engine 재실행 아님 |
| 현재 저장소 | 메모리 Dictionary, player별 최고 score | LeaderboardStore, SQL schema 없음 |
| 기존 테스트 XML | EditMode 20/20, PlayMode 1/1 | NetworkClient 결과 XML 읽기 전용 확인 |
| 기존 Unreal 보고서 | 세 실행 모두 1/1, warning/error 0 | Saved/Automation 계열 index.json |
| 기존 Windows 로그 | 빌드 성공 표식 존재 | Day4WindowsBuild.log, 새 네트워크 빌드 아님 |
| 문서 구조 | 내부 링크·JSON 예제·ADR·glossary 검사 통과 | Node 표준 API 기반 읽기 전용 검사 |
| 문서 정보 경계 | 검사 패턴 일치 0건 | 현재 Markdown만 대상, 과거 Git 이력은 범위 밖 |
| 엔진·runtime 재검증 | NOT RUN | 문서만 수정 |
| 웹 다이어그램 렌더링 | NOT RUN | 정적 도식 검토만 수행 |

| 실행한 명령 / 도구 | 목적 | 종료·결과 |
|---|---|---|
| `pwd`, `git status --short --branch` | 작업 위치·기준 상태 | 성공 |
| `rg --files`, `rg -n`, `sed -n`, `tail` | 제한된 저장소 문서·소스·package source 확인 | 조회 성공, 일부 검색은 일치 없음(exit 1) |
| `git switch -c work/technical-documentation` | 문서 작업 branch 분리 | 성공 |
| `command -v node`, `command -v mmdc` | 이미 사용 가능한 검사 도구 확인 | Node 확인, mmdc PATH 항목 없음 |
| 공식 문서 웹 조회 | 표준/API 의미 교차 확인 | ADR 0012에 출처·판정 기록 |
| Node 표준 fs/path/child_process 검사 | 링크·ADR·glossary·JSON 예제·기존 결과 집계 | 정적 검사 성공. 첫 Unreal JSON BOM 처리 실패 뒤 읽기 방식 정정으로 성공 |
| `git diff --check`, `git diff --stat` | 공백 오류·변경 경계 | 문서 diff 확인, runtime 변경 없음 |
| `git add`, `git commit`, `git push`, `git ls-remote --heads origin` | 구현·checkpoint 기록과 원격 대조 | 구현 8cabddd, checkpoint 15a01b3 push·SHA 일치 |
| `git switch main`, `git merge --no-ff work/technical-documentation`, `git push origin main` | 기본 branch 문서 통합 | merge 77435fc push·SHA 일치 |
| `gh repo view --json description,homepageUrl,visibility,defaultBranchRef` 및 topic 조회 | 저장소 소개·공개 설정 확인 | 기술과 무관한 기존 소개 확인, topic 없음 |
| `gh repo edit --description <technical-summary>` 후 재조회 | 현재 구현 요약으로 소개 정리 | 성공, PUBLIC·빈 homepage·main 유지. 문구 인수는 요약 표기 |

새로운 compile·빌드·게임 수동 PASS를 추가하지 않는다. 기존 log와 generated binary는 계속 ignored 상태이며 이번 문서 변경에 포함하지 않는다.

## 2026-09-09 기술 완결성 작업 재감사

이 절은 새 작업의 시점별 근거다. 위 2026-09-07 문서 작업 결과를 소급 변경하지 않는다. 장치 경로는 자리표시자로 치환했다.

| 항목 | 관측 | 상태·영향 |
|---|---|---|
| Git | f896940 main, 미커밋 변경 없음 | READY |
| Unity source | 6000.5.1f1 / 0d9463e84828 | READY |
| Unity Hub 목록 | 다른 patch 등록, exact version 등록 없음 | 목록만으로 부재 판정 불가 |
| 기존 설치 경로 | exact Editor executable과 ProductVersion 일치 | READY, 재사용 |
| Editor process·UnityLockfile | 실행 전 없음 | batch 검사 가능 |
| Windows .NET SDK | 10.0.401 | 과거 10.0.400과 차이, 실제 build·8/8 검사로 재검증 |
| Unity baseline | EditMode 20/20, process exit 0 | PASS, CompletionBaseline-20260909.xml |
| 의존성 확장 | MySQL·connector·SVN 설치 및 외부 쓰기 미승인 | 계획의 별도 승인 gate 유지 |

| 실제 명령 종류 | 목적 | 결과 |
|---|---|---|
| git status/log, rg, sed, tail | repo 상태·명세·소스·기존 engine 경로 | 조회 성공. 존재하지 않는 tools/.github 조회는 exit 2 |
| PowerShell Hub JSON·Test-Path·VersionInfo·Get-Process | exact Editor와 잠금 확인 | 성공. 첫 CIM filter 인용 오류는 재조회로 해결 |
| `<dotnet> --version` | SDK 실측 | 10.0.401 |
| `<dotnet> build <verification.csproj> --configuration Release --no-restore` | 변경 전 build | exit 0, warnings/errors 0 |
| `<dotnet> run --project <verification.csproj> --configuration Release --no-build --no-restore` | 변경 전 server 검사 | exit 0, 8/8 |
| exact Unity batch EditMode, 별도 testResults/logFile | 변경 전 Unity 검사 | exit 0, 20/20 |
| git switch -c work/technical-completion-design | 문서 branch 분리 | 성공 |

SDK 첫 build 출력에 HTTPS 개발 인증서 자동 생성 메시지가 있었다. 명시적 설치·신뢰 명령은 실행하지 않았으며 인증서 저장소를 읽거나 제거하지 않았다. 이는 도구 첫 실행 부작용이지 프로젝트에 HTTPS를 구현한 결과가 아니다. 이후 build에는 DOTNET_GENERATE_ASPNET_CERTIFICATE=false를 적용한다. Unity baseline 이후 tracked 파일의 자동 변경은 없었다.

## 2026-09-09 v1 보강 실행 근거

Gate는 `READY_WITH_GAPS`다. 승인된 설치 없이 기존 .NET과 exact Unity로 v1 코드를 검증했다. 현재 상태와 후속 승인은 PROCESS, 변경 이유는 [ADR 0014](adr/0014-v1-contract-hardening.md)를 따른다.

| 실제 명령 / 인수 패턴 | 목적 | 종료 상태·관측 |
|---|---|---|
| git status/log/diff, rg, sed | 분기 기준·호출자·변경 경계 확인 | 설계 checkpoint 5b29e49, 해당 시점 clean |
| git switch -c work/network-contract-hardening | 구현 작업 분리 | 성공 |
| Get-Process, Test-Path, VersionInfo | 실행 전 Editor·lock·정확한 버전 확인 | 다른 Editor 없음, lock 없음, 6000.5.1f1 revision 일치 |
| `<dotnet> build <verification.csproj> --configuration Release --no-restore` | 수정 전·후 검증 executable build | 두 실행 exit 0, warnings/errors 0 |
| `<dotnet> run --project <verification.csproj> --configuration Release --no-build --no-restore` | 회귀 재현 → 수정 확인 | 전 exit 1, 8/10; 후 exit 0, 10/10 |
| exact Unity batch EditMode | 회귀 재현 → 수정 확인 | 전 exit 2, 26/30; 후 exit 0, 30/30 |
| exact Unity batch PlayMode | 기존 gameplay 자동 회귀 | exit 0, 1/1 |
| exact Unity executeMethod ArenaProjectValidator.ValidateFromCommandLine | 실제 프로젝트 설정 검증 | exit 0, validation passed |
| XML·log 읽기, 최종 process/lock 조회 | 종료 코드와 결과 교차 확인 | XML 성공 결과 일치, 최종 Editor·lock 없음 |
| Node 표준 API 문서 검사, git diff --check | 링크·JSON·ADR·glossary·정보/변경 경계 | PASS, 문서 28개·링크 199개·JSON 9개·ADR 14개·용어 85개, 오류 0 |
| git add/commit/push, git ls-remote --heads origin | 명시적 파일만 기록·원격 대조 | 구현 c6fec8c push·SHA 일치. main은 f896940 유지 |

명령 전체 템플릿은 [DEMO_GUIDE](DEMO_GUIDE.md), 결과는 ignored `Logs/ContractHardening-*20260909.*`에 있다. .NET은 process 범위의 DOTNET_CLI_TELEMETRY_OPTOUT=1, DOTNET_GENERATE_ASPNET_CERTIFICATE=false를 적용했다. 별도 restore·외부 package 추가·engine upgrade는 없다.

Unity green 로그의 컴파일 오류는 0건이다. 재컴파일한 미변경 ArenaGame의 CS0618 API 경고, 변경 전부터 있던 라이선스 갱신 진단, validator 종료 시 외부 설정 요청 실패는 별도 환경·기존 소스 진단이다. 이력과 달리 새 GUI Console 검사는 하지 않았으므로 수동 Console은 UNKNOWN이다.

PlayMode가 새 기본 SceneTemplateSettings.json을 생성했다. 내용이 기본값이고 이번 실행 전 없던 파일임을 Git 상태·내용으로 확인한 뒤 Editor 종료 후 제거했다. 기본 파일은 다시 생성 가능하며 기존 설정의 최종 변경은 없다. 현재 Windows player·Unreal·MySQL·v2·SVN 실행과 사람 화면 검증은 NOT RUN이다.

## 2026-09-10 미완료 사유와 환경 재확인

조사 시점은 2026-09-10 KST다. 저장소 코드·문서·Git ref, Windows 명령 탐색·일반 설치 경로·관련 설치 registry·MySQL service, 로컬 Docker context만 읽었다. 사용자 홈·드라이브 전체·credential은 탐색하지 않았다. 아래는 시점별 근거이며 현재 재개 지점은 [PROCESS](../PROCESS.md#work-queue)가 기준이다.

| 항목 | 실제 관측 | 상태·해석 |
|---|---|---|
| Git 분기 전 | work/documentation-ssot 3ce607a, clean, 원격 SHA 일치 | READY. main f896940과 비교해 작업 branch에만 6개 commit, 문서 마감 branch 생성 전 수치 |
| 서버 저장소·계약 | LeaderboardStore는 Dictionary, WireProtocol.Version은 1, server csproj는 net10.0·PackageReference 없음 | MySQL·v2 미구현 확인, 기술적 불가능의 근거는 아님 |
| MySQL 명령·일반 설치 | mysql.exe·mysqld.exe 미탐지, 일반 MySQL 설치 경로·관련 registry·service 0건 | 해당 조사 범위 미탐지. 비표준·포터블 설치 UNKNOWN |
| SVN 명령·일반 설치 | svn.exe·svnadmin.exe 미탐지, TortoiseSVN·SlikSvn·VisualSVN Server 일반 경로와 관련 registry 0건 | 해당 조사 범위 미탐지. 비표준·포터블 설치 UNKNOWN |
| Docker daemon | 기본 context가 로컬 endpoint임을 확인한 뒤 Server.Version 29.7.2 응답 | READY. 초기 daemon 미응답 기록을 현재 상태로 재사용하지 않음 |
| MySQL image | mysql:* 필터 조회 0건 | 해당 태그 image MISSING. 커스텀 image·별도 DB 접속 가능성 UNKNOWN |
| Unity·Unreal·Windows·DB 실행 | 이번 조사에서 실행하지 않음 | NOT RUN. 이전 PASS는 날짜가 고정된 기록 |
| 신규 사람 확인 | 이번 변경에 대한 새 PASS 응답 없음 | 수동 검증 NOT RUN 유지, Console 현재 결과 UNKNOWN |

### 외부 명세와 프로젝트 판단 구분

| 확인한 주장 | 판정·조치 | 근거 |
|---|---|---|
| 현재 net10.0 서버에 사용할 MySQL connector가 존재한다 | 확인. 후보 2.6.2의 net10.0 대상·MIT·간접 의존성 명세가 존재함. 로컬 설치·실행 검증과는 별개 | [NuGet 배포 명세](https://www.nuget.org/packages/MySqlConnector/2.6.2) |
| SVN 실습에는 원격 서버나 실제 팀원이 반드시 필요하다 | 반증. 필요조건을 과대평가한 주장으로, 로컬 repository와 file 접근으로 개인 실습 가능. 외부 작업 공간 승인 의무는 유지 | [Apache SVN 로컬 repository 안내](https://subversion.apache.org/quick-start#setting-up-a-local-repository) |
| GitHub Release로 태그와 배포 파일을 연결할 수 있다 | 확인. 선택 가능한 배포 방법이며 이번 작업에서 release·tag·binary 업로드는 하지 않음 | [GitHub Releases](https://docs.github.com/en/repositories/releasing-projects-on-github/about-releases) |
| 모든 미완료 작업이 설치 승인 때문에 중단됐다 | 저장소 근거로 정정. 미결 계약 정리·현재 v1 사람 검사·재빌드는 DB 설치와 구분하며, v2 일괄 전환은 채택한 설계 선택 | [설계와 승인 경계](TECHNICAL_COMPLETION_DESIGN.md#6-실행-순서와-승인-gate), [실행 가이드](DEMO_GUIDE.md) |

| 실행한 명령·도구 | 목적 | 관측 결과 |
|---|---|---|
| git status/log, git ls-remote --symref origin HEAD 및 지정 branch ref, git rev-list --left-right --count main...HEAD | 분기 전 작업·원격 상태 | exit 0, 위 SHA·clean·0/6 확인 |
| rg, sed, tail | 코드·문서·승인·수동 절차 대조 | 조회 성공. 긴 출력 일부가 잘려 필요한 원문을 좁혀 재조회 |
| PowerShell Get-Command·Test-Path·Get-Service·설치 registry 필터 | MySQL/SVN 일반 설치 탐색 | exit 0, 위 미탐지 범위 확인. 개인 경로 인수는 이 기록에서 생략 |
| docker context inspect, docker version --format, docker image ls --filter reference=mysql:* | 로컬 daemon·image 확인 | exit 0, daemon 29.7.2·image 0건 |
| 공식 문서 웹 조회 | 외부 기술 주장 확인·반증 | 위 세 출처 확인, 패키지·image 다운로드 없음 |
| Node 표준 API 정적 검사, git diff --check | 문서 링크·앵커·JSON·ADR·과거 기록·변경/정보 경계 | exit 0, Markdown 28개·링크 234개·앵커 29개·JSON 9개·ADR 14개, 과거 기록 3개 보존·문서 4개만 변경 |

별도 승인 없는 설치·daemon 시작·container 생성·DB 변경·engine 실행·main 통합은 하지 않았다. 최신 Windows/Unreal build, 자동 게임 검사, 수동 화면·Console, MySQL·v2·SVN 실습은 이번 문서 작업의 실행 결과가 아니다.
