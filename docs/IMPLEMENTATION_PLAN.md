# Arena Systems Lab Implementation Plan

> Day 1~4는 Unity 게임 core를 완성하는 첫 milestone이다. 최종 포트폴리오 완료에는 [ADR 0006](adr/0006-portfolio-technology-baseline.md)의 필수 기술 확장도 모두 필요하다.

## Day 1: Playable vertical slice

### 목적

외부 asset과 새 package 없이 즉시 플레이 가능한 2D top-down 전투 loop를 만든다.

### 작업 항목

- Input System 기반 player 이동과 기본 공격
- 적 1종의 단순 추적과 주기적 생성
- 재사용 가능한 Health/Damage 처리와 중복 사망 방지
- player/enemy 사망, Game Over, restart
- 기존 `SampleScene`을 손상시키지 않는 runtime bootstrap
- Health EditMode 테스트

### 완료 기준

player가 이동·공격할 수 있고, 적이 생성·추적·피해·사망하며, player 사망 후 Game Over와 restart가 동작한다.

### 검증 방법

Unity compile 결과, EditMode Test Runner, PlayMode 수동 흐름, Console error를 확인한다.

### 의존성

Unity 6000.5.1f1, URP 17.5.0, Input System 1.19.0, Test Framework 1.7.0과 built-in 2D physics만 사용한다.

### 위험 요소

runtime bootstrap은 Scene asset 변경을 피하지만 사람의 PlayMode 확인이 필요하다.

### 현재 상태

현재 완료 상태와 다음 재개 지점은 [PROCESS.md](../PROCESS.md)를 따른다.

## Day 2: FSM and code structure

### 목적
Day 1에서 실제로 드러난 상태 전이와 책임 경계를 정리한다.

### 작업 항목

- 적 행동을 Idle/Chase/Attack/Dead 상태로 명시
- 상태 전이 조건과 debug 표시 추가
- 중복 책임과 과도한 결합만 refactor
- 상태 전이 EditMode 테스트

### 완료 기준
적 상태와 전이가 코드와 화면에서 설명 가능하고, 기존 Day 1 loop가 유지된다.

### 검증 방법
상태 전이 테스트와 PlayMode에서 상태 debug 표시를 확인한다.

### 의존성

Day 1 runtime assembly와 Test Framework만 사용한다.

### 위험 요소

한 종류의 적에 불필요한 interface/factory 계층을 만들지 않는다. 전이가 단순하면 enum과 switch로 유지한다.

## Day 3: Profiling, pooling, spatial partitioning, editor validation

### 목적
측정 결과를 근거로 병목을 고치고 반복 설정 오류를 Editor에서 조기에 찾는다.

### 작업 항목

- Unity Profiler로 CPU, allocation, object count 기준선 기록
- spawn/despawn가 병목일 때만 `UnityEngine.Pool` 적용
- 단순 scan과 `SpatialHash2D`의 correctness·query 비용 비교 실험
- 측정상 이득이 확인될 때만 spatial hash를 runtime에 연결
- 필수 reference와 arena 설정을 검사하는 최소 Editor validation menu 작성

### 완료 기준
변경 전후 profiler capture와 측정 조건이 기록되고, spatial hash 실험의 correctness와 결과가 남으며, validation 도구가 잘못된 설정을 재현 가능하게 보고한다.

### 검증 방법
동일한 enemy 수와 시간 조건으로 profiler 결과를 비교한다. spatial query는 같은 입력에 대한 단순 scan 결과와 비교하고 validation의 정상/실패 사례를 실행한다.

### 의존성

Unity Profiler, `UnityEngine.Pool`, UnityEditor API만 사용한다. Performance Testing API 직접 추가는 필요하지 않다.

### 위험 요소

측정 전에 pooling을 넣지 않는다. spatial hash는 자료구조·알고리즘 실험으로 구현하되 작은 데이터에서 손해라면 runtime에는 연결하지 않는다.

## Day 4: Tests, build, documentation, demo

### 목적
재현 가능한 검증과 채용 검토자가 빠르게 이해할 수 있는 결과물을 만든다.

### 작업 항목

- 핵심 EditMode/PlayMode regression 테스트 정리
- Windows Mono development build 생성 및 실행 확인
- profiler 근거, 설계 선택, known limitation 문서화
- 짧은 demo flow와 수동 test checklist 작성
- AI 생성 코드의 사람 검증 결과 갱신

### 완료 기준
깨끗한 환경에서 Unity 프로젝트를 열고 테스트·build·demo flow를 재현할 수 있다. 이 시점은 Unity MVP 완료이며 전체 포트폴리오 완료가 아니다.

### 검증 방법
정확한 Editor에서 전체 테스트, Windows build 실행, Console 확인, 수동 checklist를 수행한다.

### 의존성

현재 설치된 Windows Mono build support와 Test Framework를 사용한다.

### 위험 요소

Windows player build 명령과 생성된 실행 파일은 아직 검증되지 않았다.

## Milestone 5: TCP socket server and multithreading

### 목적

Unity와 Unreal이 공통으로 사용할 수 있는 최소 leaderboard 경계를 C#/.NET 표준 library로 구현하고 network, socket, multithreading 근거를 만든다.

### 작업 항목

- 4-byte big-endian length prefix와 UTF-8 JSON message 규격 작성
- `TcpListener`, `TcpClient`, `NetworkStream` 기반 loopback server와 test client 구현
- health check, score submit, leaderboard query만 지원
- 최대 message 크기, timeout, 잘못된 request 처리
- 동시 client 수를 제한하고 thread-safe shared state를 사용하는 concurrent request test

### 완료 기준

두 개 이상의 client가 동시에 접속해 request/response framing을 깨뜨리지 않고 같은 score state를 안전하게 읽고 쓴다. asynchronous I/O 실행만으로 multithreading 완료를 주장하지 않고 concurrent test와 thread-safety 근거를 남긴다.

### 검증 방법

`.NET` build와 unit test, malformed/oversized message test, loopback concurrent integration test를 실행한다.

### 의존성

확인된 Windows .NET SDK 10.0.400과 BCL만 사용한다. 이 milestone에는 NuGet package가 필요하지 않다.

### 위험 요소

network input은 신뢰 경계다. message 길이 제한과 validation을 생략하지 않으며 최초 범위는 loopback으로 제한한다.

## Milestone 6: MySQL persistence

### 목적

server의 score를 MySQL 8.4 LTS에 영속화하고 schema, query, rollback 가능한 migration 근거를 만든다.

### 작업 항목

- 최소 `players`, `runs`, `scores` schema와 versioned migration 작성
- parameterized query로 score 저장과 leaderboard 조회 구현
- local MySQL container와 test database 구성
- database unavailable, duplicate request, invalid score 처리
- schema 적용과 integration test 절차 문서화

### 완료 기준

server 재시작 뒤에도 score가 유지되고 submit/query integration test가 실제 MySQL에서 통과한다. credential은 Git에 포함되지 않는다.

### 검증 방법

schema 적용 결과, database integration test, 재시작 후 데이터 조회, Git secret/ignored-file 검사를 수행한다.

### 의존성

사용자 승인 후 Docker Official Image `mysql:8.4`와 .NET connector를 사용한다. 권장 connector는 `MySqlConnector` 2.6.2이며 대안은 Oracle `MySql.Data` 9.7.0이다. image download와 package 추가는 승인 전 실행하지 않는다.

### 위험 요소

Docker daemon과 기존 image는 아직 확인되지 않았다. local password와 data volume의 수명 주기를 명시하고 public port 노출은 하지 않는다.

## Milestone 7: Unity network client

### 목적

현재 게임의 Game Over 결과와 leaderboard를 TCP server에 연결해 Unity gameplay와 backend가 하나의 flow로 동작하게 한다.

### 작업 항목

- 기존 Game Over flow에서 score submit
- leaderboard query와 최소 화면 표시
- socket I/O와 Unity main thread object 변경 분리
- 연결 실패 시 gameplay를 유지하는 오류 표시와 재시도 1회

### 완료 기준

server가 실행 중이면 한 판의 결과가 저장·조회되고, server가 없더라도 게임 loop가 멈추거나 crash하지 않는다.

### 검증 방법

Unity EditMode test, loopback integration test, PlayMode 정상/서버 없음 흐름, Console error를 확인한다.

### 의존성

Milestone 5 protocol과 현재 Input System/Test Framework를 사용한다. Unity package를 추가하지 않는다.

### 위험 요소

Unity API는 main thread 제약이 있으므로 background callback에서 `GameObject`나 UI를 직접 변경하지 않는다.

## Milestone 8: Unreal Arena Observer

### 목적

Unreal Engine과 C++로 같은 leaderboard protocol을 소비해 engine 간 protocol 재사용과 native gameplay toolchain 경험을 증명한다.

### 작업 항목

- Unreal Engine 5.8 C++ project `ArenaObserver` 생성
- Unreal native socket API로 leaderboard query 구현
- read-only leaderboard 화면과 연결 상태 표시
- protocol fixture를 공유해 Unity/.NET 결과와 일치 확인

### 완료 기준

Unreal application이 loopback server에 접속해 Unity에서 제출된 leaderboard를 읽고, server 부재와 malformed response를 안전하게 표시한다.

### 검증 방법

Development Editor C++ build, Unreal Editor 실행, 정상/연결 실패 수동 검증, log error 확인을 수행한다.

### 의존성

확인된 Unreal Engine 5.8.0과 Visual Studio Native Game/C++ component를 재사용한다. Unreal plugin이나 marketplace asset을 추가하지 않는다.

### 위험 요소

두 번째 게임으로 범위가 커지지 않도록 observer는 read-only 한 화면으로 제한한다. 생성된 cache와 binary는 Git에서 제외한다.

## Milestone 9: SVN workflow evidence

### 목적

Git과 구분되는 centralized version control의 commit, branch, merge, conflict resolution을 재현 가능한 증거로 남긴다.

### 작업 항목

- local SVN repository에 `trunk/branches/tags` 구성
- 작은 독립 sample에서 commit과 feature branch 생성
- 의도된 text conflict를 만들고 사람이 해결한 뒤 merge
- command, revision, 결과를 sanitization한 문서로 기록

### 완료 기준

SVN revision history에 branch와 merge가 남고, conflict 해결 전후를 다른 session에서 재현할 수 있다. Git 저장소 안에는 `.svn` metadata나 SVN repository database가 추적되지 않는다.

### 검증 방법

`svn status`, `svn log`, `svn mergeinfo`, clean working copy를 확인하고 기록된 revision과 실제 결과를 대조한다.

### 의존성

현재 SVN client와 `svnadmin`은 MISSING이다. 설치 command는 사용자 승인 후에만 실행한다.

### 위험 요소

Git과 SVN을 같은 source-of-truth로 운영하지 않는다. local lab 외의 원격 server나 실제 협업자를 요구하지 않는다.

## Final Portfolio Coverage Matrix

| 필수 기술 | 구현 위치 | 완료 근거 | 현재 상태 |
|---|---|---|---|
| Unity | 현재 arena game | compile, EditMode, PlayMode, Windows build | 부분 완료 |
| Unreal Engine | `Unreal/ArenaObserver` 예정 | C++ build, server 연결, 수동 실행 | 미구현 |
| Git | repository 전체 | branch, ADR, commit, push, remote SHA | 완료 |
| SVN | local isolated lab 예정 | revision, branch, merge, conflict log | 도구 MISSING |
| MySQL | `Database/`와 server persistence 예정 | migration, integration test, restart persistence | runtime MISSING |
| Network Programming | .NET server와 두 engine client 예정 | loopback end-to-end test | 미구현 |
| Socket Programming | 공통 TCP framing 예정 | fragmentation/malformed message test | 미구현 |
| Multithreading | concurrent server request 처리 예정 | stress test와 thread-safety 근거 | 미구현 |
| OOP | Unity runtime, 이후 server/client | 책임 분리된 code와 test | Unity 범위 완료 |

최종 `DONE`은 표의 모든 행이 완료 근거를 가진 뒤에만 선언한다. 실제 협업과 live-service 대응은 사용자 결정에 따라 이 matrix에서 제외한다.
