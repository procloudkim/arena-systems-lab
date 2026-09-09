# ADR 0014: V1 Contract Hardening

- Status: Accepted
- Date: 2026-09-09
- 보완: ADR 0007·0009·0010·0012, ADR 0013의 실행 순서 2

## Context

숫자가 아닌 JSON 값이 서버의 공통 정수 읽기 함수에서 일반 예외로 분류됐다. Unity는 누락 점수를 기본 0과 구분하지 못했고 배열의 중복 ID도 거부하지 않았다. 공간 검색 실험은 query별 ID가 아닌 match 합계만 비교했다. 이 문제는 새 의존성 없이 기존 구현에서 고칠 수 있다.

## Decision

- version·score·limit의 공통 `ReadRequiredInt`에 Number 타입 검사를 추가한다. 파싱 불가 값은 invalid_request, 정수의 도메인 범위 위반은 기존 invalid_score/invalid_limit로 유지한다.
- Unity JsonUtility를 재사용하고 bestScore·entry score 초기값을 -1로 둔다. 정상 0 수용과 누락 거부를 exact Editor로 확인한다.
- 응답당 Ordinal HashSet으로 인접·비인접 중복 playerId를 거부한다. 매 frame 작업이나 새 JSON library를 추가하지 않는다.
- 기존 테스트 도구에 회귀 검사를 추가해 수정 전 실패와 수정 후 통과를 남긴다. spatial 비교는 측정 구간 밖에서 500개 query 각각의 ID와 개수를 대조한다.
- wire protocol v1·메모리 저장·기존 retry·Unreal 소스·gameplay를 유지한다. 전체 JSON schema 검사·v2·DB·SVN으로 확대하지 않는다.
- [통신 명세](../NETWORK_SECURITY.md)와 [실행 가이드](../DEMO_GUIDE.md)는 1.1.0, 요구사항은 1.0.1로 갱신한다. [glossary](../GAME_DEV_GLOSSARY.md) 0.12.0에 Hash Set을 추가한다.

## Consequences

잘못된 숫자 입력의 오류 계약과 Unity 응답 경계를 일치시키고, 검색 결과의 개수만 맞는 오류도 검출한다. 테스트에서 관측한 범위 밖의 schema 완전성·부하 안전성·성능 개선은 주장하지 않는다. 사람 검증 전 runtime 완료·main 통합은 보류한다.

## Validation

| 단계 | 결과 | 근거 |
|---|---|---|
| 수정 전 server 회귀 | FAIL, 8 PASS / 2 FAIL | Number 이외 타입 예외, 실제 소켓 응답 internal_error |
| 수정 전 Unity 회귀 | FAIL, 26 PASS / 4 FAIL | 누락 bestScore·entry score, 인접/비인접 중복 ID |
| 수정 후 server | PASS, 10/10 | SDK 10.0.401 Release 경고·오류 0, 잘못된 score 거부 후 빈 저장소 유지 |
| 수정 후 Unity EditMode | PASS, 30/30 | exact 6000.5.1f1, 누락과 정상 0 구분, ID 비교, query별 집합·경계 |
| Unity PlayMode·validator | PASS, 1/1·exit 0 | 기존 gameplay sampling과 프로젝트 validation |
| 문서 정적 검사 | PASS | Markdown 28개·링크 199개·JSON 9개·ADR 14개·glossary 85개, 정보 경계·diff 검사 |
| 화면·Console 수동 검증 | NOT RUN | 새 변경의 사람 확인 필요 |
| Windows player·Unreal 재빌드 | NOT RUN | Unreal 소스 불변, 최신 player는 후속 단계 |
| MySQL·v2·SVN | NOT RUN | 별도 승인·구현 gate |

Unity 로그의 CS0618은 변경하지 않은 ArenaGame의 API 사용 경고다. 라이선스 갱신 진단은 변경 전 baseline에도 있으며 validator 종료 시 외부 설정 요청 실패가 출력됐다. 테스트 XML과 종료 코드를 함께 확인했으며 이를 gameplay error로 단정하거나 로그 전체 무오류라고 기록하지 않는다.

검사 중 Unity가 새로 생성한 기본 SceneTemplateSettings.json은 모든 userAdded 값이 false임을 확인하고 Editor 종료 후 제거했다. 기존 설정·Scene·Prefab·package 파일의 최종 diff는 없다. 실패/성공 원본은 ignored Logs에 분리 보존하며 실행 명령은 DEMO_GUIDE, commit·remote 근거는 PROCESS에 기록한다.
