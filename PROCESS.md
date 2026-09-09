# Arena Systems Lab Process

> 이 문서는 현재 진행 상태와 다음 재개 지점의 단일 기준(SSOT)이다. 환경·AI·ADR 문서는 역사적 근거이며 현재 상태를 이 문서와 중복 관리하지 않는다.

- Last updated: 2026-09-09 KST
- Gate: `READY_WITH_GAPS`
- Default branch: `main`
- Expected handoff state: 작업 branch와 원격 ref를 확인하고 handoff 시 clean 유지
- Active work: 기술 완결성 설계·ADR 기록, 기존 v1 검증 보강 준비
- Next task: 설계 문서 검증·commit·push 후 v1 경계와 spatial test 보강

## Session Start

모든 사람과 LLM은 다음 순서로 읽는다.

1. [AGENTS.md](AGENTS.md)
2. 이 문서
3. 현재 작업과 관련된 [ADR](docs/adr/)
4. 필요할 때만 [구현 계획](docs/IMPLEMENTATION_PLAN.md), [환경 감사](docs/ENVIRONMENT_AUDIT.md), [AI 기록](docs/AI_USAGE.md)

문서의 branch나 commit 정보보다 Git의 실제 결과를 우선한다.

```bash
git status --short --branch
git log -1 --oneline --decorate
git remote -v
```

결과가 이 문서와 다르면 구현을 시작하지 말고 차이를 먼저 기록하고 정정한다. 자동 `pull`, `rebase`, `reset`은 실행하지 않는다.

## Current State

| 구분 | 현재 상태 |
|---|---|
| Environment audit | 완료, exact Unity 6000.5.1f1 확인 |
| Day 1 vertical slice | 완료 |
| Project naming | `Arena Systems Lab`로 정리 완료 |
| GitHub | public `arena-systems-lab`, 기술 소개 정리 완료. 원격 주소는 Git 설정에서 확인 |
| Process governance | ADR 0003·0012, PROCESS 상태와 기술 명세 책임 분리 |
| Technical documentation | 요구사항·아키텍처·논리 ERD·TCP 보안 명세·실행 가이드 `1.0.0`, README에서 연결 |
| Game development glossary | `0.11.0`, 84개 용어, ADR 0004·0013 적용 |
| Technical completion design | MySQL 단일 모드·전체 v2·검증·승인 gate 설계 채택, 실제 전환은 미구현 |
| Day 2 enemy FSM | `Idle`, `Chase`, `Attack`, `Dead` 구현·수동 검증 완료 |
| Day 3 measured tooling | `SpatialHash2D` 실험, profile 기준선, project validator 자동·사람 검증 완료 |
| Day 4 build and demo | Windows Mono Development build·launch smoke·standalone 수동 flow PASS |
| Network security baseline | 공식 자료 fact check, local threat model, remote exposure gate 기록 |
| Milestone 5 server foundation | BCL-only protocol·loopback server·bounded store, verification 8/8 PASS |
| Milestone 7 Unity network client | Game Over submit·상위 5개 조회 구현, 자동 검사와 server 없음·실제 server 사람 검증 PASS |
| Milestone 8 Unreal observer | C++ Top 5 HUD, protocol fixture, server 없음·actual server 자동·사람 검증 PASS |
| Technology baseline | 9개 필수 기술과 연결 구조 확정, ADR 0006 적용 |
| Reusable extension tools | Unreal 5.8, VS Native Game/C++, Windows .NET SDK 10.0.400 |
| Approval-gated gaps | SVN MISSING, MySQL runtime MISSING, Docker image UNKNOWN |
| Integrated branch | `main`, 기술 문서 merge `77435fc`으로 통합 |
| Active branch | `work/technical-completion-design` |

Day 1에는 2D top-down 이동, 공격, 적 생성·추적, Health/Damage, 사망, Game Over, 재시작이 포함된다. Scene과 Prefab 대신 runtime bootstrap을 사용한다.

Day 2 FSM은 적의 물리 접촉 여부와 게임·사망 상태를 입력으로 사용한다. 상태가 바뀔 때만 적 색상과 Hierarchy 이름을 갱신하며 상태별 class 계층은 만들지 않는다.

최종 기술 확장 범위는 Unity, Unreal Engine, Git, SVN, MySQL, network programming, socket programming, multithreading, OOP다. 현재 Unity → C#/.NET TCP 서버 → 메모리 저장소와 Unreal 조회가 구현됐다. MySQL 영속화와 Git에서 분리한 SVN lab은 승인 대기 계획이며 현재 구현으로 표시하지 않는다.

## Validation Ledger

| 검증 | 결과 | 최근 근거 |
|---|---|---|
| 2026-09-09 변경 전 Unity baseline | PASS | exact 6000.5.1f1, EditMode 20/20, CompletionBaseline-20260909.xml |
| 2026-09-09 변경 전 server baseline | PASS | SDK 10.0.401 Release 경고·오류 0, verification 8/8 |
| 2026-09-09 설계 문서 정적 검사 | PASS | Markdown 27개·내부 링크 185개·JSON 9개·ADR 13개·glossary 84개, 누락 0, diff --check |
| Runtime/Editor/Test assembly compilation | PASS | 2026-09-05 exact Editor 6000.5.1f1, Milestone 7 batch 기록 |
| EditMode tests | PASS | 20 passed / 0 failed / 0 skipped |
| Automated PlayMode profile test | PASS | 1 passed / 0 failed / 0 skipped, 5초 sampling |
| Project validator command line | PASS | exact Editor에서 validation passed |
| Day 3 performance baseline | RECORDED | [측정 조건과 수치](docs/PERFORMANCE_BASELINE.md) |
| Day 3 human validation | PASS | 사용자 확인, Editor menu·기존 gameplay·Console checklist 완료 |
| Day 1 PlayMode manual flow | PASS | 사용자 확인 |
| Day 2 PlayMode FSM flow | PASS | 사용자 상태 색상·접촉 공격·Game Over·restart 확인 |
| Unity Console after Day 3 | PASS | 사용자 확인, 오류 없음 |
| Windows Mono Development build | PASS | 2026-09-04 네트워크 기능 전 x86-64 player, Success, 64.697초 |
| Current network-enabled Windows build | NOT RUN | 현재 소스에 대한 재빌드·standalone network 검증 대기 |
| Windows player launch smoke | PASS | 8초 process 생존, managed exception·crash 없음 |
| Windows player full manual flow | PASS | 사용자 확인, [standalone checklist](docs/DEMO_GUIDE.md)와 오류 없음 |
| .NET server Release build | PASS | SDK 10.0.400, warnings 0 / errors 0 |
| Protocol/security/thread verification | PASS | fragmented·invalid length·strict input·bounded store·8-thread·loopback·timeout·concurrency, 8/8 |
| .NET server CLI smoke | PASS | Windows port 7777 health response, `Ctrl+C` graceful shutdown |
| Unity network client checks | PASS | normal framing·single retry·oversized response·server unavailable, 4/4 |
| Milestone 7 EditMode regression | PASS | 20 passed / 0 failed / 0 skipped |
| Milestone 7 PlayMode regression | PASS | 1 passed / 0 failed / 0 skipped |
| Unity server-unavailable Game Over flow | PASS | 사용자 확인, unavailable 표시·restart·Console 오류 없음 |
| Unity ↔ actual .NET server Game Over flow | PASS | 사용자 확인, 3점→11점 최고 score 갱신·중복 player 없음·Console 오류 없음 |
| Unreal Development Editor build | PASS | exact Engine 5.8.0, MSVC 14.50, Windows SDK 10.0.26100 |
| Unreal protocol automation | PASS | request/response 경계, 1 passed / 0 failed / 0 warnings |
| Unreal server-unavailable native path | PASS | port 부재 확인, 실패 상태 검사 PASS, 코드의 deadline 3초 |
| Unreal ↔ actual .NET server native path | PASS | query 1/1, server graceful shutdown·port 해제 |
| Unreal observer HUD·project log human flow | PASS | 사용자 확인, unavailable·`ObserverFixture 42` 표시와 project 오류 없음 |
| Process/ADR static checks | PASS | 2026-09-07 문서 링크·ADR 0001~0012 연속 번호/필수 section 검사 |
| Glossary static checks | PASS | 2026-09-07 version 0.10.0, 78개 term 구조 검사 |
| Technical documentation checks | PASS | 2026-09-07 코드 대조, JSON 예제 7개, 현재 Markdown의 비기술적 목적·개인 식별 패턴 0건 |
| Documentation runtime rerun | NOT RUN | 문서 전용 변경, 기존 XML/JSON·빌드 로그만 읽기 전용 재확인 |
| Technology baseline document checks | PASS | link 0건 누락, ADR 1~6 sequence/schema, forbidden Unity source/settings 변경 0건 |
| MySQL/SVN runtime validation | NOT RUN | 설치·dependency 승인 전 planning checkpoint |

검증 세부 이력은 [환경 감사](docs/ENVIRONMENT_AUDIT.md)와 각 ADR에 보존한다.

## Work Queue

1. **현재 작업:** [기술 설계서](docs/TECHNICAL_COMPLETION_DESIGN.md)·ADR 0013 정적 검사와 기록, 기존 v1 숫자 타입·Unity 응답·spatial test 보강
2. **Approval-gated:** MySQL image·connector 직접/간접 dependency 승인 후 단일 DB store·전체 v2·runId 중복 방지 구현
3. **Approval-gated:** SVN 도구·Git 외부 작업 공간 승인 후 isolated lab
4. **최종 검증:** exact Windows 재빌드·같은 서버의 Unity 제출 → Unreal 표시·재시작 영속성, 사람 실행과 문서 최종 확인

알려진 gap:

- Windows build output과 log는 local ignored generated artifact이며 repository에 포함하지 않는다.
- 문서의 기존 PASS는 날짜가 고정된 기록이다. 2026-09-07에는 엔진·서버 runtime을 재실행하지 않았다.
- 정보 경계 정리는 현재 문서와 GitHub 소개에 한정한다. Git 과거 이력·작성자·remote 주소는 재작성하지 않았다.
- Visual Studio Unity workload는 초기 검사에서 0건이었지만 현재 작업의 차단 요소가 아니다.
- Unreal source·build·native socket test와 실제 HUD 사람 검증은 완료됐다.
- SVN client/admin과 native MySQL runtime은 확인되지 않았다. 설치·download·package 추가는 사용자 승인 전 실행하지 않는다.
- Docker client는 있으나 daemon이 꺼져 기존 `mysql:8.4` image는 확인하지 못했다.
- Unity Game Over submit/query와 Unreal actual-server Top 5는 각각 사람 검증했다. 동일 server session을 연속 시연하는 최종 demo capture는 남아 있다.
- 현재 leaderboard는 player별 최고 score만 보존한다. 과거 run 이력은 MySQL milestone에서 별도 `runs` data로 구현하며 retry 중복 방지 key가 필요하다.
- server는 loopback 전용이며 TLS, authentication, rate limiting과 server-authoritative score가 없다. remote interface 공개는 금지한다.
- `companyName`과 application identifier는 ADR 0002에 따라 별도 branding 결정 전까지 유지한다.

## Checkpoints

| ID | 결과 | Branch | Implementation commit | ADR | 검증 | 통합 상태 |
|---|---|---|---|---|---|---|
| `CP-20260904-01` | Day 1 기준선 | `main` | `51bd3a4` | [ADR 0001](docs/adr/0001-commit-push-and-adr-workflow.md) | Compile PASS, EditMode 5/5, manual PASS | Integrated |
| `CP-20260904-02` | Project naming | `work/project-naming` | `20157ea`, evidence `35544d5` | [ADR 0002](docs/adr/0002-project-naming.md) | Compile PASS, EditMode 5/5 | Integrated |
| `CP-20260904-03` | Process SSOT와 ADR governance | `work/process-governance` | `c6a7fb0` | [ADR 0003](docs/adr/0003-process-and-adr-governance.md) | Static checks PASS | Integrated |
| `CP-20260904-04` | Game development glossary `0.1.0` | `work/game-dev-glossary` | `7ac3c5d` | [ADR 0004](docs/adr/0004-game-development-glossary-governance.md) | Static checks PASS | Integrated |
| `CP-20260904-05` | 최소 Enemy FSM과 debug 표시 | `work/enemy-fsm` | `8539a8b`, evidence `b294fbb` | [ADR 0005](docs/adr/0005-minimal-enemy-fsm.md) | Compile PASS, EditMode 9/9, manual PASS | Integrated |
| `CP-20260904-06` | 필수 기술 baseline과 확장 계획 | 당시 작업 branch | `01cd869` | [ADR 0006](docs/adr/0006-technology-baseline.md) | Environment audit, document static checks PASS; runtime NOT RUN | Integrated |
| `CP-20260904-07` | 측정 기반 Day 3 자료구조·Editor Tool | `work/day3-profiling-validation` | `86e90f4`, automated evidence `b64af42`, human evidence `f17e880` | [ADR 0007](docs/adr/0007-measured-day3-tooling.md) | Compile PASS, EditMode 16/16, PlayMode 1/1, CLI PASS, human PASS | Integrated as `e205ccd` |
| `CP-20260904-08` | Windows Development build·README·demo | `work/day4-build-demo` | `fca8a38`, human evidence `f87a0d5` | [ADR 0008](docs/adr/0008-reproducible-windows-build-and-demo.md) | Compile PASS, EditMode 16/16, PlayMode 1/1, build·smoke·human PASS | Integrated as `75151b9` |
| `CP-20260905-01` | Security-first loopback TCP server foundation | `work/network-security-foundation` | `3909f6b` | [ADR 0009](docs/adr/0009-loopback-first-bounded-tcp-protocol.md) | .NET Release PASS, verification 8/8, CLI smoke PASS | Integrated as `bb3a24e` |
| `CP-20260905-02` | Unity Game Over leaderboard client | `work/unity-network-client` | `ac1f24a`, human evidence `02e472a` | [ADR 0010](docs/adr/0010-unity-loopback-leaderboard-client.md) | Compile PASS, EditMode 20/20, PlayMode 1/1, validator·server 없음·actual server·Console PASS | Integrated as `b0a3f8e` |
| `CP-20260905-03` | Unreal C++ read-only leaderboard observer | `work/unreal-arena-observer` | `20b0d55`, human evidence `724a094` | [ADR 0011](docs/adr/0011-unreal-read-only-leaderboard-observer.md) | Development Editor build, protocol·server 없음·actual server 자동·사람 검증 PASS | Integrated as `b57db33` |
| `CP-20260907-01` | README·기술 문서 5종·정보 경계 정리 | `work/technical-documentation` | `8cabddd`, checkpoint `15a01b3` | [ADR 0012](docs/adr/0012-technical-documentation-governance.md) | 문서 25개, 링크 누락 0, JSON 7개, ADR 12개, glossary 78개 PASS; runtime NOT RUN | Integrated as `77435fc` |

## ADR Index and Naming

| ADR | Status | 결정 |
|---|---|---|
| [ADR 0001](docs/adr/0001-commit-push-and-adr-workflow.md) | Accepted, partially superseded | commit/push와 검증 기록 workflow |
| [ADR 0002](docs/adr/0002-project-naming.md) | Accepted | Unity project naming |
| [ADR 0003](docs/adr/0003-process-and-adr-governance.md) | Accepted | 현재 상태 SSOT와 ADR naming governance |
| [ADR 0004](docs/adr/0004-game-development-glossary-governance.md) | Accepted | 게임 개발 용어 문서와 versioning governance |
| [ADR 0005](docs/adr/0005-minimal-enemy-fsm.md) | Accepted | enum 기반 최소 enemy FSM과 debug 표시 |
| [ADR 0006](docs/adr/0006-technology-baseline.md) | Accepted | 9개 필수 기술과 end-to-end 최소 구조 |
| [ADR 0007](docs/adr/0007-measured-day3-tooling.md) | Accepted | 측정 기준선, spatial query 실험, Editor validation |
| [ADR 0008](docs/adr/0008-reproducible-windows-build-and-demo.md) | Accepted | 재현 가능한 Windows build와 demo handoff |
| [ADR 0009](docs/adr/0009-loopback-first-bounded-tcp-protocol.md) | Accepted | loopback-first bounded TCP protocol과 remote exposure gate |
| [ADR 0010](docs/adr/0010-unity-loopback-leaderboard-client.md) | Accepted | Unity Game Over submit/query, bounded client와 single retry |
| [ADR 0011](docs/adr/0011-unreal-read-only-leaderboard-observer.md) | Accepted | Unreal C++ native Top 5 query와 read-only HUD |
| [ADR 0012](docs/adr/0012-technical-documentation-governance.md) | Accepted | 기술 문서 5종·사실/계획 분리·문서 정보 경계와 ADR 0006 파일명 정리 |
| [ADR 0013](docs/adr/0013-technical-completion-design.md) | Accepted | MySQL 단일 모드·전체 v2·runId·동시성·검증·승인 경계 설계 |

ADR 파일명은 `NNNN-short-kebab-case-title.md`, checkpoint ID는 `CP-YYYYMMDD-NN` 형식을 사용한다. 전체 규칙은 ADR 0003을 따른다.

## Session End

1. 실제로 실행한 검증과 `NOT RUN` 항목을 구분한다.
2. 현재 상태, work queue, checkpoint를 이 문서에서 갱신한다.
3. 의사결정이 있으면 새 ADR을 만들거나 기존 관련 ADR을 갱신한다.
4. 새 전문 용어가 등장했다면 [용어 백과사전](docs/GAME_DEV_GLOSSARY.md)과 version history를 갱신한다.
5. AI 작업이면 [AI 사용 기록](docs/AI_USAGE.md)을 갱신한다.
6. staged diff를 검토하고 검증 결과가 포함된 message로 commit한다.
7. 현재 branch를 push하고 local/remote SHA를 비교한다.
8. 승인된 통합 작업이면 `main`에 merge·push하고 최종 clean 상태를 확인한다.

## Document Ownership

| 문서 | 책임 |
|---|---|
| `PROCESS.md` | 현재 진행 상태, 검증 요약, 다음 작업, checkpoint |
| `AGENTS.md` | 항상 적용할 작업 규칙 |
| `README.md` | 제3자의 진입점과 기술 문서 5종 탐색 |
| `docs/REQUIREMENTS.md` | 요구사항·수용 기준·소스/검증 추적·제외 범위 |
| `docs/ARCHITECTURE.md` | 시스템·컴포넌트·상태·수명·동시성 |
| `docs/DATA_MODEL.md` | 현재 논리 ERD·데이터 사전·저장 수명·미구현 영속화 경계 |
| `docs/IMPLEMENTATION_PLAN.md` | 4일 Unity core와 필수 기술 확장 계획 |
| `docs/TECHNICAL_COMPLETION_DESIGN.md` | 향후 기술 완결성의 계약·데이터·동시성·검증·승인 설계 |
| `docs/ENVIRONMENT_AUDIT.md` | 날짜가 고정된 환경·검증 증거 |
| `docs/AI_USAGE.md` | AI 작업의 시간순 기록 |
| `docs/GAME_DEV_GLOSSARY.md` | 게임·Unity·물리·검증 용어와 project example |
| `docs/PERFORMANCE_BASELINE.md` | Day 3 측정 조건, 수치, 채택하지 않은 최적화 |
| `docs/DEMO_GUIDE.md` | 환경·실행/검증 명령·테스트 범위·수동 시연 |
| `docs/NETWORK_SECURITY.md` | TCP 요청·응답·오류·제한·위협 모델·잔여 위험 |
| `docs/adr/` | 결정의 이유, 영향, 검증 |
