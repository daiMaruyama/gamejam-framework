# gamejam-framework

ゲームジャム用フレームワーク。
必要なモジュールだけ Git URL で個別に導入できる。

## ServiceLocator とは

全モジュールの土台となる**サービスの電話帳**。

各モジュール（SceneLoader 等）はゲーム起動時に自分から電話帳に名前を載せる。
使う側は「シーン遷移お願いします」と電話帳を引くだけで、誰が担当しているか・どこにいるかを知らなくていい。

```csharp
// 使う側はこれだけ。実装クラスの名前もシーン上の場所も知らなくてOK
ServiceLocator.Get<ISceneService>().LoadAsync("Title");
```

内部は `Dictionary<Type, object>` なので Find 系の走査コストもない。

## モジュール一覧

| モジュール | 説明 | 前提 |
|-----------|------|------|
| core | ServiceLocator（電話帳本体） | なし |
| scene | トランジション付きシーン遷移 | core, UniTask, DOTween |
| audio | BGM・SE 再生（フェードイン/アウト、SE プール） | core, UniTask, DOTween |

## 導入手順

### 1. 前提パッケージを入れる

| パッケージ | 導入方法 |
|-----------|---------|
| [UniTask](https://github.com/Cysharp/UniTask) | Package Manager → Add package from git URL → リポジトリの README 参照 |
| [DOTween](https://dotween.demigiant.com) | Asset Store からインストール → Tools > Demigiant > DOTween Utility Panel で Setup |

### 2. フレームワークを入れる

Package Manager → Add package from git URL に**上から順に**入力:

```text
https://github.com/daiMaruyama/gamejam-framework.git?path=core
```

使うモジュールを追加（scene・audio など）:

```text
https://github.com/daiMaruyama/gamejam-framework.git?path=scene
```

```text
https://github.com/daiMaruyama/gamejam-framework.git?path=audio
```

> core は全モジュールの依存先なので必ず最初に入れること。

## 使い方

### scene モジュール

詳しくは [scene/README.md](scene/README.md) を参照。

**セットアップ（1回だけ）:**
1. 空の GameObject を作り `SceneLoader` をアタッチ
2. 子に Canvas + CanvasGroup（画面全体を覆う黒い Image 等）を作り `FadeTransition` をアタッチ
3. SceneLoader の `Default Transition Component` に FadeTransition をドラッグ

置くだけで ServiceLocator に自己登録される。初期化スクリプトは不要。

**コードから遷移:**
```csharp
using GameJamCore;
using GameJamScene;

await ServiceLocator.Get<ISceneService>().LoadAsync("GameScene");
```

### core を単体で使う（自分でサービスを作る場合）

フレームワークに含まれないゲーム固有の機能も、同じ仕組みで管理できる。

**1. インターフェースを定義する**
```csharp
public interface IScoreService
{
    int CurrentScore { get; }
    void Add(int points);
    void Reset();
}
```

**2. 実装クラスを作り、Awake で自己登録する**
```csharp
using GameJamCore;
using UnityEngine;

public class ScoreManager : MonoBehaviour, IScoreService
{
    private int _score;

    public int CurrentScore => _score;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        ServiceLocator.Register<IScoreService>(this);
    }

    public void Add(int points) { _score += points; }
    public void Reset() { _score = 0; }
}
```

**3. どこからでも使う**
```csharp
ServiceLocator.Get<IScoreService>().Add(100);
```

## 設計方針

- **置くだけで動く** — 各サービスが Awake で自己登録するので、初期化スクリプトやシーン間の参照管理が不要
- **呼び出し側はインターフェースだけ知っていればいい** — 実装クラスの名前も場所も気にしなくていい
- **演出の差し替えが Inspector だけでできる** — TransitionBase を継承した別コンポーネントに差し替えるだけ。コード変更不要
- **モジュール間の依存は常に core への一方通行** — モジュール同士は干渉しない
