using System.Collections.Generic;
using System.Linq;
using MiniUnidux;
using MiniUnidux.SceneTransition;

namespace MiniUnidux.SceneTransition{

    public enum SceneCategory
    {
        Permanent,
        Page
    }
    
    public interface ISceneCategoryMap<TScene>
    {
        IDictionary<TScene, SceneCategory> SceneCategories { get; }
    }

    public static class ISceneCategoryMapExtension
    {
        public static IEnumerable<TScene> GetPageScenes<TScene>(
            this ISceneCategoryMap<TScene> categoryMap)
        {
            return categoryMap.SceneCategories.Where(entry => entry.Value == SceneCategory.Page)
                .Select(entry => entry.Key);
        }
    }
}
