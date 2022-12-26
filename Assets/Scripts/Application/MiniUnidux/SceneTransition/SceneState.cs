// original code is Unidux
using System;
using System.Collections.Generic;
using System.Linq;

namespace MiniUnidux.SceneTransition
{
    [Serializable]
    public class SceneState<TScene> : State, ICloneable where TScene : struct
    {
        public readonly IList<IPage<TScene>> Stack = new List<IPage<TScene>>();
        public readonly IDictionary<TScene, bool> ActiveMap = new Dictionary<TScene, bool>();

        public IPage<TScene> Current
        {
            get { return this.Stack.Count > 0 ? this.Stack.Last() : null; }
        }

        public string Name
        {
            get { return this.Stack.Count > 0 ? this.Current.Scene.ToString() : "-"; }
        }

        public ISceneData Data
        {
            get { return this.Current != null ? this.Current.Data : null; }
        }

        public TData GetData<TData>() where TData: ISceneData
        {
            return (TData) this.Current.Data;
        }

        public bool IsReady
        {
            get { return (this.Stack.Count > 0 && this.ActiveMap.Values.Any(enabled => enabled)); }
        }

        public SceneState()
        {
        }

        public SceneState(SceneState<TScene> state)
        {
            foreach (var page in state.Stack) {
                this.Stack.Add(page);
            }

            this.ActiveMap.Clear();

            foreach (var key in state.ActiveMap.Keys) {
                this.ActiveMap[key] = state.ActiveMap[key];
            }

            this.SetStateChanged(state.IsStateChanged);
        }

        public override object Clone()
        {
            return new SceneState<TScene>(this);
        }

        public IEnumerable<TScene> Removals(IEnumerable<TScene> activeScenes)
        {
            var removals = this.ActiveMap
                .Where(entry => !entry.Value)
                .Select(entry => entry.Key);
            return removals.Intersect<TScene>(activeScenes);
        }

        public IEnumerable<TScene> Additionals(IEnumerable<TScene> activeScenes)
        {
            var additionals = this.ActiveMap
                .Where(entry => entry.Value)
                .Select(entry => entry.Key);

            return additionals.Except(activeScenes);
        }

        public bool NeedsAdjust(
            IEnumerable<TScene> allPageScenes
        )
        {
            var required = new Dictionary<TScene, bool>();

            foreach (var scene in allPageScenes) {
                required[scene] = false;
            }

            foreach (var scene in allPageScenes) {
                if (required[scene] != this.ActiveMap[scene]) return true;
            }

            return false;
        }

        public override bool Equals(object obj)
        {
            if (obj is SceneState<TScene>) {
                var target = (SceneState<TScene>) obj;
                return this.ActiveMap.SequenceEqual(target.ActiveMap);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return this.ActiveMap.GetHashCode();
        }
    }
}