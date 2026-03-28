# GameJam Scene

トランジション付きシーン遷移モジュール。
SceneLoader をシーンに置くだけで、どこからでも1行で遷移できる。

## 前提パッケージ（先に導入すること）

| パッケージ | 導入方法 |
|-----------|---------|
| UniTask | Package Manager → Add package from git URL:<br>`https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask` |
| DOTween | Asset Store からインストール → Tools > Demigiant > DOTween Utility Panel で Setup |
| core | `https://github.com/daiMaruyama/gamejam-framework.git?path=core` |

## 最短セットアップ

```
SceneLoader (空の GameObject)
└── Canvas
    └── Transition (空の GameObject・RectTransform を Stretch に)
        ← ここに好きなトランジションをアタッチ
```

1. 空の GameObject → `SceneLoader` をアタッチ
2. 子に Canvas → さらに子に空の GameObject（RectTransform を画面全体に引き伸ばす）
3. 好きなトランジションをアタッチ（子要素は自動生成される）
4. SceneLoader の `Default Transition Component` にドラッグ
5. 遷移先シーンを File > Build Settings に追加

## トランジション一覧

すべてアタッチするだけで動作。色・速度・イージングは Inspector で共通設定。

| 名前 | 演出 | 固有設定 |
|------|------|---------|
| `FadeTransition` | フェードイン/アウト | — |
| `WipeTransition` | 左からワイプ | — |
| `CircleTransition` | 円形に閉じる/開く | — |
| `StripeTransition` | 短冊が時間差で波状にスケールイン | stripeCount, stagger |
| `GridTransition` | タイルが中心から外側へポップイン | columns, rows, stagger |
| `DoorTransition` | 左右パネルがドアのように開閉 | — |

### 共通 Inspector 項目（TransitionBase）

| 項目 | 説明 | デフォルト |
|------|------|-----------|
| Color | 覆う色 | 黒 |
| Duration | 演出時間（秒） | 0.4 |
| Ease In | Play 時のイージング | OutQuad |
| Ease Out | Release 時のイージング | InQuad |

## 使い方

```csharp
using GameJamCore;
using GameJamScene;

// デフォルトトランジションで遷移
await ServiceLocator.Get<ISceneService>().LoadAsync("GameScene");

// トランジションなしで即遷移
await ServiceLocator.Get<ISceneService>().LoadAsync("GameScene", null);

// 特定のトランジションを指定して遷移
await ServiceLocator.Get<ISceneService>().LoadAsync("GameScene", wipeTransition);
```

遷移中に LoadAsync を呼んでも無視されるので、ボタン連打で壊れることはない。

SceneLoader は Awake で `DontDestroyOnLoad` + ServiceLocator に自己登録される。
重複は自動で破棄される。

## 独自トランジションを作る

`TransitionBase` を継承すると、共通項目（色・速度・イージング）が自動で使える。

```csharp
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class MyTransition : TransitionBase
{
    public override async UniTask Play()
    {
        // _color, _duration, _easeIn が使える
    }

    public override async UniTask Release()
    {
        // _color, _duration, _easeOut が使える
    }
}
```
