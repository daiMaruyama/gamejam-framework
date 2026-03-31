# GameJam Audio

BGM・SE 再生サービスモジュール。
AudioManager をシーンに置くだけで、どこからでも1行で BGM/SE を制御できる。
音量スライダーコンポーネント付き（PlayerPrefs 自動保存）。

## 導入

Package Manager → Add package from git URL:

```text
https://github.com/daiMaruyama/gamejam-framework.git?path=audio
```

## 前提パッケージ（先に導入すること）

| パッケージ | 導入方法 |
|-----------|---------|
| [UniTask](https://github.com/Cysharp/UniTask) | Package Manager → Add package from git URL → リポジトリの README 参照 |
| [DOTween](https://dotween.demigiant.com) | Asset Store からインストール → Tools > Demigiant > DOTween Utility Panel で Setup |
| core | Package Manager → Add package from git URL:<br>`https://github.com/daiMaruyama/gamejam-framework.git?path=core` |

## 最短セットアップ

1. 空の GameObject → `AudioManager` をアタッチ
2. BGMData・SEData アセットを作成（Create > GameJam > Audio > BGMData / SEData）
3. AudioClip を Inspector にアサイン

## 使い方

```csharp
using GameJamCore;
using GameJamAudio;

// BGM 再生（フェードイン）
await ServiceLocator.Get<IAudioService>().PlayBGMAsync(bgmData);

// BGM 停止（フェードアウト）
await ServiceLocator.Get<IAudioService>().StopBGMAsync();

// SE 再生
ServiceLocator.Get<IAudioService>().PlaySE(seData);

// 音量変更（0〜1）
ServiceLocator.Get<IAudioService>().SetBGMVolume(0.8f);
ServiceLocator.Get<IAudioService>().SetSEVolume(0.5f);
```

## 音量スライダー

`BGMVolumeSlider` / `SEVolumeSlider` をアタッチして fillAmount 式の Image をアサインするだけで動作する。

```
Canvas
└── SliderRoot (BGMVolumeSlider または SEVolumeSlider をアタッチ)
    └── FillImage (Image・fillMethod: Horizontal)
        ← Fill Image に アサイン
```

- 起動時に PlayerPrefs から音量を自動ロードして fillAmount に反映
- クリック・ドラッグで音量操作
- fillAmount が DOTween でスムーズにアニメーション
- 音量変更は PlayerPrefs に自動保存

## Inspector 項目

### AudioManager

| 項目 | 説明 | デフォルト |
|------|------|-----------|
| BGM Fade Duration | BGM のフェード時間（秒） | 0.5 |
| SE Pool Size | SE 用 AudioSource のプール数 | 8 |

### BGMVolumeSlider / SEVolumeSlider

| 項目 | 説明 | デフォルト |
|------|------|-----------|
| Fill Image | fillAmount を操作する Image | — |
| Anim Duration | 音量変化アニメーションの時間（秒） | 0.1 |

## データアセット

### BGMData

| 項目 | 説明 |
|------|------|
| Clip | 再生する AudioClip |
| Volume | 音量（0〜1） |
| Loop | ループ再生するか |

### SEData

| 項目 | 説明 |
|------|------|
| Clip | 再生する AudioClip |
| Volume | 音量（0〜1） |

AudioManager は Awake で `DontDestroyOnLoad` + ServiceLocator に自己登録される。
重複は自動で破棄される。
