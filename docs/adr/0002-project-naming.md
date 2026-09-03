# ADR 0002: Project Naming

- Status: Accepted
- Date: 2026-09-04

## Context

검증된 Unity project가 생성 시점의 임시 이름 `My project-2026-09-03`을 folder와 PlayerSettings에 유지하고 있었다. 같은 Location에는 다른 불완전 폴더도 있어 상위 폴더 전체를 이동하면 사용자 데이터를 건드릴 위험이 있다.

## Decision

- 활성 project folder leaf를 `Arena Systems Lab`으로 변경한다.
- PlayerSettings의 `productName`과 `metroApplicationDescription`은 `Arena Systems Lab`으로 변경한다.
- 공백 없는 package identity가 필요한 `metroPackageName`은 `ArenaSystemsLab`으로 변경한다.
- `companyName`과 application identifier는 별도 branding 결정이므로 변경하지 않는다.
- Location container의 `My`, `My project` 폴더는 이동하거나 삭제하지 않는다.

## Consequences

Unity Hub의 기존 project 항목은 이전 경로를 가리킬 수 있어 새 경로를 다시 열어야 한다. 변경은 folder rename을 되돌리고 세 PlayerSettings 값을 이전 값으로 복원하면 rollback할 수 있다.

## Validation

- Exact Unity Editor 6000.5.1f1 import/compilation: PASS, compiler/failure marker 0
- EditMode tests: PASS, 5 passed / 0 failed / 0 skipped
- ProjectSettings diff: PASS, 승인된 naming field 3개만 변경
- Git root after folder rename: PASS
- Remote branch: push 후 ref 확인
- Windows player build: NOT RUN
