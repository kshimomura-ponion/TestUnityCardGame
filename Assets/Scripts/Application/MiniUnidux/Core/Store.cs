// https://tnegri.wordpress.com/2015/03/20/using-facebooks-flux-architectural-style-for-game-development-in-unity-3d/
using System;
using UnityEngine;
using UniRx;

namespace MiniUnidux
{
    public class Store<TState>: IStore<TState> where TState: State
    {
        private TState _state;
        private bool _changed;
        private Subject<TState> _subject;
        private readonly IReducer[] _matchers;

        public Subject<TState> Subject
        {
            get { return this._subject = this._subject ?? new Subject<TState>(); }
        }

        public TState State
        {
            get { return this._state; }
            set {
                this._changed = StateUtil.ApplyStateChanged(this._state, value);
                this._state = value;
            }
        }

        public object ObjectState
        {
            get { return this.State; }
            set { this.State = (TState) value; }
        }

        public Type StateType
        {
            get { return typeof(TState); }
        }

        public Store(TState state, params IReducer[] matchers)
        {
            this._state = state;
            this._changed = false;
            this._matchers = matchers ?? new IReducer[0];
        }

        public object Dispatch(object action)
        {
            foreach (var matcher in this._matchers) {
                if (matcher.IsMatchedAction(action)) {
                    this._state = (TState) matcher.ReduceAny(this.State, action);
                    this._changed = true;
                }
            }

            if (!this._changed) {
                Debug.LogWarning("'Store.Dispatch(" + action + ")' was failed. Maybe you forget to assign reducer.");
            }

            return null;
        }

        public void Update() {
            if (!this._changed) {
                return;
            }

            this._changed = false;
            TState fixedState;
            lock (this._state) {
                fixedState = (TState) this._state.Clone();

                StateUtil.ResetStateChanged(this._state);
            }
             this.Subject.OnNext(fixedState);
        }
    }
}
