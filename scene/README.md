# GameJam Scene

シーン遷移モジュール。SceneLoader をシーンに置くだけで、どこからでも `ServiceLocator.Get<ISceneService>().LoadAsync("SceneName")` で遷移できる。

## 前提パッケージ（先に導入してください）

- **UniTask**: Package Manager → Add package from git URL
  `https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask`

- **DOTween**: Asset Store からインストール
