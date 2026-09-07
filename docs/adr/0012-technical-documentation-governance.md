# ADR 0012: Technical Documentation Governance

- Status: Accepted
- Date: 2026-09-07
- Supersedes: ADR 0003의 문서 책임 구분 및 아래에 한정한 ADR 0006 파일명 보존 규칙

## Context

프로젝트의 구현 범위·구조·데이터·통신·실행 정보가 README와 시점별 기록에 흩어져 있었다. 일부 설명은 소스보다 앞선 계획이나 이전 검증 상태를 현재 사실처럼 읽히게 했다. 기술과 무관한 외부 목적 및 개인 식별 정보는 현재 문서에서 제외해야 한다.

## Decision

- README를 제3자의 진입점으로 두고 다음 다섯 문서를 기술 명세의 기준으로 사용한다. 이 분류는 프로젝트의 선택이며 보편적 문서 표준이라는 주장이 아니다.
  1. `docs/REQUIREMENTS.md`: 요구사항, 수용 기준, 구현 추적, 제외 범위
  2. `docs/ARCHITECTURE.md`: 프로세스·컴포넌트·수명·동시성 구조
  3. `docs/DATA_MODEL.md`: 현재 논리 ERD, 데이터 사전, 저장 수명
  4. `docs/NETWORK_SECURITY.md`: 기존 문서를 확장한 TCP 요청·응답·오류·보안 명세
  5. `docs/DEMO_GUIDE.md`: 기존 문서를 확장한 환경·명령·검증·시연 가이드
- 코드 확인 기준은 `fa834cf`, 확인일은 2026-09-07이다. 현재 진행 상태는 계속 PROCESS만 관리한다. 실행 결과에는 날짜·범위·미검증 항목을 표시한다.
- ERD는 현재 메모리 저장소의 단일 논리 엔터티만 표현한다. MySQL 테이블·키·마이그레이션을 구현된 것처럼 그리지 않는다.
- 문서 버전은 `1.0.0`부터 시작한다. 구조·책임의 비호환 변경은 major, 기술 내용 추가는 minor, 사실·링크·표현 정정은 patch로 올린다. wire protocol 및 엔진 버전과 혼동하지 않는다.
- 최신 추적 문서에는 외부 목적, 개인 계정, 연락처, 개인 장치의 절대 경로를 넣지 않는다. 예제에는 고정 테스트 ID와 상대 경로·자리표시자를 사용한다. 감사의 식별 인수는 치환 사실을 명시한다.
- ADR 0006만 `0006-technology-baseline.md`로 정리한다. 번호·결정·날짜·검증 commit은 보존하고 링크를 갱신한다. 당시 branch 표기를 치환하면서 존재하지 않은 branch를 만들거나 있었다고 기록하지 않는다. 다른 Accepted ADR의 파일명 불변 규칙은 유지한다.
- 이번 변경은 현재 문서 트리와 GitHub 소개 문구를 대상으로 한다. Git 과거 commit·작성자·remote 주소는 재작성하지 않는다. 문서 정리를 이력 전체의 정보 제거로 주장하지 않는다.
- GitHub 소개는 Unity 게임·.NET TCP 서버·Unreal 조회·테스트/도구의 실제 구현 요약으로 통일한다. 공개 범위·homepage·계정 설정은 변경하지 않는다.
- 설명에서 발견한 코드 한계는 기록하되 이 문서 작업에서 런타임을 수정하지 않는다. 새 패키지·프로그램·다이어그램 도구를 설치하지 않는다.

## Consequences

세 개의 문서를 추가하고 기존 통신·실행 문서를 재사용해 같은 명세의 중복을 줄인다. README에서 소스와 검증까지 따라갈 수 있고, 진행 기록과 구현 명세의 책임이 분리된다. 기존 ADR 0006 경로의 외부 북마크는 바뀌지만 저장소 내부 링크와 결정 번호는 유지한다.

## Fact Check

| 확인한 주장 | 판정 / 조치 | 근거 |
|---|---|---|
| ERD가 있으면 SQL 테이블이 구현돼 있어야 한다. | 부정. 현재 논리 모델과 미구현 물리 스키마를 분리해 작성 | [Mermaid ERD](https://mermaid.js.org/syntax/entityRelationshipDiagram.html), [LeaderboardStore](../../Server/ArenaSystemsLab.Server/LeaderboardStore.cs) |
| TCP send와 read의 경계가 같다. | 부정. 길이 접두사와 반복 읽기 필요성을 유지 | [RFC 9293 §2.2](https://www.rfc-editor.org/rfc/rfc9293.html#section-2.2), [ReadExactlyAsync](https://learn.microsoft.com/en-us/dotnet/api/system.io.stream.readexactlyasync?view=net-10.0) |
| 숫자 타입이 틀리면 TryGetInt32는 언제나 false만 반환한다. | 부정. Number 이외에는 예외. 현재 서버의 internal_error 분류 문제를 명시하고 코드 수정은 보류 | [Microsoft API 예외 명세](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonelement.trygetint32?view=net-10.0), [WireProtocol](../../Server/ArenaSystemsLab.Server/WireProtocol.cs) |
| JSON 역직렬화만으로 필수 필드 존재 여부가 모두 검증된다. | 부정. Unity DTO의 누락 필드·중복 ID 검증 한계를 표시. 추가 런타임 검사는 미실행 | [Unity FromJson](https://docs.unity3d.com/6000.0/Documentation/ScriptReference/JsonUtility.FromJson.html), [LeaderboardClient](../../Assets/ArenaSystemsLab/Runtime/LeaderboardClient.cs) |
| async I/O와 멀티스레딩 검증은 같은 근거다. | 부정. 비동기 대기와 실제 Thread 동시 갱신 검사를 분리 | [Microsoft async 시나리오](https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/async-scenarios), [서버 검사](../../Server/ArenaSystemsLab.Server.Verification/Program.cs) |
| Markdown에서 별도 이미지 파일 없이 구조·ERD를 표시할 수 있다. | 확인. Mermaid fenced block 사용, 최종 웹 렌더링은 별도 확인 필요 | [GitHub 다이어그램 문서](https://docs.github.com/en/get-started/writing-on-github/working-with-advanced-formatting/creating-diagrams) |

추가 소스 대조 정정: 공격은 버튼 눌림당 한 번이며 공간 해시는 게임에 연결되지 않았다. Unity 입력 코드는 액션 에셋 대신 장치를 직접 읽는다. 기존 Windows 빌드는 네트워크 기능 추가 전이며 Unreal 화면 사람 검증은 이미 기록됐다. 비활성 플러그인 목록을 EOS 전체 비활성화로 확대 해석하지 않는다. 해당 소스 링크는 각 기술 명세에 둔다.

## Validation

- 문서 링크·ADR 연속 번호/필수 section·용어 버전: PASS, 문서 25개, 링크 누락 0, ADR 0001~0012, glossary 0.10.0 / 78개
- JSON 예제: PASS, 7개 파싱. Mermaid: 3개 fenced block의 짝·도식 내용 정적 검토만 수행
- 현재 Markdown의 비기술적 목적·개인 식별 패턴: PASS, 일치 0건. Git 과거 이력까지 검사·제거한 결과가 아님
- 소스·패키지·엔진 설정 변경 경계: PASS, 변경 없음. `git diff --check` 통과
- Mermaid 웹 렌더링: NOT RUN, 새 도구 설치 없음
- Unity/.NET/Unreal compile·runtime·자동/수동 게임 검사: NOT RUN, 문서 전용 변경
- 구현 commit `8cabddd`를 원격 작업 branch에 push하고 SHA 일치를 확인했다. 후속 상태는 [PROCESS checkpoint](../../PROCESS.md)에서 관리한다.
- Main 통합: merge `77435fc` push 및 SHA 일치 확인. GitHub 소개 문구 변경 후 재조회로 기술 요약·PUBLIC·빈 homepage 유지 확인, topic 없음.
