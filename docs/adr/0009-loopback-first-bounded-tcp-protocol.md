# ADR 0009: Loopback-First Bounded TCP Protocol

- Status: Accepted
- Date: 2026-09-05

## Context

Milestone 5는 Unity와 Unreal이 공유할 C#/.NET leaderboard server를 시작하면서 network programming, socket programming, multithreading을 검증해야 한다. TCP는 message 경계를 제공하지 않고 socket input은 local client라도 신뢰할 수 없으므로, client를 연결하기 전에 protocol과 자원 한계를 먼저 확정해야 한다.

외부 package 설치는 승인되지 않았고 현재 Windows .NET SDK 10.0.400이 준비되어 있다. production service, remote bind, 인증, TLS, database persistence는 현재 범위가 아니다.

## Decision

- server와 독립 verification executable은 .NET 10 BCL만 사용한다. test framework나 networking package를 추가하지 않는다.
- `TcpListener`는 `IPAddress.Loopback`에만 bind한다.
- protocol v1은 4-byte big-endian 길이와 UTF-8 JSON payload를 사용한다. TCP read 경계를 message 경계로 취급하지 않는다.
- frame은 16 KiB, JSON depth는 8, 동시 client는 16, request timeout은 5초, connection당 request는 1개로 제한한다.
- `playerId`, `score`, query limit, property 집합과 protocol version을 server에서 엄격히 검증한다.
- in-memory leaderboard는 player 10,000개로 제한하고 작은 shared table 하나를 `lock` 하나로 보호한다. contention이 실제 측정될 때만 구조를 분리한다.
- `async` socket I/O는 multithreading 근거로 계산하지 않는다. verification executable에서 실제 `Thread` 8개로 shared store consistency를 별도 검증한다.
- raw payload, player ID, stack trace는 server log에 기록하지 않고 고정 error code만 기록한다.
- remote interface 공개는 TLS, 인증·권한, abuse control, server-authoritative score와 외부 검증이 준비될 때까지 금지한다.

## Consequences

client 구현 전에 언어와 engine이 공유할 수 있는 작은 protocol 경계가 생기며 partial read, malformed input, slow client와 bounded concurrency를 재현 가능하게 검사할 수 있다. BCL-only 구성이라 dependency 승인이나 network package download가 필요 없다.

현재 server는 인증·TLS·영속성이 없으며 같은 host의 다른 process가 player ID와 허용 범위 score를 사칭할 수 있다. 따라서 loopback demo와 protocol lab에는 적합하지만 production, LAN, public service로 설명하거나 배포하면 안 된다.

전체 leaderboard query는 최대 10,000개를 정렬하는 단순 구현이다. 실제 측정 전에는 cache, database index, per-player lock, worker pool을 추가하지 않는다.

## Validation

- Official security/network source review: PASS, 세부 claim과 link는 [Network Security Baseline](../NETWORK_SECURITY.md)에 기록
- Windows .NET SDK: 10.0.400
- Offline restore with cleared package sources: PASS
- Release build: PASS, warnings 0 / errors 0
- Verification executable: PASS, 8 passed / 0 failed
- Fragmented and zero/oversized frame checks: PASS
- Strict request and bounded-store checks: PASS
- Actual 8-thread shared-store check: PASS
- Loopback health, slow-client timeout, concurrent-client checks: PASS
- Actual CLI process and Windows PowerShell health client: PASS, port 7777 response 후 graceful shutdown
- Unity Scene, Prefab, Package, ProjectSettings changes: none
- Remote TLS/authentication and score authority: NOT IMPLEMENTED, remote exposure forbidden
- Remote branch SHA: PASS, implementation commit `3909f6b`가 `origin/work/network-security-foundation`과 일치
