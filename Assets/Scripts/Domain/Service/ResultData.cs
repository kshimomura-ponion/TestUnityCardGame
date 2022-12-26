using System;
using MiniUnidux.SceneTransition;

namespace TestUnityCardGame.Domain.Service
{
    [Serializable]
    public class ResultData : ISceneData
    {
        public bool isPlayer1Win { get; set; }
        
        ResultData() {}

        public ResultData(bool isPlayer1Win)
        {
            this.isPlayer1Win = isPlayer1Win;
        }
    }
}
