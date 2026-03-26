# gamejam-framework

ゲームジャム用の軽量フレームワーク。
必要なモジュールだけ個別に導入できます。

## モジュール一覧

| モジュール | 説明 | 依存 |
|-----------|------|------|
| core | ServiceLocator（全モジュールの土台） | なし |
| audio | 音声管理（今後追加） | core |
| scene | シーン遷移管理（今後追加） | core |

## 導入方法

Unity の Package Manager → Add package from git URL に以下を入力:

### core（必須）
```
https://github.com/daiMaruyama/gamejam-framework.git?path=core
```

### audio（任意）
```
https://github.com/daiMaruyama/gamejam-framework.git?path=audio
```

### scene（任意）
```
https://github.com/daiMaruyama/gamejam-framework.git?path=scene
```

## 基本的な使い方

### 1. インターフェースを定義
```csharp
public interface IAudioService
{
    void PlaySE(string name);
    void PlayBGM(string name);
    void StopBGM();
}
```

### 2. 実装を作る
```csharp
public class AudioManager : MonoBehaviour, IAudioService
{
    public void PlaySE(string name) { /* 実装 */ }
    public void PlayBGM(string name) { /* 実装 */ }
    public void StopBGM() { /* 実装 */ }
}
```

### 3. 起動時に登録
```csharp
using GameJamCore;

public class GameInitializer : MonoBehaviour
{
    [SerializeField] AudioManager audioManager;

    void Awake()
    {
        ServiceLocator.Register<IAudioService>(audioManager);
    }
}
```

### 4. どこからでも使う
```csharp
using GameJamCore;

ServiceLocator.Get<IAudioService>().PlaySE("bang");
```

## 設計方針

- Service Locatorパターンで共通機能への統一的なアクセスを提供
- インターフェース経由で実装を差し替え可能
- モジュール間の依存は常にcoreへの一方通行
- ゲームジャムの短期間で導入・理解できるシンプルさを維持
