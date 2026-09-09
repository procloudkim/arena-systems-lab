# ADR 0013: Technical Completion Design

- Status: Accepted
- Date: 2026-09-09
- 기존 결정 보완: ADR 0006·0009·0010·0011의 향후 영속화와 version 전환

## Context

기술 baseline 9개의 완료를 위해 기존 v1 경계 검증, MySQL 영속화, SVN lab, 최신 build·연속 시연이 남아 있다. 실행 이력 저장에는 응답 유실 후 retry와 신규 실행을 구분할 식별자가 필요하다. 기준선 f896940의 protocol과 저장소는 v1·memory다.

## Decision

- [기술 완결성 설계서](../TECHNICAL_COMPLETION_DESIGN.md) 0.1.0을 채택한다. 설계 채택은 전체 구현·설치 완료가 아니다.
- 사용자 선택에 따라 MySQL 단일 모드와 세 프로그램의 v2 일괄 전환을 적용한다. memory fallback과 v1 호환 계층은 만들지 않는다.
- runId는 round별로 한 번 생성하고 retry에서 유지한다. 같은 내용의 중복은 추가 이력 없이 성공, 다른 내용은 run_conflict다.
- DB transaction과 run PK로 이력·최고 score를 함께 보존한다. 관리 행 잠금으로 기존 player 상한을 유지하고 측정 전 잠금 분할은 하지 않는다.
- 현재 v1의 숫자 타입·Unity 누락 점수·중복 ID와 spatial 결과 비교부터 보강한다. package 승인 전 v2·SQL 완료를 주장하지 않는다.
- MySQL image·connector 직접/간접 package·SVN 설치·저장소 외부 쓰기는 별도 승인 gate다.
- 설계서는 미래 변경, 기존 기술 명세 5종은 현재 구현, PROCESS는 진행 상태를 담당한다.

## Consequences

최고 점수와 이력을 구분하고 network retry의 중복 기록을 방지할 계약을 갖춘다. DB가 서버 기동 의존성이 되고 구버전 client는 함께 갱신해야 한다. 게임은 server 없이 실행되지만 전송되지 않은 offline run 저장을 보장하지 않는다. DDL 복구·실제 Thread 검사·사람 시연은 별도 검증이다.

## Validation

- 설계 선택: 사용자 확인, MySQL 단일 모드와 전체 v2 전환.
- 변경 전 baseline: exact Unity EditMode 20/20, .NET SDK 10.0.401 Release build 경고·오류 0 및 server verification 8/8 PASS.
- 문서 정적 검사: PASS, Markdown 27개·내부 링크 185개·JSON 예제 9개·ADR 13개·glossary 84개, 누락 0. git diff --check PASS. 문서 외 tracked 변경 없음.
- v2·MySQL·SVN·최종 통합 runtime: NOT RUN, 별도 승인·구현 단계.
- API·DB 사실 확인 출처와 설계 한계는 설계서의 공식 자료를 따른다.
