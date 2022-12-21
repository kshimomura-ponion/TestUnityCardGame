using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiniUnidux.SceneTransition
{
    public interface ISceneData {}

    public interface IPage<TScene> where TScene : struct
    {
        TScene Scene { get; }
        ISceneData Data { get; set; }
    }
}
