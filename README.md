# Switchboard

Switchboard는 Windows 11의 창 전환을 더 선명하고 안정적으로 보여 주는 데스크톱 오버레이입니다. 열려 있는 창을 읽기 쉬운 제목과 잘리지 않는 미리보기로 표시하고, 창 개수와 화면 크기에 맞춰 레이아웃을 자동으로 조절합니다.

**현재 버전:** `v0.1.1` · **배포 형식:** Windows 11 x64 Portable

> 설치 프로그램 없이 압축을 풀어 실행하는 포터블 프리뷰입니다. 코드 서명이 적용되지 않은 빌드는 Windows SmartScreen 경고가 표시될 수 있습니다.

## 주요 기능

- `Alt+Tab` 한 번으로 Switchboard 표시/숨김 전환
- 원본 전체가 보이는 DWM 창 미리보기
- 25개를 넘는 창도 표시하는 반응형 행·열 및 스크롤
- 격자, 압축, 목록 보기
- 최근 사용, 앱, 모니터, 제목, 즐겨찾기 정렬
- 투명/어두움/밝음 테마와 투명도·전체 배율·섬네일 크기 설정
- 사용자 지정 보조 단축키(기본값 `Ctrl+Alt+Space`)
- 항상 위에 표시 설정과 시스템 트레이 상주
- 각 창 카드에서 대상 창을 정상 종료하는 닫기 버튼
- 변경되지 않은 창 목록은 다시 그리지 않아 폴링 깜빡임 최소화

## 설치 및 실행

1. `Switchboard-v0.1.1-win-x64-Portable.zip`의 압축을 새 폴더에 풉니다.
2. `Switchboard.App.exe`를 실행합니다.
3. SmartScreen 경고가 나오면 출처를 확인한 뒤 **추가 정보 → 실행**을 선택합니다.
4. 종료하려면 작업 표시줄 알림 영역의 Switchboard 아이콘을 우클릭하고 **Exit**를 선택합니다.

자체 포함 배포본에는 .NET 런타임이 포함되므로 별도 설치가 필요하지 않습니다.

## 조작법

| 입력 | 동작 |
| --- | --- |
| `Alt+Tab` | 오버레이 표시/숨김 전환 |
| `Ctrl+Alt+Space` | 오버레이 표시(기본 보조 단축키) |
| `Tab` 또는 방향키 | 창 카드 선택 이동 |
| `Enter` | 선택한 창 활성화 |
| `Esc` | 오버레이 숨기기 |
| 마우스 더블 클릭 | 해당 창 활성화 |
| 창 카드의 `X` 버튼 | 해당 창에 정상 종료 요청 |

상단의 설정 버튼에서 섬네일 크기, 투명도, 크기 정책, 기본 보기, 전체 배율, 보조 단축키를 변경할 수 있습니다. 핀 버튼은 **항상 위에 표시**를 켜거나 끕니다.

창 닫기 버튼은 강제 종료가 아닌 Windows 표준 종료 요청을 사용합니다. 저장하지 않은 작업이 있으면 대상 앱의 저장 확인창이 그대로 표시될 수 있습니다.

## 설정 파일

설정은 다음 위치에 자동 저장됩니다.

```text
%APPDATA%\Switchboard\settings.json
```

기본값으로 초기화하려면 Switchboard를 종료한 뒤 위 파일을 삭제하고 다시 실행하세요.

## 알려진 제한 사항

- 관리자 권한 창과 보안 데스크톱에서는 Windows의 포커스 제한 때문에 창 활성화가 실패할 수 있습니다.
- 현재 배포 파일에는 디지털 코드 서명이 없어 SmartScreen 경고가 표시될 수 있습니다.
- V1에서는 가상 데스크톱 관리, 창 자동 배치, 타임라인 기능을 제공하지 않습니다.

## 개발 환경

- Windows 11
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- WPF / Win32 / DWM

```powershell
git clone https://github.com/ai-blink/alt-tab.git
cd alt-tab
dotnet build Switchboard.slnx --nologo
dotnet test Switchboard.slnx --nologo
dotnet run --project src/Switchboard.App/Switchboard.App.csproj
```

## 배포 빌드

Windows x64 자체 포함 단일 실행 파일을 만들려면 다음 명령을 사용합니다.

```powershell
dotnet publish src/Switchboard.App/Switchboard.App.csproj `
  -c Release `
  -r win-x64 `
  --self-contained true `
  --nologo `
  -o artifacts/release/win-x64 `
  -p:PublishSingleFile=true `
  -p:IncludeNativeLibrariesForSelfExtract=true `
  -p:PublishTrimmed=false `
  -p:DebugType=None `
  -p:DebugSymbols=false
```

배포 ZIP 이름은 `Switchboard-v{버전}-win-x64-Portable.zip` 형식을 사용합니다. 버전별 변경 사항은 [CHANGELOG.md](CHANGELOG.md)에서 확인할 수 있습니다.

## 프로젝트 구조

- `src/Switchboard.App`: WPF 셸, 화면, ViewModel, 사용자 설정
- `src/Switchboard.Core`: 창 모델, 필터링, 정렬, 레이아웃 계산
- `src/Switchboard.Native`: Win32/DWM 창 열거, 단축키, 전경 활성화
- `tests/Switchboard.Tests`: Core 및 입력·갱신 동작 테스트

문제 제보는 [GitHub Issues](https://github.com/ai-blink/alt-tab/issues)에 재현 절차와 Windows 버전을 함께 남겨 주세요.
