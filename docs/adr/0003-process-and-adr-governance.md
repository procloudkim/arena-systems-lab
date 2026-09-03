# ADR 0003: Process and ADR Governance

- Status: Accepted
- Date: 2026-09-04
- Supersedes: ADR 0001의 session resume source-of-truth 부분

## Context

현재 milestone, 검증 결과, 미완료 작업이 `AGENTS.md`, 구현 계획, 환경 감사, AI 기록, ADR에 나뉘어 있었다. 일부는 역사적 증거지만 현재 상태처럼 읽힐 수 있었고, 새 session이 실제 재개 지점을 판단하려면 여러 문서와 Git을 다시 조합해야 했다.

## Decision

- 루트 `PROCESS.md`만 현재 진행 상태, 검증 요약, work queue, checkpoint를 관리한다.
- 모든 session은 `AGENTS.md`, `PROCESS.md`, 관련 ADR 순서로 읽는다.
- 다른 문서는 현재 상태를 복사하지 않고 `PROCESS.md`를 참조하거나 역사적 snapshot임을 명시한다.
- ADR 파일명은 `NNNN-short-kebab-case-title.md`를 사용한다. 번호는 4자리 연속 증가하며 재사용하지 않는다.
- ADR status는 `Proposed`, `Accepted`, `Superseded`, `Rejected` 중 하나다.
- ADR에는 `Context`, `Decision`, `Consequences`, `Validation` section이 필요하다.
- Accepted ADR은 rename하거나 내용을 소급해 지우지 않는다. 결정 변경은 다음 번호 ADR로 supersede하고 이전 ADR에 참조를 추가한다.
- checkpoint ID는 `CP-YYYYMMDD-NN`을 사용하며 날짜별 순번을 증가시킨다.
- 의미 있는 작업은 implementation commit을 먼저 push한 뒤 `PROCESS.md` checkpoint에 commit과 검증을 기록한다.

## Consequences

현재 상태는 한 파일에서 복원할 수 있고 감사·계획·AI 기록의 고유한 역사도 유지된다. 작업마다 checkpoint 갱신 commit이 하나 추가되지만 implementation SHA와 검증 결과를 자기 참조 없이 기록할 수 있다.

ADR 0001의 commit/push 안전 규칙은 계속 유효하다. 다만 branch와 ADR만으로 session을 재개한다는 전제는 이 ADR과 `PROCESS.md`가 대체한다.

## Validation

- Markdown relative links: PASS
- ADR filename and number uniqueness: PASS
- Required ADR metadata and sections: PASS
- Current-status duplicate search: PASS
- Runtime/Unity tests: NOT RUN, documentation-only change
- Remote branch SHA: PASS, commit `c6a7fb0`가 `origin/work/process-governance`와 일치
