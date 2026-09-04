# Arena Systems Lab Demo Guide

> 목표: 3~5분 안에 playable loop, OOP/FSM 구조, local leaderboard, 측정 근거, Editor Tool과 자동 테스트를 재현한다.

## 준비 조건

- Unity Editor `6000.5.1f1` exact match
- `Assets/Scenes/SampleScene.unity`가 Build Settings에서 활성화
- 실행 전 `git status --short --branch`로 예상하지 않은 변경이 없는지 확인
- Unity가 다른 process에서 이 project를 열고 있지 않음
- leaderboard 시연 시 Windows PowerShell에서 아래 local server를 먼저 실행

```powershell
& "<dotnet>" run --project Server/ArenaSystemsLab.Server/ArenaSystemsLab.Server.csproj --configuration Release --no-build --no-restore -- --port 7777
```

## Demo flow

| 시간 | 시연 | 확인할 근거 |
|---:|---|---|
| 0:00~0:30 | `README.md`에서 한 줄 정의와 핵심 재미 설명 | 작은 game loop와 engineering evidence를 함께 보여 주는 목적 |
| 0:30~1:00 | `Tools > Arena Systems Lab > Validate Project` 실행 | exact Editor, enabled Scene, `Player/Move`, `Player/Attack` validation PASS |
| 1:00~2:30 | `SampleScene` Play | 이동·조준·공격, spawn·chase, HP·score 증가 |
| 2:30~3:00 | enemy와 접촉 후 Hierarchy/색상 확인 | Gray `Idle`, Red `Chase`, Orange `Attack`; death는 terminal state |
| 3:00~3:40 | player 사망 후 leaderboard와 `R` 확인 | score 제출·상위 5개 조회 후 새 round 시작 |
| 3:40~4:30 | Test Runner와 `PERFORMANCE_BASELINE.md` | EditMode 20/20, PlayMode 1/1, 측정 전 최적화를 채택하지 않은 판단 |
| 4:30~5:00 | Windows build 근거 확인 | 독립 player build와 launch 기록 |

## Unity leaderboard 수동 체크리스트

기존 Windows build는 Unity client 구현 전 생성물이므로 이 검증은 우선 exact Editor PlayMode에서 수행한다.

| 단계 | 실행 | 기대 결과 | 실패 판정 |
|---:|---|---|---|
| 1 | port 7777 server가 없는 상태로 Play 후 사망 | Game Over와 final score가 표시되고 잠시 뒤 leaderboard가 `unavailable`로 바뀜 | freeze, crash, 반복 Console error |
| 2 | `R` 입력 | server 실패와 무관하게 HP 100·score 0인 새 round 시작 | 요청 취소나 restart 실패 |
| 3 | 위 PowerShell 명령으로 server 실행 | `127.0.0.1:7777`, protocol v1 listening message | 다른 interface bind, 즉시 종료 |
| 4 | 다시 Play하여 적을 처치한 뒤 사망 | `Leaderboard: connected`, `UnityPlayer`와 final score가 상위 5개에 표시 | score 불일치, 응답 미표시 |
| 5 | 한 번 더 플레이해 더 높은 score 제출 | 같은 `UnityPlayer` entry가 최고 score로 갱신 | duplicate entry 또는 낮은 score로 감소 |
| 6 | Unity Console과 server log 확인 | Unity Error/Exception 없음, server는 고정 상태 log만 출력 | payload/player ID/stack trace 노출 또는 error |
| 7 | Play 종료 후 server에서 `Ctrl+C` | server가 정상 종료되고 Unity도 PlayMode를 빠져나옴 | 종료 불가 또는 남은 process |

## Windows player 수동 체크리스트

준비: `Tools > Arena Systems Lab > Build Windows Development`를 실행하거나 README의 command-line build를 사용한다. 생성물은 `Builds/Windows/ArenaSystemsLab.exe`다.

| 단계 | 실행 | 기대 결과 | 실패 판정 |
|---:|---|---|---|
| 1 | `ArenaSystemsLab.exe` 실행 | 창이 열리고 HUD, blue player, arena가 보임 | 조기 종료, 검은 화면, missing file dialog |
| 2 | WASD/방향키로 이동 | player가 arena 경계 안에서 움직임 | 입력 없음, 경계 밖 이동 |
| 3 | mouse 조준 후 left click | yellow projectile가 pointer 방향으로 발사됨 | 발사 없음, 반복 error |
| 4 | enemy 처치 | enemy가 사라지고 score가 1 증가 | damage 또는 score 미반영 |
| 5 | enemy와 접촉 | enemy가 orange `Attack` 상태가 되고 HP가 주기적으로 감소 | 매 frame damage, 상태 전이 없음 |
| 6 | player HP를 0으로 만듦 | Game Over가 표시되고 spawn·입력이 중지됨 | round가 계속 진행되거나 exception 발생 |
| 7 | `R` 입력 | HP 100, score 0인 새 round 시작 | 이전 round object가 남거나 restart 실패 |
| 8 | player 종료 후 log/Console 확인 | 이번 변경에서 발생한 Error/Exception 없음 | compiler error, managed exception, crash |

## 현재 검증 상태

| 항목 | 상태 | 근거 |
|---|---|---|
| EditMode regression | PASS | exact Editor, 20 passed / 0 failed / 0 skipped |
| PlayMode regression/profile | PASS | 1 passed / 0 failed / 0 skipped |
| Project validator CLI | PASS | validation success log |
| Windows Mono Development build | PASS | Windows x86-64 player 생성, build result Success |
| Player launch smoke | PASS | 8초간 process 생존, 조기 종료·managed exception·crash 없음 |
| Windows player 전체 gameplay | PASS | 사용자 확인, 위 8단계와 오류 없음 |
| Unity leaderboard 자동 protocol 검사 | PASS | 정상 framing·single retry·oversized response·server unavailable 4건 |
| Server-unavailable Game Over flow | PASS | 사용자 확인, unavailable·restart·Console 오류 없음 |
| Actual server Game Over flow | PASS | 사용자 확인, `UnityPlayer` 3점→11점 최고 score 갱신·중복 없음 |
| Milestone 7 Windows player rebuild | NOT RUN | 사람 PlayMode 검증 후 최종 build 단계에서 실행 |

Player smoke log에는 GPU 환경의 D3D12 info queue 경고와 강제 종료 시점의 Unity resource cleanup 진단이 남았다. build 실패, managed exception, crash 근거는 아니며 이후 사람 검증에서도 새 Error/Exception이 없음을 확인했다.

현재 `Top 5`는 run 5개가 아니라 서로 다른 player의 최고 score 5개다. 같은 `UnityPlayer`의 이전 시도는 표시하지 않으며 과거 run 목록은 MySQL persistence 단계에서 별도 `Recent Runs`로 검증한다.

## Demo capture script

1. 제목 화면 대신 실행 중 HUD와 몰려오는 적을 먼저 보여 준다.
2. 이동하면서 mouse로 반대 방향을 조준해 수동 공격의 차별점을 보여 준다.
3. enemy 색상과 Hierarchy 이름으로 FSM을 짧게 설명한다.
4. 일부러 사망하고 `R`로 즉시 재시작한다.
5. 마지막 화면에 test 결과, performance baseline, Windows executable을 차례로 보여 준다.

영상이나 GIF는 아직 생성하지 않는다. 실제 portfolio 제출 매체와 길이가 정해질 때 이 flow를 그대로 녹화한다.
