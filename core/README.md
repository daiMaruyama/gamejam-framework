# GameJam Core

全モジュールの土台となる ServiceLocator を提供する。

## 導入

Package Manager → Add package from git URL:

```text
https://github.com/daiMaruyama/gamejam-framework.git?path=core
```

## ServiceLocator とは

型ベースのサービス電話帳。
各サービスが自分から電話帳に名前を登録し、使う側は「やりたいこと」で引くだけ。

- 実装クラスの名前を知らなくていい
- シーン上のどこにいるか気にしなくていい
- Find 系の走査コストがない（内部は Dictionary）

## API

### Register\<T\>(T service)

サービスを登録する。同じ型で再登録すると上書きされる。

```csharp
ServiceLocator.Register<IScoreService>(this);
```

### Get\<T\>()

サービスを取得する。未登録の場合は `InvalidOperationException` を投げる。

```csharp
var scene = ServiceLocator.Get<ISceneService>();
```

### TryGet\<T\>(out T service)

サービスの取得を試みる。未登録の場合は false を返す。

```csharp
if (ServiceLocator.TryGet<IScoreService>(out var score))
{
    score.Add(100);
}
```

### Unregister\<T\>()

サービスを電話帳から削除する。

```csharp
ServiceLocator.Unregister<IScoreService>();
```

## 使い方

### 1. インターフェースを定義する

```csharp
public interface IScoreService
{
    int CurrentScore { get; }
    void Add(int points);
    void Reset();
}
```

### 2. 実装クラスを作り、Awake で自己登録する

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

    private void OnDestroy()
    {
        ServiceLocator.Unregister<IScoreService>();
    }

    public void Add(int points) { _score += points; }
    public void Reset() { _score = 0; }
}
```

### 3. どこからでも使う

```csharp
ServiceLocator.Get<IScoreService>().Add(100);
```
