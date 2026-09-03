# Arena Systems Lab Implementation Plan

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
- enemy 수 증가 시 neighbor query 비용을 측정한 뒤 단순 grid spatial partition 적용
- 필수 reference와 arena 설정을 검사하는 최소 Editor validation menu 작성

### 완료 기준
변경 전후 profiler capture와 측정 조건이 기록되고, validation 도구가 잘못된 설정을 재현 가능하게 보고한다.

### 검증 방법
동일한 enemy 수와 시간 조건으로 profiler 결과를 비교하고 validation의 정상/실패 사례를 실행한다.

### 의존성

Unity Profiler, `UnityEngine.Pool`, UnityEditor API만 사용한다. Performance Testing API 직접 추가는 필요하지 않다.

### 위험 요소

측정 전에 pooling이나 spatial hash를 넣지 않는다. 작은 데이터에서는 단순 scan이 더 적절할 수 있다.

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
깨끗한 환경에서 프로젝트를 열고 테스트·build·demo flow를 재현할 수 있다.

### 검증 방법
정확한 Editor에서 전체 테스트, Windows build 실행, Console 확인, 수동 checklist를 수행한다.

### 의존성

현재 설치된 Windows Mono build support와 Test Framework를 사용한다.

### 위험 요소

Git repository와 `.gitignore`는 준비됐지만 아직 initial commit이 없다. commit 전 전체 source와 `.meta` 목록을 사람이 검토해야 한다.
