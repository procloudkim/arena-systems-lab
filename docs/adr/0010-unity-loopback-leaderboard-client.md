# ADR 0010: Unity Loopback Leaderboard Client

- Status: Accepted
- Date: 2026-09-05

## Context

Milestone 5에서 검증한 protocol을 실제 Unity Game Over 흐름에 연결해야 한다. 기존 game은 `ArenaGame`이 runtime bootstrap, score, Game Over와 IMGUI를 관리하며 Scene·Prefab·Unity package를 수정하지 않는 구조다.

Unity object는 main thread에서만 안전하게 변경해야 하고 socket 응답은 크기·형식·값을 신뢰할 수 없다. 현재 범위는 같은 Windows host의 loopback demo이며 account, TLS, public service는 포함하지 않는다.

## Decision

- `LeaderboardClient`는 Unity runtime이 제공하는 `TcpClient`, `NetworkStream`, `JsonUtility`만 사용하고 `127.0.0.1:7777`에 연결한다.
- server protocol v1과 동일한 4-byte big-endian frame, UTF-8 JSON, 16 KiB payload, player ID·score·query 상한을 적용한다.
- response의 version, 성공 여부, entry 값, 개수와 정렬 순서를 client에서도 검증한다. raw payload나 외부 error 문자열은 log에 남기지 않는다.
- Game Over에서 고정 demo ID `UnityPlayer`와 최종 score를 제출하고 상위 5개를 조회해 기존 IMGUI에 표시한다.
- network I/O는 비동기로 수행하고 `ArenaGame`의 `await` 이후 Unity 상태 갱신은 main thread context에서 처리한다. restart와 destroy에서는 `CancellationToken`으로 진행 중 요청을 취소한다.
- 연결·I/O·timeout·불완전 frame만 일시적 실패로 보고 전체 submit/query를 한 번 재시도한다. 최고 score 저장은 같은 값을 다시 제출해도 결과가 바뀌지 않는다.
- Game Over 뒤 남은 projectile이 적을 처치해 전송 score와 화면 score가 달라지지 않도록 사망 이후 score 증가를 중지한다.
- 한 구현뿐인 interface, 별도 service locator, networking package, Scene·Prefab 설정은 추가하지 않는다.

## Consequences

Unity gameplay와 standalone server가 실제 protocol로 연결되고, 서버가 없을 때도 게임 loop와 restart는 유지된다. client는 Game Over에만 통신하므로 매 frame network allocation이나 polling이 없다.

현재 player ID와 endpoint는 local demo용 고정값이다. 인증·score authority·TLS가 없으므로 다른 local process의 사칭을 막지 못하며 remote bind는 계속 금지한다.

Unity와 .NET server가 protocol 상수를 각각 보유한다. 현재 작은 protocol에서는 양쪽 integration fixture로 drift를 검출하며, protocol 종류가 실제로 늘어나기 전에는 code generation이나 공유 package를 추가하지 않는다.

## Validation

- Exact Unity Editor: `6000.5.1f1`
- Runtime/Test compilation: PASS, final compiler error 0
- EditMode regression: PASS, 20 passed / 0 failed / 0 skipped
- New client checks: PASS, normal framing·single retry·oversized response·server unavailable
- PlayMode regression/profile: PASS, 1 passed / 0 failed / 0 skipped
- Project validator command line: PASS
- Package, Scene, Prefab, tracked ProjectSettings changes: none
- Unity가 생성한 미추적 `ProjectSettings/SceneTemplateSettings.json`: 검사 후 제거
- 첫 compile: FAIL, Unity API profile의 `TcpListener`가 `IDisposable`이 아니어서 test teardown을 `Stop()`으로 수정
- 첫 두 network test runs: 중단, main-thread synchronization context를 동기 대기한 test 교착을 worker-thread 실행으로 수정
- Server-unavailable 첫 assertion: FAIL, 환경에 따라 `connection_failed` 대신 `request_timeout`이 반환되어 두 bounded failure를 계약으로 검증
- Actual .NET server와 Game Over 수동 flow, Unity Console 사람 검증: NOT RUN
