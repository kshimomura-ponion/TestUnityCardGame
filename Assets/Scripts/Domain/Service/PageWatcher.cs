using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using MiniUnidux;
using MiniUnidux.SceneTransition;
using UnityEngine;
using UniRx;

namespace TestUnityCardGame.Domain.Service
{
    public sealed class PageWatcher : MonoBehaviour, IDisposable
    {
        readonly ISceneCategoryMap<SceneName> _categoryMap = new SceneCategoryMap();
        CompositeDisposable _disposable;

        void Start()
        {
            _disposable = new CompositeDisposable();

            // 何らかのステートが変更された時，シーンの更新処理を実行する
            MiniUniduxService.OnStateChangedAsObservable()
                .Where(state => state.Scene.IsReady)
                .Subscribe(UpdateScene)
                .AddTo(_disposable);

            // シーンの変更処理が終わったら
            MiniUniduxService.OnStateChangedAsObservable()
                .Subscribe(state => _ = ChangeScenes(state.Scene))
                .AddTo(_disposable);
        }

        void IDisposable.Dispose() => _disposable?.Dispose();

        // Update Scene
        void UpdateScene(StateEntity state)
        {
            if(state.Scene.NeedsAdjust(_categoryMap.GetPageScenes()))
            {
                MiniUniduxService.Dispatch(PageActionManager<SceneName>.ActionCreator.Adjust());
            }
        }


        /// シーン変更処理後
        /// 古いシーンをアンロード(減算ロード)して受け取った新しいシーンのステートに合ったシーンファイル群を加算ロードする
        async UniTaskVoid ChangeScenes(SceneState<SceneName> sceneState, CancellationToken token = default)
        {
            // シーンファイル群の減算処理
            foreach (var scene in sceneState.Removals(SceneUtil.GetActiveScenes<SceneName>()).OrderBy(x => (int)x))
            {
                await SceneUtil.RemoveScene($"{scene}", token);
            }

            // シーンファイル群の加算処理
            foreach (var scene in sceneState.Additionals(SceneUtil.GetActiveScenes<SceneName>()).OrderBy(x => (int)x))
            {
                await SceneUtil.AddScene($"{scene}", token);
            }
        }
    }
}