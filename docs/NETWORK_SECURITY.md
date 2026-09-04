# Network Security Baseline

> 이 문서는 `ArenaSystemsLab.Server`의 현재 위협 모델, protocol 경계, 검증 근거와 원격 공개 전 필수 조건을 기록한다.

- 조사일: 2026-09-05 KST
- 적용 범위: Milestone 5 loopback TCP leaderboard foundation
- 결정: [ADR 0009](adr/0009-loopback-first-bounded-tcp-protocol.md)
- 현재 상태: loopback 전용 구현 및 자동 검증 완료, production/public network 용도 아님

## Fact Check

| 주장 | 판정 | 근거 | 프로젝트 적용 |
|---|---|---|---|
| 한 번의 TCP `send`는 상대의 한 번의 `read`와 대응한다. | **REJECTED** | TCP는 순서 있는 byte stream이며 segment·send·read buffer 경계와 application message 경계는 연관되지 않는다. [RFC 9293](https://www.rfc-editor.org/rfc/rfc9293.html) | 4-byte length prefix 뒤 [`ReadExactlyAsync`](https://learn.microsoft.com/en-us/dotnet/api/system.io.stream.readexactlyasync?view=net-10.0)로 정확한 payload 길이를 읽는다. 한 byte씩 나눠 반환하는 stream으로 검증한다. |
| loopback에 bind하면 client 신원이 인증된다. | **REJECTED** | `IPAddress.Loopback`은 local host 주소이고 `IPAddress.Any`는 모든 interface 주소다. 이는 bind 범위의 차이일 뿐 인증 기능은 아니다. 이 결론은 API 의미에서 도출한 보안상 추론이다. [IPAddress](https://learn.microsoft.com/en-us/dotnet/api/system.net.ipaddress?view=net-10.0), [TcpListener](https://learn.microsoft.com/en-us/dotnet/api/system.net.sockets.tcplistener?view=net-10.0) | `127.0.0.1`로 노출 범위를 줄이되 모든 socket input을 불신한다. local process의 score 사칭은 현재 잔여 위험이다. |
| JSON parser만 사용하면 큰 입력에 의한 자원 고갈이 방지된다. | **REJECTED** | JSON 구현은 크기·깊이 등에 제한을 둘 수 있으며, 신뢰하지 않는 입력에는 요청 크기와 입력 기반 allocation 제한이 필요하다. [RFC 8259](https://www.rfc-editor.org/rfc/rfc8259.html), [OWASP DoS Prevention](https://cheatsheetseries.owasp.org/cheatsheets/Denial_of_Service_Cheat_Sheet.html), [CWE-400](https://cwe.mitre.org/data/definitions/400.html) | frame 16 KiB, JSON 깊이 8, 동시 client 16, 요청 5초, player 10,000개로 제한한다. |
| 공식 Unity/Unreal client가 보낸 값은 server가 신뢰해도 된다. | **REJECTED** | 신뢰 경계를 넘는 입력은 server에서 구문과 의미를 모두 검증해야 한다. [OWASP Input Validation](https://cheatsheetseries.owasp.org/cheatsheets/Input_Validation_Cheat_Sheet.html) | property allowlist, type, 문자열 문자·길이, score·query 범위를 server에서 검사한다. 다만 score의 실제 gameplay 정당성은 아직 검증하지 못한다. |
| `async` socket I/O를 쓰면 multithreading을 구현한 것이다. | **REJECTED** | I/O-bound `async`는 thread를 별도로 점유하지 않고 대기할 수 있다. CPU 병렬 실행과 asynchronous I/O는 다른 선택이다. [Microsoft async scenarios](https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/async-scenarios), [Async wrappers](https://learn.microsoft.com/en-us/dotnet/standard/asynchronous-programming-patterns/async-wrappers-for-synchronous-methods) | socket은 `async` I/O로 처리하고, shared store는 실제 `Thread` 8개가 동시에 갱신하는 별도 검사로 thread safety를 증명한다. |
| 향후 TLS를 넣을 때 특정 TLS version과 cipher를 application에 고정하는 것이 항상 안전하다. | **REJECTED** | .NET은 운영체제가 TLS version을 선택하도록 두고 protocol version과 cipher suite를 직접 고정하지 않을 것을 권장한다. [SslStream best practices](https://learn.microsoft.com/en-us/dotnet/core/extensions/sslstream-best-practices) | 원격 공개가 승인되면 `SslStream`과 OS 기본 TLS 정책을 우선 검토한다. 현재는 TLS를 구현하지 않았다. |

위 판정은 공식 표준·vendor 문서·OWASP·CWE에 근거한다. 숫자 제한값은 외부 표준이 보장하는 정답이 아니라 현재 demo의 자원 상한을 명확히 하기 위한 프로젝트 정책이다.

## Assets, Actors, Trust Boundary

보호 대상은 leaderboard 무결성, server 가용성, process memory/CPU, 운영 log와 향후 database credential이다. 현재 행위자는 server 운영자, 정상 Unity/Unreal client, 오작동하거나 악의적인 local process다.

```text
Untrusted local client
        |
        | TCP 127.0.0.1, framed UTF-8 JSON
        v
[ trust boundary ]
        |
ArenaSystemsLab.Server
        |
        v
Bounded in-memory LeaderboardStore
```

현재 server는 file, shell, reflection 기반 type 생성, database, credential을 사용하지 않는다. public network와 remote attacker는 bind 범위 밖이지만, bind 주소를 바꾸는 순간 현재 위협 모델은 무효가 된다.

## Protocol v1

한 TCP connection은 request 하나와 response 하나만 처리한 뒤 닫는다.

```text
4-byte signed length, network byte order | UTF-8 JSON payload
                 1..16384 bytes          | exact schema by request type
```

| Request | 필수 JSON property | 의미 |
|---|---|---|
| `health` | `version`, `type` | server protocol 상태 확인 |
| `submit_score` | `version`, `type`, `playerId`, `score` | player의 기존 최고 score만 갱신 |
| `get_leaderboard` | `version`, `type`, `limit` | score 내림차순, 동점 `playerId` 오름차순 조회 |

모든 request는 `version: 1`이어야 한다. 예상하지 않은 property, 중복 property, 잘못된 JSON/type/range는 거부한다. 오류 response는 payload 원문이나 내부 stack trace 없이 고정된 code만 반환한다.

## Enforced Limits

| 경계 | 현재 값 | 방어 목적 |
|---|---:|---|
| Bind address | `127.0.0.1` | 원격 interface 노출 방지 |
| Payload | 16 KiB | 길이 기반 대량 allocation 방지 |
| JSON depth | 8 | 과도한 중첩 parsing 제한 |
| `playerId` | 1~32자, ASCII 영문·숫자·`_`·`-` | 형식 혼동과 log/control 문자 차단 |
| `score` | 0~1,000,000 | domain 범위 제한 |
| Query `limit` | 1~100 | response와 sort 결과 제한 |
| Stored players | 최대 10,000 | 장기 실행 시 무한 dictionary 증가 방지 |
| Concurrent clients | 최대 16 | socket/task 동시 자원 제한 |
| Request timeout | 5초 | partial request로 slot 점유 방지 |
| Request count | connection당 1개 | connection 수명과 parser surface 최소화 |

## Threats and Controls

| 위협 | 현재 control | 잔여 위험 |
|---|---|---|
| 다른 local process의 player 사칭 | loopback 제한, 엄격한 input validation | 인증이 없어 임의 `playerId`와 허용 범위 score 제출 가능 |
| frame 분할·잘림·과대 길이 | length prefix, `ReadExactlyAsync`, 16 KiB 선검사 | 다수의 반복 연결에 대한 rate limit은 없음 |
| 느린 client의 connection 점유 | 5초 timeout, 동시 client 16, connection당 1 request | local process가 반복 재연결할 수 있음 |
| shared score race | 한정된 dictionary lock, 실제 8-thread 검사 | 최대 10,000개 전체 sort의 비용은 아직 부하 측정 전 |
| parser·log를 통한 정보 노출 | 고정 error code, payload·stack trace 미기록 | 운영 수준 audit identity와 log rate limit 없음 |
| score 변조·치팅 | type/range validation, 최고 score 규칙 | gameplay 결과 자체가 server 권위가 아니므로 의미적 치팅 방지 불가 |
| 전송 도청·변조 | loopback으로 범위 축소 | TLS와 message authentication 없음 |

보안 log는 원문 payload나 credential을 기록하지 않고 고정 code만 남긴다. 외부 문자열을 log에 넣게 되면 CR/LF 등 log injection을 제거해야 한다. [OWASP Logging](https://cheatsheetseries.owasp.org/cheatsheets/Logging_Cheat_Sheet.html)

## Remote Exposure Gate

다음 조건이 모두 설계·검증되기 전에는 bind 주소를 `Any`, LAN IP 또는 public interface로 변경하지 않는다.

1. `SslStream` 기반 TLS와 server 인증서 운영 방식
2. client/player 인증과 권한 부여
3. replay·rate limiting·abuse 대응
4. server-authoritative 또는 검증 가능한 score 규칙
5. secret 관리, 보안 log 보존, patch/incident 절차
6. 외부 관점 penetration test와 부하 한계 측정

TLS는 전송 경로를 보호하지만 client가 제출한 score의 게임상 정당성까지 증명하지는 않는다. 현재 구현은 portfolio용 local protocol lab이며 live service 보안을 주장하지 않는다.

## Reproducible Validation

project root의 PowerShell에서 Windows .NET SDK 10.0.400으로 다음 argument를 검증했다. `<dotnet>`은 감사된 `dotnet.exe` 경로로 바꾼다.

```powershell
& "<dotnet>" restore Server/ArenaSystemsLab.Server.Verification/ArenaSystemsLab.Server.Verification.csproj --configfile Server/NuGet.Config
& "<dotnet>" build Server/ArenaSystemsLab.Server.Verification/ArenaSystemsLab.Server.Verification.csproj --configuration Release --no-restore
& "<dotnet>" run --project Server/ArenaSystemsLab.Server.Verification/ArenaSystemsLab.Server.Verification.csproj --configuration Release --no-build --no-restore
```

검증 프로그램은 fragmented frame, zero/oversized frame length, strict validation과 duplicate property, bounded store, 실제 multithreaded store, loopback health, slow client timeout, concurrent clients를 검사한다. 2026-09-05 결과는 Release build 경고 0·오류 0, 8 passed·0 failed다. 실제 CLI server를 port 7777에서 실행한 뒤 같은 Windows host의 PowerShell client로 `health` response를 받고 `Ctrl+C` 정상 종료도 확인했다.
