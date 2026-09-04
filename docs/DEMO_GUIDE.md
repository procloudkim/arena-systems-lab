# Arena Systems Lab Demo Guide

> 목표: 3~5분 안에 playable loop, OOP/FSM 구조, 측정 근거, Editor Tool, 자동 테스트, Windows build를 재현한다.

## 준비 조건

- Unity Editor `6000.5.1f1` exact match
- `Assets/Scenes/SampleScene.unity`가 Build Settings에서 활성화
- 실행 전 `git status --short --branch`로 예상하지 않은 변경이 없는지 확인
- Unity가 다른 process에서 이 project를 열고 있지 않음

## Demo flow

| 시간 | 시연 | 확인할 근거 |
|---:|---|---|
| 0:00~0:30 | `README.md`에서 한 줄 정의와 핵심 재미 설명 | 작은 game loop와 engineering evidence를 함께 보여 주는 목적 |
| 0:30~1:00 | `Tools > Arena Systems Lab > Validate Project` 실행 | exact Editor, enabled Scene, `Player/Move`, `Player/Attack` validation PASS |
| 1:00~2:30 | `SampleScene` Play | 이동·조준·공격, spawn·chase, HP·score 증가 |
| 2:30~3:00 | enemy와 접촉 후 Hierarchy/색상 확인 | Gray `Idle`, Red `Chase`, Orange `Attack`; death는 terminal state |
| 3:00~3:30 | player 사망 후 `R` | Game Over 후 score·enemy·health가 초기화된 새 round |
| 3:30~4:15 | Test Runner와 `PERFORMANCE_BASELINE.md` | EditMode 16/16, PlayMode 1/1, 측정 전 최적화를 채택하지 않은 판단 |
| 4:15~5:00 | Windows build 실행 | 독립 player에서도 같은 loop가 시작됨 |

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
| EditMode regression | PASS | exact Editor, 16 passed / 0 failed / 0 skipped |
| PlayMode regression/profile | PASS | 1 passed / 0 failed / 0 skipped |
| Project validator CLI | PASS | validation success log |
| Windows Mono Development build | PASS | Windows x86-64 player 생성, build result Success |
| Player launch smoke | PASS | 8초간 process 생존, 조기 종료·managed exception·crash 없음 |
| Windows player 전체 gameplay | NOT RUN | 사람이 위 8단계를 수행해야 함 |

Player smoke log에는 GPU 환경의 D3D12 info queue 경고와 강제 종료 시점의 Unity resource cleanup 진단이 남았다. build 실패, managed exception, crash 근거로 분류되지는 않았지만 사람 검증에서는 새 Error/Exception 여부를 다시 확인한다.

## Demo capture script

1. 제목 화면 대신 실행 중 HUD와 몰려오는 적을 먼저 보여 준다.
2. 이동하면서 mouse로 반대 방향을 조준해 수동 공격의 차별점을 보여 준다.
3. enemy 색상과 Hierarchy 이름으로 FSM을 짧게 설명한다.
4. 일부러 사망하고 `R`로 즉시 재시작한다.
5. 마지막 화면에 test 결과, performance baseline, Windows executable을 차례로 보여 준다.

영상이나 GIF는 아직 생성하지 않는다. 실제 portfolio 제출 매체와 길이가 정해질 때 이 flow를 그대로 녹화한다.
