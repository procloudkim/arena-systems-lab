# ADR 0011: Unreal Read-only Leaderboard Observer

- Status: Accepted
- Date: 2026-09-05

## Context

포트폴리오 하한에는 Unreal Engine과 C++의 실제 구현·빌드 근거가 필요하다. 이미 검증한 protocol을 재사용하면 두 번째 게임이나 새 server를 만들지 않고도 Unity가 제출한 data를 다른 engine에서 소비하는 경계를 보여 줄 수 있다.

Unreal client도 socket response를 신뢰할 수 없고, network wait가 game thread를 막아서는 안 된다. 현재 server에는 TLS, authentication과 score authority가 없으므로 observer 역시 `127.0.0.1:7777` 밖으로 연결 범위를 넓히면 안 된다.

## Decision

- `Unreal/ArenaObserver`에 Unreal Engine 5.8 C++ project를 두고 read-only leaderboard 한 화면만 구현한다.
- project asset이나 `.umap`을 새로 만들지 않고 Engine의 `Template_Default` map, `AGameModeBase`, `AHUD`를 재사용한다.
- `FArenaLeaderboardClient`는 built-in `Sockets`와 `Json` module만 사용해 protocol v1 `get_leaderboard` 요청을 한 번 보낸다.
- 4-byte big-endian length, UTF-8 JSON, 16 KiB payload, 3초 절대 deadline, Top 5, player ID·score·정렬·중복 검증을 client 경계에도 적용한다.
- blocking 가능성이 있는 socket 작업은 Engine thread pool에서 실행하고 결과만 game thread로 전달한다. 이 작업을 별도의 multithreading 성능 근거로 주장하지 않는다.
- HUD 문구는 response 적용 시 한 번 만들고 `DrawHUD`에서는 cache된 문자열만 그린다.
- Android File Server, Fab과 Bridge Engine plugin은 이 project에서 비활성화한다. Editor가 자동 생성한 Android File Server token은 추적하지 않는다.
- 외부 plugin, marketplace asset, UMG asset, interface/factory 계층과 재시도 정책은 추가하지 않는다.

## Consequences

Unity와 Unreal이 같은 wire contract를 독립 구현으로 소비하며 protocol drift를 automation fixture로 검출할 수 있다. server가 없어도 최대 3초 뒤 unavailable 상태를 표시하고 Editor/game loop는 유지된다.

endpoint와 query limit는 현재 local demo 요구에 맞춰 code에 고정돼 있다. remote service, runtime endpoint 설정, 주기적 refresh나 submit 기능이 실제 요구가 되기 전에는 configuration layer를 만들지 않는다.

Engine 기본 `EnhancedInput`은 Editor target에서 로드되지만 observer code는 입력 API에 의존하지 않는다. 자동 91줄 config dump를 막기 위해 필요한 기본 input class 두 줄만 `DefaultInput.ini`에 고정한다.

## Validation

- Exact Unreal Engine: 5.8.0 / CL 55116800
- Visual Studio Native Game/C++ workload, MSVC 14.50, Windows SDK 10.0.26100: READY
- Development Editor C++ build: PASS
- Unreal Automation protocol-only: PASS, 1 passed / 0 failed / 0 warnings
- Native socket server-unavailable: PASS, port 부재 확인 뒤 3초 bounded failure
- Native socket actual .NET server query: PASS, 1 passed / 0 failed / 0 warnings
- Actual server shutdown: PASS, `Ctrl+C` 후 port 7777 해제
- Remote implementation commit: PASS, `20b0d55`가 `origin/work/unreal-arena-observer`와 일치
- Protocol fixture: request bytes, valid response, 잘못된 정렬, fractional score, invalid ID, duplicate player, oversized payload 검사 PASS
- Plugin 재실행 검사: Android File Server token 재생성 없음, Fab/Bridge/EOS 초기화 없음
- Server-unavailable HUD manual verification: PASS, `Connection I/O error`가 unavailable 상태로 표시되고 Editor 응답 유지
- Actual server leaderboard HUD manual verification: PASS, ephemeral `ObserverFixture 42` 표시
- Unreal Editor project error human verification: PASS, `ArenaObserver` Error/Fatal/ensure 없음
- Cross-engine evidence: Milestone 7의 Unity actual-server submit/query PASS와 이번 actual-server Unreal HUD PASS를 조합해 양쪽 protocol 경계를 확인했다. 같은 server session의 Unity 제출값을 Unreal에서 연속 시연한 것으로 주장하지 않는다.
- 첫 shell build 호출 2회: FAIL, Windows batch를 Bash 또는 잘못된 `cmd.exe` quoting으로 호출한 환경 오류
- 첫 C++ build: FAIL, 중첩 test source의 module header include 경로 오류; test를 module root로 옮긴 뒤 해결
- 이동 직후 incremental build: FAIL, 이전 source 경로를 가진 UBT makefile cache; `-NoUBTMakefiles`로 재수집 후 해결
