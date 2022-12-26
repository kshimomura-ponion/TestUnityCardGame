// Original Code is PagaDuck.cs in the Unidux
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MiniUnidux.Util;

namespace MiniUnidux.SceneTransition
{
    public static class PageActionManager<TScene> where TScene : struct
    {
        public interface IPageAction
        {}

        [Serializable]
        public class PushAction : Page<TScene>, IPageAction
        {
            public PushAction(TScene scene, ISceneData data) : base(scene, data)
            {}
        }

        public class PopAction : IPageAction
        {}
        
        
        [Serializable]
        public class ReplaceAction : Page<TScene>, IPageAction
        {
            public ReplaceAction(TScene scene, ISceneData data) : base(scene, data)
            {}
        }

        public class ResetAction : IPageAction
        {}

        public class AdjustAction : IPageAction
        {}

        public class SetDataAction : IPageAction
        {
            public ISceneData Data { get; private set; }

            public SetDataAction(ISceneData data)
            {
                this.Data = data;
            }
        }

        public static class ActionCreator
        {
            public static PushAction Push(TScene scene, ISceneData data = null)
            {
                return new PushAction(scene, data);
            }

            public static PopAction Pop()
            {
                return new PopAction();
            }

            public static ReplaceAction Replace(TScene scene, ISceneData data = null)
            {
                return new ReplaceAction(scene, data);
            }

            public static ResetAction Reset()
            {
                return new ResetAction();
            }

            public static SetDataAction SetData(ISceneData data)
            {
                return new SetDataAction(data);
            }

            public static AdjustAction Adjust()
            {
                return new AdjustAction();
            }
        }
        public abstract class Reducer : IReducer
        {
            private ISceneCategoryMap<TScene> categoryMap;

            public Reducer(ISceneCategoryMap<TScene> categoryMap)
            {
                this.categoryMap = categoryMap;
            }

            public bool IsMatchedAction(object action)
            {
                return action is IPageAction;
            }

            public abstract object ReduceAny(object state, object action);

            public SceneState<TScene> Reduce(SceneState<TScene> sceneState, IPageAction action)
            {
                if (action is PushAction) {
                    return ReducePush(sceneState, (PushAction) action);
                } else if (action is PopAction) {
                    return ReducePop(sceneState, (PopAction) action);
                } else if (action is ResetAction) {
                    return ReduceReset(sceneState, (ResetAction) action);
                } else if (action is SetDataAction) {
                    return ReduceSetData(sceneState, (SetDataAction) action);
                } else if (action is ReplaceAction) {
                    return ReduceReplace(sceneState, (ReplaceAction)action);
                } else if (action is AdjustAction) {
                    return ReduceAdjust(sceneState);
                }

                return sceneState;
            }

            public SceneState<TScene> ReducePush(
                SceneState<TScene> sceneState,
                IPage<TScene> action
            )
            {
                if (!categoryMap.SceneCategories.ContainsKey(action.Scene)) {
                    Debug.LogWarning(
                        "Scene pushing failed. Missing categoryMapuration in ScenecategoryMap.SceneCategories: " + action.Scene);
                    return sceneState;
                }

                if (sceneState.ActiveMap.Any() && sceneState.Current.Scene.Equals(action.Scene)) {
                    Debug.LogWarning(
                        "Scene pushing failed. Cannot push same scene at once: " + action.Scene);
                    return sceneState;
                }

                sceneState.Stack.Add(action);
                sceneState.SetStateChanged();

                ReduceAdjust(sceneState);
                return sceneState;
            }

            public SceneState<TScene> ReducePop(
                SceneState<TScene> sceneState,
                PopAction action
            )
            {
                // don't remove last scene
                if (sceneState.ActiveMap.Count > 1) {
                    var lastKey = sceneState.ActiveMap.Keys.Last();
                    sceneState.ActiveMap.Remove(lastKey);
                }

                sceneState.SetStateChanged();

                ReduceAdjust(sceneState);
                return sceneState;
            }

            public SceneState<TScene> ReduceReplace(
                SceneState<TScene> sceneState,
                IPage<TScene> action
            )
            {
                // don't remove last page
                if (sceneState.Stack.Count > 1) {
                    sceneState.Stack.RemoveAt(sceneState.Stack.Count - 1);
                }

                ReducePush(sceneState, action);
                return sceneState;
            }

            public SceneState<TScene> ReduceReset(SceneState<TScene> sceneState,
                ResetAction action)
            {
                sceneState.Stack.Clear();
                sceneState.ActiveMap.Clear();
                sceneState.SetStateChanged();
                return sceneState;
            }

            public SceneState<TScene> ReduceSetData(SceneState<TScene> state, SetDataAction action)
            {
                if (!state.IsReady) {
                    Debug.LogWarning("PageActionManager.SetData is failed. Set some scene before you set scene data");
                    return state;
                }
                
                state.Current.Data = action.Data;
                state.SetStateChanged();
                return state;
            }

            public SceneState<TScene> ReduceAdjust(SceneState<TScene> sceneState)
            {
                Remove(sceneState, categoryMap.GetPageScenes());
                if (categoryMap.SceneCategories.Any()) {
                    var page = sceneState.Current;
                    
                    if (!categoryMap.SceneCategories.ContainsKey(page.Scene))
                    {
                        Debug.LogWarning(
                            "Page adjust failed. Missing categoryMapuration in ScenecategoryMap.SceneCategories: " + page);
                    }

                    Add(sceneState, page.Scene);
                }

                sceneState.SetStateChanged();
                return sceneState;
            }
            public static void ResetAll(SceneState<TScene> state)
            {
                Set(state.ActiveMap, EnumUtil.All<TScene>(), false);
            }

            public static void Add(SceneState<TScene> state, TScene scene)
            {
                state.ActiveMap[scene] = true;
            }

            public static void Remove(SceneState<TScene> state, IEnumerable<TScene> scenes)
            {
                Set(state.ActiveMap, scenes, false);
            }

            public static void Set(IDictionary<TScene, bool> activeMap, IEnumerable<TScene> scenes, bool value)
            {
                foreach (var scene in scenes) {
                    activeMap[scene] = value;
                }
            }
        }
    }
}