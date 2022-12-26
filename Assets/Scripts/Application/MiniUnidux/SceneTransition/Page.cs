using System;
using MiniUnidux;
using MiniUnidux.SceneTransition;

// 状態を持つ項目の設定
namespace MiniUnidux.SceneTransition
{
    [Serializable]
    public class Page<TScene> : IPage<TScene>, ICloneable where TScene : struct
    {
        public TScene Scene { get; private set; }
        public ISceneData Data { get; set; }

        public Page(TScene scene, ISceneData data)
        {
            this.Scene = scene;
            this.Data = data;
        }

        public Page(Page<TScene> page)
        {
            this.Scene = page.Scene;
            this.Data = page.Data;
        }

        public object Clone()
        {
            return new Page<TScene>(this);
        }

        public override bool Equals(object obj)
        {
            if (obj is Page<TScene>) {
                var target = (Page<TScene>) obj;
                return this.Scene.Equals(target.Scene) &&
                        (this.Data == null && target.Data == null || this.Data != null && this.Data.Equals(target.Data));
            }

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
