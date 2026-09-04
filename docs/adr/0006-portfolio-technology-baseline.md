# ADR 0006: Portfolio Technology Baseline

- Status: Accepted
- Date: 2026-09-04

## Context

현재 저장소는 Unity 기반 플레이 가능한 vertical slice, OOP 책임 분리, Enemy FSM, Git 운영 근거를 갖고 있다. 포트폴리오의 최종 하한에는 Unity, Unreal Engine, Git, Apache Subversion(SVN), MySQL, network programming, socket programming, multithreading, OOP가 모두 실제 검증 근거와 함께 포함돼야 한다.

Unity 게임과 별개인 데모를 아홉 개 만드는 것은 기능 간 연결이 약하고 검증 비용만 키운다. 현재 환경에는 정확한 Unity Editor, Unreal Engine 5.8, Visual Studio Native Game toolchain, Windows .NET SDK 10.0.400, Git이 있다. SVN과 MySQL server는 확인되지 않았고 Docker Desktop client는 있으나 daemon과 기존 image는 확인하지 못했다.

사용자 결정에 따라 실제 다직군 협업과 live-service 대응 경험의 구현·주장은 이번 프로젝트 완료 기준에서 제외한다.

## Decision

하나의 end-to-end 기능으로 필수 기술을 연결한다.

1. 현재 Unity 프로젝트를 플레이 가능한 arena client의 기준 구현으로 유지한다.
2. 저장소 루트의 Unity `Assets/` 밖에 C#/.NET TCP leaderboard server를 둔다. BCL의 `TcpListener`, `TcpClient`, `NetworkStream`, `System.Text.Json`을 우선 사용한다.
3. protocol은 최대 message 크기를 제한한 4-byte big-endian length prefix와 UTF-8 JSON으로 고정한다. 최초 범위는 health check, score submit, leaderboard query 세 요청뿐이다.
4. server는 제한된 수의 client를 동시에 처리하고 공유 상태의 thread safety를 자동 test로 입증한다. asynchronous I/O만 사용했다는 이유로 multithreading을 구현했다고 주장하지 않는다.
5. MySQL 8.4 LTS에 run과 score를 저장한다. schema와 migration SQL은 추적하지만 credential과 local data volume은 추적하지 않는다.
6. Unity client는 gameplay 종료 결과를 server로 보내고 leaderboard를 받아 표시한다. socket 대기 때문에 Unity main thread를 막지 않으며 Unity object 변경은 main thread에서만 수행한다.
7. Unreal Engine 5.8 C++ `ArenaObserver`는 같은 protocol로 leaderboard를 조회하는 최소 read-only client로 만든다. 두 번째 게임이나 별도 server는 만들지 않는다.
8. Git은 이 저장소의 유일한 canonical VCS로 유지한다. SVN은 승인 후 local lab에서 trunk/branches/tags, commit, branch, merge, conflict resolution을 재현하고 명령·결과만 추적 문서로 남긴다. 활성 Git working tree 안에 nested `.svn` metadata를 만들지 않는다.
9. 최종 완료는 아홉 기술 각각에 source, test 또는 실행 log, 사람 검증 중 하나 이상의 직접 근거가 있을 때만 선언한다.

디렉터리는 필요할 때만 `Server/`, `Database/`, `Unreal/`, `docs/evidence/`를 추가한다. 빈 scaffold는 미리 만들지 않는다.

Day 1~4 Unity MVP의 package 제한은 그대로 유지한다. 이후 확장에서도 Unity package는 추가하지 않는다. SVN 설치, MySQL image download, .NET MySQL connector 추가는 각각 사용자의 명시적 승인을 받은 뒤 수행한다.

## Consequences

필수 기술이 leaderboard라는 한 경로에서 연결돼 검토자가 데이터 흐름을 재현할 수 있다. Unreal 범위를 read-only observer로 제한하고 SVN을 독립 lab으로 격리해 두 engine과 두 VCS를 억지로 동시에 운용하는 복잡성을 피한다.

MySQL wire protocol은 직접 구현하지 않는다. 권장 connector는 `MySqlConnector` 2.6.2이며 추가 전 승인이 필요하다. 대안은 Oracle `MySql.Data` 9.7.0이다. SVN은 현재 미설치이므로 해당 milestone은 승인 전까지 시작할 수 없다.

공개 배포, authentication, cloud hosting, multiplayer simulation, 실제 협업·live 대응은 이 결정의 범위가 아니다. 최초 network 통합은 loopback에서만 검증한다.

## Validation

- Unity 6000.5.1f1 project와 기존 9개 EditMode test: 이전 checkpoint에서 PASS
- Unreal Editor 5.8.0 executable과 Visual Studio Native Game/C++ component: READY
- Windows .NET SDK 10.0.400: READY
- Git 2.43.0과 public origin: READY
- SVN command, standard install path, registry probe: MISSING
- MySQL command, service, standard install path, registry probe: MISSING
- Docker client 29.7.2: READY; daemon과 MySQL image: UNKNOWN
- 문서 link, ADR schema, glossary shape 검사: 첫 commit 전에 실행
- Runtime, database, Unreal, SVN 통합 검사: NOT RUN
- Remote branch SHA: 첫 push 후 기록
