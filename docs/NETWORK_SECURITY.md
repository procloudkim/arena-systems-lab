# TCP 프로토콜 및 보안 명세

- 문서 버전: `1.0.0` / wire protocol: `1` (서로 다른 버전)
- 최초 보안 조사: 2026-09-05 KST / 코드 재확인: 2026-09-07 KST
- 구현 기준: `fa834cf`의 [WireProtocol](../Server/ArenaSystemsLab.Server/WireProtocol.cs), [LeaderboardServer](../Server/ArenaSystemsLab.Server/LeaderboardServer.cs)
- 관련 결정: [ADR 0009](adr/0009-loopback-first-bounded-tcp-protocol.md), [ADR 0010](adr/0010-unity-loopback-leaderboard-client.md), [ADR 0011](adr/0011-unreal-read-only-leaderboard-observer.md)

## 적용 범위와 신뢰 경계

현재 구현은 동일 컴퓨터의 IPv4 TCP `127.0.0.1:7777`를 사용한다. HTTP·REST·WebSocket 서비스가 아니다. 서버 CLI는 `--port`로 포트를 바꿀 수 있지만 Unity 게임과 Unreal observer는 7777을 사용하므로 기본 실행에서는 변경하지 않는다.

보호 대상은 점수 무결성, 서버 가용성, 메모리·CPU, 로그다. 모든 클라이언트 입력을 신뢰 경계 밖의 데이터로 다룬다. 로컬 주소에 bind해도 클라이언트 신원이 인증되지는 않는다. 이는 [IPAddress.Loopback](https://learn.microsoft.com/en-us/dotnet/api/system.net.ipaddress.loopback?view=net-10.0)의 주소 범위와 현재 인증 코드 부재에서 도출한 결론이다.

## 프레임과 연결 수명

```text
[4바이트: big-endian signed int32 N][N바이트: UTF-8 JSON]
N = 1..16384, 접두사 4바이트는 N에 포함하지 않음
연결당 요청 1개 → 응답 1개 → 서버 연결 종료
```

TCP는 메시지 경계가 아닌 바이트 스트림을 제공한다. 한 번의 send와 read가 대응한다고 가정하지 않는다. 서버는 `ReadExactlyAsync`, 클라이언트는 반복 읽기로 접두사와 본문을 채운다. [RFC 9293 §2.2](https://www.rfc-editor.org/rfc/rfc9293.html#section-2.2), [Stream.ReadExactlyAsync](https://learn.microsoft.com/en-us/dotnet/api/system.io.stream.readexactlyasync?view=net-10.0)

아래 JSON 예제는 본문만 보여 준다. TCP에 JSON 텍스트만 보내면 올바른 요청이 아니다. 속성 순서는 의미가 없으며 이름과 문자열 비교는 대소문자를 구분한다.

## 요청과 정상 응답

### health

```json
{"version":1,"type":"health"}
```

```json
{"version":1,"ok":true,"type":"health"}
```

프로세스가 protocol v1 요청을 처리하는지 확인한다. 데이터베이스·다른 클라이언트의 상태를 검사하지 않는다.

### submit_score

```json
{"version":1,"type":"submit_score","playerId":"UnityPlayer","score":11}
```

```json
{"version":1,"ok":true,"bestScore":11}
```

기존 최고 점수가 더 크면 `bestScore`는 제출값보다 클 수 있다. 플레이 이력을 추가하는 API가 아니다. 저장 규칙은 [데이터 모델](DATA_MODEL.md)을 따른다.

### get_leaderboard

```json
{"version":1,"type":"get_leaderboard","limit":5}
```

```json
{"version":1,"ok":true,"entries":[{"playerId":"UnityPlayer","score":11}]}
```

빈 서버는 `entries: []`를 반환한다. 배열 길이는 limit 이하이며 점수 내림차순, 동점은 ID Ordinal 오름차순이다. 서버는 최대 100개를 허용하지만 게임과 observer는 5개를 요청한다.

## 서버 입력 제한

| 경계 | 규칙 |
|---|---|
| 루트 | JSON object, 깊이 최대 8, 주석·trailing comma 금지 |
| 공통 필드 | 정수 `version=1`, 문자열 `type` |
| 정확한 속성 집합 | health 2개, submit_score 4개, get_leaderboard 3개. 누락·추가·중복 속성 거부 |
| playerId | 길이 1~32, ASCII 영문·숫자·밑줄·하이픈 |
| score | int32로 표현 가능한 정수 중 0~1,000,000 |
| limit | 정수 1~100 |
| 요청·응답 본문 | 최대 16 KiB |
| 저장된 ID | 기본 최대 10,000개, 기존 ID 갱신은 용량이 차도 가능 |
| 동시 처리 | 기본 최대 16개 클라이언트, 소켓 대기열과 OS 자원까지 총 16개라는 의미는 아님 |
| 처리 시간 | 연결 처리 시작부터 읽기·정상 응답까지 기본 5초 취소 타이머 |
| 오류 응답 | 가능한 경우 별도 최대 1초 전송 시도, 전달 보장 없음 |

이 수치는 프로젝트의 정책이며 표준이 정한 보안 보증값이 아니다. JSON 파서 자체만으로 모든 자원 고갈을 방지하지 못하므로 길이·깊이·개수 제한을 함께 적용한다. [RFC 8259 §9](https://www.rfc-editor.org/rfc/rfc8259.html#section-9), [OWASP 입력 검증](https://cheatsheetseries.owasp.org/cheatsheets/Input_Validation_Cheat_Sheet.html)

## 오류 계약과 알려진 한계

연결이 쓰기 가능한 경우 다음 형식의 고정 코드 응답을 시도한다.

```json
{"version":1,"ok":false,"error":"invalid_score"}
```

| error | 코드상 발생 조건 |
|---|---|
| `invalid_frame_length` | 접두사가 0 이하 또는 16 KiB 초과 |
| `incomplete_frame` | 접두사·본문을 채우기 전 EOF |
| `invalid_json` | JSON 문법·깊이 등 JsonException |
| `invalid_request` | 루트/필수 필드/속성 수/필드 표현 검사 실패. 아래 숫자 타입 예외는 별도 |
| `duplicate_property` | 같은 이름의 속성이 두 번 등장 |
| `unsupported_version` / `unsupported_type` | 지원하지 않는 버전 / 요청 종류 |
| `invalid_player_id` | ID 길이·문자 규칙 위반 |
| `invalid_score` / `invalid_limit` | 정수 파싱 뒤 도메인 범위 위반 |
| `leaderboard_capacity_reached` | 저장소가 가득 찬 상태에서 새 ID 제출 |
| `response_too_large` | 직렬화된 응답이 16 KiB 초과 |
| `internal_error` | 위 분류 밖의 예외 |

`request_timeout`, `connection_io_error`, `socket_error`는 서버 로그 코드이며 해당 catch 경로는 JSON 오류 응답을 보내지 않는다. 클라이언트는 연결 종료·취소·부분 응답도 실패로 처리해야 한다. 로그에는 고정 코드와 일반 예외 타입만 남기며 원문 요청·스택 추적을 출력하지 않는다.

숫자 타입 오류 분류의 알려진 문제: `ReadRequiredInt`는 `ValueKind` 검사 없이 `TryGetInt32`를 호출한다. `score: "11"`처럼 Number가 아닌 값은 InvalidOperationException을 일으켜 서버의 일반 예외 처리로 이동하고 `internal_error` 응답을 시도한다. “모든 잘못된 숫자 타입이 invalid_request가 된다”는 설명은 틀리다. API 명세와 코드 경로로 확인했으며 이 문서 작업에서는 런타임 재현·코드 수정을 수행하지 않았다. [Microsoft TryGetInt32 예외 명세](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonelement.trygetint32?view=net-10.0)

## 클라이언트별 동작 차이

| 항목 | Unity LeaderboardClient | Unreal FArenaLeaderboardClient |
|---|---|---|
| 요청 | submit 후 query, 각각 새 연결 | query 한 번 |
| 시간 제한 | 각 요청마다 기본 3초 | Fetch 작업의 기본 3초 절대 deadline |
| 재시도 | 지정된 일시 오류에 전체 submit/query 흐름을 최대 1회 재시도, 250ms 간격 | 없음 |
| 응답 검사 | 버전·ok·길이·ID·점수 범위·정렬·제출값 이상 bestScore | 버전·ok·길이·정수 타입·ID·범위·정렬·중복 ID |
| JSON 처리 | strict UTF-8 디코딩 후 JsonUtility | Unreal Json 모듈 |
| 수명 | R 재시작·ArenaGame 파괴 시 취소 | 작업 완료 후 weak HUD 참조 확인 |
| 표시 | Game Over HUD | Play 시작 후 조회 전용 HUD, Play 재시작으로 갱신 |

Unity 재시도 대상은 `connection_failed`, `connection_io_error`, `request_timeout`, `incomplete_frame`이다. 두 단계와 재시도가 있으므로 전체 Game Over 네트워크 흐름을 “3초 이내 완료”라고 보장하지 않는다. Unreal deadline도 OS 스케줄링까지 보장하는 실시간 시스템의 상한은 아니다.

Unity 응답 검사에는 중복 playerId 검사가 없다. 또한 `JsonUtility`는 필드의 존재 여부 자체를 모두 검사하는 스키마 검증기가 아니며, 현재 DTO의 누락된 숫자 필드는 기본값과 구분되지 않을 수 있다. 이 누락·중복 경계의 추가 회귀 검사는 미실행이다. Unreal도 미지정 속성·중복 JSON 속성 전체를 검사하는 엄격한 서버 파서와 동일하다고 주장하지 않는다. [Unity JsonUtility.FromJson](https://docs.unity3d.com/6000.0/Documentation/ScriptReference/JsonUtility.FromJson.html) 참조는 Unity 6 계열의 API 의미 확인용이며 정확한 6000.5 실행 검증을 대신하지 않는다.

## 잔여 위험과 원격 공개 게이트

| 잔여 위험 | 현재 상태 |
|---|---|
| ID 사칭·조작된 점수 | 인증과 게임 결과 검증 없음. 형식에 맞는 임의 점수 제출 가능 |
| 반복 연결·요청 남용 | 연결 수와 시간 제한만 있음. 요청 빈도 제한 없음 |
| 도청·변조 | TLS 없음. loopback은 암호화나 인증이 아님 |
| 장기 보관·복구 | 메모리 저장만 있음 |
| 전체 정렬·단일 lock 비용 | 실제 부하 한계 미측정 |
| 잘못된 입력의 오류 일관성 | 숫자 타입 예외 분류와 클라이언트 검증 차이가 남음 |

TLS·인증/권한·replay 및 rate limiting·점수 정당성 검증·비밀 관리·보안 로그·부하/보안 검증을 설계하고 승인하기 전에는 bind 주소를 LAN이나 공개 인터페이스로 바꾸지 않는다. 현재 구현은 로컬 프로토콜 실험이며 운영 서비스의 보안 수준을 보장하지 않는다.

## 재현 가능한 검사

실행 명령은 [실행 및 검증 가이드](DEMO_GUIDE.md)에만 관리한다. 서버 검증 실행 파일은 프레임 분할·길이 오류·요청 경계·저장소 용량·8-thread 갱신·loopback·timeout·동시 연결을 검사한다. 과거 결과와 새 실행 결과를 구분하며 보안 검사가 모든 공격을 다뤘다고 해석하지 않는다.
