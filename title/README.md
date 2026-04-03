# GameJam Title

タイトル画面の基盤モジュール。
`TitleManager` をアタッチするだけでアイドルタイマー・オープニング演出が動く。
スタート入力時は `UnityEvent` を発火するので、シーン遷移などは外部から接続する。

## 導入

Package Manager → Add package from git URL:

```text
https://github.com/daiMaruyama/gamejam-framework.git?path=title
```

## 前提パッケージ（先に導入すること）

| パッケージ | 導入方法 |
|-----------|---------|
| [UniTask](https://github.com/Cysharp/UniTask) | Package Manager → Add package from git URL → リポジトリの README 参照 |
| [DOTween](https://dotween.demigiant.com) | Asset Store からインストール → Tools > Demigiant > DOTween Utility Panel で Setup |

## セットアップ

1. タイトルシーンの GameObject に `TitleManager` をアタッチ
2. Inspector で各項目を設定
3. オープニング演出用の CanvasGroup を用意してアサイン（省略可）
4. On Start Requested にシーン遷移などのメソッドを接続

## Inspector 項目

| 項目 | 説明 | デフォルト |
|------|------|-----------|
| Idle Time Limit | アイドルと判定するまでの時間（秒） | 10 |
| Opening Canvas Group | オープニング演出の CanvasGroup（省略可） | — |
| Fade Duration | フェードイン/アウトの時間（秒） | 0.5 |
| On Start Requested | スタート入力時に呼ぶイベント | — |

## 動作

| 状態 | 操作 | 結果 |
|------|------|------|
| 待機中 | 何もしない | 一定時間後にオープニング演出フェードイン |
| 待機中 | キー入力 | On Start Requested を発火 |
| オープニング再生中 | キー入力 | オープニングをフェードアウトして待機状態に戻る |
