using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MiniUnidux;
using MiniUnidux.SceneTransition;

namespace TestUnityCardGame.Domain.Service
{
    public class SceneCategoryMap : ISceneCategoryMap<SceneName>
    {
        // カテゴリーマップの設定
        public IDictionary<SceneName, SceneCategory> SceneCategories { get; } =
            new Dictionary<SceneName, SceneCategory>
        {
            // Permanent Scene 設定
            {SceneName.Common, SceneCategory.Permanent},

            // Title Scene 設定
            {SceneName.Title, SceneCategory.Page},

            // Hero Select Scene 設定
            {SceneName.HeroSelect, SceneCategory.Page},

            // Main Battle Scene 設定
            {SceneName.Battle, SceneCategory.Page},

            // Result Scene 設定
            {SceneName.Result, SceneCategory.Page}
        };
    }
}
