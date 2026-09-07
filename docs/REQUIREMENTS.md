# 요구사항 명세

- 문서 버전: `1.0.0`
- 코드 확인일: 2026-09-07 KST
- 확인 기준: `fa834cf`의 소스. 최신 진행 상태와 검증 결과는 [PROCESS.md](../PROCESS.md)에서 관리한다.
- 문서 체계: [README](../README.md), [ADR 0012](adr/0012-technical-documentation-governance.md)

## 목적과 범위

Arena Systems Lab은 직접 이동·조준·발사하는 Unity 2D 생존 게임과, 같은 컴퓨터에서 점수를 교환하는 .NET 서버 및 Unreal 조회 클라이언트다. 게임의 반복 구조는 적 회피 → 공격 → 처치 점수 획득 → 사망 → 재시작이다. 재미나 완성도는 측정 결과가 아니라 이 구조의 설계 의도다.

`구현`은 해당 코드가 존재한다는 뜻이다. 자동 검사와 사람 검증의 통과 여부는 별도이며, `계획`은 현재 사용할 수 없는 기능이다.

## 기능 요구사항과 추적성

| ID | 요구사항 / 수용 기준 | 상태 | 구현 근거 | 검증 경로 |
|---|---|---|---|---|
| FR-01 | 기존 Scene 로드 후 플레이어·카메라·적 생성기를 구성하고 새 라운드를 시작한다. | 구현 | [ArenaGame](../Assets/ArenaSystemsLab/Runtime/ArenaGame.cs) | 실행 가이드의 Unity 기본 흐름 |
| FR-02 | 이동 입력의 길이를 최대 1로 제한하고 플레이어 중심을 경기장 범위 안으로 제한한다. | 구현 | [PlayerController](../Assets/ArenaSystemsLab/Runtime/PlayerController.cs) | 이동·경계 수동 검사 |
| FR-03 | 공격 버튼을 누른 순간 조준 방향으로 투사체를 발사한다. 재사용 대기 중 입력은 무시한다. | 구현 | [PlayerController](../Assets/ArenaSystemsLab/Runtime/PlayerController.cs), [Projectile](../Assets/ArenaSystemsLab/Runtime/Projectile.cs) | 발사·명중 수동 검사 |
| FR-04 | 경기장 가장자리에서 적 한 종류를 생성하고 최대 생존 적 수를 제한한다. | 구현 | [EnemySpawner](../Assets/ArenaSystemsLab/Runtime/EnemySpawner.cs) | 생성·상한 수동 검사 |
| FR-05 | 적은 플레이어를 추적하고 접촉 중 주기적으로 피해를 준다. 사망 상태는 되돌리지 않는다. | 구현 | [EnemyController](../Assets/ArenaSystemsLab/Runtime/EnemyController.cs), [EnemyStateMachine](../Assets/ArenaSystemsLab/Runtime/EnemyStateMachine.cs) | EnemyStateMachineTests 및 접촉 수동 검사 |
| FR-06 | 유효한 피해만 체력에 반영하고 체력을 0 아래로 내리지 않는다. 사망 이벤트는 한 번 발생한다. | 구현 | [Health](../Assets/ArenaSystemsLab/Runtime/Health.cs) | HealthTests |
| FR-07 | 적 사망당 1점을 얻는다. 플레이어 사망 시 조작과 적 생성을 중단하고 Game Over를 표시한다. | 구현 | [ArenaGame](../Assets/ArenaSystemsLab/Runtime/ArenaGame.cs) | 점수·사망 수동 검사 |
| FR-08 | Game Over에서 R을 누르면 이전 라운드와 네트워크 요청을 정리하고 체력 100·점수 0으로 재시작한다. | 구현 | [ArenaGame](../Assets/ArenaSystemsLab/Runtime/ArenaGame.cs) | 재시작 수동 검사 |
| FR-09 | Game Over에서 고정 ID `UnityPlayer`의 점수를 제출하고 상위 5개를 조회한다. 서버 실패는 게임 재시작을 막지 않는다. | 구현 | [LeaderboardClient](../Assets/ArenaSystemsLab/Runtime/LeaderboardClient.cs) | LeaderboardClientTests 및 서버 유무 수동 검사 |
| FR-10 | 서버는 플레이어별 최고 점수 하나만 유지하며 점수 내림차순·동점 ID 순서로 반환한다. | 구현 | [LeaderboardStore](../Server/ArenaSystemsLab.Server/LeaderboardStore.cs) | 서버 검증 프로그램 |
| FR-11 | Unreal은 Play 시작 시 Top 5를 한 번 조회하고 HUD에 결과 또는 연결 실패를 표시한다. | 구현 | [ArenaObserverHUD](../Unreal/ArenaObserver/Source/ArenaObserver/ArenaObserverHUD.cpp) | Unreal automation 및 HUD 수동 검사 |
| FR-12 | Editor 도구는 버전·활성 Scene·입력 액션을 검사하고 Windows Mono Development 빌드를 생성한다. | 구현 | [Editor 소스](../Assets/ArenaSystemsLab/Editor/) | validator 검사 및 빌드 가이드 |

## 기본 게임 파라미터

아래 값은 생성 코드의 기본값이며 외부 밸런스 데이터 파일은 없다.

| 대상 | 값 | 소스 |
|---|---|---|
| 경기장 중심 이동 범위 | X ±8.5, Y ±5 | ArenaGame |
| 플레이어 | 체력 100, 이동 속도 5 units/s | ArenaGame, PlayerController |
| 공격 | 버튼을 누를 때 1회, 대기 0.2초 | PlayerController |
| 투사체 | 피해 10, 속도 11 units/s, 수명 2초 | PlayerController, Projectile |
| 적 | 체력 20, 이동 속도 1.8 units/s | EnemySpawner, EnemyController |
| 접촉 공격 | 적 한 개 기준 피해 10, 간격 0.75초 | EnemyController |
| 적 생성 | 첫 생성 0.5초 후, 이후 1.1초 간격, 최대 40개 | EnemySpawner, ArenaGame |

공격은 누르고 있는 동안의 자동 연사가 아니다. 조준은 유효한 오른쪽 스틱 입력 → 마우스 순으로 확인하고, 사용할 방향이 없으면 마지막 조준 방향을 유지한다. 이동할 때 마지막 조준 방향도 이동 방향으로 갱신한다. 키보드와 왼쪽 스틱을 동시에 쓸 때는 0이 아닌 키보드 이동이 우선한다. 재시작에는 키보드 R이 필요하다.

## 비기능 요구사항

| ID | 경계 | 현재 적용과 한계 |
|---|---|---|
| NFR-01 | 환경 재현성 | Unity 버전은 프로젝트 파일과 정확히 일치해야 한다. .NET·Unreal 실행 환경은 [실행 가이드](DEMO_GUIDE.md)를 따른다. |
| NFR-02 | 네트워크 입력 | 길이·JSON 깊이·필드·값 범위와 연결 수를 제한한다. 상세 제한과 알려진 예외 분류 문제는 [통신 명세](NETWORK_SECURITY.md)를 따른다. |
| NFR-03 | 공유 상태 | 서버 저장소를 단일 lock으로 보호한다. 8개 Thread의 동시 쓰기 검사가 있으며 처리량 보장은 없다. |
| NFR-04 | 실패 격리 | 네트워크 대기가 게임 스레드를 막지 않도록 처리한다. 서버 없이도 게임 루프를 실행할 수 있다. |
| NFR-05 | 성능 근거 | [측정 기준선](PERFORMANCE_BASELINE.md)의 조건과 결과만 인용한다. 일반적인 FPS·무할당·개선율을 보장하지 않는다. |
| NFR-06 | 변경 추적 | Git, PROCESS, ADR로 결정·변경·검증을 추적한다. 설치·패키지·엔진 업그레이드는 사전 승인이 필요하다. |

## 미구현과 제외 범위

| 구분 | 항목 | 현재 의미 |
|---|---|---|
| 계획 | MySQL 저장, 모든 플레이 이력, 재시도 중복 방지 키 | 스키마·마이그레이션·연결 코드가 없다. [데이터 모델](DATA_MODEL.md) 참조 |
| 계획 | 별도 SVN 실습 | Git 저장소와 분리하며 도구 승인 전 실행하지 않는다. |
| 검증 대기 | 네트워크 기능을 포함한 Windows 재빌드, 같은 서버 세션의 Unity → Unreal 연속 시연 | 기존 빌드나 개별 클라이언트 PASS로 대신하지 않는다. |
| 적용 보류 | 공간 해시의 게임 연결, Object Pool | 공간 해시는 독립 실험만 존재하며 풀은 구현하지 않았다. |
| 범위 제외 | 온라인 멀티플레이, 로그인, 공개 서버, 사운드, 저장 게임, 성장·아이템·자동 공격 | 현재 요구사항의 완료 항목이 아니다. |

실행 단계·기대 결과·실패 판정은 [실행 및 검증 가이드](DEMO_GUIDE.md), 향후 작업 순서는 [구현 계획](IMPLEMENTATION_PLAN.md)에 둔다.
