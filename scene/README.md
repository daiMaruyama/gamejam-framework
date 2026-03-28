# GameJam Scene

トランジション付きシーン遷移モジュール。
SceneLoader をシーンに置くだけで、どこからでも1行で遷移できる。

## 前提パッケージ（先に導入すること）

| パッケージ | 導入方法 |
|-----------|---------|
| UniTask | Package Manager → Add package from git URL:<br>`https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask` |
| DOTween | Asset Store からインストール → Tools > Demigiant > DOTween Utility Panel で Setup |
| core | `https://github.com/daiMaruyama/gamejam-framework.git?path=core` |

## セットアップ

1. 空の GameObject を作り `SceneLoader` をアタッチ
2. 子に Canvas + CanvasGroup（画面全体を覆う黒い Image 等）を作り `FadeTransition` をアタッチ
3. SceneLoader の `Default Transition Component` に FadeTransition をドラッグ
4. 遷移先のシーンを File > Build Settings に追加

SceneLoader は Awake で `DontDestroyOnLoad` + ServiceLocator に自己登録される。
同じシーンや別シーンに2つ目の SceneLoader があっても、重複は自動で破棄される。

## 使い方

```csharp
using GameJamCore;
using GameJamScene;

// デフォルトトランジション（Inspector 設定の FadeTransition）で遷移
await ServiceLocator.Get<ISceneService>().LoadAsync("GameScene");

// トランジションなしで即遷移
await ServiceLocator.Get<ISceneService>().LoadAsync("GameScene", null);

// 特定のトランジションを指定して遷移
await ServiceLocator.Get<ISceneService>().LoadAsync("GameScene", wipeTransition);
```

遷移中に LoadAsync を呼んでも無視されるので、ボタン連打で壊れることはない。

## 独自トランジションを作る

`TransitionBase` を継承した MonoBehaviour を作り、SceneLoader にアタッチするか LoadAsync の第2引数に渡す。

```csharp
using Cysharp.Threading.Tasks;

public class MyTransition : TransitionBase
{
    public override async UniTask Play()
    {
        // 画面を覆う演出
    }

    public override async UniTask Release()
    {
        // 画面を開く演出
    }
}
```

Play() でシーン読み込み前の演出、Release() で読み込み後の演出を行う。
