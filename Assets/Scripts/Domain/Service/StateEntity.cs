using System;
using MiniUnidux;
using MiniUnidux.SceneTransition;

namespace TestUnityCardGame.Domain.Service
{
    // State Entity
    [Serializable]
    public class StateEntity : State
    {
            public SceneState<SceneName> Scene { get; set; } = new();
    }
}
