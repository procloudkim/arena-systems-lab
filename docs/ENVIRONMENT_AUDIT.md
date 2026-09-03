# Environment Audit

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
| Product Name | 목표 이름과 다름 | `My project-2026-09-03` | CONFLICT | `ProjectSettings.asset` | 별도 승인 후 변경 |
| VS Code Server | 감사 중 자동 갱신 | 새 server version 생성 | CONFLICT | `code --version` 부작용 | 추가 CLI 호출·자동 rollback 금지 |

## Gate 판정

**READY_WITH_GAPS**

정확한 Editor, Windows Mono build support, Input System, Test Framework가 있고 compile 및 Day 1 검증을 통과했다. Git repository와 `.gitignore`도 준비됐지만 Visual Studio Unity workload, 목표 프로젝트 이름, Windows player build는 미완료다. 이 항목들은 현재 구현과 검증을 차단하지 않으므로 판정은 `READY_WITH_GAPS`를 유지한다.

## 필수 조치

- Day 1 필수 검증은 완료됐다.

## 선택 조치

- initial commit 전에 전체 source와 `.meta` 목록을 사람이 검토한다.
- Visual Studio를 주 IDE로 사용할 때만 Unity workload 설치를 검토한다.
- 프로젝트 폴더 이름 정리는 Editor를 닫고 변경 이력을 확보한 뒤 별도 작업으로 수행한다.

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
