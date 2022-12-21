// https://tnegri.wordpress.com/2015/03/20/using-facebooks-flux-architectural-style-for-game-development-in-unity-3d/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiniUnidux
{
    public interface IDispatcher
    {
        public void Dispatch(IAction action);
        public void WaitFor(int registrationKey);
    }
}
