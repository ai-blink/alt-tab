# Switchboard

[English](README.md) | [한국어](README.ko.md) | [中文](README.zh-CN.md) | [日本語](README.ja.md)

Switchboard は、Windows 11 でのウィンドウ切り替えを、より見やすく安定させるデスクトップオーバーレイです。開いているウィンドウを読みやすいタイトルと切り取られないプレビューで表示し、ウィンドウ数と利用可能な画面領域に合わせてレイアウトを調整します。

**現在のバージョン:** [v0.3.1](https://github.com/ai-blink/alt-tab/releases/tag/v0.3.1) · **配布形式:** Windows 11 x64 Portable

> インストーラーを使わず、アーカイブを展開して実行するポータブルプレビュー版です。コード署名されていないビルドでは Windows SmartScreen の警告が表示される場合があります。

## 機能

- Alt+Tab を一度押して Switchboard の表示と非表示を切り替え
- 元のウィンドウ全体を表示する DWM プレビュー
- 25 個を超えるウィンドウにも対応するレスポンシブな行・列レイアウトとスクロール
- グリッド、コンパクト、リストの各ビュー
- 空白領域のドラッグで保存されるオーバーレイ位置、9 方向の一時位置リモコン、保存位置への復帰
- 最近使用、アプリ、モニター、タイトル、お気に入りでの並べ替え
- 透明、ダーク、ライトのテーマ、不透明度、全体 UI スケール（60/70/80/100/125/150/200%）、サムネイルサイズ設定
- カスタム補助ホットキー（既定: Ctrl+Alt+Space）
- 常に手前に表示するオプション、システムトレイ常駐、ウィンドウ切り替えを示す専用アプリアイコン
- 各ウィンドウカードから通常の終了を要求する閉じるボタン
- ウィンドウ一覧が変化した場合だけ更新し、ポーリング時のちらつきを抑制

## インストールと実行

1. Switchboard-v0.3.1-win-x64-Portable.zip を新しいフォルダーに展開します。
2. Switchboard.App.exe を実行します。
3. SmartScreen が表示された場合は、配布元を確認してから **詳細情報 → 実行** を選択します。
4. 終了するには、タスクバーの通知領域にある Switchboard アイコンを右クリックし、**Exit** を選択します。

自己完結型の配布物には .NET ランタイムが含まれているため、別途ランタイムをインストールする必要はありません。

## 操作

| 入力 | 動作 |
| --- | --- |
| Alt+Tab | オーバーレイの表示と非表示を切り替え |
| Ctrl+Alt+Space | オーバーレイを表示（既定の補助ホットキー） |
| Tab または矢印キー | ウィンドウカードの選択を移動 |
| Enter | 選択したウィンドウをアクティブ化 |
| Esc | オーバーレイを非表示 |
| マウスのダブルクリック | そのウィンドウをアクティブ化 |
| ウィンドウカードの X ボタン | そのウィンドウに通常の終了を要求 |

上部の設定ボタンは、別個のモーダルウィンドウを開きます。左側のタブでは、位置と移動、外観とサイズ、既定の動作を分けて設定できます。オーバーレイの空白領域をドラッグすると新しい位置が自動保存され、位置リモコンは保存済みの位置を変更せずに一度だけ移動します。全体 UI スケールは文字、ボタン、余白、ウィンドウサイズをまとめて調整し、コンパクトモードで追加の縮小は行いません。ピンボタンは **常に手前に表示** を切り替えます。

閉じるボタンは強制終了ではなく、Windows 標準の終了要求を送信します。対象アプリに未保存の作業がある場合は、通常の保存確認ダイアログが表示されることがあります。

## 設定ファイル

設定は次の場所に自動保存されます。

~~~
%APPDATA%\Switchboard\settings.json
~~~

既定値に戻すには、Switchboard を終了し、このファイルを削除してから再起動してください。

## 既知の制限

- Windows のフォーカス制限により、管理者権限のウィンドウやセキュアデスクトップ上のウィンドウをアクティブ化できない場合があります。
- 現在の配布物にはデジタルコード署名がないため、SmartScreen が表示される場合があります。
- 現在、Alt+Tab フックを無効にする個別設定はありません。標準の Windows Alt+Tab を使うには Switchboard を終了してください。
- V1 には仮想デスクトップ管理、自動ウィンドウ配置、タイムライン機能は含まれません。

## 開発環境

- Windows 11
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- WPF / Win32 / DWM

~~~powershell
git clone https://github.com/ai-blink/alt-tab.git
cd alt-tab
dotnet build Switchboard.slnx --nologo
dotnet test Switchboard.slnx --nologo
dotnet run --project src/Switchboard.App/Switchboard.App.csproj
~~~

## リリースビルド

Windows x64 用の自己完結型単一ファイル実行可能ファイルを作成するには、次のコマンドを使います。

~~~powershell
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
~~~

リリース ZIP は Switchboard-v{version}-win-x64-Portable.zip の命名規則を使用します。バージョンごとの変更は [CHANGELOG.md](CHANGELOG.md) を参照してください。

## プロジェクト文書

| 文書 | 目的 |
| --- | --- |
| [CLAUDE.md](CLAUDE.md) | プロジェクトの作業規則と標準コマンドに関する唯一の正本エントリポイント |
| [現在のコンテキスト](rules/dev-context.md) · [進捗](rules/dev-progress.md) · [ロードマップ](rules/dev-roadmap.md) | 現在のスライス、検証状態、次の作業 |
| [アーキテクチャ](rules/dev-arch.md) · [UX 原則](rules/dev-ux.md) · [決定](rules/dev-decisions.md) | 継続して適用する実装境界とプロダクト判断 |
| [計画](notes/plans/) · [実行記録](notes/runs/) · [Stitch 参考資料](references/stitch/) | 過去の計画と検証根拠。現在状態の正本ではない |

## プロジェクト構成

- src/Switchboard.App: WPF シェル、ビュー、ViewModel、ユーザー設定
- src/Switchboard.Core: ウィンドウモデル、フィルタリング、並べ替え、レイアウト計算
- src/Switchboard.Native: Win32/DWM のウィンドウ列挙、ホットキー、前面化
- tests/Switchboard.Tests: Core、入力、更新動作のテスト

不具合は、再現手順と Windows バージョンを添えて [GitHub Issues](https://github.com/ai-blink/alt-tab/issues) に報告してください。
