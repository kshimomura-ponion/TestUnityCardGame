using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MiniUnidux;
using MiniUnidux.Util;
using UniRx;

namespace TestUnityCardGame.Domain.Service
{
    public class MiniUniduxService : SingletonMonoBehaviour<MiniUniduxService> 
    {
        // Get Store Instance
        Store<StateEntity> _store;
        static Store<StateEntity> Store => _instance._store ??= new Store<StateEntity>(InitialState, Reducers);

        // Get Reducer
        static IReducer[] Reducers => new IReducer[] { new PageReducer() };

        // Initial State(default)
        static StateEntity InitialState => new();

        // Get State in Store
        public static StateEntity State => Store.State;

        // ステートのIObservable
        public static IObservable<StateEntity> OnStateChangedAsObservable()
            => Store.Subject.Where(state => state.Scene.IsStateChanged);

        // Dispatch Object
        public static object Dispatch<TAction>(TAction action) => Store.Dispatch(action);

        // Updating
        void Update() => Store.Update();

    }
}