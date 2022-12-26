// Original Code is from UniduxSTSample
using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace MiniUnidux.SceneTransition
{
    public static class SceneUtil
    {
        // Get Active Scene
        public static IEnumerable<TScene> GetActiveScenes<TScene>() where TScene : struct
        {
            var sceneCount = SceneManager.sceneCount;

            for (var i = 0; i < sceneCount; i++) {
                var scene = SceneManager.GetSceneAt(i);
                var enumScene = (TScene)Enum.Parse(typeof(TScene), scene.name);
                yield return enumScene;
            }
        }

        // Load Scene File
        public static async UniTask AddScene(string name, CancellationToken token)
        {
            if (!SceneManager.GetSceneByName(name).isLoaded && !IsAlreadyLoadedScene(name)) {
                await SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive).WithCancellation(token);
            }
        }

        // Unload Scene File
        public static async UniTask RemoveScene(string name, CancellationToken token)
        {
            if (SceneManager.GetSceneByName(name).isLoaded) {
                await SceneManager.UnloadSceneAsync(name).WithCancellation(token);
            }
        }

        // Is Already-Loaded Screen?
        static bool IsAlreadyLoadedScene(string name)
        {
            var sceneCount = SceneManager.sceneCount;
            for (var i = 0; i < sceneCount; i++) {
                if (SceneManager.GetSceneAt(i).name == name)
                {
                    return true;
                }
            }
            return false;
        }
    }
}