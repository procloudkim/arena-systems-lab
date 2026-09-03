# ADR 0001: Commit, Push, and Decision Log Workflow

- Status: Accepted
- Date: 2026-09-04

## Context

작업 결과와 주요 분기점을 사람과 LLM이 다음 session에서도 정확히 복원할 수 있어야 한다. 로컬 변경만 남기거나 검증 상태를 commit과 분리하면 구현 근거와 실패 이력이 유실될 수 있다.

## Decision

- 최초 검증 기준선은 `main`에 commit하고 push한다.
- 이후 주요 작업은 `work/<short-kebab-topic>` branch에서 수행하고 해당 branch를 push한다.
- 의미 있는 변경마다 이 디렉터리의 관련 ADR을 새로 만들거나 갱신한다.
- commit message에는 변경 목적과 실제 실행한 검증 결과를 기록한다.
- 작업 turn은 staged diff 검토, commit, push, remote ref 확인으로 끝낸다.
- 실행하지 않은 검증은 `NOT RUN`, 확인할 수 없는 항목은 `UNKNOWN`으로 남긴다.
- credential, token, Unity 생성물은 commit하지 않는다.
- history를 훼손하는 reset, rebase, force push는 사용하지 않는다.

## Consequences

원격 branch와 ADR만 읽어도 변경 의도, 검증 수준, 다음 작업 기준점을 확인할 수 있다. 작은 문서 수정도 작업 단위가 끝나면 commit과 push가 필요하지만, architecture 결정이 없는 경우 불필요한 새 ADR 대신 현재 관련 ADR의 기록을 갱신한다.

## Initial Checkpoint Evidence

- Unity Editor/runtime/test assembly compilation: PASS
- EditMode tests: PASS, 5 passed / 0 failed / 0 skipped
- PlayMode manual verification: PASS, 사용자 확인
- Unity Console: PASS, 사용자 확인
- Windows player build: NOT RUN
- Package, Project Settings, Scene, Prefab 변경: 없음
