# ADR 0004: Game Development Glossary Governance

- Status: Accepted
- Date: 2026-09-04

## Context

프로젝트 개발 중 FSM, Unity lifecycle, 2D physics, profiling 같은 기본 용어가 계속 등장한다. 설명이 대화에만 남으면 다음 session에서 재사용할 수 없고 용어 이해와 실제 코드 사이의 연결도 사라진다.

## Decision

- [Game Development Glossary](../GAME_DEV_GLOSSARY.md)를 용어 정의의 단일 기준으로 사용한다.
- 영어 canonical term을 heading으로 쓰고 한국어 설명, 프로젝트 예시, 주의점을 기록한다.
- 코드·계획·검증에서 처음 등장한 용어만 같은 작업 checkpoint에 추가한다.
- 이미 정의된 용어는 중복 설명하지 않고 glossary를 참조한다.
- category 안에서는 개념 흐름에 맞춰 배치하고 검색 가능한 영어 이름을 유지한다.
- 문서 version은 semantic versioning을 사용한다.
  - `MAJOR`: 항목 구조나 분류의 비호환 변경
  - `MINOR`: 새 용어나 분류 추가
  - `PATCH`: 정의·예시·오탈자·링크 수정
- version 변경은 `Version History`, `PROCESS.md`, AI 작업 기록과 같은 commit에 반영한다.

## Consequences

사람과 LLM이 같은 용어 정의와 프로젝트 예시를 공유할 수 있다. 모든 게임 개발 용어를 미리 채우지 않고 실제 개발 범위에 맞춰 증가시키므로 문서가 불필요하게 비대해지는 것을 막는다.

## Validation

- Initial glossary version: `0.1.0`
- Required entry shape: definition, project example, caution
- Markdown links and heading uniqueness: PASS
- Term count: 28
- Runtime/Unity tests: NOT RUN, documentation-only change
- Remote branch SHA: checkpoint commit에서 기록
