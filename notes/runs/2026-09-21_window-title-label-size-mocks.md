# 창 제목 크기 목업 기록

- 현황: `MainWindow.xaml`은 창 제목을 격자 11pt, 압축 10pt, 목록 12pt로 각각 표시한다. 제목만 별도로 조절하는 사용자 설정값은 없다.
- 목업: `notes/mocks/2026-09-21_window-title-label-size/window-title-label-size-mocks.html`에 “모양과 크기 > 창 제목 크기” 슬라이더와 80/100/125/150% 빠른 선택을 추가했다. 선택값은 각 보기의 현재 기본 제목 크기에 동일한 비율로 적용되며, 격자/압축/목록 미리 보기에서 즉시 확인할 수 있다.
- 구현: 승인된 계약을 `UserSettings`, `MainWindowViewModel`, `SettingsWindow.xaml`, `MainWindow.xaml` 및 회귀 테스트에 반영했다. 새 JSON 설정값은 `WindowTitleScalePreset`이며 기본값은 100%다. 후속 요청에 따라 앞쪽 앱 라벨도 같은 배율로 연동했다.
- 검증: 실행 중이던 `Switchboard.App` 프로세스를 종료한 뒤 `dotnet build Switchboard.slnx --nologo`와 `dotnet test Switchboard.slnx --nologo`를 실행해, 표준 실행본 교체 및 66개 테스트 전체 통과를 확인했다. `git diff --check`도 통과했다. 새 빌드 앱 창의 실행은 확인했으며, 이 환경의 Windows UI 자동화 표면에서 네이티브 앱 창을 열거하지 못해 설정창 클릭 기반 화면 검증은 `NEEDS_USER_UI_CHECK`다.
