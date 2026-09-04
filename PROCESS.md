# Arena Systems Lab Process

> 이 문서는 현재 진행 상태와 다음 재개 지점의 단일 기준(SSOT)이다. 환경·AI·ADR 문서는 역사적 근거이며 현재 상태를 이 문서와 중복 관리하지 않는다.

- Last updated: 2026-09-04 KST
- Gate: `READY_WITH_GAPS`
- Default branch: `main`
- Expected handoff state: `main...origin/main`, clean
- Active work: 없음
- Next task: Day 3 profiling 기준선 측정과 Editor validation

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
| GitHub | public `procloudkim/arena-systems-lab` |
| Process governance | 완료, ADR 0003 적용 |
| Game development glossary | `0.3.0`, 43개 용어, ADR 0004 적용 |
| Day 2 enemy FSM | `Idle`, `Chase`, `Attack`, `Dead` 구현·수동 검증 완료 |
| Portfolio technology baseline | 9개 필수 기술과 연결 구조 확정, ADR 0006 적용 |
| Reusable extension tools | Unreal 5.8, VS Native Game/C++, Windows .NET SDK 10.0.400 |
| Approval-gated gaps | SVN MISSING, MySQL runtime MISSING, Docker image UNKNOWN |
| Integrated branch | `main` |
| Runtime work in progress | 없음 |

Day 1에는 2D top-down 이동, 공격, 적 생성·추적, Health/Damage, 사망, Game Over, 재시작이 포함된다. Scene과 Prefab 대신 runtime bootstrap을 사용한다.

Day 2 FSM은 적의 물리 접촉 여부와 게임·사망 상태를 입력으로 사용한다. 상태가 바뀔 때만 적 색상과 Hierarchy 이름을 갱신하며 상태별 class 계층은 만들지 않는다.

최종 portfolio 완료 하한은 Unity, Unreal Engine, Git, SVN, MySQL, network programming, socket programming, multithreading, OOP다. Unity arena → C#/.NET TCP leaderboard server → MySQL 흐름을 만들고 Unreal C++ `ArenaObserver`가 같은 server를 읽는다. SVN은 Git과 분리된 local workflow lab으로 검증하며 실제 협업·live-service 대응은 현재 범위에서 제외한다.

## Validation Ledger

| 검증 | 결과 | 최근 근거 |
|---|---|---|
| Runtime/Test assembly compilation | PASS | exact Editor 6000.5.1f1, Day 2 변경 후 재검증 |
| EditMode tests | PASS | 9 passed / 0 failed / 0 skipped, FSM 4 + Health 5 |
| Day 1 PlayMode manual flow | PASS | 사용자 확인 |
| Day 2 PlayMode FSM flow | PASS | 사용자 상태 색상·접촉 공격·Game Over·restart 확인 |
| Unity Console after Day 2 | PASS | 사용자 확인, 오류 없음 |
| Process/ADR static checks | PASS | link, filename, required-section, duplicate-status 검사 |
| Glossary static checks | PASS | version 0.3.0, 43개 heading, entry shape 검사 |
| Portfolio baseline document checks | PASS | link 0건 누락, ADR 1~6 sequence/schema, forbidden Unity source/settings 변경 0건 |
| .NET/Unreal/MySQL/SVN runtime validation | NOT RUN | 구현·승인 전 planning checkpoint |
| Windows player build | NOT RUN | Day 4 예정 |

검증 세부 이력은 [환경 감사](docs/ENVIRONMENT_AUDIT.md)와 각 ADR에 보존한다.

## Work Queue

1. **Next — Day 3:** profiling 기준선, `SpatialHash2D` correctness/비용 비교, 최소 Editor validation
2. **Then — Day 4:** PlayMode regression, Windows Mono build·실행, `README.md`, demo 문서
3. **Required extension — Milestone 5:** BCL 기반 TCP socket server와 concurrent/thread-safety test
4. **Approval-gated — Milestone 6·9:** MySQL persistence와 isolated SVN workflow lab
5. **Required extension — Milestone 7·8:** Unity network client와 Unreal C++ `ArenaObserver`

알려진 gap:

- Windows player build는 아직 실행하지 않았다.
- Visual Studio Unity workload는 없지만 현재 작업의 차단 요소가 아니다.
- Unreal Editor와 Native Game/C++ toolchain은 있으나 `.uproject`와 Unreal build 결과는 없다.
- SVN client/admin과 native MySQL runtime은 확인되지 않았다. 설치·download·package 추가는 사용자 승인 전 실행하지 않는다.
- Docker client는 있으나 daemon이 꺼져 기존 `mysql:8.4` image는 확인하지 못했다.
- network/socket/multithreading source와 test는 아직 없다.
- `companyName`과 application identifier는 ADR 0002에 따라 별도 branding 결정 전까지 유지한다.
- Location container의 불완전한 `My`, `My project` 폴더는 사용자 데이터 보호를 위해 건드리지 않는다.

## Checkpoints

| ID | 결과 | Branch | Implementation commit | ADR | 검증 | 통합 상태 |
|---|---|---|---|---|---|---|
| `CP-20260904-01` | Day 1 기준선 | `main` | `51bd3a4` | [ADR 0001](docs/adr/0001-commit-push-and-adr-workflow.md) | Compile PASS, EditMode 5/5, manual PASS | Integrated |
| `CP-20260904-02` | Project naming | `work/project-naming` | `20157ea`, evidence `35544d5` | [ADR 0002](docs/adr/0002-project-naming.md) | Compile PASS, EditMode 5/5 | Integrated |
| `CP-20260904-03` | Process SSOT와 ADR governance | `work/process-governance` | `c6a7fb0` | [ADR 0003](docs/adr/0003-process-and-adr-governance.md) | Static checks PASS | Integrated |
| `CP-20260904-04` | Game development glossary `0.1.0` | `work/game-dev-glossary` | `7ac3c5d` | [ADR 0004](docs/adr/0004-game-development-glossary-governance.md) | Static checks PASS | Integrated |
| `CP-20260904-05` | 최소 Enemy FSM과 debug 표시 | `work/enemy-fsm` | `8539a8b`, evidence `b294fbb` | [ADR 0005](docs/adr/0005-minimal-enemy-fsm.md) | Compile PASS, EditMode 9/9, manual PASS | Integrated |
| `CP-20260904-06` | Portfolio 필수 기술 baseline과 확장 계획 | `work/portfolio-technology-baseline` | `01cd869` | [ADR 0006](docs/adr/0006-portfolio-technology-baseline.md) | Environment audit, document static checks PASS; runtime NOT RUN | Integrated |

## ADR Index and Naming

| ADR | Status | 결정 |
|---|---|---|
| [ADR 0001](docs/adr/0001-commit-push-and-adr-workflow.md) | Accepted, partially superseded | commit/push와 검증 기록 workflow |
| [ADR 0002](docs/adr/0002-project-naming.md) | Accepted | Unity project naming |
| [ADR 0003](docs/adr/0003-process-and-adr-governance.md) | Accepted | 현재 상태 SSOT와 ADR naming governance |
| [ADR 0004](docs/adr/0004-game-development-glossary-governance.md) | Accepted | 게임 개발 용어 문서와 versioning governance |
| [ADR 0005](docs/adr/0005-minimal-enemy-fsm.md) | Accepted | enum 기반 최소 enemy FSM과 debug 표시 |
| [ADR 0006](docs/adr/0006-portfolio-technology-baseline.md) | Accepted | 9개 필수 기술과 end-to-end 최소 구조 |

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
| `docs/IMPLEMENTATION_PLAN.md` | 4일 Unity core와 필수 portfolio 확장 계획 |
| `docs/ENVIRONMENT_AUDIT.md` | 날짜가 고정된 환경·검증 증거 |
| `docs/AI_USAGE.md` | AI 작업의 시간순 기록 |
| `docs/GAME_DEV_GLOSSARY.md` | 게임·Unity·물리·검증 용어와 project example |
| `docs/adr/` | 결정의 이유, 영향, 검증 |
