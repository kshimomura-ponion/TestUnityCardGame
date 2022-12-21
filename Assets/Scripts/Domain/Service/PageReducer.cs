using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MiniUnidux;
using MiniUnidux.SceneTransition;

namespace TestUnityCardGame.Domain.Service
{
    // Page SceneのStateを変更する（継承禁止）
    public sealed class PageReducer : PageActionManager<SceneName>.Reducer
    {
        public PageReducer() : base(new SceneCategoryMap()) {}

        public override object ReduceAny(object state, object action)
        {
            var newState = (StateEntity) state;
            var newAction = (PageActionManager<SceneName>.IPageAction) action;
            newState.Scene = base.Reduce(newState.Scene, newAction);
            return state;
        }
    }
}
