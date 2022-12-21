// https://tnegri.wordpress.com/2015/03/20/using-facebooks-flux-architectural-style-for-game-development-in-unity-3d/
// Unidux
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
//using Views;

namespace MiniUnidux{
    public delegate void StoreObserver();
    public interface IStore<TState> where TState: State
    {
        Subject<TState> Subject { get; }
        TState State { get; set; }
        void Update();

        object Dispatch(object action);
    }
}
