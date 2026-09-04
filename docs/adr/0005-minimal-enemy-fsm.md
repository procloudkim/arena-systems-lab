# ADR 0005: Minimal Enemy FSM

- Status: Accepted
- Date: 2026-09-04

## Context

Day 1의 `EnemyController`는 추적, 접촉 공격, 사망을 조건문으로 직접 처리해 현재 행동 상태를 외부에서 설명하거나 독립적으로 시험할 수 없었다. Day 2에는 기존 전투 loop를 유지하면서 적 상태와 전이 규칙을 명시하고 debug 표시를 제공해야 한다.

## Decision

- 적 상태는 `EnemyState` enum의 `Idle`, `Chase`, `Attack`, `Dead` 네 값으로 제한한다.
- `EnemyStateMachine`은 현재 상태와 전이 결정만 담당한다.
- 전이 우선순위는 `Dead` → 행동 불가 시 `Idle` → player 접촉 시 `Attack` → 그 외 `Chase`다.
- `Dead`는 다른 상태로 돌아가지 않는 종료 상태다.
- `EnemyController`는 Unity lifecycle, 2D 물리 이동, 충돌 접촉, damage와 상태별 debug 색상을 담당한다.
- 물리 접촉은 `OnCollisionEnter2D`, `OnCollisionStay2D`, `OnCollisionExit2D`로 추적하며 `Attack`에서만 접촉 damage를 적용한다.
- 상태가 바뀔 때만 sprite 색상과 Hierarchy 이름을 갱신하고 HUD에 색상 범례를 표시한다.
- 상태별 interface, subclass, factory는 만들지 않는다.

## Consequences

상태 규칙을 Unity 물리 callback 없이 빠르게 시험할 수 있고 현재 행동을 색상과 Hierarchy에서 확인할 수 있다. `Attack`은 임의 거리 대신 실제 collider 접촉을 뜻하므로 기존 접촉 damage 규칙과 일치한다. 적 행동 종류가 실질적으로 늘어나기 전까지는 enum 방식이 가장 작은 구조다.

## Validation

- Exact Unity Editor 6000.5.1f1 runtime/test assembly compilation: PASS
- EditMode tests: PASS, 9 passed / 0 failed / 0 skipped
- Enemy state transition tests: PASS, 4 passed
- Existing Health regression tests: PASS, 5 passed
- PlayMode FSM flow: PASS, human verification
- Unity Console after PlayMode: PASS, human verification with no errors
- Scene, Prefab, Package, ProjectSettings changes: none
- Batch shutdown diagnostics: test exit code 0 이후 Mono thread cleanup과 debugger-agent 메시지 존재, compiler/test failure 표식 없음
- Remote branch SHA: PASS, commit `8539a8b`가 `origin/work/enemy-fsm`과 일치
