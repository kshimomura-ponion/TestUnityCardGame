using System;
using MiniUnidux.SceneTransition;

namespace TestUnityCardGame
{
    [Serializable]
    public class ResultInitialData : ISceneData
    {
        public bool isPlayer1Win { get; set; }
        
        ResultInitialData() {}

        public ResultInitialData(bool isPlayer1Win)
        {
            this.isPlayer1Win = isPlayer1Win;
        }
    }
}
