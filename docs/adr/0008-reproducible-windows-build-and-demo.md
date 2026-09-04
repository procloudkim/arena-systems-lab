# ADR 0008: Reproducible Windows Build and Demo

- Status: Accepted
- Date: 2026-09-04

## Context

Day 4에는 Editor 내부 동작을 넘어 Windows player가 실제로 생성·시작되는 근거와 채용 검토자가 짧게 따라갈 demo flow가 필요하다. 기존 project에는 Windows Mono support와 enabled `SampleScene`이 있지만 재현 가능한 build entry point, root README, standalone checklist가 없었다.

## Decision

- 기존 `ArenaSystemsLab.Editor` assembly에 static `ArenaWindowsBuilder` 하나를 추가한다.
- build 전에 `ArenaProjectValidator`를 재사용하고 Standalone scripting backend가 Mono인지 확인한다.
- enabled Build Settings Scene만 대상으로 Windows x86-64 `Development` build를 만든다.
- Editor menu와 batchmode entry point가 같은 build method를 사용한다.
- build target이나 scripting backend를 자동 변경하지 않는다.
- output은 `Builds/Windows/ArenaSystemsLab.exe`로 고정하고 generated binary와 log는 Git에 commit하지 않는다.
- root `README.md`를 project entry point로, `docs/DEMO_GUIDE.md`를 demo와 standalone 수동 검증의 단일 문서로 사용한다.
- 실제 build가 source asset과 ProjectSettings를 자동 직렬화하면 diff를 검사하고 이번 실행이 만든 승인 범위 밖 변경만 복원한다.

## Consequences

Editor menu와 CI형 command line에서 동일한 build를 재현할 수 있고 잘못된 version, Scene, input, backend는 build 전에 명확히 실패한다. 현재 자동화는 portfolio 대상인 Windows Mono Development build만 지원하며 release, IL2CPP, installer, artifact upload를 만들지 않는다.

첫 build는 약 166 MB의 local output을 만들지만 `.gitignore` 대상이다. Unity/URP가 build 중 4개 tracked 설정 asset과 미추적 `SceneTemplateSettings.json`을 자동 직렬화했으며, build 결과를 확인한 뒤 작업 전 Git 내용으로 복원했다.

## Validation

- Exact Unity Editor: 6000.5.1f1
- Editor/Runtime/Test compilation: PASS
- EditMode regression: PASS, 16 passed / 0 failed / 0 skipped
- PlayMode regression/profile: PASS, 1 passed / 0 failed / 0 skipped
- Project validator command line: PASS
- Windows build: PASS, `Build Finished, Result: Success`, 64.697 seconds
- Output: PE32+ GUI x86-64, local directory 약 166 MB
- Executable SHA-256 for this local build: `6fade785e5b76fc6848554611ba9e78af06c522cc80e05e6a22087c603da04f7`
- Player launch smoke: PASS, 8 seconds alive then target process closed
- Player log: early exit, managed exception, crash 없음; D3D12 info queue와 shutdown cleanup 진단 존재
- Windows player full gameplay checklist: PASS, 사용자 확인 및 오류 없음
- First test invocation with an assumed `C:` Editor path: NOT RUN, executable missing; actual audited `D:` installation reused
- Scene, Prefab, Package, tracked ProjectSettings final changes: none
- Markdown links: PASS, missing 0
- ADR governance: PASS, 0001~0008 sequence와 필수 section 확인
- Glossary: PASS, version 0.5.0, 52 entries
- Remote branch SHA: PASS, implementation commit `fca8a38`가 `origin/work/day4-build-demo`와 일치
- Human evidence commit: PASS, `f87a0d5`가 `origin/work/day4-build-demo`와 일치
