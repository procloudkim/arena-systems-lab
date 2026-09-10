# Arena Systems Lab

> 직접 이동·조준·발사하는 Unity 2D 생존 게임과, .NET TCP 점수 서버 및 Unreal C++ 조회 클라이언트.

적을 피하며 투사체로 처치하고, 사망하면 점수를 확인한 뒤 다시 시작한다. Unity는 플레이를 담당하고 Unreal은 같은 로컬 서버의 순위만 읽는다. 서버 없이도 게임을 실행할 수 있다.

## 빠른 실행

1. Unity Hub에서 이 저장소의 프로젝트 폴더를 Unity `6000.5.1f1`로 연다. 다른 버전으로 변환하지 않는다.
2. `Assets/Scenes/SampleScene.unity`를 열고 Play를 누른다.
3. WASD/방향키로 이동하고 마우스로 조준하며 왼쪽 버튼을 눌러 공격한다. Game Over에서 R로 재시작한다.

| 동작 | 키보드·마우스 | 게임패드 |
|---|---|---|
| 이동 | WASD / 방향키 | 왼쪽 스틱 |
| 조준 | 마우스 | 오른쪽 스틱 |
| 공격 | 왼쪽 클릭 / Enter, 누를 때 1회 | 서쪽 버튼, 누를 때 1회 |
| 재시작 | Game Over에서 R | 전용 입력 없음 |

조준 우선순위와 기본 수치는 [요구사항 명세](docs/REQUIREMENTS.md)에 있다. 패키지나 엔진 설치가 필요하면 자동 설치하지 말고 환경 차이를 먼저 확인한다.

## 개발 5대 문서

이 저장소에서 사용하는 문서 분류이며, 보편적인 단일 표준을 뜻하지 않는다. 처음 읽는 사람은 아래 순서로 범위 → 구조 → 데이터 → 통신 → 실행을 확인한다.

| 문서 | 답하는 질문 |
|---|---|
| 1. [요구사항 명세](docs/REQUIREMENTS.md) | 무엇을 구현했으며 완료 기준과 제외 범위는 무엇인가? |
| 2. [아키텍처 설계서](docs/ARCHITECTURE.md) | 컴포넌트·상태·스레드·프로세스는 어떻게 연결되는가? |
| 3. [데이터 모델 및 ERD](docs/DATA_MODEL.md) | 무엇을 어디에 저장하며 순위와 플레이 이력은 어떻게 다른가? |
| 4. [TCP 프로토콜 및 보안 명세](docs/NETWORK_SECURITY.md) | 요청·응답·오류·자원 제한·신뢰 경계는 무엇인가? |
| 5. [실행 및 검증 가이드](docs/DEMO_GUIDE.md) | 어떤 환경과 명령으로 실행·테스트·빌드하는가? |

## 구성과 환경

| 구성 | 역할 | 소스 / 환경 기준 |
|---|---|---|
| Unity | 2D 이동·전투·적 FSM·Game Over·순위 표시 | [Runtime](Assets/ArenaSystemsLab/Runtime/), Unity `6000.5.1f1`, Input System `1.19.0`, URP `17.5.0` |
| .NET 서버 | 로컬 TCP 요청 처리, 플레이어별 최고 점수 | [Server](Server/ArenaSystemsLab.Server/), `net10.0`, 외부 NuGet 의존성 없음 |
| Unreal | Play 시작 시 Top 5를 한 번 조회하는 HUD | [ArenaObserver](Unreal/ArenaObserver/), Engine association `5.8`, 검증 엔진 `5.8.0` |
| 개발 도구 | Unity 프로젝트 검사, Windows Development 빌드 | [Editor](Assets/ArenaSystemsLab/Editor/) |
| 검증 | Unity 테스트, 서버 검증 실행 파일, Unreal automation | [실행 및 검증 가이드](docs/DEMO_GUIDE.md) |

서버는 `127.0.0.1:7777`만 사용한다. 준비·빌드 후 가이드의 서버 명령을 실행하면 Unity Game Over에서 제출·조회하고 Unreal Play에서 같은 순위를 조회할 수 있다. 두 클라이언트는 서로 직접 통신하지 않는다.

## 읽기 전에 알아둘 제한

- 현재 저장소는 메모리 기반이다. 서버를 재시작하면 점수가 사라진다. MySQL·SQL 스키마·모든 플레이 이력 저장은 미구현이다.
- Top 5는 서로 다른 플레이어의 최고 점수다. `UnityPlayer`가 3점 다음 11점을 내면 `UnityPlayer 11` 한 줄만 남는다.
- 인증·TLS·점수의 게임상 정당성 검증이 없다. LAN·공개 서버로 운영하지 않는다.
- 공간 해시는 독립 실험이며 게임 루프에 연결되지 않았다. Object Pool도 적용하지 않았다.
- 기존 Windows 빌드 검증은 2026-09-04의 네트워크 기능 추가 전 결과다. 현재 네트워크 기능을 포함한 재빌드와 Unity → Unreal 연속 시연은 검증 대기다.
- 2026-09-05 기록에는 Unity EditMode 20/20, PlayMode 1/1, 서버 검증 8/8, Unreal automation 각 실행 1/1 PASS가 있다. 이는 날짜가 고정된 기록이며 새 환경의 실행 보증이 아니다. 원본 로그·빌드 생성물은 Git에 포함되지 않는다.

## 개발 기록

현재 상태와 다음 작업은 [PROCESS](PROCESS.md), 작업 규칙은 [AGENTS](AGENTS.md)가 기준이다.

- [구현 계획](docs/IMPLEMENTATION_PLAN.md)
- [기술 완결성 설계서 — 구현 예정](docs/TECHNICAL_COMPLETION_DESIGN.md)
- [환경 감사 기록](docs/ENVIRONMENT_AUDIT.md)
- [성능 측정 기준선](docs/PERFORMANCE_BASELINE.md)
- [게임 개발 용어 백과사전](docs/GAME_DEV_GLOSSARY.md)
- [AI 작업 및 사람 검증 기록](docs/AI_USAGE.md)
- [의사결정 기록](docs/adr/)

문서 버전과 기술 문서의 관리 경계는 [ADR 0012](docs/adr/0012-technical-documentation-governance.md)를 따른다.
