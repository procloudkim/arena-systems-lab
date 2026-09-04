# Game Development Glossary

> Arena Systems Lab 개발 중 실제로 만난 게임·Unity·물리·검증 용어를 한국어로 설명하는 생활형 백과사전이다.

- Version: `0.6.0`
- Last updated: 2026-09-05 KST
- Governance: [ADR 0004](adr/0004-game-development-glossary-governance.md)

## 사용 방법

- 코드·문서·검증에서 새 용어가 처음 등장하면 같은 작업 checkpoint에서 추가한다.
- 정의는 일반적인 의미와 이 프로젝트의 실제 사용 예를 함께 기록한다.
- 이미 있는 용어는 중복 작성하지 않고 이 문서의 항목을 참조한다.
- 모르는 용어는 영어 이름으로 검색한다.

## 문서 버전 규칙

- `MAJOR`: 분류나 항목 형식이 호환되지 않게 바뀔 때 증가한다.
- `MINOR`: 새 용어나 새 분류를 추가할 때 증가한다.
- `PATCH`: 정의, 예시, 오탈자, 링크만 바로잡을 때 증가한다.

## Engines and Object-Oriented Design

### Unity

- 정의: GameObject와 Component 중심 구조, C# scripting, Editor를 제공하는 real-time game engine이다.
- 프로젝트 예: 현재 2D arena game의 이동, 전투, 물리, UI, test와 Windows build를 담당한다.
- 주의: Unity package와 Editor version은 별도이므로 프로젝트가 요구하는 exact Editor를 확인한다.

### Unreal Engine

- 정의: Actor와 Component, C++, Blueprint, Editor를 제공하는 real-time game engine이다.
- 프로젝트 예: 계획된 `ArenaObserver`가 C++ native socket으로 같은 leaderboard server를 조회한다.
- 주의: Unity 기능을 그대로 복제하지 않고 Engine 5.8의 최소 read-only client로 범위를 제한한다.

### C# (C Sharp)

- 정의: .NET type system과 garbage collection을 사용하는 managed programming language다.
- 프로젝트 예: Unity gameplay code와 standalone `ArenaSystemsLab.Server`에 사용한다.
- 주의: Unity의 C# runtime과 별도 .NET application의 target framework·사용 가능 API는 같다고 가정하지 않는다.

### C++

- 정의: memory와 object lifetime을 세밀하게 제어할 수 있는 native compiled programming language다.
- 프로젝트 예: 계획된 Unreal `ArenaObserver` module을 구현하는 언어다.
- 주의: Unreal의 reflection·garbage collection 대상 object와 일반 C++ object의 lifetime 규칙을 구분한다.

### Object-Oriented Programming (OOP, 객체지향 프로그래밍)

- 정의: data와 behavior를 object로 묶고 책임, encapsulation, composition을 통해 system을 구성하는 방식이다.
- 프로젝트 예: `Health`, `EnemyController`, `EnemyStateMachine`, `EnemySpawner`가 서로 다른 책임을 가진다.
- 주의: class 수나 inheritance 깊이가 OOP 품질을 뜻하지 않는다. 한 구현뿐인 interface는 실제 교체 이유가 생길 때까지 만들지 않는다.

## Version Control

### Git

- 정의: 각 working copy가 전체 history를 가지는 distributed version control system이다.
- 프로젝트 예: work branch, ADR, commit, push와 remote SHA로 모든 변경을 추적한다.
- 주의: generated file, credential, 다른 VCS metadata를 commit하지 않고 Git을 이 저장소의 canonical VCS로 유지한다.

### Apache Subversion (SVN)

- 정의: 중앙 repository의 revision을 기준으로 checkout, commit, branch, merge를 수행하는 centralized version control system이다.
- 프로젝트 예: 계획된 isolated local lab에서 `trunk/branches/tags`, merge와 conflict resolution을 재현한다.
- 주의: Git working tree 안에 active `.svn` metadata를 섞거나 두 VCS를 동시에 source-of-truth로 사용하지 않는다.

## Networking and Concurrency

### Network Programming

- 정의: 서로 다른 process나 machine이 protocol을 통해 data를 교환하도록 연결, timeout, 오류, 보안을 다루는 개발 영역이다.
- 프로젝트 예: C# leaderboard server가 loopback client의 score 제출과 조회를 처리하며 Unity와 Unreal client가 후속 연결된다.
- 주의: 정상 연결뿐 아니라 disconnect, timeout, malformed input과 partial read를 처리해야 한다.

### Client-Server Architecture

- 정의: client가 request를 보내고 server가 규칙과 shared data를 관리해 response하는 책임 분리 구조다.
- 프로젝트 예: 두 game engine은 client이고 .NET application은 score 저장·조회 server다.
- 주의: client가 보낸 score를 신뢰하지 않으며 최초 범위는 public service가 아닌 loopback demo다.

### Socket

- 정의: process가 network protocol endpoint를 통해 byte를 송수신하는 operating-system resource와 programming interface다.
- 프로젝트 예: .NET server의 `TcpListener`, verification client의 `TcpClient`와 향후 Unreal native socket이 연결된다.
- 주의: 한 번의 read가 한 message 전체를 반환한다고 가정하지 않고 close와 timeout을 항상 처리한다.

### Transmission Control Protocol (TCP)

- 정의: 순서와 전달을 보장하는 connection-oriented byte-stream transport protocol이다.
- 프로젝트 예: leaderboard message를 4-byte length prefix와 UTF-8 JSON payload로 framing한다.
- 주의: TCP는 message 경계를 보존하지 않으므로 application protocol이 길이와 최대 크기를 정의해야 한다.

### Application Protocol (애플리케이션 프로토콜)

- 정의: transport 위에서 message 형식, 순서, 의미와 오류 처리를 application끼리 합의한 규칙이다.
- 프로젝트 예: protocol v1이 `health`, `submit_score`, `get_leaderboard` JSON request와 response를 정의한다.
- 주의: TCP 연결 성공은 protocol version이나 payload 의미가 올바르다는 뜻이 아니다.

### Message Framing / Length Prefix (메시지 프레이밍 / 길이 접두사)

- 정의: 경계가 없는 byte stream에서 message 시작과 끝을 구분하는 규칙이며, length prefix는 앞부분에 payload byte 수를 기록한다.
- 프로젝트 예: `WireProtocol`이 4-byte 길이를 먼저 읽고 그 수만큼 UTF-8 JSON을 정확히 읽는다.
- 주의: 길이를 검증하기 전에 그 크기로 memory를 할당하면 resource exhaustion 공격에 취약하다.

### Network Byte Order / Big-Endian (네트워크 바이트 순서 / 빅 엔디언)

- 정의: 여러 byte로 된 정수의 가장 큰 자리 byte를 먼저 전송하는 network 표현 순서다.
- 프로젝트 예: frame 길이는 `BinaryPrimitives.ReadInt32BigEndian`과 `WriteInt32BigEndian`으로 인코딩한다.
- 주의: client와 server가 byte order를 다르게 해석하면 같은 4 byte가 전혀 다른 길이가 된다.

### Loopback (루프백)

- 정의: network packet이 외부 interface로 나가지 않고 같은 host 안의 process로 돌아오는 주소 범위다.
- 프로젝트 예: server는 IPv4 `127.0.0.1`에만 bind해 local integration으로 노출을 제한한다.
- 주의: local process라면 접속할 수 있으므로 loopback은 authentication이 아니다.

### Trust Boundary (신뢰 경계)

- 정의: data나 control이 서로 다른 신뢰 수준의 영역 사이를 통과하는 지점이다.
- 프로젝트 예: `NetworkStream`에서 받은 frame이 server parser로 들어오는 socket 경계에서 모든 값을 다시 검증한다.
- 주의: 공식 client나 같은 PC에서 왔다는 이유로 경계를 없애면 안 된다.

### Input Validation (입력 검증)

- 정의: 외부 입력의 구조, type, 길이, 문자 집합과 의미 범위가 허용 규칙에 맞는지 확인하는 작업이다.
- 프로젝트 예: protocol version과 정확한 property 집합, `playerId`, score, query limit를 server가 검사한다.
- 주의: client-side 검사는 사용자 경험용일 뿐 server-side 보안 검사를 대신하지 않는다.

### Resource Exhaustion / Denial of Service (자원 고갈 / 서비스 거부)

- 정의: memory, CPU, thread, socket, storage 같은 제한 자원을 과도하게 소비시켜 정상 처리를 방해하는 실패 또는 공격이다.
- 프로젝트 예: frame 16 KiB, client 16개, timeout 5초, stored player 10,000개 상한을 둔다.
- 주의: 요청 하나만 제한해도 반복 요청이나 누적 state가 무제한이면 전체 자원은 계속 증가할 수 있다.

### Transport Layer Security (TLS, 전송 계층 보안)

- 정의: network 연결의 기밀성, 무결성과 endpoint 인증을 제공하는 cryptographic protocol이다.
- 프로젝트 예: 현재 loopback server에는 없으며 remote exposure 승인 시 .NET `SslStream` 적용이 필수 gate다.
- 주의: TLS는 client가 제출한 gameplay score 자체가 정당하다는 사실까지 보장하지 않는다.

### Authentication (인증)

- 정의: 접속자나 service가 주장한 identity가 실제로 맞는지 확인하는 절차다.
- 프로젝트 예: 현재 protocol에는 인증이 없어 `playerId`는 표시용 입력일 뿐 보안 identity가 아니다.
- 주의: authorization과 구분하며, IP 주소나 client application 이름만으로 인증됐다고 판단하지 않는다.

### Asynchronous I/O (비동기 입출력)

- 정의: 입출력 완료를 기다리는 동안 호출 thread를 계속 점유하지 않고 나중에 완료를 이어가는 실행 방식이다.
- 프로젝트 예: server는 accept, frame read/write에 `async`/`await`와 cancellation을 사용한다.
- 주의: 비동기 I/O 자체는 여러 CPU thread가 병렬로 shared state를 실행했다는 증거가 아니다.

### Multithreading (멀티스레딩)

- 정의: 한 process에서 여러 execution thread가 작업을 동시에 또는 병렬로 진행하는 방식이다.
- 프로젝트 예: verification executable이 실제 `Thread` 8개로 같은 `LeaderboardStore`를 동시에 갱신한다.
- 주의: asynchronous I/O 자체를 multithreading과 동일시하지 않고 thread scheduling과 shared state 접근을 별도로 확인한다.

### Thread Safety (스레드 안전성)

- 정의: 여러 thread가 같은 state에 접근해도 race condition이나 손상된 결과가 생기지 않는 성질이다.
- 프로젝트 예: `LeaderboardStore`의 score 갱신과 조회를 하나의 `lock`으로 보호하고 concurrent test로 확인한다.
- 주의: 무조건 큰 global lock을 두기보다 먼저 공유 범위를 줄이고 필요한 최소 synchronization을 사용한다.

### Race Condition (경쟁 상태)

- 정의: 여러 execution 흐름의 접근 순서에 따라 결과가 달라지는 timing-dependent 오류다.
- 프로젝트 예: 같은 player의 최고 score를 여러 thread가 갱신해도 더 낮은 값으로 되돌아가지 않아야 한다.
- 주의: test가 한 번 통과한 것만으로 모든 race가 없다고 단정하지 않고 공유 state와 synchronization 경계를 함께 검토한다.

## Data Persistence

### MySQL

- 정의: table, key, constraint와 SQL query로 structured data를 저장하는 relational database management system이다.
- 프로젝트 예: 계획된 server가 player run과 leaderboard score를 MySQL 8.4 LTS에 저장한다.
- 주의: query는 parameterized 형태로 실행하고 credential, connection string, local data volume을 Git에 넣지 않는다.

## Architecture and State

### Finite State Machine (FSM, 유한 상태 머신)

- 정의: 객체가 한 번에 하나의 유한한 상태를 가지며 조건에 따라 다른 상태로 전환되는 구조다.
- 프로젝트 예: 적을 `Chase`, `Attack`, `Dead` 상태로 나누고 거리나 사망 여부에 따라 전환한다.
- 주의: 적 한 종류에는 `enum`과 `switch`면 충분하며 상태별 class 계층은 필요할 때만 만든다.

### State (상태)

- 정의: 객체가 현재 어떤 행동 규칙을 실행하는지를 나타내는 모드다.
- 프로젝트 예: `Chase` 상태에서는 플레이어 방향으로 이동하고 `Dead` 상태에서는 이동과 공격을 중단한다.
- 주의: animation 이름이 아니라 실제 행동과 전환 규칙을 설명해야 한다.

### Transition (상태 전이)

- 정의: 조건이 충족됐을 때 현재 상태에서 다음 상태로 바뀌는 과정이다.
- 프로젝트 예: 플레이어가 공격 거리 안에 들어오면 `Chase → Attack`, Health가 0이면 `Any → Dead`가 된다.
- 주의: 같은 frame에 여러 전이가 경쟁할 때 사망처럼 우선순위가 높은 조건을 먼저 처리한다.

### Terminal State (종료 상태)

- 정의: 한 번 진입하면 정상 흐름에서는 다른 상태로 빠져나가지 않는 마지막 상태다.
- 프로젝트 예: `EnemyState.Dead`에 진입한 적은 이후 입력이 달라져도 살아 있는 상태로 돌아가지 않는다.
- 주의: 종료 상태 진입 직후 객체를 제거하면 화면 표시는 매우 짧을 수 있으므로 상태 전이 테스트도 함께 둔다.

## Unity Object Model

### Component (컴포넌트)

- 정의: `GameObject`에 붙여 데이터나 행동을 제공하는 Unity 객체다.
- 프로젝트 예: 적 하나에 `SpriteRenderer`, `Rigidbody2D`, `CircleCollider2D`, `Health`, `EnemyController`가 붙는다.
- 주의: 한 component에 이동, 체력, 생성 책임을 모두 넣지 않는다.

### GameObject (게임 오브젝트)

- 정의: Scene 안에서 이름, 활성 상태, `Transform`, 여러 component를 담는 기본 컨테이너다.
- 프로젝트 예: player, enemy, projectile이 각각 하나의 `GameObject`다.
- 주의: `GameObject` 자체보다 붙어 있는 component가 실제 기능을 결정한다.

### MonoBehaviour

- 정의: Unity lifecycle callback과 component 기능을 사용하는 script의 기본 class다.
- 프로젝트 예: `PlayerController`, `EnemyController`, `Health`가 `MonoBehaviour`를 상속한다.
- 주의: 일반 계산 로직까지 모두 `MonoBehaviour`로 만들 필요는 없다.

### Prefab (프리팹)

- 정의: 반복해서 생성할 `GameObject`와 component 구성을 asset으로 저장한 template이다.
- 프로젝트 예: Day 1은 Scene/Prefab 수정을 피하기 위해 runtime에서 적과 projectile을 구성한다.
- 주의: prefab instance 변경과 prefab asset 변경을 구분해야 한다.

### Runtime Bootstrap (런타임 부트스트랩)

- 정의: 게임 시작 시 필요한 객체와 연결을 코드로 구성하는 초기화 단계다.
- 프로젝트 예: `ArenaGame`이 Scene 로드 후 player, spawner, HUD와 부모 객체를 만든다.
- 주의: 빠른 vertical slice에는 유용하지만 구성이 커지면 Inspector나 data asset이 더 읽기 쉬울 수 있다.

### Scene (씬)

- 정의: Unity가 저장하는 `GameObject` 계층과 component 상태의 실행 단위다.
- 프로젝트 예: Build Settings에는 `Assets/Scenes/SampleScene.unity`가 등록돼 있다.
- 주의: Scene YAML은 Unity Editor가 관리하므로 이해하지 못한 직접 수정을 피한다.

### Serialization (직렬화)

- 정의: 객체의 상태를 저장 가능한 형식으로 변환하는 과정이다.
- 프로젝트 예: `[SerializeField] private float moveSpeed`는 private field지만 Unity asset에 저장될 수 있다.
- 주의: property와 일반 private field는 기본적으로 Unity가 직렬화하지 않는다.

## Frame and Time

### `deltaTime`

- 정의: 직전 frame부터 현재 frame까지 흐른 시간이다.
- 프로젝트 예: frame rate와 무관한 이동량을 계산할 때 속도에 시간을 곱한다.
- 주의: 물리 tick에서는 `Time.fixedDeltaTime`을 사용한다.

### `FixedUpdate`

- 정의: Unity 물리 시간 간격에 맞춰 호출되는 lifecycle callback이다.
- 프로젝트 예: player와 enemy가 `Rigidbody2D.MovePosition`으로 이동할 때 사용한다.
- 주의: 키 입력의 순간 이벤트는 `Update`에서 읽고 물리 이동값만 `FixedUpdate`에서 적용한다.

### `Update`

- 정의: 활성화된 `MonoBehaviour`에서 화면 frame마다 호출되는 lifecycle callback이다.
- 프로젝트 예: player 입력과 공격, enemy spawn 시간을 확인한다.
- 주의: frame마다 collection이나 LINQ 결과를 만들면 반복적인 GC allocation이 생길 수 있다.

### Main Thread (메인 스레드)

- 정의: Unity player loop와 대부분의 Unity object API가 실행되는 주 실행 thread다.
- 프로젝트 예: Day 3 baseline에서 `Main Thread` profiler counter의 frame 시간을 기록한다.
- 주의: 측정값에는 실제 script 실행 외에 frame 대기나 Editor overhead가 포함될 수 있으며 background thread에서 Unity object를 직접 변경하면 안 된다.

## 2D Physics

### Collider2D (2D 콜라이더)

- 정의: 2D 물리 세계에서 충돌 또는 겹침을 판정하는 모양이다.
- 프로젝트 예: player, enemy, projectile은 `CircleCollider2D`를 사용한다.
- 주의: 보이는 sprite 모양과 collider 모양은 별개다.

### Collision (충돌)

- 정의: trigger가 아닌 collider끼리 접촉해 물리 반응과 collision callback이 발생하는 상황이다.
- 프로젝트 예: `EnemyController.OnCollisionStay2D`가 player와 접촉하는 동안 일정 간격으로 damage를 준다.
- 주의: callback이 매 물리 tick 반복될 수 있으므로 damage cooldown이 필요하다.

### Continuous Collision Detection (연속 충돌 검출)

- 정의: 빠르게 움직이는 body가 collider를 한 frame 사이에 통과하는 tunneling을 줄이는 충돌 방식이다.
- 프로젝트 예: 빠른 projectile에 `CollisionDetectionMode2D.Continuous`를 설정한다.
- 주의: Discrete 방식보다 계산 비용이 크므로 필요한 body에만 사용한다.

### Rigidbody2D

- 정의: `GameObject`를 Unity 2D 물리 simulation에 참여시키는 component다.
- 프로젝트 예: player와 enemy는 `MovePosition`, projectile은 `linearVelocity`로 움직인다.
- 주의: `Transform`을 직접 바꾸는 방식과 물리 이동 API를 무분별하게 섞지 않는다.

### Trigger (트리거)

- 정의: 물리적으로 밀어내지 않고 collider가 겹쳤다는 callback만 발생시키는 설정이다.
- 프로젝트 예: projectile collider의 `isTrigger`를 켜고 `OnTriggerEnter2D`에서 enemy damage를 처리한다.
- 주의: 두 객체의 collider와 필요한 `Rigidbody2D` 구성이 맞아야 callback이 발생한다.

## Gameplay Systems

### Damage (데미지)

- 정의: 대상의 Health를 감소시키려는 게임 규칙상의 값 또는 사건이다.
- 프로젝트 예: projectile과 enemy 접촉 공격이 `Health.ApplyDamage`를 호출한다.
- 주의: 0 이하 값과 이미 죽은 대상에 대한 중복 처리를 명확히 정의한다.

### Health (체력)

- 정의: 피해 누적과 생존·사망 상태를 관리하는 gameplay 상태다.
- 프로젝트 예: `Health`는 현재 체력, 최대 체력, 사망 여부와 `Died` event를 관리한다.
- 주의: 체력이 0이 된 뒤 사망 event가 중복 발생하지 않도록 보장한다.

### Projectile (투사체)

- 정의: 발사 후 공간을 이동하며 충돌 대상에 효과를 전달하는 객체다.
- 프로젝트 예: player projectile은 방향, 속도, damage, lifetime을 받아 enemy와 trigger 충돌 시 damage를 준다.
- 주의: 빠른 이동의 tunneling과 수명 종료 후 정리를 고려한다.

### Spawn / Spawner (생성 / 생성기)

- 정의: runtime 중 새로운 게임 객체를 만들고 초기 상태를 설정하는 과정과 그 책임 객체다.
- 프로젝트 예: `EnemySpawner`가 arena 가장자리의 무작위 위치에 enemy를 생성한다.
- 주의: 생성·파괴 빈도가 높아 실제 병목이 측정될 때 pooling을 검토한다.

## Performance

### GC Allocation

- 정의: managed heap에 새 객체가 할당돼 이후 Garbage Collector가 정리해야 하는 메모리 비용이다.
- 프로젝트 예: frame마다 새 collection이나 LINQ 결과를 만들지 않는 코딩 규칙을 사용한다.
- 주의: allocation이 있다는 사실만으로 병목이라 단정하지 말고 Profiler로 빈도와 크기를 측정한다.

### Object Pooling (오브젝트 풀링)

- 정의: 객체를 반복 생성·파괴하는 대신 미리 만들거나 반환받아 재사용하는 기법이다.
- 프로젝트 예: enemy나 projectile의 `new GameObject`/`Destroy` 비용이 측정상 문제가 될 때 적용 후보가 된다.
- 주의: 현재는 적용하지 않았으며 수명 초기화 누락과 비활성 object 관리 비용이 생긴다.

### Profiling (프로파일링)

- 정의: CPU 시간, memory, allocation, rendering 등 실제 실행 비용을 측정하는 작업이다.
- 프로젝트 예: Day 3에서 같은 enemy 수와 실행 시간으로 변경 전후를 비교한다.
- 주의: 측정 조건 없이 “최적화됐다”고 주장하지 않는다.

### Benchmark (벤치마크)

- 정의: 고정된 입력과 실행 조건으로 구현들의 처리 시간이나 자원 사용량을 비교하는 측정이다.
- 프로젝트 예: 같은 20,000 point와 500 query를 spatial hash와 brute-force scan에 적용한다.
- 주의: 한 환경의 짧은 결과를 전체 gameplay 성능이나 보편적인 우위로 일반화하지 않는다.

### ProfilerRecorder

- 정의: Unity Profiler marker와 counter의 sample을 code에서 수집하는 runtime API다.
- 프로젝트 예: PlayMode test가 Main Thread, frame GC allocation, GameObject count를 5초 동안 기록한다.
- 주의: counter 이름과 option은 Unity version에 따라 검증해야 하며 recorder와 test instrumentation 자체도 측정에 영향을 준다.

### Brute-Force Search (전수 탐색)

- 정의: 후보를 줄이는 index 없이 모든 element를 차례로 검사해 조건에 맞는 값을 찾는 방식이다.
- 프로젝트 예: 모든 point의 거리를 검사한 결과를 `SpatialHash2D` query correctness 기준으로 사용한다.
- 주의: 작은 collection에는 단순하고 충분히 빠를 수 있으므로 항상 공간 분할로 교체할 필요는 없다.

### Spatial Hash (공간 해시)

- 정의: 공간을 cell로 나누고 객체를 위치 기반 bucket에 넣어 가까운 후보만 조회하는 자료구조다.
- 프로젝트 예: 많은 enemy의 주변 이웃 탐색이 실제로 필요하고 전체 탐색이 병목일 때 적용한다.
- 주의: cell 크기와 갱신 비용이 있으며 현재 gameplay에는 아직 구현하지 않았다.

## Testing and Validation

### Editor Validation

- 정의: 게임 실행 전에 Editor에서 project asset과 설정의 필수 조건을 자동 검사하는 개발 도구다.
- 프로젝트 예: `ArenaProjectValidator`가 Editor version, Build Scene, Input Actions와 Move/Attack action을 확인한다.
- 주의: 설정 검사는 실제 gameplay, physics callback, player build 검증을 대신하지 않는다.

### EditMode Test

- 정의: Play Mode에 들어가지 않고 Editor 환경에서 빠르게 실행하는 Unity test다.
- 프로젝트 예: `Health`의 damage 감소, 사망, 중복 사망 방지, 잘못된 damage 처리를 검증한다.
- 주의: 실제 frame 진행, physics callback, player input을 완전히 대신하지 않는다.

### PlayMode Test

- 정의: 실제 player loop와 lifecycle이 동작하는 Play Mode에서 실행하는 Unity test다.
- 프로젝트 예: 향후 Game Over와 restart 같은 여러 component의 통합 흐름을 검증한다.
- 주의: EditMode test보다 느리고 Scene·frame timing의 영향을 받는다.

### Player Build

- 정의: Unity Editor 밖에서 독립적으로 실행할 수 있도록 Scene, script, engine runtime과 asset을 platform별 application으로 묶은 결과물이다.
- 프로젝트 예: `Builds/Windows/ArenaSystemsLab.exe`와 동반 data file을 Windows x86-64 player로 생성한다.
- 주의: Editor PlayMode 성공이 player build의 compile, graphics, input과 실행 성공을 자동으로 보장하지 않는다.

### Development Build

- 정의: profiler 연결과 개발 진단 정보를 포함해 debugging·profiling하기 쉽게 만든 player build다.
- 프로젝트 예: Day 4 `ArenaWindowsBuilder`가 `BuildOptions.Development`로 Windows Mono build를 생성한다.
- 주의: release build보다 크고 느릴 수 있으므로 배포 성능 수치로 일반화하지 않는다.

### Regression Test (회귀 테스트)

- 정의: 새 변경이 이전에 정상 동작하던 기능을 깨뜨리지 않았는지 반복 확인하는 테스트다.
- 프로젝트 예: Day 4에 전체 EditMode 16건과 PlayMode 1건을 다시 실행한다.
- 주의: 기존 test가 다루지 않는 player input과 화면 흐름은 별도 수동 검증이 필요하다.

### Smoke Test (스모크 테스트)

- 정의: 세부 기능 전체보다 application이 시작되고 즉시 치명적으로 실패하지 않는지를 짧게 확인하는 첫 실행 검사다.
- 프로젝트 예: 생성된 Windows player를 8초 실행해 process 생존과 log의 managed exception·crash를 확인한다.
- 주의: smoke PASS는 전투, Game Over, restart 전체가 정상이라는 뜻이 아니다.

## Version History

| Version | Date | 변경 | ADR |
|---|---|---|---|
| `0.6.0` | 2026-09-05 | network security·protocol·concurrency 용어 11개 추가, 총 63개 | [ADR 0009](adr/0009-loopback-first-bounded-tcp-protocol.md) |
| `0.5.0` | 2026-09-04 | Day 4 build·regression·smoke 용어 4개 추가, 총 52개 | [ADR 0008](adr/0008-reproducible-windows-build-and-demo.md) |
| `0.4.0` | 2026-09-04 | Day 3 측정·알고리즘·Editor Tool 용어 5개 추가, 총 48개 | [ADR 0007](adr/0007-measured-day3-tooling.md) |
| `0.3.0` | 2026-09-04 | 필수 engine, language, VCS, network, concurrency, database 용어 14개 추가, 총 43개 | [ADR 0006](adr/0006-portfolio-technology-baseline.md) |
| `0.2.0` | 2026-09-04 | Day 2 FSM에서 사용한 Terminal State 추가, 총 29개 | [ADR 0005](adr/0005-minimal-enemy-fsm.md) |
| `0.1.0` | 2026-09-04 | 현재 코드와 Day 2~4 계획에서 사용한 기본 용어 28개 수록 | [ADR 0004](adr/0004-game-development-glossary-governance.md) |
