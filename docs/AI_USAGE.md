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

## 사람이 추후 결정하거나 확인해야 할 항목

- Windows player build와 실행

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

- Windows build

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
