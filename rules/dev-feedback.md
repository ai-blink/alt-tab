---
description: 반복 실수 패턴 + 프로젝트 운영 원칙. Stop hook dev-docs-enforcer 필수 파일.
updated: 2026-09-27
---

# dev-feedback

## 운영 원칙
- 사용자 프로파일: 한국어 응답, 방식 결정은 옵션 제시 후 선택, 커밋은 Conventional Commits(`feat(settings): ...`).
- 선행 미커밋 작업이 같은 파일을 건드리면 빌드·테스트 확인 후 **별도 커밋하고** 새 슬라이스를 시작한다(2026-09-27 사용자 선택).
- 라이브 오버레이·설정창 화면은 에이전트가 Windows UI 자동화로 열거하지 못한다 → 빌드·테스트·프로세스 생존까지만 확인하고 화면은 `NEEDS_USER_UI_CHECK`로 남긴다.

## 재발 방지 패턴 (recurring-mistakes)
- **인계 파일 수치를 그대로 믿지 말 것**: "XAML 47줄/16줄"은 파일 길이가 아니라 한글 포함 줄 수였고, "`.cs` 한글 0건"은 `MainWindowViewModel.cs`의 위치 라벨 9개를 놓쳤다. 착수 전 `grep`으로 대조한다.
- **실행 중인 `Switchboard.App.exe`가 `bin/Debug` 출력을 잠근다**: 빌드 전 프로세스를 종료하고, 끝나면 새 빌드로 다시 실행한다.
- **같은 파일에 LF/CRLF가 섞여 있다**(예: `MainWindowViewModel.cs` — 창 제목 크기 커밋분만 LF). 스크립트로 치환할 때는 두 줄바꿈을 모두 허용하고 `newline=''`로 읽고 쓴다.
- **`{DynamicResource}` 키 검사 테스트는 문자열 키 접두어(`Main.`·`Settings.`·`Position.`·`Appearance.`·`Behavior.`)로 한정**한다. 스타일 키(`WindowCardItemStyle`)가 오탐으로 잡힌다.
